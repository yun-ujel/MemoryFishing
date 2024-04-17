Shader "Custom/Water"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
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

                for (int wi = 0; wi < _WaveCount; wi++)
				{
					h += CalculateOffset(firstInputs.positionWS.xyz, _Waves[wi]);
				}
                
                float4 position = input.positionOS + float4(h, 0.0f);
                
                VertexPositionInputs newInputs = GetVertexPositionInputs(position.xyz);
                output.positionWS = newInputs.positionWS.xyz;
                output.positionCS = newInputs.positionCS;

                for (int wi = 0; wi < _WaveCount; wi++)
                {
                    n += CalculateNormal(newInputs.positionWS.xyz, _Waves[wi]);
                }

                output.normal = normalize(TransformObjectToWorldNormal(normalize(float3(-n.x, 1.0f, -n.y))));

	            return output;
            }

            float4 Fragment(Interpolators input) : SV_TARGET
            {
                float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                Light light = GetMainLight(shadowCoord);

                float d = dot(input.normal, light.direction);

	            return d;
            }
            ENDHLSL
        }
    }
}
