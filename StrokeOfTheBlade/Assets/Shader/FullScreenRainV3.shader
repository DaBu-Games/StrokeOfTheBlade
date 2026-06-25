Shader "Custom/FullscreenRainV3"
{
    Properties
    {
        [Header(Static Rain Drop Settings)]
        _SRainDropScale("Static Rain Drop Scale", Float) = 20
        
        [Header(Dynamic Rain Drop Settings)]
        _DRainDropScale ("Dynamic rain drop scale", Float) = 6
        _Distortion ("Distortion", Range(-5, 5)) = 0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "Rain shader"
            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment Frag

            // smoothstep short cut
            #define S(a, b, t) smoothstep(a, b, t)

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _SRainDropScale;
            
            float _DRainDropScale;
            float _Distortion;

            float N21(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            float2 StaticRainDrop(float2 uv)
            {
                float2 rainUV = uv * _SRainDropScale;

                float2 cell = floor(rainUV);
                float2 gv = frac(rainUV) - 0.5;

                float2 rnd = float2(
                    N21(cell + 12.56),
                    N21(cell + 12.34)
                );

                float noise = N21(cell);

                // per-cell offset so drops are not synced
                float t = (_Time.y * 0.5) + noise * 6.2831;

                // base oscillation 0 → 1 → 0
                float wave = sin(t * 0.6) * 0.5 + 0.5;

                // 🔥 create a "dead zone" (pause when invisible)
                float gate = step(0.15, wave) * step(wave, 0.85);

                // smooth fade inside active range
                float life = smoothstep(0.15, 0.4, wave) * (1.0 - smoothstep(0.6, 0.85, wave));

                // final combined
                life *= gate;

                float2 center = (rnd - 0.5) * 0.8;

                float2 dir = gv - center;

                float dist = length(dir);

                float mask = S(0.08, 0.04, dist);

                return dir * mask * life;
            }
            
            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord);

                float t = fmod(_Time.y, 7200); // reset after every 2 hours
                
                float4 col = 0;

                float2 aspect = float2(2, 1);
                float2 rainUV = uv * _DRainDropScale * aspect;
                // slowly move the whole grid down using the offset
                rainUV.y += t * 0.25;
                // get the center point of the cell
                float2 gv = frac(rainUV) - 0.5;
                float2 seed = floor(rainUV);
                
                float noise = N21(seed); // random number between 0-1
                t += noise * 6.2831;
                
                float wiggels = uv.y * 10;
                float x = ((noise - 0.5) * 0.8); // random nr between -0.4- 0.4
                x += ( 0.4 - abs(x)) * sin(3 * wiggels) * pow(sin(wiggels), 6) * 0.45;
                
                float y = -sin(t + sin(t + sin(t) * 0.5)) * 0.45;
                y -= (gv.x - x) * (gv.x - x);
                
                float2 dropPos = (gv - float2(x, y)) / aspect;
                float drop = S(0.05, 0.03, length(dropPos));

                float2 trailPos = (gv - float2(x, t * 0.25)) / aspect;
                trailPos.y = (frac(trailPos.y * 8) - 0.5 ) / 8;
                float trail = S(0.03, 0.01, length(trailPos));
                
                float fogTrail = S(-0.05, 0.05, dropPos.y);
                fogTrail *= S(0.5, y, gv.y);
                trail *= fogTrail;

                //if (gv.x > 0.48 || gv.y > 0.49)
                //   col = float4(1,0,0,1);

                float2 staticRainMask = StaticRainDrop(uv);

                float2 offset = drop * dropPos + trail * trailPos + staticRainMask; //trail;
                offset *= _Distortion;
                col.rgb = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + offset).rgb;
                
                return col;
            }

            ENDHLSL
        }
    }
}