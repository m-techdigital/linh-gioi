"""Refine closed headwear, coherent hair, and cape clearance on the shared wardrobe.
Run with Blender --background --python this_file -- --source INPUT --output OUTPUT.
"""
import argparse
import math
import sys
from pathlib import Path

import bmesh
import bpy
import numpy as np
from mathutils import Vector, Matrix

parser = argparse.ArgumentParser()
parser.add_argument('--source', type=Path, required=True)
parser.add_argument('--output', type=Path, required=True)
args = parser.parse_args(sys.argv[sys.argv.index('--') + 1:])
bpy.ops.wm.open_mainfile(filepath=str(args.source.resolve()))
body = bpy.data.objects['Keeper Macro Reconstruction']
rig = bpy.data.objects['Armature']
for bone in rig.pose.bones:
    bone.matrix_basis.identity()
bpy.context.view_layer.update()
# The head is rigid. Convert the designed silhouette to the shared bind space explicitly.
head_inverse = (rig.pose.bones['Head'].matrix @ rig.data.bones['Head'].matrix_local.inverted()).inverted()
bm = bmesh.new()
bm.from_mesh(body.data)
slot = bm.faces.layers.int.get('appearance_slot')
assert slot is not None, 'Wardrobe ownership must exist before replacing the hat'
bmesh.ops.delete(bm, geom=[face for face in bm.faces if face[slot] in (1, 3)], context='FACES')
bmesh.ops.delete(bm, geom=[vertex for vertex in bm.verts if not vertex.link_faces], context='VERTS')
# Give the lower cape clearance over animated calves while retaining its broad silhouette.
for vertex in bm.verts:
    if vertex.link_faces and all(face[slot] == 12 for face in vertex.link_faces):
        t = max(0., min(1., (1.1 - vertex.co.z) / .8))
        vertex.co.y += .115 * t * t * (3 - 2 * t)
bm.to_mesh(body.data)
bm.free()

