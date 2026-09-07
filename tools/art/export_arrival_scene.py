"""Export the modular player from its editable Blender source (run with Blender)."""
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
assert len(mesh.data.loop_triangles) > 0
assert len(mesh.data.materials) == 2
assert len(mesh.data.uv_layers) == 1
assert all(1 <= len(v.groups) <= 4 and abs(sum(g.weight for g in v.groups) - 1) < 1e-5
           for v in mesh.data.vertices)
args.output.mkdir(parents=True, exist_ok=True)
# Both atlases are shared with Keeper; preserve native UVs and do not emit duplicate textures.
assert tuple(bpy.data.images['KeeperReconstructionAlbedo'].size) == (4096, 2048)
assert tuple(bpy.data.images['KeeperReconstructionFace'].size) == (1024, 512)
sys.path.insert(0, str(Path(__file__).resolve().parent))
sys.dont_write_bytecode = True
from wardrobe_sections import label_sections
sections, slots = label_sections(mesh)
bpy.ops.object.select_all(action='DESELECT')
mesh.select_set(True)
rig.select_set(True)
bpy.context.view_layer.objects.active = mesh
fbx = args.output / 'ArrivalScene.fbx'
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
    'runtime_verified': False, 'new_textures': [],
}
(args.output / 'export-receipt.json').write_text(json.dumps(receipt, indent=2) + '\n')
print('LGO_ARRIVAL_SCENE_EXPORTED ' + json.dumps(receipt), flush=True)
