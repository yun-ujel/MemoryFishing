#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

struct Attributes
{
    float4 positionOS : POSITION;
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
};

float4 _Color;

Varyings vert(Attributes input)
{
    Varyings output;

    VertexPositionInputs vertInputs = GetVertexPositionInputs(input.positionOS.xyz);
    output.positionCS = vertInputs.positionCS;

    return output;
}

float4 frag(Varyings input) : SV_Target
{
    return _Color;
}