// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#pragma vertex vert
#pragma geometry geom
#pragma fragment frag


#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float3 scale : NORMAL;
};

struct v2g
{
    float4 pos : SV_POSITION;
    float3 scale : NORMAL;
};

struct g2f
{
    float4 pos : SV_POSITION;
    float3 normal : NORMAL;
    float3 worldPos : TEXCOORD1;
    float3 viewDir : TEXCOORD2;
};

float4 _MainColor;
float _Metallic;
float _Smoothness;

v2g vert (appdata v)
{
    v2g o;
    o.pos = v.vertex;
    o.scale = v.scale;
    return o;
}

g2f createCubeVertex(v2g p, float4 vertex, float3 normal){
    g2f o;
    float4 cubeVertex = p.pos + vertex * float4(p.scale,1);
    o.pos = UnityObjectToClipPos(cubeVertex);
    o.normal = UnityObjectToWorldNormal(normal);
    o.worldPos = mul(unity_ObjectToWorld, cubeVertex);
    o.viewDir = normalize(UnityWorldSpaceViewDir(cubeVertex));
    
    return o;
}

[maxvertexcount(24)] 
void geom(point v2g input[1], inout TriangleStream<g2f> triStream)
{
    float4 vertices[24] = {
        float4(-1, -1, 1, 1)/2,
        float4(1, -1, 1, 1)/2,  
        float4(-1, 1, 1, 1)/2,  
        float4(1, 1, 1, 1)/2,   

        float4(1, -1, 1, 1)/2,
        float4(1, -1, -1, 1)/2,         
        float4(1, 1, 1, 1)/2,
        float4(1, 1, -1, 1)/2,

        float4(1, -1, -1, 1)/2,
        float4(-1, -1, -1, 1)/2,            
        float4(1, 1, -1, 1)/2,
        float4(-1, 1, -1, 1)/2,

        float4(-1, -1, -1, 1)/2,
        float4(-1, -1, 1, 1)/2,         
        float4(-1, 1, -1, 1)/2,
        float4(-1, 1, 1, 1)/2,

        float4(-1, -1, -1, 1)/2,
        float4(1, -1, -1, 1)/2,         
        float4(-1, -1, 1, 1)/2,
        float4(1, -1, 1, 1)/2,

        float4(-1, 1, 1, 1)/2,
        float4(1, 1, 1, 1)/2,           
        float4(-1, 1, -1, 1)/2,
        float4(1, 1, -1, 1)/2
    };

    float3 normals[6] = {
        cross(float3(-1, -1, 1), float3(1, -1, 1)),
        cross(float3(1, -1, 1), float3(1, -1, -1)),
        cross(float3(1, -1, -1), float3(-1, -1, -1)),
        cross(float3(-1, -1, -1), float3(-1, -1, 1)),
        cross(float3(-1, -1, -1), float3(1, -1, -1)),
        cross(float3(-1, 1, 1), float3(1, 1, 1))
    };

    g2f o;
    for(uint i = 0; i < vertices.Length; i+=4){
        float3 normal = normalize(normals[i/4]);
        o = createCubeVertex(input[0], vertices[i], normal);
        triStream.Append(o);
        o = createCubeVertex(input[0], vertices[i+1], normal);
        triStream.Append(o);
        o = createCubeVertex(input[0], vertices[i+2], normal);
        triStream.Append(o);
        o = createCubeVertex(input[0], vertices[i+3], normal);
        triStream.Append(o);
        triStream.RestartStrip();
    }
}

UnityLight CreateLight (g2f i) {
	UnityLight light;
    
    #if defined(POINT)
        light.dir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
        light.dir.y+=0.5f;
    #else
        light.dir = _WorldSpaceLightPos0.xyz;
    #endif

    UNITY_LIGHT_ATTENUATION(attenuation, 0, i.worldPos);
    light.color = _LightColor0.rgb * attenuation;
    light.ndotl = DotClamped(i.normal, light.dir);
	return light;
}

float4 frag (g2f i) : SV_Target
{
    i.normal = normalize(i.normal);
    float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

    float3 albedo = _MainColor.rgb;

    float3 specularTint;
    float oneMinusReflectivity;
    albedo = DiffuseAndSpecularFromMetallic(
        albedo, _Metallic, specularTint, oneMinusReflectivity
    );

    UnityIndirect indirectLight;
    indirectLight.diffuse = 0.3;
    indirectLight.specular = 0;

    return UNITY_BRDF_PBS(
        albedo, specularTint,
        oneMinusReflectivity, _Smoothness,
        i.normal, i.viewDir,
        CreateLight(i), indirectLight
    );
}