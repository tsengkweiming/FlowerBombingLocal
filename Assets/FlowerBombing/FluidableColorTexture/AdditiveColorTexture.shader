Shader "Fluidable/Transparent/AdditiveColorTexture" {
	Properties {
		_MainTex ("Texture", 2D) = "black" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Power ("Power", Float) = 1
        _Fluidity ("Fluidity", Range(0,1)) = 1
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }

		Pass {
            Blend One OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
            #include "Assets/Packages/SimpleFluid/Shaders/SimpleFluid_Fluidable.cginc"

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
            float _Power;
            float _Fluidity;
			
			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target	{
				fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb *= _Power;
                return col * fluidOutMultiplier(_Color, float4(_Color.rgb, _Fluidity));
			}
			ENDCG
		}
	}
}
