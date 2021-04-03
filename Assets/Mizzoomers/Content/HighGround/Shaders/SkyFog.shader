Shader "Unlit/SkyFog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Fog ("Fog", Color) = (1, 1, 1, 1)
        _FadeDistance ("Fade Distance", Float) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            float hash(float3 p)
            {
                p = frac(p * 0.3183099 + .1);
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float randColor : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 _Color, _Fog;
            float _FadeDistance;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.randColor = hash(mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz) * 0.1;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 normal = normalize(i.normal);
                float3 lightDir = normalize(float3(1, 3, -1));

                float3 col = tex2D(_MainTex, i.uv).rgb * _Color;

                float light = dot(normal, lightDir);
                light = max(0, light) + 0.5;

                col *= light;

                float fade = saturate(i.worldPos.y / _FadeDistance);
                fade -= 0.15 + i.randColor;
                fade = saturate(fade);
                fade = fade * fade;

                col = lerp(_Fog, col, fade);

                return float4(col, 1);
            }
            
            ENDCG
        }
    }
}