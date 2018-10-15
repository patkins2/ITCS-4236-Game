using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseballScript : MonoBehaviour {

    [SerializeField] private GameObject currentPlayer;
    [SerializeField] private GameObject rightHand;
    private Rigidbody rb;
    [SerializeField] private GameObject ball;

	// Use this for initialization
	void Start () {
        ball.transform.parent = rightHand.transform;
        //ball.transform.parent = rightHand.transform;
        rb = ball.GetComponent<Rigidbody>();
        rb.useGravity = false;
        

	}
	
	// Update is called once per frame
	void Update () {
        
    }

    public void Release()
    {
        ball.transform.parent = null;
        rb.detectCollisions = true;

        rb.useGravity = true;
        ball.transform.rotation = rightHand.transform.rotation;
        rb.AddForce(ball.transform.forward * 5000);
    }
}
