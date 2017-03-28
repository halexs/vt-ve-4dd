using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpacityScript : DataPoint {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}


	/*
	 * 	Currently sets the opacity to anything based on the t value.
	 * Might need to sanitize the inputs to make sure the range is between 0-1
	 * 2 Ways to do this. Have T be HSV, or a slider scale from 0-1. At the moment, attempt slider
	 * scale 0-1
	 * 
	 * Have t value be a hex value, HSV from 0x000000 to 0xFFFFFF
	 * For simplicity sake, we'll only focus on 2 values, have have the t value be a slider
	 * in-between them.
	 * Red: 	0xFF0000
	 * Green: 	0x00FF00
	 */

	public override void setT(float t) {

		//int min = 0x00FF00;
		//int max = 0xFF0000
		gameObject.GetComponent<Renderer> ().material.color = new Color(1.0f, 1.0f, 1.0f, t);
		Debug.Log ("T:" + t);
	}
}
