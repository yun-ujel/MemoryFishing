Shader "Screen/Kuwahara"
{
	SubShader
	{
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
		LOD 100
        ZWrite Off Cull Off
        Pass
		{
			HLSLPROGRAM
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // The Blit.hlsl file provides the vertex shader (Vert),
            // input structure (Attributes) and output strucutre (Varyings)
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
			
            #pragma vertex Vert
            #pragma fragment frag

			float4 _BlitTexture_TexelSize;
            int _KernelSize;

            SamplerState sampler_point_clamp;

			float luminance(float3 color)
            {
                return dot(color, float3(0.299f, 0.587f, 0.114f));
            }

            float4 SampleQuadrant(float2 uv, int x1, int x2, int y1, int y2, int numSamples)
            {
                float luminanceSum = 0.0f;
                float sqrLuminanceSum = 0.0f;
                float3 colSum = 0.0f;

                [loop]
                for (int x = x1; x <= x2; x++)
                {
                    [loop]
                    for (int y = y1; y <= y2; y++)
                    {
                        float3 sample = _BlitTexture.Sample(sampler_point_clamp, uv + float2(x, y) * _BlitTexture_TexelSize.xy).rgb;
                        float l = luminance(sample);
                        luminanceSum += l;
                        sqrLuminanceSum += l * l;
                        colSum += saturate(sample);
                    }
                }

                float mean = luminanceSum / numSamples;
                float std = abs(sqrLuminanceSum / numSamples - mean * mean);

                return float4(colSum / numSamples, std);
            }

			float4 frag (Varyings input) : SV_Target
			{
                float windowSize = 2.0f * _KernelSize + 1;
                int quadrantSize = int(ceil(windowSize / 2.0f));
                int numSamples = quadrantSize * quadrantSize;

                float4 q1 = SampleQuadrant(input.texcoord, -_KernelSize, 0, -_KernelSize, 0, numSamples);
                float4 q2 = SampleQuadrant(input.texcoord, 0, _KernelSize, -_KernelSize, 0, numSamples);
                float4 q3 = SampleQuadrant(input.texcoord, 0, _KernelSize, 0, _KernelSize, numSamples);
                float4 q4 = SampleQuadrant(input.texcoord, -_KernelSize, 0, 0, _KernelSize, numSamples);
                
                float minstd = min(q1.a, min(q2.a, min(q3.a, q4.a)));
                int4 q = float4(q1.a, q2.a, q3.a, q4.a) == minstd;

                float alpha = _BlitTexture.Sample(sampler_point_clamp, input.texcoord).a;

                return saturate(float4(q1.rgb * q.x + q2.rgb * q.y + q3.rgb * q.z + q4.rgb * q.w, alpha));
			}
			ENDHLSL
		}
	}
}