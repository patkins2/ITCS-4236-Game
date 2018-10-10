using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * PlayerController keeps all information about the player and holds all the logic for their actions
 * 
 */ 
public class PlayerController : MonoBehaviour {

    [SerializeField] private float minSpeed, maxSpeed, minPower, maxPower;
    [SerializeField] private GameObject bat;
    [SerializeField] private GameObject batPrefab;
    [SerializeField] private float runSpeed, swingPower;

    private GameObject currentPlayer;
    private GameObject hand;
    private Animator animator;
	// Use this for initialization
	void Start () {
        runSpeed = Random.Range(minSpeed, maxSpeed);
        swingPower = Random.Range(minPower, maxPower);
        currentPlayer = this.gameObject;
        hand = null;
        animator = GetComponent<Animator>();

        if (currentPlayer.name.Equals("Batting"))
        {
            animator.Play("Baseball Idle");
        }
        else
        {
            animator.Play("Idle (1)");
        }
        
        //Gets all of the children objects of the player
        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.name.Equals("mixamorig:RightHand"))
            {
                //Gets the right hand object
                hand = child.gameObject;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(1))
        {
            if (currentPlayer.name.Equals("Pitcher"))
            {
                Debug.Log("Pitching");
                animator.Play("Baseball Pitching");
            }
            if (currentPlayer.name.Equals("Batting"))
            {
                Debug.Log("Batter");
                animator.Play("Baseball Hit");
            }
        }
        
        if(bat != null)
        {
            //Attaches the bat so it will move with the player's hand
            bat.transform.parent = hand.transform.parent;
            bat.transform.rotation = hand.transform.rotation;
        }
        
	}

   
}
