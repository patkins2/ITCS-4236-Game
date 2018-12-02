using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseballScript : MonoBehaviour {

    [SerializeField] public GameObject rightHand;
    [SerializeField] private GameObject ball;
    [SerializeField] private float throwForce = 3500f;      //How hard the ball is thrown

    private Rigidbody rb;
    private Collision collision;
    private Vector3 relativePos;
    private Quaternion rotation;
    private GameObject target;
    public float passVelocity;
    //public Transform trans;

    public bool held = false;
    public GameObject catcher;


    // Use this for initialization
    void Start () {
        ball.transform.parent = rightHand.transform;
        rb = GetComponent<Rigidbody>();
        BallInHand();
	}

    //Called from pitcher animator, releases ball from pitcher's hand and launches towards the batter
    public void ReleaseBall(GameObject throwDestination)
    {
        //print("ball thrown");
        rb.detectCollisions = true;
        rb.constraints = RigidbodyConstraints.None;
        ball.transform.parent = null;

        relativePos = throwDestination.transform.position - ball.transform.position;
        //if throwing to player, throw up 1 unit (otherwise ball is throw to their feet)
        if (throwDestination != GameManager.self.strikeZone)
        {
            relativePos = new Vector3(relativePos.x, relativePos.y + 1, relativePos.z);
        }
        //rb.useGravity = true;
        rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        ball.transform.rotation = rotation;
        rb.AddForce(relativePos.normalized * throwForce);
        StartCoroutine(BallNotHeld());
    }

    private IEnumerator BallNotHeld() {
        yield return new WaitForSeconds(0.1f);
        held = false;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Bat"))
        {
            
            //Ball is randomly hit towards one of the invisible targets on the field
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
            //print("Going to " + target.name);
            // need to look into ballistic velocity
            SetVelocity(BallisticVel(target.transform.position, 15f));
        }
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

    public void BallInHand() {
        rb.velocity = Vector3.zero;
        transform.position = rightHand.transform.position;
        transform.parent = rightHand.transform;
        rb = ball.GetComponent<Rigidbody>();
        rb.detectCollisions = false;
        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        held = true;
    }
}
