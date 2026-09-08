using UnityEngine;
using LinhGioi.Art;

namespace LinhGioi.World
{
    public sealed partial class OnboardingBlockoutWorld
    {
        private void CreateTrainingSquare(Material paving, Material walls, Material roof, Material timber)
        {
            // Local candidate for DESIGN.md/layout-v2.json. The original arrival street remains available.
            Box("Training square ground",new Vector3(0,-.1f,0),new Vector3(80,.2f,100),paving).GetComponent<Renderer>().enabled=false;
            _pavingTexture=Resources.Load<Texture2D>("LGOCitySurfaces/AgedPaving");
            if(_pavingTexture==null) throw new System.InvalidOperationException("Square needs shared paving.");
            paving.color=Color.white; paving.mainTexture=_pavingTexture;
            var corners=new[]{new Vector3(-40,.002f,-50),new Vector3(40,.002f,-50),new Vector3(-40,.002f,50),new Vector3(40,.002f,50)};
            var uv=new Vector2[4];for(var i=0;i<4;i++)uv[i]=new Vector2(corners[i].x/3,corners[i].z/3);
            _pavingMesh=new Mesh{name="Training square continuous paving",vertices=corners,uv=uv,triangles=new[]{0,2,1,1,2,3}};
            _pavingMesh.RecalculateNormals();_pavingMesh.RecalculateBounds();
            var floor=new GameObject("Training square paving");floor.transform.SetParent(transform,false);
            floor.AddComponent<MeshFilter>().sharedMesh=_pavingMesh;floor.AddComponent<MeshRenderer>().sharedMaterial=paving;
            for(var side=-1;side<=1;side+=2)
            {
                foreach(var z in new[]{-18f,-12f,12f,18f})
                    CreateStreetHouse(new Vector3(side*18,0,z),3.5f+(Mathf.Abs(z)>15?1f:0f),walls,roof,timber);
                foreach(var x in new[]{-12f,-6f,6f,12f})
                {
                    var center=new Vector3(x,0,side*24f);
                    var rotation=Quaternion.Euler(0,side*(x<0?90:-90),0);
                    _city.BeginModule(center,rotation);
                    _city.House(center,4.2f+Mathf.Abs(x)*.06f);
                    _city.Roof(center+Vector3.up*(4.3f+Mathf.Abs(x)*.06f),new Vector3(1.8f,1f,2.7f));
                    _city.EndModule();
                    Box("Square perimeter house",center+Vector3.up*2,new Vector3(4.8f,4f,3f),walls);
                }
                foreach(var z in new[]{-15f,15f})
                {
                    var tree=new Vector3(side*13.5f,.025f,z);
                    _city.Tree(tree,5.2f,side<0,name:"Square garden tree");
                    Box("Square tree trunk collision",tree+Vector3.up*1.7f,new Vector3(.55f,3.4f,.55f),timber).GetComponent<Renderer>().enabled=false;
                    _city.Masonry(tree+Vector3.down*.04f,new Vector3(3.8f,.12f,3.8f));
                }
                // Low outer boundary protects the local candidate edge; exits have visible continuation space.
                Box("Square boundary",new Vector3(side*40,.5f,0),new Vector3(.2f,1,100),walls);
                Box("Square end boundary",new Vector3(0,.5f,side*50),new Vector3(80,1,.2f),walls);
            }
            // Connected outskirts around the square, with ground beneath every skyline module.
            var earth=Material(new Color(.29f,.33f,.27f));
            Box("City surrounding ground",new Vector3(0,-.22f,15),new Vector3(180,.2f,180),earth,false);
            for(var side=-1;side<=1;side+=2)
            {
                foreach(var z in new[]{-20f,-8f,8f,20f,32f})
                    CreateStreetHouse(new Vector3(side*29f,0,z),4f+(Mathf.Abs(z)%3f),walls,roof,timber,scenery:true);
                foreach(var x in new[]{-18f,-9f,9f,18f})
                    CreateStreetHouse(new Vector3(x,0,side*37f),4.5f,walls,roof,timber,scenery:true);
            }
            _city.SquareBackdrop();
            // Tea pavilion: open-sided at the eastern perimeter, well outside the 7–11m circulation ring.
            var tea=new Vector3(15,0,4);
            for(var x=-1;x<=1;x+=2)for(var z=-1;z<=1;z+=2)
                Box("Tea pavilion pillar",tea+new Vector3(x*1.8f,1.7f,z*2.4f),new Vector3(.22f,3.4f,.22f),timber);
            _city.Roof(tea+Vector3.up*3.5f,new Vector3(2.3f,1.1f,2.9f));
            for(var z=-1;z<=1;z+=2)
            {
                Box("Tea pavilion beam",tea+new Vector3(0,3.3f,z*2.4f),new Vector3(4,.22f,.22f),timber,false);
                Box("Tea bench",tea+new Vector3(0,.45f,z*1.8f),new Vector3(2.7f,.16f,.45f),timber);
            }
            Box("Tea table",tea+Vector3.up*.7f,new Vector3(1.8f,.15f,1.1f),timber);
            Box("Tea table support",tea+Vector3.up*.35f,new Vector3(.5f,.7f,.5f),timber);
            _city.Masonry(tea+Vector3.down*.06f,new Vector3(4.7f,.1f,5.8f));
            _city.Finish();
            // Continuous ring inlays, drawn flush with the ground and without navigation obstacles.
            var bronze=Material(new Color(.48f,.37f,.21f));
            for(var ring=0;ring<2;ring++)for(var segment=0;segment<64;segment++)
            {
                var angle=segment*Mathf.PI*2/64;
                var radius=ring==0?7f:11f;
                var strip=Box("Square circulation inlay",new Vector3(Mathf.Cos(angle)*radius,.009f,Mathf.Sin(angle)*radius),
                    new Vector3(.045f,.012f,radius*Mathf.PI*2/64+.01f),bronze,false);
                strip.transform.rotation=Quaternion.Euler(0,-angle*Mathf.Rad2Deg,0);
            }
            // Lamps and benches live on the outside of the ring, leaving the central routes open.
            var paper=Material(new Color(.95f,.74f,.35f));
            foreach(var point in new[]{new Vector3(-12,0,-8),new Vector3(12,0,-8),new Vector3(-12,0,8),new Vector3(12,0,8)})
            {
                Box("Square lantern base",point+Vector3.up*.2f,new Vector3(.7f,.4f,.7f),walls);
                Box("Square lantern post",point+Vector3.up*1f,new Vector3(.18f,1.5f,.18f),timber);
                Box("Square lantern shade",point+Vector3.up*1.9f,new Vector3(.55f,.75f,.55f),paper,false);
                Box("Square lantern cap",point+Vector3.up*2.3f,new Vector3(.7f,.13f,.7f),bronze,false);
                for(var x=-1;x<=1;x+=2)for(var z=-1;z<=1;z+=2)
                    Box("Square lantern frame",point+new Vector3(x*.29f,1.9f,z*.29f),new Vector3(.045f,.78f,.045f),timber,false);
                Box("Square lantern lower rail",point+Vector3.up*1.51f,new Vector3(.67f,.09f,.67f),timber,false);
            }
            Debug.Log("LGO_TRAINING_SQUARE_CREATED footprint=40x48 landmark=6.5m circulation=7to11m legacy_street=retained");
        }

