Shader "Hidden/OceanShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OceanColor ("OceanColor", Color) = (1, 1, 1, 1)
        _WaveColor ("WaveColor", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            fixed4 _OceanColor;
            fixed4 _WaveColor;

            float2 Rand2DVec(float2 uv)
            {
                float2 tuv = uv;
    
                tuv = float2( dot(tuv, float2(127.1, 311.7)),
                            dot(tuv, float2(269.5, 184.2)));
                
                return frac(sin(tuv) * 18.73462);
            }

            float2 VoronoiNoise(float2 uv, float cellDensity)
            {
                float2 cellNumber = floor(uv * cellDensity);
                float2 pointCoordinate = frac(uv * cellDensity);
    
                float2 distanceAndPointID = float2(10., 10.);
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        float2 neighboringGrid = float2( float(x),  float(y));
                        float2 randPointCoords = Rand2DVec(cellNumber + neighboringGrid);
            
                        float2 randomPoint = neighboringGrid - pointCoordinate + 0.5 + 0.5*sin(randPointCoords * 10. + _Time.y);
                        float distanceFromPoint = dot(randomPoint, randomPoint);
            
                        if (distanceFromPoint < distanceAndPointID.x)
                            distanceAndPointID = float2(distanceFromPoint, randPointCoords.x);
                    }
                }
    
                return distanceAndPointID;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                fixed2 uv = i.uv;
                uv -= 0.5;
                uv.x *= _ScreenParams.x / _ScreenParams.y;
                uv = floor(uv * 1028) / 1028;

                float value = VoronoiNoise(uv + 0.01*sin(_Time.y), 50.).x;

                // just invert the colors
                col.rgb = lerp(_OceanColor, _WaveColor, value);
                col = floor(col * 16) / 16;
                return col;
            }
            ENDCG
        }
    }
}
