using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ExperimentUtilities;

public class ModelSimulation : MonoBehaviour {

	[SerializeField, Range(1, 4096)]
	private int _pointNum; //振動子の数
	public int pointNum {
		get { return _pointNum;}
		private set { _pointNum = value;}
	}

	[SerializeField]
	private float _baseFreq;
	public float baseFreq {
		get { return _baseFreq;}
		private set { _baseFreq = value;}
	}

	[SerializeField]
	private float _connectionCoefficient;
	public float connectionCoefficient {
		get { return _connectionCoefficient;}
		private set { _connectionCoefficient = value;}
	}

	[SerializeField] Shader kernelShader;
	[SerializeField] Shader surfaceShader;
	[SerializeField] private Mesh mesh;

	private Material kernelMat;
	private Material surfaceMat;

	private Texture2D naturalFreqBuffer;
	private RenderTexture phaseBuffer;
	private RenderTexture velocityBuffer;


	[SerializeField] private Color _color1;
	public Color color1 {
		get{ return _color1;}
		set{ _color1 = value;}
	}

	[SerializeField] private Color _color2;
	public Color color2 {
		get{ return _color2;}
		set{ _color2 = value;}
	}

	[SerializeField, Range(0f, 1.0f)] private float _metallic = 0.5f;
	public float metallic {
		get{ return _metallic;}
		set{ _metallic = value;}
	}

	[SerializeField, Range(0f, 1.0f)] private float _smoothness = 0.5f;
	public float smoothness {
		get{ return _smoothness;}
		set{ _smoothness = value;}
	}

	[SerializeField, Range(0.5f, 2.0f)] private float _radius = 1.0f;
	public float radius {
		get{ return _radius;}
		set{ _radius = value;}
	}
		
	private float elapsedTime;

	void Start () {
		Initialize ();
	}	

	void Update () {
		
		kernelMat.SetFloat ("_DeltaTime", Time.deltaTime);
		kernelMat.SetFloat ("_K", connectionCoefficient);
		kernelMat.SetFloat ("_BaseFreq", baseFreq);

		UpdateVelocity ();
		UpdatePhase ();

		elapsedTime += Time.deltaTime;
		DrawMesh ();
	}

	void Initialize(){

		naturalFreqBuffer = Buffer.CreateT2Buffer (pointNum);
		phaseBuffer = Buffer.CreateRTBuffer (pointNum);
		velocityBuffer = Buffer.CreateRTBuffer (pointNum);

		InitializeMat ();
		InitializePosition (); 
		InitializeNaturalFreqs (); 

		elapsedTime = 0f;
		int vNum = mesh.vertexCount;

		Vector2[] uv = new Vector2[vNum];

		for(int i = 0; i < vNum; i++){
			uv [i] = new Vector2 (((float)i + 0.5f)/ (float)vNum, 0.5f);
		}

		mesh.uv = uv;
	}

	void InitializeMat(){

		surfaceMat = MaterialFuncs.CreateMaterial (surfaceShader);
		kernelMat = MaterialFuncs.CreateMaterial (kernelShader);
		kernelMat.SetInt ("_PointNum", pointNum);
		kernelMat.SetFloat ("_K", connectionCoefficient);
		kernelMat.SetFloat ("_BaseFreq", baseFreq);
		kernelMat.SetFloat ("_DeltaTime", 0);
	}

	void InitializePosition(){
		
		Graphics.Blit (null, phaseBuffer, kernelMat, 0);
		kernelMat.SetTexture ("_PhaseTex", phaseBuffer);
	}

	void InitializeNaturalFreqs(){

		RandomBoxMuller random = new RandomBoxMuller();

		for(int i = 0; i < pointNum; i++){
			naturalFreqBuffer.SetPixel (1, i, new Color(((float)random.next(0, 2.0, true) - 0.5f) * baseFreq , 0, 0, 0));
			//naturalFreqBuffer.SetPixel (1, i, new Color((Random.value - 0.5f) * baseFreq , 0, 0, 0));
		}
		naturalFreqBuffer.Apply ();

		kernelMat.SetTexture ("_NaturalFreqTex", naturalFreqBuffer);
		surfaceMat.SetTexture ("_NaturalFreqTex", naturalFreqBuffer);
	}
					
	void UpdateVelocity(){
		Graphics.Blit (null, velocityBuffer, kernelMat, 1);

		kernelMat.SetTexture ("_VelocityTex", velocityBuffer);
	}

	void UpdatePhase(){
		
		Graphics.Blit (null, phaseBuffer, kernelMat, 2);

		float[] param = calcParams ();

		kernelMat.SetTexture ("_PhaseTex", phaseBuffer);
		kernelMat.SetFloat ("_ParamTheta", param[0]);
		kernelMat.SetFloat ("_ParamR", param[1]);
	}

	void DrawMesh(){
		
		surfaceMat.SetColor ("_Color1", color1);
		surfaceMat.SetColor ("_Color2", color2);
		surfaceMat.SetTexture ("_PhaseTex", phaseBuffer);
		surfaceMat.SetFloat ("_Metallic", metallic);
		surfaceMat.SetFloat ("_Smoothness", smoothness);
		surfaceMat.SetFloat ("_Radius", radius);
		surfaceMat.SetFloat ("_ElapsedTime", elapsedTime);
		surfaceMat.SetFloat ("_BaseFreq", baseFreq);

		Graphics.DrawMesh (mesh, transform.localToWorldMatrix, surfaceMat, gameObject.layer);
	}

	float[] calcParams(){

		Texture2D tex = new Texture2D(phaseBuffer.width, phaseBuffer.height, TextureFormat.RGBAFloat, false);

		RenderTexture.active = phaseBuffer;
		tex.ReadPixels(new Rect(0, 0, phaseBuffer.width, phaseBuffer.height), 0, 0);
		tex.Apply();

		float real = 0;
		float imag = 0;

		for(int i = 0; i < pointNum; i++){
			float phai = tex.GetPixel (i, 0).r;
			real += Mathf.Cos (phai);
			imag += Mathf.Sin (phai);
		}

		real /= (float)pointNum;
		imag /= (float)pointNum;

		float paramTheta = Mathf.Atan (imag/real);
		float paramR = Mathf.Sqrt (real * real + imag * imag);

		radius = paramR * 0.7f + 0.5f;

		return new float[2]{paramTheta, paramR};
	}
}
