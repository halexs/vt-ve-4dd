using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct RigidBodyGeneric{
	//public string name;
	//public Vector3 position;
	//public Quaternion rotation;

	private string nameVal;
	private Vector3 positionVal;
	private Quaternion rotationVal;

	public string name{
		get{
			return nameVal;
		}
		set{
			nameVal = value;
		}
	}

	public Vector3 position{
		get{
			return positionVal;
		}
		set{
			positionVal = value;
		}
	}

	public Quaternion rotation{
		get{
			return rotationVal;
		}
		set{
			rotationVal = value;
		}
	}
}
	
public enum MocapServer {
	Perform_OptitrackMotiveBody, Cube_QualisysTrackManager
}

public delegate void RegisteredDelegate(Vector3 position, Quaternion rotation);

public class MotionCaptureStreamingReceiver : MonoBehaviour {

	// The server type from which we will receive data - enum - either Perform_OptitrackMotiveBody or Cube_QualisysTrackManager
	public MocapServer mocapServer = MocapServer.Perform_OptitrackMotiveBody;

    // IP address of the motion capture server
    public string mocapServerIP = "127.0.0.1";

    /* QTM */
	// the IP address of the QTM server
	public string QTMserverIP = "192.168.1.154";
    // OSC port of the QTM server; default is 22225
    public int QTMserverPort = 22225;
    // Port used to receive data on local machine from QTM
    public int QTMlocalPort = 22289;

    /* OMB */
	/* these can all be copied from the OMB streaming panel in OMB */
	// the IP address on which OMB is doing multicast
	public string OMBmulticastIP = "239.255.42.99";
	// the IP address of the OMB server
	public string OMBhostIP = "128.173.21.168";
	public int OMBdataPort = 1511;
	public int OMBcommandPort = 1510;
	// the current version of nat net... note this is NOT the current version of OMB
	public string OMBNatNetVersion = "2.9.0.0";

	/* the following variables allow for scaling, offsetting, and rotating the values coming from the motion capture server.
	 * note that these manipulations will affect the incoming information for ALL rigidbodies.
	 * the purpose of these is not to position specific rigidbodies in specific places, (that should be done in the controlling script for the specific rigid body)
	 * use these if all the data from QTM needs to be offset or scaled, in a global sense.  for example, if the scale of your world is much larger than the scale of the QTM coordinates, meaning all of the rigid bodies would need to correspond to the larger scale, this would be an appropriate place to take care of that
	 */
	//Scales(divides) each QTM position value. 1000 means scale mm to meters.
	public float scaleX = 1.0f;
	public float scaleY = 1.0f;
	public float scaleZ = 1.0f;

	//Y value of Unity terrain to offset the QTM Y value by.
	public float offsetX = 0.0f;
	public float offsetY = 0.0f;
	public float offsetZ = 0.0f;

	// smooth out the motion of the camera to reduce motion capture irregularities
	public  bool bSmoothCamera = false;

	// holds instance of OMBInterface, which is constructed in Start
	private OMBInterface OMBInterface;
	// holds instance of QTMInterface, which is constructed in Start
	private QTMInterface QTMInterface;

	// a dictionary of the delegates that are registered when RegisterDelegate is called.
	// the delegates are indexed by the name of a rigid body
	// when information about that rigid body is received, the delegates are called, passing info on the rigid body to the delegates
	Dictionary<string, List<RegisteredDelegate>> registeredDelegatesByName = new Dictionary<string, List<RegisteredDelegate>>();

	// used to keep track of another instance of this script
	static MotionCaptureStreamingReceiver _instance;

    void Awake() {
        if (_instance == null) {
            //If I am the first instance, keep track of me
            _instance = this;
            //DontDestroyOnLoad(this);
        }
        else {
            //If a Singleton already exists and you find another reference in scene, destroy it.
            if (this != _instance) {
                Debug.Log("ERROR!!!!  You have more than one instance of the Motion Capture Streaming Receiver in your scene.  There can be only one.  Anything that happens from this point on may be weird.");
                //Destroy(this.gameObject);
            }
        }
    }

	// Use this for initialization
	void Start () {
		Debug.Log ("mocapServer: " + mocapServer);
		OMBInterface = new OMBInterface(OMBmulticastIP, OMBhostIP, OMBdataPort, OMBcommandPort, OMBNatNetVersion);
		OMBInterface.Initialize ();
		QTMInterface = new QTMInterface (QTMserverIP, QTMserverPort, QTMlocalPort);
		QTMInterface.Initialize ();
	}
		
	
	// Update is called once per frame
	void Update () {
		List<RigidBodyGeneric> rbs = new List<RigidBodyGeneric>();

        if (mocapServer == MocapServer.Cube_QualisysTrackManager) {
			QTMInterface.Report(ref rbs);
        }
		else if (mocapServer == MocapServer.Perform_OptitrackMotiveBody) {
			OMBInterface.Update();
			OMBInterface.Report(ref rbs);
        }

		// iterate over the rigid bodies to adjust transform
		for (int i = 0; i < rbs.Count; i++) {
			Vector3 pos = new Vector3 ();
			pos = rbs [i].position;
			pos.x /= scaleX;
			pos.x += offsetX;
			pos.y /= scaleY;
			pos.y += offsetY;
			pos.z /= scaleZ;
			pos.z += offsetZ;
			RigidBodyGeneric rb = new RigidBodyGeneric ();
			rb.position = pos;
			rb.rotation = rbs [i].rotation;
			rb.name = rbs [i].name;
			rbs [i] = rb;
		}
			
		// iterate over the incoming rigid bodies.  if a Unity user has registered a delegate to associate with the name of the incoming rigid body, call that delegate and pass it the established parameters
		//Debug.Log("rbs.Count: " + rbs.Count);
		for (int i = 0; i < rbs.Count; i++) {
			List<RegisteredDelegate> delegates;
			if (registeredDelegatesByName.TryGetValue (rbs [i].name, out delegates)) {
				for (int delegateCounter = 0; delegateCounter < delegates.Count; delegateCounter++) {
					delegates [delegateCounter] (rbs [i].position, rbs[i].rotation);
				}
			}
		}
			
	}
		

	public void RegisterDelegate(string name, RegisteredDelegate inDelegate){
		// if the name is alerady registered, add this delegate to the array of delegates associated with that name
		if (registeredDelegatesByName.ContainsKey (name)) {
			List<RegisteredDelegate> associatedDelegates;
			if (registeredDelegatesByName.TryGetValue (name, out associatedDelegates)) {
				associatedDelegates.Add (inDelegate);
				registeredDelegatesByName [name] = associatedDelegates;
			}
		}
		// if the name is not already registered, make a new key with it, an add the associated delegate
		else {
			List<RegisteredDelegate> associatedDelegates = new List<RegisteredDelegate> ();
			associatedDelegates.Add (inDelegate);
			registeredDelegatesByName.Add (name, associatedDelegates);
		}
	}

	public void OnDestroy(){
		OMBInterface.OnDestroy ();
	}

	public void OnApplicationQuit(){
		OMBInterface.OnApplicationQuit ();
	}
}

