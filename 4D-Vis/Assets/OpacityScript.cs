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

	public override void setT(float t) {
		//Currently sets the opacity to anything based on the t value.
		//Might need to sanitize the inputs to make sure the range is between 0-1
		gameObject.GetComponent<Renderer> ().material.color = new Color(1.0f, 1.0f, 1.0f, t);

	}
}
