import bpy
import bmesh
import math
from pathlib import Path
from mathutils import Vector, Quaternion

import argparse
import sys

parser = argparse.ArgumentParser(description="Build the draft arrival outfit from the licensed source FBX.")
parser.add_argument("--staging", type=Path, default=Path(__file__).resolve().parents[2] / "build/asset-staging/quaternius-base")
parser.add_argument("--variant", choices=("arrival", "keeper"), default="arrival")
args = parser.parse_args(sys.argv[sys.argv.index("--") + 1:] if "--" in sys.argv else [])
ROOT = args.staging.resolve()
keeper = args.variant == "keeper"
OUT = ROOT / "keeper" if keeper else ROOT
OUT.mkdir(parents=True, exist_ok=True)
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.fbx(filepath=str(ROOT / "Superhero_Male_FullBody.fbx"))
rig = bpy.data.objects["Armature"]
rig.rotation_euler.z = 0
body = bpy.data.objects["SuperHero_Male"]

def material(name, color, texture=None):
    mat = bpy.data.materials.new(name)
    mat.diffuse_color = (*color, 1)
    mat.use_nodes = True
    shader = mat.node_tree.nodes.get("Principled BSDF")
    shader.inputs["Base Color"].default_value = (*color, 1)
    shader.inputs["Roughness"].default_value = .8
    if texture:
        node = mat.node_tree.nodes.new("ShaderNodeTexImage")
        node.image = bpy.data.images.load(str(ROOT / texture))
        mat.node_tree.links.new(node.outputs["Color"], shader.inputs["Base Color"])
    return mat

cloth = material("Keeper gray robe" if keeper else "Arrival unbleached cloth", (.28, .29, .28) if keeper else (.65, .63, .57))
sash = material("Keeper antique gold" if keeper else "Arrival muted blue sash", (.42, .34, .19) if keeper else (.09, .15, .19))
pants = material("Arrival charcoal trousers", (.06, .065, .06))
hair = material("Keeper silver hair" if keeper else "Arrival black hair", (.68, .70, .69) if keeper else (.012, .014, .018))
skin = material("Base skin", (.7, .5, .35), "T_Superhero_Male_Ligh.png")
eyes = material("Base eyes", (.4, .3, .2), "T_Eye_Brown.png")
body.data.materials.clear()
body.data.materials.append(skin)
for obj in list(bpy.context.scene.objects):
    if obj.type == "MESH" and obj != body:
        obj.data.materials.clear()
        obj.data.materials.append(eyes if obj.name == "Eyes" else hair)
        for face in obj.data.polygons:
            face.material_index = 0

def garment(name, select, mat, expand):
    obj = body.copy()
    obj.data = body.data.copy()
    obj.name = name
    bpy.context.collection.objects.link(obj)
    bm = bmesh.new()
    bm.from_mesh(obj.data)
    bmesh.ops.delete(bm, geom=[f for f in bm.faces if not select(f.calc_center_median())], context="FACES")
    bm.normal_update()
    for v in bm.verts:
        v.co += v.normal * expand
    bm.to_mesh(obj.data)
    bm.free()
    obj.data.materials.clear()
    obj.data.materials.append(mat)
    for face in obj.data.polygons:
        face.material_index = 0
        face.use_smooth = True
    return obj

garment("Arrival trousers", lambda p: .12 < p.z < 1.02, pants, .018)
garment("Arrival calf wraps", lambda p: .10 < p.z < .39, cloth, .024)

def mesh(name, vertices, faces, mat, bone="pelvis"):
    data = bpy.data.meshes.new(name)
    data.from_pydata(vertices, [], faces)
    data.update()
    obj = bpy.data.objects.new(name, data)
    bpy.context.collection.objects.link(obj)
    data.materials.append(mat)
    for face in data.polygons:
        face.use_smooth = True
    obj.parent = rig
    group = obj.vertex_groups.new(name=bone)
    group.add(list(range(len(vertices))), 1, "REPLACE")
    obj.modifiers.new("Shared body rig", "ARMATURE").object = rig
    return obj

