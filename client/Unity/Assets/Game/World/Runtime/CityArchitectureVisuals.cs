using System.Collections.Generic;
using LinhGioi.Art;
using UnityEngine;

namespace LinhGioi.World
{
    // A small reusable architecture kit; reference boards never enter the runtime.
    // Decorative geometry has no collision. The existing street owns navigation.
    internal sealed class CityArchitectureVisuals : MonoBehaviour
    {
        private readonly List<Mesh> _meshes = new List<Mesh>();
        private readonly List<Material> _materials = new List<Material>();
        private readonly Dictionary<Material, List<CombineInstance>> _batches = new Dictionary<Material, List<CombineInstance>>();
        private Mesh _cube, _roof, _arch, _sphere, _archTrim;
        private Matrix4x4 _module = Matrix4x4.identity;
        internal void BeginModule(Vector3 centre, Quaternion rotation) => _module = Matrix4x4.TRS(centre, rotation, Vector3.one) * Matrix4x4.Translate(-centre);
        internal void EndModule() => _module = Matrix4x4.identity;
        private Material _gold, _wood, _navy, _ivory, _window, _teal, _leaf, _pink;
        private Texture2D _woodTexture, _plasterTexture, _tileTexture;
        internal Texture2D PlasterTexture => _plasterTexture;

        internal void Initialize()
        {
            _gold = Surface("Aged bronze", new Color(.65f,.43f,.17f), .5f);
            _wood = Surface("Warm timber", new Color(.35f,.19f,.105f));
            _navy = Surface("Navy ceramic roof", new Color(.075f,.15f,.23f), .2f);
            _ivory = Surface("Ivory limestone", new Color(.72f,.65f,.51f));
            _window = Surface("Amber window recess", new Color(.21f,.125f,.07f));
            _teal = Surface("City teal banners", new Color(.065f,.30f,.34f));
            _leaf = Surface("Garden foliage", new Color(.21f,.37f,.15f));
            _pink = Surface("Blossom clusters", new Color(.66f,.35f,.40f));
            _woodTexture = new Texture2D(64,64,TextureFormat.RGBA32,true) { name="Shared timber grain",wrapMode=TextureWrapMode.Repeat };
            var pixels = new Color[4096];
            for (var y=0;y<64;y++) for(var x=0;x<64;x++)
            {
                var grain=.82f+.16f*Mathf.PerlinNoise(x*.33f,y*.026f);
                pixels[y*64+x]=new Color(grain,grain,grain);
            }
            _woodTexture.SetPixels(pixels); _woodTexture.Apply(true,true); _wood.mainTexture=_woodTexture;
            _plasterTexture = SurfaceTexture(false);
            _tileTexture = SurfaceTexture(true);
            _ivory.mainTexture = _plasterTexture;
            _navy.mainTexture = _tileTexture;
            _cube = Primitive(PrimitiveType.Cube);
            _sphere = FoliageMesh(); _meshes.Add(_sphere);
            _roof = RoofMesh(); _meshes.Add(_roof);
            _arch = ArchMesh(.24f); _meshes.Add(_arch);
            _archTrim = ArchMesh(.045f); _meshes.Add(_archTrim);
        }

        private Material Surface(string label, Color color, float metal = 0f)
        {
            var material=RuntimeArtCatalog.CreateMaterial(label,color);
            material.SetFloat("_Metallic",metal); material.SetFloat("_Smoothness",.25f);
            _materials.Add(material); return material;
        }
        private static Mesh Primitive(PrimitiveType type)
        {
            var go=GameObject.CreatePrimitive(type);
            var mesh=go.GetComponent<MeshFilter>().sharedMesh;
            go.SetActive(false); Destroy(go); return mesh;
        }
        private void Part(Mesh mesh, Material material, Vector3 point, Vector3 scale, Quaternion rotation)
        {
            if (!_batches.TryGetValue(material,out var batch)) _batches[material]=batch=new List<CombineInstance>();
            batch.Add(new CombineInstance {mesh=mesh,transform=_module * Matrix4x4.TRS(point,rotation,scale)});
        }
        private void Box(Material material, Vector3 point, Vector3 scale) => Part(_cube,material,point,scale,Quaternion.identity);
        private void Beam(Material material,Vector3 a,Vector3 b,float width)
        {
            var delta=b-a;
            Part(_cube,material,(a+b)*.5f,new Vector3(width,delta.magnitude,width),Quaternion.FromToRotation(Vector3.up,delta));
        }

