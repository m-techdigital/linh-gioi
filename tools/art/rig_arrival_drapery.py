"""Add five garment joints to an arrival wardrobe; preserve the shared humanoid rig.

Run on the editable wardrobe, never on a derived FBX or joined mesh.
The joints are driven after Humanoid animation by WardrobeDrapeMotion in Unity.
"""
import argparse
import sys
from pathlib import Path

import bpy

p = argparse.ArgumentParser()
p.add_argument('--source', type=Path, required=True)
p.add_argument('--output', type=Path, required=True)
a = p.parse_args(sys.argv[sys.argv.index('--') + 1:])
bpy.ops.wm.open_mainfile(filepath=str(a.source.resolve()))
rig = bpy.data.objects['Armature']
assert len(rig.data.bones) == 65, 'Migration requires the original humanoid skeleton'
for bone in rig.pose.bones:
    bone.matrix_basis.identity()
bpy.context.view_layer.objects.active = rig
bpy.ops.object.mode_set(mode='EDIT')
for i in range(3):
    bone = rig.data.edit_bones.new('drape_cape_' + str(i))
    bone.head = (0, .22 + .035 * i, 1.51 - .71 * i / 3)
    bone.tail = (0, .22 + .035 * (i + 1), 1.51 - .71 * (i + 1) / 3)
    bone.parent = rig.data.edit_bones['spine_03' if i == 0 else 'drape_cape_' + str(i - 1)]
for side, x in [('l', .12), ('r', -.12)]:
    bone = rig.data.edit_bones.new('drape_hem_' + side)
    bone.head = (x, 0, .55)
    bone.tail = (x, 0, .41)
    bone.parent = rig.data.edit_bones['thigh_' + side]
bpy.ops.object.mode_set(mode='OBJECT')

def skin(part, vertex, weights):
    for group in part.vertex_groups:
        group.remove([vertex.index])
    total = sum(weights.values())
    for name, weight in weights.items():
        if weight > 1e-6:
            group = part.vertex_groups.get(name) or part.vertex_groups.new(name=name)
            group.add([vertex.index], weight / total, 'REPLACE')

cape = bpy.data.objects['Cape']
for v in cape.data.vertices:
    row = max(0., min(2., (1.51 - v.co.z) / .71 * 3 - .4))
    lo = int(row)
    hi = min(2, lo + 1)
    weights = {'drape_cape_' + str(lo): 1 - (row - lo)}
    if hi != lo:
        weights['drape_cape_' + str(hi)] = row - lo
    skin(cape, v, weights)
for name, side in [('Outer robe left', 'l'), ('Outer robe right', 'r'),
                   ('Ivory tabard l', 'l'), ('Ivory tabard r', 'r')]:
    part = bpy.data.objects[name]
    for v in part.data.vertices:
        leg = max(0., min(1., (1.08 - v.co.z) / .25))
        leg = leg * leg * (3 - 2 * leg)
        hem = max(0., min(1., (.62 - v.co.z) / .21))
        hem = hem * hem * (3 - 2 * hem)
        skin(part, v, {'pelvis': 1 - leg, 'thigh_' + side: leg * (1 - hem),
                      'drape_hem_' + side: leg * hem})
assert len(rig.data.bones) == 70
for side, angle in [('l', -60), ('r', 60)]:
    import math
    bone = rig.pose.bones['upperarm_' + side]
    bone.rotation_mode = 'XYZ'
    bone.rotation_euler.z = math.radians(angle)
a.output.parent.mkdir(parents=True, exist_ok=True)
bpy.context.preferences.filepaths.save_version = 0
bpy.ops.wm.save_as_mainfile(filepath=str(a.output.resolve()), compress=True)
print('LGO_DRAPERY_RIG_READY humanoid_bones=65 garment_bones=5', flush=True)
