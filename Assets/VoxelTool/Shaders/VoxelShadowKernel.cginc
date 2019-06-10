// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


#pragma vertex SHADOW_VS_Main
#pragma fragment SHADOW_FS_Main
#pragma geometry SHADOW_GS_Main
#pragma multi_compile_shadowcaster
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"


// **************************************************************
// Data structures                                              *
// **************************************************************
struct SHADOW_GS_INPUT
{
    V2F_SHADOW_CASTER;
};
struct SHADOW_FS_INPUT
{
    V2F_SHADOW_CASTER;
};
// **************************************************************
// Vars                                                         *
// **************************************************************

float _Size;
float4x4 _VP;
// **************************************************************
// Shader Programs                                              *
// **************************************************************

// Vertex Shader ------------------------------------------------
SHADOW_GS_INPUT SHADOW_VS_Main(appdata_base v)
{
    SHADOW_GS_INPUT output = (SHADOW_GS_INPUT)0;
    output.pos =  mul(unity_ObjectToWorld, v.vertex);
    //TRANSFER_SHADOW_CASTER(output)
    return output;
}
// Geometry Shader -----------------------------------------------------
[maxvertexcount(4)]
void SHADOW_GS_Main(point SHADOW_GS_INPUT p[1], inout TriangleStream<SHADOW_FS_INPUT> triStream)
{
    //get up vector
    float3 up = UNITY_MATRIX_IT_MV[1].xyz;
    //get look vector
    float3 look = _WorldSpaceCameraPos - p[0].pos;
    //look.y = 0;
    look = normalize(look);
    //get right vector
    float3 right = cross(up, look);
    //half the size please..
    float halfS = 0.5f * _Size;
    //offsets
    float4 v[4];
    v[0] = float4(p[0].pos + halfS * right - halfS * up, 1.0f);
    v[1] = float4(p[0].pos + halfS * right + halfS * up, 1.0f);
    v[2] = float4(p[0].pos - halfS * right - halfS * up, 1.0f);
    v[3] = float4(p[0].pos - halfS * right + halfS * up, 1.0f);
    float4x4 vp = UnityObjectToClipPos(unity_WorldToObject);

    SHADOW_FS_INPUT pIn=p[0];
    //TRANSFER_SHADOW_CASTER(pIn)
    pIn.pos = mul(vp, v[0]);
    triStream.Append(pIn);
    //TRANSFER_SHADOW_CASTER(pIn)
    pIn.pos =  mul(vp, v[1]);
    triStream.Append(pIn);
    //TRANSFER_SHADOW_CASTER(pIn)
    pIn.pos =  mul(vp, v[2]);
    triStream.Append(pIn);
    //TRANSFER_SHADOW_CASTER(pIn)
    pIn.pos =  mul(vp, v[3]);
    triStream.Append(pIn);
    //TRANSFER_SHADOW_CASTER(pIn)
}
// Fragment Shader -----------------------------------------------
float4 SHADOW_FS_Main(SHADOW_FS_INPUT input) : COLOR
{
    SHADOW_CASTER_FRAGMENT(input)
    //return _SpriteTex.Sample(sampler_SpriteTex, input.tex0);
}