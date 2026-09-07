"""Blender authoring preview. Reads kit-layouts.json; never imports reference images.
Run: Blender --background --python this_file -- --output build/asset-staging/linh-thanh-kit
"""
import argparse
import json
import math
from pathlib import Path
import sys
import bpy
from mathutils import Vector

parser = argparse.ArgumentParser()
parser.add_argument('--output', required=True)
args = parser.parse_args(sys.argv[sys.argv.index('--') + 1:])
out = Path(args.output).resolve()
out.mkdir(parents=True, exist_ok=True)
catalog = json.loads(Path(__file__).with_name('kit-layouts.json').read_text())
bpy.ops.wm.read_factory_settings(use_empty=True)
mats = {}
for name, color in catalog['materials'].items():
    mat = bpy.data.materials.new(name)
    mat.diffuse_color = color
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get('Principled BSDF')
    bsdf.inputs['Base Color'].default_value = color
    bsdf.inputs['Roughness'].default_value = .67
    if name == 'gold':
        bsdf.inputs['Metallic'].default_value = .45
    if name == 'amber':
        bsdf.inputs['Emission Color'].default_value = color
        bsdf.inputs['Emission Strength'].default_value = .65
    mats[name] = mat

def mesh(name, vertices, faces, material):
    data = bpy.data.meshes.new(name)
    data.from_pydata(vertices, [], faces)
    data.materials.append(mats[material])
    obj = bpy.data.objects.new(name, data)
    bpy.context.collection.objects.link(obj)
    return obj

def box(name, p, size, material, bevel=0):
    bpy.ops.mesh.primitive_cube_add(size=1, location=p)
    obj = bpy.context.object
    obj.name = name
    obj.scale = size
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    obj.data.materials.append(mats[material])
    if bevel:
        mod = obj.modifiers.new('Soft carved edge', 'BEVEL')
        mod.width = bevel
        mod.segments = 1
        bpy.context.view_layer.objects.active = obj
        bpy.ops.object.modifier_apply(modifier=mod.name)
    return obj

def beam(name, a, b, width, material):
    a, b = Vector(a), Vector(b)
    obj = box(name, (a+b)*.5, (width,width,(b-a).length), material)
    obj.rotation_euler = (b-a).to_track_quat('Z','Y').to_euler()
    return obj

def roof(p, width=4.8, depth=4.8, rise=1.05):
    # Same swept eave family as CityArchitectureVisuals.Roof, adapted to Blender Z-up.
    x0,y0,z0=p
    n=12
    verts=[]
    for y in range(n+1):
        for x in range(n+1):
            u=x/n*2-1;v=y/n*2-1
            t=max(abs(u),abs(v))
            z=rise*(1-t)**2+.09*t**6+.22*(abs(u)*abs(v))**4
            verts.append((x0+u*width/2,y0+v*depth/2,z0+z))
    faces=[]
    for y in range(n):
        for x in range(n):
            a=y*(n+1)+x;faces.append((a,a+1,a+n+2,a+n+1))
    obj=mesh('Curved navy roof',verts,faces,'navy')
    solid=obj.modifiers.new('Roof underside','SOLIDIFY');solid.thickness=.10
    bpy.context.view_layer.objects.active=obj
    bpy.ops.object.modifier_apply(modifier=solid.name)
    for edge in range(4):
        row=([i for i in range(n+1)] if edge==0 else
             [n*(n+1)+i for i in range(n+1)] if edge==1 else
             [i*(n+1) for i in range(n+1)] if edge==2 else
             [i*(n+1)+n for i in range(n+1)])
        for i in range(n):beam('Bronze eave',verts[row[i]],verts[row[i+1]],.055,'gold')
    # Repeated strips are authored geometry shared through collection instances.
    for x in range(1,n,2):
        for y in range(n):
            a=y*(n+1)+x;b=a+n+1
            beam('Ceramic seam',Vector(verts[a])+Vector((0,0,.015)),Vector(verts[b])+Vector((0,0,.015)),.023,'navy')

