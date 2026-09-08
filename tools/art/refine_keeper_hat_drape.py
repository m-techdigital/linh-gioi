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
parser.add_argument('--headwear-only', action='store_true', help='Preserve the verified cape on subsequent headwear passes')
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
    if not args.headwear_only and vertex.link_faces and all(face[slot] == 12 for face in vertex.link_faces):
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
lathe([(.002,1.905),(.027,1.901),(.032,1.892),(.025,1.888),(.002,1.892)],gold,24)
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
# Open four-petal crown, with the height kept inside the existing humanoid guard.
for sector in range(4):
    angle = sector * math.tau / 4
    points=[]
    for i in range(25):
        t=i/24
        r=.033*math.sin(math.pi*t)
        points.append((r*math.cos(angle),.07+r*math.sin(angle),1.900+.049*t))
    tube(points,.0035,gold)
# Repeated paired cloud curls form a readable ornamental band around the crown.
# These are geometry in the same wardrobe mesh, not decals or extra renderers.
for sector in range(12):
    angle=sector*math.tau/12
    for mirror in (-1,1):
        points=[]
        for i in range(33):
            t=i/32; turn=t*math.tau*1.25
            radius=.040*(1-.88*t)
            radial=.262+radius*math.cos(turn)
            tangent=mirror*(.022+radius*math.sin(turn))
            x=radial*math.cos(angle)-tangent*math.sin(angle)
            y=radial*math.sin(angle)+tangent*math.cos(angle)
            points.append((x,.07+y,hat_z(math.hypot(x,y))))
        tube(points,.0025,gold)
    # Swept ribs connect the crown and ornamental band.
    points=[]
    for i in range(17):
        t=i/16; r=.082+.145*t; a=angle+.10*math.sin(t*math.pi)
        points.append((r*math.cos(a),.07+r*math.sin(a),hat_z(r)))
    tube(points,.0018,gold)
for side in (-1,1):
    x=side*.295;y=-.085
    tube([(x+.012*math.cos(i*math.tau/24),y,1.733+.018*math.sin(i*math.tau/24)) for i in range(25)],.0025,gold)
    tube([(x,y,1.760),(x,y,1.751)],.0025,gold)
    tube([(x,y,1.715),(x,y,1.685)],.0025,gold)
    start=len(vertices)
    vertices.extend([(x+dx,y+dy,z) for z in (1.685,1.600) for dy in (-.003,.003) for dx in (-.012,.012)])
    for f in ((0,1,3,2),(4,6,7,5),(0,4,5,1),(2,3,7,6),(0,2,6,4),(1,5,7,3)):
        faces.append(tuple(start+i for i in f));colors.append(gold)
    # Inlaid dark seal on both faces of the tag, with a gold border.
    for face_y in (y-.0034,y+.0034):
        for row in range(4):
            z=1.671-row*.016
            tube([(x-.006,face_y,z),(x+.006,face_y,z)],.0014,navy)
        tube([(x,face_y,1.674),(x,face_y,1.615)],.0012,navy)
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
