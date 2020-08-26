Shader "Fluidable/Opaque/ColorTexture" {
	Properties {
		_MainTex ("Texture", 2D) = "black" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Power ("Power", Float) = 1
        _Fluidity ("Fluidity", Range(0,1)) = 1
		_Swizzle ("Color Matrix", Vector) = (1, 2, 4, 8)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }

		Pass {
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
				int4x4 swizzle : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
            float4 _Color;
            float _Power;
			int4 _Swizzle;
			
			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.swizzle = int4x4(
					(int(_Swizzle.x) & int4(1, 2, 4, 8)) >> int4(0, 1, 2, 3),
					(int(_Swizzle.y) & int4(1, 2, 4, 8)) >> int4(0, 1, 2, 3),
					(int(_Swizzle.z) & int4(1, 2, 4, 8)) >> int4(0, 1, 2, 3),
					(int(_Swizzle.w) & int4(1, 2, 4, 8)) >> int4(0, 1, 2, 3)
					);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target	{
				float4 cmain = tex2D(_MainTex, i.uv);
                float4 c = cmain * _Color * _Power;
				c = mul(i.swizzle, c);
				c = saturate(c);
                return fluidOutMultiplier(c);
			}
			ENDCG
		}
	}
}
