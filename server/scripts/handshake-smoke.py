#!/usr/bin/env python3
from __future__ import annotations
import argparse, socket, struct, sys

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


def field_bytes(number: int, value: str) -> bytes:
    raw = value.encode('utf-8')
    return varint((number << 3) | 2) + varint(len(raw)) + raw


def client_hello(protocol_version: int = 1, gamedata_version: int = 1) -> bytes:
    return b''.join([
        field_varint(1, protocol_version),
        field_bytes(2, 'm0-wire-smoke'),
        field_varint(3, gamedata_version),
        field_bytes(4, 'python-smoke'),
        field_bytes(5, 'vi-VN'),
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
            if length < 0 or offset + length > len(data):
                raise ValueError('truncated length-delimited field')
            value = data[offset:offset + length]
            offset += length
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


def exchange(host: str, port: int, payload: bytes) -> bytes:
    with socket.create_connection((host, port), timeout=3.0) as sock:
        sock.settimeout(3.0)
        sock.sendall(struct.pack('>I', len(payload)) + payload)
        length = struct.unpack('>I', recv_exact(sock, 4))[0]
        if not (1 <= length <= MAX_FRAME):
            raise ValueError(f'invalid server frame length {length}')
        return recv_exact(sock, length)


def assert_server_hello(payload: bytes, expected_accepted: bool, expected_error: str | None = None) -> None:
    msg = parse_message(payload)
    accepted = bool(msg.get(1, [(0, 0)])[0][1])
    protocol = int(msg.get(2, [(0, 0)])[0][1])
    gamedata = int(msg.get(3, [(0, 0)])[0][1])
    if accepted != expected_accepted:
        raise AssertionError(f'accepted={accepted}, expected={expected_accepted}')
    if protocol != 1 or gamedata != 1:
        raise AssertionError(f'unexpected versions protocol={protocol} gamedata={gamedata}')
    if expected_error:
        error_raw = msg.get(5, [(2, b'')])[0][1]
        if not isinstance(error_raw, bytes):
            raise AssertionError('error field is not length-delimited')
        error = parse_message(error_raw)
        code_raw = error.get(1, [(2, b'')])[0][1]
        code = code_raw.decode('utf-8') if isinstance(code_raw, bytes) else ''
        if code != expected_error:
            raise AssertionError(f'error code={code!r}, expected={expected_error!r}')


def send_malformed(host: str, port: int) -> None:
    with socket.create_connection((host, port), timeout=3.0) as sock:
        sock.settimeout(1.0)
        sock.sendall(struct.pack('>I', 1) + b'\xff')
        try:
            data = sock.recv(1)
        except socket.timeout:
            raise AssertionError('malformed ClientHello was not closed promptly')
        if data:
            raise AssertionError('malformed ClientHello unexpectedly received response payload')


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument('--host', default='127.0.0.1')
    ap.add_argument('--port', required=True, type=int)
    args = ap.parse_args()
    if not (1 <= args.port <= 65535):
        ap.error('--port must be 1..65535')

    assert_server_hello(exchange(args.host, args.port, client_hello()), True)
    assert_server_hello(
        exchange(args.host, args.port, client_hello(protocol_version=999)),
        False,
        'UNSUPPORTED_PROTOCOL_VERSION',
    )
    send_malformed(args.host, args.port)
    # Server must survive rejected/malformed clients.
    assert_server_hello(exchange(args.host, args.port, client_hello()), True)
    print(f'HANDSHAKE_SMOKE_PASS host={args.host} port={args.port}')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