def house():
    box('Foundation',(0,0,.15),(4,4,.3),'stone',.05)
    box('Plaster walls',(0,0,1.75),(3.8,3.8,3.2),'ivory',.03)
    for x in [-1.8,0,1.8]:box('Front timber',(x,-1.94,1.75),(.15,.15,3.2),'wood')
    for z in [.45,2.5,3.2]:box('Cross beam',(0,-1.97,z),(3.9,.15,.13),'wood')
    for x in [-1.05,1.05]:
        box('Window shadow',(x,-1.96,1.7),(1.15,.08,1.15),'wood')
        box('Window amber',(x,-2.01,1.7),(.93,.04,.94),'amber')
        for offset in [-.32,0,.32]:box('Window lattice',(x+offset,-2.05,1.7),(.055,.055,1),'wood')
        box('Window lattice',(x,-2.05,1.7),(1,.05,.055),'wood')
    # Side and rear faces stay readable from a rotating game camera.
    for angle in [math.pi/2, math.pi, math.pi*1.5]:
        before=set(bpy.data.objects)
        for x in [-1.45,1.45]:box('Side timber',(x,-1.94,1.75),(.14,.14,3.2),'wood')
        for z in [.45,2.5,3.2]:box('Side cross beam',(0,-1.97,z),(3.8,.14,.12),'wood')
        for x in [-.75,.75]:
            box('Side window',(x,-1.96,1.65),(.85,.08,.95),'wood')
            box('Side amber',(x,-2.01,1.65),(.68,.04,.76),'amber')
            box('Side lattice',(x,-2.05,1.65),(.055,.055,.8),'wood')
        for obj in set(bpy.data.objects)-before:
            x,y,z=obj.location
            obj.location=(x*math.cos(angle)-y*math.sin(angle),x*math.sin(angle)+y*math.cos(angle),z)
            obj.rotation_euler.z=angle
    roof((0,0,3.3))
    box('Upper room',(0,.2,4.05),(2.5,2.1,.8),'wood')
    for x in [-.75,-.25,.25,.75]:box('Upper light',(x,-.87,4.1),(.27,.05,.4),'amber')
    roof((0,.2,4.5),3.25,2.9,.75)

def lantern():
    box('Foot',(0,0,.12),(.5,.5,.24),'stone',.05)
    box('Pole',(0,0,1.25),(.13,.13,2.5),'wood')
    box('Lantern light',(0,0,2.55),(.42,.42,.55),'amber')
    for x in [-.24,.24]:
        for y in [-.24,.24]:box('Lantern frame',(x,y,2.55),(.045,.045,.66),'gold')
    roof((0,0,2.9),.75,.75,.23)

def bench():
    box('Seat',(0,0,.5),(2,.65,.15),'wood',.035)
    box('Back',(0,.28,.86),(2,.12,.5),'wood',.03)
    for x in [-.8,.8]:box('Bench leg',(x,0,.25),(.18,.48,.5),'stone')

def blossom():
    beam('Trunk',(0,0,0),(.2,.05,2.7),.24,'wood')
    for i in range(9):
        a=i*2.399963
        p=(math.cos(a)*(1+(i%3)*.15), math.sin(a)*(1+(i%3)*.15),2.5+(i%3)*.4)
        beam('Branch',(.1,0,1.7),p,.085,'wood')
        bpy.ops.mesh.primitive_ico_sphere_add(subdivisions=1,radius=.7,location=p)
        obj=bpy.context.object;obj.name='Blossom canopy study';obj.scale=(1,1,.65);obj.data.materials.append(mats['pink'])

