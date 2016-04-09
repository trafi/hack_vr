using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AladdinNetworkDiscovery : NetworkDiscovery
{
	public override void OnReceivedBroadcast (string fromAddress, string data)
	{
		Debug.Log ("OnReceivedBroadcast fromAddress: " + fromAddress + " data: " + data);
		NetworkManager.singleton.networkAddress = fromAddress;
		NetworkManager.singleton.StartClient();
	}

}