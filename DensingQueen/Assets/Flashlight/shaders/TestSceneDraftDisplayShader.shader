Shader "Unlit/TestSceneDraftDisplayShader"
{
    Properties
    {
        _MainTex ("Displayed Texture", 2D) = "white" {}
		_AlphaTex ("Alpha Map", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+100"}
        LOD 100

        Pass
        {
            ZWrite Off

			 ColorMask RGB
             Blend OneMinusSrcAlpha SrcAlpha

			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
			sampler2D _AlphaTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				//o.vertex = v.vertex;
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                
                half4 col = tex2D(_MainTex, i.uv);
				half4 aTex = tex2D(_AlphaTex, i.uv);
				col.a = 1 - col.a * aTex.g;
                return col;
            }
            ENDCG
        }
    }
}
