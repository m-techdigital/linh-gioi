#!/usr/bin/env python3
from __future__ import annotations
import json, re, sys, tempfile, subprocess
from pathlib import Path

ROOT=Path(__file__).resolve().parents[1]
UNITY=ROOT/'client/Unity'
errors=[]

def require(path:Path):
    if not path.exists(): errors.append(f'missing: {path.relative_to(ROOT)}')

def main()->int:
    required=[
        UNITY/'ProjectSettings/ProjectVersion.txt',
        UNITY/'Packages/manifest.json',
        UNITY/'packages.config',
        UNITY/'Assets/Game/Foundation/Runtime/ClientRuntimeConfig.cs',
        UNITY/'Assets/Game/Bootstrap/Runtime/GameBootstrap.cs',
        UNITY/'Assets/Game/Networking/Runtime/TcpRealtimeClient.cs',
        UNITY/'Assets/Game/UI/design-tokens.json',
        UNITY/'Assets/Game/UI/Runtime/UIPrimitives.cs',
        UNITY/'Assets/Game/Foundation/Editor/M0ProjectGenerator.cs',
    ]
    for p in required: require(p)
    if errors: return finish()

    pv=(UNITY/'ProjectSettings/ProjectVersion.txt').read_text()
    if '6000.3.2f1' not in pv: errors.append('Unity editor pin must be 6000.3.2f1')
    manifest=json.loads((UNITY/'Packages/manifest.json').read_text())
    deps=manifest.get('dependencies',{})
    expected={'com.unity.render-pipelines.universal':'17.3.0','com.unity.test-framework':'1.4.3','com.github-glitchenzo.nugetforunity':'4.5.0'}
    for name,version in expected.items():
        if deps.get(name)!=version: errors.append(f'Unity package {name} expected {version}, got {deps.get(name)!r}')
    if 'Google.Protobuf' not in (UNITY/'packages.config').read_text(): errors.append('Google.Protobuf NuGet dependency missing')

    # asmdef graph: project-local references must remain acyclic. External references are ignored.
    asmdefs=[]; names={}
    for p in sorted((UNITY/'Assets/Game').rglob('*.asmdef')):
        if 'Generated' in p.parts: continue
        d=json.loads(p.read_text()); name=d.get('name')
        if not name: errors.append(f'asmdef without name: {p.relative_to(ROOT)}'); continue
        if name in names: errors.append(f'duplicate asmdef name: {name}')
        names[name]=p; asmdefs.append((p,d))
    graph={d['name']:[r for r in d.get('references',[]) if r in names] for _,d in asmdefs}
    visiting=set(); visited=set()
    def visit(n,stack):
        if n in visiting:
            errors.append('asmdef cycle: '+' -> '.join(stack+[n])); return
        if n in visited:return
        visiting.add(n)
        for m in graph.get(n,[]): visit(m,stack+[n])
        visiting.remove(n); visited.add(n)
    for n in graph: visit(n,[])

    generator=(UNITY/'Assets/Game/Foundation/Editor/M0ProjectGenerator.cs').read_text()
    if 'DeleteAsset(GeneratedRoot)' in generator:
        errors.append('M0ProjectGenerator must not delete GeneratedRoot; it would remove prepared protocol C# surface')
    for controlled in ['RenderPipeline','Scenes','UI']:
        if f'GeneratedRoot + "/{controlled}"' not in generator:
            errors.append(f'M0ProjectGenerator missing controlled disposable generated folder cleanup: {controlled}')

    if 'RendererDataPath' not in generator or 'EnsureUrpDefaultRenderer' not in generator:
        errors.append('M0ProjectGenerator must configure a concrete URP default renderer for batch Linux player builds')
    if 'm_RendererDataList' not in generator or 'm_DefaultRendererIndex' not in generator:
        errors.append('M0ProjectGenerator missing serialized URP default renderer binding')

    # Required UI primitives from authoritative design system.
    ui=(UNITY/'Assets/Game/UI/Runtime/UIPrimitives.cs').read_text()
    for cls in ['BaseButton','IconButton','BasePanel','ModalPanel','ProgressBar','HealthBar','ManaBar','SkillButton','AvatarView','Nameplate','TabBar','Toast','CurrencyDisplay']:
        if not re.search(rf'\bclass\s+{cls}\b',ui): errors.append(f'missing UI primitive: {cls}')

    # Canonical codegen must be able to prepare the disposable Unity protocol surface in a clean temp root.
    with tempfile.TemporaryDirectory(prefix='linhgioi-unity-proto-') as td:
        out=Path(td)/'Protocol'
        cp=subprocess.run([sys.executable,str(ROOT/'tools/prepare_unity_protocol.py'),'--output',str(out)],cwd=ROOT,text=True,capture_output=True)
        if cp.returncode!=0:
            errors.append('Unity protocol prepare failed: '+(cp.stderr or cp.stdout).strip())
        else:
            cs=list(out.glob('*.cs'))
            if len(cs)!=6: errors.append(f'Unity protocol expected 6 generated C# files, got {len(cs)}')
            if not (out.parent/'LinhGioi.Protocol.asmdef').exists(): errors.append('generated Unity protocol asmdef missing')

    if not (UNITY/'Assets/Game/Protocol/LinhGioi.Protocol.asmdef').exists():
        errors.append('stable LinhGioi.Protocol asmdef missing')

    # Hygiene: source baseline must not ship reproducible generated assets.
    disposable_paths = [
        UNITY/'Assets/Game/Generated',
        UNITY/'Assets/Game/Protocol/Generated',
    ]
    for generated in disposable_paths:
        if generated.exists() and any(p.is_file() for p in generated.rglob('*')):
            errors.append(f'{generated.relative_to(ROOT)} contains disposable generated source/assets; clean before packaging')
    return finish()

def finish()->int:
    if errors:
        print('UNITY FOUNDATION STATIC VALIDATION FAILED',file=sys.stderr)
        for e in errors: print(' -',e,file=sys.stderr)
        return 1
    print('UNITY FOUNDATION STATIC VALIDATION PASS')
    return 0

if __name__=='__main__': raise SystemExit(main())
