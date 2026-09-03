#!/usr/bin/env python3
from __future__ import annotations
import json, re, sys, xml.etree.ElementTree as ET
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
SERVER=ROOT/'server'
errors=[]

def main():
    for p in [SERVER/'pom.xml',SERVER/'api/pom.xml',SERVER/'shared/pom.xml',SERVER/'realtime/pom.xml']:
        try: ET.parse(p)
        except Exception as e: errors.append(f'{p.relative_to(ROOT)} invalid XML: {e}')
    text=(SERVER/'pom.xml').read_text()
    if '<java.version>25</java.version>' not in text or '<maven.compiler.release>25</maven.compiler.release>' not in text:
        errors.append('server parent POM must target Java 25')
    if '<version>4.1.1</version>' not in text:
        errors.append('server parent POM must pin Spring Boot 4.1.1')
    if '<netty.version>4.2.17.Final</netty.version>' not in text or '<artifactId>netty-bom</artifactId>' not in text:
        errors.append('server parent POM must pin/import Netty BOM 4.2.17.Final')
    rt=(SERVER/'realtime/pom.xml').read_text()
    for needle in ['protobuf-java','<version>3.13.0</version>','build-helper-maven-plugin','build/generated/protocol/java','netty-codec','maven-shade-plugin','<version>3.6.1</version>']:
        if needle not in rt: errors.append(f'realtime POM missing {needle}')
    for p in SERVER.rglob('*.proto'): errors.append(f'protocol source leaked under server: {p.relative_to(ROOT)}')
    alljava='\n'.join(p.read_text(errors='ignore') for p in SERVER.rglob('*.java'))
    if re.search(r'class\s+(ClientHello|ServerHello)\b',alljava): errors.append('hand-written ClientHello/ServerHello DTO detected')
    for p in [SERVER/'build.sh',SERVER/'test.sh']:
        s=p.read_text()
        if './scripts/prepare-protocol.sh' not in s: errors.append(f'{p.relative_to(ROOT)} does not prepare canonical generated Java protocol')
    if errors:
        print('SERVER SOURCE VALIDATION FAILED',file=sys.stderr)
        for e in errors:print(' -',e,file=sys.stderr)
        return 1
    print('SERVER SOURCE VALIDATION PASS')
    return 0
if __name__=='__main__': raise SystemExit(main())
