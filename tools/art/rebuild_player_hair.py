"""Tied-back arrival hair from the arrival reference silhouette, using existing body atlas only."""
import bpy,bmesh,math
from mathutils import Vector,kdtree

def rebuild_hair(o,rig,head_profile):
 samples=[];tag=o.data.attributes['appearance_slot'];uv=o.data.uv_layers.active
 for p in o.data.polygons:
  if tag.data[p.index].value==3:
   for li in p.loop_indices:samples.append((o.data.vertices[o.data.loops[li].vertex_index].co.copy(),uv.data[li].uv.copy()))
 assert samples
 tree=kdtree.KDTree(len(samples))
 for i,(co,_) in enumerate(samples):tree.insert(co,i)
 tree.balance()
 bm=bmesh.new();bm.from_mesh(o.data);st=bm.faces.layers.int.get('appearance_slot');bmesh.ops.delete(bm,geom=[f for f in bm.faces if f[st]==3],context='FACES');bmesh.ops.delete(bm,geom=[v for v in bm.verts if not v.link_faces],context='VERTS');bm.to_mesh(o.data);bm.free()
 vs=[];fs=[]
 def surface(nu,nv,fn):
  off=len(vs)
  for j in range(nv+1):
   for i in range(nu):vs.append(fn(i/nu,j/nv))
  for j in range(nv):
   for i in range(nu):
    q=(off+j*nu+i,off+j*nu+(i+1)%nu,off+(j+1)*nu+(i+1)%nu,off+(j+1)*nu+i)
    fs.extend([(q[0],q[1],q[2]),(q[0],q[2],q[3])])
  for ring in [off,off+nv*nu]:
   center=len(vs);vs.append(sum((Vector(vs[ring+i]) for i in range(nu)),Vector())/nu)
   for i in range(nu):fs.append((center,ring+i,ring+(i+1)%nu))
 def crown(u,t):
  a=math.tau*u;back=(1+math.cos(a))/2;front_part=.23*math.exp(-((a-math.pi)/.58)**2);theta=.025+(1.12+1.49*back-front_part)*t
  # Subtle swept grooves within one continuous crown, without ribbon shards.
  groove=.0012*math.cos(a*22+t*2.5)*math.sin(theta)
  z=1.687+.093*math.cos(theta);width,front,back=head_profile(z)
  center=(front+back)/2;depth=(back-front)/2
  return Vector(((width+.003+groove)*math.sin(a),center+(depth+.003+groove)*math.cos(a),z))
 surface(88,30,crown)
 def flowing_lock(points,width,depth,grooves=5):
  pts=[Vector(p) for p in points]
  def curve(t):return pts[0]*(1-t)**3+pts[1]*3*(1-t)**2*t+pts[2]*3*(1-t)*t*t+pts[3]*t**3
  def fn(u,t):
   center=curve(t);dt=curve(min(1,t+.001))-curve(max(0,t-.001));tangent=dt.normalized();wide=tangent.cross(Vector((0,1,0))).normalized();deep=tangent.cross(wide).normalized();a=math.tau*u
   taper=max(.004,(1-t)**.70)*(0.55+0.45*math.sin(math.pi*min(1,t*2)))
   ridge=1+.09*math.cos(a*grooves+.7*t)
   return center+wide*(math.cos(a)*width*taper*ridge)+deep*(math.sin(a)*depth*taper*ridge)
  surface(28,48,fn)
 # Forehead part: broad smooth sections swept over the skull and around the temples.
 flowing_lock([crown(.52,.12),crown(.61,.47),crown(.69,.84),(-.068,-.056,1.664)],.020,.007)
 flowing_lock([crown(.48,.12),crown(.39,.46),crown(.31,.83),(.067,-.054,1.677)],.018,.006)
 flowing_lock([crown(.53,.24),crown(.56,.58),(-.042,-.091,1.735),(-.051,-.093,1.697)],.010,.004)
 flowing_lock([crown(.47,.24),crown(.44,.57),(.049,-.089,1.743),(.056,-.091,1.707)],.010,.004)
 # A compact raised tie and three broad curved tail sections match the long gathered reference hair.
 def tie(u,t):
  a=u*math.tau;theta=.02+(math.pi-.04)*t
  return Vector((.023*math.sin(theta)*math.cos(a),.072+.025*math.sin(theta)*math.sin(a),1.782+.032*math.cos(theta)))
 surface(32,20,tie)
 flowing_lock([(0,.086,1.793),(-.024,.158,1.787),(-.061,.194,1.436),(-.016,.191,1.31)],.052,.022,9)
 flowing_lock([(.012,.109,1.762),(.055,.179,1.648),(.065,.208,1.451),(.030,.190,1.342)],.031,.018,7)
 flowing_lock([(-.017,.103,1.758),(-.061,.165,1.632),(-.078,.195,1.481),(-.052,.204,1.375)],.026,.016,7)
 data=bpy.data.meshes.new('Arrival swept tied hair');data.from_pydata(vs,[],fs);data.update();bm=bmesh.new();bm.from_mesh(data);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(data);bm.free()
 ob=bpy.data.objects.new('Arrival swept tied hair',data);bpy.context.collection.objects.link(ob);data.materials.append(bpy.data.materials['Keeper Reconstruction']);sa=data.attributes.new('appearance_slot','INT','FACE');coarse=data.attributes.new('npc_part','INT','FACE');uv=data.uv_layers.new(name='UVMap')
 for p in data.polygons:
  p.use_smooth=True;sa.data[p.index].value=3;coarse.data[p.index].value=2
  for li in p.loop_indices:uv.data[li].uv=samples[tree.find(data.vertices[data.loops[li].vertex_index].co)[1]][1]
 for b in rig.data.bones:ob.vertex_groups.new(name=b.name)
 ob.vertex_groups[next(b.name for b in rig.data.bones if b.name.lower()=='head')].add(list(range(len(data.vertices))),1,'REPLACE')
 bpy.ops.object.select_all(action='DESELECT');ob.select_set(True);o.select_set(True);bpy.context.view_layer.objects.active=o;bpy.ops.object.join()
 return o
