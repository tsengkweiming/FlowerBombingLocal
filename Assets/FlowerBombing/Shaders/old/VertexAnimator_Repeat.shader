// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VertexAnimator_Repeat(AutoPlay)" {
    Properties{
        _MainTex("Base (RGB) Gloss (A)", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _Outline("Outline width", Range(.002, 0.03)) = .005
        _AnimTex("PosTex", 2D) = "white" {}
        _AnimTex_Scale("Scale", Vector) = (1,1,1,1)
        _AnimTex_Offset("Offset", Vector) = (0,0,0,0)
        _AnimTex_AnimEnd("End (Time, Frame)", Vector) = (0, 0, 0, 0)
        _AnimTex_AnimStart("Start (Time, Frame)", Float) = 0
        _AnimTex_T("Time", float) = 0
        _AnimTex_OffsetTime("OffsetTime", float) = 0
        _AnimTex_FPS("Frame per Sec(FPS)", Float) = 30
        _StopUpdate("Stop Update", Range(0, 1)) = 1

        _Hue("Hue", Range(0,1)) = 0
        _Saturation("Saturation", Range(0,1)) = 1
        _Value("Value", Range(0,1)) = 1
        _ClampEndTime("Clamp End Time (Time)", Range(0,50)) = 23
        _InitialTilt("Initial Tilt", Vector) = (0,0,0,1)
        //[PerRendererData] _InitialTilt("Initial Tilt", Vector) = (0,0,0,1)

    }
    SubShader{
        Tags { "RenderType" = "Opaque" }
        LOD 700 Cull Off

        Pass {
            Name "BaseTexture"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Assets/Packages/VertexAnimator/Shader/AnimTexture.cginc"
            #include "Assets/Packages/Gist/CGIncludes/Quaternion.cginc"

            struct vsin {
                uint vid: SV_VertexID;
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct vs2ps {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _Color;
            float _StepTime;
            float _StopUpdate;
            sampler2D _FlowerTex;

            uniform float _Outline;
            uniform float4 _OutlineColor;

            float _Hue;
            float _Saturation;
            float _Value;
            float _ClampEndTime;
            int _Turns;
            float _ScatterRate;
            sampler2D _NormalTex;
            float4x4 _WorldToUVMatrix;
            float4 _InitialTilt;
            float _AnimTex_OffsetTime;
            int _StopMotion;
            float random(float t) {
                return frac(sin(t * 12345.564) * 7658.76);
            }
            float4 quaternionFromNormalTexture() {
                float3 worldPivot = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
                float2 uv = mul(_WorldToUVMatrix, float4(worldPivot, 1)).xy;
                float3 n = normalize(2.0 * tex2Dlod(_NormalTex, float4(uv, 0, 0)).xyz - 1.0);
                //float4 initTilt = UNITY_ACCESS_INSTANCED_PROP(MyProps2, _InitialTilt);
                float4 initTilt = _InitialTilt;
                float4 q = qfromto(float3(0, 0, -1), -n);
                //q = qmultiply(q, qfromto(float3(0,1,0), float3(0,0,-1));
                q = qmultiply(q, initTilt);
                return q;
            }
            float3 getPosition(uint vid) {
                float t = _AnimTex_T + _AnimTex_OffsetTime + _StepTime;// _Time.y;
                t = clamp(t % _AnimTex_AnimEnd.x, _AnimTex_AnimStart, _Turns == 1 && _ScatterRate > 0.4 ? _ClampEndTime : _AnimTex_AnimEnd.x);
                float3 v;
                /*if (_SkipMotion.x <= t && t < _SkipMotion.y) {
                    float s = saturate((t - _SkipMotion.x) / (_SkipMotion.y - _SkipMotion.x));
                    float3 v0 = AnimTexVertexPos(vid, 10);
                    float3 v1 = AnimTexVertexPos(vid, 20);
                    v = lerp(v0, v1, s);
                }
                else {
                    v = AnimTexVertexPos(vid, t);
                }*/
                v = AnimTexVertexPos(vid, t);

                float4 q = quaternionFromNormalTexture();
                v = (_Turns <= 3 ? v : qrotate(q, v));
                return v;
            }

            vs2ps vert(vsin v) {
                float t = _AnimTex_T + _AnimTex_OffsetTime + _StepTime;// _Time.y;
                t = clamp(t % _AnimTex_AnimEnd.x, _AnimTex_AnimStart, _Turns == 1 && _ScatterRate > 0.4? _ClampEndTime : _AnimTex_AnimEnd.x);
                //v.vertex.xyz = AnimTexVertexPos(v.vid, t);

                v.vertex.xyz = getPosition(v.vid);

                vs2ps OUT;
                OUT.vertex = UnityObjectToClipPos(float4(v.vertex.xyz, 1));
                OUT.uv = v.texcoord;

                float3 norm = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
                float2 offset = TransformViewToProjection(norm.xy);

                //#ifdef UNITY_Z_0_FAR_FROM_CLIPSPACE //to handle recent standard asset package on older version of unity (before 5.5)
                //    OUT.vertex.xy += offset * _Outline;// * UNITY_Z_0_FAR_FROM_CLIPSPACE(o.pos.z)
                //#else
                //    OUT.vertex.xy += offset * _Outline;// * o.pos.z for camera perspective view 
                //#endif
                //OUT.color = _OutlineColor;
                return OUT;
            }

            fixed3 hsv2rgb(fixed3 c) {
                fixed4 K = fixed4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                fixed3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }
            float4 frag(vs2ps IN) : COLOR{
                return tex2D(_FlowerTex, IN.uv) * _Color *float4(hsv2rgb(fixed3(_Hue, _Saturation, _Value)), 1);
            }
            ENDCG
        }
        /*Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            CGPROGRAM
            #pragma multi_compile_shadowcaster
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Assets/Packages/VertexAnimator/Shader/AnimTexture.cginc"

            struct vsin {
                uint vid: SV_VertexID;
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
            };

            struct vs2ps {
                V2F_SHADOW_CASTER;
            };

            sampler2D _MainTex;
            float4 _Color;

            vs2ps vert(vsin v) {
                float t = _AnimTex_T;
                t = clamp(t % _AnimTex_AnimEnd.x, 0, _AnimTex_AnimEnd.x);
                v.vertex.xyz = AnimTexVertexPos(v.vid, t);

                vs2ps OUT;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(OUT);
                return OUT;
            }

            float4 frag(vs2ps IN) : COLOR {
                SHADOW_CASTER_FRAGMENT(IN);
            }
            ENDCG
        }*/

        //Tags { "RenderType" = "Opaque" }
        //Pass {
        //    Name "OUTLINE"
        //    Tags { "LightMode" = "Always" }
        //    Cull Front
        //    ZWrite On
        //    ColorMask RGB
        //    Blend SrcAlpha OneMinusSrcAlpha

        //    CGPROGRAM
        //    #pragma vertex vert
        //    #pragma fragment frag
        //    #pragma multi_compile_fog

        //    #include "UnityCG.cginc"
        //    #include "Assets/Packages/VertexAnimator/Shader/AnimTexture.cginc"

        //    struct vsin {
        //        uint vid: SV_VertexID;
        //        float4 vertex : POSITION;
        //        float2 texcoord : TEXCOORD0;
        //        float3 normal : NORMAL;
        //    };

        //    struct v2f {
        //        float4 vertex : SV_POSITION;
        //        float2 uv : TEXCOORD0;
        //        fixed4 color : COLOR;
        //    };

        //    uniform float _Outline;
        //    uniform float4 _OutlineColor;
        //    float _StepTime;

        //    v2f vert(vsin v) {
        //        float t = _AnimTex_T + _StepTime;// _Time.y;
        //        t = clamp(t % _AnimTex_AnimEnd.x, 0, _AnimTex_AnimEnd.x);
        //        v.vertex.xyz = AnimTexVertexPos(v.vid, t);

        //        v2f OUT;
        //        OUT.vertex = UnityObjectToClipPos(float4(v.vertex.xyz, 1));
        //        OUT.uv = v.texcoord;

        //        float3 norm = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
        //        float2 offset = TransformViewToProjection(norm.xy);

        //        #ifdef UNITY_Z_0_FAR_FROM_CLIPSPACE //to handle recent standard asset package on older version of unity (before 5.5)
        //            OUT.vertex.xy += offset * _Outline;// * UNITY_Z_0_FAR_FROM_CLIPSPACE(o.pos.z)
        //        #else
        //            OUT.vertex.xy += offset * _Outline;// * o.pos.z for camera perspective view 
        //        #endif
        //        OUT.color = _OutlineColor;
        //        UNITY_TRANSFER_FOG(OUT, OUT.vertex);
        //        return OUT;
        //    }
        //    fixed4 frag(v2f i) : SV_Target
        //    {
        //        UNITY_APPLY_FOG(i.fogCoord, i.color);
        //        return i.color;
        //    }
        //    ENDCG
        //}
    }
    FallBack "Toon/Basic"
}
