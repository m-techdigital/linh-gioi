#!/usr/bin/env python3
from __future__ import annotations
import argparse
import socket
import struct
import sys

MAX_FRAME = 64 * 1024


def varint(value: int) -> bytes:
    out = bytearray()
    while True:
        b = value & 0x7F
        value >>= 7
        if value:
            out.append(b | 0x80)
        else:
            out.append(b)
            return bytes(out)


def field_varint(number: int, value: int) -> bytes:
    return varint((number << 3) | 0) + varint(value)


def field_bytes(number: int, value: bytes | str) -> bytes:
    raw = value.encode('utf-8') if isinstance(value, str) else value
    return varint((number << 3) | 2) + varint(len(raw)) + raw


def field_float(number: int, value: float) -> bytes:
    return varint((number << 3) | 5) + struct.pack('<f', value)


def client_hello() -> bytes:
    return b''.join([
        field_varint(1, 1),
        field_bytes(2, 'm2-online-session-smoke'),
        field_varint(3, 1),
        field_bytes(4, 'python-m2-smoke'),
        field_bytes(5, 'vi-VN'),
    ])


def vec2(x: float, y: float) -> bytes:
    return field_float(1, x) + field_float(2, y)


def move_intent(sequence: int, x: float, y: float, delta_seconds: float) -> bytes:
    return b''.join([
        field_varint(1, sequence),
        field_bytes(2, vec2(x, y)),
        field_float(3, delta_seconds),
    ])


def read_varint(data: bytes, offset: int) -> tuple[int, int]:
    value = 0
    shift = 0
    while offset < len(data) and shift < 64:
        b = data[offset]
        offset += 1
        value |= (b & 0x7F) << shift
        if not (b & 0x80):
            return value, offset
        shift += 7
    raise ValueError('invalid/truncated varint')


def parse_message(data: bytes) -> dict[int, list[tuple[int, object]]]:
    out: dict[int, list[tuple[int, object]]] = {}
    offset = 0
    while offset < len(data):
        key, offset = read_varint(data, offset)
        number, wire = key >> 3, key & 7
        if number == 0:
            raise ValueError('invalid field number 0')
        if wire == 0:
            value, offset = read_varint(data, offset)
        elif wire == 2:
            length, offset = read_varint(data, offset)
            if offset + length > len(data):
                raise ValueError('truncated length-delimited field')
            value = data[offset:offset + length]
            offset += length
        elif wire == 5:
            if offset + 4 > len(data):
                raise ValueError('truncated fixed32 field')
            value = struct.unpack('<f', data[offset:offset + 4])[0]
            offset += 4
        else:
            raise ValueError(f'unsupported wire type {wire}')
        out.setdefault(number, []).append((wire, value))
    return out


def recv_exact(sock: socket.socket, size: int) -> bytes:
    out = bytearray()
    while len(out) < size:
        part = sock.recv(size - len(out))
        if not part:
            raise ConnectionError('connection closed before full frame')
        out.extend(part)
    return bytes(out)


def send_frame(sock: socket.socket, payload: bytes) -> None:
    sock.sendall(struct.pack('>I', len(payload)) + payload)


def recv_frame(sock: socket.socket) -> bytes:
    length = struct.unpack('>I', recv_exact(sock, 4))[0]
    if not (1 <= length <= MAX_FRAME):
        raise ValueError(f'invalid frame length {length}')
    return recv_exact(sock, length)


def assert_server_hello(payload: bytes) -> None:
    msg = parse_message(payload)
    accepted = bool(msg.get(1, [(0, 0)])[0][1])
    protocol = int(msg.get(2, [(0, 0)])[0][1])
    gamedata = int(msg.get(3, [(0, 0)])[0][1])
    if not accepted:
        raise AssertionError('ServerHello was not accepted')
    if protocol != 1 or gamedata != 1:
        raise AssertionError(f'unexpected protocol/gamedata versions: {protocol}/{gamedata}')


def parse_snapshot(payload: bytes) -> dict[str, object]:
    msg = parse_message(payload)
    entity_id = int(msg.get(1, [(0, 0)])[0][1])
    sequence = int(msg.get(2, [(0, 0)])[0][1])
    position_raw = msg.get(3, [(2, b'')])[0][1]
    if not isinstance(position_raw, bytes):
        raise AssertionError('snapshot position is not length-delimited')
    position = parse_message(position_raw)
    x = float(position.get(1, [(5, 0.0)])[0][1])
    y = float(position.get(2, [(5, 0.0)])[0][1])
    z = float(position.get(3, [(5, 0.0)])[0][1])
    yaw = float(msg.get(4, [(5, 0.0)])[0][1])
    server_time = int(msg.get(5, [(0, 0)])[0][1])
    return {
        'entity_id': entity_id,
        'acknowledged_sequence': sequence,
        'x': x,
        'y': y,
        'z': z,
        'yaw_degrees': yaw,
        'server_time_unix_ms': server_time,
    }


def assert_snapshot(payload: bytes, *, expected_sequence: int, expected_x: float, expected_y: float = 0.0, expected_z: float = 0.0) -> dict[str, object]:
    snapshot = parse_snapshot(payload)
    if snapshot['entity_id'] != 1001:
        raise AssertionError(f"unexpected entity_id={snapshot['entity_id']}")
    if snapshot['acknowledged_sequence'] != expected_sequence:
        raise AssertionError(f"unexpected acknowledged sequence={snapshot['acknowledged_sequence']}; expected {expected_sequence}")
    if (
        abs(float(snapshot['x']) - expected_x) > 0.001
        or abs(float(snapshot['y']) - expected_y) > 0.001
        or abs(float(snapshot['z']) - expected_z) > 0.001
    ):
        raise AssertionError(f"unexpected position=({snapshot['x']}, {snapshot['y']}, {snapshot['z']})")
    return snapshot


def run(host: str, port: int) -> dict[str, object]:
    with socket.create_connection((host, port), timeout=3.0) as sock:
        sock.settimeout(3.0)
        send_frame(sock, client_hello())
        assert_server_hello(recv_frame(sock))

        send_frame(sock, move_intent(1, 1.0, 0.0, 0.1))
        first = assert_snapshot(recv_frame(sock), expected_sequence=1, expected_x=0.4)

        send_frame(sock, move_intent(1, 1.0, 0.0, 0.1))
        duplicate = assert_snapshot(recv_frame(sock), expected_sequence=1, expected_x=0.4)

        send_frame(sock, move_intent(2, 0.0, 1.0, 0.05))
        second = assert_snapshot(recv_frame(sock), expected_sequence=2, expected_x=0.4, expected_z=0.2)

        return {
            'first': first,
            'duplicate': duplicate,
            'second': second,
        }


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument('--host', default='127.0.0.1')
    ap.add_argument('--port', required=True, type=int)
    args = ap.parse_args()
    if not (1 <= args.port <= 65535):
        ap.error('--port must be 1..65535')
    snapshot = run(args.host, args.port)
    print('M2_ONLINE_SESSION_SMOKE_PASS', snapshot)
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
