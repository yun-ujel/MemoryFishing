Shader "Custom/Water"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)

		_Smoothness ("Smoothness", Range(0, 1)) = 0.5
		_NormalStrength("Normal Strength", Range(0, 1)) = 0.5
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vp
			#pragma fragment fp

			#include "UnityPBSLighting.cginc"

			sampler2D _MainTex;

			struct VertexData
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
			};

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

			fixed4 _Color;

			float _Smoothness, _NormalStrength;

			float GetWaveCoord(float3 v, float2 d, Wave w)
			{
				return (v.x * d.x) + (v.z * d.y);
			}

			float GetTime(Wave w)
			{
				return _Time.y * w.phase;
			}

			float Sine(float3 v, Wave w)
			{
				float xz = GetWaveCoord(v, w.direction, w);
				float t = GetTime(w);

				return w.amplitude * sin(xz * w.frequency + t);
			}

			float3 SineNormal(float3 v, Wave w)
			{
				float xz = GetWaveCoord(v, w.direction, w);
				float t = GetTime(w);

				float2 n = w.frequency * w.amplitude * w.direction * cos(xz * w.frequency + t);

				return float3(n.x, n.y, 0.0f);
			}

			float3 CalculateOffset(float3 v, Wave w)
			{
				return float3(0.0f, Sine(v, w), 0.0f);
			}

			float3 CalculateNormal(float3 v, Wave w)
			{
				return SineNormal(v, w);
			}

			v2f vp(VertexData v)
			{
				v2f i;
				i.worldPos = mul(unity_ObjectToWorld, v.vertex);
				
				float3 h = 0.0f;
				float3 n = 0.0f;
				
				for (int wi = 0; wi < _WaveCount; wi++)
				{
					h += CalculateOffset(i.worldPos, _Waves[wi]);
				}

				float4 newPos = v.vertex + float4(h, 0.0f);
				i.worldPos = mul(unity_ObjectToWorld, newPos);
				i.pos = UnityObjectToClipPos(newPos);

				return i;
			}
	
			float4 fp(v2f i) : SV_TARGET
			{
				float3 lightDir = normalize(_WorldSpaceLightPos0);

				float3 normal = 0.0f;

				for (int wi = 0; wi < _WaveCount; ++wi)
				{
					normal += CalculateNormal(i.worldPos, _Waves[wi]);
				}

				normal = normalize(normal);
				normal.xz *= _NormalStrength;
				normal = normalize(normal);

				float ndotl = dot(lightDir, normal);
				ndotl = (ndotl + 1) / 2;

				float3 diffuse = _LightColor0.rgb * ndotl;
				
				
				float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
				float3 halfwayDir = normalize(lightDir + viewDir);

				float specdot = dot(normal, halfwayDir);
				specdot = (specdot + 1) / 2;

				float specular = pow(specdot, _Smoothness * 100);

				
				float3 output = diffuse + _Color + specular;

				return float4(output, 1.0f);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
} 