image = bpy.data.images['KeeperReconstructionAlbedo']
w, h = image.size
pixels = np.array(image.pixels[:], dtype=np.float32).reshape(h, w, 4)
def swatch(rgb):
    # Retained left atlas is shared with the player. Do not touch or duplicate its pixels.
    sample = pixels[8:h-8:8, 8:w//2-8:8, :3]
    row, col = np.unravel_index(np.argmin(np.sum((sample - rgb) ** 2, axis=2)), sample.shape[:2])
    return ((8 + col * 8 + .5) / w, (8 + row * 8 + .5) / h)
navy = swatch((.055, .075, .12))
gold = swatch((.58, .43, .21))
vertices, faces, colors = [], [], []
def lathe(profile, color, segments=64):
    start = len(vertices)
    for radius, z in profile:
        for i in range(segments):
            angle = i * math.tau / segments
            vertices.append((radius * math.cos(angle), .10 + radius * math.sin(angle), z))
    for row in range(len(profile)):
        following = (row + 1) % len(profile)
        for i in range(segments):
            nxt = (i + 1) % segments
            # Outward winding: the old top was inside-out and disappeared under Unity backface culling.
            faces.append((start + row * segments + i, start + following * segments + i,
                          start + following * segments + nxt, start + row * segments + nxt))
            colors.append(color)
lathe([(.002, 1.86), (.10, 1.837), (.22, 1.802), (.35, 1.773),
       (.35, 1.764), (.22, 1.791), (.10, 1.826), (.002, 1.848)], navy)
lathe([(.344, 1.776), (.36, 1.773), (.36, 1.761), (.344, 1.764)], gold)
lathe([(.088, 1.842), (.102, 1.838), (.102, 1.834), (.088, 1.837)], gold)
# One restrained closed finial and paired rim ornaments; no doubled coplanar sheets.
lathe([(.001, 1.934), (.016, 1.90), (.026, 1.875), (.018, 1.854), (.001, 1.854)], gold, 12)
for side in (-1, 1):
    start = len(vertices)
    x = side * .29
    vertices.extend([(x+dx, y, z) for z in (1.767, 1.64) for y in (-.09, -.096) for dx in (-.009, .009)])
    for indices in ((0,1,3,2),(4,6,7,5),(0,4,5,1),(2,3,7,6),(0,2,6,4),(1,5,7,3)):
        faces.append(tuple(start+i for i in indices)); colors.append(gold)
mesh = bpy.data.meshes.new('Keeper closed headwear')
mesh.from_pydata([head_inverse @ Vector(v) for v in vertices], [], faces)
mesh.update()
hat = bpy.data.objects.new('Headwear', mesh)
bpy.context.collection.objects.link(hat)
mesh.materials.append(body.data.materials[0])
uv = mesh.uv_layers.new(name=body.data.uv_layers.active.name)
for polygon, color in zip(mesh.polygons, colors):
    polygon.use_smooth = True
    for loop in polygon.loop_indices:
        uv.data[loop].uv = color
for name in ('npc_part', 'appearance_slot'):
    attr = mesh.attributes.new(name, 'INT', 'FACE')
    for entry in attr.data: entry.value = 1
hat.vertex_groups.new(name='Head').add(list(range(len(vertices))), 1, 'REPLACE')
bpy.ops.object.select_all(action='DESELECT')
body.select_set(True); hat.select_set(True)
bpy.context.view_layer.objects.active = body
bpy.ops.object.join()
# A closed scalp, one continuous back mass and paired side locks replace scattered hair shards.
hair_vertices, hair_faces, hair_weights = [], [], []
def hair_grid(nu, nv, point, weight):
    start = len(hair_vertices)
    for j in range(nv + 1):
        for i in range(nu + 1):
            u, t = i / nu, j / nv
            hair_vertices.append(point(u, t)); hair_weights.append(weight(t))
    for j in range(nv):
        for i in range(nu):
            a = start + j * (nu + 1) + i
            hair_faces.append((a, a + 1, a + nu + 2, a + nu + 1))
def scalp(u, t):
    phi = u * math.tau
    front = max(0., -math.sin(phi))
    theta = .015 + t * (2.22 - 1.20 * front ** 3)
    return (.099 * math.sin(theta) * math.cos(phi),
            .030 + .099 * math.sin(theta) * math.sin(phi), 1.665 + .102 * math.cos(theta))
hair_grid(32, 12, scalp, lambda t: {'Head': 1.})
def curtain(u, t):
    q = 2 * u - 1
    width = .081 + .026 * math.sin(math.pi * t) - .006 * t
    bottom = 1.195 + .028 * math.cos(q * math.pi * 8) + .045 * abs(q)
    return (width * q, .112 + .140 * t + .060 * math.sin(math.pi * t) - .025 * q * q + .004 * math.cos(q * math.pi * 10),
            1.685 * (1 - t) + bottom * t)
def curtain_weights(t):
    # Lower hair follows the same support as the cape so idle spine motion cannot bury it in cloth.
    head = (1 - t) ** 2
    z = 1.685 * (1 - t) + 1.215 * t
    support = max(0., min(1., (z - 1.0) / .43))
    support = support * support * (3 - 2 * support)
    return {'Head': head, 'spine_03': (1 - head) * support, 'pelvis': (1 - head) * (1 - support)}
hair_grid(32, 16, curtain, curtain_weights)
for side in (-1, 1):
    def lock(u, t, side=side):
        angle = u * math.tau
        radius = .014 * (.85 + .3 * math.sin(math.pi * t)) * (1 - .9 * t ** 5)
        return (side * (.084 + .012 * math.sin(t * math.pi)) + radius * math.cos(angle),
                -.030 - .034 * t + radius * .65 * math.sin(angle), 1.70 - .285 * t)
    hair_grid(12, 14, lock, lambda t: {'Head': 1.})
hair_mesh = bpy.data.meshes.new('Coherent scalp and hair')
hair_mesh.from_pydata(hair_vertices, [], hair_faces); hair_mesh.update()
hair = bpy.data.objects.new('Hair', hair_mesh); bpy.context.collection.objects.link(hair)
hair_mesh.materials.append(body.data.materials[0])
for name, value in (('npc_part', 2), ('appearance_slot', 3)):
    attr = hair_mesh.attributes.new(name, 'INT', 'FACE')
    for entry in attr.data: entry.value = value
hair_uv = hair_mesh.uv_layers.new(name=body.data.uv_layers.active.name)
dark = swatch((.028, .030, .035))
for polygon in hair_mesh.polygons:
    polygon.use_smooth = True
    for loop in polygon.loop_indices: hair_uv.data[loop].uv = dark
for name in ('Head', 'neck_01', 'spine_03', 'pelvis'): hair.vertex_groups.new(name=name)
for i, weights in enumerate(hair_weights):
    for name, weight in weights.items():
        if weight > 0: hair.vertex_groups[name].add([i], weight, 'REPLACE')
# Consistent normals and physical thickness keep both sides visible without a two-sided shader.
bm = bmesh.new(); bm.from_mesh(hair_mesh)
bmesh.ops.recalc_face_normals(bm, faces=list(bm.faces)); bm.to_mesh(hair_mesh); bm.free()
bpy.ops.object.select_all(action='DESELECT'); hair.select_set(True); bpy.context.view_layer.objects.active = hair
shell = hair.modifiers.new('Hair surface thickness', 'SOLIDIFY'); shell.thickness = .003; shell.offset = -1
bpy.ops.object.modifier_apply(modifier=shell.name)
body.select_set(True); bpy.context.view_layer.objects.active = body; bpy.ops.object.join()
assert len(body.data.materials) == 2
bpy.ops.wm.save_as_mainfile(filepath=str(args.output.resolve()), compress=True)
print('LGO_HEAD_AND_CLEARANCE_REFINED headwear_vertices=' + str(len(vertices)) + ' hair=coherent cape_clearance=true')
