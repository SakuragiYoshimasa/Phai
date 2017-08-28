Shader "Hidden/Kernel"
{
    Properties
    {
       _PhaseTex ("-", 2D) = ""{}
       _VelocityTex ("-", 2D) = ""{}
       _NaturalFreqTex("-", 2D) = ""{}
       _K           ("-", Float) = 0
       _ParamTheta  ("-", Float) = 0
       _ParamR      ("-", Float) = 0
       _BaseFreq    ("-", Float) = 0
       _DeltaTime   ("-", Float) = 0
       _PointNum    ("-", int) = 0
    }

    CGINCLUDE

    #include "UnityCG.cginc"
    #include "SimplexNoiseGrad3D.cginc"
   
    sampler2D _PhaseTex;
	sampler2D _VelocityTex;
	sampler2D _NaturalFreqTex;

	float _BaseFreq;
	float _ParamTheta;
	float _ParamR;
	float _K;
	float _DeltaTime;

	int _PointNum;


	float nrand(float2 uv, float salt)
    {
        uv += float2(salt, 0);
        return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
    }

	float4 frag_init_phase(v2f_img i) : SV_Target {		

		float v = nrand(i.uv, 10) * _BaseFreq * 2.0;

		return float4(v,0,0,0);
	}


	float4 frag_update_velocity(v2f_img i) : SV_Target {

		float p = tex2D(_PhaseTex, i.uv).x;
		float nf = tex2D(_NaturalFreqTex, i.uv).x;
		float v = nf + _K * _ParamR * sin(_ParamTheta - p);

		return float4(v, 0, 0, 0);
	}

    float4 frag_update_phase(v2f_img i) : SV_Target {

		float p = tex2D(_PhaseTex, i.uv).x;
        float v = tex2D(_VelocityTex, i.uv).x;

        v = p + v * _DeltaTime;

		return float4(v, 0, 0, 0);
    }

    ENDCG

    SubShader
    {
    	Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_img
            #pragma fragment frag_init_phase
            ENDCG
        }


        Pass
        {
        	CGPROGRAM
        	#pragma target 3.0
        	#pragma vertex vert_img
        	#pragma fragment frag_update_velocity
        	ENDCG
  
        }

        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_img
            #pragma fragment frag_update_phase
            ENDCG
        }
    }
}