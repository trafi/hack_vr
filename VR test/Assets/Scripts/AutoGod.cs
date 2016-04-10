using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.VR;

class LandingObjectTimeout {
	public float livetime;
	public GameObject obj;

	public LandingObjectTimeout(float livetime, GameObject obj) {
		this.livetime = livetime;
		this.obj = obj;
	}
}

class SinkingObject {
	public Vector3 vel;
	public GameObject obj;

	public SinkingObject(Vector3 vel, GameObject obj) {
		this.vel = vel;
		this.obj = obj;
	}
}

public class AutoGod : NetworkBehaviour {

	public string DeadlyTag = "Deadly";
	private string GodTag = "God";

	public GameObject Thrower;
	public GameObject[] Objects;
	public float TimeBetweenThrows = 2;
	public float TimeToAppear = 2;
	public float TimeToGetBig = 2;
	public float ThrowForce = 300;
	public float ThrowableObjectSize = 3; 
	public GameObject Target;

	public float TimeForBounce = 10.0f;
	public float DeathDepth = -20.0f;
	public float SinkSpeed = 0.8f;

	private float throwerTimePassed;

	private Vector3 nextObjectSmallScale;
	private Vector3 nextObjectRealScale;
	private GameObject nextObject;

	private float thrownTimePassed;
	private Vector3 thrownObjectSmallScale;
	private Vector3 thrownObjectRealScale;
	private GameObject thrownObject;

	private ArrayList landingObjects = new ArrayList();
	private ArrayList sinkingObjects = new ArrayList();

	void Start () {
		throwerTimePassed = 0.0f;
		TakeNextObject ();
	}

	void Update () {
		//Debug.Log ("Update local " + isLocalPlayer + " server " + isServer);

		Thrower = GameObject.FindGameObjectWithTag (GodTag);
		if (null == Thrower) {
			return;
		}

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
			var landingObject = (LandingObjectTimeout)landingObjects[i];
			landingObject.livetime += Time.deltaTime;

			if (landingObject.livetime >= TimeForBounce) {
				var rb = landingObject.obj.GetComponent<Rigidbody> ();
				var vel = rb.velocity;
				rb.useGravity = false;
				rb.isKinematic = true;
				landingObject.obj.tag = "Untagged";

				sinkingObjects.Add (new SinkingObject(vel, landingObject.obj));
				landingObjects.RemoveAt (i);
			}
		}

		for (var i = sinkingObjects.Count - 1; i >= 0; i--) {
			var sinkingObject = (SinkingObject)sinkingObjects[i];
			if (sinkingObject.obj.transform.position.y < DeathDepth) {
				ReturnThrowableToQueue (sinkingObject.obj);
				sinkingObjects.RemoveAt (i);
			} else {
				sinkingObject.vel = new Vector3 (sinkingObject.vel.x * 0.97f, sinkingObject.vel.y - SinkSpeed, sinkingObject.vel.z * 0.97f);
				sinkingObject.obj.transform.position += sinkingObject.vel * Time.deltaTime;
			}
		}
	}

	void TakeNextObject() {
		var index = Random.Range (0, Objects.Length);
		var o = Objects[index];
		o.tag = DeadlyTag;
		nextObject = Instantiate (o);

		nextObject.transform.position = Thrower.transform.position;
		nextObject.GetComponent<Rigidbody> ().isKinematic = true;
		nextObject.SetActive (true);
		nextObjectRealScale = nextObject.transform.localScale;
		var size = nextObject.GetComponent<Renderer> ().bounds.size.magnitude;
		var smallObjScale = ThrowableObjectSize / (size > 0 ? size : 1);
		nextObjectSmallScale = nextObject.transform.localScale * smallObjScale;
		nextObject.transform.localScale = Vector3.zero;

		NetworkServer.Spawn (nextObject);
	}

	void ThrowNextObject() {
		var rb = nextObject.GetComponent<Rigidbody> ();
		rb.useGravity = true;
		rb.isKinematic = false;
		rb.angularVelocity = new Vector3 (Random.Range(0.0f, 0.8f), Random.Range(0.0f, 0.8f), Random.Range(0.0f, 0.8f));
		if (Thrower != null) {
			Quaternion headRotation = Thrower.transform.rotation;
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
			
		landingObjects.Add (new LandingObjectTimeout (0, nextObject));

		thrownObject = nextObject;
		thrownObjectSmallScale = nextObjectSmallScale;
		thrownObjectRealScale = nextObjectRealScale;
		thrownTimePassed = 0;

		nextObject = null;
	}

	void ReturnThrowableToQueue(GameObject throwable) {
		Destroy (throwable);
	}
}
