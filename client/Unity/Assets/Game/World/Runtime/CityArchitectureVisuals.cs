using System.Collections.Generic;
using LinhGioi.Art;
using UnityEngine;

namespace LinhGioi.World
{
    // A small reusable architecture kit; reference boards never enter the runtime.
    // Decorative geometry has no collision. The existing street owns navigation.
    internal sealed partial class CityArchitectureVisuals : MonoBehaviour
    {
        private readonly List<Mesh> _meshes = new List<Mesh>();
        private readonly List<Material> _materials = new List<Material>();
        private readonly Dictionary<Material, List<CombineInstance>> _batches = new Dictionary<Material, List<CombineInstance>>();
        private Mesh _cube, _roof, _arch, _sphere, _archTrim, _flowers, _needles;
        private Matrix4x4 _module = Matrix4x4.identity;
        internal void BeginModule(Vector3 centre, Quaternion rotation) => _module = Matrix4x4.TRS(centre, rotation, Vector3.one) * Matrix4x4.Translate(-centre);
        internal void EndModule() => _module = Matrix4x4.identity;
        private Material _gold, _wood, _navy, _ivory, _window, _teal, _leaf, _pink, _bark, _pineLeaf, _stone, _stoneLight;
        private int _treeTriangles;
        private Texture2D _woodTexture, _plasterTexture, _tileTexture;
        internal Texture2D PlasterTexture => _plasterTexture;
        internal Texture2D TimberTexture => _woodTexture;

