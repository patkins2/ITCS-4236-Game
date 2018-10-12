using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseballScript : MonoBehaviour {

    [SerializeField] private GameObject currentPlayer;
    private GameObject rightHand;
    private Rigidbody rigidbody;
    [SerializeField] private GameObject ball;

	// Use this for initialization
	void Start () {
        
        Transform[] components = currentPlayer.GetComponentsInChildren<Transform>();
        foreach (Transform component in components)
        {
            if (component.name.Equals("mixamorig:RightHand"))
            {
                rightHand = component.gameObject;
            }
        }
        ball.transform.parent = rightHand.transform;
        rigidbody = ball.GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Release()
    {
        ball.transform.parent = null;

        rigidbody.useGravity = true;
        ball.transform.rotation = rightHand.transform.rotation;
        rigidbody.AddForce(ball.transform.forward * 20000);
    }
}
