Shader "Custom/FullscreenRain"
{
    Properties
    {
        [Header(Rain Settings)]
        _RainScale ("Rain Scale", Float) = 20.0

        [Header(Static Rain Drop Settings)]
        _DropSpawnRate("Rain Drop Spawn Rate", Float) = 0.2
        _DropSpawnWindow ("Spawn Window", Float) = 10
        _MinDropSize ("Min Drop Size", Float) = 0.25
        _MaxDropSize ("Max Drop Size", Float) = 0.75
        _MinDropLifeTime ("Min Life", Float) = 1
        _MaxDropLifeTime ("Max Life", Float) = 10.0
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "Rain"
            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _RainScale;
            float _DropSpawnRate;
            float _DropSpawnWindow;
            float _MinDropSize;
            float _MaxDropSize;
            float _MinDropLifeTime;
            float _MaxDropLifeTime;
            
            float Hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            float StaticRainDrops(float2 uv)
            {
                // create the grid
                float2 rainUV = uv * _RainScale;
                
                // get the current cell
                float2 cell = floor(rainUV);
                
                // random drop size
                float rndSize = Hash(cell);
                float dropSize = lerp(_MinDropSize, _MaxDropSize, rndSize);

                // drop life time
                float rndTime = Hash(cell + 1.34);
                float life = lerp(_MinDropLifeTime, _MaxDropLifeTime, rndTime);
                float cycle = life + _DropSpawnWindow;

                // loop the drop life time cycle
                float t = fmod(_Time.y + rndTime * cycle, cycle);
                float progress = saturate(t / life);

                // shrink curve
                float shrink = 1.0 - progress;
                shrink *= shrink;

                // check if you should spawn a drop or not
                float spawnChance = Hash(cell + 2.34);
                float hasDrop = step(1.0 - _DropSpawnRate, spawnChance);

                // the position inside the cell
                float2 localUV = frac(rainUV);

                // get a random center and make sure the drop stays inside the cell
                float2 rndCenter = float2(Hash(cell + 3.34),Hash(cell + 4.34));
                float2 center = lerp(dropSize, 1.0 - dropSize, rndCenter);
                float dist = length(localUV - center);

                // droplet shape
                float finalSize = dropSize * shrink;
                float dropletShape = smoothstep(finalSize, 0.0, dist);
                return dropletShape * hasDrop;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord);

                // Scene
                half3 scene = SAMPLE_TEXTURE2D_X(
                    _BlitTexture,
                    sampler_LinearClamp,
                    uv
                ).rgb;

                float drop = StaticRainDrops(uv);

                float2 texel = 1.0 / _ScreenParams.xy;

                half3 blur =
                    SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv).rgb +
                    SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(texel.x, 0)).rgb +
                    SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - float2(texel.x, 0)).rgb +
                    SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(0, texel.y)).rgb +
                    SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - float2(0, texel.y)).rgb;

                blur /= 5.0;
                
                float inner = smoothstep(0.6, 0.9, drop);
                float outer = smoothstep(0.2, 0.6, drop);

                float edge = saturate(outer - inner);
                
                half3 result = lerp(scene, blur, inner);
                
                result += edge * 1.2;

                return half4(result, 1);
            }

            ENDHLSL
        }
    }
}