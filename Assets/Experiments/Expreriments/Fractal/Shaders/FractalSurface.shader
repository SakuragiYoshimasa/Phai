Shader "Custom/FractalSurface" {
	Properties {
		_Color1 ("Color", Color) = (1,1,1,1)
		_Color2 ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_I ("-", Float) = 0
		_MAX ("-", Float) = 1
		 _PointSize("PointSize", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		float _I;
		float _MAX;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color1;
		fixed4 _Color2;

		void surf (Input IN, inout SurfaceOutputStandard o) {

			float lp =  float(_I) / float(_MAX);
			//o.Albedo = _Color1.rgb * lp + _Color2.rgb;
			o.Albedo = _Color1.rgb;
			//o.Metallic = _Metallic;
			o.Metallic = 0.5;
			
			o.Smoothness = 0.5;
			o.Alpha = _Color1.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}