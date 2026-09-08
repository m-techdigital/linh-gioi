using System;
using System.Collections.Generic;
using UnityEngine;

namespace LinhGioi.World
{
    // Semantic prop modules: lathed ceramics, swept hollow spout/handles and forged tools.
    // Components remain editable here, then join the city's material batches in Finish().
    internal sealed partial class CityArchitectureVisuals
    {
        private Material _ceramic, _clay, _iron;
        private Mesh _potBody, _potLid, _teaCup, _teaSaucer, _potSpout, _potHandle;
        private Mesh _toolHandle, _forgedBlock, _fittingCrystal;

        private void InitializeEverydayProps()
        {
            if(_potBody!=null) return;
            _ceramic=Surface("Celadon tea ware",new Color(.49f,.65f,.56f));
            _ceramic.SetFloat("_Smoothness",.62f);
            _clay=Surface("Warm fired stoneware",new Color(.38f,.15f,.075f));
            _clay.SetFloat("_Smoothness",.38f);
            _iron=Surface("Forged dark iron",new Color(.22f,.25f,.27f),.72f);
            _iron.SetFloat("_Smoothness",.44f);
            _potBody=LathedProp("Teapot body",new[]{V(0,0),V(.09f,0),V(.11f,.012f),V(.14f,.045f),V(.152f,.095f),V(.146f,.145f),V(.124f,.185f),V(.096f,.205f),V(.095f,.218f)},32);
            _potLid=LathedProp("Teapot fitted lid and knob",new[]{V(0,0),V(.098f,0),V(.102f,.007f),V(.091f,.016f),V(.060f,.03f),V(.014f,.033f),V(.015f,.044f),V(.022f,.05f),V(.019f,.061f),V(0,.067f)},32);
            // Profile returns down the inner wall and across the inner floor: the cup has a real cavity.
            _teaCup=LathedProp("Open tea cup with inner bowl",new[]{V(0,0),V(.027f,0),V(.030f,.005f),V(.039f,.009f),V(.049f,.035f),V(.057f,.059f),V(.054f,.062f),V(.050f,.058f),V(.043f,.034f),V(.034f,.012f),V(0,.010f)},28);
            _teaSaucer=LathedProp("Tea saucer",new[]{V(0,0),V(.035f,0),V(.075f,.008f),V(.077f,.015f),V(.069f,.014f),V(.039f,.005f),V(0,.005f)},28);
            _potSpout=SweptProp("Hollow rising teapot spout",new[]{new Vector3(.115f,.08f,0),new Vector3(.155f,.097f,0),new Vector3(.185f,.13f,0),new Vector3(.212f,.168f,0),new Vector3(.241f,.211f,0)},new[]{.043f,.035f,.027f,.022f,.020f},16,true);
            var handle=new Vector3[17];var radii=new float[17];
            for(var i=0;i<handle.Length;i++)
            {
                var t=i/16f;
                handle[i]=new Vector3(-.116f-Mathf.Sin(t*Mathf.PI)*.115f,.056f+t*.135f,0);
                radii[i]=.015f;
            }
            _potHandle=SweptProp("Continuous teapot loop handle",handle,radii,12,false);
            _toolHandle=LathedProp("Tapered wooden tool handle",new[]{V(0,-.5f),V(.36f,-.5f),V(.50f,-.43f),V(.42f,-.12f),V(.35f,.3f),V(.40f,.47f),V(0,.5f)},12);
            _forgedBlock=BevelledProp();
            _fittingCrystal=LathedProp("Faceted mineral blank",new[]{V(0,0),V(.7f,.04f),V(1,.22f),V(.85f,.7f),V(0,1)},6,true);
        }

        private static Vector2 V(float radius,float height)=>new Vector2(radius,height);

        private Mesh LathedProp(string name,Vector2[] profile,int sides,bool faceted=false)
        {
            var vertices=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();
            for(var row=0;row<profile.Length;row++)for(var side=0;side<=sides;side++)
            {
                var angle=side*Mathf.PI*2/sides;
                vertices.Add(new Vector3(Mathf.Cos(angle)*profile[row].x,profile[row].y,Mathf.Sin(angle)*profile[row].x));
                uv.Add(new Vector2(side/(float)sides,profile[row].y));
            }
            for(var row=0;row<profile.Length-1;row++)for(var side=0;side<sides;side++)
            {
                var a=row*(sides+1)+side;var b=a+sides+1;
                triangles.AddRange(new[]{a,b,b+1,a,b+1,a+1});
            }
            if(faceted)
            {
                var flat=new List<Vector3>();var flatUv=new List<Vector2>();var indices=new List<int>();
                foreach(var i in triangles){indices.Add(flat.Count);flat.Add(vertices[i]);flatUv.Add(uv[i]);}
                return StoreProp(name,flat,indices,flatUv);
            }
            return StoreProp(name,vertices,triangles,uv);
        }

