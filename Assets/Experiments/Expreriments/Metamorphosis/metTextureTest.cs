using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class metTextureTest : MonoBehaviour {


	public Metamorphosis m;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Image image = this.GetComponent<Image> ();
		image.sprite = Sprite.Create(m.congruentPointsIndexBuffer, new Rect(0,0,1,m.congruentPointsIndexBuffer.height), Vector2.zero);

		if(Input.GetKeyDown(KeyCode.A)){
			Texture2D tex = m.congruentPointsIndexBuffer;

			foreach(Color pix in tex.GetPixels()){
				Debug.Log (pix);
			}
		}
	}
}
