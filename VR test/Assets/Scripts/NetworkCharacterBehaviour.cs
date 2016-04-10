using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkCharacterBehaviour : NetworkBehaviour {

	private GameObject camera;

	public override void OnStartLocalPlayer() {
		camera = gameObject.transform.GetChild(0).GetChild(0).gameObject;
		camera.SetActive (true);
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
