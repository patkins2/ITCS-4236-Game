using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseballCameraScript : MonoBehaviour {

    private GameObject baseball;

    //private GameObject camera;
    private Transform trans;

    private bool followTheBall = false;

	// Use this for initialization
	void Start () {
        //camera = this.gameObject;
        trans = transform;
        baseball = GameManager.self.baseball;
	}
	
	// Update is called once per frame
	void Update () {
        if (followTheBall)
        {
            //camera.transform.position = baseball.transform.position;
            trans.position = baseball.transform.position;
        }
        
	}

    public void followBall(bool command)
    {
        followTheBall = command;
    }
}
