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

	public override void setT(float t) {
		//Currently sets the color to blue. Should change this whenever
		GetComponent<Renderer> ().material.color = Color.blue;
	}
}
