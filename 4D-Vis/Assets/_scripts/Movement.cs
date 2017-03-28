using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : DataPoint {
	//private GameObject sp;

	private float t;
	// Use this for initialization
	void Start () {
		//sp = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (Time.deltaTime*t*200 + 1, 0, 0);
	}

	public override void setT(float t) {
		this.t = t;
	}


}
