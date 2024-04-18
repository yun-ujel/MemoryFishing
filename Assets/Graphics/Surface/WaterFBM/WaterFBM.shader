Shader "Custom/WaterFBM"
{
    Properties
    {
        _Color ("Base Color", Color) = (1, 1, 1, 1)
        _Smoothness("Smoothness", Float) = 1

        _NormalStrength("Normal Strength", Float) = 10
        _DiffuseReflectance("Diffuse Reflectance", Float) = 1
        _SpecularReflectance("Specular Reflectance", Float) = 1
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalRenderPipeline" }
        LOD 100

        Pass
        {
            Name "UniversalForward"
            Tags{"LightMode" = "UniversalForward"}
            Cull Back
            Blend One Zero
            ZTest LEqual
            ZWrite On

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
	            float3 positionOS : POSITION;
            };

            struct Interpolators
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD2;
            };

            int _VertexWaveCount;
            int _FragmentWaveCount;

            float _Seed, _SeedIteration, _Frequency, _FrequencyMultiplier, _Amplitude, _AmplitudeMultiplier, _Speed, _SpeedMultiplier;

            float4 _Color;

            float _NormalStrength, _Smoothness;
            float _DiffuseReflectance, _SpecularReflectance;

            float Specular(float3 lightDir, float3 normal, float3 positionWS, float smoothness)
            {
                float3 viewDir = normalize(_WorldSpaceCameraPos - positionWS);
                float3 halfwayDir = normalize(lightDir + viewDir);
                float NdotH = saturate(dot(normal, halfwayDir));

                return pow(NdotH, smoothness);
            }

            Interpolators Vertex(Attributes input)
            {
                float f = _Frequency;
                float a = _Amplitude;
                float speed = _Speed;

                float seed = _Seed;
                
                VertexPositionInputs posnInputs = GetVertexPositionInputs(input.positionOS);
                float3 worldPos = posnInputs.positionWS;

                float amplitudeSum = 0.0f;

                float h = 0.0f;

                for (int i = 0; i < _VertexWaveCount; i++)
                {
                    float2 direction = normalize(float2(cos(seed), sin(seed)));

                    float x = dot(direction, worldPos.xz) * f + _Time.y * speed;
                    float wave = a * exp(sin(x) - 1);
                    float dx = wave * cos(x);

                    h += wave;
                    worldPos.xz += direction * -dx * a;

                    amplitudeSum += a;
                    f *= _FrequencyMultiplier;
                    a *= _AmplitudeMultiplier;
                    speed *= _SpeedMultiplier;
                    seed += _SeedIteration;
                }

                float3 newPos = input.positionOS + float3(0.0f, h / amplitudeSum, 0.0f);
                posnInputs = GetVertexPositionInputs(newPos);

                Interpolators output;

                output.positionCS = posnInputs.positionCS;
                output.positionWS = posnInputs.positionWS;

                return output;
            }

            float4 Fragment(Interpolators input) : SV_TARGET
            {
                float f = _Frequency;
                float a = _Amplitude;
                float speed = _Speed;

                float seed = _Seed;

                float3 worldPos = input.positionWS;

                float2 n = 0.0f;

                for (int i = 0; i < _FragmentWaveCount; i++)
                {
                    float2 direction = normalize(float2(cos(seed), sin(seed)));

                    float x = dot(direction, worldPos.xz) * f + _Time.y * speed;
                    float wave = a * exp(sin(x) - 1);
                    float2 dw = f * direction * (wave * cos(x));

                    worldPos.xz += -dw * a;

                    n += dw;
                    
                    f *= _FrequencyMultiplier;
                    a *= _AmplitudeMultiplier;
                    speed *= _SpeedMultiplier;
                    seed += _SeedIteration;
                }
                
                float3 normal = normalize(TransformObjectToWorldNormal(normalize(float3(-n.x, 1.0f, -n.y))));

                normal.xz *= _NormalStrength;
                normal = normalize(normal);
                
                float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                Light light = GetMainLight(shadowCoord);
                float3 lightDir = normalize(light.direction);

                float NdotL = dot(normal, lightDir);
                float diffuseReflectance = _DiffuseReflectance / PI;
                float3 diffuse = light.color * NdotL * diffuseReflectance;

                float spec = Specular(lightDir, normal, input.positionWS, _Smoothness) * NdotL;
                float3 specular = light.color * _SpecularReflectance * spec;

                float3 output = _Color + specular + diffuse;

	            return float4(output, 1.0f);
            }

            ENDHLSL
        }
    }
}
