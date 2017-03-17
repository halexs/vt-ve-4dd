using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Graph : MonoBehaviour {
	//Accepts file with lines in the form x,y,z,t
	public string csv_file_path;
	public GameObject datapoint_prefab;
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
				Debug.Log (data [i,j]);
			}
		}

		for (int i = 0; i < lines.Length; i++) {
			float x = data [i, 0];
			float y = data [i, 1];
			float z = data [i, 2];
			float t = data [i, 3];

			GameObject point = GameObject.Instantiate(datapoint_prefab, transform.position, transform.rotation);
			point.transform.position = new Vector3 (x, y, z);


			/*
			 * Change nothing
			//Scale size by the 4th dimension. Add a new vector to the previous position vector to the localScale
			//point.transform.localScale += new Vector3(t,t,t);
			//To change the length, we can simply change either x, y, or z
			//point.transform.localScale += new Vector3(t,0,0);

			//To change the color, change the r,g, or b values
			//To change transparency, change the opacity value between 0 to 1
			float rvalue, gvalue, bvalue, opacity;
			//Color newColor = new Color (rvalue, gvalue, bvalue, opacity);
			Color newColor = new Color (200f, 123F, 213F, 1);
			MeshRenderer gORenderer = point.GetComponent<MeshRenderer> ();
			Material newMat = new Material (new Shader ());
			newMat.color = newColor;
			gORenderer.material = newMat; */
		}
	}
}
