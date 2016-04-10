using UnityEngine;
using System.Collections;

public class SpectatorMovement : MonoBehaviour {

	//initial speed
	public float moveSpeed = 10;
	public float lookSpeed = 10;

	private bool mouseDown;
	private float rotationX;
	private float rotationY;

	private NetworkCharacterBehaviour network;

	// Use this for initialization
	void Start () {
		network = GetComponent<NetworkCharacterBehaviour>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(null != network && !network.isLocalPlayer) {
			return;
		}

		if (Input.GetMouseButtonUp (0)) {
			mouseDown = false;
		} else if (Input.GetMouseButtonDown (0)) {
			mouseDown = true;
		}

		if (mouseDown) {
			rotationX += Input.GetAxis ("Mouse X") * lookSpeed;
			rotationY += Input.GetAxis ("Mouse Y") * lookSpeed;
			rotationY = Mathf.Clamp (rotationY, -90, 90);
		}

		transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
		transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

		transform.position += transform.forward*moveSpeed*Input.GetAxis("Vertical");
		transform.position += transform.right*moveSpeed*Input.GetAxis("Horizontal");
	}
}
