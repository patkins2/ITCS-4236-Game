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


    public bool ballHit = false;

    [SerializeField] private GameObject batter;
    [SerializeField] private GameObject pitcher;

    private Animator anim;
    private Transform trans;
    private GameObject currentPlayer;
    private GameObject strikeZone;
    private GameObject firstBase;
    private GameObject secondBase;
    private GameObject thirdBase;
    private GameObject baseball;
    private GameObject bat;

    private TeamManager myTeamManager;
    
    private bool firstBaseVisited = false;
    private bool secondBaseVisited = false;
    private bool thirdBaseVisited = false;
    private float radiusOfSatisfaction = 1.25f;     //distance to destination when they can stop the running animation and start slowing down
	
    // Use this for initialization
	void Start () {
        currentPlayer = this.gameObject;
        trans = transform;
        anim = GetComponent<Animator>();

        myTeamManager = transform.parent.GetComponent<TeamManager>();
        if (!myTeamManager)
            Debug.LogError("No manager");

        foreach(Transform position in GameManager.self.fieldPositions)
        {
            if(position.name.Equals("First Base"))
                firstBase = position.gameObject;
            else if(position.name.Equals("Second Base"))
                secondBase = position.gameObject;   
            else if(position.name.Equals("Third Base"))
                thirdBase = position.gameObject; 
        }

        strikeZone = GameObject.FindGameObjectWithTag("StrikeZone");

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

            if (trans.rotation.eulerAngles == Vector3.zero)
                Debug.LogError(name + " viewing self");
        }
       
        //Set players' animations based on their position
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
                    bat.GetComponent<BatScript>().batter = currentPlayer.GetComponent<PlayerController>();
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
        
        if (ballHit)
        {
            anim.SetBool("Run", true);
            runToBase();
            print("Batter hit ball");  
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

    private void runToBase()
    {
        //Find which base to run to
        Vector3 basePosition;
        float step = 5f * Time.deltaTime;
        if (!firstBaseVisited && !secondBaseVisited && !thirdBaseVisited)
            basePosition = firstBase.transform.position;
        else if (firstBaseVisited && !secondBaseVisited && !thirdBaseVisited)
            basePosition = secondBase.transform.position;
        else if (firstBaseVisited && secondBaseVisited && !thirdBaseVisited)
            basePosition = thirdBase.transform.position;
        else
        {
            basePosition = Vector3.zero;
            Debug.LogError("I don't know where I've been or where to go next");
        }

        //If runner is far from base
        if ((basePosition - trans.position).magnitude > radiusOfSatisfaction)
        {
            //Turn to face destination base
            Quaternion rotation = Quaternion.LookRotation(basePosition);
            print("Moving towards " + basePosition);
            print("Turning towards " + rotation.eulerAngles);
            trans.position = Vector3.MoveTowards(trans.position, basePosition, step);
            trans.LookAt(basePosition);
            //trans.rotation = rotation;//Quaternion.RotateTowards(trans.rotation, rotation, Time.deltaTime * 100f);
        }
        //Close enough to base to stop running OR go to next base
        else
        {
            if (basePosition == firstBase.transform.position)
                firstBaseVisited = true;
            else if (basePosition == secondBase.transform.position)
                secondBaseVisited = true;
            else if (basePosition == thirdBase.transform.position)
                thirdBaseVisited = true;
        }
            
    }
    
   
}
