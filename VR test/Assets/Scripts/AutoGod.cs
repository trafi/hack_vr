using UnityEngine;
using System.Collections;

public class AutoGod : MonoBehaviour {

	public string DeadlyTag = "Deadly";

	public GameObject[] Objects;
	public GameObject Thrower;
	public float TimeBetweenThrows = 2;
	public float ThrowForce = 300;
	public GameObject Target;
	public GameObject Ground;

	public float DeathDepth = -20.0f;
	public float HitGroundSlowdown = 0.2f;
	public float MinimumVelocity = 0.1f;
	public float BeforeSinkSlowdown = 0.8f;
	public float SinkSpeed = 0.8f;

	private float timePassed;
	private GameObject nextObject;

	private ArrayList unusedThrowableObjects = new ArrayList();
	private ArrayList landingObjects = new ArrayList();
	private ArrayList sinkingObjects = new ArrayList();

	// Use this for initialization
	void Start () {
		timePassed = 0.0f;
		TimeBetweenThrows = 1;
		TakeNextObject ();

		for (var i = 0; i < 30; i++) {
			unusedThrowableObjects.Add(CreateRandomThrowable());
		}
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

		for (var i = landingObjects.Count - 1; i >= 0; i--) {
			var landingObject = (GameObject)landingObjects[i];
			if (landingObject.transform.position.y < 0) {
				sinkingObjects.Add (landingObject);
				var rb = landingObject.GetComponent<Rigidbody> ();
				rb.useGravity = false;
				rb.velocity.Normalize ();
				rb.velocity *= HitGroundSlowdown;

				landingObjects.RemoveAt (i);
			}
		}

		for (var i = sinkingObjects.Count - 1; i >= 0; i--) {
			var sinkingObject = (GameObject)sinkingObjects[i];
			if (sinkingObject.transform.position.y < DeathDepth) {
				ReturnThrowableToQueue (sinkingObject);
				sinkingObjects.RemoveAt (i);
			} else {
				var rb = sinkingObject.GetComponent<Rigidbody> ();
				if (rb.velocity.magnitude < MinimumVelocity) {
					sinkingObject.transform.position -= new Vector3 (0, SinkSpeed * Time.deltaTime, 0);
				} else {
					rb.velocity *= BeforeSinkSlowdown * Time.deltaTime;
				}
			}
		}
	}

	void TakeNextObject() {
		nextObject = GetThrowableObject();
		nextObject.transform.position = Thrower.transform.position;
		nextObject.GetComponent<Rigidbody> ().isKinematic = true;
		nextObject.SetActive (true);
	}

	void ThrowNextObject() {
		nextObject.GetComponent<Rigidbody> ().isKinematic = false;
		var direction = Target.transform.position - Thrower.transform.position;
		direction.y = 0;
		nextObject.GetComponent<Rigidbody>().AddForce (direction * ThrowForce);

		landingObjects.Add (nextObject);
		nextObject = null;
	}

	GameObject GetThrowableObject() {
		if (unusedThrowableObjects.Count > 0) {
			var obj = unusedThrowableObjects [0];
			unusedThrowableObjects.RemoveAt (0);
			return (GameObject)obj;
		}
		return CreateRandomThrowable ();
	}

	GameObject CreateRandomThrowable() {
		var index = Random.Range (0, Objects.Length);
		var o = Objects[index];
		return Instantiate (o);
	}

	void ReturnThrowableToQueue(GameObject throwable) {
		throwable.SetActive (false);
		var rb = throwable.GetComponent<Rigidbody> ();
		rb.isKinematic = false;
		rb.useGravity = true;
		rb.velocity = Vector3.zero;
		unusedThrowableObjects.Add (throwable);
	}
}