        private Mesh SweptProp(string name,Vector3[] centres,float[] radius,int sides,bool hollow)
        {
            var vertices=new List<Vector3>();var triangles=new List<int>();var uv=new List<Vector2>();
            var rings=centres.Length;
            for(var shell=0;shell<(hollow?2:1);shell++)for(var ring=0;ring<rings;ring++)
            {
                var tangent=(centres[Mathf.Min(ring+1,rings-1)]-centres[Mathf.Max(0,ring-1)]).normalized;
                // These authored paths lie in XY, so a constant Z binormal avoids frame twisting.
                var normal=Vector3.Cross(tangent,Vector3.forward).normalized;
                for(var side=0;side<=sides;side++)
                {
                    var angle=side*Mathf.PI*2/sides;
                    vertices.Add(centres[ring]+(normal*Mathf.Cos(angle)+Vector3.forward*Mathf.Sin(angle))*(radius[ring]-(shell==1?.005f:0)));
                    uv.Add(new Vector2(side/(float)sides,ring/(float)(rings-1)));
                }
            }
            for(var shell=0;shell<(hollow?2:1);shell++)for(var ring=0;ring<rings-1;ring++)for(var side=0;side<sides;side++)
            {
                var a=(shell*rings+ring)*(sides+1)+side;var b=a+sides+1;
                if(shell==0)triangles.AddRange(new[]{a,b,b+1,a,b+1,a+1});
                else triangles.AddRange(new[]{a,b+1,b,a,a+1,b+1});
            }
            if(hollow)for(var side=0;side<sides;side++)
            {
                var a=(rings-1)*(sides+1)+side;var b=a+rings*(sides+1);
                triangles.AddRange(new[]{a,b,b+1,a,b+1,a+1});
            }
            return StoreProp(name,vertices,triangles,uv);
        }

        private Mesh BevelledProp()
        {
            var points=new[]{new Vector2(-.36f,-.5f),new Vector2(.36f,-.5f),new Vector2(.5f,-.36f),new Vector2(.5f,.36f),new Vector2(.36f,.5f),new Vector2(-.36f,.5f),new Vector2(-.5f,.36f),new Vector2(-.5f,-.36f)};
            var vertices=new List<Vector3>();var triangles=new List<int>();var uv=new List<Vector2>();
            for(var row=0;row<4;row++)for(var i=0;i<8;i++)
            {
                var scale=row==0 || row==3?.78f:1f;
                vertices.Add(new Vector3(points[i].x*scale,new[]{-.5f,-.39f,.39f,.5f}[row],points[i].y*scale));uv.Add(points[i]);
            }
            for(var row=0;row<3;row++)for(var i=0;i<8;i++)
            {
                var a=row*8+i;var b=row*8+(i+1)%8;
                triangles.AddRange(new[]{a,a+8,b+8,a,b+8,b});
            }
            for(var i=1;i<7;i++)triangles.AddRange(new[]{0,i,i+1,24,24+i+1,24+i});
            // Sharp broad faces, softened by actual bevel faces, rather than a smoothed cube.
            var flat=new List<Vector3>();var flatUv=new List<Vector2>();var indices=new List<int>();
            foreach(var i in triangles){indices.Add(flat.Count);flat.Add(vertices[i]);flatUv.Add(uv[i]);}
            return StoreProp("Forged bevelled block",flat,indices,flatUv);
        }

        private Mesh StoreProp(string name,List<Vector3> vertices,List<int> triangles,List<Vector2> uv)
        {
            var mesh=new Mesh{name=name,vertices=vertices.ToArray(),triangles=triangles.ToArray(),uv=uv.ToArray()};
            mesh.RecalculateNormals();mesh.RecalculateBounds();_meshes.Add(mesh);return mesh;
        }

