using UnityEngine;
using System.Collections;

public class RigidBodyExample : MonoBehaviour {

	// drag the MocapReceiver object from your scene Heirarchy into this slot in the Inspector for this object
	public MotionCaptureStreamingReceiver mocapReceiver;

	// in the Inspector, type the name of the rigidbody you would like to correspond with this object
	public string rbName;

	void Start () {
		// tell the MocapReceiver to call the GetWandPosition method from this script every time it gets new data for the rigidbody named in the first argument
		mocapReceiver.RegisterDelegate (rbName, GetWandPosition);
	}

	// this is the method that MocapReceiver will call when it gets new data for the rigidbody you named
	public void GetWandPosition(Vector3 position, Quaternion rotation){
		transform.position = new Vector3(position.x, position.y + 0.0f, position.z);
		transform.rotation = rotation;
	}
}
