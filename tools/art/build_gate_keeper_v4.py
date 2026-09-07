"""Build a simplified young guide candidate against the approved v4 reference.

Export to staging; review geometry, poses and matching portrait before Unity ingest.
Reference: story design reference pack v0.1, 02_gate_keeper_character_sheet.png.
"""

import argparse
import math
from pathlib import Path
import sys

import bpy
import bmesh
from mathutils import Vector
from mathutils.bvhtree import BVHTree

sys.path.insert(0, str(Path(__file__).parent))
sys.dont_write_bytecode = True
import json
import hashlib

parser = argparse.ArgumentParser()
parser.add_argument("--source", type=Path, required=True)
parser.add_argument("--output", type=Path, required=True)
args = parser.parse_args(sys.argv[sys.argv.index("--") + 1:])
if args.output.exists():
    raise ValueError("Use a new staging directory; preserve earlier source and reviews.")
bpy.ops.wm.open_mainfile(filepath=str(args.source.resolve()))
rig = bpy.data.objects["Armature"]
body = bpy.data.objects["SuperHero_Male"]
if any(o.name.startswith("Keeper beard") for o in bpy.context.scene.objects):
    raise ValueError("Use the arrival source, not the retired elder variant.")
rest_matrices = {b.name: tuple(v for row in b.matrix_local for v in row) for b in rig.data.bones}
for bone in rig.pose.bones:
    bone.matrix_basis.identity()

def material(name, color, metallic=0):
    mat = bpy.data.materials.get(name) or bpy.data.materials.new(name)
    mat.use_nodes = True
    mat.diffuse_color = (*color, 1)
    shader = mat.node_tree.nodes.get("Principled BSDF")
    shader.inputs["Base Color"].default_value = (*color, 1)
    shader.inputs["Metallic"].default_value = metallic
    shader.inputs["Roughness"].default_value = .5 if metallic else .78
    return mat

ivory = material("Keeper ivory cloth", (.76, .77, .72))
ink = material("Keeper ink cloth", (.012, .027, .06))
gold = material("Keeper warm brass", (.52, .32, .10), .65)

# Assign the new outfit without modifying shared arrival material assets.
for obj in bpy.context.scene.objects:
    if obj.type == "MESH":
        for slot in obj.material_slots:
            if slot.material.name == "Arrival unbleached cloth":
                slot.material = ivory
            elif slot.material.name == "Arrival muted blue sash":
                slot.material = ink
            elif slot.material.name == "Arrival charcoal trousers":
                slot.material = bpy.data.materials["Arrival black hair"]

def mesh(name, vertices, faces, mat, bone):
    data = bpy.data.meshes.new(name)
    data.from_pydata(vertices, [], faces)
    data.update()
    obj = bpy.data.objects.new(name, data)
    bpy.context.collection.objects.link(obj)
    data.materials.append(mat)
    obj.parent = rig
    obj.vertex_groups.new(name=bone).add(list(range(len(vertices))), 1, "REPLACE")
    obj.modifiers.new("Shared guide rig", "ARMATURE").object = rig
    return obj

def shell(name, levels, mat, bone, start=0, end=math.tau, segments=64):
    vertices = [(rx * math.sin(a), .03 - ry * math.cos(a), z)
                for z, rx, ry in levels
                for a in (start + (end - start) * i / segments for i in range(segments + 1))]
    faces = []
    for row in range(len(levels) - 1):
        for col in range(segments):
            a = row * (segments + 1) + col
            face = (a, a + 1, a + segments + 2, a + segments + 1)
            p, q, r = (Vector(vertices[i]) for i in face[:3])
            if (q - p).cross(r - p).dot(Vector((p.x, p.y - .03, 0))) < 0:
                face = tuple(reversed(face))
            faces.append(face)
    return mesh(name, vertices, faces, mat, bone)

# Each garment is authored in the arrival skeleton's unchanged rest space.
# Independent rails define the silhouette and overlapping layers before skinning.
def solidify(obj):
    obj["outer_face_count"] = len(obj.data.polygons)
    bpy.context.view_layer.objects.active = obj
    thickness = obj.modifiers.new("Garment thickness", "SOLIDIFY")
    thickness.thickness = .003
    thickness.offset = -1
    bpy.ops.object.modifier_apply(modifier=thickness.name)


