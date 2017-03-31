using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MocapCameraController : MonoBehaviour {

	// drag the MocapReceiver object from your scene Heirarchy into this slot in the Inspector for this object
	public MotionCaptureStreamingReceiver mocapReceiver;

	// in the Inspector, type the name of the rigidbody you would like to correspond with this object
	public string rbName;

	private Vector3 positionOffset = new Vector3();

	void Start () {
		// tell the MocapReceiver to call the GetWandPosition method from this script every time it gets new data for the rigidbody named in the first argument
		mocapReceiver.RegisterDelegate (rbName, SetTransform);
	}

	// this is the method that MocapReceiver will call when it gets new data for the rigidbody you named
	public void SetTransform(Vector3 position, Quaternion rotation){
		position += positionOffset;
		transform.localPosition = position - transform.up * 0.14f - transform.forward * 0.07f;
		//transform.rotation = rotation;
	}

	public void SetPositionOffset(Vector3 newOffset){
		positionOffset = newOffset;
	}

	public void ModifyPositionOffset(Vector3 offsetAdjustment){
		positionOffset += offsetAdjustment;
	}
}
