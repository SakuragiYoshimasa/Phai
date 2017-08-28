using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalCameraController : MonoBehaviour {

	public FractalTest fractal;

	public Vector3 posOffset;
	public Vector3 lookAtOffset;

	private float time;

	// Use this for initialization
	void Start () {
		time = 0;
	}
	
	// Update is called once per frame
	void Update () {

		time += Time.deltaTime;

		posOffset = new Vector3 (10f * Mathf.Sin(time), 10f   * Mathf.Cos(time * 2.0f),90f *  Mathf.Cos(time / 6.0f));
		lookAtOffset = new Vector3 (2f * Mathf.Sin(time), 2f   * Mathf.Cos(time), 9f *  Mathf.Sin(time / 12.0f));

		this.transform.position = new Vector3 (0, 0, fractal.getZpos()) + posOffset;
		this.transform.LookAt (new Vector3 (0, 0, fractal.getZpos()) + lookAtOffset, Vector3.up);
	}
}
