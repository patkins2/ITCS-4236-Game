using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseballScript : MonoBehaviour {

    [SerializeField] private GameObject currentPlayer;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject ball;
    [SerializeField] private GameObject strikeZone;
    [SerializeField] private GameObject target;
    [SerializeField] private float throwForce = 5000f;      //How hard the ball is thrown

    private Rigidbody rb;
    private Collision collision;
    private Vector3 relativePos;
    private Quaternion rotation;
   


    // Use this for initialization
    void Start () {
        ball.transform.parent = rightHand.transform;
        rb = ball.GetComponent<Rigidbody>();
        rb.detectCollisions = false;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
	}

    //Called from pitcher animator, releases ball from pitcher's hand and launches towards the batter
    public void ReleaseBall()
    {
        rb.detectCollisions = true;
        rb.constraints = RigidbodyConstraints.None;
        ball.transform.parent = null;

        relativePos = strikeZone.transform.position - ball.transform.position;
        //rb.useGravity = true;
        rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        ball.transform.rotation = rotation;
        rb.AddForce(relativePos.normalized * throwForce);

        /*if(collision.gameObject.name == "bat")
        {

        }*/
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Bat"))
        {
             print("Ball hit bat");


             relativePos = target.transform.position - ball.transform.position;
             /*List<Vector3> positionList;
             // ...

             positionList = new List<Vector3>();

             int maxIterations = Mathf.RoundToInt(5.0f / Time.fixedDeltaTime);
             relativePos = ball.transform.position;
             Vector3 vel = rb.velocity;
             float drag = rb.drag;
             positionList.Add(relativePos);
             float elapsedTime = 0.0f;

             for (int i = 0; i < maxIterations; i++)
             {
                 vel = vel + (Physics.gravity * Time.fixedDeltaTime);
                 vel *= drag;
                relativePos += vel * Time.fixedDeltaTime * -1;
                 elapsedTime += Time.fixedDeltaTime;
                 positionList.Add(relativePos);
             }*/
           // relativePos *= -1;
        }
        else if (other.gameObject.name.Equals("Catcher"))
        {
            //Add code to return ball to pitcher
            
        }
        //else
            //print("Ball hit: " + other.gameObject.name);
    }
}
