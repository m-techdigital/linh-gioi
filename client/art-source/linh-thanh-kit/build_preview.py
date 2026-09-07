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
    if name in ['stone','ivory','wood','navy']:
        nodes=mat.node_tree.nodes;links=mat.node_tree.links
        coords=nodes.new('ShaderNodeTexCoord');noise=nodes.new('ShaderNodeTexNoise')
        noise.inputs['Scale'].default_value=5 if name=='wood' else 8
        noise.inputs['Detail'].default_value=2
        ramp=nodes.new('ShaderNodeValToRGB')
        ramp.color_ramp.elements[0].position=.15
        ramp.color_ramp.elements[0].color=tuple(c*.76 for c in color[:3])+(1,)
        ramp.color_ramp.elements[1].position=.85
        ramp.color_ramp.elements[1].color=tuple(color)
        links.new(coords.outputs['Generated'],noise.inputs['Vector'])
        links.new(noise.outputs['Fac'],ramp.inputs['Fac']);links.new(ramp.outputs['Color'],bsdf.inputs['Base Color'])
        bsdf.inputs['Roughness'].default_value=.82
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

def tube(name, points, radius, material):
    # One mesh for a repeated ceramic cap or moulding, rather than many tiny objects.
    vertices=[];faces=[]
    for i,p in enumerate(points):
        direction=Vector(points[min(i+1,len(points)-1)])-Vector(points[max(i-1,0)])
        tangent=direction.normalized()
        side=tangent.cross(Vector((0,0,1))).normalized()
        if side.length<.1:side=Vector((1,0,0))
        up=tangent.cross(side).normalized()
        for j in range(6):
            angle=j*math.tau/6
            vertices.append(Vector(p)+radius*(side*math.cos(angle)+up*math.sin(angle)))
        if i:
            for j in range(6):
                a=(i-1)*6+j;b=(i-1)*6+(j+1)%6
                faces.append((a,b,b+6,a+6))
    faces.append(tuple(reversed(range(6))))
    faces.append(tuple((len(points)-1)*6+j for j in range(6)))
    obj=mesh(name,vertices,faces,material)
    for poly in obj.data.polygons:poly.use_smooth=True
    return obj

def roof(p, width=4.8, depth=4.8, rise=1.05):
    # Hipped roof with a real horizontal ridge, deep timber eave and lifted corners.
    # This replaces the old point-pyramid profile across the whole architectural family.
    x0,y0,z0=p
    n=20
    ridge=max(width*.17,(width-depth)*.35)
    def surface(x,y):
        u=abs(x)/(width/2);v=abs(y)/(depth/2)
        hip=max(0,(abs(x)-ridge)/(width/2-ridge))
        t=max(v,hip)
        height=rise*(1-t)**1.5+.08*t**6+.24*(u*v)**5
        return Vector((x0+x,y0+y,z0+height))
    verts=[surface((x/n*2-1)*width/2,(y/n*2-1)*depth/2) for y in range(n+1) for x in range(n+1)]
    faces=[]
    for y in range(n):
        for x in range(n):
            a=y*(n+1)+x;faces.append((a,a+1,a+n+2,a+n+1))
    obj=mesh('Layered hipped ceramic roof',verts,faces,'navy')
    for poly in obj.data.polygons:poly.use_smooth=True
    solid=obj.modifiers.new('Heavy tiled edge','SOLIDIFY');solid.thickness=.13 if width>2 else .05
    bpy.context.view_layer.objects.active=obj
    bpy.ops.object.modifier_apply(modifier=solid.name)
    for edge in range(4):
        row=([i for i in range(n+1)] if edge==0 else
             [n*(n+1)+i for i in range(n+1)] if edge==1 else
             [i*(n+1) for i in range(n+1)] if edge==2 else
             [i*(n+1)+n for i in range(n+1)])
        points=[verts[i] for i in row]
        tube('Timber fascia',[v-Vector((0,0,.13 if width>2 else .04)) for v in points],.075 if width>2 else .025,'wood')
        tube('Eave ceramic lip',points,.035 if width>2 else .014,'navy')
    if width>1.5:
        # Rounded tile caps create narrow repeated shadow lines, no drawn board texture.
        count=max(8,round(width/.18))
        for i in range(1,count):
            x=(i/count*2-1)*width/2
            for side in [-1,1]:
                points=[surface(x,side*depth/2*j/10)+Vector((0,0,.026)) for j in range(11)]
                tube('Rounded tile cap',points,.031,'navy')
        for side in [-1,1]:
            for row in range(1,6):
                y=side*depth/2*row/6
                tube('Overlapping tile course',[surface((i/n*2-1)*width/2,y)+Vector((0,0,.014)) for i in range(n+1)],.012,'navy')
        # A repeated pair of corbels transfers roof weight to the timber frame.
        for i in range(5):
            x=x0+(i/4*2-1)*width*.36
            for side in [-1,1]:
                inner=y0+side*depth*.35;outer=y0+side*depth*.47
                beam('Eave corbel',(x,inner,z0-.40),(x,outer,z0-.09),.10,'wood')
                box('Bracket capital',(x,outer,z0-.11),(.27,.22,.11),'wood')
                box('Bracket lower arm',(x,inner,z0-.29),(.35,.24,.10),'wood')
        for side in [-1,1]:
            tube('Carved ridge finial',[(x0+side*ridge,y0,z0+rise+.07),(x0+side*(ridge+.18),y0,z0+rise+.14),(x0+side*(ridge+.28),y0,z0+rise+.35)],.065,'gold')
    tube('Ridge cap',[surface(-ridge,0)+Vector((0,0,.06)),surface(ridge,0)+Vector((0,0,.06))],.08 if width>2 else .03,'navy')

