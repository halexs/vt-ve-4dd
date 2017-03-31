using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(MotionCaptureStreamingReceiver))]
//[CanEditMultipleObjects]
public class MotionCaptureStreamingReceiverEditor : Editor{

	/* this version WORKS, but uses EditorUtility.SetDirty which is set to be deprecated */
	MotionCaptureStreamingReceiver script;

	void OnEnable(){
		// get reference to the script whose inspector we are editing
		script = (MotionCaptureStreamingReceiver)target;
	}

	public override void OnInspectorGUI(){
		// define some styles
		GUIStyle headingFont = new GUIStyle(EditorStyles.boldLabel);
		headingFont.fontSize = 14;
		headingFont.fontStyle = FontStyle.Bold;
		headingFont.alignment = TextAnchor.MiddleCenter;

		// show the script field itself in the inspector
		MonoScript targetscript = MonoScript.FromMonoBehaviour ((MotionCaptureStreamingReceiver)target);
		targetscript = EditorGUILayout.ObjectField ("Script:", targetscript, typeof(MonoScript), false) as MonoScript;

		// this section lets the user choose which mocap server, and shows the public variables for each kind of server
		GUILayout.Space(10);
		GUILayout.Label ("Motion Capture Server", headingFont);
		script.mocapServer = (MocapServer)EditorGUILayout.EnumPopup ((MocapServer)script.mocapServer);
		GUILayout.Space(5);

		if (script.mocapServer == MocapServer.Perform_OptitrackMotiveBody) {
			script.OMBmulticastIP = EditorGUILayout.TextField ("multicastIP", script.OMBmulticastIP);
			script.OMBhostIP = EditorGUILayout.TextField ("hostIP", script.OMBhostIP);
			script.OMBdataPort = EditorGUILayout.IntField ("dataPort", script.OMBdataPort);
			script.OMBcommandPort = EditorGUILayout.IntField ("commandPort", script.OMBcommandPort);
			script.OMBNatNetVersion = EditorGUILayout.TextField ("NatNetVersion", script.OMBNatNetVersion);
		}
		else if(script.mocapServer == MocapServer.Cube_QualisysTrackManager){
			script.QTMserverIP = EditorGUILayout.TextField ("QTMserverIP", script.QTMserverIP);
			script.QTMlocalPort = EditorGUILayout.IntField ("QTMlocalPort", script.QTMlocalPort);
			script.QTMserverPort = EditorGUILayout.IntField ("QTMserverPort", script.QTMserverPort);
		}

		GUILayout.Space(20);
		GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
		GUILayout.Label ("Transform", headingFont);
		script.scaleX = EditorGUILayout.FloatField ("scaleX", script.scaleX);
		script.scaleY = EditorGUILayout.FloatField ("scaleY", script.scaleY);
		script.scaleZ = EditorGUILayout.FloatField ("scaleZ", script.scaleZ);
		GUILayout.Space(10);
		script.offsetX = EditorGUILayout.FloatField ("offsetX", script.offsetX);
		script.offsetY = EditorGUILayout.FloatField ("offsetY", script.offsetY);
		script.offsetZ = EditorGUILayout.FloatField ("offsetZ", script.offsetZ);
		GUILayout.Space(10);
		script.bSmoothCamera = EditorGUILayout.Toggle ("bSmoothCamera", script.bSmoothCamera);

		//EditorUtility.SetDirty (script);
		//EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene());
        Undo.RecordObject(target, "MotionCaptureStreamingReceiver Change");
        
	}




	/*  this method uses serialization */
	/*
	MotionCaptureStreamingReceiver script;
	SerializedProperty mocapServer;
	SerializedProperty scaleX;

	void OnEnable(){
		// get reference to the script whose inspector we are editing
		MotionCaptureStreamingReceiver script = (MotionCaptureStreamingReceiver)target;

		mocapServer = serializedObject.FindProperty ("mocapServer");
		scaleX = serializedObject.FindProperty ("scaleX");

	}


	public override void OnInspectorGUI(){

		serializedObject.Update ();
		EditorGUILayout.PropertyField (mocapServer);

		if (mocapServer.enumValueIndex ==  0) {
			EditorGUILayout.PropertyField (scaleX);
		}
		serializedObject.ApplyModifiedProperties ();
	}*/
}

