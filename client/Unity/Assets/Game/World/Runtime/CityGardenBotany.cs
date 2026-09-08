using System.Collections.Generic;
using UnityEngine;

namespace LinhGioi.World
{
    internal sealed partial class CityArchitectureVisuals
    {
        private Mesh _gardenShrub,_gardenBlooms,_gardenGrass,_canopyLeaves,_canopyFlowers;
        private Material _gardenWhite,_gardenFresh;
        private bool _squareBotany;

        private void InitializeGardenBotany()
        {
            _gardenWhite=Surface("Garden ivory petals",new Color(.91f,.85f,.65f));
            _gardenFresh=Surface("Fresh garden leaves",new Color(.24f,.38f,.13f));
            _gardenShrub=BotanicalMesh(0);_gardenBlooms=BotanicalMesh(1);_gardenGrass=BotanicalMesh(2);
            _canopyLeaves=BotanicalMesh(3);_canopyFlowers=BotanicalMesh(4);
            _meshes.Add(_canopyLeaves);_meshes.Add(_canopyFlowers);
            _meshes.Add(_gardenShrub);_meshes.Add(_gardenBlooms);_meshes.Add(_gardenGrass);
        }

        // Shared solid, double-sided plant meshes. No alpha cards or individual leaf GameObjects.
        private static Mesh BotanicalMesh(int kind)
        {
            var vertices=new List<Vector3>();var normals=new List<Vector3>();var triangles=new List<int>();
            void Face(Vector3 a,Vector3 b,Vector3 c,Vector3 normal)
            {
                var start=vertices.Count;vertices.AddRange(new[]{a,b,c});
                normals.AddRange(new[]{normal,normal,normal});
                triangles.AddRange(new[]{start,start+1,start+2,start+2,start+1,start});
            }
            void Blade(Vector3 root,Quaternion rotation,float length,float width,float arch)
            {
                Vector3 P(float t,float side)
                {
                    var breadth=width*Mathf.Pow(Mathf.Max(0,Mathf.Sin(t*Mathf.PI)),.65f);
                    return root+rotation*new Vector3(side*breadth,arch*Mathf.Sin(t*Mathf.PI)-Mathf.Abs(side)*width*.22f,t*length);
                }
                for(var segment=0;segment<4;segment++)
                {
                    var t=segment/4f;var next=(segment+1)/4f;
                    var n=(rotation*Vector3.up+Vector3.up*1.4f).normalized;
                    foreach(var side in new[]{-1f,1f})
                    {
                        Face(P(t,0),P(next,0),P(next,side),n);
                        Face(P(t,0),P(next,side),P(t,side),n);
                    }
                }
            }
            void Stem(Vector3 from,Vector3 to,float width)
            {
                Face(from+Vector3.left*width,to+Vector3.left*width,to+Vector3.right*width,Vector3.back);
                Face(from+Vector3.left*width,to+Vector3.right*width,from+Vector3.right*width,Vector3.back);
                Face(from+Vector3.back*width,to+Vector3.back*width,to+Vector3.forward*width,Vector3.right);
                Face(from+Vector3.back*width,to+Vector3.forward*width,from+Vector3.forward*width,Vector3.right);
            }
            if(kind==3 || kind==4)
            {
                var count=kind==3?36:16;
                for(var item=0;item<count;item++)
                {
                    var y=(item+.5f)/count*2f-1f;var angle=item*2.399963f;
                    var ring=Mathf.Sqrt(1-y*y);
                    var root=new Vector3(Mathf.Cos(angle)*ring*.31f,y*.24f,Mathf.Sin(angle)*ring*.31f);
                    if(kind==3)
                    {
                        Stem(root*.35f,root,.003f);
                        Blade(root,Quaternion.LookRotation(root.normalized,Vector3.up),.19f,.067f,.033f);
                    }
                    else
                    {
                        var orientation=Quaternion.FromToRotation(Vector3.up,(root.normalized+Vector3.up*.6f).normalized);
                        for(var petal=0;petal<5;petal++)
                            Blade(root,orientation*Quaternion.Euler(-12f,petal*72f,0),.105f,.050f,.021f);
                    }
                }
            }
            else if(kind==0 || kind==2)
            {
                var fronds=kind==2?11:7;
                for(var i=0;i<fronds;i++)
                {
                    var yaw=i*137.5f;var rotation=Quaternion.Euler(-25f+(i%3)*8f,yaw,0);
                    var tip=rotation*new Vector3(0,.12f,.34f+(i%3)*.08f);
                    Stem(Vector3.zero,tip,.006f);
                    if(kind==2) Blade(Vector3.zero,Quaternion.Euler(-64f+(i%3)*12f,yaw,0),.46f+(i%4)*.07f,.017f,.10f);
                    else for(var pair=1;pair<=3;pair++)for(var side=-1;side<=1;side+=2)
                    {
                        var p=Vector3.Lerp(Vector3.zero,tip,pair/4f);
                        Blade(p,Quaternion.Euler(-8f,yaw+side*56f,side*12f),.23f-pair*.025f,.057f,.045f);
                    }
                }
            }
            else if(kind==1)
            {
                for(var flower=0;flower<7;flower++)
                {
                    var theta=flower*2.399963f;var r=flower==0?0f:.15f;
                    var root=new Vector3(Mathf.Cos(theta)*r,.23f+(flower%3)*.055f,Mathf.Sin(theta)*r);
                    for(var petal=0;petal<5;petal++)
                        Blade(root,Quaternion.Euler(-14f,petal*72f+flower*17f,0),.105f,.048f,.023f);
                    // A small cupped centre keeps flowers distinct from star-shaped leaf clumps.
                    for(var edge=0;edge<10;edge++)
                    {
                        var a=new Vector3(Mathf.Cos(edge*Mathf.PI/5),0,Mathf.Sin(edge*Mathf.PI/5))*.022f;
                        var b=new Vector3(Mathf.Cos((edge+1)*Mathf.PI/5),0,Mathf.Sin((edge+1)*Mathf.PI/5))*.022f;
                        Face(root+Vector3.up*.018f,root+a,root+b,Vector3.up);
                    }
                }
            }
            var mesh=new Mesh{name=kind==0?"Shared paired-leaf garden shrub":kind==1?"Shared cupped five-petal flowers":"Shared curved grass tuft"};
            mesh.SetVertices(vertices);mesh.SetNormals(normals);mesh.SetTriangles(triangles,0);mesh.RecalculateBounds();return mesh;
        }

        private void GardenPlant(Vector3 point,int variant,bool pink,float scale,Quaternion rotation)
        {
            var material=variant%2==0?_leaf:_gardenFresh;
            Part(_gardenShrub,material,point,Vector3.one*scale,rotation);
            if(variant%3==0)
                Part(_gardenBlooms,pink?_pink:_gardenWhite,point,Vector3.one*scale,rotation);
            else Part(_gardenGrass,material,point,Vector3.one*(scale*.72f),rotation);
        }
    }
}