        internal void Initialize()
        {
            _gold = Surface("Aged bronze", new Color(.65f,.43f,.17f), .5f);
            _wood = Surface("Aged timber", Color.white);
            _navy = Surface("Navy ceramic roof", new Color(.075f,.15f,.23f), .2f);
            _ivory = Surface("Ivory limestone", new Color(.72f,.65f,.51f));
            _window = Surface("Amber window recess", new Color(.21f,.125f,.07f));
            _teal = Surface("City teal banners", new Color(.065f,.30f,.34f));
            _leaf = Surface("Garden foliage", new Color(.14f,.27f,.105f));
            _pink = Surface("Blossom petals", new Color(.48f,.23f,.31f));
            _bark = Surface("Garden bark", new Color(.70f,.73f,.67f));
            _pineLeaf = Surface("Pine needles", new Color(.07f,.16f,.105f));
            _stone = Surface("Weathered grey masonry",new Color(.38f,.41f,.40f));
            _stoneLight = Surface("Worn stone coping",new Color(.55f,.56f,.52f));
            _woodTexture = Resources.Load<Texture2D>("LGOCitySurfaces/AgedTimber");
            if (_woodTexture == null) throw new System.InvalidOperationException("Missing shared city timber albedo.");
            _wood.mainTexture = _woodTexture;
            _bark.mainTexture = _woodTexture;
            _plasterTexture = SurfaceTexture(false);
            _tileTexture = SurfaceTexture(true);
            _ivory.mainTexture = _plasterTexture;
            _stone.mainTexture = _plasterTexture;
            _stoneLight.mainTexture = _plasterTexture;
            _navy.mainTexture = _tileTexture;
            _cube = Primitive(PrimitiveType.Cube);
            _sphere = FoliageMesh(false); _meshes.Add(_sphere);
            _flowers = FoliageMesh(true); _meshes.Add(_flowers);
            _needles = FoliageMesh(false,true); _meshes.Add(_needles);
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

        // Physical courses and projecting caps reuse the city stone albedo.
        // These are visual shells: the world retains its original collision boxes.
        internal void Masonry(Vector3 centre,Vector3 size,bool piers=false)
        {
            var alongZ=size.z>size.x;
            var length=alongZ?size.z:size.x;
            var depth=alongZ?size.x:size.z;
            var rotation=alongZ?Quaternion.Euler(0,90,0):Quaternion.identity;
            Vector3 At(float x,float y,float z)=>centre+rotation*new Vector3(x,y,z);
            void Block(Material m,float x,float y,float z,Vector3 dimensions)
                =>Part(_cube,m,At(x,y,z),dimensions,rotation);
            // Inset mortar core closes the joints without flattening the stone face relief.
            Block(_stone,0,0,0,new Vector3(length,size.y,Mathf.Max(.02f,depth-.025f)));
            var rows=Mathf.Max(1,Mathf.CeilToInt(size.y/.3f));
            var course=size.y/rows;
            for(var row=0;row<rows;row++)
            {
                var cursor=-length*.5f;
                while(cursor<length*.5f-.001f)
                {
                    var width=Mathf.Min(row%2==1 && cursor==-length*.5f?.44f:.88f,length*.5f-cursor);
                    Block(_stone,cursor+width*.5f,-size.y*.5f+(row+.5f)*course,0,
                        new Vector3(Mathf.Max(.01f,width-.016f),course-.012f,depth));
                    cursor+=width;
                }
            }
            var capCount=Mathf.CeilToInt(length/1.1f);var capWidth=length/capCount;
            for(var i=0;i<capCount;i++)
                Block(_stoneLight,-length*.5f+(i+.5f)*capWidth,size.y*.5f+.035f,0,
                    new Vector3(capWidth-.01f,.07f,depth+.07f));
            if(!piers)return;
            var spans=Mathf.Max(1,Mathf.CeilToInt(length/2.8f));
            for(var i=0;i<=spans;i++)
            {
                var x=-length*.5f+i*length/spans;
                Block(_stoneLight,x,0,0,new Vector3(.29f,size.y+.09f,depth+.10f));
                Block(_stone,x,-size.y*.5f+.09f,0,new Vector3(.38f,.18f,depth+.19f));
                Block(_stoneLight,x,size.y*.5f+.115f,0,new Vector3(.39f,.14f,depth+.20f));
            }
        }

        internal void StoneGatePost(Vector3 centre)
        {
            Masonry(centre,new Vector3(.8f,4.6f,.8f));
            Box(_stone,centre+Vector3.down*2.03f,new Vector3(1.02f,.54f,1.02f));
            Box(_stoneLight,centre+Vector3.down*1.73f,new Vector3(1.12f,.12f,1.12f));
            Box(_stoneLight,centre+Vector3.up*2.18f,new Vector3(1.03f,.15f,1.03f));
        }

        internal void Roof(Vector3 eave, Vector3 scale)
        {
            Part(_roof,_navy,eave,scale,Quaternion.identity);
            // Broad dark fascia and ceramic lips, as in the shared authoring roof family.
            for(var edge=0;edge<4;edge++)
            for(var step=0;step<8;step++)
            {
                var a=eave+Vector3.Scale(EavePoint(edge,step/8f),scale);
                var b=eave+Vector3.Scale(EavePoint(edge,(step+1)/8f),scale);
                Beam(_wood,a-Vector3.up*.07f,b-Vector3.up*.07f,.14f);
                Beam(_navy,a+Vector3.up*.035f,b+Vector3.up*.035f,.06f);
            }
            Beam(_navy,eave+Vector3.Scale(new Vector3(0,1.12f,-.52f),scale),eave+Vector3.Scale(new Vector3(0,1.12f,.52f),scale),.15f);
            // Physical tile ridges stay legible against the sky; the shared tile texture supplies joints.
            var rows=Mathf.Max(8,Mathf.CeilToInt(scale.z*2f/.22f));
            for(var row=0;row<=rows;row++) for(var side=-1;side<=1;side+=2)
            {
                var z=row/(float)rows*2f-1f;
                for(var step=0;step<8;step++)
                {
                    Vector3 P(float t) => eave+Vector3.Scale(new Vector3(side*t,
                        RoofHeight(t)-.22f*Mathf.Pow(t,6)+.22f*Mathf.Pow(Mathf.Abs(z),4)*Mathf.Pow(t,6)+.025f,
                        z*(.52f+.48f*t)),scale);
                    Beam(_navy,P(step/8f),P((step+1)/8f),.047f);
                }
            }
            for(var side=-1;side<=1;side+=2) for(var bay=-2;bay<=2;bay++)
            {
                var p=eave+new Vector3(side*scale.x*.8f,-.17f,bay*scale.z*.38f);
                Box(_wood,p,new Vector3(.45f,.13f,.18f));
                Beam(_wood,p+new Vector3(-side*.25f,-.32f,0),p+new Vector3(side*.22f,.10f,0),.12f);
            }
            for(var x=-1;x<=1;x+=2) for(var z=-1;z<=1;z+=2)
            {
                var corner=eave+Vector3.Scale(new Vector3(x,.34f,z),scale);
                Beam(_navy,corner,corner+new Vector3(x*.12f,.22f,z*.12f)*scale.y,.10f);
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

        private static Mesh FoliageMesh(bool flowers,bool needles=false)
        {
            var vertices=new List<Vector3>();var indices=new List<int>();var normals=new List<Vector3>();
            var count=flowers?24:(needles?192:64);
            for(var item=0;item<count;item++)
            {
                var angle=item*2.399963f;var y=(item+.5f)/count*2f-1f;
                var ring=Mathf.Sqrt(1-y*y);
                var center=new Vector3(Mathf.Cos(angle)*ring,y*.45f,Mathf.Sin(angle)*ring)*.42f;
                var rotation=Quaternion.Euler(item*17f,item*137.5f,item*7f);
                var petals=flowers?5:1;
                for(var petal=0;petal<petals;petal++)
                {
                    var orient=rotation*Quaternion.Euler(0,petal*72f,0);
                    var length=flowers?.075f:.14f+.035f*Mathf.Sin(item*1.8f);
                    var width=flowers?.04f:(needles?.014f:.052f);
                    var n=vertices.Count;
                    var origin=center+(flowers?orient*Vector3.forward*.04f:Vector3.zero);
                    vertices.Add(origin+orient*new Vector3(0,.025f,0));
                    normals.Add((center.normalized+Vector3.up*1.6f).normalized);
                    for(var edge=0;edge<6;edge++)
                    {
                        var theta=edge*Mathf.PI/3;
                        vertices.Add(origin+orient*new Vector3(Mathf.Sin(theta)*width,0,Mathf.Cos(theta)*length));
                        normals.Add((center.normalized+Vector3.up*1.6f).normalized);
                    }
                    for(var edge=0;edge<6;edge++)
                    {
                        var a=n+1+edge;var b=n+1+(edge+1)%6;
                        indices.AddRange(new[]{n,a,b,n,b,a});
                    }
                }
            }
            var mesh=new Mesh{name=flowers?"Shared five-petal blossom spray":"Shared curved leaf spray"};
            mesh.SetVertices(vertices);mesh.SetNormals(normals);mesh.SetTriangles(indices,0);mesh.RecalculateBounds();return mesh;
        }

        private void Branch(Vector3 start,Vector3 bend,Vector3 end,float radius,float groundY=float.NegativeInfinity)
        {
            const int rings=7,sides=8;
            var vertices=new List<Vector3>();var uv=new List<Vector2>();var indices=new List<int>();
            for(var ring=0;ring<rings;ring++)
            {
                var t=ring/(float)(rings-1);var point=(1-t)*(1-t)*start+2*(1-t)*t*bend+t*t*end;
                var tangent=(2*(1-t)*(bend-start)+2*t*(end-bend)).normalized;
                var across=Vector3.Cross(tangent,Mathf.Abs(tangent.y)>.9f?Vector3.forward:Vector3.up).normalized;
                var other=Vector3.Cross(tangent,across).normalized;var r=radius*Mathf.Lerp(1,.16f,t);
                for(var side=0;side<=sides;side++)
                {
                    var angle=side*Mathf.PI*2/sides;
                    var vertex=point+(across*Mathf.Cos(angle)+other*Mathf.Sin(angle))*r;
                    vertex.y=Mathf.Max(vertex.y,groundY);vertices.Add(vertex);
                    uv.Add(new Vector2(side/(float)sides,t*Vector3.Distance(start,end)/.6f));
                    if(ring==rings-1 || side==sides)continue;
                    var a=ring*(sides+1)+side;var b=a+sides+1;
                    indices.AddRange(new[]{a,a+1,b,a+1,b+1,b});
                }
            }
            // Seal both tube ends; roots enter soil and branch tips never expose a hollow cut.
            var baseCenter=vertices.Count;vertices.Add(start);uv.Add(Vector2.zero);
            var endCenter=vertices.Count;vertices.Add(end);uv.Add(Vector2.one);
            for(var side=0;side<sides;side++)
            {
                indices.AddRange(new[]{baseCenter,side+1,side});
                var last=(rings-1)*(sides+1);indices.AddRange(new[]{endCenter,last+side,last+side+1});
            }
            var mesh=new Mesh{name="Tapered curved garden branch"};mesh.SetVertices(vertices);mesh.SetUVs(0,uv);mesh.SetTriangles(indices,0);
            mesh.RecalculateNormals();mesh.RecalculateBounds();_meshes.Add(mesh);
            Part(mesh,_bark,Vector3.zero,Vector3.one,Quaternion.identity);
        }

        // The same timber / lattice / gallery family as linh-thanh-kit/build_preview.py.
        // All coordinates stay inside the existing house footprint except overhead eaves.
        internal void House(Vector3 centre,float height)
        {
            var sign=centre.x<0?1f:-1f;
            Vector3 P(float depth,float y,float along)=>centre+new Vector3(sign*depth,y,along);
            void B(Material m,float depth,float y,float along,float d,float h,float w)
                =>Box(m,P(depth,y,along),new Vector3(d,h,w));
            void Window(float depth,float y,float along,float h,float w)
            {
                B(_wood,depth,y,along,.12f,h+.18f,w+.18f);
                B(_ivory,depth+.075f,y,along,.035f,h,w);
                for(var slat=-2;slat<=2;slat++) B(_wood,depth+.11f,y,along+slat*w/5f,.055f,h,.038f);
                for(var rail=-1;rail<=1;rail+=2) B(_wood,depth+.11f,y+rail*h*.27f,along,.055f,.04f,w);
                B(_wood,depth+.10f,y-h*.5f-.11f,along,.32f,.12f,w+.3f);
            }
            B(_wood,0,height+.09f,0,3.04f,.34f,4.84f);
            B(_ivory,0,.12f,0,3.14f,.24f,4.94f);
            B(_ivory,0,.32f,0,3.08f,.13f,4.88f);
            // Continuous frame wraps the visible front and both gables.
            foreach(var depth in new[]{-1.52f,1.52f})
            {
                foreach(var along in new[]{-2.3f,0f,2.3f}) B(_wood,depth,height*.5f,along,.18f,height,.18f);
                foreach(var y in new[]{.48f,2.35f,height-.08f}) B(_wood,depth,y,0,.17f,.17f,4.7f);
            }
            foreach(var along in new[]{-2.41f,2.41f})
            {
                foreach(var y in new[]{.48f,2.35f,height-.08f}) B(_wood,0,y,along,3.1f,.14f,.14f);
                foreach(var depth in new[]{-.8f,.8f}) B(_wood,depth,height*.5f,along,.12f,height,.12f);
            }
            foreach(var side in new[]{-1f,1f}) Window(1.55f,1.5f,side*1.52f,1.13f,1.03f);
            B(_wood,1.57f,1.25f,0,.13f,1.95f,1.10f);
            for(var plank=-3;plank<=3;plank++) B(_window,1.65f,1.25f,plank*.137f,.04f,1.74f,.115f);
            foreach(var y in new[]{.61f,1.65f}) B(_wood,1.69f,y,0,.055f,.10f,1.04f);
            foreach(var side in new[]{-1f,1f}) B(_gold,1.70f,1.21f,side*.11f,.045f,.13f,.045f);
            // Lanterns hang above walking headroom. No protruding ground props in the route.
            foreach(var along in new[]{-2.05f,2.05f})
            {
                B(_wood,1.73f,height-.20f,along,.53f,.10f,.10f);
                B(_gold,1.91f,height-.40f,along,.025f,.28f,.025f);
                B(_ivory,1.91f,height-.77f,along,.24f,.43f,.28f);
                foreach(var y in new[]{height-1.01f,height-.53f}) B(_wood,1.91f,y,along,.32f,.06f,.37f);
                foreach(var offset in new[]{-.16f,.16f}) B(_wood,2.05f,height-.77f,along+offset,.04f,.46f,.04f);
            }
            if(height<3f) return;
            // A narrower upper room rises out of the lower roof, replacing the flat repeated skyline.
            var upperHeight=centre.z>5f && centre.z<7f ? 2.3f : 1.35f;
            var floor=height+.35f;var top=floor+upperHeight;
            var roomBottom=height-.1f;var roomHeight=top-roomBottom;
            B(_ivory,0,roomBottom+roomHeight*.5f,0,2.40f,roomHeight,3.4f);
            foreach(var depth in new[]{-1.23f,1.23f})
            {
                foreach(var along in new[]{-1.65f,0f,1.65f}) B(_wood,depth,roomBottom+roomHeight*.5f,along,.14f,roomHeight,.14f);
                foreach(var y in new[]{floor+.05f,top-.05f}) B(_wood,depth,y,0,.15f,.14f,3.5f);
            }
            foreach(var side in new[]{-1f,1f}) Window(1.24f,floor+upperHeight*.55f,side*.83f,upperHeight*.64f,1.14f);
            B(_wood,1.42f,floor-.04f,0,.78f,.13f,3.6f);
            foreach(var along in new[]{-1.65f,0f,1.65f})
                Beam(_wood,P(1.2f,height-.15f,along),P(1.77f,floor-.09f,along),.15f);
            foreach(var y in new[]{floor+.22f,floor+.65f}) B(_wood,1.79f,y,0,.075f,.075f,3.5f);
            for(var rail=-5;rail<=5;rail++) B(_wood,1.79f,floor+.39f,rail*.33f,.055f,.53f,.055f);
            Roof(centre+Vector3.up*(top+.04f),new Vector3(1.53f,.70f,2.04f));
            // Door shelter: half-width 1 m leaves the existing lantern at z=10.7 outside its edge.
            if(centre.x>0 && centre.z>11f && centre.z<13f)
            {
                Part(_cube,_ivory,P(1.88f,2.43f,0),new Vector3(.84f,.055f,2.0f),Quaternion.Euler(0,0,-sign*10f));
                B(_teal,2.28f,2.28f,0,.04f,.16f,2.0f);
                B(_wood,1.76f,2.04f,1.84f,.12f,.45f,.55f);
                B(_ivory,1.84f,2.05f,1.8f,.025f,.18f,.18f);
                B(_ivory,1.84f,2.05f,1.93f,.025f,.12f,.065f);
            }
        }

        internal void Landmarks()
        {
            // Gateway beyond the garden supplies the vista, never an invisible walking barrier.
            var z=44f;
            GatewayGardens();
            CityBackdrop();
            for(var side=-1;side<=1;side+=2)
            {
                Box(_ivory,new Vector3(side*5.65f,3.2f,z),new Vector3(2f,6.4f,2f));
                Box(_gold,new Vector3(side*5.65f,1f,z),new Vector3(2.1f,.24f,2.1f));
                Tower(new Vector3(side*8.8f,0,z+1.6f),side<0?11.4f:12.8f);
                Banner(new Vector3(side*5.65f,5.1f,z-1.1f),1.05f,2.7f);
            }
            Part(_arch,_ivory,new Vector3(0,5.5f,z),new Vector3(4.8f,4.8f,1.5f),Quaternion.identity);
            Part(_archTrim,_gold,new Vector3(0,5.5f,z-.81f),new Vector3(4.85f,4.85f,.12f),Quaternion.identity);
            // Circular spirit insignia above the opening.
            Part(_cube,_teal,new Vector3(0,10.6f,z-.94f),new Vector3(.87f,.87f,.10f),Quaternion.Euler(0,0,45));
            Ring(new Vector3(0,10.6f,z-1.01f),.63f,_gold);
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
        internal void SquareBackdrop()
        {
            Tower(new Vector3(-15,0,31),13f);Tower(new Vector3(14,0,33),15f);
            Tower(new Vector3(-6,0,39),18f);Tower(new Vector3(6,0,45),21f);
            Tower(new Vector3(-24,0,18),15f);Tower(new Vector3(25,0,21),17f);
        }

        private void CityBackdrop()
        {
            // Layered inhabited skyline from the arrival composition. Scenery beyond the
            // playable boundary shares materials and uses roof silhouettes, not near tile ribs.
            Box(_stone,new Vector3(0,-.3f,94),new Vector3(82,.5f,65));
            for(var row=0;row<3;row++)
            for(var column=-4;column<=4;column++)
            {
                var x=column*8.8f+(row%2==0?0:3.4f);
                var z=72f+row*17f+Mathf.Sin(column*2.1f+row)*2.4f;
                var levels=3+row+(Mathf.Abs(column+row)%3);
                var step=3.1f;
                var width=5.2f+(Mathf.Abs(column)%2)*1.2f;
                var p=new Vector3(x,row*.9f,z);
                if(row==2 && column==0)levels=8;
                // Each raised row rests on a stone foundation down to the shared ground.
                Box(_stone,new Vector3(x,(p.y-.05f)*.5f,z),new Vector3(width+.3f,p.y+.05f,width+.3f));
                _module=Matrix4x4.Translate(p)*Matrix4x4.Scale(new Vector3(width/4f,1,width/4f))*Matrix4x4.Translate(-p);
                Tower(p,levels*step,levels,true);
                EndModule();
            }
            Debug.Log("LGO_CITY_BACKDROP buildings=27 rows=3 collision=none roof=shared_silhouette");
        }

        private void GatewayGardens()
        {
            // Reference city courtyards: distant scenery only, beyond the existing garden boundary.
            Box(_leaf,new Vector3(0,-.18f,51f),new Vector3(38f,.2f,48f));
            Box(_ivory,new Vector3(0,-.07f,51f),new Vector3(10f,.12f,48f));
            Box(_ivory,new Vector3(0,-.04f,42.5f),new Vector3(33f,.10f,4f));
            for(var side=-1;side<=1;side+=2)
            {
                Box(_ivory,new Vector3(side*18f,1.1f,49f),new Vector3(.45f,2.2f,44f));
                Box(_navy,new Vector3(side*18f,2.25f,49f),new Vector3(.72f,.16f,44f));
                // Garden terraces break up the open plane while keeping the central gate vista clear.
                for(var row=0;row<2;row++)
                {
                    var z=32f+row*6.1f;
                    Box(_ivory,new Vector3(side*8.8f,.11f,z),new Vector3(6.3f,.22f,4.8f));
                    Box(_leaf,new Vector3(side*8.8f,.24f,z),new Vector3(5.9f,.06f,4.4f));
                    if(row==1) Tree(new Vector3(side*10.1f,.28f,z),4.1f,false);
                }
                for(var row=0;row<2;row++)
                {
                    var centre=new Vector3(side*14.8f,0,32.5f+row*6.1f);
                    Box(_ivory,centre+Vector3.up*.16f,new Vector3(3.4f,.32f,6f));
                    Roof(centre+Vector3.up*3.15f,new Vector3(1.95f,.8f,3.2f));
                    for(var x=-1;x<=1;x+=2)for(var z=-1;z<=1;z+=2)
                    {
                        var post=centre+new Vector3(x*1.38f,1.73f,z*2.75f);
                        Box(_wood,post,new Vector3(.18f,2.82f,.18f));
                        Beam(_wood,post+Vector3.up*.85f,post+new Vector3(-x*.35f,1.35f,0),.12f);
                    }
                    for(var x=-1;x<=1;x+=2)
                        Box(_wood,centre+new Vector3(x*1.38f,2.99f,0),new Vector3(.18f,.18f,5.7f));
                    // Solid rear parapet and sparse rails ground the covered gallery at human scale.
                    Box(_ivory,centre+new Vector3(side*1.42f,.63f,0),new Vector3(.20f,.62f,5.7f));
                    Box(_wood,centre+new Vector3(side*1.42f,1.01f,0),new Vector3(.24f,.10f,5.8f));
                }
            }
        }

        private void Tower(Vector3 p,float height,int levels=3,bool distant=false)
        {
            var step=height/(levels+.9f);
            for(var level=0;level<levels;level++)
            {
                var width=distant?Mathf.Max(2f,4f-level*.30f):4.0f-level*.60f;
                var bottom=level*step;
                // Stone base and recessed upper chambers sit inside a visible timber frame.
                Box(level==0?_ivory:_window,p+Vector3.up*(bottom+step*.5f),new Vector3(width*.9f,step,width*.9f));
                Box(_ivory,p+Vector3.up*(bottom+.12f),new Vector3(width+ .2f,.24f,width+.2f));
                Box(_wood,p+Vector3.up*(bottom+step-.12f),new Vector3(width+.12f,.22f,width+.12f));
                for(var x=-1;x<=1;x+=2)for(var z=-1;z<=1;z+=2)
                    Box(_wood,p+new Vector3(x*(width*.5f-.06f),bottom+step*.5f,z*(width*.5f-.06f)),new Vector3(.21f,step,.21f));
                // All four faces share the same lattice and balcony construction.
                for(var face=0;face<4;face++)
                {
                    var rotation=Quaternion.Euler(0,face*90f,0);
                    Vector3 At(float x,float y,float z)=>p+rotation*new Vector3(x,bottom+y,z);
                    void Panel(Material material,float x,float y,float z,Vector3 size)
                        =>Part(_cube,material,At(x,y,z),size,rotation);
                    var front=-width*.5f-.025f;
                    if(level==0)
                        Panel(_window,0,step*.51f,front,new Vector3(width*.48f,step*.62f,.055f));
                    else
                    {
                        Panel(distant?_window:_teal,0,step*.60f,front+.06f,new Vector3(width*.51f,step*.43f,.04f));
                        Panel(_wood,0,.23f,front-.27f,new Vector3(width+.22f,.14f,.66f));
                        Panel(_wood,0,.86f,front-.54f,new Vector3(width+.24f,.09f,.09f));
                        Panel(_wood,0,.43f,front-.54f,new Vector3(width+.24f,.065f,.065f));
                        for(var rail=-4;rail<=4;rail++)
                            Panel(_wood,rail*width/8f,.64f,front-.54f,new Vector3(.055f,.44f,.055f));
                    }
                    for(var side=-1;side<=1;side+=2)
                        Panel(_wood,side*width*.27f,step*.57f,front-.025f,new Vector3(.12f,step*.68f,.14f));
                    Panel(_wood,0,step*.88f,front-.025f,new Vector3(width*.61f,.12f,.14f));
                    for(var slat=-2;slat<=2;slat++)
                        Panel(_gold,slat*width*.09f,step*.62f,front-.03f,new Vector3(.028f,step*.43f,.05f));
                    for(var side=-1;side<=1;side+=2)
                        Beam(_wood,At(side*width*.40f,step-.57f,front),At(side*width*.40f,step-.07f,front-.47f),.13f);
                }
                var roofPoint=p+Vector3.up*(bottom+step);
                var roofScale=new Vector3(width*.68f,.95f,width*.68f);
                if(distant) Part(_roof,_navy,roofPoint,roofScale,Quaternion.identity);
                else Roof(roofPoint,roofScale);
            }
            var top=p+Vector3.up*(levels*step+1.1f);
            Beam(_gold,top,top+Vector3.up*.75f,.09f);
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
        internal void Tree(Vector3 p,float height,bool blossom,bool pine=false,string name="Garden tree")
        {
            var treeMaterials=new[]{_bark,_leaf,_pink,_pineLeaf};var starts=new int[4];
            for(var i=0;i<4;i++) starts[i]=_batches.TryGetValue(treeMaterials[i],out var old)?old.Count:0;
            var trunkTop=p+new Vector3(.13f,height*.89f,.025f);
            Branch(p+Vector3.up*.025f,p+new Vector3(-height*.17f,height*.42f,.08f),trunkTop,height*.067f,p.y-.015f);
            // Low roots meet the bed; the broad canopy starts well above human head height.
            for(var root=0;root<5;root++)
            {
                var angle=root*Mathf.PI*2/5;var foot=p+new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*height*.10f;
                Branch(p+Vector3.up*.28f,p+new Vector3(Mathf.Cos(angle)*.2f,.13f,Mathf.Sin(angle)*.2f),foot,height*.027f);
            }
            for(var limb=0;limb<12;limb++)
            {
                var tier=limb/4;var angle=limb*2.399963f;
                var start=p+new Vector3(-height*.045f,height*(.45f+tier*.15f),0);
                var reach=height*(.34f-tier*.065f)*(1+.13f*Mathf.Sin(limb*4.1f));
                var tip=p+new Vector3(Mathf.Cos(angle)*reach,height*(.56f+tier*.16f+Mathf.Sin(limb*1.7f)*(pine?.028f:.09f)),Mathf.Sin(angle)*reach);
                Branch(start,(start+tip)*.5f-Vector3.up*height*.075f,tip,height*(.024f-tier*.004f));
                for(var twig=0;twig<3;twig++)
                {
                    var offset=Quaternion.Euler(0,angle*Mathf.Rad2Deg+twig*90f,0)*new Vector3(height*.11f,height*.015f,0);
                    var end=tip+offset;
                    Branch(Vector3.Lerp(start,tip,.70f),tip+Vector3.up*.10f,end,height*.009f);
                    var scale=new Vector3(height*.25f,height*(pine?.14f:.22f),height*.24f);
                    Part(blossom?_flowers:(pine?_needles:_sphere),blossom?_pink:(pine?_pineLeaf:_leaf),end,scale,Quaternion.Euler(0,limb*37+twig*71,0));
                    if(blossom && twig==0) Part(_sphere,_leaf,end-Vector3.up*.16f,scale*.78f,Quaternion.Euler(0,limb*37,0));
                }
            }
            if(pine) Part(_needles,_pineLeaf,trunkTop,new Vector3(height*.22f,height*.13f,height*.22f),Quaternion.identity);
            // Per-tree material batches permit real bounds/shadows and targeted runtime inspection.
            var tree=new GameObject(name);tree.transform.SetParent(transform,false);tree.transform.localPosition=p;
            for(var i=0;i<4;i++)
            {
                if(!_batches.TryGetValue(treeMaterials[i],out var list) || list.Count==starts[i])continue;
                var parts=list.GetRange(starts[i],list.Count-starts[i]);list.RemoveRange(starts[i],list.Count-starts[i]);
                for(var j=0;j<parts.Count;j++) {var part=parts[j];part.transform=Matrix4x4.Translate(-p)*part.transform;parts[j]=part;}
                var mesh=new Mesh{name=name+" "+treeMaterials[i].name,indexFormat=UnityEngine.Rendering.IndexFormat.UInt32};
                mesh.CombineMeshes(parts.ToArray(),true,true);_meshes.Add(mesh);_treeTriangles+=mesh.triangles.Length/3;
                var child=new GameObject(treeMaterials[i].name);child.transform.SetParent(tree.transform,false);
                child.AddComponent<MeshFilter>().sharedMesh=mesh;child.AddComponent<MeshRenderer>().sharedMaterial=treeMaterials[i];mesh.UploadMeshData(true);
            }
        }
        internal void Finish()
        {
            var triangles=_treeTriangles;
            foreach(var pair in _batches)
            {
                if(pair.Value.Count==0)continue;
                var mesh=new Mesh{name="City kit batch "+pair.Key.name,indexFormat=UnityEngine.Rendering.IndexFormat.UInt32};
                mesh.CombineMeshes(pair.Value.ToArray(),true,true);_meshes.Add(mesh);triangles+=mesh.triangles.Length/3;
                var go=new GameObject(mesh.name);go.transform.SetParent(transform,false);
                go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>().sharedMaterial=pair.Key;
                mesh.UploadMeshData(true);
            }
            Debug.Log("LGO_CITY_KIT materials="+_batches.Count+" triangles="+triangles+" textures=shared_timber_1024+2x128x128 collision=existing_route");
            _batches.Clear();
        }
        private void OnDestroy()
        {
            foreach(var mesh in _meshes)if(mesh!=null)Destroy(mesh);
            foreach(var material in _materials)if(material!=null)Destroy(material);
            // Resource albedo belongs to the asset system and can be reused by the next city instance.
            if(_plasterTexture!=null)Destroy(_plasterTexture);
            if(_tileTexture!=null)Destroy(_tileTexture);
        }
    }
}
