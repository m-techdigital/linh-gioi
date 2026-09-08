using UnityEngine;
using LinhGioi.Art;

namespace LinhGioi.World
{
    public sealed partial class OnboardingBlockoutWorld
    {
        private Mesh _landmarkSealMesh;

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
            // Houses face the square along broken arcs, with clear cardinal routes.
            foreach(var angle in new[]{18f,36f,54f,72f,108f,126f,144f,162f,198f,216f,234f,306f,324f,342f})
            {
                var radians=angle*Mathf.Deg2Rad;
                var centre=new Vector3(Mathf.Cos(radians),0,Mathf.Sin(radians))*26f;
                var inward=(-centre).normalized;
                var initial=centre.x<0?Vector3.right:Vector3.left;
                var rotation=Quaternion.FromToRotation(initial,inward);
                var style=((int)(angle/18f))%3;
                var height=style==2?3.6f:3.2f;
                _city.BeginModule(centre,rotation);
                _city.SquareResidence(centre,height,style);
                _city.EndModule();
                var collision=Box("Square residence collision",centre+Vector3.up*(height*.5f),new Vector3(3f,height,4.8f),walls);
                collision.transform.rotation=rotation;collision.GetComponent<Renderer>().enabled=false;
            }
            for(var side=-1;side<=1;side+=2)
            {
                foreach(var z in new[]{-15f,15f})
                {
                    var tree=new Vector3(side*16.5f,.025f,z);
                    _city.Tree(tree,5.7f,side<0,name:"Square garden tree");
                    Box("Square tree trunk collision",tree+Vector3.up*1.7f,new Vector3(.55f,3.4f,.55f),timber).GetComponent<Renderer>().enabled=false;
                    _city.GardenBed(tree,new Vector2(4.7f,3.8f),side<0);
                }
                Box("Square boundary",new Vector3(side*40,.5f,0),new Vector3(.2f,1,100),walls);
                Box("Square end boundary",new Vector3(0,.5f,side*50),new Vector3(80,1,.2f),walls);
            }
            var earth=Material(new Color(.29f,.33f,.27f));
            Box("City surrounding ground",new Vector3(0,-.22f,15),new Vector3(180,.2f,180),earth,false);
            foreach(var angle in new[]{20f,50f,75f,110f,140f,165f,200f,225f,315f,340f})
            {
                var radians=angle*Mathf.Deg2Rad;
                var centre=new Vector3(Mathf.Cos(radians),0,Mathf.Sin(radians))*39f;
                CreateStreetHouse(centre,4f+(angle%3f),walls,roof,timber,scenery:true);
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
            _city.TeaPavilionDetails(tea);
            _city.GardenBed(new Vector3(18.3f,.025f,4),new Vector2(1.5f,6.8f),true);
            _city.GardenBed(new Vector3(15,.025f,8),new Vector2(6f,1.2f),false);
            var craft=new Vector3(12,0,16);
            _city.CraftStall(craft);
            Box("Craft workbench collision",craft+new Vector3(0,.48f,0),new Vector3(3.1f,.96f,.95f),timber).GetComponent<Renderer>().enabled=false;
            foreach(var x in new[]{-2f,2f})
                Box("Craft pillar collision",craft+new Vector3(x,1.5f,.8f),new Vector3(.2f,3f,.2f),timber).GetComponent<Renderer>().enabled=false;
            foreach(var point in new[]{new Vector3(-12,0,-8),new Vector3(12,0,-8),new Vector3(-12,0,8),new Vector3(12,0,8)})
            {
                _city.SquareLantern(point);
                Box("Square lantern collision",point+Vector3.up*1.05f,new Vector3(.36f,2.1f,.36f),walls).GetComponent<Renderer>().enabled=false;
            }
            SquareSign("Gian nghề",craft+new Vector3(0,2.65f,-.48f),Quaternion.identity,1.65f);
            SquareSign("Trà đình",new Vector3(12.94f,2.83f,4),Quaternion.Euler(0,90,0),1.14f);
            _city.Finish();
            CreateSquareSky();
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
            Debug.Log("LGO_TRAINING_SQUARE_CREATED footprint=40x48 landmark=6.5m circulation=7to11m legacy_street=retained");
        }