def stall():
    box('Counter',(0,0,.85),(2.5,1.5,.2),'wood',.035)
    for x in [-1.2,1.2]:
        for y in [-.7,.7]:box('Stall post',(x,y,1.2),(.12,.12,2.4),'wood')
    roof((0,0,2.4),3,2.2,.6)
    box('Teal counter cloth',(0,-.78,.61),(1.5,.04,.55),'teal')
    for i in range(6):box('Display box',(-.9+(i%3)*.85,(i//3)*.45-.3,1),(.5,.35,.2),'gold',.02)

def cafe():
    # Social landmark: open storefront and outdoor tables, with a contemporary upper balcony.
    box('Cafe foundation',(0,0,.10),(6,4,.2),'stone',.05)
    box('Cafe upstairs',(0,.15,3.55),(5.8,3.7,1.8),'ivory',.03)
    box('Cafe rear',(0,1.85,1.35),(5.8,.22,2.5),'wood')
    for x in [-2.8,0,2.8]:box('Cafe front pier',(x,-1.75,1.4),(.18,.18,2.8),'wood')
    box('Cafe counter',(0,.5,.95),(4.8,.8,.9),'wood',.04)
    for x in [-2,-1,0,1,2]:box('Cafe shelf goods',(x,.5,1.5),(.38,.3,.28),'gold',.04)
    box('Storefront light',(0,1.68,1.9),(4.8,.04,.5),'amber')
    for x in [-2,-1,0,1,2]:
        box('Upper window recess',(x,-1.74,3.65),(.8,.08,1.1),'wood')
        box('Upper glazing',(x,-1.8,3.65),(.66,.04,.9),'teal')
    box('Balcony deck',(0,-2.05,2.75),(5.9,.85,.16),'stone')
    for x in [-2.7,-2.1,-1.5,-.9,-.3,.3,.9,1.5,2.1,2.7]:
        box('Balcony rail',(x,-2.4,3.1),(.05,.05,.7),'wood')
    box('Balcony top rail',(0,-2.4,3.45),(5.7,.08,.08),'gold')
    roof((0,.15,4.55),6.4,4.4,.7)
    # Teal/ivory awning stripes read as a cafe at game-camera distance.
    for i in range(10):
        obj=box('Cafe awning',(-2.7+i*.6,-2.4,2.6),(.6,1.4,.05),'ivory' if i%2 else 'teal')
        obj.rotation_euler.x=math.radians(10)
    box('Cafe hanging sign',(2.75,-2.15,2.03),(.7,.10,.52),'wood',.035)
    # Cup pictogram is geometry, not board text or a runtime interaction promise.
    box('Cup symbol',(2.71,-2.22,2.04),(.24,.035,.23),'ivory',.02)
    beam('Cup handle',(2.83,-2.23,2.11),(2.92,-2.23,2.11),.035,'ivory')
    beam('Cup handle',(2.92,-2.23,2.11),(2.92,-2.23,1.99),.035,'ivory')
    for x in [-1.55,1.1]:
        box('Cafe table',(x,-3.3,.78),(.85,.8,.08),'wood',.04)
        box('Table pedestal',(x,-3.3,.38),(.1,.1,.75),'wood')
        for side in [-1,1]:
            cx=x+side*.78
            box('Chair seat',(cx,-3.3,.46),(.42,.45,.08),'wood',.02)
            box('Chair back',(cx+side*.18,-3.3,.7),(.07,.45,.5),'wood')
            for dx in [-.15,.15]:
                for dy in [-.16,.16]:box('Chair leg',(cx+dx,-3.3+dy,.23),(.055,.055,.46),'wood')
        box('Coffee cup',(x,-3.3,.9),(.10,.10,.15),'ivory',.01)
    for x in [-2.6,2.6]:
        box('Cafe planter',(x,-2.75,.25),(.5,.5,.5),'stone',.05)
        bpy.ops.mesh.primitive_ico_sphere_add(subdivisions=1,radius=.39,location=(x,-2.75,.67))
        bpy.context.object.name='Cafe greenery';bpy.context.object.data.materials.append(mats['green'])

def modern_lodge():
    # Tall everyday mixed-use silhouette; one roof crown instead of another repeated pagoda.
    box('Lodge foundation',(0,0,.12),(4.2,4.2,.24),'stone',.05)
    box('Lodge mass',(0,0,3.05),(3.8,3.8,5.9),'ivory',.025)
    for side in range(4):
        angle=side*math.pi/2
        before=set(bpy.data.objects)
        for z in [1.25,3.05,4.85]:
            box('Modern window band',(0,-1.94,z),(3.3,.06,1.15),'teal')
            for x in [-1.6,-.8,0,.8,1.6]:box('Window mullion',(x,-1.99,z),(.06,.07,1.25),'wood')
            box('Floor slab',(0,-2.02,z-.72),(4.0,.22,.13),'stone')
        for x in [-1.84,1.84]:box('Slender facade pier',(x,-1.98,3.05),(.13,.13,5.8),'wood')
        for obj in set(bpy.data.objects)-before:
            x,y,z=obj.location
            obj.location=(x*math.cos(angle)-y*math.sin(angle),x*math.sin(angle)+y*math.cos(angle),z)
            obj.rotation_euler.z=angle
    roof((0,0,6.1),4.8,4.8,.8)

def gate():
    for x in [-4,4]:
        box('Gate plinth',(x,0,.3),(1.5,2,.6),'stone',.08)
        box('Gate timber',(x,0,2.7),(.7,.9,4.8),'wood')
        box('Gate teal panel',(x,-.49,2.8),(.45,.08,2.8),'teal')
        for offset in [-.29,.29]:box('Column trim',(x+offset,-.55,2.8),(.055,.055,4),'gold')
    box('Lintel',(0,0,5),(9.3,1,.65),'wood')
    roof((0,0,5.3),11,3.4,1.1)
    box('Upper gate room',(0,0,6.5),(5,1.7,.7),'wood')
    roof((0,0,6.85),7,2.8,.8)
    for i in range(24):
        a=i*math.pi/24;b=(i+1)*math.pi/24
        beam('Arch bronze',(math.cos(a)*3.65,-.55,1.9+math.sin(a)*2.6),(math.cos(b)*3.65,-.55,1.9+math.sin(b)*2.6),.20,'gold')
    for x in [-2,2]:
        box('Hanging light',(x,-.7,4.55),(.38,.38,.65),'amber')
    for i in range(32):
        a=i*math.tau/32;b=(i+1)*math.tau/32
        beam('Gate crest',(math.cos(a)*.52,-.92,6.5+math.sin(a)*.52),(math.cos(b)*.52,-.92,6.5+math.sin(b)*.52),.065,'gold')

def plaza_medallion():
    for radius in [3.9,3.5,2.55,1.0]:
        for i in range(64):
            a=i*math.tau/64;b=(i+1)*math.tau/64
            beam('Plaza inlay',(radius*math.cos(a),radius*math.sin(a),.04),(radius*math.cos(b),radius*math.sin(b),.04),.055,'gold')
    for i in range(8):
        a=i*math.tau/8
        beam('Compass inlay',(math.cos(a),math.sin(a),.04),(2.5*math.cos(a),2.5*math.sin(a),.04),.07,'gold')

builders={k:globals()[k] for k in catalog['modules']}
collections={}
for name,build in builders.items():
    before=set(bpy.data.objects)
    build()
    # Bake pieces by material inside each kit module; collections instance these shared meshes.
    groups={}
    for obj in set(bpy.data.objects)-before:
        groups.setdefault(obj.data.materials[0].name,[]).append(obj)
    for group in groups.values():
        bpy.ops.object.select_all(action='DESELECT')
        for obj in group:obj.select_set(True)
        bpy.context.view_layer.objects.active=group[0]
        bpy.ops.object.join()
    collection=bpy.data.collections.new('KIT_'+name)
    for obj in set(bpy.data.objects)-before:
        for old in list(obj.users_collection):old.objects.unlink(obj)
        collection.objects.link(obj)
    collections[name]=collection

receipt={'status':'AUTHORING_PREVIEW_NOT_RUNTIME','layouts':{}}
for name,layout in catalog['layouts'].items():
    scene=bpy.data.scenes.new(name)
    bpy.context.window.scene=scene
    scene.render.engine='CYCLES';scene.cycles.samples=16
    scene.cycles.use_denoising=True
    scene.render.resolution_x=1100;scene.render.resolution_y=800;scene.render.resolution_percentage=100
    scene.world=bpy.data.worlds.new('Warm daylight '+name)
    scene.world.use_nodes=True
    scene.world.node_tree.nodes['Background'].inputs[0].default_value=(.40,.53,.67,1)
    scene.world.node_tree.nodes['Background'].inputs[1].default_value=.55
    scene.view_settings.view_transform='AgX'
    w,d=layout['extent_m']
    box('Scene platform',(0,0,-.18),(w,d,.3),'stone',.12)
    paving_meshes={}
    for x in range(-int(w/2),int(w/2),2):
        for y in range(-int(d/2),int(d/2),2):
            material='ivory' if abs(x)<4 else 'stone'
            if material not in paving_meshes:
                obj=box('Reusable paving',(x+1,y+1,-.015),(1.975,1.975,.035),material)
                paving_meshes[material]=obj.data
            else:
                obj=bpy.data.objects.new('Reusable paving',paving_meshes[material])
                obj.location=(x+1,y+1,-.015);scene.collection.objects.link(obj)
    for item in layout['instances']:
        obj=bpy.data.objects.new(item['module'],None)
        obj.instance_type='COLLECTION';obj.instance_collection=collections[item['module']]
        x,y,z=item['position_m'];obj.location=(x,z,y)
        obj.rotation_euler.z=-math.radians(item['yaw_degrees'])
        scene.collection.objects.link(obj)
    bpy.ops.object.light_add(type='AREA',location=(-8,-8,18))
    light=bpy.context.object;light.data.energy=4500;light.data.shape='DISK';light.data.size=15
    light.rotation_euler=(Vector((0,0,0))-light.location).to_track_quat('-Z','Y').to_euler()
    bpy.ops.object.camera_add(location=(-22,-30,24))
    camera=bpy.context.object;camera.rotation_euler=(Vector((0,1,2))-camera.location).to_track_quat('-Z','Y').to_euler()
    camera.data.type='ORTHO';camera.data.ortho_scale=36;scene.camera=camera
    scene.render.filepath=str(out/(name+'.png'))
    bpy.ops.render.render(write_still=True)
    receipt['layouts'][name]={'instances':len(layout['instances']),'image':scene.render.filepath}
for data in list(bpy.data.meshes):
    if data.users==0:bpy.data.meshes.remove(data)
bpy.ops.wm.save_as_mainfile(filepath=str(out/'LinhThanhKit.blend'),compress=True)
receipt['unique_meshes']=len(bpy.data.meshes)
receipt['shared_materials']=len(mats)
(out/'preview-receipt.json').write_text(json.dumps(receipt,indent=2)+'\n')
