"""Fit the complete traveller outfit from the 16846a8 master.

Narrow the ceremonial side panels for locomotion, fit forearm armour, and shape
the cloak's shoulder yoke. Reuse the atlas, semantic objects and 70-joint rig;
the sewn ivory lining receives its own UV strip within the shared fabric tile.
This is a one-time art migration; subsequent edits belong in the saved master.
"""
import argparse
import hashlib
import math
import sys
from pathlib import Path

import bpy
from mathutils import Vector
from mathutils.bvhtree import BVHTree
from mathutils.geometry import barycentric_transform

p = argparse.ArgumentParser()
p.add_argument('--source', type=Path, required=True)
p.add_argument('--output', type=Path, required=True)
a = p.parse_args(sys.argv[sys.argv.index('--') + 1:])
assert hashlib.sha256(a.source.read_bytes()).hexdigest() == 'bd20d7d4a840c06103375fd90315cefdda485b4607b51b6bd14c03f76986e91f', 'Use the 16846a8 master; never apply this migration twice'
bpy.ops.wm.open_mainfile(filepath=str(a.source.resolve()))
rig = bpy.data.objects['Armature']
assert len(rig.data.bones) == 70
for bone in rig.pose.bones:
    bone.matrix_basis.identity()

# The guardian's semicircular skirt is too deep for a mobile traveller. Keep
# the waist seam, taper the depth, and open the front/back edges towards the hem.
pants = bpy.data.objects['LowerBody']
def envelope(z):
    # Fit over the actual trousers, including their hip volume. A silhouette
    # taper alone can otherwise make the visible trouser mesh pierce the coat.
    band = [v.co for v in pants.data.vertices if abs(v.co.z - z) < .045]
    if not band:
        return 0., 0.
    return max(abs(v.x) for v in band) + .012, max(abs(v.y - .027) for v in band) + .012

