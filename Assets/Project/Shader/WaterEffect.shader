Shader "Unlit/WaterEffect"
{
    Properties
    {
        _MainTex ("Background Texture", 2D) = "white" {}
        _WaveSpeed ("Wave Speed", Range(0.1, 5.0)) = 1.0
        _DistortionStrength ("Distortion Strength", Range(0, 0.1)) = 0.02
        _LightColor ("Light Color", Color) = (0.0, 0.5, 0.7, 1.0)
        _BlurStrength ("Blur Strength", Range(0, 5)) = 1.0
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Propriedades
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _WaveSpeed;
            float _DistortionStrength;
            float _BlurStrength;
            float4 _LightColor;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float2 WaveDistortion(float2 uv, float time, float strength)
            {
                uv.x += sin(uv.y * 20.0 + time * _WaveSpeed) * strength;
                uv.y += cos(uv.x * 20.0 + time * _WaveSpeed) * strength;
                return uv;
            }

            fixed4 ApplyBlur(sampler2D tex, float2 uv, float blurStrength)
            {
                fixed4 col = tex2D(tex, uv) * 0.36;

                col += tex2D(tex, uv + float2(blurStrength, 0)) * 0.16;
                col += tex2D(tex, uv - float2(blurStrength, 0)) * 0.16;
                col += tex2D(tex, uv + float2(0, blurStrength)) * 0.16;
                col += tex2D(tex, uv - float2(0, blurStrength)) * 0.16;

                return col;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Time.y;


                float2 distortedUV = WaveDistortion(i.uv, time, _DistortionStrength);

                fixed4 col = ApplyBlur(_MainTex, distortedUV, _BlurStrength);

                float lightEffect = sin(i.uv.y * 10.0 + time * 2.0) * 0.1 + 0.1;

                col.rgb += _LightColor.rgb * lightEffect;

                return col;
            }
            ENDCG
        }
    }
}