def railing(a,b,height=.7,stone=False):
    a,b=Vector(a),Vector(b)
    mat='stone' if stone else 'wood'
    beam('Gallery top rail',a+Vector((0,0,height)),b+Vector((0,0,height)),.10,mat)
    beam('Gallery lower rail',a+Vector((0,0,.15)),b+Vector((0,0,.15)),.09,mat)
    count=max(2,round((b-a).length/.4))
    for i in range(count+1):
        p=a.lerp(b,i/count)
        beam('Gallery post',p,p+Vector((0,0,height+.08)),.105 if i in [0,count] else .055,mat)
        if stone and i in [0,count]:box('Stone post cap',p+Vector((0,0,height+.1)),(.22,.22,.12),mat,.025)

def dressed_base(width,depth):
    # Shallow stepped plinth and low garden edges; same footprint family, no raised temple dais.
    box('Plinth lower course',(0,0,.085),(width+.18,depth+.18,.17),'stone',.045)
    box('Plinth moulding',(0,0,.26),(width+.12,depth+.12,.09),'stone',.025)
    for side in [-1,1]:
        x=side*(width/2+.16)
        railing((x,-depth*.35,.20),(x,depth*.35,.20),.52,True)

def hanging_lantern(x,y,z):
    beam('Hanging chain',(x,y,z+.45),(x,y,z+.15),.025,'wood')
    box('Hanging amber lantern',(x,y,z),(.24,.24,.34),'amber',.02)
    for side in [-1,1]:
        box('Lantern slender frame',(x+side*.14,y,z),(.025,.3,.38),'wood')
    box('Lantern wooden cap',(x,y,z+.19),(.34,.34,.06),'wood',.02)


def lattice_window(x, y, z, width=1.05, height=1.05):
    box('Recessed timber window',(x,y,z),(width+.14,.12,height+.16),'wood',.018)
    box('Warm paper inset',(x,y-.075,z),(width,.035,height),'ivory')
    for offset in [-.35,0,.35]:
        box('Window vertical lattice',(x+offset*width,y-.105,z),(.045,.055,height),'wood')
    for offset in [-.3,.3]:
        box('Window horizontal lattice',(x,y-.105,z+offset*height),(width,.055,.045),'wood')
    box('Window projecting sill',(x,y-.09,z-height/2-.10),(width+.28,.30,.10),'wood',.018)