        internal void Roof(Vector3 eave, Vector3 scale)
        {
            Part(_roof,_navy,eave,scale,Quaternion.identity);
            // Four swept eave edges, ridge and sparse ceramic ribs preserve the curved silhouette.
            for(var edge=0;edge<4;edge++)
            for(var step=0;step<8;step++)
            {
                var a=EavePoint(edge,step/8f); var b=EavePoint(edge,(step+1)/8f);
                Beam(_gold,eave+Vector3.Scale(a,scale),eave+Vector3.Scale(b,scale),.045f*scale.y);
            }
            Beam(_gold,eave+Vector3.Scale(new Vector3(0,1.1f,-.52f),scale),eave+Vector3.Scale(new Vector3(0,1.1f,.52f),scale),.065f*scale.y);
            // Ceramic rows are a small repeated surface texture, not hundreds of cube ribs.
            for(var x=-1;x<=1;x+=2) for(var z=-1;z<=1;z+=2)
            {
                var corner=eave+Vector3.Scale(new Vector3(x,.34f,z),scale);
                Beam(_gold,corner,corner+new Vector3(x*.12f,.22f,z*.12f)*scale.y,.055f*scale.y);
            }
        }
        private static float RoofHeight(float t) => 1.1f*(1-t)*(1-t)+.34f*Mathf.Pow(t,6);
        private static Vector3 EavePoint(int side,float t)
        {
            var v=t*2-1; var y=.12f+.22f*Mathf.Pow(Mathf.Abs(v),4);
            switch(side){case 0:return new Vector3(-1,y,v);case 1:return new Vector3(1,y,v);case 2:return new Vector3(v,y,-1);default:return new Vector3(v,y,1);}
        }
        private static Mesh RoofMesh()
        {
            var vertices=new List<Vector3>();var indices=new List<int>();
            void Quad(Vector3 a,Vector3 b,Vector3 c,Vector3 d)
            {var n=vertices.Count; vertices.AddRange(new[]{a,b,c,d});indices.AddRange(new[]{n,n+1,n+2,n,n+2,n+3});}
            // Rings collapse onto a ridge at the top, producing a hipped swept roof.
            for(var ring=0;ring<5;ring++) for(var edge=0;edge<4;edge++) for(var section=0;section<8;section++)
            {
                Vector3 P(float t,float u)
                {
                    var e=EavePoint(edge,u); var x=e.x*t;var z=e.z*(.52f+.48f*t);
                    var cornerLift=.22f*Mathf.Pow(Mathf.Abs(e.x*e.z),4)*Mathf.Pow(t,6);
                    return new Vector3(x,RoofHeight(t)-.22f*Mathf.Pow(t,6)+cornerLift,z);
                }
                var t0=ring/5f;var t1=(ring+1)/5f;var u0=section/8f;var u1=(section+1)/8f;
                var a=P(t0,u0);var b=P(t1,u0);var c=P(t1,u1);var d=P(t0,u1);
                if(Vector3.Cross(b-a,c-a).y<0) { Quad(d,c,b,a); Quad(a-Vector3.up*.07f,b-Vector3.up*.07f,c-Vector3.up*.07f,d-Vector3.up*.07f); }
                else { Quad(a,b,c,d); Quad(d-Vector3.up*.07f,c-Vector3.up*.07f,b-Vector3.up*.07f,a-Vector3.up*.07f); }
            }
            var mesh=new Mesh {name="Shared swept hip roof",vertices=vertices.ToArray(),triangles=indices.ToArray()};
            var uv = new Vector2[vertices.Count];
            for (var i=0;i<uv.Length;i++) uv[i] = new Vector2(vertices[i].x*3f, vertices[i].z*4f);
            mesh.uv=uv;
            mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
        }
        private static Mesh ArchMesh(float thickness)
        {
            var v=new List<Vector3>();var t=new List<int>();
            void Q(Vector3 a,Vector3 b,Vector3 c,Vector3 d){var n=v.Count;v.AddRange(new[]{a,b,c,d});t.AddRange(new[]{n,n+1,n+2,n,n+2,n+3});}
            for(var i=0;i<24;i++)
            {
                var a=i*Mathf.PI/24;var b=(i+1)*Mathf.PI/24;
                Vector3 P(float angle,float radius,float depth)=>new Vector3(Mathf.Cos(angle)*radius,Mathf.Sin(angle)*radius,depth);
                Q(P(a,1,-.5f),P(b,1,-.5f),P(b,(1f+thickness),-.5f),P(a,(1f+thickness),-.5f));
                Q(P(a,(1f+thickness),.5f),P(b,(1f+thickness),.5f),P(b,1,.5f),P(a,1,.5f));
                Q(P(a,1,.5f),P(b,1,.5f),P(b,1,-.5f),P(a,1,-.5f));
                Q(P(a,(1f+thickness),-.5f),P(b,(1f+thickness),-.5f),P(b,(1f+thickness),.5f),P(a,(1f+thickness),.5f));
            }
            var mesh=new Mesh{name="Shared city arch",vertices=v.ToArray(),triangles=t.ToArray()};mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
        }

