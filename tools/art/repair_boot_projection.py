"""Reuse the clean front boot surface across the rear; preserve the atlas and semantic wardrobe ownership."""
import argparse,sys
from pathlib import Path
import bpy
from mathutils import Vector
from mathutils.geometry import closest_point_on_tri
p=argparse.ArgumentParser();p.add_argument('--source',type=Path,required=True);p.add_argument('--output',type=Path,required=True)
a=p.parse_args(sys.argv[sys.argv.index('--')+1:]);a.output.parent.mkdir(parents=True,exist_ok=True)
bpy.ops.wm.open_mainfile(filepath=str(a.source.resolve()))
o=next(o for o in bpy.data.objects if o.type=='MESH' and o.name in ('Keeper Macro Reconstruction','Arrival Scene Reconstruction'))
mesh=o.data;mesh.calc_loop_triangles();slot=mesh.attributes['appearance_slot'];uv=mesh.uv_layers.active.data
# Copy source UVs first: loops crossing the same boot seam must not depend on mutation order.
source=[]
for tri in mesh.loop_triangles:
 if slot.data[tri.polygon_index].value!=14 or tri.normal.y>=-.8:continue
 points=[Vector((mesh.vertices[v].co.x,mesh.vertices[v].co.z,0)) for v in tri.vertices]
 if (points[1]-points[0]).cross(points[2]-points[0]).length<1e-10:continue
 source.append((points,[uv[i].uv.copy() for i in tri.loops]))
assert source,'Front boot triangles required'
count=0
for face in mesh.polygons:
 if slot.data[face.index].value!=14 or face.normal.y<-.8:continue
 for li in face.loop_indices:
  co=mesh.vertices[mesh.loops[li].vertex_index].co;center=.0983024 if co.x>=0 else -.0983024
  point=Vector((center+(co.x-center)*.2,co.z,0));best=None
  for ps,us in source:
   if ps[0].x*co.x<0:continue
   near=closest_point_on_tri(point,*ps);distance=(near-point).length_squared
   if best is None or distance<best[0]:best=(distance,near,ps,us)
  assert best
  _,near,(v0,v1,v2),us=best
  e0=v1-v0;e1=v2-v0;d=near-v0
  aa=e0.dot(e0);ab=e0.dot(e1);bb=e1.dot(e1);da=d.dot(e0);db=d.dot(e1);den=aa*bb-ab*ab
  b=(bb*da-ab*db)/den;c=(aa*db-ab*da)/den
  uv[li].uv=us[0]*(1-b-c)+us[1]*b+us[2]*c;count+=1
bpy.context.preferences.filepaths.save_version=0
bpy.ops.wm.save_as_mainfile(filepath=str(a.output.resolve()),compress=True)
print('LGO_BOOT_PROJECTION_REPAIRED loops='+str(count)+' source_triangles='+str(len(source))+' atlas_unchanged=true')
