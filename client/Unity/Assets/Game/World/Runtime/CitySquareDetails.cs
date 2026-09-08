using UnityEngine;
using System.Collections.Generic;

namespace LinhGioi.World
{
    // Reusable courtyard modules; all decorative parts join the existing material batches.
    internal sealed partial class CityArchitectureVisuals
    {
        internal void SquareResidence(Vector3 centre,float height,int style=0)
        {
            Box(_ivory,centre+Vector3.up*height*.5f,new Vector3(3f,height,4.8f));
            Masonry(centre+Vector3.up*.15f,new Vector3(3.08f,.3f,4.88f));
            House(centre,height,style==2);Roof(centre+Vector3.up*(height+.1f),new Vector3(1.8f,1f,2.7f));
            var sign=centre.x<0?1f:-1f;
            Vector3 P(float depth,float y,float along)=>centre+new Vector3(sign*depth,y,along);
            // Deep porch, stone feet, curved brackets and a low lattice balustrade.
            Box(_stoneLight,P(2.1f,.10f,0),new Vector3(1.6f,.2f,5.2f));
            for(var side=-1;side<=1;side+=2)
            {
                Box(_stone,P(2.7f,.24f,side*2.25f),new Vector3(.40f,.48f,.4f));
                Box(_wood,P(2.7f,1.6f,side*2.25f),new Vector3(.17f,2.8f,.17f));
                Beam(_wood,P(2.7f,2.55f,side*2.25f),P(2.2f,3f,side*2.25f),.13f);
                Box(_wood,P(2.7f,.88f,side*1.45f),new Vector3(.10f,.09f,1.4f));
                for(var slat=0;slat<5;slat++)
                    Box(_wood,P(2.7f,.56f,side*(.86f+slat*.28f)),new Vector3(.06f,.6f,.055f));
                var planter=P(2.25f,.35f,side*1.5f);
                Box(_stone,planter,new Vector3(.45f,.5f,.75f));
                GardenPlant(planter+Vector3.up*.26f,0,true,.75f,Quaternion.identity);
            }
            if(style==1)
            {
                // Open shop counter and stacked wares distinguish commerce from quiet homes.
                Box(_wood,P(2.22f,.76f,0),new Vector3(.75f,.13f,2.2f));
                for(var item=-2;item<=2;item++)
                {
                    Box(item%2==0?_teal:_ivory,P(2.2f,.92f,item*.36f),new Vector3(.32f,.20f,.28f));
                    Box(_gold,P(2.4f,.91f,item*.36f),new Vector3(.025f,.13f,.035f));
                }
                Box(_teal,P(2.74f,2.62f,0),new Vector3(.035f,.35f,2.6f));
                for(var i=-4;i<=4;i++)Box(_ivory,P(2.765f,2.60f,i*.28f),new Vector3(.025f,.30f,.06f));
            }
            // Reuse the same curved tile family on the projecting porch canopy.
            Roof(P(2.05f,3.05f,0),new Vector3(.95f,.45f,2.75f));
        }

        internal void SquareLantern(Vector3 point)
        {
            Masonry(point+Vector3.up*.20f,new Vector3(.65f,.4f,.65f));
            Box(_wood,point+Vector3.up*1.05f,new Vector3(.18f,1.6f,.18f));
            Box(_gold,point+Vector3.up*1.54f,new Vector3(.40f,.10f,.40f));
            Box(_ivory,point+Vector3.up*1.98f,new Vector3(.47f,.73f,.47f));
            for(var x=-1;x<=1;x+=2)for(var z=-1;z<=1;z+=2)
                Box(_wood,point+new Vector3(x*.255f,1.98f,z*.255f),new Vector3(.065f,.80f,.065f));
            foreach(var y in new[]{1.6f,2.36f})Box(_wood,point+Vector3.up*y,new Vector3(.64f,.09f,.64f));
            for(var side=-1;side<=1;side+=2)
            {
                Beam(_gold,point+new Vector3(-.20f,1.68f,side*.245f),point+new Vector3(.20f,2.27f,side*.245f),.027f);
                Beam(_gold,point+new Vector3(.20f,1.68f,side*.245f),point+new Vector3(-.20f,2.27f,side*.245f),.027f);
            }
            Roof(point+Vector3.up*2.4f,new Vector3(.48f,.22f,.48f));
        }