        private void TeaService(Vector3 origin)
        {
            InitializeEverydayProps();
            Box(_wood,origin-Vector3.up*.015f,new Vector3(1.06f,.03f,.62f));
            foreach(var z in new[]{-.30f,.30f})Box(_wood,origin+new Vector3(0,.006f,z),new Vector3(1.06f,.035f,.023f));
            foreach(var x in new[]{-.52f,.52f})Box(_wood,origin+new Vector3(x,.006f,0),new Vector3(.023f,.035f,.58f));
            for(var i=-5;i<=5;i++)Box(_wood,origin+new Vector3(i*.082f,.003f,0),new Vector3(.065f,.009f,.55f));
            var pot=origin+new Vector3(0,.01f,-.035f);var turn=Quaternion.Euler(0,35,0);
            Part(_potBody,_clay,pot,Vector3.one,turn);
            Part(_potLid,_clay,pot+Vector3.up*.216f,Vector3.one,turn);
            Part(_potSpout,_clay,pot,Vector3.one,turn);
            Part(_potHandle,_clay,pot,Vector3.one,turn);
            foreach(var x in new[]{-.34f,.34f})foreach(var z in new[]{-.15f,.17f})
            {
                var cup=origin+new Vector3(x,.01f,z);
                Part(_teaSaucer,_ceramic,cup,Vector3.one,Quaternion.identity);
                Part(_teaCup,_ceramic,cup+Vector3.up*.007f,Vector3.one,Quaternion.identity);
            }
        }

        private void WorkshopTools(Vector3 origin)
        {
            InitializeEverydayProps();
            // Rack tools: two different hammers, a broad chisel, and articulated tongs.
            for(var i=0;i<4;i++)
            {
                var p=origin+new Vector3(-.90f+i*.59f,1.65f,1.03f);
                Box(_gold,p+Vector3.up*.43f,new Vector3(.04f,.075f,.16f));
                if(i<3)
                {
                    Part(_toolHandle,_wood,p,new Vector3(.066f,.48f,.066f),Quaternion.identity);
                    Part(_forgedBlock,_iron,p+Vector3.up*.19f,i==2?new Vector3(.12f,.20f,.028f):new Vector3(i==0?.30f:.20f,.12f,.115f),Quaternion.identity);
                    Part(_forgedBlock,_gold,p+Vector3.up*.10f,new Vector3(.071f,.042f,.071f),Quaternion.identity);
                }
                else
                {
                    foreach(var side in new[]{-1f,1f})
                    {
                        var path=new[]{new Vector3(side*.072f,-.26f,0),new Vector3(side*.055f,-.07f,0),Vector3.zero,new Vector3(-side*.07f,.16f,0),new Vector3(-side*.033f,.24f,0)};
                        var tong=SweptProp("Forged tong arm",path,new[]{.014f,.014f,.018f,.020f,.017f},8,false);
                        Part(tong,_iron,p,Vector3.one,Quaternion.identity);
                    }
                    Part(_forgedBlock,_gold,p,new Vector3(.055f,.055f,.044f),Quaternion.identity);
                }
            }
            // Bench vice: two separate jaws, sliding screw and cross handle.
            var vice=origin+new Vector3(.73f,1.035f,0);
            Part(_forgedBlock,_iron,vice,new Vector3(.49f,.065f,.38f),Quaternion.identity);
            Part(_forgedBlock,_iron,vice+new Vector3(0,.09f,.095f),new Vector3(.37f,.16f,.10f),Quaternion.identity);
            Part(_forgedBlock,_iron,vice+new Vector3(0,.09f,-.095f),new Vector3(.37f,.16f,.10f),Quaternion.identity);
            foreach(var z in new[]{-.095f,.095f})
                Part(_forgedBlock,_stoneLight,vice+new Vector3(0,.18f,z),new Vector3(.39f,.035f,.105f),Quaternion.identity);
            Part(_toolHandle,_iron,vice+new Vector3(0,.065f,-.20f),new Vector3(.048f,.40f,.048f),Quaternion.Euler(90,0,0));
            Part(_toolHandle,_iron,vice+new Vector3(0,.065f,-.38f),new Vector3(.031f,.26f,.031f),Quaternion.Euler(0,0,30));
            // Rimmed fittings tray and uneven mineral blanks, kept distinct from gameplay loot.
            var tray=origin+new Vector3(-.65f,1.035f,0);
            Box(_wood,tray,new Vector3(.65f,.04f,.47f));
            foreach(var z in new[]{-.23f,.23f})Box(_wood,tray+new Vector3(0,.035f,z),new Vector3(.65f,.06f,.028f));
            foreach(var x in new[]{-.31f,.31f})Box(_wood,tray+new Vector3(x,.035f,0),new Vector3(.025f,.06f,.45f));
            for(var i=0;i<3;i++)Part(_fittingCrystal,_teal,tray+new Vector3(-.20f+i*.2f,.02f,0),new Vector3(.07f,.15f+i*.02f,.065f),Quaternion.Euler(0,i*45,10-i*8));
        }
    }
}
