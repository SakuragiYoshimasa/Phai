Shader "Custom/Surface" {
   	Properties
    {
        _PhaseTex ("-", 2D) = ""{}
   		_NaturalFreqTex("-", 2D) = ""{}

        _Color1 ("-", Color) = (1, 1, 1, 1)
        _Color2 ("-", Color) = (1, 1, 1, 1)

        _Metallic ("-", Float) = 0
        _Smoothness ("-", Float) = 0

        _Radius ("-", Float) = 0
        _PointNum ("-", Float) = 0
    }

    CGINCLUDE

    sampler2D _PhaseTex;
    sampler2D _NaturalFreqTex;
    float4 _PositionTex_TexelSize;
    float4 _NaturalFreqTex_TexelSize;

    half4 _Color1;
    half4 _Color2;

    half _Metallic;
    half _Smoothness;

    float _Radius;
    float _PointNum;

    float _ElapsedTime;
    float _BaseFreq;

    struct Input {
        half color;
    };

    float nrand(float2 uv, float salt)
    {
        uv += float2(salt, 0);
        return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
    }

    void vert(inout appdata_base v, out Input data)
    {
        UNITY_INITIALIZE_OUTPUT(Input, data);

        float2 uv = v.texcoord;
        float p = tex2Dlod(_PhaseTex, float4(uv , 0, 0)).x;
        float theta = _BaseFreq * _ElapsedTime + p;

        v.vertex.xyz = v.vertex.xyz + v.normal * (sin(theta) + 0.3) * (length(v.vertex.xyz)) * _Radius;

        float ln = (sin(theta) + 1.0) / 2.0;
        data.color = ln;
    }
    ENDCG

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM

        #pragma surface surf Standard vertex:vert nolightmap addshadow
        #pragma target 3.0

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = lerp(_Color1, _Color2, IN.color);
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
        }

        ENDCG
    }
}