using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
	//private GameObject sp;
	// Use this for initialization
	void Start () {
		//sp = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (Time.deltaTime*30, 0, 0);
	}

}
