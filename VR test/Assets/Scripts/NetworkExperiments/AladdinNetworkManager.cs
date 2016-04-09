using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AladdinNetworkManager : NetworkManager {

	public NetworkDiscovery discovery;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.touchCount > 0) {
			Touch touch = Input.GetTouch (0);
			if (touch.phase == TouchPhase.Began) {
				float x = touch.position.x / Screen.width;
				float y = touch.position.y / Screen.height;
				if (x < 0.5) {
					if (y < 0.5) {
						Debug.Log ("Starting host");
						StartHost ();
					} else {
						Debug.Log ("Starting client");
						StartClient ();
					}
				} else {
					if (y < 0.5) {
						if (!discovery.isServer) {
							Debug.Log ("Starting discovery as server");
							discovery.Initialize ();
							discovery.StartAsServer ();
						}
					} else {
						if (!discovery.isClient) {
							Debug.Log ("Starting discovery as client");
							discovery.Initialize ();
							discovery.StartAsClient ();
						}
					}
				}
			}
		}
	}

	public override void OnStartHost ()
	{
		base.OnStartHost ();
		Debug.Log ("OnStartHost");
	}

	public override void OnServerConnect(NetworkConnection conn)
	{
		Debug.Log ("OnPlayerConnected");
	}
}