        private static Texture2D SurfaceTexture(bool tiles)
        {
            const int size=128;
            var texture=new Texture2D(size,size,TextureFormat.RGBA32,true)
            { name=tiles ? "Shared ceramic rows 128" : "Shared aged lime plaster 128",wrapMode=TextureWrapMode.Repeat,filterMode=FilterMode.Trilinear,anisoLevel=2 };
            var pixels=new Color[size*size];
            for(var y=0;y<size;y++)for(var x=0;x<size;x++)
            {
                var grain=Mathf.PerlinNoise(x*.18f,y*.18f);
                var mottling=Mathf.PerlinNoise(x*.045f,y*.045f);
                float shade;
                if(tiles)
                {
                    var row=y/16;var column=(x+(row%2)*8)%16;
                    var ridge=.82f+.18f*Mathf.Sin(column/16f*Mathf.PI);
                    shade=(y%16<2 ? .63f : ridge)*(.92f+.08f*grain);
                }
                else shade=.79f+.14f*mottling+.07f*grain;
                pixels[y*size+x]=new Color(shade,shade,shade);
            }
            texture.SetPixels(pixels);texture.Apply(true,true);return texture;
        }

        private static Mesh FoliageMesh()
        {
            // Sparse double-sided folded leaves make real gaps in the canopy silhouette.
            // A single cluster is reused by every branch, with no alpha sorting or billboard.
            var vertices=new List<Vector3>();var indices=new List<int>();
            for(var leaf=0;leaf<48;leaf++)
            {
                var angle=leaf*2.399963f;
                var height=(leaf+.5f)/48f*2f-1f;
                var ring=Mathf.Sqrt(1-height*height);
                var center=new Vector3(Mathf.Cos(angle)*ring,height*.65f,Mathf.Sin(angle)*ring)*(.31f+.055f*Mathf.Sin(leaf*3.1f));
                var rotation=Quaternion.Euler(leaf*41f,leaf*137.5f,leaf*23f);
                var length=.14f+.045f*Mathf.Sin(leaf*1.8f);
                var n=vertices.Count;
                vertices.Add(center+rotation*new Vector3(0,0,-length));
                vertices.Add(center+rotation*new Vector3(-length*.52f,.025f,0));
                vertices.Add(center+rotation*new Vector3(0,0,length));
                vertices.Add(center+rotation*new Vector3(length*.52f,-.015f,0));
                indices.AddRange(new[]{n,n+1,n+2,n,n+2,n+3,n+2,n+1,n,n+3,n+2,n});
            }
            var mesh=new Mesh{name="Shared open canopy leaves",vertices=vertices.ToArray(),triangles=indices.ToArray()};
            // Backfaces share the front normal to avoid cancellation when recalculating double-sided faces.
            var normals=new Vector3[vertices.Count];
            for(var i=0;i<vertices.Count;i+=4)
            {var normal=Vector3.Cross(vertices[i+1]-vertices[i],vertices[i+2]-vertices[i]).normalized;for(var j=0;j<4;j++)normals[i+j]=normal;}
            mesh.normals=normals;mesh.RecalculateBounds();return mesh;
        }