        private void SquareSign(string text,Vector3 point,Quaternion rotation,float width)
        {
            var label=WorldLabelPresenter.Create("Square sign "+text,text,point,RuntimeArtCatalog.Gold);
            label.transform.SetParent(transform,true);label.transform.rotation=rotation;
            WorldLabelPresenter.ApplyStyle(label,64,.075f);
            var bounds=label.GetComponent<Renderer>().bounds;
            var span=rotation==Quaternion.identity?bounds.size.x:bounds.size.z;
            if(span>.001f)label.transform.localScale=Vector3.one*(width/span);
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
            var frame=renderer.sharedMaterials[1];
            var stone=renderer.sharedMaterials[0];
            var panel=Material(new Color(.045f,.13f,.21f));
            Box("Stone touch plinth",new Vector3(0,.7f,-2.18f),new Vector3(.85f,1.4f,.36f),stone);
            Box("Stone touch plaque",new Vector3(0,1.02f,-2.375f),new Vector3(.65f,.6f,.035f),panel,false);
            var seal=Box("Blockout stone seal front",new Vector3(0,1.02f,-2.399f),Vector3.one,_stoneSealMaterial,false);
            seal.GetComponent<MeshFilter>().sharedMesh=_stoneSealMesh;
            seal.transform.rotation=Quaternion.LookRotation(Vector3.back);
            seal.transform.localScale=Vector3.one*2.2f;
            var coreSeal=Box("Landmark crystal resonance",new Vector3(0,3.5f,-.835f),Vector3.one,_stoneSealMaterial,false);
            _landmarkSealMesh=Instantiate(_stoneSealMesh);
            _landmarkSealMesh.name="Landmark surface-conforming resonance seal";
            coreSeal.GetComponent<MeshFilter>().sharedMesh=_landmarkSealMesh;
            coreSeal.transform.rotation=Quaternion.LookRotation(Vector3.back);coreSeal.transform.localScale=Vector3.one*3f;
            // Project once onto the authored facets; a flat shared seal would sink into the new mineral.
            Physics.SyncTransforms();
            var sealVertices=_landmarkSealMesh.vertices;
            var stoneCollider=renderer.GetComponent<MeshCollider>();
            for(var i=0;i<sealVertices.Length;i++)
            {
                var point=coreSeal.transform.TransformPoint(sealVertices[i]);
                var origin=new Vector3(point.x,point.y,-4f);
                if(!stoneCollider.Raycast(new Ray(origin,Vector3.forward),out var hit,8f))
                    throw new System.InvalidOperationException("Landmark seal missed the crystal surface.");
                sealVertices[i]=coreSeal.transform.InverseTransformPoint(hit.point+Vector3.back*.012f);
            }
            _landmarkSealMesh.vertices=sealVertices;
            _landmarkSealMesh.RecalculateNormals();_landmarkSealMesh.RecalculateBounds();
            // Four shallow steps invite approach; touch remains available from ground level.
            for(var step=0;step<4;step++)
                Box("Landmark approach step",new Vector3(.95f,(step+1)*.1f,-3.1f+step*.27f),
                    new Vector3(1.3f,(step+1)*.2f,.3f),stone);
            // Raised bronze plaque frame and stair nosings, shared with the landmark materials.
            foreach(var x in new[]{-.36f,.36f})
                Box("Touch plaque border",new Vector3(x,1.02f,-2.4f),new Vector3(.055f,.69f,.055f),frame,false);
            foreach(var y in new[]{.69f,1.35f})
                Box("Touch plaque border",new Vector3(0,y,-2.4f),new Vector3(.78f,.055f,.055f),frame,false);
            for(var step=0;step<4;step++)
                Box("Landmark step nosing",new Vector3(.95f,(step+1)*.2f+.012f,-3.24f+step*.27f),new Vector3(1.3f,.025f,.045f),frame,false);
            return renderer;
        }
    }
}
