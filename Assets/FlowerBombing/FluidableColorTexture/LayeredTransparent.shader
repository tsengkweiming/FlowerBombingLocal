Shader "Fluidable/Transparent/LayeredColorTexture" {
	Properties {
		_MainTex ("Texture", 2D) = "black" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Fluidity ("Fluidity", Range(0,1)) = 1

        _BlendTex ("Blend", 2D) = "black" {}

        _Tex1 ("Layer 1", 2D) = "black" {}
        _Tex2 ("Layer 2", 2D) = "black" {}
        _Tex3 ("Layer 3", 2D) = "black" {}
        _Color1 ("Color 1", Color) = (1,1,1,1)
        _Color2 ("Color 2", Color) = (1,1,1,1)
        _Color3 ("Color 3", Color) = (1,1,1,1)
        _Pows ("Layer Powers", Vector) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }

		Pass {
            Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

            #include "Assets/Packages/SimpleFluid/Shaders/SimpleFluid_Fluidable.cginc"
            #pragma multi_compile FLUIDABLE_OUTPUT_COLOR FLUIDABLE_OUTPUT_SOURCE
			
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;
                float2 uvMain : TEXCOORD4;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
            sampler2D _BlendTex;
            sampler2D _Tex1;
            sampler2D _Tex2;
            sampler2D _Tex3;
			
            float4 _Color;
            float4 _Color1;
            float4 _Color2;
            float4 _Color3;
            float4 _Pows;

            float4 _MainTex_ST;
            float4 _BlendTex_ST;
            float4 _Tex1_ST;
            float4 _Tex2_ST;
            float4 _Tex3_ST;

			v2f vert (appdata v) {
                float2 aspect = float2(_ScreenParams.x / _ScreenParams.y, 1.0);

				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _BlendTex);
                o.uvMain = TRANSFORM_TEX(v.uv * aspect, _MainTex);
                o.uv1 = TRANSFORM_TEX(v.uv * aspect, _Tex1);
                o.uv2 = TRANSFORM_TEX(v.uv * aspect, _Tex2);
                o.uv3 = TRANSFORM_TEX(v.uv * aspect, _Tex3);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target {
				float4 c = tex2D(_MainTex, i.uvMain);
                float4 b = tex2D(_BlendTex, i.uv);
                float bsum = saturate(dot(b.rgb, 1));
                float4x4 cmat = float4x4(
                    tex2D(_Tex1, i.uv1) * _Color1 * _Pows.x * _Pows.w, 
                    tex2D(_Tex2, i.uv2) * _Color2 * _Pows.y * _Pows.w, 
                    tex2D(_Tex3, i.uv3) * _Color3 * _Pows.z * _Pows.w, 
                    c);

                float4 col = mul(float4(b.rgb,1.0-bsum), cmat) * _Color;
                return fluidOutMultiplier(col);
			}
			ENDCG
		}
	}
}
