using UnityEngine;
using System.Collections;

public class GroundSinkController : MonoBehaviour {

	void OnCollisionEnter(GameObject other) {
		if (other.transform.position.y < transform.position.y) {
			Destroy (other);
		}
	}
}
