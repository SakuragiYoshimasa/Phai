

	using UnityEngine;
	using System.Collections;
	
	public class GUITest : MonoBehaviour {

	void Start () {

		GameObject obj = GameObject.Find("zippermouth_a_PF");

		if(obj){
			obj.GetComponent<Animation>().Play("idle");
		}

	}
		
//色変え--------------------------------------------------------------
		void OnGUI () {

		// 色変え。(位置XYサイズXY)
			GUI.Box(new Rect(120,10,100,120), "color");
			
			GameObject cola = GameObject.Find("zippermouth_a_PF");
			GameObject colb = GameObject.Find("zippermouth_b_PF");
			GameObject colc = GameObject.Find("zippermouth_c_PF");



		// a表示
			if(GUI.Button (new Rect (130, 40, 80, 20), "01")){
			cola.transform.localScale = new Vector3(1, 1, 1);
			colb.transform.localScale = new Vector3(0, 0, 0);
			colc.transform.localScale = new Vector3(0, 0, 0);
			}
		// b表示
			if (GUI.Button (new Rect (130, 70, 80, 20), "02")) {
			cola.transform.localScale = new Vector3 (0, 0, 0);
			colb.transform.localScale = new Vector3 (1, 1, 1);
			colc.transform.localScale = new Vector3 (0, 0, 0);
			}
		// c表示
			if(GUI.Button (new Rect (130, 100, 80, 20), "03")){
			cola.transform.localScale = new Vector3(0, 0, 0);
			colb.transform.localScale = new Vector3(0, 0, 0);
			colc.transform.localScale = new Vector3(1, 1, 1);
			}

//-------------------------------------------------------------------------


		// バックグラウンド ボックスを作成します。(位置XYサイズXY)
			GUI.Box(new Rect(10,10,100,210), "animation");
			
			GameObject obj = GameObject.Find("zippermouth_a_PF");
			GameObject objb = GameObject.Find("zippermouth_b_PF");
			GameObject objc = GameObject.Find("zippermouth_c_PF");
			
			
			// 1 つ目のボタンを作成します。 押すと、Application.Loadlevel (1) が実行されます。
			if(GUI.Button(new Rect(20,40,80,20), "idle")) {
			obj.GetComponent<Animation>().Play("idle");
			objb.GetComponent<Animation>().Play("idle");
			objc.GetComponent<Animation>().Play("idle");
			}
			
			// 2 つ目のボタンを作成します。
			if(GUI.Button(new Rect(20,70,80,20), "run")) {
			obj.GetComponent<Animation>().Play("run");
			objb.GetComponent<Animation>().Play("run");
			objc.GetComponent<Animation>().Play("run");
			}
				
				// 3 つ目のボタンを作成します。
			if(GUI.Button(new Rect(20,100,80,20), "attack")) {
			obj.GetComponent<Animation>().Play("attack");
			objb.GetComponent<Animation>().Play("attack");
			objc.GetComponent<Animation>().Play("attack");
			}

				// 4
			if(GUI.Button(new Rect(20,130,80,20), "special")) {
			obj.GetComponent<Animation>().Play("special");
			objb.GetComponent<Animation>().Play("special");
			objc.GetComponent<Animation>().Play("special");
			}

				// 5
			if(GUI.Button(new Rect(20,160,80,20), "wound")) {
			obj.GetComponent<Animation>().Play("wound");
			objb.GetComponent<Animation>().Play("wound");
			objc.GetComponent<Animation>().Play("wound");
			}
			
			// 6
			if(GUI.Button(new Rect(20,190,80,20), "deth")) {
			obj.GetComponent<Animation>().Play("deth");
			objb.GetComponent<Animation>().Play("deth");
			objc.GetComponent<Animation>().Play("deth");
			}


		}
	}