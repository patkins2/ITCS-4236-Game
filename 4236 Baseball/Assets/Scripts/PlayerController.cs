using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * PlayerController keeps all information about the player and holds all the logic for their actions
 * 
 */ 
public class PlayerController : MonoBehaviour {

    public enum Position 
    {
        PITCHER, CATCHER,
        BASEMAN_FIRST, BASEMAN_SECOND, BASEMAN_THIRD,
        SHORTSTOP, OUTFIELD_LEFT, OUTFIELD_CENTER, OUTFIELD_RIGHT,
        BATTER, ONDECK, BENCH, INVALID
    }
    public Position myPosition = Position.INVALID;  //default before position is set

    [SerializeField] private GameObject batter;
    [SerializeField] private GameObject pitcher;
    private GameObject strikeZone;

    [SerializeField] private float minSpeed, maxSpeed, minPower, maxPower;
    [SerializeField] private float runSpeed, swingPower;

    private Animator anim;
    private Transform trans;
    private GameObject currentPlayer;
    private GameObject firstBase;
    private GameObject secondBase;
    private GameObject thirdBase;
    private GameObject baseball;
    private GameObject bat;

    private bool running = false;
    private bool firstBaseVisited = false;
    private bool secondBaseVisited = false;
    private bool thirdBaseVisited = false;
    private float radiusOfSatisfaction = 1f;     //distance to destination when they can stop the running animation and start slowing down
	
    // Use this for initialization
	void Start () {
        strikeZone = GameObject.FindGameObjectWithTag("StrikeZone");
        currentPlayer = this.gameObject;
        //trans = currentPlayer.transform;
        trans = transform;
        firstBaseVisited = secondBaseVisited = thirdBaseVisited = false;
        runSpeed = Random.Range(minSpeed, maxSpeed);
        swingPower = Random.Range(minPower, maxPower);
        anim = GetComponent<Animator>();

        Transform[] fieldingPositions = GameObject.Find("Stadium").GetComponentsInChildren<Transform>();
        foreach(Transform position in fieldingPositions)
        {
            if(position.name.Equals("First Base"))
            {
                //print("Found first base");
                firstBase = position.gameObject;
            }
            else if(position.name.Equals("Second Base"))
            {
                //print("Found second base");
                secondBase = position.gameObject;   
            }
            else if(position.name.Equals("Third Base"))
            {
                //print("Found third base");
                thirdBase = position.gameObject; 
            }
        }

        //Make all players face the batter, except the batter who faces the pitcher
        Vector3 relativePos;
        Quaternion rotation;

        if (currentPlayer.name.Equals("Batting"))
        {
            relativePos = pitcher.transform.position - trans.position;  //direction: batter -> pitcher
        }else if (currentPlayer.name.Equals("Pitcher"))
        {
            relativePos = strikeZone.transform.position - trans.position;
            rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            trans.rotation = rotation;
        }
        else
        {
            relativePos = batter.transform.position - trans.position;   //direction: this player -> batter
            rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            trans.rotation = rotation;
        }
        //Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        //trans.rotation = rotation;
       
        
        if (currentPlayer.name.Equals("Pitcher"))   //if this player is the pitcher, find the baseball child component and store a reference to its GameObject
        {
            Transform[] components = currentPlayer.GetComponentsInChildren<Transform>();
            foreach (Transform component in components)
            {
                if (component.name.Equals("Baseball"))
                {
                    baseball = component.gameObject;
                }
            }
        }
        else if (currentPlayer.name.Equals("Batting"))  //if this player is the batter, find the baseball bat child component and store a reference to its GameObject
        {
            Transform[] components = currentPlayer.GetComponentsInChildren<Transform>();
            foreach(Transform component in components)
            {
                if (component.name.Equals("Bat"))
                {
                    bat = component.gameObject;
                }
            }
            anim.Play("Baseball Idle");
        }
        else if(currentPlayer.name.Equals("Batter On Deck"))
        {
            anim.Play("Batter On Deck");
        }
   
    }
	
	void Update () {
        //When player right clicks, the ball is pitched to batter
        if (Input.GetMouseButtonDown(1)) {
            if (currentPlayer.name.Equals("Pitcher")) {
                //Debug.Log("Pitching");
                anim.SetTrigger("Pitch");    
            }  
        }

        //when player left clicks, batter swings and starts running
        if (Input.GetMouseButtonDown(0)) {
            if (currentPlayer.name.Equals("Batting")) {
                //Debug.Log("Batter");
                anim.SetTrigger("Swing");
            }
        }
        
        if (running)
        {
            runToBase();
        }
	}

    public void pitch()
    {
        baseball.GetComponent <BaseballScript> ().ReleaseBall();
        anim.ResetTrigger("Pitch");
            }

    public void hit()
    {
        bat.GetComponent <BatScript> ().ReleaseBat();
        anim.ResetTrigger("Swing");
    }

    public void run()
    {
        anim.SetTrigger("Run");
        running = true;
    }

    private void runToBase()
    {
        Quaternion baseRotation;
        Vector3 basePosition;
        float step = 5f * Time.deltaTime;
        if (!firstBaseVisited && !secondBaseVisited && !thirdBaseVisited)
        {
            //Debug.Log("Running to first");
            baseRotation = firstBase.transform.rotation;
            basePosition = firstBase.transform.position;
            
        }else if(firstBaseVisited && !secondBaseVisited && !thirdBaseVisited)
        {
            //Debug.Log("Running to second");
            baseRotation = secondBase.transform.rotation;
            basePosition = secondBase.transform.position;
        }else if(firstBaseVisited && secondBaseVisited && !thirdBaseVisited)
        {
            //Debug.Log("Running to third");
            baseRotation = thirdBase.transform.rotation;
            basePosition = thirdBase.transform.position;
        }
        else
        {
            baseRotation = new Quaternion();
            basePosition = Vector3.zero;
        }
        
        if((basePosition - trans.position).magnitude > radiusOfSatisfaction)
        {
            running = true;
            Quaternion rotation = Quaternion.LookRotation(basePosition.normalized);
            trans.position = Vector3.MoveTowards(trans.position, basePosition, step);
            trans.rotation = Quaternion.Lerp(trans.rotation, rotation, Time.deltaTime * 10f);
        }
        else
        {
            anim.SetTrigger("No Run");
            anim.ResetTrigger("Run");
            running = false;
        }   
    }
    
   
}
