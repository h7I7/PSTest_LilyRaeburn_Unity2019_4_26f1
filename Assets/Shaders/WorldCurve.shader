////////////////////////////////////////////////////////////
// Author: Lily Raeburn
// File Name: TestShader.shader
// Description: 
// Date Created: 11/05/2021
// Last Edit: 13/05/2021
// Comments: 
////////////////////////////////////////////////////////////

Shader "Custom/World Curve"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (0.0, 0.0, 0.0, 1.0)
        _Radius("World Radius", Float) = 0.002
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert addshadow

        // Handy function pulled from StackOverflow, I'm not going to pretend to understand the swizzling happening here but it works! c:
        // https://stackoverflow.com/questions/54170722/simply-get-the-scaling-of-an-object-inside-the-cg-shader
        half3 ObjectScale()
        {
            return half3(
                length(unity_ObjectToWorld._m00_m10_m20),
                length(unity_ObjectToWorld._m01_m11_m21),
                length(unity_ObjectToWorld._m02_m12_m22)
            );
        }

        struct Input
        {
            float2 uv_MainTex;
        };

        uniform float4 _PlayerPosition;
        float _Radius;

        void vert(inout appdata_full v)
        {
            float radius = 0.003f;
            float3 worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)).xyz - _PlayerPosition.xyz;
            float scaleY = ObjectScale().y;
            v.vertex.y -= ((worldPos.z * worldPos.z * radius) + (worldPos.x * worldPos.x * radius)) / scaleY;
        }

        sampler2D _MainTex;
        float4 _Color;

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color;
        }

        ENDCG
    }
    Fallback "Diffuse"
}