def ring(name, levels, mat, start=0, end=math.tau, segments=32, bone="pelvis"):
    verts = []
    for z, rx, ry in levels:
        for i in range(segments + 1):
            a = start + (end - start) * i / segments
            verts.append((rx * math.sin(a), .03 - ry * math.cos(a), z))
    faces = []
    for row in range(len(levels) - 1):
        for i in range(segments):
            a = row * (segments + 1) + i
            face=(a, a+1, a+segments+2, a+segments+1)
            p,q,r=(Vector(verts[j]) for j in face[:3])
            radial=Vector((p.x,p.y-.03,0))
            if (q-p).cross(r-p).dot(radial)<0:
                face=tuple(reversed(face))
            faces.append(face)
    return mesh(name, verts, faces, mat, bone)

def tailored_tunic():
    obj = body.copy()
    obj.data = body.data.copy()
    obj.name = "Arrival continuous tunic"
    bpy.context.collection.objects.link(obj)
    bm = bmesh.new()
    bm.from_mesh(obj.data)
    # Exact plane cuts retain interpolated deform weights at the hem, neck and cuffs.
    cuff = .67 if keeper else .495
    for point, normal in (((0,0,1.045),(0,0,-1)), ((0,0,1.555),(0,0,1)),
                          ((cuff,0,0),(1,0,0)), ((-cuff,0,0),(-1,0,0))):
        bmesh.ops.bisect_plane(bm, geom=list(bm.verts)+list(bm.edges)+list(bm.faces),
                              plane_co=point, plane_no=normal, dist=.00001, clear_outer=True)
    for _ in range(8):
        bmesh.ops.smooth_vert(bm, verts=list(bm.verts), factor=.45, use_axis_x=True, use_axis_y=True, use_axis_z=True)
    bm.normal_update()
    for v in bm.verts:
        v.co += v.normal * .032
    bm.to_mesh(obj.data)
    bm.free()
    obj.data.materials.clear()
    obj.data.materials.append(cloth)
    for face in obj.data.polygons:
        face.material_index = 0
        face.use_smooth = True
    return obj

tunic=tailored_tunic()
from mathutils.bvhtree import BVHTree
collar_surface=BVHTree.FromPolygons([v.co for v in tunic.data.vertices],[list(p.vertices) for p in tunic.data.polygons])
body_surface=BVHTree.FromPolygons([v.co for v in body.data.vertices],[list(p.vertices) for p in body.data.polygons])
bm=bmesh.new()
bm.from_mesh(tunic.data)
slope=.43
for normal in ((1,0,-slope),(-1,0,-slope),(0,1,0)):
    bmesh.ops.bisect_plane(bm,geom=list(bm.verts)+list(bm.edges)+list(bm.faces),
                          plane_co=(0,0,1.335),plane_no=normal,dist=.00001)
opening=[f for f in bm.faces if f.calc_center_median().y<0
         and abs(f.calc_center_median().x)<slope*(f.calc_center_median().z-1.335)]
assert opening, "No V neckline faces selected"
bmesh.ops.delete(bm,geom=opening,context="FACES")
bm.to_mesh(tunic.data)
bm.free()

for side,suffix in ((1,"l"),(-1,"r")):
    vertices=[]
    for i in range(12):
        z=1.34+(1.548-1.34)*i/11
        for edge in (0,.024):
            x=side*(slope*(z-1.335)+edge)
            hit=collar_surface.ray_cast(Vector((x,-1,z)),Vector((0,1,0)),2)[0]
            if hit is None:
                hit=body_surface.ray_cast(Vector((x,-1,z)),Vector((0,1,0)),2)[0]
            assert hit is not None, "Collar ray missed its supporting surface"
            vertices.append((x,hit.y-.006,z))
    faces=[(i*2,i*2+1,i*2+3,i*2+2) for i in range(11)]
    mesh("Arrival fitted collar "+suffix,vertices,faces,sash,"spine_03")

vertices=[]
for i in range(9):
    z=1.337+(1.49-1.337)*i/8
    width=slope*(z-1.335)+.003
    for x in (-width,0,width):
        hit=body_surface.ray_cast(Vector((x,-1,z)),Vector((0,1,0)),2)[0]
        assert hit is not None, "Inner collar missed body surface"
        vertices.append((x,hit.y-.022,z))
