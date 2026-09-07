"""Export the editable arrival appearance; run with Blender --background --python ... -- --output PATH."""
import argparse
import hashlib
import json
import sys
from pathlib import Path
import bpy

parser = argparse.ArgumentParser()
parser.add_argument('--source', type=Path, default=Path(__file__).resolve().parents[2] / 'client/art-source/arrival-scene/ArrivalScene.blend')
parser.add_argument('--output', type=Path, required=True)
args = parser.parse_args(sys.argv[sys.argv.index('--') + 1:])
bpy.ops.wm.open_mainfile(filepath=str(args.source.resolve()))
rig = bpy.data.objects['Armature']
mesh = bpy.data.objects['Arrival Scene Reconstruction']
for bone in rig.pose.bones:
    bone.matrix_basis.identity()
bpy.context.view_layer.update()
mesh.data.calc_loop_triangles()
assert len(rig.data.bones) == 65
assert len(mesh.data.loop_triangles) <= 25000
assert {m.name for m in mesh.data.materials} == {'Arrival Scene Palette', 'Keeper Reconstruction', 'Keeper Reconstruction Face'}
assert all(1 <= len(v.groups) <= 4 and abs(sum(g.weight for g in v.groups) - 1) < 1e-5 for v in mesh.data.vertices)
# Keeper's original native atlas occupies the left half of its expanded body atlas.
# Arrival authoring retains original UVs; map the shared body/hair section on export only.
body_slot = next(i for i, material in enumerate(mesh.data.materials) if material.name == 'Keeper Reconstruction')
uv = mesh.data.uv_layers.active.data
for polygon in mesh.data.polygons:
    if polygon.material_index == body_slot:
        for loop_index in polygon.loop_indices:
            uv[loop_index].uv.x *= 0.5
args.output.mkdir(parents=True, exist_ok=True)
palette = bpy.data.images['ArrivalPalette']
palette.filepath_raw = str((args.output / 'ArrivalPalette.png').resolve())
palette.file_format = 'PNG'
palette.save()
bpy.ops.object.select_all(action='DESELECT')
mesh.select_set(True)
rig.select_set(True)
bpy.context.view_layer.objects.active = mesh
fbx = args.output / 'ArrivalScene.fbx'
bpy.ops.export_scene.fbx(filepath=str(fbx.resolve()), use_selection=True,
    object_types={'ARMATURE', 'MESH'}, add_leaf_bones=False, bake_anim=False,
    axis_forward='-Z', axis_up='Y', path_mode='STRIP')
receipt = {'source_sha256': hashlib.sha256(args.source.read_bytes()).hexdigest(),
    'fbx_sha256': hashlib.sha256(fbx.read_bytes()).hexdigest(),
    'triangles': len(mesh.data.loop_triangles), 'vertices': len(mesh.data.vertices), 'bones': 65,
    'material_sections': 3, 'new_texture': [48, 8], 'shared_textures': ['KeeperReconstructionAlbedo', 'KeeperReconstructionFace']}
(args.output / 'export-receipt.json').write_text(json.dumps(receipt, indent=2) + '\n')
print('LGO_ARRIVAL_SCENE_EXPORTED ' + json.dumps(receipt), flush=True)
