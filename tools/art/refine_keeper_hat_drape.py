"""Remake ceremonial headwear and flatten the cape scoop without changing the shared atlas.
Run with Blender --background --python this_file -- --source INPUT --output OUTPUT.
"""
import argparse
import math
import sys
from pathlib import Path

import bmesh
import bpy
import numpy as np
from mathutils import Vector

parser = argparse.ArgumentParser()
parser.add_argument('--source', type=Path, required=True)
parser.add_argument('--output', type=Path, required=True)
args = parser.parse_args(sys.argv[sys.argv.index('--') + 1:])
args.output.parent.mkdir(parents=True,exist_ok=True)
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
bmesh.ops.delete(bm, geom=[face for face in bm.faces if face[slot] == 1], context='FACES')
bmesh.ops.delete(bm, geom=[vertex for vertex in bm.verts if not vertex.link_faces], context='VERTS')
# Flatten the excessive transverse scoop while retaining a broad, continuous drape.
for vertex in bm.verts:
    if vertex.link_faces and all(face[slot] == 12 for face in vertex.link_faces):
        t = max(0., min(1., (1.49 - vertex.co.z) / 1.22))
        width = .165 + .29 * t ** .85
        q = max(-1., min(1., vertex.co.x / width))
        clearance = t*t*(3-2*t)
        vertex.co.y = .175 + .17*t + .115*clearance - .055*q*q*t + .008*math.cos(q*math.pi*4)*math.sin(math.pi*t)
        vertex.co.x *= 1 - .055*t
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
            vertices.append((radius * math.cos(angle), .07 + radius * math.sin(angle), z))
    for row in range(len(profile)):
        following = (row + 1) % len(profile)
        for i in range(segments):
            nxt = (i + 1) % segments
            # Outward winding: the old top was inside-out and disappeared under Unity backface culling.
            faces.append((start + row * segments + i, start + following * segments + i,
                          start + following * segments + nxt, start + row * segments + nxt))
            colors.append(color)
lathe([(.002,1.90),(.07,1.871),(.16,1.822),(.27,1.774),(.34,1.758),
       (.34,1.748),(.27,1.764),(.16,1.812),(.07,1.861),(.002,1.885)],navy)
lathe([(.332,1.762),(.345,1.758),(.345,1.748),(.332,1.752)],gold)
lathe([(.072,1.875),(.084,1.865),(.084,1.860),(.072,1.870)],gold)
lathe([(.001,1.948),(.028,1.932),(.018,1.909),(.028,1.896),(.001,1.896)],gold,12)
def tube(points,radius,color):
    start=len(vertices); sides=6
    for index,p in enumerate(points):
        tangent=(Vector(points[min(index+1,len(points)-1)])-Vector(points[max(0,index-1)])).normalized()
        across=tangent.cross(Vector((0,0,1)))
        if across.length<.01:across=tangent.cross(Vector((0,1,0)))
        across.normalize(); other=tangent.cross(across).normalized()
        for side in range(sides):
            a=side*math.tau/sides
            vertices.append(Vector(p)+radius*(across*math.cos(a)+other*math.sin(a)))
    for row in range(len(points)-1):
        for side in range(sides):
            a=start+row*sides+side;b=start+row*sides+(side+1)%sides
            faces.append((a,b,b+sides,a+sides));colors.append(color)
    faces.append(tuple(start+i for i in reversed(range(sides))));colors.append(color)
    faces.append(tuple(start+(len(points)-1)*sides+i for i in range(sides)));colors.append(color)
def hat_z(r):return float(np.interp(r,[.07,.16,.27,.34],[1.871,1.822,1.774,1.758]))+.003
# Eight raised cloud scrolls remain one headwear module and reuse the atlas swatch.
for sector in range(8):
    points=[]
    for i in range(33):
        t=i/32; a=sector*math.tau/8 + .26*math.sin(t*math.tau)
        r=.12+.19*t
        points.append((r*math.cos(a),.07+r*math.sin(a),hat_z(r)))
    tube(points,.0024,gold)
for side in (-1,1):
    x=side*.295;y=-.085
    tube([(x,y,1.762),(x,y,1.685)],.003,gold)
    start=len(vertices)
    vertices.extend([(x+dx,y+dy,z) for z in (1.685,1.600) for dy in (-.003,.003) for dx in (-.012,.012)])
    for f in ((0,1,3,2),(4,6,7,5),(0,4,5,1),(2,3,7,6),(0,2,6,4),(1,5,7,3)):
        faces.append(tuple(start+i for i in f));colors.append(gold)
mesh = bpy.data.meshes.new('Keeper closed headwear')
mesh.from_pydata([head_inverse @ (Vector(v)-Vector((0,0,.02))) for v in vertices], [], faces)
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
bm=bmesh.new();bm.from_mesh(mesh);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(mesh);bm.free()
hat.vertex_groups.new(name='Head').add(list(range(len(vertices))), 1, 'REPLACE')
bpy.ops.object.select_all(action='DESELECT')
body.select_set(True); hat.select_set(True)
bpy.context.view_layer.objects.active = body
bpy.ops.object.join()

# Preserve the two-material shared atlas contract.
assert len(body.data.materials)==2
bpy.context.preferences.filepaths.save_version=0
bpy.ops.wm.save_as_mainfile(filepath=str(args.output.resolve()),compress=True)
print('LGO_KEEPER_HAT_DRAPE_REFINED vertices='+str(len(vertices))+' shared_atlas_unchanged=true')
