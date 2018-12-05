using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseballCameraScript : MonoBehaviour {

    [SerializeField] GameObject baseball;

    private GameObject camera;

    private bool followTheBall = false;

	// Use this for initialization
	void Start () {
        camera = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        if (followTheBall)
        {
            camera.transform.position = baseball.transform.position;
        }
        
	}

    public void followBall(bool command)
    {
        followTheBall = command;
    }
}
