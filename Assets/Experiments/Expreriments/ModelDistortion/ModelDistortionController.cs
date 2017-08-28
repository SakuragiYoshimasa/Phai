using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelDistortionController : MonoBehaviour {

	[SerializeField]
	private List<Material> mats;

	private int matIndex;
	[SerializeField]
	private Material targetMat;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		targetMat.SetFloat ("_Amount", Mathf.Abs( Mathf.Cos(Time.fixedTime * 0.5f)) * 0.02f);
		//targetMat.SetFloat ("_OffsetX", Mathf.Abs( Mathf.Sin(Time.fixedTime * 1.5f * 0.01f)) * 1f);
		//targetMat.SetFloat ("_OffsetY", Mathf.Abs( Mathf.Cos(Time.fixedTime * 1.5f * 0.02f)) * 1f);
		transform.Rotate (new Vector3(0.3f, 0.3f, 0.3f));

	}
}
