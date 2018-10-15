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
        
        if (currentPlayer.name.Equals("Pitcher"))
        {
            Debug.Log("In pitcher");
            Transform[] components = currentPlayer.GetComponentsInChildren<Transform>();
            foreach (Transform component in components)
            {
                if (component.name.Equals("Baseball"))
                {
                    Debug.Log("Baseball! Is here!");
                    baseball = component.gameObject;
                    Rigidbody rb = baseball.GetComponent<Rigidbody>();
                    rb.detectCollisions = false;
                    
                  
                    
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
                animator.SetTrigger("Pitch");
                
            }
            /*if (currentPlayer.name.Equals("Batting"))
            {
                Debug.Log("Batter");
                animator.SetTrigger("Swing");
                
            }*/
        }
        
        
       
	}

    public void pitch()
    {
        baseball.GetComponent <BaseballScript> ().Release();
        animator.ResetTrigger("Pitch");
        

        /*
        Transform[] components = currentPlayer.GetComponentsInChildren<Transform>();
        foreach(Transform component in components)
        {
            if (component.name.Equals("Baseball"))
            {
                component.parent = null;
                Rigidbody rigidbody = component.gameObject.AddComponent<Rigidbody>();
                rigidbody.mass = 1;
                rigidbody.useGravity = true;
                rigidbody.drag = 0.5f;
            }
        }*/
    }

   
}
