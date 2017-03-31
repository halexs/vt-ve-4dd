// Adapted from UDPReceiver.cs for OSC.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Bespoke.Common;
using Bespoke.Common.Osc;

public class QTMInterface
{
	//IP address of the QTM server.
     private string serverIP = "192.168.1.154";
	
	//OSC port of the QTM server; default is 22225.
     private int serverPort = 22225;
	
	//Port used to receive info on your local machine.
     private int localPort = 22289;
	
	//Scales(divides) each QTM position value. 1000 means scale mm to meters.
	 private float scaleX = 1000.0f;
     private float scaleY = 1000.0f;
     private float scaleZ = 1000.0f;
	
	//Y value of Unity terrain to offset the QTM Y value by.
     private float offsetX = 0.0f;
     private float offsetY = 0.0f;
     private float offsetZ = 0.0f;

	// for motion smoothing
	public  bool bSmoothCamera = false;
	private  Vector3 virtualPosition = new Vector3(0,0,0);
	private  Quaternion virtualRotation = new Quaternion(0,0,0,0);

	//------------------------------------------------------------------------
	//List of rigid body names to acknowledge from QTM.
	//public string[] rigidBodyNames = new string[6];
	
	//Game objects manipulated by acknowledged rigid body info.  
	public  GameObject[] positionTargets = new GameObject[6];
	public  GameObject[] orientationTargets = new GameObject[6];
	
	/* Example: For the rigid body "Hard Hat" in QTM to be your Main Camera:
	 * 		rigidBodyNames[0] = "Hard Hat"
	 * 		positionTargets[0] = Main Camera
	 * 		orientationTargets[0] = Main Camera
	 * -Note that every index is the same. 
	 * -Values are set in Unity, where this script is a GameObject's component.
	*/
	//------------------------------------------------------------------------


     Thread streamingThread; //Named "workingThread" in UDPReceiver.cs
     UdpClient streamingClient; //Named "streamingClient" in UDPReceiver.cs
     byte[] bytePacket; //Named "bytePacket" in UDPReceiver.cs
     volatile bool dataReceived;
     IPEndPoint receivedEndPoint;
	
	//Used to resend connect command to QTM server until a reply is received.
     bool connected;
     int frameCounter;
	
	//Contains formatted rigidBodyNames[] for comparisons and index look up.
	 List<string> multipleBodyNames = new List<string>();
	
	 private OscMessage qtmCommand;
     IPEndPoint localEndPoint;
     IPEndPoint serverEndPoint;

	public QTMInterface(string _serverIP, int _serverPort, int _localPort){
		serverIP = _serverIP;
		serverPort = _serverPort;
		localPort = _localPort;
	}

	public  void Initialize ()
	{	
		connected = false;
		
		streamingThread = new Thread (new ThreadStart (ReceiveData));
		streamingThread.IsBackground = true;
		streamingThread.Start ();
		
		localEndPoint = new IPEndPoint(IPAddress.Loopback, localPort);
		serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
		qtmCommand = new OscMessage (localEndPoint, "/qtm");
		OscPacket.UdpClient = new UdpClient();
	}
	
	// Update is called once per frame
	public  void Report (ref List<RigidBodyGeneric> rbs)
	{
		if (!connected && frameCounter + 60 < Time.frameCount)
		{
			SendCommand ("Connect " + localPort);
			frameCounter = Time.frameCount;
		}
		if (dataReceived)
		{
			dataReceived = false;
            ParsePacket(OscPacket.FromByteArray(receivedEndPoint, bytePacket), ref rbs);
		}
	}
	
	// Use this for exiting
	void OnApplicationQuit ()
	{
		try
		{
			SendCommand ("StreamFrames Stop");
			SendCommand ("Disconnect");
			if (streamingClient.Available == 1)
			{
				streamingClient.Close ();
			}
			if (streamingThread != null)
			{
				streamingThread.Abort ();
			}
		} catch (Exception e)
		{
			Debug.Log (e.Message);
		}
	}
	
