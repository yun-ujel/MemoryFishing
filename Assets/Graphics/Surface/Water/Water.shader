Shader "Custom/Water"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_Amplitude ("Amplitude", Float) = 1
		_Wavelength ("Wavelength", Float) = 10
		_Speed ("Speed", Float) = 2
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

			sampler2D _MainTex;

			struct VertexData
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};


			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

			float _Amplitude, _Wavelength, _Speed;

			v2f vp(VertexData v)
			{
				float3 p = v.vertex.xyz;

				float k = 2 * 3.141 / _Wavelength;
				p.y = _Amplitude * sin(k * (p.x - _Speed * _Time.y));
				
				v.vertex.xyz = p;

				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				
				return o;
			}
	
			float4 fp(v2f i) : SV_TARGET
			{
				return _Color;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
} 