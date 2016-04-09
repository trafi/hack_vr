using UnityEngine;
using System.Collections;

public class PickupController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	        
	}

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("collision " + other.gameObject.name + " @ " + transform.position);
        if (other.gameObject.tag == "Pickup")
        {
            Destroy(other.gameObject);
        }
    }
}
