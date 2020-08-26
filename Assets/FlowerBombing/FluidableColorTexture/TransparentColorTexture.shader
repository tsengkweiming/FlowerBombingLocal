Shader "Fluidable/Transparent/ColorTexture" {
	Properties {
		_MainTex ("Texture", 2D) = "black" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Fluidity ("Fluidity", Range(0,1)) = 1
        _LocalShift ("Local HSV Shift", Vector) = (0,0,0,0)
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" "IgnoreProjector"="True" }
        ZTest LEqual ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

		Pass {

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
            #include "Assets/Packages/SimpleFluid/Shaders/SimpleFluid_Fluidable.cginc"
            #include "Assets/Packages/Gist/CGIncludes/ColorSpace.cginc"

            #pragma multi_compile FLUIDABLE_OUTPUT_COLOR FLUIDABLE_OUTPUT_SOURCE

			struct appdata 	{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f 	{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
            float4 _Color;
            float4 _LocalShift;
			
			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target	{
				float4 col = tex2D(_MainTex, i.uv) * _Color;
                col.rgb = HSVShift(col, _LocalShift);
                col = fluidOutMultiplier(col);
                return col;
			}
			ENDCG
		}
	}
}
