Shader "Unlit/Normals"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

        	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
		        float3 normalOS : NORMAL;
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float3 normalWS : TEXCOORD2;
                float4 positionCS : SV_POSITION;
            };

            float4 _Color;

            Varyings vert (Attributes input)
            {
                Varyings output;

                output.normalWS = TransformObjectToWorldNormal(input.normalOS);

                VertexPositionInputs vertInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertInputs.positionCS;

                return output;
            }

            float4 frag (Varyings input) : SV_Target
            {
                return float4(input.normalWS, 1);
            }
            ENDHLSL
        }
    }
}
