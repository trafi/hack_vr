using UnityEngine;
using System.Collections;

public class DeadlyObjectController : MonoBehaviour {

	public GameObject Ground;

	void OnFixedUpdate() {

		if (transform.position.y < Ground.transform.position.y) {
			Destroy (this.gameObject);
		}

	}
}
