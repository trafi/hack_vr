﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PickupSpawner : NetworkBehaviour
{

    public GameObject pickup;
    GameObject spawnedPickup;
    Vector3 lastPos;

    private Vector3 getPos()
    {
        return new Vector3(Random.Range(-100, 100), 15, Random.Range(-100, 100));
    }

	// Use this for initialization
	void Start () {
        Debug.Log("Pickup spawner start");
	}
	
	// Update is called once per frame
	void Update () {
        if(spawnedPickup == null)
        {
            Vector3 newPos = getPos();
            for (int i = 0; Vector3.Distance(newPos, lastPos) < 100; i++)
            {
                newPos = getPos();
            }
//            Debug.Log("Distance " + Vector3.Distance(newPos, lastPos));
            lastPos = newPos;
            spawnedPickup = (GameObject)Instantiate(pickup, newPos, Quaternion.Euler(270, 0, 0));
            NetworkServer.Spawn(spawnedPickup);
        }
        //Instantiate(pickup, getPos(), new Quaternion());
    }
}
