using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

//http://graphics.zcu.cz/files/REP_2005_Parus_Jindrich.pdf

public class Metamorphosis : MonoBehaviour {


	[SerializeField]
	private GameObject originObject;
	[SerializeField]
	private GameObject targetObject;

	private Mesh originMesh;
	private Mesh targetMesh;

	[SerializeField, Range(0, 1.0f)]
	private float metamorphosity = 0f;

	List<int> congruentPointsIndices;
	public Texture2D originPositionBuffer;
	public Texture2D targetPositionBuffer;
	public Texture2D congruentPointsIndexBuffer;
	RenderTexture interpolatedPositionBuffer;
	Mesh metamorphosedMesh;

	[SerializeField] Shader kernelShader;
	[SerializeField] Shader surfaceShader;
	Material kernelMaterial;
	Material surfaceMaterial;

	[SerializeField]
	ShadowCastingMode _castShadows;
	[SerializeField]
	bool _receiveShadows = false;

	public bool isInitialized = false;

	public void Init(Mesh origMesh, Mesh targMesh){
		//originMesh = originObject.GetComponent<MeshFilter> ().mesh;
		//targetMesh = targetObject.GetComponent<MeshFilter> ().mesh;

		originMesh = origMesh;
		targetMesh = targMesh;

		//recalcMeshes (); //No need

		//originObject.GetComponent<MeshFilter> ().mesh = originMesh;
		//targetObject.GetComponent<MeshFilter> ().mesh = targetMesh;

		originPositionBuffer = createBuffer (originMesh);
		targetPositionBuffer = createBuffer (targetMesh);

		createMetamorphosedMesh ();

		congruentPointsIndices = calcCongruentPoints (); 

		congruentPointsIndexBuffer = createBuffer (congruentPointsIndices);
		interpolatedPositionBuffer = createBuffer (originMesh.vertexCount);

		if (!kernelMaterial) kernelMaterial = CreateMaterial(kernelShader);
		if (!surfaceMaterial)   surfaceMaterial   = CreateMaterial(surfaceShader);

		metamorphosity = 0;	

		isInitialized = true;
	}

	void Update () {

		if (!isInitialized) {
			return;
		}
		
		kernelMaterial.SetTexture ("_OriginPostionBuffer", originPositionBuffer);
		kernelMaterial.SetTexture ("_TargetPostionBuffer", targetPositionBuffer);
		kernelMaterial.SetTexture ("_CongruentPointsIndexBuffer", congruentPointsIndexBuffer);
		kernelMaterial.SetFloat ("_Metamorphosity", metamorphosity);
		kernelMaterial.SetFloat ("_VertexCount", (float)metamorphosedMesh.vertexCount);
		Graphics.Blit (null, interpolatedPositionBuffer, kernelMaterial, 0);

		var m = surfaceMaterial;

		m.SetColor("_Color1", Color.red);
		m.SetColor("_Color2", Color.blue);
		m.SetFloat("_Metallic", 0.5f);
		m.SetFloat("_VertexCount", metamorphosedMesh.vertexCount);

		//m.SetVector("_LineWidth", new Vector2(1 -_lineWidthRandomness, 1) * _lineWidth);
		//m.SetFloat("_Throttle", _throttle);

		m.SetTexture("_PositionTex", interpolatedPositionBuffer);

		var props = new MaterialPropertyBlock();
		var matrix = transform.localToWorldMatrix;

		props.SetFloat("_Flip", 1);
		Graphics.DrawMesh(
			metamorphosedMesh, matrix, surfaceMaterial, gameObject.layer,
			null, 0, props, _castShadows, _receiveShadows
		);
			
		props.SetFloat("_Flip", -1);
		Graphics.DrawMesh(
			metamorphosedMesh, matrix, surfaceMaterial, gameObject.layer,
			null, 0, props, _castShadows, _receiveShadows
		);
	}

	#region MeshModification

	void createMetamorphosedMesh(){
		metamorphosedMesh = new Mesh ();
		metamorphosedMesh.vertices = originMesh.vertices;
		metamorphosedMesh.normals = originMesh.normals;
		metamorphosedMesh.uv = originMesh.uv;
		metamorphosedMesh.triangles = originMesh.triangles;

		List<Vector3> verts = new List<Vector3> (0);
		verts.AddRange (metamorphosedMesh.vertices);

		for(int i = 0; i < metamorphosedMesh.vertexCount; i++){
			verts [i] = new Vector3 ((float)i , 0, 0);	
		}

		metamorphosedMesh.vertices = verts.ToArray ();
	}
		
	#endregion

	Material CreateMaterial(Shader shader)
	{
		var material = new Material(shader);
		material.hideFlags = HideFlags.DontSave;
		return material;
	}

	RenderTexture createBuffer(int size){
		var format = RenderTextureFormat.ARGBFloat;
		var buffer = new RenderTexture(1, size, 0, format);
		buffer.hideFlags = HideFlags.DontSave;
		buffer.filterMode = FilterMode.Point;
		buffer.wrapMode = TextureWrapMode.Clamp;
		return buffer;
	}

	Texture2D createBuffer(Mesh mesh){

		var texture = new Texture2D(1, mesh.vertexCount, TextureFormat.RGBAFloat, false);

		for(int i = 0; i < mesh.vertexCount; i++){

			float x = mesh.vertices [i].x;
			float y = mesh.vertices [i].y;
			float z = mesh.vertices [i].z;

			texture.SetPixel (1, i, new Color(x, y, z));
		}
		texture.Apply ();
		return texture;
	}

