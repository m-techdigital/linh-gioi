"""Export the whole keeper remake from its editable Blender source (run with Blender)."""
import argparse
import hashlib
import json
import sys
from pathlib import Path

import bpy

parser = argparse.ArgumentParser()
parser.add_argument('--source', type=Path, required=True)
parser.add_argument('--output', type=Path, required=True)
parser.add_argument('--wardrobe-master', action='store_true', help='Export marked garment objects from the editable wardrobe master')
args = parser.parse_args(sys.argv[sys.argv.index('--') + 1:])
bpy.ops.wm.open_mainfile(filepath=str(args.source.resolve()))
rig = bpy.data.objects['Armature']
for bone in rig.pose.bones:
    bone.matrix_basis.identity()
bpy.context.view_layer.update()
if args.wardrobe_master:
    parts = [obj for obj in bpy.context.scene.objects if obj.type == 'MESH' and obj.asset_data is not None]
    assert parts and all(obj.data.attributes.get('appearance_slot') is not None for obj in parts)
    for part in parts:
        bpy.context.view_layer.objects.active = part
        for modifier in list(part.modifiers):
            if modifier.type != 'ARMATURE':
                bpy.ops.object.modifier_apply(modifier=modifier.name)
    bpy.ops.object.select_all(action='DESELECT')
    for part in parts:
        part.select_set(True)
    bpy.context.view_layer.objects.active = parts[0]
    bpy.ops.object.join()
    mesh = bpy.context.object
    mesh.name = 'Keeper Macro Reconstruction'
    old_materials = list(mesh.data.materials)
    body = bpy.data.materials['Keeper Reconstruction']
    face = bpy.data.materials['Keeper Reconstruction Face']
    assert all(old_materials[polygon.material_index] in (body, face) for polygon in mesh.data.polygons)
    surface_ids = [0 if old_materials[polygon.material_index] == body else 1 for polygon in mesh.data.polygons]
    mesh.data.materials.clear()
    mesh.data.materials.append(body)
    mesh.data.materials.append(face)
    for polygon, surface in zip(mesh.data.polygons, surface_ids):
        polygon.material_index = surface
else:
    mesh = bpy.data.objects['Keeper Macro Reconstruction']
mesh.data.calc_loop_triangles()
assert len(rig.data.bones) == 65
assert len(mesh.data.loop_triangles) > 0
assert len(mesh.data.materials) == 2
assert len(mesh.data.uv_layers) == 1
assert all(1 <= len(v.groups) <= 4 and abs(sum(g.weight for g in v.groups) - 1) < 1e-5
           for v in mesh.data.vertices)
args.output.mkdir(parents=True, exist_ok=True)
for name in ['KeeperReconstructionAlbedo', 'KeeperReconstructionFace']:
    image = bpy.data.images[name]
    # Review the native authoring atlas; platform reduction is deferred until fidelity is approved.
    native_size = (1024, 512) if name.endswith('Face') else (4096, 2048)
    assert tuple(image.size) == native_size, (name, list(image.size))
    image.filepath_raw = str((args.output / (name + '.png')).resolve())
    image.file_format = 'PNG'
    image.save()
sys.path.insert(0, str(Path(__file__).resolve().parent))
sys.dont_write_bytecode = True
from wardrobe_sections import label_sections
sections, slots = label_sections(mesh)
bpy.ops.object.select_all(action='DESELECT')
mesh.select_set(True)
rig.select_set(True)
bpy.context.view_layer.objects.active = mesh
fbx = args.output / 'GateKeeper.fbx'
bpy.ops.export_scene.fbx(filepath=str(fbx.resolve()), use_selection=True,
                        object_types={'ARMATURE', 'MESH'}, add_leaf_bones=False,
                        bake_anim=False, axis_forward='-Z', axis_up='Y', path_mode='STRIP')
receipt = {
    'source_sha256': hashlib.sha256(args.source.read_bytes()).hexdigest(),
    'fbx_sha256': hashlib.sha256(fbx.read_bytes()).hexdigest(),
    'triangles': len(mesh.data.loop_triangles), 'vertices': len(mesh.data.vertices),
    'bones': len(rig.data.bones), 'materials': 2, 'authoring_material_sections': len(sections),
    'slots': slots,
    'texture_sizes': {'albedo': list(bpy.data.images['KeeperReconstructionAlbedo'].size), 'face': list(bpy.data.images['KeeperReconstructionFace'].size)},
    'runtime_verified': False,
}
(args.output / 'export-receipt.json').write_text(json.dumps(receipt, indent=2) + '\n')
print('LGO_KEEPER_RECONSTRUCTION_EXPORTED ' + json.dumps(receipt), flush=True)
