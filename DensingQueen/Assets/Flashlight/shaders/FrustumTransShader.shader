﻿Shader "Custom/FrustumTrans"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Trans("Out Of Frustum Transparency", Float) = 0.15
        _Rad("View Radius" , Float) = 5.0
    }
        SubShader
        {
            Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "ForceNoShadowCasting" = "True"}

            LOD 200
            Cull Off
            ZWrite On

            //ZWrite Off
            Stencil{
                Ref 2
                Comp always
                Pass replace
            }

            CGPROGRAM
           
            #pragma surface surf Standard fullforwardshadows alpha:fade
            #pragma target 3.0

            sampler2D _MainTex;

            struct Input {
                float2 uv_MainTex;
                float3 worldPos;
            };

            half _Glossiness;
            half _Metallic;
            half _Trans;
            half _Rad;
            fixed4 _Color;

            fixed4 plane1;
            fixed4 plane2;
            fixed4 plane3;
            fixed4 plane4;
            fixed4 plane5;
            fixed4 plane6;
            fixed3 rayOri;
            fixed3 rayDir;

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                o.Alpha = 1;

                fixed4 pos = fixed4(IN.worldPos.xyz,1);
                if ((dot(plane1, pos) > 0 && dot(plane2, pos) > 0 && dot(plane3, pos) > 0 && dot(plane4, pos) > 0 && dot(plane5, pos) > 0 && dot(plane6, pos) > 0) && length(cross(rayDir,pos.xyz-rayOri))< _Rad) {
                    
                }
                else {
                    o.Alpha = _Trans;
                }

                
            }
            ENDCG
        }
            FallBack "Standard"
}