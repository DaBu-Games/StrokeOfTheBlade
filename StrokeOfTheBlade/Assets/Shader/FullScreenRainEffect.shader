Shader "Custom/FullscreenRain"
{
    Properties
    {
        _RainIntensity ("Rain Intensity", Float) = 1.0
        _RainSize ("Droplet Size", Float) = 0.15
        _RainBlur ("Blur Strength", Float) = 0.002
        _RainScale ("Rain Scale", Float) = 20.0
        _RainTex ("Rain Mask", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "Invert"
            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _RainIntensity;
            float _RainSize;
            float _RainBlur;
            float _RainScale;

            TEXTURE2D(_RainTex);
            SAMPLER(sampler_RainTex);
            
            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord);

                float2 texel = 1.0 / _ScreenParams.xy;
                float2 offset = texel * _RainBlur;

                half4 sharp = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

                half4 blur = 0;
                blur += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
                blur += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + offset);
                blur += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - offset);
                blur += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(0, offset.y));
                blur += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - float2(0, offset.y));
                blur *= 0.2;

                float2 rainUV = uv * _RainScale;

                float t = _Time.y;

                rainUV.y += t * 0.2;
                rainUV.x += sin(rainUV.y * 5.0 + t) * 0.02;

                float noise = SAMPLE_TEXTURE2D(_RainTex, sampler_RainTex, rainUV).r;
                
                float inner = step(0.85, noise);
                
                float outer = step(0.80, noise);
                float edge = inner - outer;
                
                half4 col = lerp(sharp, blur, inner);
                
                col.rgb += edge * float3(1,1,1) * 2.0;

                return col;
            }

            ENDHLSL
        }
    }
}