        internal void House(Vector3 centre,float height)
        {
            var sign=centre.x<0?1f:-1f;var face=centre.x+sign*1.535f;
            for(var end=-1;end<=1;end+=2)
                Box(_wood,new Vector3(face,height*.5f,centre.z+end*2.3f),new Vector3(.17f,height,.17f));
            for(var level=0;level<2;level++)
                Box(_wood,new Vector3(face,level==0?.45f:height-.38f,centre.z),new Vector3(.14f,.15f,4.7f));
            for(var side=-1;side<=1;side+=2)
            {
                Box(_window,new Vector3(face-sign*.035f,1.5f,centre.z+side*1.65f),new Vector3(.02f,.74f,.72f));
                // Bracket arms under the eaves frame the walkable street at human scale.
                Beam(_wood,new Vector3(face,2.65f,centre.z+side*2.15f),new Vector3(face+sign*.32f,height-.08f,centre.z+side*2.15f),.12f);
            }
            Box(_wood,new Vector3(face+sign*.12f,height-.05f,centre.z),new Vector3(.38f,.18f,4.8f));
            // Upper clerestory and balcony-like rails give modules a second architectural rhythm.
            if(height>3)
            {
                Box(_teal,new Vector3(face+sign*.03f,height-.7f,centre.z),new Vector3(.035f,.48f,1.35f));
                for(var slat=-3;slat<=3;slat++)
                    Box(_gold,new Vector3(face+sign*.065f,height-.7f,centre.z+slat*.19f),new Vector3(.05f,.48f,.023f));
            }
        }

