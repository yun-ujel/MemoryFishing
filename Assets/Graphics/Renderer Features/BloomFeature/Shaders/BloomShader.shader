Shader "Screen/Bloom"
{
	SubShader
	{
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
		LOD 100
		ZWrite Off Cull Off

		Pass
		{
			Name "Main"

			HLSLPROGRAM
			// The Blit.hlsl file provides the vertex shader (Vert),
            // input structure (Attributes) and output strucutre (Varyings)

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
			
            #pragma vertex Vert
            #pragma fragment frag

			TEXTURE2D(_BloomTexture);
			SamplerState sampler_bilinear_clamp;

			float4 frag (Varyings input) : SV_Target
			{
				float3 sample = _BloomTexture.Sample(sampler_bilinear_clamp, input.texcoord).rgb;

				return float4(sample, 1);
			}

			ENDHLSL
		}

		Pass
		{
			Name "Downsample"

			HLSLPROGRAM

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
			
            #pragma vertex Vert
            #pragma fragment frag

			float4 _BlitTexture_TexelSize;
			float _DownDelta;
		
			SamplerState sampler_bilinear_clamp;

			float3 Sample(float2 uv)
			{
                return _BlitTexture.Sample(sampler_bilinear_clamp, uv).rgb;
            }

			float3 SampleBox(float2 uv, float delta)
			{
				float4 o = _BlitTexture_TexelSize.xyxy * float2(-delta, delta).xxyy;
				float3 s = Sample(uv + o.xy) + Sample(uv + o.zy) + Sample(uv + o.xw) + Sample(uv + o.zw);
				
				return s * 0.25f;
			}

			float4 frag (Varyings input) : SV_Target
			{
				return float4(SampleBox(input.texcoord, _DownDelta), 1.0f);
			}

			ENDHLSL
		}

		Pass
		{
			Name "Upsample"

			HLSLPROGRAM

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
			
            #pragma vertex Vert
            #pragma fragment frag

			float4 _BlitTexture_TexelSize;
			float _UpDelta;
		
			SamplerState sampler_bilinear_clamp;

			float3 Sample(float2 uv)
			{
                return _BlitTexture.Sample(sampler_bilinear_clamp, uv).rgb;
            }

			float3 SampleBox(float2 uv, float delta)
			{
				float4 o = _BlitTexture_TexelSize.xyxy * float2(-delta, delta).xxyy;
				float3 s = Sample(uv + o.xy) + Sample(uv + o.zy) + Sample(uv + o.xw) + Sample(uv + o.zw);
				
				return s * 0.25f;
			}

			float4 frag (Varyings input) : SV_Target
			{
				return float4(SampleBox(input.texcoord, _UpDelta), 1.0f);
			}

			ENDHLSL
		}

		Pass
		{
			Name "Blend"

			HLSLPROGRAM

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
			
            #pragma vertex Vert
            #pragma fragment frag

			float4 _BloomTexture_TexelSize;
			TEXTURE2D(_BloomTexture);
		
			SamplerState sampler_bilinear_clamp;
			SamplerState sampler_point_clamp;
			float _Intensity;

			float3 Sample(float2 uv)
			{
                return _BloomTexture.Sample(sampler_bilinear_clamp, uv).rgb;
            }

			float3 SampleBox(float2 uv, float delta)
			{
				float4 o = _BloomTexture_TexelSize.xyxy * float2(-delta, delta).xxyy;
				float3 s = Sample(uv + o.xy) + Sample(uv + o.zy) + Sample(uv + o.xw) + Sample(uv + o.zw);
				
				return s * 0.25f;
			}

			float4 frag (Varyings input) : SV_Target
			{
				float3 color = _BlitTexture.Sample(sampler_point_clamp, input.texcoord);
				color += pow(_Intensity * pow(SampleBox(input.texcoord, 0.5f), 1.0f / 2.2f), 2.2f);

				return float4(color, 1.0f);
			}

			ENDHLSL
		}
	}
}