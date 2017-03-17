using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleScript : DataPoint {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void setT(float t) {
		transform.localScale += (new Vector3 (1.0f, 0f, 0f));
	}
}
