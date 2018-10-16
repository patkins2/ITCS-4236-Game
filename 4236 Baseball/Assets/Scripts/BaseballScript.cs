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
        rb = ball.GetComponent<Rigidbody>();
        rb.detectCollisions = false;
        rb.useGravity = false;
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    public void ReleaseBall()
    {
        rb.constraints = RigidbodyConstraints.None;
        ball.transform.parent = null;
        rb.detectCollisions = true;

        rb.useGravity = true;
        ball.transform.rotation = currentPlayer.transform.rotation;
        rb.AddForce(ball.transform.forward * 5000);
    }
}