def cape_support(obj):
    obj.vertex_groups.clear()
    pelvis = obj.vertex_groups.new(name="pelvis")
    spine = obj.vertex_groups.new(name="spine_01")
    for vertex in obj.data.vertices:
        blend = max(0, min(1, (vertex.co.z - 1.05) / .44))
        pelvis.add([vertex.index], 1-blend, "REPLACE")
        spine.add([vertex.index], blend, "REPLACE")


def garment_panel(name, rails, mat, support="pelvis", trim=True):
    vertices = []
    columns = 12
    for left, right in rails:
        for col in range(columns + 1):
            t = col / columns
            point = Vector(left).lerp(Vector(right), t)
            # A shallow fold, shared by all layers, avoids a rigid flat sheet.
            point.y += math.sin(t * math.pi * 4) * .008
            vertices.append(tuple(point))
    faces = []
    for row in range(len(rails) - 1):
        for col in range(columns):
            i = row * (columns + 1) + col
            faces.append((i, i + 1, i + columns + 2, i + columns + 1))
    desired = Vector((0, 1 if "back cape" in name else -1, 0))
    for i, face in enumerate(faces):
        p, q, r = (Vector(vertices[j]) for j in face[:3])
        if (q-p).cross(r-p).dot(desired) < 0:
            faces[i] = tuple(reversed(face))
    obj = mesh(name, vertices, faces, mat, support)
    assert all(f.normal.dot(desired) > 0 for f in obj.data.polygons), name
    if support == "spine_01":
        cape_support(obj)
    for face in obj.data.polygons:
        face.use_smooth = True
    if trim:
        for edge in (0, columns):
            points = []
            for row in range(len(rails)):
                outer = Vector(vertices[row * (columns + 1) + edge])
                inner = Vector(vertices[row * (columns + 1) + (1 if edge == 0 else columns - 1)])
                inset = outer.lerp(inner, .36)
                outer.y -= .003
                inset.y -= .003
                points.extend((tuple(outer), tuple(inset)))
            piping = mesh(name + " piping", points,
                          [(i*2,i*2+1,i*2+3,i*2+2) for i in range(len(rails)-1)], gold, support)
            if support == "spine_01":
                cape_support(piping)
            solidify(piping)
    solidify(obj)
    return obj

for name in ("Arrival split coat", "Arrival back sash tails", "Arrival front sash tails"):
    bpy.data.objects.remove(bpy.data.objects[name], do_unlink=True)

# Front wrap ends above the ankle: boots and the route remain readable.
garment_panel("Keeper front tabard", [
    ((-.15,-.175,1.05),(.13,-.175,1.05)),
    ((-.16,-.20,.86),(.14,-.20,.86)),
    ((-.18,-.22,.66),(.16,-.22,.66)),
    ((-.20,-.245,.43),(.18,-.245,.43)),
    ((-.17,-.25,.36),(.16,-.25,.38))], ivory)
for side, suffix in ((1,"l"),(-1,"r")):
    garment_panel("Keeper side panel " + suffix, [
        ((side*.14,-.17,1.05),(side*.22,.07,1.05)),
        ((side*.18,-.19,.87),(side*.25,.10,.87)),
        ((side*.23,-.21,.66),(side*.31,.14,.66)),
        ((side*.30,-.23,.42),(side*.39,.20,.38)),
        ((side*.29,-.22,.31),(side*.37,.22,.26))], ink)
    garment_panel("Keeper back cape " + suffix, [
        ((side*.025,.17,1.49),(side*.22,.15,1.49)),
        ((side*.025,.23,1.30),(side*.23,.23,1.30)),
        ((side*.025,.255,1.06),(side*.25,.27,1.06)),
        ((side*.035,.29,.82),(side*.31,.31,.82)),
        ((side*.065,.34,.55),(side*.40,.36,.53)),
        ((side*.11,.39,.26),(side*.46,.41,.18))], ink, "spine_01")

