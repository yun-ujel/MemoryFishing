Shader "Custom/Water"
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

            float4 _Color;
            float _Smoothness;

            float _NormalStrength;
            float _DiffuseReflectance;
            float _SpecularReflectance;

			struct Wave
			{
				float2 direction;
				float2 origin;

				float frequency;
				float amplitude;
				float phase;
				float steepness;
			};

			StructuredBuffer<Wave> _Waves;
			int _WaveCount;

            struct Attributes
            {
	            float4 positionOS : POSITION;
            };

            struct Interpolators
            {
	            float4 positionCS : SV_POSITION;

                float3 normal : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            float GetWaveCoord(float3 worldPos, float2 direction)
            {
                return (worldPos.x * direction.x) + (worldPos.z * direction.y);
            }
            
            float Sine(float3 worldPos, Wave w)
            {
                float xz = GetWaveCoord(worldPos, w.direction);
                float t = _Time.y * w.phase;

                return w.amplitude * sin(xz * w.frequency + t);
            }

            float3 CalculateOffset(float3 worldPos, Wave w)
			{
				return float3(0.0f, Sine(worldPos, w), 0.0f);
			}

            float WaveDDX(float3 worldPos, Wave w)
            {
                float xz = GetWaveCoord(worldPos, w.direction);
                float t = _Time.y * w.phase;

                return w.direction.x * cos(xz * w.frequency + t);
            }
            
            float WaveDDZ(float3 worldPos, Wave w)
            {
                float xz = GetWaveCoord(worldPos, w.direction);
                float t = _Time.y * w.phase;

                return w.direction.y * cos(xz * w.frequency + t);
            }

            float3 CalculateNormal(float3 worldPos, Wave w)
            {
                float3 tangent = float3(1, 0, WaveDDX(worldPos, w));
                float3 binormal = float3(0, 1, WaveDDZ(worldPos, w));

                return normalize(cross(tangent, binormal));
            }

            Interpolators Vertex(Attributes input)
            {
	            Interpolators output;

                VertexPositionInputs firstInputs = GetVertexPositionInputs(input.positionOS.xyz);
                float3 h = 0.0f;
                float3 n = 0.0f;

                for (int i = 0; i < _WaveCount; i++)
				{
					h += CalculateOffset(firstInputs.positionWS.xyz, _Waves[i]);
				}
                
                float4 position = input.positionOS + float4(h, 0.0f);
                
                VertexPositionInputs newInputs = GetVertexPositionInputs(position.xyz);
                output.positionWS = newInputs.positionWS.xyz;
                output.positionCS = newInputs.positionCS;

                for (int j = 0; j < _WaveCount; j++)
                {
                    n += CalculateNormal(newInputs.positionWS.xyz, _Waves[j]);
                }

                output.normal = normalize(TransformObjectToWorldNormal(normalize(float3(n.x, 1.0f, n.y))));

	            return output;
            }

            float Specular(float3 lightDir, float3 normal, float3 positionWS, float smoothness)
            {
                float3 viewDir = normalize(_WorldSpaceCameraPos - positionWS);
                float3 halfwayDir = normalize(lightDir + viewDir);
                float NdotH = saturate(dot(normal, halfwayDir));

                return pow(NdotH, smoothness);
            }

            float4 Fragment(Interpolators input) : SV_TARGET
            {
                float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                Light light = GetMainLight(shadowCoord);

                float3 normal = input.normal;
                normal.xz *= _NormalStrength;
                normal = normalize(normal);

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
