using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExperimentUtilities;


public class FractalTest : MonoBehaviour {
	
	Complex A;
	Complex B;
	Complex C;
	Complex D;

	[SerializeField] int MaxMesh;
	[SerializeField] int maxIter;
	[SerializeField] Shader shader;

	[SerializeField] List<Material> mats;
	[SerializeField] List<Mesh> meshes;

	float time;
	int[] triangles;
	int[] indices;

	[SerializeField] Color color1;
	[SerializeField] Color color2;

	public float getZpos(){
		return -time * 4000.0f;
	}

	Complex f0(Complex z){
		return A * z + B * z.Conj;
	}

	Complex f1(Complex z){
		return C * (z - 1.0f) + D * (z.Conj - 1.0f) + 1.0f;
	}
		
	void Start () {
		
		time = 0;

		triangles = new int[3 * (int)(Mathf.Pow(2.0f, maxIter))];

		for(int i = 0; i < maxIter - 1; i++){
			
			for(int j = 0; j <= (int)Mathf.Pow(2.0f, (float)i) - 1; j++){

				triangles [((int)Mathf.Pow (2.0f, (float)i) - 1) * 3]     = (int)Mathf.Pow (2.0f, (float)i) - 1 + j;
				triangles [((int)Mathf.Pow (2.0f, (float)i) - 1) * 3 + 1] = (int)Mathf.Pow (2.0f, (float)(i + 1)) - 1 + 2 * j;
				triangles [((int)Mathf.Pow (2.0f, (float)i) - 1) * 3 + 2] = (int)Mathf.Pow (2.0f, (float)(i + 1)) - 1 + 2 * j + 1;
			}
		}

		meshes = new List<Mesh> (0);

		mats = new List<Material> (0);

		for(int i = 0; i < MaxMesh; i++){
			mats.Add (MaterialFuncs.CreateMaterial(shader));
			mats[i].SetFloat ("_I", i);
			mats[i].SetFloat ("_MAX", maxIter);
			mats[i].SetFloat ("_Metallic", 0.5f);
			mats[i].SetFloat ("_Glossiness", 0.5f);
			mats[i].SetFloat ("_PointSize", 1000.0f);
			mats[i].SetColor ("_Color1", color1);
			mats[i].SetColor ("_Color2", color2);
		}
	}
				
	void Update () {
		time += Time.deltaTime / 100.0f;

		A = new Complex (
			Mathf.Sin(time * 2.3f * Mathf.Cos(time) + 2.3f * Mathf.Cos(time)), 
			Mathf.Cos(time * 2.3f * Mathf.Cos(time) + 10.0f * Mathf.Sin(time))
		);

		B = new Complex (
			Mathf.Sin(time * 3.6f + 11.5f * Mathf.Cos(time) * Mathf.Sin(time)),
			Mathf.Cos(time * 3.5f * 2.3f  * Mathf.Cos(time) + 50.0f * Mathf.Cos(time))
		);

		C = new Complex (
			Mathf.Sin(time * 5.0f + 27.0f * Mathf.Sin(time * 2.3f * Mathf.Cos(time))),
			Mathf.Cos(time * 2.3f * 5.4f  * Mathf.Cos(time) + 3.0f * Mathf.Sin(time))		
		);

		D = new Complex (
			Mathf.Sin(time * 6.7f + 17.0f * Mathf.Sin(time * 2.3f * Mathf.Cos(time))),
			Mathf.Cos(time * 7.1f + 10.0f * Mathf.Cos(time) * 2.3f * Mathf.Cos(time))				
		);

		if(meshes.Count >= MaxMesh){
			meshes.RemoveAt (0);
		}

		List<Vector3> vertices = new List<Vector3> (0);
		List<Complex> cSpace = new List<Complex> (0);

		cSpace.Add (new Complex(0, 0));
		vertices.Add (cSpace[0].getPosition(getZpos(), 2.0f));

		for(int i = 0; i < maxIter; i++){
			List<Complex> tempCPace = new List<Complex> (0);

			for(int k = 0; k < cSpace.Count; k++){
				tempCPace.Add (f0(cSpace[k]));
				tempCPace.Add (f1(cSpace[k]));

				vertices.Add (f0(cSpace[k]).getPosition(getZpos(), 2.0f));
				vertices.Add (f1(cSpace[k]).getPosition(getZpos(), 2.0f));
				
			}

			cSpace = tempCPace;
		}

		Mesh newMesh = new Mesh ();

		newMesh.vertices = vertices.ToArray ();
		newMesh.triangles = triangles;
		newMesh.RecalculateNormals ();

		if(indices == null){
			indices = new int[newMesh.vertexCount];

			for(int i = 0; i < newMesh.vertexCount; i++){
				indices [i] = i;
			}
		}
			
		newMesh.SetIndices (indices, MeshTopology.Points, 0);
		meshes.Add (newMesh);

		for(int i = 0; i < meshes.Count; i++){
			mats[i].SetColor ("_Color1", color1);
			mats[i].SetColor ("_Color2", color2);
		
			Graphics.DrawMesh (meshes[i], transform.localToWorldMatrix, mats[i], this.gameObject.layer);
		}
	}
}
