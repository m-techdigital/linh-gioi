"""Preview a complete editable wardrobe and deformation before Unity import.

Blender invocation: --python preview_wardrobe.py -- --source master.blend --output DIR
Only renders staging images; never saves or changes the source blend file.
"""
import argparse
import math
import sys
from pathlib import Path

import bpy
from mathutils import Vector

p = argparse.ArgumentParser()
p.add_argument('--source', type=Path, required=True)
p.add_argument('--output', type=Path, required=True)
a = p.parse_args(sys.argv[sys.argv.index('--') + 1:])
a.output.mkdir(parents=True, exist_ok=True)
bpy.ops.wm.open_mainfile(filepath=str(a.source.resolve()))
rig = bpy.data.objects['Armature']
assert rig.data.bones['thigh_l'].head_local.x > 0, 'Verify leg naming before pose preview'
s = bpy.context.scene
s.render.engine = 'BLENDER_WORKBENCH'
s.display.shading.light = 'STUDIO'
s.display.shading.color_type = 'TEXTURE'
s.display.shading.show_shadows = True
s.display.shading.show_cavity = True
s.render.resolution_x = 800
s.render.resolution_y = 1000
s.render.resolution_percentage = 100
bpy.ops.object.camera_add()
camera = bpy.context.object
camera.data.type = 'ORTHO'
camera.data.ortho_scale = 2.15
s.camera = camera
for name, angle, stride, raised in [('front', 0, False, False), ('side', 90, False, False),
                                  ('back', 180, False, False), ('stride', 25, True, False),
                                  ('raised-arm', -25, False, True)]:
    for bone in rig.pose.bones:
        bone.matrix_basis.identity()
    def rotate(name, axis, degrees):
        bone = rig.pose.bones[name]
        bone.rotation_mode = 'XYZ'
        bone.rotation_euler[axis] = math.radians(degrees)
    rotate('upperarm_l', 2, -60)
    rotate('upperarm_r', 2, 60)
    if stride:
        rotate('thigh_l', 0, 25)
        rotate('thigh_r', 0, -25)
    if raised:
        rotate('upperarm_r', 2, 15)
        rotate('lowerarm_r', 0, 40)
    radians = math.radians(angle)
    camera.location = (3 * math.sin(radians), -3 * math.cos(radians), 1.15)
    camera.rotation_euler = (Vector((0, 0, 1.05)) - camera.location).to_track_quat('-Z', 'Y').to_euler()
    s.render.filepath = str((a.output / (name + '.png')).resolve())
    bpy.ops.render.render(write_still=True)
print('LGO_WARDROBE_PREVIEW_COMPLETE frames=5 runtime_verified=false', flush=True)
