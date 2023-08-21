Shader "Custom/TestSceneDraft"
{
    Properties
    {
        //_Color ("Color", Color) = (1,1,1,1)
        
		_ProjTex ("Displayed Texture", 2D) = "white" {}
		_AlphaTex ("Alpha Map", 2D) = "white" {}

        //_Glossiness ("Smoothness", Range(0,1)) = 0.5
        //_Metallic ("Metallic", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+100" }
        Pass
         {
             ZWrite Off
			 Offset -1, -1

             Fog { Mode Off }
    
             ColorMask RGB
             Blend OneMinusSrcAlpha SrcAlpha
 
 
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #pragma fragmentoption ARB_fog_exp2
             #pragma fragmentoption ARB_precision_hint_fastest
             #include "UnityCG.cginc"
            
             struct v2f
             {
                 float4 pos      : SV_POSITION;
                 float4 uv       : TEXCOORD0;
             };
            
                 sampler2D _ProjTex;
                 sampler2D _AlphaTex;
                 float4x4 unity_Projector;
                 float4 _Color;
            
             v2f vert(appdata_tan v)
             {
                 v2f o;
                 o.pos = UnityObjectToClipPos (v.vertex);
                 o.uv = mul (unity_Projector, v.vertex);
                 return o;
             }
            
             half4 frag (v2f i) : COLOR
             {
                 half4 tex = tex2Dproj(_ProjTex, i.uv);
                 half4 aTex = tex2Dproj(_AlphaTex, i.uv);
                 tex.a = 1-tex.a * aTex.g;
                 if (i.uv.w < 0)
                 {
                     tex = float4(0,0,0,1);
                 }
                 return tex;
             }
             ENDCG
        
         }
	}
		/*
		//LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
	*/
}
