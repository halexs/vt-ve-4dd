using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Graph : MonoBehaviour {
	
	//Accepts file with lines in the form x,y,z,t
	public string csv_file_path;
	public DataPoint datapoint_prefab;

	public float xMin;
	public float xMax;
	public float yMin;
	public float yMax;
	public float zMin;
	public float zMax;

	public float tMin = 0;
	public float tMax = 1;

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
		data = normalize (data, lines.Length);


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


	public float[,] normalize(float[,] data, int length ) {

		float[,] normal = new float[length, 4];
		float[] mins = new float[]{ xMin, yMin, zMin, tMin };

		float[] maxes = new float[]{ xMax, yMax, zMax, tMax };


		for (int j = 0; j < 4; j++) {
			float min = float.MaxValue;
			float max = float.MinValue;
			for (int i = 0; i < length; i++) {
				float val = data [i, j];
				if (val < min) {
					min = val;
				}
				if (val > max) {
					max = val;
				}
			}
	
			for (int i = 0; i < length; i++) {
				float val = data [i, j];
				float scaled = (val - min) / (max - min) * (maxes [j] - mins [j]) + mins [j];
				normal [i, j] = scaled;
			}
		}
		return normal;
	}
}

