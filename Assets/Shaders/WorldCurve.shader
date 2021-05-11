////////////////////////////////////////////////////////////
// Author: Lily Raeburn
// File Name: TestShader.shader
// Description: 
// Date Created: 11/05/2021
// Last Edit: 11/05/2021
// Comments: 
////////////////////////////////////////////////////////////

Shader "Custom/World Curve"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Radius("World Radius", Float) = 1
        _VerticalOffset("Vertical Offset", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

        struct Input
        {
            float2 uv_MainTex;
        };

        uniform float4 _PlayerPosition;
        uniform float _Radius;
        uniform float _VerticalOffset;

        void vert(inout appdata_full v)
        {
            float3 worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz - _PlayerPosition.xyz, 1.0)).xyz;
            v.vertex.y -= (worldPos.z * worldPos.z * _Radius) + (worldPos.x * worldPos.x * _Radius) - _VerticalOffset;
        }

        sampler2D _MainTex;

        void surf(Input IN, inout SurfaceOutput o)
        {
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
        }

        ENDCG
    }
    Fallback "Diffuse"
}