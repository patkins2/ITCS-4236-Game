using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseballScript : MonoBehaviour {

    [SerializeField] private GameObject currentPlayer;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject ball;
    [SerializeField] private GameObject strikeZone;
    [SerializeField] private float throwForce = 3500f;      //How hard the ball is thrown
    

    private Rigidbody rb;
    private Collision collision;
    private Vector3 relativePos;
    private Quaternion rotation;
    private GameObject target;
    public float passVelocity;
    //public Transform trans;



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
        print("ball going");
        rb.detectCollisions = true;
        rb.constraints = RigidbodyConstraints.None;
        ball.transform.parent = null;

        relativePos = strikeZone.transform.position - ball.transform.position;
        //rb.useGravity = true;
        rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        ball.transform.rotation = rotation;
        rb.AddForce(relativePos.normalized * throwForce);

    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Bat"))
        {
            print("Ball hit bat");

            //Random rnd = new Random();
            int targetNum = Random.Range(1, 8); //random number between 1 and 9

            //based on the random number, assign where the ball will go. 
            switch (targetNum)
            {
                case 1:
                    target = GameObject.Find("Target2");
                    break;

                case 2:
                    target = GameObject.Find("Target3");
                    break;

                case 3:
                    target = GameObject.Find("Target4");
                    break;

                case 4:
                    target = GameObject.Find("Target5");
                    break;

                case 5:
                    target = GameObject.Find("Target7");
                    break;

                case 6:
                    target = GameObject.Find("Target8");
                    break;

                case 7:
                    target = GameObject.Find("Target9");
                    break;

                default:
                    break;
            }


            this.GetComponent<Collider>().isTrigger = false;
            rb.useGravity = true;

            //relativePos = target.transform.position - ball.transform.position;
            //rb.AddForce(relativePos.normalized * throwForce * 2); 
            
            // need to look into ballistic velocity

            SetVelocity(BallisticVel(target.transform.position, 15f));
            




        }
        else if (other.gameObject.name.Equals("Catcher"))
        {
            //Add code to return ball to pitcher
            
        }
        //else
            //print("Ball hit: " + other.gameObject.name);
    }


    Vector3 BallisticVel(Vector3 target, float angle)
    {
        Vector3 elevatedTarget = new Vector3(target.x, 1f, target.z);
        Vector3 dir = elevatedTarget - ball.transform.position;  // get target direction
        float h = dir.y;  // get height difference
        dir.y = 0;  // retain only the horizontal direction
        float dist = dir.magnitude;  // get horizontal distance
        float a = angle * Mathf.Deg2Rad;  // convert angle to radians
        dir.y = dist * Mathf.Tan(a);  // set dir to the elevation angle
        dist += h / Mathf.Tan(a);  // correct for small height differences
                                   // calculate the velocity magnitude
        passVelocity = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        Vector3 normVel = passVelocity * dir.normalized;
        
        return normVel;
    }

    public void SetVelocity(Vector3 v)
    {
        rb.velocity = v;
    }
}
