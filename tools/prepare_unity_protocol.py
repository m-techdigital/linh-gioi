#!/usr/bin/env python3
from __future__ import annotations
import argparse, json, shutil, subprocess, tempfile
from pathlib import Path

ROOT=Path(__file__).resolve().parents[1]
DEFAULT_OUT=ROOT/'client/Unity/Assets/Game/Protocol/Generated'

def main()->int:
    ap=argparse.ArgumentParser()
    ap.add_argument('--output', type=Path, default=DEFAULT_OUT)
    args=ap.parse_args()
    out=args.output.resolve()
    with tempfile.TemporaryDirectory(prefix='linhgioi-csharp-proto-') as td:
        tmp=Path(td)
        subprocess.run([str(ROOT/'tools/protocol_codegen.sh'),'generate','--language','csharp','--output-root',str(tmp)],cwd=ROOT,check=True)
        source=tmp/'csharp'
        files=sorted(source.rglob('*.cs'))
        if not files:
            raise SystemExit('ERROR: canonical C# codegen produced no files')
        if out.exists(): shutil.rmtree(out)
        out.mkdir(parents=True)
        for src in files:
            shutil.copy2(src,out/src.name)
        asm_path = out.parent / 'LinhGioi.Protocol.asmdef'
        asm = {
            'name': 'LinhGioi.Protocol',
            'rootNamespace': 'LinhGioi.Protocol.V1',
            'references': [],
            'autoReferenced': True,
            'overrideReferences': True,
            'precompiledReferences': ['Google.Protobuf.dll']
        }
        asm_path.write_text(json.dumps(asm, indent=2) + '\n')
        (out/'README.generated.txt').write_text('Generated from protocol/*.proto by tools/prepare_unity_protocol.py. Do not edit.\n')
    print(f'UNITY_PROTOCOL_PREPARE_PASS output={out} csharp_files={len(files)}')
    return 0
if __name__=='__main__': raise SystemExit(main())