def house():
    dressed_base(4,4)
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
    # Front gallery and entry thresholds add inhabited depth beneath the broad eave.
    box('Entry threshold',(0,-2.13,.16),(1.05,.6,.17),'stone',.035)
    box('Entry dark frame',(0,-2.0,1.22),(.72,.10,1.75),'wood')
    for x in [-1.65,1.65]:hanging_lantern(x,-2.10,2.68)
    roof((0,0,3.3))
    box('Upper room',(0,.2,4.05),(2.5,2.1,.8),'wood')
    for x in [-.75,-.25,.25,.75]:box('Upper light',(x,-.87,4.1),(.27,.05,.4),'amber')
    box('Upper gallery',(0,-1.08,3.77),(2.9,.7,.10),'wood')
    railing((-1.35,-1.38,3.82),(1.35,-1.38,3.82),.42)
    roof((0,.2,4.65),3.55,3.2,.80)

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
    # Social landmark: a cafe in a timber old-town facade, with a sheltered upper balcony.
    dressed_base(6,4)
    box('Cafe upstairs',(0,.15,3.55),(5.8,3.7,1.8),'ivory',.03)
    box('Cafe rear',(0,1.85,1.35),(5.8,.22,2.5),'wood')
    for x in [-2.8,0,2.8]:box('Cafe front pier',(x,-1.75,1.4),(.18,.18,2.8),'wood')
    box('Cafe counter',(0,.5,.95),(4.8,.8,.9),'wood',.04)
    for x in [-2,-1,0,1,2]:box('Cafe shelf goods',(x,.5,1.5),(.38,.3,.28),'gold',.04)
    box('Storefront light',(0,1.68,1.9),(4.8,.04,.5),'amber')
    for x in [-2,-1,0,1,2]:
        lattice_window(x,-1.80,3.65,.66,.9)
    box('Balcony deck',(0,-2.05,2.75),(5.9,.85,.16),'wood')
    for x in [-2.7,-2.1,-1.5,-.9,-.3,.3,.9,1.5,2.1,2.7]:
        box('Balcony rail',(x,-2.4,3.1),(.05,.05,.7),'wood')
    box('Balcony top rail',(0,-2.4,3.45),(5.7,.08,.08),'wood')
    for x in [-2.75,0,2.75]:
        box('Cafe upper timber post',(x,-1.77,3.6),(.14,.14,1.75),'wood')
    for z in [2.88,4.35]:box('Cafe upper lintel',(0,-1.83,z),(5.8,.16,.16),'wood')
    # Folded shutters and hanging lanterns preserve the old-town frontage.
    for x in [-2.8,2.8]:
        box('Cafe folded shutter',(x,-1.75,1.45),(.40,.15,2.1),'wood')
        hanging_lantern(x*.80,-2.1,2.10)
    roof((0,.15,4.55),6.65,4.8,.94)
    # Natural canvas shelter and restrained teal valance under the old timber frontage.
    for i in range(10):
        obj=box('Cafe awning',(-2.7+i*.6,-2.4,2.6),(.6,1.4,.05),'ivory')
        obj.rotation_euler.x=math.radians(10)
    box('Awning edge',(0,-3.08,2.48),(5.95,.04,.18),'teal')
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

def townhouse():
    # Narrow old-town upper floors: timber frame, recessed lattice windows and weathered plaster.
    dressed_base(4.2,4.2)
    box('Lodge mass',(0,0,3.05),(3.8,3.8,5.9),'ivory',.025)
    for side in range(4):
        angle=side*math.pi/2
        before=set(bpy.data.objects)
        for z in [1.25,3.05,4.85]:
            for x in [-1.05,1.05]: lattice_window(x,-1.96,z,.94,1.03)
            box('Timber floor rail',(0,-2.02,z-.72),(4.0,.22,.17),'wood')
            box('Plaster panel sill',(0,-1.95,z+.68),(3.72,.12,.10),'ivory')
            box('Central timber post',(0,-1.97,z),(.14,.14,1.7),'wood')
        for x in [-1.84,1.84]:box('Slender facade pier',(x,-1.98,3.05),(.13,.13,5.8),'wood')
        for obj in set(bpy.data.objects)-before:
            x,y,z=obj.location
            obj.location=(x*math.cos(angle)-y*math.sin(angle),x*math.sin(angle)+y*math.cos(angle),z)
            obj.rotation_euler.z=angle
    for x in [-1.6,-.8,0,.8,1.6]:
        beam('Carved eave bracket',(x,-1.85,5.65),(x,-2.25,6.05),.12,'wood')
    # One sheltered gallery breaks the sheer three-storey box without changing the footprint.
    box('Townhouse balcony',(0,-2.10,3.94),(3.8,.72,.14),'wood')
    railing((-1.8,-2.42,4.01),(1.8,-2.42,4.01),.64)
    roof((0,-1.96,4.85),4.3,1.1,.35)
    for x in [-1.4,1.4]:hanging_lantern(x,-2.04,2.0)
    roof((0,0,6.1),5.15,5.15,1.10)

def gate():
    for x in [-4,4]:
        box('Gate plinth',(x,0,.3),(1.5,2,.6),'stone',.08)
        box('Gate plinth lower moulding',(x,0,.14),(1.68,2.18,.16),'stone',.035)
        box('Gate plinth crown',(x,0,.62),(1.6,2.1,.12),'stone',.035)
        box('Gate timber',(x,0,2.7),(.7,.9,4.8),'wood')
        box('Gate teal panel',(x,-.49,2.8),(.45,.08,2.8),'teal')
        for offset in [-.29,.29]:box('Column trim',(x+offset,-.55,2.8),(.055,.055,4),'gold')
    box('Lintel',(0,0,5),(9.3,1,.65),'wood')
    for x in [-4,-2,0,2,4]:
        box('Gate lintel bracket',(x,0,4.75),(.45,1.4,.18),'wood')
        beam('Gate arch support',(x,0,4.45),(x,-1.25,5.14),.16,'wood')
    roof((0,0,5.3),11,3.7,1.15)
    box('Upper gate room',(0,0,6.5),(5,1.7,.7),'wood')
    for x in [-1.8,-.9,0,.9,1.8]:lattice_window(x,-.91,6.5,.6,.38)
    roof((0,0,6.85),7,3.1,.95)
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
        if len(group)>1:bpy.ops.object.join()
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
