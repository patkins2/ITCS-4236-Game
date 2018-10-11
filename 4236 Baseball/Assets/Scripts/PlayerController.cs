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

    private GameObject currentPlayer;
    private GameObject baseball;
    private GameObject rightHand;
    private Animator animator;
	// Use this for initialization
	void Start () {
        runSpeed = Random.Range(minSpeed, maxSpeed);
        swingPower = Random.Range(minPower, maxPower);
        currentPlayer = this.gameObject;
       
        animator = GetComponent<Animator>();
        
        
        if (currentPlayer.name.Equals("Batting"))
        {
            animator.Play("Baseball Idle");
        }
        if (currentPlayer.name.Equals("Pitcher"))
        {
            Transform[] components = currentPlayer.GetComponentsInChildren<Transform>();
            foreach (Transform component in components)
            {
                if (component.name.Equals("mixamorig:RightHand"))
                {
                    rightHand = component.gameObject;
                }
                if (component.name.Equals("Baseball"))
                {
                    baseball = component.gameObject;
                    baseball.transform.parent = rightHand.transform;
                    
                }
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
                pitch();
            }
            if (currentPlayer.name.Equals("Batting"))
            {
                Debug.Log("Batter");
                animator.SetTrigger("Swing");
                
            }
        }
        
        
       
	}

    public void pitch()
    {
        animator.SetTrigger("Pitch");
        Transform[] components = currentPlayer.GetComponentsInChildren<Transform>();
        foreach(Transform component in components)
        {
            if (component.name.Equals("Baseball"))
            {
                component.parent = null;
                Rigidbody rigidbody = component.GetComponent<Rigidbody>();
                rigidbody.useGravity = true;
                rigidbody.drag = 0.5f;
            }
        }
    }

   
}
