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
args = parser.parse_args(sys.argv[sys.argv.index('--') + 1:])
bpy.ops.wm.open_mainfile(filepath=str(args.source.resolve()))
rig = bpy.data.objects['Armature']
mesh = bpy.data.objects['Keeper Macro Reconstruction']
for bone in rig.pose.bones:
    bone.matrix_basis.identity()
bpy.context.view_layer.update()
mesh.data.calc_loop_triangles()
assert len(rig.data.bones) == 65
assert len(mesh.data.loop_triangles) <= 25000
assert len(mesh.data.materials) == 2
assert len(mesh.data.uv_layers) == 1
assert all(1 <= len(v.groups) <= 4 and abs(sum(g.weight for g in v.groups) - 1) < 1e-5
           for v in mesh.data.vertices)
args.output.mkdir(parents=True, exist_ok=True)
for name in ['KeeperReconstructionAlbedo', 'KeeperReconstructionFace']:
    image = bpy.data.images[name]
    image.scale(256 if name.endswith('Face') else 512, 256 if name.endswith('Face') else 512)
    image.filepath_raw = str((args.output / (name + '.png')).resolve())
    image.file_format = 'PNG'
    image.save()
# Semantic material sections transport module ownership through FBX without guessing from bone weights or height.
parts = mesh.data.attributes.get('npc_part')
assert parts is not None, 'Authoring mesh must explicitly label NPC parts'
part_ids = [entry.value for entry in parts.data]
body = mesh.data.materials[0]
face = mesh.data.materials[1]
face_polygons = {p.index for p in mesh.data.polygons if p.material_index == 1}
labels = [('Keeper Headwear', 1), ('Keeper Head Hair', 2), ('Keeper Outfit Body', 3)]
mesh.data.materials.clear()
for name, part in labels:
    material = body.copy()
    material.name = name
    mesh.data.materials.append(material)
mesh.data.materials.append(face)
for polygon in mesh.data.polygons:
    part = part_ids[polygon.index]
    assert part in (1, 2, 3), part
    polygon.material_index = 3 if polygon.index in face_polygons else part - 1
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
    'bones': len(rig.data.bones), 'materials': 2, 'authoring_material_sections': 4,
    'texture_sizes': {'albedo': 512, 'face': 256},
    'runtime_verified': False,
}
(args.output / 'export-receipt.json').write_text(json.dumps(receipt, indent=2) + '\n')
print('LGO_KEEPER_RECONSTRUCTION_EXPORTED ' + json.dumps(receipt), flush=True)
