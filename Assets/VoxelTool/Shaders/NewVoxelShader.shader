// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Voxel/NewVoxelShader"
{
    Properties
    {
        _MainColor("Main Color", Color) = (1,1,1,1)
		_Metallic("Metallic", Range(0,1)) = 0.5
		_Smoothness("Smoothness", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
			Tags{"LightMode" = "ForwardBase"}
            CGPROGRAM
			#include "VoxelKernel.cginc"

			#pragma target 5.0

			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
            
            ENDCG
        }
		Pass
        {
			Tags{"LightMode" = "ForwardAdd"}

			Blend One One
			ZWrite Off

            CGPROGRAM
			#include "VoxelKernel.cginc"

			#pragma target 5.0

			#pragma multi_compile DIRECTIONAL POINT

			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag


			ENDCG
		}
    }
}