faces=[]
for row in range(8):
    for col in range(2):
        a=row*3+col
        faces.append((a,a+1,a+4,a+3))
mesh("Arrival inner wrap",vertices,faces,cloth if keeper else sash,"spine_03")

for side, suffix in ((1,"l"),(-1,"r")):
    wrap=body.copy()
    wrap.data=body.data.copy()
    wrap.name="Arrival wrist wrap "+suffix
    bpy.context.collection.objects.link(wrap)
    bm=bmesh.new()
    bm.from_mesh(wrap.data)
    for x, normal in ((.51,-side),(.69,side)):
        bmesh.ops.bisect_plane(bm,geom=list(bm.verts)+list(bm.edges)+list(bm.faces),
                              plane_co=(side*x,0,0),plane_no=(normal,0,0),dist=.00001,clear_outer=True)
    bm.normal_update()
    for vertex in bm.verts:
        vertex.co += vertex.normal*.012
    bm.to_mesh(wrap.data)
    bm.free()
    wrap.data.materials.clear()
    wrap.data.materials.append(cloth)
    for face in wrap.data.polygons:
        face.material_index=0
        face.use_smooth=True
        radial=Vector((0,face.center.y-.065,face.center.z-1.455))
        assert face.normal.dot(radial)>0, "Wrist wrap face points inward: "+suffix

ring("Arrival waist sash", [(1.0,.235,.195),(1.045,.235,.195),(1.10,.235,.195)], pants if keeper else sash)
ring("Arrival split coat", [(1.01,.23,.185),(.85,.265,.195),(.65,.28,.20),(.27 if keeper else .46,.30,.215)], cloth,
     start=.12, end=math.tau-.12)
mesh("Arrival back sash tails", [(-.07,.232,1.02),(.06,.232,1.02),(.08,.26,.53),(-.04,.26,.51)], [(0,1,2,3)], pants if keeper else sash)
mesh("Arrival front sash tails", [(.11,-.18,1.02),(.18,-.15,1.02),(.23,-.22,.70),(.15,-.24,.68)], [(0,1,2,3)], pants if keeper else sash)

# Hair is a volume study, not final hair cards or a production hairstyle.
def ellipsoid(name, location, scale, mat):
    bpy.ops.mesh.primitive_uv_sphere_add(segments=16, ring_count=8, location=location)
    obj = bpy.context.object
    obj.name = name
    obj.scale = scale
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    obj.data.materials.append(mat)
    for face in obj.data.polygons:
        face.use_smooth = True
    obj.parent = rig
    obj.vertex_groups.new(name="Head").add(list(range(len(obj.data.vertices))), 1, "REPLACE")
    obj.modifiers.new("Shared head rig", "ARMATURE").object = rig
    return obj

ellipsoid("Arrival hair crown", (0,.037,1.754), (.094,.095,.08), hair)
def hair_lock(name, points, widths):
    vertices=[]
    for (x,y,z),width in zip(points,widths):
        for i in range(6):
            angle=math.tau*i/6
            vertices.append((x+width*math.cos(angle),y+width*.45*math.sin(angle),z))
    faces=[]
    for row in range(len(points)-1):
        for i in range(6):
            a=row*6+i
            b=row*6+(i+1)%6
            faces.append((a,b,b+6,a+6))
    return mesh(name,vertices,faces,hair,"Head")

for i in range(7):
    spread=(i-3)*.014
    hair_lock("Arrival ponytail lock "+str(i),
              [(spread*.15,.115,1.79),(spread,.17,1.69),(spread*1.25,.19,1.56),(spread*.9,.17,1.40),(spread*.6,.15,1.35+abs(spread))],
              [.017,.02,.019,.012,.001])
for side in (-1,1):
    hair_lock("Arrival temple lock "+str(side),
              [(side*.048,-.01,1.79),(side*.074,-.065,1.73),(side*.085,-.09,1.64),(side*.086,-.077,1.58)],
              [.024,.022,.014,.001])
