Shader "Custom/WaterFBM"
{
    Properties
    {
        [Header(Color)][Space]
        _Color ("Base Color", Color) = (1, 1, 1, 1)
        
        _SpecularColor("Specular Color", Color) = (1, 1, 1, 1)
        _DiffuseColor("Diffuse Color", Color) = (1, 1, 1, 1)

        [Header(Lighting)][Space]

        _SpecularNormalStrength("Specular Normal Strength", Float) = 10
        _SpecularReflectance("Specular Reflectance", Float) = 1
        _Smoothness("Smoothness", Float) = 1

        [Space]

        _DiffuseNormalStrength("Diffuse Normal Strength", Float) = 10
        _DiffuseReflectance("Diffuse Reflectance", Float) = 1

        [Header(Waves)][Space]

        _VertexWaveCount("Vertex Wave Count", Int) = 12
        _FragmentWaveCount("Fragment Wave Count", Int) = 12

        [Space]
        
        _Frequency("Frequency", Float) = 0.4
        _FrequencyMultiplier("Frequency Multiplier", Float) = 1.18

        [Space]

        _Amplitude("Amplitude", Float) = 2
        _AmplitudeMultiplier("Amplitude Multiplier", Float) = 0.82

        [Space]

        _Speed("Speed", Float) = 0.5
        _SpeedMultiplier("Speed Multiplier", Float) = 1.07
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

            struct Wave
            {
                float2 direction;
                float2 origin;
            };
            
			StructuredBuffer<Wave> _Waves;

            int _VertexWaveCount;
            int _FragmentWaveCount;

            float _Frequency, _FrequencyMultiplier, _Amplitude, _AmplitudeMultiplier, _Speed, _SpeedMultiplier;

            float4 _Color, _SpecularColor, _DiffuseColor;

            float _SpecularNormalStrength, _SpecularReflectance, _Smoothness;
            float _DiffuseNormalStrength, _DiffuseReflectance;

            float4 Specular(float3 lightDir, float3 normal, float3 positionWS, float smoothness)
            {
                float3 viewDir = normalize(_WorldSpaceCameraPos - positionWS);
                float3 halfwayDir = normalize(lightDir + viewDir);
                float NdotH = saturate(dot(normal, halfwayDir));

                return pow(NdotH, smoothness) * _SpecularColor;
            }

            Interpolators Vertex(Attributes input)
            {
                float f = _Frequency;
                float a = _Amplitude;
                float speed = _Speed;
                
                VertexPositionInputs posnInputs = GetVertexPositionInputs(input.positionOS);
                float3 worldPos = posnInputs.positionWS;

                float amplitudeSum = 0.0f;

                float h = 0.0f;

                for (int i = 0; i < _VertexWaveCount; i++)
                {
                    float2 direction = _Waves[i].direction;

                    float x = dot(direction, worldPos.xz) * f + _Time.y * speed;
                    float wave = a * exp(sin(x) - 1);
                    float dx = wave * cos(x);

                    h += wave;
                    worldPos.xz += direction * -dx * a;

                    amplitudeSum += a;
                    f *= _FrequencyMultiplier;
                    a *= _AmplitudeMultiplier;
                    speed *= _SpeedMultiplier;
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

                float3 worldPos = input.positionWS;

                float2 n = 0.0f;

                for (int i = 0; i < _FragmentWaveCount; i++)
                {
                    float2 direction = _Waves[i].direction;

                    float x = dot(direction, worldPos.xz) * f + _Time.y * speed;
                    float wave = a * exp(sin(x) - 1);
                    float2 dw = f * direction * (wave * cos(x));

                    worldPos.xz += -dw * a;

                    n += dw;
                    
                    f *= _FrequencyMultiplier;
                    a *= _AmplitudeMultiplier;
                    speed *= _SpeedMultiplier;
                }
                
                float3 normal = normalize(TransformObjectToWorldNormal(normalize(float3(-n.x, 1.0f, -n.y))));

                float3 specNormal = normal;
                specNormal.xz *= _SpecularNormalStrength;
                specNormal = normalize(specNormal);

                float3 diffNormal = normal;
                diffNormal.xz *= _DiffuseNormalStrength;
                diffNormal = normalize(diffNormal);
                
                float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                Light light = GetMainLight(shadowCoord);
                float3 lightDir = normalize(light.direction);

                float DiffDotL = dot(diffNormal, lightDir);
                float diffuseReflectance = _DiffuseReflectance / PI;
                float3 diffuse = light.color * DiffDotL * diffuseReflectance * _DiffuseColor.rgb;

                float SpecDotL = dot(specNormal, lightDir);
                float4 spec = Specular(lightDir, specNormal, input.positionWS, _Smoothness) * SpecDotL;
                float specularReflectance = _SpecularReflectance / PI;
                float3 specular = specularReflectance * spec.rgb;

                float3 output = _Color.rgb + specular + diffuse;

	            return float4(output, 1.0f);
            }

            ENDHLSL
        }
    }
}
