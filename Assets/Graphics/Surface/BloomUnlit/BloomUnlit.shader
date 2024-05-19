Shader "Unlit/Bloom"
{
	Properties
	{
		_Color ("Color", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
		
		Pass
		{
			Name "Main"
			Tags { "LightMode" = "SRPDefaultUnlit" }

			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "Assets/Graphics/Surface/BasicUnlit.hlsl"

            ENDHLSL
		}

		Pass
		{
			Name "Bloom"
			Tags { "LightMode" = "Bloom" }

			HLSLPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag

			#include "Assets/Graphics/Surface/BasicUnlit.hlsl"

            ENDHLSL
		}
	}
}