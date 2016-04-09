using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class AutoGod : MonoBehaviour {

	public string DeadlyTag = "Deadly";

	public GameObject[] Objects;
	public GameObject Thrower;
	public float TimeBetweenThrows = 2;
	public float TimeToAppear = 2;
	public float TimeToGetBig = 2;
	public float ThrowForce = 300;
	public float ThrowableObjectSize = 3; 
	public GameObject Target;
	public GameObject LookAtObject;

	public float GroundDepth = 2.0f;
	public float DeathDepth = -20.0f;
	public float HitGroundSlowdown = 0.2f;
	public float MinimumVelocity = 0.1f;
	public float BeforeSinkSlowdown = 0.8f;
	public float SinkSpeed = 0.8f;

	private float throwerTimePassed;

	private Vector3 nextObjectSmallScale;
	private Vector3 nextObjectRealScale;
	private GameObject nextObject;

	private float thrownTimePassed;
	private Vector3 thrownObjectSmallScale;
	private Vector3 thrownObjectRealScale;
	private GameObject thrownObject;

	private ArrayList unusedThrowableObjects = new ArrayList();
	private ArrayList landingObjects = new ArrayList();
	private ArrayList sinkingObjects = new ArrayList();

	void Start () {
		throwerTimePassed = 0.0f;
		TakeNextObject ();

		for (var i = 0; i < 30; i++) {
			unusedThrowableObjects.Add(CreateRandomThrowable());
		}
	}

	void FixedUpdate () {
		throwerTimePassed += Time.deltaTime;
		if (nextObject == null) {
			if (throwerTimePassed > TimeBetweenThrows) {
				TakeNextObject ();
				throwerTimePassed = 0;
			}
		} else {
			if (throwerTimePassed > TimeToAppear) {
				ThrowNextObject ();
				throwerTimePassed = 0;
			} else {
				var progress = throwerTimePassed / TimeToAppear;
				nextObject.transform.localScale = nextObjectSmallScale * progress;
			}
		}

		thrownTimePassed += Time.deltaTime;
		if (thrownObject != null) {
			if (thrownTimePassed < TimeToGetBig) {
				var progress = thrownTimePassed / TimeToGetBig;
				thrownObject.transform.localScale = thrownObjectSmallScale + (thrownObjectRealScale - thrownObjectSmallScale) * progress;
			} else {
				thrownObject = null;
			}
		}

		for (var i = landingObjects.Count - 1; i >= 0; i--) {
			var landingObject = (GameObject)landingObjects[i];
			var objSize = landingObject.GetComponent<Renderer> ().bounds.size.magnitude;

			if (landingObject.transform.position.y < GroundDepth + objSize / 4.0f) {
				sinkingObjects.Add (landingObject);
				var rb = landingObject.GetComponent<Rigidbody> ();
				rb.useGravity = false;
				rb.isKinematic = true;
				rb.velocity.Normalize ();
				rb.velocity *= HitGroundSlowdown;
				landingObject.GetComponent<Collider> ().isTrigger = false;
				landingObject.tag = "Untagged";

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
		nextObjectRealScale = nextObject.transform.localScale;
		var size = nextObject.GetComponent<Renderer> ().bounds.size.magnitude;
		var smallObjScale = ThrowableObjectSize / (size > 0 ? size : 1);
		nextObjectSmallScale = nextObject.transform.localScale * smallObjScale;
		nextObject.transform.localScale = Vector3.zero;
	}

	void ThrowNextObject() {
		var rb = nextObject.GetComponent<Rigidbody> ();
		rb.useGravity = true;
		rb.isKinematic = false;
		rb.angularVelocity = new Vector3 (Random.Range(0.0f, 0.8f), Random.Range(0.0f, 0.8f), Random.Range(0.0f, 0.8f));
		if (LookAtObject != null) {
			Quaternion headRotation = InputTracking.GetLocalRotation(VRNode.Head);
			var moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			if(moveDirection == Vector3.zero)
			{
				moveDirection = (headRotation * Vector3.forward);
			}
			rb.AddForce (moveDirection * ThrowForce * 300);
		} else if (Target != null) {
			var direction = Target.transform.position - Thrower.transform.position;
			direction.y = 0;
			rb.AddForce (direction * ThrowForce);
		}

		landingObjects.Add (nextObject);

		thrownObject = nextObject;
		thrownObjectSmallScale = nextObjectSmallScale;
		thrownObjectRealScale = nextObjectRealScale;
		thrownTimePassed = 0;

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
		o.GetComponent<Collider> ().isTrigger = true;
		o.tag = DeadlyTag;
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
