﻿using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class MovementController : MonoBehaviour
{
    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    public float maxTurnRadius = 0.3f;
    private Vector3 moveDirection = Vector3.zero;
	CharacterController character;

	private NetworkCharacterBehaviour network;

	void Start() {
		network = GetComponent<NetworkCharacterBehaviour>();
		character = GetComponent<CharacterController> ();
	}

    void Update()
	{
		if(null != network && !network.isLocalPlayer) {
			return;
		}

        Quaternion headRotation = InputTracking.GetLocalRotation(VRNode.Head);
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if(moveDirection == Vector3.zero)
        {
            moveDirection = (headRotation * Vector3.forward);
        }
        moveDirection.y = -1;
        moveDirection.z = 1;
        moveDirection.x = Mathf.Clamp(moveDirection.x, -maxTurnRadius, maxTurnRadius);
        transform.Rotate(0, moveDirection.x, 0);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= speed;
        if (Input.GetButton("Jump"))
            moveDirection.y = jumpSpeed;
        moveDirection.y -= gravity * Time.deltaTime;
		character.Move(moveDirection * Time.deltaTime);
    }
}