ellipsoid("Arrival hair tie", (0,.095,1.795), (.044,.046,.044), sash)
if keeper:
    ellipsoid("Keeper topknot", (0,.047,1.837), (.037,.041,.04), hair)
    for i in range(5):
        x = (i-2)*.011
        hair_lock("Keeper beard " + str(i),
                  [(x,-.083,1.625),(x*1.2,-.125,1.575),(x*.8,-.15,1.50),(x*.25,-.16,1.435+abs(x))],
                  [.016,.018,.012,.001])

for side, suffix in ((1,"l"),(-1,"r")):
    bm=bmesh.new()
    for v in body.data.vertices:
        if v.co.z < .15 and v.co.x*side > 0:
            bm.verts.new(v.co)
    hull=bmesh.ops.convex_hull(bm,input=list(bm.verts),use_existing_faces=False)
    bmesh.ops.delete(bm,geom=list(set(hull["geom_unused"]+hull["geom_interior"])),context="VERTS")
    bm.normal_update()
    for v in bm.verts:
        v.co += v.normal*.012
    bm.verts.ensure_lookup_table()
    bm.verts.index_update()
    shoe=mesh("Arrival cloth shoe "+suffix,[tuple(v.co) for v in bm.verts],
              [tuple(v.index for v in f.verts) for f in bm.faces],pants,"foot_"+suffix)
    bm.free()

# Fit the sash to actual garment surfaces instead of a detached circular cylinder.
from mathutils.bvhtree import BVHTree
surfaces=[]
for name in ("Arrival continuous tunic","Arrival split coat",body.name):
    data=bpy.data.objects[name].data
    surfaces.append(BVHTree.FromPolygons([v.co for v in data.vertices],[list(p.vertices) for p in data.polygons]))
waist=bpy.data.objects["Arrival waist sash"]
envelope=[]
for v in waist.data.vertices:
    radial=Vector((v.co.x,v.co.y-.03,0)).normalized()
    center=Vector((0,.03,v.co.z))
    hits=[tree.ray_cast(center+radial,-radial,1.0)[0] for tree in surfaces]
    distances=[(hit-center).dot(radial) for hit in hits if hit is not None]
    assert distances, "Sash fit ray missed the garment and body"
    envelope.append(max(distances)+.012)
for index,v in enumerate(waist.data.vertices):
    radial=Vector((v.co.x,v.co.y-.03,0)).normalized()
    # Span the tunic/coat seam instead of following bare skin through their join.
    height=(v.co.z-1.0)/.10
    bottom=envelope[index%33]
    top=envelope[66+index%33]
    radius=max(envelope[index],bottom+(top-bottom)*height)
    v.co=Vector((0,.03,v.co.z))+radial*radius
for index, v in enumerate(waist.data.vertices):
    distance = (v.co - Vector((0, .03, v.co.z))).length
    assert distance >= envelope[index] - .001, "Sash penetrates its fitted surface: " + str(index)
    if index < 33 or index >= 66:
        assert abs(distance - envelope[index]) < .001, "Sash edge floats above garment: " + str(index)

# A wrapped sash spans concave seams; the convex support envelope removes those inward notches.
bm=bmesh.new()
for v in waist.data.vertices:
    bm.verts.new(v.co)
hull=bmesh.ops.convex_hull(bm,input=list(bm.verts),use_existing_faces=False)
bmesh.ops.delete(bm,geom=list(set(hull["geom_unused"]+hull["geom_interior"])),context="VERTS")
caps=[face for face in bm.faces if max(v.co.z for v in face.verts)-min(v.co.z for v in face.verts)<1e-5]
bmesh.ops.delete(bm,geom=caps,context="FACES_ONLY")
bm.normal_update()
for face in bm.faces:
    face.smooth=True
bm.to_mesh(waist.data)
bm.free()
waist.vertex_groups.clear()
waist.vertex_groups.new(name="pelvis").add(list(range(len(waist.data.vertices))),1.0,"REPLACE")

# Transfer leg deformation to the coat instead of leaving its entire skirt rigid on the pelvis.
from mathutils.kdtree import KDTree
tree = KDTree(len(body.data.vertices))
for v in body.data.vertices:
    tree.insert(v.co, v.index)
