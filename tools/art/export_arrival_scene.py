"""Export the modular player from its editable Blender source (run with Blender)."""
import argparse
import hashlib
import json
import sys
from pathlib import Path

import bpy

parser = argparse.ArgumentParser()
parser.add_argument('--source', type=Path, default=Path(__file__).resolve().parents[2] / 'client/art-source/arrival-scene/ArrivalScene.blend')
parser.add_argument('--wardrobe-master', action='store_true')
parser.add_argument('--joined-source', type=Path)
parser.add_argument('--output', type=Path, required=True)
args = parser.parse_args(sys.argv[sys.argv.index('--') + 1:])
bpy.ops.wm.open_mainfile(filepath=str(args.source.resolve()))
rig = bpy.data.objects['Armature']
for bone in rig.pose.bones:
    bone.matrix_basis.identity()
bpy.context.view_layer.update()
sys.path.insert(0, str(Path(__file__).resolve().parent))
sys.dont_write_bytecode = True
from wardrobe_master import join_master, save_joined
mesh = join_master(rig, 'Arrival Scene Reconstruction') if args.wardrobe_master else bpy.data.objects['Arrival Scene Reconstruction']

mesh.data.calc_loop_triangles()
garment_bones = {b.name for b in rig.data.bones if b.name.startswith('drape_')}
assert garment_bones in (set(), {'drape_cape_0', 'drape_cape_1', 'drape_cape_2', 'drape_hem_l', 'drape_hem_r'})
assert len(rig.data.bones) - len(garment_bones) == 65
assert len(mesh.data.loop_triangles) > 0
assert len(mesh.data.materials) == 2
assert len(mesh.data.uv_layers) == 1
assert all(1 <= len(v.groups) <= 4 and abs(sum(g.weight for g in v.groups) - 1) < 1e-5
           for v in mesh.data.vertices)
if args.joined_source:
    save_joined(mesh, args.source, args.joined_source)
args.output.mkdir(parents=True, exist_ok=True)
# Keep the shared wardrobe atlas; the continuous player head owns its facial albedo.
assert tuple(bpy.data.images['KeeperReconstructionAlbedo'].size) == (4096, 2048)
face = bpy.data.images['ArrivalFace']
assert face.size[0] > 0 and face.size[1] > 0
face_path = args.output / 'ArrivalFace.png'
face.save(filepath=str(face_path.resolve()))
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
    'texture_sizes': {'albedo': list(bpy.data.images['KeeperReconstructionAlbedo'].size), 'face': list(face.size)},
    'face_sha256': hashlib.sha256(face_path.read_bytes()).hexdigest(),
    'runtime_verified': False, 'new_textures': ['ArrivalFace.png'],
}
(args.output / 'export-receipt.json').write_text(json.dumps(receipt, indent=2) + '\n')
print('LGO_ARRIVAL_SCENE_EXPORTED ' + json.dumps(receipt), flush=True)