	Texture2D createBuffer(List<int> cIndices){
		
		var texture = new Texture2D(1, cIndices.Count, TextureFormat.RGBAFloat, false);

		for(int i = 0; i < cIndices.Count; i++){
			if (cIndices [i] != 0) {
				texture.SetPixel (1, i, new Color((float)cIndices[i], 0, 0, 0));
			} else {
				texture.SetPixel (1, i, new Color(0, 0, 0, 0));
			}
		}
		texture.Apply ();
		return texture;
	}
		
	List<int> calcCongruentPoints(){
		int origPointNum = originMesh.vertexCount;
		int targetPointNum = targetMesh.vertexCount;

		List<int> nearestIndices = new List<int>();

		for(int i = 0; i < origPointNum; i++){
			nearestIndices.Add (i);
			/*
			Vector3 op = originMesh.vertices [i];

			float nearestDistance = float.MaxValue;

			for (int j = 0; j < targetPointNum; j++) {
				if((op - targetMesh.vertices [j]).magnitude < nearestDistance && !nearestIndices.Contains(j)){
					nearestDistance = (op - targetMesh.vertices [j]).magnitude;

					if (nearestIndices.Count != i + 1) {
						nearestIndices.Add (j);	
					} else {
						nearestIndices [i] = j;
					}
				}
			}*/
		}

		Debug.Log(originMesh.triangles.GetLength(0));
		Debug.Log(targetMesh.triangles.GetLength(0));

		Debug.Log(originMesh.vertexCount);
		Debug.Log(targetMesh.vertexCount);

		return nearestIndices;

		//一番近い三角形の検索
//		List<int> nearestTriIndices = new List<int>();
//		int triNum = originMesh.triangles / 3;
//
//		List<Vector3> origTriPos = new List<Vector3> (0);
//		List<Vector3> targTriPos = new List<Vector3> (0);
//
//		for(int i = 0; i < triNum; i++){
//			origTriPos.Add ((originMesh.vertices[i * 3] + originMesh.vertices[i * 3 + 1] + originMesh.vertices[i * 3 + 2]) / 3.0f);
//			targTriPos.Add ((targetMesh.vertices[i * 3] + targetMesh.vertices[i * 3 + 1] + targetMesh.vertices[i * 3 + 2]) / 3.0f);
//		}
//
//		for(int i = 0; i < triNum; i++){
//			Vector3 op = origTriPos[i];
//
//			float nearestDistance = float.MaxValue;
//
//			for(int j = 0; j < triNum; j++){
//				Vector3 tp = targTriPos [j];
//
//				if((op - tp).magnitude < nearestDistance && !nearestTriIndices.Contains(j)){
//					nearestDistance = (op - tp).magnitude;
//
//					if (nearestTriIndices.Count != i + 1) {
//						nearestTriIndices.Add (j);	
//					} else {
//						nearestTriIndices [i] = j;
//					}
//				}	
//			}
//		}

		Debug.Log(originMesh.triangles.GetLength(0));
		Debug.Log(targetMesh.triangles.GetLength(0));
		return nearestIndices;

	}
		
	void recalcMeshes(){

		int origTriNum = originMesh.triangles.GetLength (0) / 3;
		int targTriNum = targetMesh.triangles.GetLength (0) / 3;

		int sub = origTriNum - targTriNum;

		if (sub == 0) {
			return;
		}
		if (sub > 0) {
			targetMesh = subdevideMesh (targetMesh, sub );	
		} else {
			originMesh = subdevideMesh (originMesh, -sub );
		}
	}
	Mesh subdevideMesh(Mesh mesh , int addVertexNum){
		for(int i = 0; i < addVertexNum; i++){
			float randv = Random.value;
			int tindex = (int)((float)mesh.triangles.Length  / 3.0f * randv);
			int v0 = mesh.triangles [tindex * 3];
			int v1 = mesh.triangles [tindex * 3 + 1];
			int v2 = mesh.triangles [tindex * 3 + 2];


			int newIndex = mesh.vertexCount;

			randv = Random.value;
			if (randv > 0.67f) {
				int temp = v2;
				v2 = v1;
				v1 = v0;
				v0 = temp;
			} else if (randv > 0.33f) {
				int temp0 = v2;
				int temp1 = v1;
				v2 = v0;
				v1 = temp0;
				v0 = temp1;
			} else {
			}

			List<Vector3> newVertices = new List<Vector3> (0);
			newVertices.AddRange (mesh.vertices);
			newVertices.Add ((mesh.vertices [v0] + mesh.vertices [v1]) / 2.0f);

			List<Vector3> newNormals = new List<Vector3> (0);
			newNormals.AddRange (mesh.normals);
			newNormals.Add ((mesh.normals [v0] + mesh.normals [v1]) / 2.0f);

			List<Vector2> newUVs = new List<Vector2> (0);
			newUVs.AddRange (mesh.uv);
			newUVs.Add ((mesh.uv [v0] + mesh.uv [v1]) / 2.0f);

			List<int> newTriangles = new List<int> (0);
			newTriangles.AddRange (mesh.triangles);

			newTriangles [tindex * 3] = v0;
			newTriangles [tindex * 3 + 1] = newIndex;
			newTriangles [tindex * 3 + 2] = v2;

			newTriangles.Add (newIndex);
			newTriangles.Add (v1);
			newTriangles.Add (v2);

			mesh.vertices = newVertices.ToArray();
			mesh.normals = newNormals.ToArray();
			mesh.triangles = newTriangles.ToArray();
			mesh.uv = newUVs.ToArray();
		}

		return mesh;
	}
}
