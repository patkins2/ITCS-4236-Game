using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatScript : MonoBehaviour {

    [HideInInspector] public PlayerController batter;

    [SerializeField] private GameObject currentPlayer;
    [SerializeField] private GameObject leftHand;
    private Rigidbody rb;
    //[SerializeField] private GameObject bat;
    private GameObject bat;
    public bool hit = false;

    // Use this for initialization
    void Start () {
        bat = this.gameObject;
        bat.transform.parent = leftHand.transform;
        rb = bat.GetComponent<Rigidbody>();
        //rb.detectCollisions = false;
        rb.useGravity = false;
    }

    //Called by batter animator, drops bat from players hand and enables physics on it
    public void ReleaseBat()
    {
        rb.constraints = RigidbodyConstraints.None;
        bat.transform.parent = null;
        rb.detectCollisions = true;

        rb.useGravity = true;
        bat.transform.rotation = leftHand.transform.rotation;
        rb.AddForce(bat.transform.forward * 500);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Ball"))
        {
            TellBatterBallHit();
            //print("triggered by ball");
            ReleaseBat();
            //tell camera to go to overhead view
        }
    }

    private void TellBatterBallHit() {
        GameManager.self.currentGameState = GameManager.GameStates.BallInPlay;
        batter.ballHit = true;
    }
}