# Torso is a separate garment slot, with arm openings under the sleeve overlap.
tunic = bpy.data.objects["Arrival continuous tunic"]
bm = bmesh.new()
bm.from_mesh(tunic.data)
for x, normal in ((.23, (1,0,0)),(-.23,(-1,0,0))):
    bmesh.ops.bisect_plane(bm, geom=list(bm.verts)+list(bm.edges)+list(bm.faces),
                          plane_co=(x,0,0),plane_no=normal,dist=.00001,clear_outer=True)
bm.to_mesh(tunic.data)
bm.free()

# Continuous sleeves overlap the torso and cover the old cuff gap.
tunic = bpy.data.objects["Arrival continuous tunic"]
for side, suffix in ((1,"l"),(-1,"r")):
    old = bpy.data.objects.get("Arrival wrist wrap " + suffix)
    if old:
        bpy.data.objects.remove(old, do_unlink=True)
    vertices = []
    levels = ((.19,.115),(.25,.12),(.32,.13),(.39,.13),(.45,.12),(.50,.10),(.55,.074),(.605,.062))
    for x, radius in levels:
        for col in range(25):
            angle = math.tau * col / 24
            vertices.append((side*x, .069 + math.cos(angle)*radius*.86,
                             1.4555 + math.sin(angle)*radius))
    faces = []
    for row in range(len(levels)-1):
        for col in range(24):
            i = row*25+col
            face = (i,i+1,i+26,i+25)
            faces.append(tuple(reversed(face)) if side == -1 else face)
    sleeve = mesh("Keeper continuous sleeve " + suffix, vertices, faces, ivory, "upperarm_" + suffix)
    sleeve.data.materials.append(ink)
    sleeve.data.materials.append(gold)
    sleeve.vertex_groups.clear()
    upper = sleeve.vertex_groups.new(name="upperarm_" + suffix)
    lower = sleeve.vertex_groups.new(name="lowerarm_" + suffix)
    for vertex in sleeve.data.vertices:
        blend = max(0,min(1,(abs(vertex.co.x)-.34)/.115))
        upper.add([vertex.index],1-blend,"REPLACE")
        lower.add([vertex.index],blend,"REPLACE")
    for face in sleeve.data.polygons:
        row = face.index//24
        face.use_smooth = True
        face.material_index = 1 if row >= 5 else 0
        radial = Vector((0,face.center.y-.069,face.center.z-1.4555))
        assert face.normal.dot(radial)>0, "Sleeve normals point inward"
    # Shoulder cap is an explicit curved pattern with a straight hem.
    vertices = []
    for x, radius in ((.125,.11),(.18,.145),(.295,.145),(.315,.13)):
        for col in range(17):
            angle = -.18 + (math.pi+.36)*col/16
            vertices.append((side*x,.068+math.cos(angle)*radius,1.4555+math.sin(angle)*radius))
    faces = []
    for row in range(3):
        for col in range(16):
            i=row*17+col
            face=(i,i+1,i+18,i+17)
            faces.append(tuple(reversed(face)) if side == -1 else face)
    guard=mesh("Keeper shoulder guard " + suffix,vertices,faces,ink,"upperarm_" + suffix)
    guard.data.materials.append(gold)
    for face in guard.data.polygons:
        face.use_smooth=True
        if face.index//16 == 2:
            face.material_index=1
    solidify(guard)

# Tailored front panels follow the actual torso surface and retain its weights.
surface = BVHTree.FromPolygons([v.co for v in tunic.data.vertices], [list(p.vertices) for p in tunic.data.polygons])
for side in (1,):
    vertices = []
    for row in range(16):
        t = row / 15
        z = 1.08 + t * .41
        center = (.10 - .22 * t) if side == 1 else (-.13 + .02 * t)
        for edge in (-1, 1):
            x = center + edge * (.033 if side == 1 else .016)
            hit = surface.ray_cast(Vector((x,-1,z)), Vector((0,1,0)), 2)[0]
            if hit is None:
                raise ValueError("Front panel does not meet the fitted tunic.")
            vertices.append((x,hit.y-.012,z))
    faces = [(i*2,i*2+1,i*2+3,i*2+2) for i in range(15)]
    panel = mesh("Keeper front lapel", vertices, faces, ink, "spine_02")
    # Transfer only torso support here; leg remapping is inappropriate above the belt.
    from mathutils.kdtree import KDTree
    tree = KDTree(len(tunic.data.vertices))
    for vertex in tunic.data.vertices:
        tree.insert(vertex.co, vertex.index)
    tree.balance()
    panel.vertex_groups.clear()
    for vertex in panel.data.vertices:
        _, index, _ = tree.find(vertex.co)
        for influence in tunic.data.vertices[index].groups:
            name = tunic.vertex_groups[influence.group].name
            group = panel.vertex_groups.get(name) or panel.vertex_groups.new(name=name)
            group.add([vertex.index], influence.weight, "REPLACE")