	// Receive thread
     private void ReceiveData()
	{
		streamingClient = new UdpClient (localPort);
		streamingClient.Client.ReceiveTimeout = 500;
		while (true)
		{
			try
			{
				IPEndPoint anyIP = new IPEndPoint (IPAddress.Any, 0);
				bytePacket = streamingClient.Receive (ref anyIP);
				receivedEndPoint = anyIP;
				dataReceived = true;
			}
			catch (Exception err)
			{
				SocketException sockErr = (SocketException)err;
				if (sockErr.ErrorCode != 10060)
				{
					Debug.Log ("Error receiving packet: " + sockErr.ToString ());
				}
			}
		}
	}
	
	// Send QTM OSC Command to server
	private  void SendCommand (string cmd)
	{
		qtmCommand.ClearData ();
		qtmCommand.Append (cmd);
		LogMessage (qtmCommand);
		qtmCommand.Send (serverEndPoint);
	}
	
	// Process Data Frame OscBundle
	private  void ParsePacket(OscPacket packet, ref List<RigidBodyGeneric> rbs) {
		if(packet.IsBundle)
		{
			//OSC Data frame packet; defined in section 6.8 of the QTM RT Server Protocol Documentation.
			foreach(OscMessage message in ((OscBundle)packet).Messages)
			{
				//LogMessage (message);
				// make sure the message is containing a 6DOF RB
				string[] messageParts = message.Address.Split ('/');
				if (messageParts[2] == "6d") {
					Vector3 position = new Vector3 ();
					Quaternion rotation = new Quaternion ();
					ExtractPositionAndRotation (message, ref position, ref rotation);
					RigidBodyGeneric rb = new RigidBodyGeneric ();
					rb.name = messageParts [3];
					rb.position = position;
					rb.rotation = rotation;
					rbs.Add (rb);
				}

				/*
				//Check if this rigid body name was given in rbNames[]
                for (int i = 0; i < rbNames.Length; i++) {
                    if (rbNames[i] == message.Address) {
                        Vector3 position = new Vector3();
                        Quaternion rotation = new Quaternion();
                        ExtractPositionAndRotation(message, ref position, ref rotation);
                        positionsReport[i] = position;
                        rotationsReport[i] = rotation;
                    }
                }
                */
			}
		}
		else
		{
			LogMessage((OscMessage)packet);
			if (String.Compare(((OscMessage)packet).Address, "/qtm/cmd_res") == 0)
			{
				//OSC Command response packet; defined in section 6.6 of the QTM RT Server Protocol Documentation.
				connected = true;
				//Upon receiving response from server, start streaming data frames.
				SendCommand ("StreamFrames AllFrames 6D");
			}
		}
	}
	
	//OSC equivalent of "byteParser(byte[] data)" in UDPReceiver.cs
	private  void ExtractPositionAndRotation(OscMessage message, ref Vector3 position, ref Quaternion rotation)
	{
		float[] matrix = new float[9];
		matrix[0] = (float)message.Data[3];
		matrix[1] = (float)message.Data[4];
		matrix[2] = (float)message.Data[5];
		matrix[3] = (float)message.Data[6];
		matrix[4] = (float)message.Data[7];
		matrix[5] = (float)message.Data[8];
		matrix[6] = (float)message.Data[9];
		matrix[7] = (float)message.Data[10];
		matrix[8] = (float)message.Data[11];
		
		
		Vector3 pos = new Vector3 ((float)message.Data[0], (float)message.Data[1], (float)message.Data[2]);
		Quaternion rot = QuaternionFromMatrix(matrix);

		// convert incoming mm units to meter units for Unity
		pos.x /= 1000;
		pos.y /= 1000;
		pos.z /= 1000;

		pos = convertRightHandedToLeftHandedPosition (pos);
		rot = convertRightHandedToLeftHandedRotation (rot);

		// convert from QTM's right handed coordinate system to Unity's left handed coordinate system
		//pos.x = -pos.x;
		//rot.y = -rot.y;
		//rot.z = -rot.z;
		
		//adjust scale
		//pos.x /= scaleX;
		//pos.x += offsetX;
		//pos.y /= scaleY;
		//pos.y += offsetY;
		//pos.z /= scaleZ;
		//pos.z += offsetZ;

		// update the camera's y only if current pos.y exceeds posPrevious.y by some threshold amount
		// this is to reduce the wavy motions of the walking
		/*
		if (bSmoothCamera) {
			if (Vector3.Distance (virtualPosition, pos) > 0.1) {
				virtualPosition = pos;
			}

			if(Quaternion.Angle(virtualRotation, rot) > 0.1){
				virtualRotation = rot;
			}
				
		} else {
			virtualPosition = pos;
			virtualRotation = rot;
		}
		*/

		position = pos;
		rotation = rot;
	}
	
