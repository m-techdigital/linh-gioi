Shader "LGO/TrainingStoneSurface"
{
    Properties
    {
        [MainColor] _BaseColor("Tint", Color) = (0.1,0.4,0.8,1)
        [HDR] _EmissionColor("Resonance", Color) = (0.025,0.15,0.27,1)
        _SurfaceKind("Stone 0, bronze 1, crystal 2", Float) = 2
        _Metallic("Metallic", Range(0,1)) = 0
        _Smoothness("Smoothness", Range(0,1)) = 0.7
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor;
            half4 _EmissionColor;
            float _SurfaceKind;
            half _Metallic;
            half _Smoothness;
        CBUFFER_END
        struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS : TEXCOORD0;
            float3 positionOS : TEXCOORD1;
            half3 normalWS : TEXCOORD2;
            half fog : TEXCOORD3;
        };
        Varyings Vert(Attributes v)
        {
            Varyings o;
            VertexPositionInputs p = GetVertexPositionInputs(v.positionOS.xyz);
            o.positionCS = p.positionCS; o.positionWS = p.positionWS;
            o.positionOS = v.positionOS.xyz;
            o.normalWS = TransformObjectToWorldNormal(v.normalOS);
            o.fog = ComputeFogFactor(p.positionCS.z);
            return o;
        }
        float2 CellHash(float2 p)
        {
            float3 q = frac(float3(p.xyx) * float3(.1031,.1030,.0973));
            q += dot(q,q.yzx + 33.33);
            return frac((q.xx+q.yz)*q.zy);
        }
        // Nine neighboring cells, one projection per pixel. No texture fetches or ray marching.
        float2 MineralCells(float2 p)
        {
            float2 cell = floor(p), f = frac(p);
            float closest = 8, second = 8;
            [unroll] for(int y=-1;y<=1;y++)
                [unroll] for(int x=-1;x<=1;x++)
                {
                    float2 offset = float2(x,y);
                    float2 d = offset + .15 + .7 * CellHash(cell+offset) - f;
                    float distance = dot(d,d);
                    second = min(second,max(closest,distance));
                    closest = min(closest,distance);
                }
            return float2(sqrt(closest), sqrt(second)-sqrt(closest));
        }
        half4 Frag(Varyings i) : SV_Target
        {
            InputData input = (InputData)0;
            input.positionWS = i.positionWS;
            input.normalWS = NormalizeNormalPerPixel(i.normalWS);
            input.viewDirectionWS = GetWorldSpaceNormalizeViewDir(i.positionWS);
            input.shadowCoord = TransformWorldToShadowCoord(i.positionWS);
            input.bakedGI = SampleSH(input.normalWS);
            input.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(i.positionCS);
            input.shadowMask = half4(1,1,1,1);
            SurfaceData surface = (SurfaceData)0;
            surface.alpha = 1; surface.occlusion = 1;
            surface.normalTS = half3(0,0,1);
            surface.metallic = _Metallic; surface.smoothness = _Smoothness;
            float3 p = i.positionOS;
            float2 plane = float2(p.x + p.z * .73, p.y + p.z * .21);
            float2 mineral = MineralCells(plane * (_SurfaceKind > 1.5 ? 2.1 : 6.0));
            float edgeWidth = max(fwidth(mineral.y), .008);
            half vein = 1-smoothstep(.012,.025+edgeWidth,mineral.y);
            half broad = .5 + .5 * sin(p.y * 7 + sin(p.x * 5 + p.z * 4) * 2);
            half fresnel = pow(1-saturate(dot(input.normalWS,input.viewDirectionWS)),3);
            if (_SurfaceKind > 1.5)
            {
                // Opaque stylized crystal: view-dependent interior bands, luminous seams,
                // and a clear edge. This is an artistic depth cue, not physical refraction.
                float3 viewOS = TransformWorldToObjectDir(input.viewDirectionWS);
                half interior = .5 + .5 * sin((p.y + viewOS.y * .04) * 5
                    + sin((p.x + p.z + viewOS.x * .035) * 4) * 2.4);
                surface.albedo = lerp(half3(.008,.038,.15),_BaseColor.rgb, .2 + .55*interior);
                surface.albedo = lerp(surface.albedo,half3(.10,.63,.82),vein * .55);
                surface.emission = _EmissionColor.rgb * (.24 + vein * 2.7 + fresnel * .85)
                    + half3(.015,.07,.13) * interior;
                surface.clearCoatMask = .8; surface.clearCoatSmoothness = .9;
            }
            else if (_SurfaceKind > .5)
            {
                half patina = smoothstep(.40,.66,mineral.x) * .25;
                surface.albedo = lerp(_BaseColor.rgb * (.85 + broad * .15),half3(.045,.16,.14),patina);
                surface.smoothness = lerp(_Smoothness,.16,patina);
                // Muted edge reflection keeps the carved bronze readable under the city eaves.
                surface.emission = _BaseColor.rgb * fresnel * .09;
            }
            else
            {
                surface.albedo = _BaseColor.rgb * (.55 + .55 * mineral.x + broad * .12);
                surface.smoothness = .16;
            }
            half4 color = UniversalFragmentPBR(input,surface);
            color.rgb = MixFog(color.rgb,i.fog);
            return color;
        }
        ENDHLSL
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForwardOnly" }
            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }
            ZWrite On ZTest LEqual ColorMask 0
            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex ShadowVert
            #pragma fragment ShadowFrag
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
            float3 _LightDirection;
            float3 _LightPosition;
            float4 ShadowVert(Attributes v) : SV_POSITION
            {
                float3 position = TransformObjectToWorld(v.positionOS.xyz);
                float3 normal = TransformObjectToWorldNormal(v.normalOS);
                #if defined(_CASTING_PUNCTUAL_LIGHT_SHADOW)
                    float3 lightDirection = normalize(_LightPosition-position);
                #else
                    float3 lightDirection = _LightDirection;
                #endif
                float4 clip = TransformWorldToHClip(ApplyShadowBias(position,normal,lightDirection));
                #if UNITY_REVERSED_Z
                    clip.z = min(clip.z,UNITY_NEAR_CLIP_VALUE);
                #else
                    clip.z = max(clip.z,UNITY_NEAR_CLIP_VALUE);
                #endif
                return clip;
            }
            half4 ShadowFrag() : SV_Target { return 0; }
            ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }
            ZWrite On ColorMask R
            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex Vert
            #pragma fragment DepthFrag
            half4 DepthFrag(Varyings i) : SV_Target { return i.positionCS.z; }
            ENDHLSL
        }
    }
    FallBack Off
}
