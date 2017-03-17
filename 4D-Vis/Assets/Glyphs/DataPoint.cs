using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataPoint : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setPosition (Vector3 position) {
		transform.position = position;
	}

	public abstract void setT(float t);
}
