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
    [SerializeField] private GameObject currentPlayer;
    [SerializeField] private GameObject throwingHand;
    [SerializeField] private Collider catcherCollider;

    private Animator anim;
    private Transform trans;
    private Rigidbody rb;

    private GameObject strikeZone;
    private GameObject firstBase;
    private GameObject secondBase;
    private GameObject thirdBase;
    private GameObject baseball;
    
    private GameObject bat;

    private GameObject throwHere;

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
        rb = GetComponent<Rigidbody>();

        myTeamManager = transform.parent.GetComponent<TeamManager>();
        if (!myTeamManager)
            Debug.LogError("No manager");

        //Get position of each base
        foreach(Transform position in GameManager.self.fieldPositions)
        {
            if(position.name.Equals("First Base"))
                firstBase = position.gameObject;
            else if(position.name.Equals("Second Base"))
                secondBase = position.gameObject;   
            else if(position.name.Equals("Third Base"))
                thirdBase = position.gameObject; 
        }

        /*
        //give each fielding player access to their throwing hand
        //handled in inspector
        if (myTeamManager.role == TeamManager.TeamRole.FIELDING)
        {
            foreach(Transform component in currentPlayer.transform)
            {
                if (component.name.Equals("mixamorig:RightHand"))
                    throwingHand = component.gameObject;
            }
        }
        */

        strikeZone = GameObject.FindGameObjectWithTag("StrikeZone");
        batter = GameObject.FindGameObjectWithTag("Batter");

        //Make all players face the batter, except the batter who faces the pitcher
        Vector3 relativePos;
        Quaternion rotation;
        if (currentPlayer.name.Equals("Batting") || currentPlayer.name.Equals("Catcher"))
        {
            relativePos = pitcher.transform.position - trans.position;  //direction: batter -> pitcher
        }
        else if (currentPlayer.name.Equals("Pitcher"))
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
       
        //Tell ball this is the catcher
        if (currentPlayer.name.Equals("Catcher"))
        {
            GameManager.self.baseball.GetComponent<BaseballScript>().catcher = currentPlayer;
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
                    GameManager.self.baseball = baseball;
                }
            }
        }
        else if (currentPlayer.name.Equals("Catcher"))
        {
            anim.Play("Catcher Idle");
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
        
        if (GameManager.self.currentGameState == GameManager.GameStates.BallInPlay)
        {
            if (myTeamManager.role == TeamManager.TeamRole.BATTING && currentPlayer == myTeamManager.playersOnTeam[0])
            {
                anim.SetBool("Run", true);
                runToBase();
                //print("Batter hit ball");
            }
            else if (myTeamManager.role == TeamManager.TeamRole.FIELDING)
            {
                RunToBall();
            }
        }

        //RunToBall();
	}

    public void pitch()
    {
        GameManager.self.baseball.GetComponent<BaseballScript>().ReleaseBall(GameManager.self.strikeZone);
        GameManager.self.currentGameState = GameManager.GameStates.BallPitched;
    }

    public void hit()
    {
        bat.GetComponent <BatScript> ().ReleaseBat();
        anim.ResetTrigger("Swing");
    }

    public void runToBase()
    {
        //Find which base to run to based on which bases have already been reached
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
            trans.position = Vector3.MoveTowards(trans.position, basePosition, step);
            //Turns instantly towards next base, finding correct rotation to turn smoothly gave wrong direction
            trans.LookAt(basePosition);
            //trans.rotation = Quaternion.RotateTowards(trans.rotation, rotation, Time.deltaTime * 100f);
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

    public void CatchBall() {
        //if (catcherCollider)
           //catcherCollider.enabled = false;

        anim.SetTrigger("Catch");
        StartCoroutine(ParentBallToHand());
    }

    private IEnumerator ParentBallToHand() {
        float waitTime = 0.3f;
        //print(name + " has the ball");
        //Index 1 for the fielding teams' players is the catcher
        // TODO check if game state is pitching, (if ball is in play, don't throw to pitcher)
        if (myTeamManager.role == TeamManager.TeamRole.FIELDING && currentPlayer == myTeamManager.playersOnTeam[1])
        {
            //If catcher has the ball, state is returning ball to pitcher and wait slightly longer to throw
            GameManager.self.currentGameState = GameManager.GameStates.ResetBall;
            waitTime = 0.7f;
            //print("reset ball and waiting");
        }
        
        //GameManager.self.baseball.transform.parent = throwingHand.transform;
        GameManager.self.baseball.GetComponent<BaseballScript>().rightHand = throwingHand;
        GameManager.self.baseball.GetComponent<BaseballScript>().BallInHand();

        yield return new WaitForSeconds(waitTime);
        //print("wait time up, throwing");
        anim.SetTrigger("Throw");
    }

    private void ThrowToPitcher() {
        //print("throwing to pitcher");
        GameManager.self.baseball.GetComponent<BaseballScript>().ReleaseBall(myTeamManager.playersOnTeam[0]);
    }

    private void PitchWhenReady() {
        if (GameManager.self.currentGameState == GameManager.GameStates.ReadyToPitch)
        {
            anim.SetTrigger("Pitch");
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject == GameManager.self.baseball)
        {
            if (!myTeamManager)
                return;

            if (myTeamManager.role == TeamManager.TeamRole.FIELDING && !GameManager.self.baseball.GetComponent<BaseballScript>().held)
            {
                //If pitcher catches ball set new game state
                if (currentPlayer == myTeamManager.playersOnTeam[0])
                    GameManager.self.currentGameState = GameManager.GameStates.ReadyToPitch;

                if (currentPlayer == myTeamManager.playersOnTeam[1])
                    GameManager.self.currentGameState = GameManager.GameStates.ResetBall;

                GameManager.self.baseball.GetComponent<BaseballScript>().rightHand = throwingHand;
                GameManager.self.baseball.GetComponent<BaseballScript>().BallInHand();
                CatchBall();
            }
            
        }
    }


    private void RunToBall()
    {
        float shortestDist = 10000f;
        int savedIndex = 0;

        GameObject target = GameManager.self.baseball.GetComponent<BaseballScript>().target;


        Vector3 targetPosition = target.transform.position;

        if (target.name.Equals("Target4") || target.name.Equals("Target7")) //in theory this should not have someone go after the ball if it is going to be a homerun 
        {
            return;
        }
        else
        {

            for (int x = 0; x < myTeamManager.playersOnTeam.Length; x++)
            {
                GameObject player = myTeamManager.playersOnTeam[x];
                float distanceCheck = Vector3.Distance(targetPosition, player.transform.position);

                if (distanceCheck < shortestDist)
                {
                    shortestDist = distanceCheck;
                    savedIndex = x;
                    //print(savedIndex);
                }
            }
            //print(savedIndex);
            GameObject goingToCatch = myTeamManager.playersOnTeam[savedIndex];



            Vector3 ballPos = GameManager.self.baseball.transform.position;
            //Ball's position with y position equal to the catching player's position to stop them trying to run up
            Vector3 ballPosAdjusted = new Vector3(ballPos.x, goingToCatch.transform.position.y, ballPos.z);

            if (Vector3.Distance(goingToCatch.transform.position, ballPosAdjusted) > radiusOfSatisfaction && currentPlayer == goingToCatch)
            {

                if (!target.name.Equals("Target8") || !target.name.Equals("Target9"))
                {
                    goingToCatch.transform.position = Vector3.MoveTowards(goingToCatch.transform.position, ballPosAdjusted, 5f * Time.deltaTime);
                }
                else
                {
                    goingToCatch.transform.position = Vector3.MoveTowards(goingToCatch.transform.position, ballPosAdjusted, 5f * Time.deltaTime);
                }
                //Turns instantly towards ball, finding correct rotation to turn smoothly gave wrong direction
                trans.LookAt(ballPosAdjusted);


                float distanceToBall = Vector3.Distance((GameManager.self.baseball.transform.position), goingToCatch.transform.position);



                if (distanceToBall < 2f)
                {
                    CatchBall();
                    return;
                }

                //I tried using ThrowToPitcher but it made the ball go crazy


                //function call for the throw
                figureWhereToThrow();
            }
        }
        
    }

    private void figureWhereToThrow()
    {
        

        PlayerController runner = myTeamManager.otherTeam.playersOnTeam[0].GetComponent<PlayerController>();
        if (!runner.firstBaseVisited && !runner.secondBaseVisited && !runner.thirdBaseVisited)
            throwHere = myTeamManager.playersOnTeam[2];
        else if (runner.firstBaseVisited && !runner.secondBaseVisited && !runner.thirdBaseVisited)
            throwHere = myTeamManager.playersOnTeam[3];
        else if (runner.firstBaseVisited && runner.secondBaseVisited && runner.thirdBaseVisited)
            throwHere = myTeamManager.playersOnTeam[4];
        else
        {
            //throwHere = Vector3.zero;
            Debug.LogError("I don't know who to throw to");
        }
        GameManager.self.baseball.GetComponent<BaseballScript>().ReleaseBall(throwHere);
    }

}
