using UnityEngine;
using System.Collections;

public class PickupController : MonoBehaviour {

    public GameObject[] pickupIndicators = new GameObject[10];
    int collectableCounter = 0;

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
            pickupIndicators[collectableCounter].GetComponent<Renderer>().enabled = true;
            collectableCounter++;
        }
        if(other.gameObject.tag == "Bounds")
        {
            transform.position = new Vector3(0, 0, 0);
        }
    }
}
