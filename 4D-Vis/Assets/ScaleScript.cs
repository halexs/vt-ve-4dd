using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleScript : DataPoint {
	public float scale = 1f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void setT(float t) {
		t *= scale;
		transform.localScale += (new Vector3 (t, t, t));

	}
}
