Shader "Custom/FullscreenRainV2"
{
    Properties
    {
        [Header(Rain Settings)]
        _RainScale ("Rain Scale", Float) = 20.0
        
        [Header(Rain Drop Settings)]
        _DropTime ("Drop time", Float) = 1
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

            float _RainScale;
            
            float _DropTime;
            float _Distortion;

            float N21(float2 p)
            {
                p = frac(p * float2(123.34, 345.45));
                p += dot(p, p + 34.345);
                return frac(p.x * p.y);
            }
            
            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord);

                float t = fmod(_Time.y + _DropTime, 7200); // reset after every 2 hours
                
                float4 col = 0;

                float2 aspect = float2(2, 1);
                float2 rainUV = uv * _RainScale * aspect;
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
                fogTrail *= S(0.05, 0.04, abs(dropPos.x));

                col += fogTrail * 0.5;
                col += trail;
                col += drop;

                //if (gv.x > 0.48 || gv.y > 0.49)
                //   col = float4(1,0,0,1);

                float2 offset = drop * dropPos + trail * trailPos; //trail;
                offset *= _Distortion;
                col.rgb = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + offset).rgb;
                
                return col;
            }

            ENDHLSL
        }
    }
}