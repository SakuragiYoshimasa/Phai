using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;



public class TestPatternSceneController : MonoBehaviour {



	#region Definition
	[SerializeField,Range(0,7)]
	private int pattern;

	[SerializeField]
	private Material screenMat;

	[SerializeField]
	private float powerFreq;

	[SerializeField]
	private float freqDifferenceThreshold = 50.0f;

	[SerializeField, Range(0, 1.0f)]
	private float t = 0.2f;

	[SerializeField, Range(0, 1.0f)]
	private float probability = 0.7f;

	[SerializeField]
	private int[] beatsRemain = {0, 0, 0};

	[SerializeField]
	private int beatsTimeWindow = 2;

	[SerializeField]
	private float speed = 0;

	[SerializeField]
	private float blurAmp;
	public MotionBlur motionBlur;

	[SerializeField, Range(2.0f, 8.0f)]
	private float screenDevide = 2.0f;

	[SerializeField]
	private float patternTimer = 0;

	[SerializeField]
	private float patternChangeWaitingTime;

	#endregion

	void Start(){}

	void Update () {

		/*
		if (patternChangeWaitingTime < 0) {

			pattern = 0;
			foreach (var item in DataManager.I.audioModel.GetBeatsArray().Select((v, i) => new {v, i})) {
				if (item.v) beatsRemain [item.i] = beatsTimeWindow;
				if (beatsRemain [item.i] > 0) pattern += 1 << item.i;	
				beatsRemain [item.i] -= 1;
			}
				
			patternChangeWaitingTime = 0.7f;
		} else {
			patternChangeWaitingTime -= Time.deltaTime;
			for(int i = 0; i < 3; i++){
				beatsRemain [i] -= 1;
			}
		}

		float[] freq = DataManager.I.audioModel.GetFftDatas () [DataManager.I.audioModel.GetFftDatas ().Count - 1].spectrumData;

		int maxIdx = freq.Select((val, idx) => new { V = val * Mathf.Exp(Mathf.Sqrt((float)idx * 0.075f)), I = idx }).Aggregate((max, working) => (max.V > working.V) ? max : working).I;
		float pFreq = (float)maxIdx;

		if (Mathf.Abs (pFreq - powerFreq) > freqDifferenceThreshold) {
			powerFreq = (1.0f - t) * powerFreq + t * pFreq;
		} else {
			powerFreq = pFreq;
		}

		//Debug.Log (powerFreq);

		int offset = UnityEngine.Random.value > 0.5f ? -1 : 1;
		offset = UnityEngine.Random.value > probability && offset + pattern >= 0 && offset + pattern <= 7 ? offset : 0;

		//motionBlur.blurAmount = 0.3f * blurAmp;

		screenMat.SetFloat ("_PowerFreq", powerFreq);
		screenMat.SetFloat ("_Speed", speed);

		if(patternTimer < 0){
			screenMat.SetInt ("_Pattern", pattern + offset);
			patternTimer = 0.1f;
		}else{
			patternTimer -= Time.deltaTime;
		}
		screenMat.SetFloat("_ScreenDevide", screenDevide);
		*/
		screenMat.SetFloat("_ScreenDevide", Mathf.Abs(Mathf.Sin(Time.fixedTime )) * 30.0f);
		screenMat.SetInt ("_Pattern", ((int)((float)Time.frameCount * 0.4f)) % 8 );

	}

	void OnEnable(){
		//InputManager.I.OnGetKey_Up += IncreaseDevide;
		//InputManager.I.OnGetKey_Down += DecreaseDevide;
	}

	void OnDisable(){
		//InputManager.I.OnGetKey_Up -= IncreaseDevide;
		//InputManager.I.OnGetKey_Down -= DecreaseDevide;
	}

	void IncreaseDevide(){
		if(screenDevide < 8){
			screenDevide++;
		}
	}

	void DecreaseDevide(){
		if(screenDevide > 0){
			screenDevide--;
		}
	}

	//public override void Enable (){}
	//public override void Disable (){}
}