tree.balance()
for obj in (bpy.data.objects["Arrival split coat"], bpy.data.objects["Arrival front sash tails"], bpy.data.objects["Arrival back sash tails"],
            bpy.data.objects["Arrival fitted collar l"],bpy.data.objects["Arrival fitted collar r"],bpy.data.objects["Arrival inner wrap"]):
    # The stationary elder's long robe hangs from the hips, not each calf independently.
    if keeper and obj.name == "Arrival split coat":
        continue
    obj.vertex_groups.clear()
    groups = [obj.vertex_groups.new(name=g.name) for g in body.vertex_groups]
    for v in obj.data.vertices:
        _, index, _ = tree.find(v.co)
        influences=sorted(body.data.vertices[index].groups,key=lambda g:g.weight,reverse=True)[:4]
        total=sum(g.weight for g in influences)
        assert total > 0, "Source body vertex has no deform weight"
        for g in influences:
            groups[g.group].add([v.index],g.weight/total,"REPLACE")

# Reduce the broad heroic silhouette in both mesh and rest skeleton together.
for obj in list(bpy.context.scene.objects):
    if obj.type == "MESH":
        for v in obj.data.vertices:
            v.co.x *= .86
bpy.context.view_layer.objects.active = rig
rig.select_set(True)
bpy.ops.object.mode_set(mode="EDIT")
# Connected endpoints update their neighbours, so never scale their live coordinates in-place.
rest_positions = [(bone, bone.head.copy(), bone.tail.copy()) for bone in rig.data.edit_bones]
for bone, head, tail in rest_positions:
    head.x *= .86
    tail.x *= .86
    bone.head = head
    bone.tail = tail
for bone, head, tail in rest_positions:
    assert (bone.head - head).length < 1e-6 and (bone.tail - tail).length < 1e-6, "Rig width transform drift: " + bone.name
bpy.ops.object.mode_set(mode="OBJECT")
bpy.ops.object.select_all(action="DESELECT")
export_parts=[]
for obj in list(bpy.context.scene.objects):
    if obj.type=="MESH":
        part=obj.copy()
        part.data=obj.data.copy()
        bpy.context.collection.objects.link(part)
        part.select_set(True)
        export_parts.append(part)
bpy.context.view_layer.objects.active=export_parts[0]
bpy.ops.object.join()
export_mesh=bpy.context.object
export_mesh.name="Arrival outfit candidate"
for v in export_mesh.data.vertices:
    weights=sorted([(g.group,g.weight) for g in v.groups if g.weight>0],key=lambda g:g[1],reverse=True)[:4]
    total=sum(weight for _,weight in weights)
    assert total>0, "Unweighted export vertex"
    for group in [g.group for g in v.groups]:
        export_mesh.vertex_groups[group].remove([v.index])
    for group,weight in weights:
        export_mesh.vertex_groups[group].add([v.index],weight/total,"REPLACE")
rig.select_set(True)
bpy.ops.export_scene.fbx(filepath=str(OUT / ("keeper-candidate.fbx" if keeper else "arrival-outfit-candidate.fbx")),use_selection=True,
                         object_types={"ARMATURE","MESH"},add_leaf_bones=False,bake_anim=False,
                         axis_forward="-Z",axis_up="Y",path_mode="STRIP")
bpy.data.objects.remove(export_mesh,do_unlink=True)
for side, angle in (("l", -65), ("r", 65)):
    bone = rig.pose.bones["upperarm_" + side]
    bone.rotation_mode = "XYZ"
    bone.rotation_euler.z = math.radians(angle)

bpy.ops.wm.save_as_mainfile(filepath=str(OUT / "arrival-outfit-study.blend"))
scene = bpy.context.scene
scene.render.engine = "CYCLES"
scene.cycles.samples = 24
scene.render.resolution_x = 720
scene.render.resolution_y = 960
scene.render.resolution_percentage = 100
scene.world = bpy.data.worlds.new("Study world")
scene.world.use_nodes = True
scene.world.node_tree.nodes["Background"].inputs[0].default_value = (.32,.35,.38,1)
scene.world.node_tree.nodes["Background"].inputs[1].default_value = .6
bpy.ops.object.light_add(type="AREA", location=(2,-3,4))
bpy.context.object.data.energy = 350
bpy.context.object.data.shape = "DISK"
bpy.context.object.data.size = 4
bpy.ops.object.camera_add(location=(2.5,-4,2))
camera = bpy.context.object
camera.data.type = "ORTHO"
camera.data.ortho_scale = 2.15
scene.camera = camera
for name, location in (("front",(0,-4,1.4)),("rear",(0,4,1.4))):
    camera.location = location
    camera.rotation_euler = (Vector((0,0,.93))-camera.location).to_track_quat("-Z","Y").to_euler()
    scene.render.filepath = str(OUT / ("outfit-" + name + ".png"))
    bpy.ops.render.render(write_still=True)

