using UnityEngine;
using System.Collections;

public class AutoGod : MonoBehaviour {

	public GameObject[] Objects;
	public GameObject Thrower;
	public float TimeBetweenThrows = 2;
	public float ThrowForce = 300;
	public GameObject Target;

	private float timePassed;
	private GameObject nextObject;

	// Use this for initialization
	void Start () {
		timePassed = 0.0f;
		TimeBetweenThrows = 1;
		TakeNextObject ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		timePassed += Time.deltaTime;
		if (timePassed > TimeBetweenThrows) {
			if (nextObject == null) {
				TakeNextObject ();
			} else {
				ThrowNextObject ();
			}
			timePassed = 0;
		}
	}

	void TakeNextObject() {
		var index = Random.Range (0, Objects.Length);
		var o = Objects[index];
		nextObject = Instantiate (o);
		nextObject.transform.position = Thrower.transform.position;
		nextObject.GetComponent<Rigidbody> ().isKinematic = true;
		nextObject.SetActive (true);
	}

	void ThrowNextObject() {
		nextObject.GetComponent<Rigidbody> ().isKinematic = false;
		var direction = Target.transform.position - Thrower.transform.position;
		direction.y = 0;
		nextObject.GetComponent<Rigidbody>().AddForce (direction * ThrowForce);
		nextObject = null;
	}

}