        internal void CraftStall(Vector3 centre)
        {
            Masonry(centre+new Vector3(0,.04f,.35f),new Vector3(4.7f,.08f,3.1f));
            foreach(var x in new[]{-2f,2f})
            {
                Masonry(centre+new Vector3(x,.18f,.8f),new Vector3(.42f,.36f,.42f));
                Box(_wood,centre+new Vector3(x,1.6f,.8f),new Vector3(.20f,3.2f,.20f));
                Beam(_wood,centre+new Vector3(x,2.6f,.8f),centre+new Vector3(x*.7f,3.2f,.8f),.13f);
            }
            Roof(centre+new Vector3(0,3.2f,.45f),new Vector3(2.6f,.65f,1.8f));
            Box(_wood,centre+new Vector3(0,2.65f,-.40f),new Vector3(2f,.48f,.12f));
            Box(_wood,centre+new Vector3(0,.93f,0),new Vector3(3.1f,.16f,.95f));
            foreach(var x in new[]{-1.3f,1.3f})Box(_wood,centre+new Vector3(x,.44f,0),new Vector3(.17f,.88f,.7f));
            Box(_wood,centre+new Vector3(0,.27f,0),new Vector3(2.7f,.10f,.75f));
            // Wooden tool rack: physical hooks, handles and bronze heads share the existing palette.
            foreach(var y in new[]{1.3f,2.1f})Box(_wood,centre+new Vector3(0,y,1.15f),new Vector3(3f,.09f,.13f));
            WorkshopTools(centre);
            for(var i=0;i<2;i++)
            {
                var crate=centre+new Vector3(-1.3f+i*.85f,.48f,.1f);
                Box(_wood,crate,new Vector3(.65f,.48f,.5f));
                foreach(var x in new[]{-.23f,.23f})Box(_gold,crate+new Vector3(x,0,-.255f),new Vector3(.035f,.48f,.025f));
            }
        }

        internal void GardenBed(Vector3 centre,Vector2 size,bool blossom)
        {
            Box(_leaf,centre+Vector3.down*.01f,new Vector3(size.x,.10f,size.y));
            for(var side=-1;side<=1;side+=2)
            {
                Masonry(centre+new Vector3(side*size.x*.5f,.1f,0),new Vector3(.16f,.2f,size.y+.16f));
                Masonry(centre+new Vector3(0,.1f,side*size.y*.5f),new Vector3(size.x,.2f,.16f));
            }
            var count=Mathf.CeilToInt(size.x*size.y*2.4f);
            for(var i=0;i<count;i++)
            {
                var x=Mathf.Repeat(i*.618034f,1f)-.5f;var z=Mathf.Repeat(i*.414214f,1f)-.5f;
                var p=centre+new Vector3(x*(size.x-.55f),.10f,z*(size.y-.55f));
                var rotation=Quaternion.Euler(0,i*137.5f,0);
                GardenPlant(p,i,blossom,.84f+(i%3)*.09f,rotation);
                if(i%9==0)Part(_cube,_stone,p+Vector3.down*.06f,new Vector3(.21f,.13f,.27f),rotation);
            }
        }

        internal void TeaPavilionDetails(Vector3 centre)
        {
            for(var x=-1;x<=1;x+=2)for(var z=-1;z<=1;z+=2)
            {
                var p=centre+new Vector3(x*1.8f,0,z*2.4f);
                Masonry(p+Vector3.up*.15f,new Vector3(.43f,.3f,.43f));
                Beam(_wood,p+Vector3.up*2.8f,p+new Vector3(-x*.65f,3.28f,0),.13f);
                Beam(_wood,p+Vector3.up*2.8f,p+new Vector3(0,3.28f,-z*.65f),.13f);
                Box(_gold,p+Vector3.up*2.67f,new Vector3(.24f,.08f,.24f));
            }
            // Open west frontage faces the stone. Low rear screens shelter the seating.
            foreach(var z in new[]{-1f,1f})
            {
                Box(_wood,centre+new Vector3(0,1f,z*2.35f),new Vector3(3.55f,.12f,.12f));
                for(var i=-5;i<=5;i++)
                    Box(_wood,centre+new Vector3(i*.31f,.69f,z*2.35f),new Vector3(.055f,.60f,.055f));
                for(var x=-1;x<=1;x+=2)
                    Box(_wood,centre+new Vector3(x*.98f,.24f,z*1.8f),new Vector3(.16f,.46f,.35f));
            }
            TeaService(centre+Vector3.up*.82f);
            var sign=centre+new Vector3(-1.98f,2.83f,0);
            Box(_wood,sign,new Vector3(.11f,.46f,1.35f));

        }
    }
}