if keeper:
    scene.render.resolution_x = scene.render.resolution_y = 128
    scene.render.film_transparent = True
    scene.render.image_settings.color_mode = "RGBA"
    camera.data.ortho_scale = .48
    camera.location = (0,-4,1.66)
    camera.rotation_euler = (Vector((0,0,1.66))-camera.location).to_track_quat("-Z","Y").to_euler()
    scene.render.filepath = str(OUT / "keeper-portrait.png")
    bpy.ops.render.render(write_still=True)
    scene.render.film_transparent = False
    scene.render.resolution_x, scene.render.resolution_y = 720, 960
    camera.data.ortho_scale = 2.15

for name, angle in (("thigh_l",-25),("thigh_r",20),("calf_l",15),("calf_r",35)):
    bone=rig.pose.bones[name]
    basis=rig.data.bones[name].matrix_local.to_quaternion()
    bone.rotation_mode="QUATERNION"
    bone.rotation_quaternion=basis.inverted() @ Quaternion((1,0,0),math.radians(angle)) @ basis
camera.location=(2.6,-4,1.5)
camera.rotation_euler=(Vector((0,0,.93))-camera.location).to_track_quat("-Z","Y").to_euler()
scene.render.filepath=str(OUT / "outfit-stride.png")
bpy.ops.render.render(write_still=True)

coat=bpy.data.objects["Arrival split coat"]
missing=sum(not v.groups for v in coat.data.vertices)
assert missing == 0, "Coat has unweighted vertices"
assert all(abs(sum(g.weight for g in v.groups)-1)<.001 for v in coat.data.vertices), "Coat weights not normalized"
for name in ("Arrival split coat","Arrival waist sash"):
    obj=bpy.data.objects[name]
    obj.data.update()
    assert all(p.normal.dot(Vector((p.center.x,p.center.y-.03,0)))>0 for p in obj.data.polygons), name+" has inward faces"
print("LGO_OUTFIT_STUDY weights_normalized=true outward_coat_and_sash=true coat_vertices="+str(len(coat.data.vertices)))

# Render at the blockout camera's vertical field of view and follow distance, not portrait scale.
for bone in rig.pose.bones:
    if bone.name.startswith(("thigh_","calf_")):
        bone.rotation_mode="QUATERNION"
        bone.rotation_quaternion=Quaternion()
camera.data.type="PERSP"
camera.data.sensor_fit="VERTICAL"
camera.data.sensor_height=24
camera.data.lens=24/(2*math.tan(math.radians(55)/2))
camera.location=(0,6.8,3.14)
camera.rotation_euler=(Vector((0,0,1.14))-camera.location).to_track_quat("-Z","Y").to_euler()
scene.render.resolution_x=960
scene.render.resolution_y=540
scene.render.filepath=str(OUT / "outfit-camera-mobile.png")
bpy.ops.render.render(write_still=True)
from bpy_extras.object_utils import world_to_camera_view
deps=bpy.context.evaluated_depsgraph_get()
projected=[]
triangles=0
for obj in scene.objects:
    if obj.type=="MESH":
        obj.data.calc_loop_triangles()
        triangles+=len(obj.data.loop_triangles)
        evaluated=obj.evaluated_get(deps)
        data=evaluated.to_mesh()
        projected.extend(world_to_camera_view(scene,camera,evaluated.matrix_world @ v.co).y for v in data.vertices)
        evaluated.to_mesh_clear()
height=max(projected)-min(projected)
assert .20 <= height <= .25, "Camera-scale study outside design framing: "+str(height)
print("LGO_OUTFIT_CAMERA_STUDY height_fraction="+str(height)+" triangles="+str(triangles)+" viewport=960x540")
