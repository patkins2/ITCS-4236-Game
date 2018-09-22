using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * PlayerController keeps all information about the player and holds all the logic for their actions
 * 
 */ 
public class PlayerController : MonoBehaviour {

    [SerializeField] private float minSpeed, maxSpeed, minPower, maxPower;
    [SerializeField] private float runSpeed, swingPower;

	// Use this for initialization
	void Start () {
        runSpeed = Random.Range(minSpeed, maxSpeed);
        swingPower = Random.Range(minPower, maxPower);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
