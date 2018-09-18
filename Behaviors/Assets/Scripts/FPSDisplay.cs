using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSDisplay : MonoBehaviour {

	[SerializeField] private TextMeshProUGUI fpsText;

	// Use this for initialization
	void Start () {
		fpsText.text = "FPS: 0.00";
	}
	
	// Update is called once per frame
	void Update () {
		float fps = 1.0f / Time.deltaTime;
		fpsText.text = "FPS: " + Mathf.Ceil(fps);

	}
}
