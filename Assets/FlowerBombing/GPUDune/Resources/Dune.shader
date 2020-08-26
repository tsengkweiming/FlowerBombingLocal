Shader "Hidden/Dune" {
	Properties {
		_NoiseFreq ("Noise frequency", Float) = 1
		_NoiseHeight ("Noise height", Float) = 1
		_UvToPosScale ("Position scale", Vector) = (1,1,0,0)
		_UvToPosOffset ("Position offset", Vector) = (0,0,0,0)
	}
	SubShader {
		Cull Off ZWrite Off ZTest Always

		CGINCLUDE
			#pragma multi_compile ___ HEIGHT

			#include "UnityCG.cginc"
			#include "Assets/Packages/HLSLNoise/all.cginc"
			
			#pragma vertex vert

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
		ENDCG

		Pass {
			Name "Normal Map"

			CGPROGRAM
			#pragma fragment frag

			float _NoiseFreq;
			float _NoiseHeight;
			float4 _UvToPosScale;
			float4 _UvToPosOffset;

			fixed4 frag (v2f IN) : SV_Target {
				float2 uv = IN.uv;
				float3 pos = float3(_UvToPosScale.xy * uv, 0) + _UvToPosOffset.xyz;
				float h = _NoiseHeight * snoise(_NoiseFreq * pos);

				float2 dh = float2(ddx(h), ddy(h));
				float2 dxy = float2(ddx(pos.x), ddy(pos.y));
				
				float2 dhdxy = dh / dxy;
				float3 n = normalize(float3(-dhdxy.x, dhdxy.y, 1));

				#if defined(HEIGHT)
				return 0.5 * (h + 1.0);
				#else
				return float4(0.5 * (n + 1.0), 0);
				#endif
			}
			ENDCG
		}
	}
}
