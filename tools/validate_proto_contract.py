#!/usr/bin/env python3
from __future__ import annotations
import re, sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
P=ROOT/'protocol'
errors=[]
files=sorted(P.glob('*.proto'))
if not files: errors.append('no proto files')
for f in files:
    s=f.read_text(encoding='utf-8')
    if 'syntax = "proto3";' not in s: errors.append(f'{f.name}: missing proto3 syntax')
    if 'package linhgioi.v1;' not in s: errors.append(f'{f.name}: wrong/missing package')
    if 'option csharp_namespace = "LinhGioi.Protocol.V1";' not in s: errors.append(f'{f.name}: missing csharp namespace')
    if 'option java_package = "com.linhgioi.protocol.v1";' not in s: errors.append(f'{f.name}: missing java package')
    for imp in re.findall(r'import\s+"([^"]+)"\s*;', s):
        if not (P/imp).exists(): errors.append(f'{f.name}: missing import {imp}')
    for mm in re.finditer(r'message\s+(\w+)\s*\{([^}]*)\}', s, flags=re.S):
        name,body=mm.group(1),mm.group(2)
        nums=[]
        for fm in re.finditer(r'\b(?:repeated\s+)?[\w.<>]+\s+\w+\s*=\s*(\d+)\s*;', body): nums.append(int(fm.group(1)))
        dup={n for n in nums if nums.count(n)>1}
        if dup: errors.append(f'{f.name}:{name}: duplicate field numbers {sorted(dup)}')
if errors:
    print('PROTO CONTRACT STATIC CHECK FAILED')
    for e in errors: print(' -',e)
    sys.exit(1)
print(f'PROTO CONTRACT STATIC CHECK PASS: {len(files)} files')
print('NOTE: static lexical guard only; run ./tools/protocol_codegen.sh verify for canonical protoc compile/codegen evidence.')