	//Based on qlm::quat_cast
	private  Quaternion QuaternionFromMatrix(float[] m)
	{
		float fourXSquaredMinus1 = m[0] - m[4] - m[8];
		float fourYSquaredMinus1 = m[4] - m[0] - m[8];
		float fourZSquaredMinus1 = m[8] - m[0] - m[4];
		float fourWSquaredMinus1 = m[0] + m[4] + m[8];
		
		int biggestIndex = 0;
		float fourBiggestSquaredMinus1 = fourWSquaredMinus1;
		if(fourXSquaredMinus1 > fourBiggestSquaredMinus1)
		{
			fourBiggestSquaredMinus1 = fourXSquaredMinus1;
			biggestIndex = 1;
		}
		if(fourYSquaredMinus1 > fourBiggestSquaredMinus1)
		{
			fourBiggestSquaredMinus1 = fourYSquaredMinus1;
			biggestIndex = 2;
		}
		if(fourZSquaredMinus1 > fourBiggestSquaredMinus1)
		{
			fourBiggestSquaredMinus1 = fourZSquaredMinus1;
			biggestIndex = 3;
		}
		
		float biggestVal = Mathf.Sqrt(fourBiggestSquaredMinus1 + 1.0f) * 0.5f;
		float mult = 0.25f / biggestVal;
		
		Quaternion Result = new Quaternion();
		switch(biggestIndex)
		{
		case 0:
			Result.w = biggestVal; 
			Result.x = (m[5] - m[7]) * mult;
			Result.y = (m[6] - m[2]) * mult;
			Result.z = (m[1] - m[3]) * mult;
			break;
		case 1:
			Result.w = (m[5] - m[7]) * mult;
			Result.x = biggestVal;
			Result.y = (m[1] + m[3]) * mult;
			Result.z = (m[6] + m[2]) * mult;
			break;
		case 2:
			Result.w = (m[6] - m[2]) * mult;
			Result.x = (m[1] + m[3]) * mult;
			Result.y = biggestVal;
			Result.z = (m[5] + m[7]) * mult;
			break;
		case 3:
			Result.w = (m[1] - m[3]) * mult;
			Result.x = (m[6] + m[2]) * mult;
			Result.y = (m[5] + m[7]) * mult;
			Result.z = biggestVal;
			break;
		}
		
		return Result;
	}

    // Log OscMessage or OscBundle
    private  void LogPacket(OscPacket packet) {
        if (packet.IsBundle) {
            foreach (OscMessage message in ((OscBundle)packet).Messages) {
                LogMessage(message);
            }
        }
        else {
            LogMessage((OscMessage)packet);
        }
    }

    // Log OscMessage
    private  void LogMessage(OscMessage message) {
        StringBuilder s = new StringBuilder();
        s.Append(message.Address);
        for (int i = 0; i < message.Data.Count; i++) {
            s.Append(" ");
            if (message.Data[i] == null) {
                s.Append("Nil");
            }
            else {
                s.Append(message.Data[i] is byte[] ? BitConverter.ToString((byte[])message.Data[i]) : message.Data[i].ToString());
            }
        }
        Debug.Log(s);
    }

	public Vector3 convertRightHandedToLeftHandedPosition(Vector3 p){
		return new Vector3(-p.x, p.y, p.z);
	}
	public Quaternion convertRightHandedToLeftHandedRotation(Quaternion q){
		return new Quaternion(-q.x, q.y, q.z, -q.w);
	}

	// The type tag for Error.
	protected const string ErrorTag = "/qtm/error";
	
	// The type tag for Error.
	protected const string CommandTag = "/qtm";
	
	// The type tag for Error.
	protected const string CommandResponseTag = "/qtm/cmd_res";
	
	// The type tag for Error.
	protected const string XMLTag = "/qtm/xml";
	
	// The type tag for Error.
	protected const string DataFrameHeaderTag = "/qtm/data";
	
	// The type tag for Error.
	protected const string NoMoreDataTag = "/qtm/no_data";
	
	// The type tag for Error.
	protected const string EventTag = "/qtm/event";
}
