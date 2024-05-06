Shader "Custom/WaterFBM"
{
    Properties
    {
        [Header(Color)][Space]
        _Color ("Base Color", Color) = (1, 1, 1, 1)
        
        _SpecularColor("Specular Color", Color) = (1, 1, 1, 1)
        _DiffuseColor("Diffuse Color", Color) = (1, 1, 1, 1)

        [Header(Foam)][Space]

        [Toggle(ENABLE_FOAM)] _EnableFoam ("Enable Foam", Float) = 1

        [Space]

        _FoamColor ("Foam Color", Color) = (1, 1, 1, 1)
        _TimeOffset ("Time Offset", Float) = 1

        [Space][Space]

        _FoamPower ("Foam Power", Float) = 2
        _FoamMultiplier ("Foam Multiplier", Float) = 1
        _OffsetFoamMultiplier ("Offset Foam Multiplier", Float) = 1

        [Space][Space]

        _FoamTexture ("Foam Texture", 2D) = "white" {}
        _FoamTextureSpread ("Spread", Float) = 0.1

        [Header(Lighting)][Space]

        _SpecularNormalStrength("Specular Normal Strength", Float) = 10
        _SpecularReflectance("Specular Reflectance", Float) = 1
        _Smoothness("Smoothness", Float) = 1

        [Space][Space]

        _DiffuseNormalStrength("Diffuse Normal Strength", Float) = 10
        _DiffuseReflectance("Diffuse Reflectance", Float) = 1

        [Header(Waves)][Space]

        _VertexWaveCount("Vertex Wave Count", Int) = 12
        _FragmentWaveCount("Fragment Wave Count", Int) = 12

        [Space][Space]
        
        _Frequency("Frequency", Float) = 0.4
        _FrequencyMultiplier("Frequency Multiplier", Float) = 1.18

        [Space][Space]

        _Amplitude("Amplitude", Float) = 2
        _AmplitudeMultiplier("Amplitude Multiplier", Float) = 0.82

        [Space][Space]

        _Speed("Speed", Float) = 0.5
        _SpeedMultiplier("Speed Multiplier", Float) = 1.07

        [Space][Space]

        _VertexHeightMultiplier("Vertex Height Multiplier", Float) = 1
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

            #pragma multi_compile __ ENABLE_FOAM

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
	            float3 positionOS : POSITION;
                float2 UV : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD2;
                float2 UV : TEXCOORD0;
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

            float _TimeOffset, _FoamPower;
            float _FoamMultiplier, _OffsetFoamMultiplier;
            float4 _FoamColor;

            TEXTURE2D(_FoamTexture);
            float _FoamTextureSpread;
            float4 _FoamTexture_ST;

            SamplerState sampler_bilinear_repeat;

            float _VertexHeightMultiplier;

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

                float3 newPos = input.positionOS + float3(0.0f, (h / amplitudeSum) * _VertexHeightMultiplier, 0.0f);
                posnInputs = GetVertexPositionInputs(newPos);

                Interpolators output;

                output.positionCS = posnInputs.positionCS;
                output.positionWS = posnInputs.positionWS;

                output.UV = TRANSFORM_TEX(input.UV, _FoamTexture);

                return output;
            }

            float4 Fragment(Interpolators input) : SV_TARGET
            {
                float f = _Frequency;
                float a = _Amplitude;
                float speed = _Speed;

                float3 worldPos = input.positionWS;

                float2 n = 0.0f;
                float foamH = 0.0f;

                float amplitudeSum = 0.0f;

                for (int i = 0; i < _FragmentWaveCount; i++)
                {
                    float2 direction = _Waves[i].direction;

                    float x = dot(direction, worldPos.xz) * f + _Time.y * speed;
                    float wave = a * exp(sin(x) - 1);
                    float2 dw = f * direction * (wave * cos(x));

                    #ifdef ENABLE_FOAM
                    float foamX = dot(direction, worldPos.xz) * f + (_Time.y + _TimeOffset) * speed;
                    float foamWave = a * exp(sin(foamX) - 1);

                    foamH += foamWave * _OffsetFoamMultiplier;
                    foamH += wave * _FoamMultiplier;
                    #endif

                    worldPos.xz += -dw * a;

                    n += dw;
                    amplitudeSum += a;
                    
                    f *= _FrequencyMultiplier;
                    a *= _AmplitudeMultiplier;
                    speed *= _SpeedMultiplier;
                }

                #ifdef ENABLE_FOAM
                foamH /= amplitudeSum;
                foamH = pow(foamH, _FoamPower);
                
                float dither = _FoamTexture.Sample(sampler_bilinear_repeat, input.UV).r;
                foamH -= dither * _FoamTextureSpread;
                foamH = max(foamH, 0);
                #endif
                
                float3 normal = normalize(TransformObjectToWorldNormal(normalize(float3(-n.x, 1.0f, -n.y))));

                float3 specNormal = normal;
                specNormal.xz *= _SpecularNormalStrength;
                specNormal = normalize(specNormal);

                float3 diffNormal = normal;
                diffNormal.xz *= _DiffuseNormalStrength;
                diffNormal = normalize(diffNormal);
                
                float4 shadowCoord = TransformWorldToShadowCoord(worldPos);
                Light light = GetMainLight(shadowCoord, worldPos, half4(1, 1, 1, 1));
                float3 lightDir = normalize(light.direction);

                float DiffDotL = dot(diffNormal, lightDir);
                float diffuseReflectance = _DiffuseReflectance / PI;
                float3 diffuse = light.color * DiffDotL * diffuseReflectance * _DiffuseColor.rgb;

                float SpecDotL = dot(specNormal, lightDir);
                float4 spec = Specular(lightDir, specNormal, worldPos, _Smoothness) * SpecDotL;
                float specularReflectance = _SpecularReflectance / PI;
                float3 specular = specularReflectance * spec.rgb;

                #ifdef ENABLE_FOAM
                float3 albedo = lerp(_Color.rgb, _FoamColor.rgb, foamH);
                #else
                float3 albedo = _Color.rgb;
                #endif

                float3 output = albedo + specular + diffuse;
                output *= MainLightRealtimeShadow(shadowCoord);

	            return float4(output, 1.0f);
            }

            ENDHLSL
        }
    }
}
