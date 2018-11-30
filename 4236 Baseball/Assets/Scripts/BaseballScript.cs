using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseballScript : MonoBehaviour {

    [SerializeField] private GameObject currentPlayer;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject ball;
    [SerializeField] private GameObject strikeZone;
    [SerializeField] private float throwForce = 5000f;      //How hard the ball is thrown

    private Rigidbody rb;
    private Collision collision;
    private Vector3 relativePos;
    private Quaternion rotation;
    private GameObject target;



    // Use this for initialization
    void Start () {
        ball.transform.parent = rightHand.transform;
        rb = ball.GetComponent<Rigidbody>();
        rb.detectCollisions = false;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
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

            //Random rnd = new Random();
            int targetNum = Random.Range(1, 10); //random number between 1 and 9

            //based on the random number, assign where the ball will go. 
            switch (targetNum)
            {
                case 1:
                    target = GameObject.Find("Target1");
                    break;

                case 2:
                    target = GameObject.Find("Target2");
                    break;

                case 3:
                    target = GameObject.Find("Target3");
                    break;

                case 4:
                    target = GameObject.Find("Target4");
                    break;

                case 5:
                    target = GameObject.Find("Target5");
                    break;

                case 6:
                    target = GameObject.Find("Target6");
                    break;

                case 7:
                    target = GameObject.Find("Target7");
                    break;

                case 8:
                    target = GameObject.Find("Target8");
                    break;

                case 9:
                    target = GameObject.Find("Target9");
                    break;

                default:
                    break;
            }


            this.GetComponent<Collider>().isTrigger = false;
            rb.useGravity = true;

             relativePos = target.transform.position - ball.transform.position;
             rb.AddForce(relativePos.normalized * throwForce * 2); // need to look into ballistic velocity
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
