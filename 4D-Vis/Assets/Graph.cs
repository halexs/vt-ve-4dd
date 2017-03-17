
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Graph : MonoBehaviour {
	
	//Accepts file with lines in the form x,y,z,t
	public string csv_file_path;
	public DataPoint datapoint_prefab;
	public int meh;


	private string file_contents;
	private float[,] data;
	// Use this for initialization
	void Start () {
		file_contents = File.ReadAllText(csv_file_path);

		string[] lines = file_contents.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);
		data = new float[lines.Length,4];
		for (int i = 0; i < lines.Length; i++) {
			string line = lines [i];

			string[] axes_raw = line.Split(new string[] { "," }, System.StringSplitOptions.None);

			for (int j = 0; j < 4; j++) {
				data [i,j] = float.Parse (axes_raw [j]);

			}
		}

		for (int i = 0; i < lines.Length; i++) {
			float x = data [i, 0];
			float y = data [i, 1];
			float z = data [i, 2];
			float t = data [i, 3];

			DataPoint point = GameObject.Instantiate(datapoint_prefab, transform.position, transform.rotation);
			point.setPosition (new Vector3 (x, y, z));
			point.setT (t);
		}
	}
}
