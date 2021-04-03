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
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 _Color, _Fog;
            float _FadeDistance;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 normal = normalize(i.normal);
                float3 lightDir = normalize(float3(1, 3, -1));
                
                float3 col = tex2D(_MainTex, i.uv).rgb * _Color;
                
                float light = dot(normal, lightDir);
                light = max(0, light) + 0.5;

                col *= light;

                
                float fade = saturate(i.worldPos.y / _FadeDistance);
                fade -= 0.2;
                fade = saturate(fade);
                fade = fade * fade;

                col = lerp(_Fog, col, fade);
                // col = float3(fade, fade, fade);
                
                return float4(col, 1);
            }
            ENDCG
        }
    }
}
