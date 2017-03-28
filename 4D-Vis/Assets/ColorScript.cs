using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorScript : DataPoint {

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
		//Currently sets the color to blue. Should change this whenever
		//float min = 0x00FF00;
		//float max = 0xFF0000;
		//float newVal = (max-min) * t;
		//GetComponent<Renderer> ().material.color = Color.blue;
		GetComponent<Renderer> ().material.color = new Color(t,.5f,0);
	}
}