for name, side in [('Outer robe left', 1), ('Outer robe right', -1)]:
    part = bpy.data.objects[name]
    assert len(part.data.vertices) == 625, 'Expected the editable 24x24 quad panel'
    for vertex in part.data.vertices:
        t = (vertex.index // 25) / 24
        u = (vertex.index % 25) / 24
        if side < 0:
            u = 1 - u
        angle = (.30 + .40 * t * t) * (1 - u) + (math.pi - .35 * t * t) * u
        fold = .005 * math.sin(math.pi * t) * math.cos(angle * 6)
        body_x, body_y = envelope(vertex.co.z)
        vertex.co.x = side * (max(.125 + .055 * t, body_x) + fold) * math.sin(angle)
        vertex.co.y = .027 - (max(.125 + .015 * t, body_y) + fold) * math.cos(angle)
# Sew the ivory lining to the same front edge and support weights as the coat.
# Keeping a separately shaped wide tabard after tapering exposes two floating
# white flaps; a shared edge remains continuous during the whole stride.
for side, suffix, panel_name in [(1, 'l', 'Outer robe left'), (-1, 'r', 'Outer robe right')]:
    panel = bpy.data.objects[panel_name]
    part = bpy.data.objects['Ivory tabard ' + suffix]
    vertices, faces, uvs, weights = [], [], [], []
    cols = 3
    for row in range(25):
        edge = panel.data.vertices[row * 25 + (0 if side == 1 else 24)]
        support = {panel.vertex_groups[g.group].name: g.weight for g in edge.groups}
        t = row / 24
        for col in range(cols + 1):
            u = col / cols
            point = edge.co.copy()
            point.x += side * (.005 - .031 * u)
            point.y += .003
            vertices.append(tuple(point))
            uvs.append(((32.5 + (.08 + .24 * u) * 511) / 4096,
                        (608.5 + (.06 + .88 * (1 - t)) * 511) / 2048))
            weights.append(support)
    for row in range(24):
        for col in range(cols):
            k = row * (cols + 1) + col
            face = (k, k + 1, k + cols + 2, k + cols + 1)
            faces.append(face if side == 1 else tuple(reversed(face)))
    mesh = bpy.data.meshes.new('Sewn ivory lining ' + suffix)
    mesh.from_pydata(vertices, [], faces)
    mesh.update()
    mesh.materials.append(part.data.materials[0])
    part.data = mesh
    uv = mesh.uv_layers.new(name='UVMap')
    for face in mesh.polygons:
        face.use_smooth = True
        for loop in face.loop_indices:
            uv.data[loop].uv = uvs[mesh.loops[loop].vertex_index]
    for name, value in [('appearance_slot', 10), ('npc_part', 3)]:
        for item in mesh.attributes.new(name, 'INT', 'FACE').data:
            item.value = value
    for index, support in enumerate(weights):
        for name, value in support.items():
            group = part.vertex_groups.get(name) or part.vertex_groups.new(name=name)
            group.add([index], value, 'REPLACE')

# Scale cross-sections around the forearm, preserving arm length and articulation.
body = bpy.data.objects['UpperBody']
for vertex in body.data.vertices:
    weights = {body.vertex_groups[g.group].name: g.weight for g in vertex.groups}
    for side in ['l', 'r']:
        influence = weights.get('lowerarm_' + side, 0.)
        if influence < .5:
            continue
        bone = rig.data.bones['lowerarm_' + side]
        axis = (bone.tail_local - bone.head_local).normalized()
        centre = bone.head_local + axis * (vertex.co - bone.head_local).dot(axis)
        vertex.co = centre + (vertex.co - centre) * (1 - .16 * influence)
for vertex in bpy.data.objects['Gloves'].data.vertices:
    bone = rig.data.bones['hand_' + ('l' if vertex.co.x > 0 else 'r')]
    axis = (bone.tail_local - bone.head_local).normalized()
    centre = bone.head_local + axis * (vertex.co - bone.head_local).dot(axis)
    vertex.co = centre + (vertex.co - centre) * .92

# A curved yoke reaches under both shoulder guards instead of floating behind
# the neck as the straight edge of a rectangular cape.
cape = bpy.data.objects['Cape']
assert len(cape.data.vertices) == 841
body.data.calc_loop_triangles()
shirt_triangles = [tuple(tri.vertices) for tri in body.data.loop_triangles]
shirt_surface = BVHTree.FromPolygons([v.co for v in body.data.vertices], shirt_triangles, all_triangles=True)
for vertex in cape.data.vertices:
    t = (vertex.index // 29) / 28
    lateral = 2 * (vertex.index % 29) / 28 - 1
    blend = max(0., 1 - t / .30) ** 2
    vertex.co.x += lateral * .06 * blend
    vertex.co.y += (-.09 - .11 * abs(lateral) ** 1.5) * blend
    vertex.co.z += (.02 - .07 * abs(lateral) ** 1.5) * blend
    if t < .30:
        hit, _, triangle, _ = shirt_surface.ray_cast(Vector((vertex.co.x, 1., vertex.co.z)), Vector((0, -1, 0)), 2.)
        if hit is not None:
            vertex.co.y = max(vertex.co.y, hit.y + .018)
            source = [body.data.vertices[index] for index in shirt_triangles[triangle]]
            bary = barycentric_transform(hit, *[item.co for item in source],
                                         Vector((1, 0, 0)), Vector((0, 1, 0)), Vector((0, 0, 1)))
            support = {}
            for item, factor in zip(source, bary):
                for group in item.groups:
                    name = body.vertex_groups[group.group].name
                    support[name] = support.get(name, 0.) + group.weight * max(0., factor)
            attached = max(0., min(1., (.30 - t) / .18))
            old_weights = {cape.vertex_groups[g.group].name: g.weight for g in vertex.groups}
            weights = {name: value * (1 - attached) for name, value in old_weights.items()}
            for name, value in support.items():
                weights[name] = weights.get(name, 0.) + value * attached
            weights = dict(sorted(((name, value) for name, value in weights.items() if value > 1e-6), key=lambda item: item[1], reverse=True)[:4])
            total = sum(weights.values())
            assert total > 0
            for group in cape.vertex_groups:
                group.remove([vertex.index])
            for name, weight in weights.items():
                group = cape.vertex_groups.get(name) or cape.vertex_groups.new(name=name)
                group.add([vertex.index], weight / total, 'REPLACE')

for side, angle in [('l', -60), ('r', 60)]:
    bone = rig.pose.bones['upperarm_' + side]
    bone.rotation_mode = 'XYZ'
    bone.rotation_euler.z = math.radians(angle)
a.output.parent.mkdir(parents=True, exist_ok=True)
bpy.context.preferences.filepaths.save_version = 0
bpy.ops.wm.save_as_mainfile(filepath=str(a.output.resolve()), compress=True)
print('LGO_TRAVELLER_FIT_READY panels=4 forearms=2 gloves=2 shoulder_yoke=true sewn_lining=true', flush=True)
