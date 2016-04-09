using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AladdinNetworkDiscovery : NetworkDiscovery
{
	private AladdinNetworkManager networkManager;

	void Start () {
		networkManager = (AladdinNetworkManager)NetworkManager.singleton;
	}

	public override void OnReceivedBroadcast (string fromAddress, string data)
	{
		Debug.Log ("OnReceivedBroadcast fromAddress: " + fromAddress + " data: " + data);
		networkManager.OnReceivedBroadcast (fromAddress);
	}

}