# Belt seal and hanging cloth are separate head/waist accessories, no gameplay stats.
def seal(name, x, y, z, radius, bone):
    vertices=[]
    for r in (radius,radius*.72):
        for i in range(24):
            angle=math.tau*i/24
            vertices.append((x+math.sin(angle)*r,y,z+math.cos(angle)*r))
    faces=[(i,(i+1)%24,(i+1)%24+24,i+24) for i in range(24)]
    ring = mesh(name,vertices,[tuple(reversed(face)) for face in faces],gold,bone)
    assert all(face.normal.y < -.99 for face in ring.data.polygons), "Seal must face forward"
    solidify(ring)
    inset = mesh(name+" inset",[(x,y-.003,z+radius*.48),(x+radius*.35,y-.003,z),
         (x,y-.003,z-radius*.48),(x-radius*.35,y-.003,z)],[(3,2,1,0)],gold,bone)
    assert all(face.normal.y < -.99 for face in inset.data.polygons), "Seal inset must face forward"
    solidify(inset)
seal("Keeper waist seal",0,-.202,1.045,.065,"pelvis")
for side in (-1,1):
    x=side*.11
    garment_panel("Keeper belt ribbon",[
        ((x-.012,-.205,1.03),(x+.012,-.205,1.03)),
        ((x-.018,-.23,.89),(x+.01,-.23,.89)),
        ((x-.016,-.24,.79),(x+.008,-.24,.80))],gold,trim=False)

# A closed thin brim preserves the silhouette from both gameplay and portrait cameras.
hat = shell("Keeper conical hat", [(1.91,.008,.008),(1.865,.10,.10),(1.79,.34,.34),
                                  (1.782,.34,.34),(1.85,.10,.10),(1.90,.008,.008)], ink, "Head")
shell("Keeper hat rim", [(1.789,.341,.341),(1.782,.341,.341)], gold, "Head")
shell("Keeper hat band", [(1.85,.15,.15),(1.84,.17,.17)], gold, "Head")
for side in (-1, 1):
    x = side * .225
    mesh("Keeper hat pendant", [(x,-.07,1.783),(x+.014,-.07,1.783),
                                (x+.014,-.07,1.65),(x,-.07,1.65)], [(0,1,2,3),(3,2,1,0)], gold, "Head")

hat_width = max(v.co.x for v in hat.data.vertices) - min(v.co.x for v in hat.data.vertices)
assert .65 < hat_width < .72
assert rest_matrices == {b.name: tuple(v for row in b.matrix_local for v in row) for b in rig.data.bones}, "Shared skeleton rest pose changed"
parts = [o for o in bpy.context.scene.objects if o.type == "MESH"]
for obj in parts:
    assert obj.find_armature() == rig, obj.name
    for vertex in obj.data.vertices:
        weights = sorted([(obj.vertex_groups[g.group].name, g.weight) for g in vertex.groups if g.weight > 0],
                         key=lambda pair: pair[1], reverse=True)[:4]
        total = sum(weight for _, weight in weights)
        assert total > 0, (obj.name, vertex.index)
        for influence in list(vertex.groups):
            obj.vertex_groups[influence.group].remove([vertex.index])
        for name, weight in weights:
            obj.vertex_groups[name].add([vertex.index], weight / total, "REPLACE")

args.output.mkdir(parents=True)
bpy.ops.object.select_all(action="DESELECT")
copies = []
for obj in parts:
    duplicate = obj.copy()
    duplicate.data = obj.data.copy()
    bpy.context.collection.objects.link(duplicate)
    duplicate.select_set(True)
    copies.append(duplicate)
