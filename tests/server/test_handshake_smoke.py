from __future__ import annotations

import importlib.util
import socket
import struct
import subprocess
import sys
import threading
import time
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
SCRIPT = ROOT / "server" / "scripts" / "handshake-smoke.py"
SPEC = importlib.util.spec_from_file_location("handshake_smoke", SCRIPT)
MODULE = importlib.util.module_from_spec(SPEC)
sys.modules[SPEC.name] = MODULE
assert SPEC.loader is not None
SPEC.loader.exec_module(MODULE)


def server_hello(accepted: bool, error_code: str | None = None) -> bytes:
    out = bytearray()
    if accepted:
        out += MODULE.field_varint(1, 1)
    out += MODULE.field_varint(2, 1)
    out += MODULE.field_varint(3, 1)
    out += MODULE.field_varint(4, int(time.time() * 1000))
    if error_code:
        nested = MODULE.field_bytes(1, error_code) + MODULE.field_bytes(2, "rejected")
        out += MODULE.varint((5 << 3) | 2) + MODULE.varint(len(nested)) + nested
    return bytes(out)


def recv_exact(sock: socket.socket, count: int) -> bytes:
    data = bytearray()
    while len(data) < count:
        chunk = sock.recv(count - len(data))
        if not chunk:
            raise ConnectionError("closed")
        data.extend(chunk)
    return bytes(data)


class FakeHandshakeServer(threading.Thread):
    def __init__(self):
        super().__init__(daemon=True)
        self.listener = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.listener.bind(("127.0.0.1", 0))
        self.listener.listen(8)
        self.port = self.listener.getsockname()[1]
        self.error: BaseException | None = None

    def run(self):
        try:
            for _ in range(4):
                conn, _addr = self.listener.accept()
                with conn:
                    length = struct.unpack(">I", recv_exact(conn, 4))[0]
                    payload = recv_exact(conn, length)
                    if payload == b"\xff":
                        continue
                    parsed = MODULE.parse_message(payload)
                    protocol = int(parsed.get(1, [(0, 0)])[0][1])
                    if protocol == 1:
                        response = server_hello(True)
                    else:
                        response = server_hello(False, "UNSUPPORTED_PROTOCOL_VERSION")
                    conn.sendall(struct.pack(">I", len(response)) + response)
        except BaseException as exc:
            self.error = exc
        finally:
            self.listener.close()


class HandshakeSmokeToolTest(unittest.TestCase):
    def test_wire_smoke_valid_rejected_malformed_then_valid(self):
        server = FakeHandshakeServer()
        server.start()
        completed = subprocess.run(
            [sys.executable, str(SCRIPT), "--host", "127.0.0.1", "--port", str(server.port)],
            cwd=ROOT,
            text=True,
            capture_output=True,
            timeout=10,
        )
        server.join(timeout=3)
        self.assertEqual(0, completed.returncode, completed.stderr + completed.stdout)
        self.assertIn("HANDSHAKE_SMOKE_PASS", completed.stdout)
        self.assertIsNone(server.error)
        self.assertFalse(server.is_alive())


if __name__ == "__main__":
    unittest.main()