        private Renderer CreateSquareLandmark()
        {
            var renderer=TrainingStoneVisuals.Create(transform,Material,true);
            renderer.gameObject.name="Blockout Stone";
            _stoneMesh=renderer.GetComponent<MeshFilter>().sharedMesh;
            renderer.gameObject.AddComponent<MeshCollider>().sharedMesh=_stoneMesh;
            _stoneCrystalMaterials=new[]{renderer.sharedMaterials[2],renderer.sharedMaterials[3]};
            _stoneSealMaterial=Material(new Color(.18f,.78f,1f));
            _stoneSealMesh=CreateStoneSealMesh();
            var frame=Material(new Color(.48f,.32f,.13f));
            var panel=Material(new Color(.045f,.13f,.21f));
            Box("Stone touch plinth",new Vector3(0,.7f,-2.18f),new Vector3(.85f,1.4f,.36f),frame);
            Box("Stone touch plaque",new Vector3(0,1.02f,-2.375f),new Vector3(.65f,.6f,.035f),panel,false);
            var seal=Box("Blockout stone seal front",new Vector3(0,1.02f,-2.399f),Vector3.one,_stoneSealMaterial,false);
            seal.GetComponent<MeshFilter>().sharedMesh=_stoneSealMesh;
            seal.transform.rotation=Quaternion.LookRotation(Vector3.back);
            seal.transform.localScale=Vector3.one*2.2f;
            var coreSeal=Box("Landmark crystal resonance",new Vector3(0,3.5f,-.835f),Vector3.one,_stoneSealMaterial,false);
            coreSeal.GetComponent<MeshFilter>().sharedMesh=_stoneSealMesh;
            coreSeal.transform.rotation=Quaternion.LookRotation(Vector3.back);coreSeal.transform.localScale=Vector3.one*3f;
            // Four shallow steps invite approach; touch remains available from ground level.
            for(var step=0;step<4;step++)
                Box("Landmark approach step",new Vector3(.95f,(step+1)*.1f,-3.1f+step*.27f),
                    new Vector3(.9f,(step+1)*.2f,.3f),panel);
            return renderer;
        }
    }
}
