using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatScript : MonoBehaviour {

    [SerializeField] private GameObject currentPlayer;
    [SerializeField] private GameObject leftHand;
    private Rigidbody rb;
    //[SerializeField] private GameObject bat;
    private GameObject bat;

    // Use this for initialization
    void Start () {
        bat = this.gameObject;
        bat.transform.parent = leftHand.transform;
        rb = bat.GetComponent<Rigidbody>();
        rb.detectCollisions = false;
        rb.useGravity = false;
    }

    public void ReleaseBat()
    {
        rb.constraints = RigidbodyConstraints.None;
        bat.transform.parent = null;
        rb.detectCollisions = true;

        rb.useGravity = true;
        bat.transform.rotation = leftHand.transform.rotation;
        rb.AddForce(bat.transform.forward * 500);
    }
}
