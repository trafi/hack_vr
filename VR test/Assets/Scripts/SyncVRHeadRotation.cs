using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.VR;

public class SyncVRHeadRotation : NetworkBehaviour {
	
	// Update is called once per frame
	void Update () {
		if (!isServer && isLocalPlayer) {
			CmdSendGodHeadRotation (InputTracking.GetLocalRotation (VRNode.Head));
		}
	}

	[Command]
	void CmdSendGodHeadRotation(Quaternion rotation) {
		transform.rotation = rotation;
	}
}