bpy.context.view_layer.objects.active = copies[0]
bpy.ops.object.join()
export = bpy.context.object
export.name = "Gate Keeper v4"
export.data.calc_loop_triangles()
used_materials={slot.material.name for obj in parts for slot in obj.material_slots}
assert len(used_materials) == 6, used_materials
(args.output / "asset-metrics.json").write_text(json.dumps({
    "status":"candidate_offline_review", "source":str(args.source),
    "source_sha256":hashlib.sha256(args.source.read_bytes()).hexdigest(),
    "recipe_sha256":hashlib.sha256(Path(__file__).read_bytes()).hexdigest(),
    "blender_version":bpy.app.version_string,
    "reference":"02_gate_keeper_character_sheet.png", "rig_rest_unchanged":True,
    "bones":len(rest_matrices), "vertices":len(export.data.vertices),
    "triangles":len(export.data.loop_triangles), "materials":sorted(used_materials),
    "export_renderers":1, "max_bone_influences":4,
    "new_textures":0, "runtime_verified":False,
    "outfit_scope":"stationary guide; walking cape fit not validated"
},indent=2)+"\n")
print("LGO_KEEPER_V4_GEOMETRY vertices=" + str(len(export.data.vertices)) +
      " triangles=" + str(len(export.data.loop_triangles)), flush=True)
rig.select_set(True)
bpy.ops.export_scene.fbx(filepath=str((args.output / "gate-keeper.fbx").resolve()), use_selection=True,
                        object_types={"ARMATURE","MESH"}, add_leaf_bones=False, bake_anim=False,
                        axis_forward="-Z", axis_up="Y", path_mode="STRIP")
assert len(export.data.loop_triangles) <= 25000, "Guide exceeds candidate triangle budget"
metrics_path = args.output / "asset-metrics.json"
metrics = json.loads(metrics_path.read_text())
metrics["fbx_sha256"] = hashlib.sha256((args.output / "gate-keeper.fbx").read_bytes()).hexdigest()
metrics_path.write_text(json.dumps(metrics,indent=2)+"\n")
bpy.data.objects.remove(export, do_unlink=True)
for side, angle in (("l", -65), ("r", 65)):
    bone = rig.pose.bones["upperarm_" + side]
    bone.rotation_mode = "XYZ"
    bone.rotation_euler.z = math.radians(angle)

scene = bpy.context.scene
scene.render.engine = "CYCLES"
scene.cycles.samples = 32
scene.render.resolution_percentage = 100
scene.render.image_settings.file_format = "PNG"
scene.render.image_settings.color_mode = "RGBA"
scene.render.film_transparent = True
scene.world = bpy.data.worlds.new("Guide review world")
scene.world.use_nodes = True
scene.world.node_tree.nodes["Background"].inputs[0].default_value = (.4,.43,.46,1)
scene.world.node_tree.nodes["Background"].inputs[1].default_value = .7
bpy.ops.object.light_add(type="AREA", location=(2,-3,4))
bpy.context.object.data.energy = 450
bpy.context.object.data.size = 4
bpy.ops.object.camera_add(location=(2.5,-4,1.5))
camera = bpy.context.object
camera.data.type = "ORTHO"
scene.camera = camera
for name, location, aim, scale, size in (
        ("front", (0,-4,1), (0,0,.98), 2.15, 768),
        ("rear", (0,4,1), (0,0,.98), 2.15, 768),
        ("portrait", (0,-4,1.72), (0,0,1.72), .76, 128)):
    camera.location = location
    camera.rotation_euler = (Vector(aim) - camera.location).to_track_quat("-Z","Y").to_euler()
    camera.data.ortho_scale = scale
    scene.render.resolution_x = scene.render.resolution_y = size
    scene.render.filepath = str((args.output / (name + ".png")).resolve())
    bpy.ops.render.render(write_still=True)
bpy.ops.wm.save_as_mainfile(filepath=str((args.output / "gate-keeper.blend").resolve()))
print("LGO_KEEPER_V4_STAGING_COMPLETE runtime_imported=false", flush=True)
