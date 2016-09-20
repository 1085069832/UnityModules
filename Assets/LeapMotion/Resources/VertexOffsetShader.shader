﻿Shader "LeapMotion/VertexOffsetShader" {
	Properties{
		_Color("Color", Color) = (1, 1, 1, 1)
		_Amount("Extrusion Amount", Range(-1, 1)) = 0.5
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0, 1)) = 0.5
			_Metallic("Metallic", Range(0, 1)) = 0.0
	}
	SubShader{
			Tags{ "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows vertex:vert

			// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

			uniform float4x4 _rightHandPos;
			uniform float4x4 _rightHandRot;
			void vert(inout appdata_full v) {
				v.vertex = mul(unity_WorldToObject, mul(_rightHandPos, mul(unity_ObjectToWorld, mul(_rightHandRot, v.vertex))));
			}

			sampler2D _MainTex;

			struct Input {
				float2 uv_MainTex;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

			void surf(Input IN, inout SurfaceOutputStandard o) {
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
			}
			ENDCG
		}
		FallBack "Diffuse"
}