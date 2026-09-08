"""Preview a complete editable wardrobe and deformation before Unity import.

Blender invocation: --python preview_wardrobe.py -- --source master.blend --output DIR
Only renders staging images; never saves or changes the source blend file.
"""
import argparse
import math
import sys
import json
from pathlib import Path

import bpy
from mathutils import Vector, Matrix

p = argparse.ArgumentParser()
p.add_argument('--source', type=Path, required=True)
p.add_argument('--output', type=Path, required=True)
p.add_argument('--poses', type=Path, help='Retargeted Unity pose library from WardrobePoseExporter')
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
if a.poses:
    import numpy as np
    library = json.loads(a.poses.read_text())['poses']
    def matrix(values):
        return Matrix([values[row * 4:(row + 1) * 4] for row in range(4)])
    entries = [item for item in library[0]['bones'] if item['name'] in rig.data.bones and not item['name'].startswith('drape_')]
    unity = np.array([list(matrix(item['rest']).translation) + [1.] for item in entries])
    blender = np.array([list(rig.data.bones[item['name']].head_local) + [1.] for item in entries])
    transform = np.linalg.lstsq(unity, blender, rcond=None)[0].T
    error = float(np.max(np.abs(unity @ transform.T - blender)))
    assert error < .002, f'Runtime/source rig alignment error: {error}'
    space = Matrix(transform.tolist())
    inverse_space = space.inverted()
    for pose in library:
        for bone in rig.pose.bones:
            bone.matrix_basis.identity()
        targets = {item['name']: space @ matrix(item['posed']) @ matrix(item['rest']).inverted() @ inverse_space @ rig.data.bones[item['name']].matrix_local
                   for item in pose['bones'] if item['name'] in rig.data.bones and not item['name'].startswith('drape_')}
        # Parents first; each assigned matrix is in armature space.
        for bone in sorted(rig.pose.bones, key=lambda value: len(value.parent_recursive)):
            if bone.name in targets:
                bone.matrix = targets[bone.name]
                bpy.context.view_layer.update()
        hip_delta = rig.pose.bones['pelvis'].matrix.to_quaternion() @ rig.data.bones['pelvis'].matrix_local.to_quaternion().inverted()
        forward = hip_delta @ Vector((0, -1, 0))
        forward.z = 0
        forward.normalize()
        if 'drape_cape_0' in rig.pose.bones:
            facing = Vector((0, -1, 0)).rotation_difference(forward)
            def aim(name, direction, free=1.):
                bone = rig.pose.bones[name]
                rest = bone.bone.matrix_local.to_quaternion()
                axis = (bone.bone.tail_local - bone.bone.head_local).normalized()
                rotation = (facing @ axis).rotation_difference(direction.normalized()) @ facing @ rest
                if free < 1.:
                    torso_delta = rig.pose.bones['spine_03'].matrix.to_quaternion() @ rig.data.bones['spine_03'].matrix_local.to_quaternion().inverted()
                    rotation = (torso_delta @ rest).slerp(rotation, free)
                bone.matrix = Matrix.Translation(bone.head) @ rotation.to_matrix().to_4x4()
                bpy.context.view_layer.update()
            for i in range(3):
                aim('drape_cape_' + str(i), Vector((0, 0, -1)) - forward * (.15 + pose['speed'] * .055), [0., .65, 1.][i])
            for side in ['l', 'r']:
                leg = (rig.pose.bones['calf_' + side].head - rig.pose.bones['thigh_' + side].head).normalized()
                aim('drape_hem_' + side, Vector((0, 0, -.3)) + leg * .7)
        for angle in ([25, 100] if pose['name'] in ['Jog_Fwd_Loop-2', 'Jog_Fwd_Loop-6'] else [25]):
            radians = math.radians(angle)
            lateral = Vector((-forward.y, forward.x, 0))
            camera.location = forward * (3 * math.cos(radians)) + lateral * (3 * math.sin(radians)) + Vector((0, 0, 1.15))
            camera.rotation_euler = (Vector((0, 0, 1.05)) - camera.location).to_track_quat('-Z', 'Y').to_euler()
            s.render.filepath = str((a.output / (pose['name'] + '-' + str(angle) + '.png')).resolve())
            bpy.ops.render.render(write_still=True)
    print(f'LGO_RUNTIME_POSE_PREVIEW_COMPLETE poses={len(library)} alignment_error={error} secondary_motion=equilibrium runtime_verified=false', flush=True)
    sys.exit(0)
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
        rotate('thigh_l', 0, 55)
        rotate('calf_l', 0, -65)
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
