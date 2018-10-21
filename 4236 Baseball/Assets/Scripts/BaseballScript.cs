using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseballScript : MonoBehaviour {

    [SerializeField] private GameObject currentPlayer;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject ball;
    [SerializeField] private float throwForce = 5000f;      //How hard the ball is thrown

    private Rigidbody rb;
    
	// Use this for initialization
	void Start () {
        ball.transform.parent = rightHand.transform;
        rb = ball.GetComponent<Rigidbody>();
        rb.detectCollisions = false;
        rb.useGravity = false;
	}

    //Called from pitcher animator, releases ball from pitcher's hand and launches towards the batter
    public void ReleaseBall()
    {
        rb.detectCollisions = true;
        rb.constraints = RigidbodyConstraints.None;
        ball.transform.parent = null;
        

        //rb.useGravity = true;
        ball.transform.rotation = currentPlayer.transform.rotation;
        rb.AddForce(ball.transform.forward * throwForce);
    }
}
