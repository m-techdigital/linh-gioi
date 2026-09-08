Shader "LGO/CityDaySky"
{
    SubShader
    {
        Tags {"Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" "RenderPipeline"="UniversalPipeline"}
        Cull Off ZWrite Off
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Attributes {float4 positionOS:POSITION;};
            struct Varyings {float4 positionCS:SV_POSITION;float3 direction:TEXCOORD0;};
            Varyings Vert(Attributes v)
            {
                Varyings o;o.positionCS=TransformObjectToHClip(v.positionOS.xyz);o.direction=v.positionOS.xyz;return o;
            }
            float Hash(float2 p){return frac(sin(dot(p,float2(127.1,311.7)))*43758.5453);}
            float Noise(float2 p)
            {
                float2 i=floor(p),f=frac(p);f=f*f*(3-2*f);
                return lerp(lerp(Hash(i),Hash(i+float2(1,0)),f.x),lerp(Hash(i+float2(0,1)),Hash(i+1),f.x),f.y);
            }
            half4 Frag(Varyings i):SV_Target
            {
                float3 d=normalize(i.direction);
                float elevation=saturate(d.y);
                float3 sky=lerp(float3(.72,.83,.87),float3(.17,.43,.69),pow(elevation,.42));
                float2 p=d.xz/max(.015,d.y)*1.5+float2(7.3,1.9);
                float n=Noise(p)*.55+Noise(p*2.03)*.28+Noise(p*4.11)*.12+Noise(p*8.1)*.05;
                float cloud=smoothstep(.49,.69,n)*smoothstep(.10,.28,d.y);
                float3 clouds=lerp(float3(.68,.77,.84),float3(.99,.96,.88),smoothstep(.47,.72,n));
                sky=lerp(sky,clouds,cloud*.94);
                float sun=pow(saturate(dot(d,normalize(float3(-.45,.62,-.64)))),160);
                sky+=float3(1,.82,.51)*sun*.45;
                return half4(sky,1);
            }
            ENDHLSL
        }
    }
}