        internal void Landmarks()
        {
            // Gateway beyond the garden supplies the vista, never an invisible walking barrier.
            var z=44f;
            Box(_ivory,new Vector3(0,-.16f,51f),new Vector3(34f,.2f,48f));
            for(var side=-1;side<=1;side+=2)
            {
                Box(_ivory,new Vector3(side*5.65f,3.2f,z),new Vector3(2f,6.4f,2f));
                Box(_gold,new Vector3(side*5.65f,1f,z),new Vector3(2.1f,.24f,2.1f));
                Tower(new Vector3(side*7.3f,0,z+1.6f),side<0?11.4f:12.8f);
                Banner(new Vector3(side*5.65f,5.1f,z-1.1f),1.05f,2.7f);
            }
            Part(_arch,_ivory,new Vector3(0,5.5f,z),new Vector3(4.8f,4.8f,1.5f),Quaternion.identity);
            Part(_archTrim,_gold,new Vector3(0,5.5f,z-.81f),new Vector3(4.85f,4.85f,.12f),Quaternion.identity);
            // Circular spirit insignia above the opening.
            Ring(new Vector3(0,10.6f,z-1f),.63f,_gold);
            Beam(_gold,new Vector3(0,10f,z-1f),new Vector3(0,11.2f,z-1f),.1f);
            Roof(new Vector3(0,11.25f,z),new Vector3(3.4f,1.45f,1.65f));
            Tower(new Vector3(-13,0,58),15f);Tower(new Vector3(13,0,63),17f);Tower(new Vector3(1,0,69),20f);
            for(var side=-1;side<=1;side+=2)
            {
                for(var row=0;row<3;row++) Banner(new Vector3(side*3.95f,3.0f,1.8f+row*6),.52f,1.05f);
                Tree(new Vector3(side*8.1f,.025f,16.3f),4.4f,side<0);
                Tree(new Vector3(side*10.5f,.025f,29.2f),5.7f,true);
            }
            // Raised street curbs and inset strips bring scale without adding collision.
            for(var side=-1;side<=1;side+=2)
            {
                Box(_ivory,new Vector3(side*3.9f,.045f,7),new Vector3(.18f,.08f,33));
                Box(_gold,new Vector3(side*3.77f,.008f,7),new Vector3(.035f,.008f,33));
            }
        }
        private void Tower(Vector3 p,float height)
        {
            var levels=3;var step=height/3.9f;
            for(var level=0;level<levels;level++)
            {
                var width=2.8f-level*.45f;var bottom=level*step;
                Box(_ivory,p+Vector3.up*(bottom+step*.5f),new Vector3(width,step,width));
                for(var x=-1;x<=1;x+=2)for(var z=-1;z<=1;z+=2)
                    Box(_wood,p+new Vector3(x*(width*.5f-.1f),bottom+step*.5f,z*(width*.5f-.1f)),new Vector3(.18f,step,.18f));
                Box(_window,p+new Vector3(0,bottom+step*.58f,-width*.505f),new Vector3(width*.5f,step*.4f,.03f));
                Roof(p+Vector3.up*(bottom+step),new Vector3(width*.7f,.9f,width*.7f));
            }
            var top=p+Vector3.up*(3*step+1.1f);
            Beam(_gold,top,top+Vector3.up*.95f,.09f);
        }
        private void Banner(Vector3 p,float width,float height)
        {
            Box(_wood,p+new Vector3(0,height*.5f+.04f,0),new Vector3(width+.3f,.07f,.07f));
            Box(_teal,p,new Vector3(width,height,.035f));
            for(var s=-1;s<=1;s+=2)Box(_gold,p+Vector3.right*(s*(width*.5f-.025f)),new Vector3(.035f,height,.045f));
            Ring(p+Vector3.back*.028f,width*.24f,_gold);
            Beam(_gold,p+new Vector3(0,-width*.3f,-.04f),p+new Vector3(0,width*.3f,-.04f),.024f);
        }
        private void Ring(Vector3 p,float radius,Material material)
        {
            for(var i=0;i<32;i++)
            {
                Vector3 P(int j)=>p+new Vector3(Mathf.Cos(j*Mathf.PI/16),Mathf.Sin(j*Mathf.PI/16),0)*radius;
                Beam(material,P(i),P(i+1),radius*.075f);
            }
        }
        internal void Tree(Vector3 p,float height,bool blossom)
        {
            var top=p+Vector3.up*height*.72f;
            Beam(_wood,p,top+Vector3.right*.2f,height*.065f);
            for(var i=0;i<23;i++)
            {
                var angle=i*2.4f;var spread=height*(.13f+(i%4)*.055f);var branch=top+new Vector3(Mathf.Cos(angle)*spread,(i%5-2)*height*.055f,Mathf.Sin(angle)*spread);
                Beam(_wood,p+Vector3.up*height*.46f,branch,height*.026f);
                Part(_sphere,blossom?_pink:_leaf,branch,new Vector3(height*.24f,height*.20f,height*.23f),Quaternion.Euler(i*13,i*37,0));
            }
        }
        internal void Finish()
        {
            var triangles=0;
            foreach(var pair in _batches)
            {
                var mesh=new Mesh{name="City kit batch "+pair.Key.name,indexFormat=UnityEngine.Rendering.IndexFormat.UInt32};
                mesh.CombineMeshes(pair.Value.ToArray(),true,true);_meshes.Add(mesh);triangles+=mesh.triangles.Length/3;
                var go=new GameObject(mesh.name);go.transform.SetParent(transform,false);
                go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>().sharedMaterial=pair.Key;
                mesh.UploadMeshData(true);
            }
            Debug.Log("LGO_CITY_KIT materials="+_batches.Count+" triangles="+triangles+" textures=64x64+2x128x128 collision=existing_route");
            _batches.Clear();
        }
        private void OnDestroy()
        {
            foreach(var mesh in _meshes)if(mesh!=null)Destroy(mesh);
            foreach(var material in _materials)if(material!=null)Destroy(material);
            if(_woodTexture!=null)Destroy(_woodTexture);
            if(_plasterTexture!=null)Destroy(_plasterTexture);
            if(_tileTexture!=null)Destroy(_tileTexture);
        }
    }
}
