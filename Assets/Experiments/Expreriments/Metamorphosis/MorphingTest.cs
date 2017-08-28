using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MorphingTest : MonoBehaviour {

	[SerializeField]
	private GameObject LaycastMeshObject;
	[SerializeField]
	private Material mat;
	[SerializeField]
	private GameObject marker;
	[SerializeField]
	private Metamorphosis met;

	[SerializeField, Range(0, 1.0f)]
	private float gamma;

	public GameObject inputSourceMeshObject;
	public GameObject inputTargetMeshObject;

	private Mesh inputSourceMesh;
	private Mesh inputTargetMesh;

	private Mesh sBaseMesh;
	private Mesh tBaseMesh;

	void Start () {
		inputSourceMesh = inputSourceMeshObject.GetComponent<MeshFilter> ().mesh;
		inputTargetMesh = inputTargetMeshObject.GetComponent<MeshFilter> ().mesh;
		GenerateBaseMesh ();
		//CalcShortestPath ();
	}

	void Update () {

		var props = new MaterialPropertyBlock();
		var matrix = transform.localToWorldMatrix;

		Graphics.DrawMesh(
			tBaseMesh, matrix, mat, gameObject.layer,
			null, 0, props, ShadowCastingMode.On, true
		);
		if(Input.GetKeyDown(KeyCode.A)){
			met.Init (sBaseMesh, tBaseMesh);
		}
	}

	void GenerateBaseMesh(){

		List<Vector3> sVerts = new List<Vector3> (0);
		List<Vector3> tVerts = new List<Vector3> (0);

		Mesh LaycastMesh = LaycastMeshObject.GetComponent<MeshFilter> ().mesh;

		Vector3[] castTargetPoints = LaycastMesh.vertices; 

		for (int i = 0; i < castTargetPoints.GetLength (0); i++) {

			Ray ray = new Ray (castTargetPoints [i] * 20.0f, castTargetPoints [i] * -1f);
			RaycastHit[] hits = Physics.RaycastAll (ray, Mathf.Infinity);

			foreach (var obj in hits) {
				if (obj.collider.gameObject.name == inputSourceMeshObject.name) {
					if (sVerts.Count == i) {
						sVerts.Add (obj.point);
					} else {
						if (sVerts [i].magnitude < obj.point.magnitude) {
							sVerts [i] = obj.point;	
						}
					}
				} else if (obj.collider.gameObject.name == inputTargetMeshObject.name) {
					if (tVerts.Count == i) {
						tVerts.Add (obj.point);
					} else {
						if (tVerts [i].magnitude < obj.point.magnitude) {
							tVerts [i] = obj.point;	
						}
					}

				} else {
					Debug.Log ("No Hit Target or Source");
				}
			}
			Debug.DrawRay (ray.origin, ray.direction * 10.0f, Color.red, 5.0f);
		}
					
		sBaseMesh = new Mesh ();
		tBaseMesh = new Mesh ();
		sBaseMesh.vertices = sVerts.ToArray ();
		tBaseMesh.vertices = tVerts.ToArray ();
		sBaseMesh.triangles = LaycastMesh.triangles;
		tBaseMesh.triangles = LaycastMesh.triangles;
		sBaseMesh.RecalculateNormals ();
		tBaseMesh.RecalculateNormals ();
		sBaseMesh.uv = LaycastMesh.uv;
		tBaseMesh.uv = LaycastMesh.uv;
	}

	void CalcShortestPath (){

		int vNum = sBaseMesh.vertexCount;

		float[,] sBaseConnectMatricx = GetConnectionMatrix (sBaseMesh);
		float[,] tBaseConnectMatricx = GetConnectionMatrix (tBaseMesh);

		float[,] sInputConnectMatricx = GetConnectionMatrix (inputSourceMesh);
		float[,] tInputConnectMatricx = GetConnectionMatrix (inputTargetMesh);

		Dictionary<int[], List<int>> sShortestPath = new Dictionary<int[], List<int>> (0); 
		Dictionary<int[], List<int>> tShortestPath = new Dictionary<int[], List<int>> (0); 

		//ORIGINNALは正、ONFACEは負の値を持たせる
		List<Vector3> initialSteinerPoints = new List<Vector3>(0);
		List< List<float> > G0 = preProcessG0ToCalcPath (sInputConnectMatricx, inputSourceMesh.triangles, inputSourceMesh.vertices, gamma, initialSteinerPoints);


		for(int i = 0; i < vNum; i++){
			for(int j = i + 1; j < vNum; j++){
				
				if(!isConnected(sBaseConnectMatricx[i, j])) continue;

				//STEP0 近似メッシュの頂点から最も近い入力メッシュの頂点を算出して用いる
				int startVertexIndex = NearestVertex(inputSourceMesh.vertices, sBaseMesh.vertices[i]);
				int endVertexIndex = NearestVertex(inputSourceMesh.vertices, sBaseMesh.vertices[j]);

				List< List<float> > G = new List<List<float>>(G0);
				List<Vector3> steinerPoints = new List<Vector3> (initialSteinerPoints);

				List<int> GVertIndices = new List<int> (0);
				for(int k = 0; k < inputSourceMesh.vertexCount + steinerPoints.Count; k++){
					GVertIndices.Add (k);
				}

				float pathLength = float.PositiveInfinity;

				//収束するまでループ
				while(true){
					//STEP1 ダイクストラ法でGiの最短経路を算出する
					List<int> shortestPath = Dijkstra(G);
					float updatedLength = 0;

					for(int k = 0; k < shortestPath.Count - 1; k++){
						updatedLength = shortestPath [k] > shortestPath [k + 1] ? G [shortestPath [k]] [shortestPath [k + 1]] : G [shortestPath [k + 1]] [shortestPath [k]]; 
					}

					//STEP1' 継続判断
					if(Mathf.Abs(updatedLength - pathLength) < 0.001) break;

					//STEP2 最短経路上の頂点をGi+1に追加する, ~
					//STEP3 頂点、エッジの付与
					//STEP4 グラフの更新
					G = UpdateGraph(G, steinerPoints, GVertIndices);
				}
			}
		}
	}

	List<int> Dijkstra(List< List<float> > G){
		
		return new List<int>(0);
	}

	List< List<float> > preProcessG0ToCalcPath (float[,] CM, int[] tris, Vector3[] verts,float gamma, List<Vector3> sps){

		int vNum = CM.GetLength (0);
		List< List<float> > g0 = new List<List<float>>(){}; 
		float max = 0; //For standarize

		for(int i = 0; i < vNum; i++){
			g0.Add (new List<float>(0));
			for(int j = 0; j < i; j++){
				g0 [i].Add (CM[i, j]);
				if(max < CM[i, j]){
					max = CM[i, j];
				}
			}	
		}

		//スタイナ点の追加 //始点、終点
		List<int[]> steinerPoints = new List<int[]>(0);

		for(int i = 0; i < g0.Count; i++){
			for (int j = 0; j < i; j++) {
				int steinerPointNum = Mathf.FloorToInt ((CM [i, j] / max) / gamma) - 1;

				if (steinerPointNum < 1)
					continue;

				//とりあえず一個で実装する
				//for(int k = 0; k < steinerPointNum; k++){
				steinerPoints.Add (new int[2]{ i, j });
				g0.Add (new List<float> (0));

				for (int l = 0; l < g0.Count; l++) {
					g0 [l].Add (0);
					sps.Add ((verts[i] + verts[j])/2.0f);
				}
				//}
			}
		}

		//スタイナ点の評価のための面情報
		List<int[]> faces = new List<int[]>(0);
		for(int i = 0; i < tris.GetLength(0)/3; i++){
			List<int> face = new List<int> (3){ tris [i * 3], tris [i * 3 + 1], tris [i * 3 + 2] };
			face.Sort ((a, b) => a - b);
			faces.Add (face.ToArray());
		}

		//スタイナ点を追加した後のエッジの追加
		//1:同じエッジ上で隣接しているエッジを追加する
		//2:同じ面上で異なるエッジ上

		for(int i = vNum; i < vNum + steinerPoints.Count; i++){
			for(int j = 0; j < i; j++){
				
				//今回は一個しかないので同じ面上にあればエッジを追加する
				if (j < vNum) {
					int[] sp0 = steinerPoints[i - vNum];
					if(sp0[0] == j || sp0[1] == j){
						g0 [i] [j] = g0 [i] [j] / 2.0f;
					}
				} else {
					int[] sp0 = steinerPoints[i - vNum];
					int[] sp1 = steinerPoints[j - vNum];

					if(sp0[0] == sp1[0]){
						g0 [i] [j] = -((verts [sp0 [1]] - verts [sp0 [0]]) / 2.0f - (verts [sp1 [1]] - verts [sp1 [0]]) / 2.0f).magnitude;
					}
					else if(sp0[0] == sp1[1]){
						g0 [i] [j] = -((verts [sp0 [1]] - verts [sp0 [0]]) / 2.0f - (verts [sp1 [0]] - verts [sp1 [1]]) / 2.0f).magnitude;
					}
					else if(sp0[1] == sp1[0]){
						g0 [i] [j] = -((verts [sp0 [0]] - verts [sp0 [1]]) / 2.0f - (verts [sp1 [1]] - verts [sp1 [0]]) / 2.0f).magnitude;
					}
					else if(sp0[1] == sp1[1]){
						g0 [i] [j] = -((verts [sp0 [0]] - verts [sp0 [1]]) / 2.0f - (verts [sp1 [0]] - verts [sp1 [1]]) / 2.0f).magnitude;
					}
				}
			}
		}

		return g0;
	}

	List< List<float> > UpdateGraph(List< List<float> > G, List<Vector3> steinerPoints, List<int> GVertIndices){
		return new List<List<float>> (0);
	}

	float[,] GetConnectionMatrix(Mesh mesh){

		int vNum = sBaseMesh.vertexCount;
		int[] tris = mesh.triangles;
		float[,] cM = new float[vNum,vNum];
		Vector3[] verts = mesh.vertices;

		for(int i = 0; i < tris.GetLength(0) / 3; i++){
			if(cM[tris[i * 3], tris[i * 3 + 1]] == 0){
				cM[tris[i * 3], tris[i * 3 + 1]] = (verts[tris[i * 3]] - verts[tris[i * 3 + 1]]).magnitude;
				cM[tris[i * 3 + 1], tris[i * 3]] = (verts[tris[i * 3]] - verts[tris[i * 3 + 1]]).magnitude;
			}
			if(cM[tris[i], tris[i + 2]] == 0){
				cM[tris[i * 3], tris[i * 3 + 2]] = (verts[tris[i * 3]] - verts[tris[i * 3 + 2]]).magnitude;
				cM[tris[i * 3 + 2], tris[i * 3]] = (verts[tris[i * 3]] - verts[tris[i * 3 + 2]]).magnitude;
			}
			if(cM[tris[i * 3 + 1], tris[i * 3 + 2]] == 0){
				cM[tris[i * 3 + 1], tris[i * 3 + 2]] = (verts[tris[i * 3 + 1]] - verts[tris[i * 3 + 2]]).magnitude;
				cM[tris[i * 3 + 2], tris[i * 3 + 1]] = (verts[tris[i * 3 + 1]] - verts[tris[i * 3 + 2]]).magnitude;
			}
		}
		return cM;
	}

	bool isConnected(float val){
		return val != 0;
	}

	Material CreateMaterial(Shader shader)
	{
		var material = new Material(shader);
		material.hideFlags = HideFlags.DontSave;
		return material;
	}

	int NearestVertex(Vector3[] verts, Vector3 v){

		int nearestVertexIndex = 0;
		float dist = float.PositiveInfinity;

		for(int i = 0; i < verts.GetLength(0); i++){
			if((v - verts[i]).magnitude < dist){
				nearestVertexIndex = i;
				dist = (v - verts [i]).magnitude;
			}
		}

		return nearestVertexIndex;
	}
}
