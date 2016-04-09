using UnityEngine;
using System.Collections;

public class MoveAroundController : MonoBehaviour {

	private Vector3 direction;
	private float maxDistance;

	void Start () {
		direction = (-transform.position).normalized;
		direction.y = 0;
		maxDistance = transform.position.magnitude;
	}

	void FixedUpdate () {
		transform.position += direction;
		if (transform.position.magnitude > maxDistance) {
			direction = -direction;
		}
	}
}
