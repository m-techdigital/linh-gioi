"""Rigid shoulder armor in the existing shared skeleton, preserving its designed lowered-arm placement."""
import argparse,sys,math
from pathlib import Path
import bpy
from mathutils import Matrix
p=argparse.ArgumentParser();p.add_argument('--source',type=Path,required=True);p.add_argument('--output',type=Path,required=True)
a=p.parse_args(sys.argv[sys.argv.index('--')+1:]);a.output.parent.mkdir(parents=True,exist_ok=True)
bpy.ops.wm.open_mainfile(filepath=str(a.source.resolve()))
o=bpy.data.objects['Keeper Macro Reconstruction'];rig=bpy.data.objects['Armature']
for b in rig.pose.bones:b.matrix_basis.identity()
for side,angle in [('l',-60),('r',60)]:
 b=rig.pose.bones['upperarm_'+side];b.rotation_mode='XYZ';b.rotation_euler.z=math.radians(angle)
bpy.context.view_layer.update()
transforms={b.name:rig.pose.bones[b.name].matrix@b.matrix_local.inverted() for b in rig.data.bones}
slot=o.data.attributes['appearance_slot'];owned=set()
for face in o.data.polygons:
 if slot.data[face.index].value==15:owned.update(face.vertices)
assert owned
for index in owned:
 v=o.data.vertices[index];old=Matrix([[0.]*4 for _ in range(4)])
 ws={o.vertex_groups[g.group].name:g.weight for g in v.groups}
 for name,w in ws.items():old+=transforms[name]*w
 position=old@v.co
 side='l' if position.x>=0 else 'r';name='upperarm_'+side
 v.co=transforms[name].inverted()@position
 for group in o.vertex_groups:group.remove([index])
 o.vertex_groups[name].add([index],1.,'REPLACE')
for b in rig.pose.bones:b.matrix_basis.identity()
bpy.context.view_layer.update();bpy.context.preferences.filepaths.save_version=0
bpy.ops.wm.save_as_mainfile(filepath=str(a.output.resolve()),compress=True)
print('LGO_KEEPER_RIGID_SHOULDERS vertices='+str(len(owned))+' new_bones=0 new_materials=0')
