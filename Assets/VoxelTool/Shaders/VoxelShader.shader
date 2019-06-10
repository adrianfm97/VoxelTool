// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Voxel/VoxelShader"
{
    Properties
    {
        _MainColor("Main Color", Color) = (1,1,1,1)

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
			Tags{"LightMode" = "ForwardBase"}
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float3 scale : NORMAL;
            };

            struct v2g
            {
                float4 vertex : SV_POSITION;
				float3 scale : NORMAL;
            };

            struct g2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                //float4 color : COLOR;
            };

            float4 _MainColor;

            v2g vert (appdata v)
            {
                v2g o;
                o.vertex = v.vertex;
				o.scale = v.scale;
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

				//v2g point = input[0];
                g2f o;

				for(uint i = 0; i < vertices.Length; i+=4){
					o.vertex = UnityObjectToClipPos(input[0].vertex + vertices[i] * input[0].scale);
                    o.normal = UnityObjectToWorldNormal(normals[i/4]);
					triStream.Append(o);
                    o.vertex = UnityObjectToClipPos(input[0].vertex + vertices[i+1] * input[0].scale);
                    o.normal = UnityObjectToWorldNormal(normals[i/4]);
					triStream.Append(o);
                    o.vertex = UnityObjectToClipPos(input[0].vertex + vertices[i+2] * input[0].scale);
                    o.normal = UnityObjectToWorldNormal(normals[i/4]);
					triStream.Append(o);
					o.vertex = UnityObjectToClipPos(input[0].vertex + vertices[i+3] * input[0].scale);
                    o.normal = UnityObjectToWorldNormal(normals[i/4]);
					triStream.Append(o);
					triStream.RestartStrip();
				}
            }

            fixed4 frag (g2f i) : SV_Target
            {
                return _MainColor;
            }
            ENDCG
        }
    }
}
