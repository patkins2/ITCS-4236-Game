using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        BATTER, ONDECK, BENCH, INVALID, RUNNER_FIRST, RUNNER_SECOND, RUNNER_THIRD
    }
    public Position myPosition = Position.INVALID;  //default before position is set


    public bool ballHit = false;
    public GameObject leftHand;

    [SerializeField] private GameObject batter;
    [SerializeField] private GameObject pitcher;
    [SerializeField] private GameObject currentPlayer;
    [SerializeField] public GameObject throwingHand;
    [SerializeField] private Collider catcherCollider;
    [SerializeField] private Camera actionCamera;

    private Animator anim;
    private Transform trans;
    private List<GameObject> batters;
    private List<GameObject> runners;
    //private Rigidbody rb;

    private GameObject strikeZone;
    private GameObject firstBase;
    private GameObject secondBase;
    private GameObject thirdBase;
    private GameObject homeBase;
    private GameObject baseball;
    private GameObject onDeck;
    private GameObject onDeckPosition;
    private GameObject runnerFirst;
    private GameObject runnerSecond;
    private GameObject runnerThird;
    private GameObject runningHome;

    
    private GameObject bat;

    private TeamManager myTeamManager;
    
    public bool firstBaseVisited = false;
    public bool secondBaseVisited = false;
    public bool thirdBaseVisited = false;
    public bool reachedHome = false;
    private bool keepRunning = true;
    private bool arrivedAtBase = false;
    private float radiusOfSatisfaction = 1.25f;     //distance to destination when they can stop the running animation and start slowing down
	
    // Use this for initialization
	void Start () {
        //actionCamera.enabled = false;
        currentPlayer = this.gameObject;
        trans = transform;
        anim = GetComponent<Animator>();
        batters = new List<GameObject>();
        runners = new List<GameObject>();
        //rb = GetComponent<Rigidbody>();
        

        myTeamManager = transform.parent.GetComponent<TeamManager>();
        if (!myTeamManager)
            Debug.LogError("No manager");

        //Get position of each base
        foreach(Transform position in GameManager.self.fieldPositions)
        {
            if (position.name.Equals("First Base"))
                firstBase = position.gameObject;
            else if (position.name.Equals("Second Base"))
                secondBase = position.gameObject;
            else if (position.name.Equals("Third Base"))
                thirdBase = position.gameObject;
            else if (position.name.Equals("Batting"))
                homeBase = position.gameObject;
            else if (position.name.Equals("Batter On Deck"))
                onDeckPosition = position.gameObject;
        }

        strikeZone = GameObject.FindGameObjectWithTag("StrikeZone");
        batter = GameObject.FindGameObjectWithTag("Batter");

        //Make all players face the batter, except the batter who faces the pitcher
        Vector3 relativePos;
        Quaternion rotation;

        if (currentPlayer.name.Contains("Bench"))
        {
            batters.Add(currentPlayer);
        }
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
                    GameManager.self.ballScript = baseball.GetComponent<BaseballScript>();
                    GameManager.self.playerWithBall = this.gameObject;
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
                    GameManager.self.bat = bat;
                    GameManager.self.batRB = bat.GetComponent<Rigidbody>();
                    //GameManager.self.initialBatRotation = bat.transform.rotation.eulerAngles;
                }
            }
            anim.Play("Baseball Idle");
        }
        else if(currentPlayer.name.Equals("Batter On Deck"))
        {
            anim.Play("Batter On Deck");
            onDeck = currentPlayer;
        }
   
    }
	
	void Update () {
        //When player right clicks, the ball is pitched to batter
        if (Input.GetMouseButtonDown(1)) {
            if (myTeamManager.role == TeamManager.TeamRole.FIELDING && currentPlayer == myTeamManager.playersOnTeam[0]) {
                anim.SetTrigger("Pitch");    
            }  
        }

        if (Input.GetButtonDown("Don't Run"))
        {
            keepRunning = false;
            Debug.Log("R was pressed");
        }

        //when player left clicks, batter swings and starts running
        if (Input.GetMouseButtonDown(0)) {
            if (myTeamManager.role == TeamManager.TeamRole.BATTING && currentPlayer == myTeamManager.playersOnTeam[0]) {
                anim.SetTrigger("Swing");
            }
        }

        if (GameManager.self.currentGameState == GameManager.GameStates.BallInPlay)
        {
            if (myTeamManager.role == TeamManager.TeamRole.BATTING && currentPlayer == myTeamManager.playersOnTeam[0])
            {
                
                runToBase();
                //print("Batter hit ball");
            }
            else if (myTeamManager.role == TeamManager.TeamRole.FIELDING)
            {
                RunToBall();
            }
        }
        else if (GameManager.self.currentGameState == GameManager.GameStates.ResetBall)
            ReturnToPosition();
	}

    public void pitch()
    {
        GameManager.self.playerWithBall = null;
        GameManager.self.baseball.GetComponent<BaseballScript>().ReleaseBall(GameManager.self.strikeZone);
        GameManager.self.currentGameState = GameManager.GameStates.BallPitched;
    }

    public void hit()
    {
        actionCamera.enabled = true;
        actionCamera.GetComponent<BaseballCameraScript>().followBall(true);
        GameManager.self.playerWithBall = null;
        bat.GetComponent <BatScript> ().ReleaseBat();
        anim.ResetTrigger("Swing");
    }

    public void positionRunnerOnBase(GameObject currentBase)
    {
        float step = 5f * Time.deltaTime;
        anim.SetTrigger("Walk Past Base");
        Vector3 basePosition;
        basePosition = currentBase.transform.position;
        Vector3 offset;
        if (currentBase.name.Equals("First Base")){  
            offset = new Vector3(2.0f, 0, -2.2f);
        }else if(currentBase.name.Equals("Second Base"))
        {
           
            offset = new Vector3(3.2f, 0, 2.2f);
        }
        else
        {
            offset = new Vector3(-2.8f, 0, 2.5f);
        }
        basePosition = basePosition - offset;
        while((basePosition - trans.position).magnitude > radiusOfSatisfaction)
        {
            basePosition = new Vector3(basePosition.x, trans.position.y, basePosition.z);
            trans.position = Vector3.MoveTowards(trans.position, basePosition, step);
            trans.LookAt(basePosition);
        }
        anim.ResetTrigger("Walk Past Base");
        anim.SetTrigger("Stop Walking");
    }

    public void runToBase()
    {
        //if runner is told to stop running and is currently on a base
        if(!keepRunning && arrivedAtBase)
        {
            GameObject currentBase;
            if (firstBaseVisited && !secondBaseVisited && !thirdBaseVisited)
            {
                currentBase = firstBase;
            }
            else if(firstBaseVisited && secondBaseVisited && !thirdBaseVisited)
            {
                currentBase = secondBase;
            }
            else
            {
                currentBase = thirdBase;
            }
            print("stopping running");
            anim.SetBool("Run", false);
            //positionRunnerOnBase(currentBase);
            return;
        }

        if (GameManager.self.currentGameState == GameManager.GameStates.ResetBall || GameManager.self.currentGameState == GameManager.GameStates.ReadyToPitch)
        {
            ReturnToPosition();
            return;
        }
            
        //Find which base to run to based on which bases have already been reached
        Vector3 basePosition;
        float step = 5f * Time.deltaTime;
        if (!firstBaseVisited && !secondBaseVisited && !thirdBaseVisited)
            basePosition = GameManager.self.basePositions[0];
        else if (firstBaseVisited && !secondBaseVisited && !thirdBaseVisited)
            basePosition = GameManager.self.basePositions[1];
        else if (firstBaseVisited && secondBaseVisited && !thirdBaseVisited)
            basePosition = GameManager.self.basePositions[2];
        //if all bases visited, head towards home
        else if (firstBaseVisited && secondBaseVisited && thirdBaseVisited)
            basePosition = GameManager.self.basePositions[3];
        else
        {
            basePosition = Vector3.zero;
            Debug.LogError("I don't know where I've been or where to go next");
        }

        //If runner is far from base
        if ((basePosition - trans.position).magnitude > radiusOfSatisfaction)
        {
            if (!anim.GetBool("Run"))
                anim.SetBool("Run", true);

            //match base y position to runners y position, stops them from running up
            basePosition = new Vector3(basePosition.x, trans.position.y, basePosition.z);
            trans.position = Vector3.MoveTowards(trans.position, basePosition, step);
            //Turns instantly towards next base, finding correct rotation to turn smoothly gave wrong direction
            trans.LookAt(basePosition);
            //trans.rotation = Quaternion.RotateTowards(trans.rotation, rotation, Time.deltaTime * 100f);
        }
        //Close enough to base to stop running OR go to next base
        else
        {
            if (basePosition == GameManager.self.basePositions[0])
            {
                firstBaseVisited = true;
                if (!keepRunning)
                {
                    arrivedAtBase = true;
                }
            }
            else if (basePosition == GameManager.self.basePositions[1])
            {
                secondBaseVisited = true;
                if (!keepRunning)
                {
                    arrivedAtBase = true;
                }
            }
            else if (basePosition == GameManager.self.basePositions[2])
            {
                thirdBaseVisited = true;
                if (!keepRunning)
                {
                    arrivedAtBase = true;
                }
            }
            //
            else if (basePosition == GameManager.self.basePositions[3])
            {
                GameManager.self.Reset();
            }

        }
            
    }

    public void CatchBall() {
        anim.SetTrigger("Catch");
        //TODO if a baseman, check if they are in front of the runner
        PlayerController runner = myTeamManager.otherTeam.playersOnTeam[0].GetComponent<PlayerController>();

        //first baseman, player not yet arrived
        if (currentPlayer == myTeamManager.playersOnTeam[2] && !runner.firstBaseVisited)
        {
            GameManager.self.currentGameState = GameManager.GameStates.ResetBall;
            print(currentPlayer.name + " has ball before runner arrived");
        }
        //first baseman, player already passed here
        else if (currentPlayer == myTeamManager.playersOnTeam[2] && runner.firstBaseVisited)
        {
            //print(currentPlayer.name + " has ball after runner arrived");
        }
        //second baseman, player not yet arrived
        else if (currentPlayer == myTeamManager.playersOnTeam[3] && !runner.secondBaseVisited)
        {
            GameManager.self.currentGameState = GameManager.GameStates.ResetBall;
            print(currentPlayer.name + " has ball before runner arrived");
        }
        //second baseman, player already passed here
        else if (currentPlayer == myTeamManager.playersOnTeam[3] && runner.secondBaseVisited)
        {
            //print(currentPlayer.name + " has ball after runner arrived");
        }
        //third baseman, player not yet arrived
        else if (currentPlayer == myTeamManager.playersOnTeam[4] && !runner.thirdBaseVisited)
        {
            GameManager.self.currentGameState = GameManager.GameStates.ResetBall;
            //GameManager.self.Reset();
            print(currentPlayer.name + " has ball before runner arrived");
        }
        //third baseman, player already passed here
        else if (currentPlayer == myTeamManager.playersOnTeam[4] && runner.thirdBaseVisited)
        {
            ///print(currentPlayer.name + " has ball after runner arrived");
        }
        else if (currentPlayer == myTeamManager.playersOnTeam[1] && !runner.reachedHome)
        {

        }
        else
        {
            //print(currentPlayer.name + " has ball unfortunately");
            //print("Bases Visited: \nFirst: " + firstBaseVisited + " \nSecond: " + secondBaseVisited + " \nThird: " + thirdBaseVisited);
        }
        StartCoroutine(ParentBallToHand());
    }

    private IEnumerator ParentBallToHand() {
        float waitTime = 0.3f;

        //Index 1 for the fielding team's players is the catcher
        // TODO check if game state is pitching, (if ball is in play, don't throw to pitcher)
        if (myTeamManager.role == TeamManager.TeamRole.FIELDING && (currentPlayer == myTeamManager.playersOnTeam[1] || GameManager.self.currentGameState == GameManager.GameStates.ResetBall))
        {
            //If catcher has the ball, state is returning ball to pitcher and wait slightly longer to throw
            GameManager.self.currentGameState = GameManager.GameStates.ResetBall;
            waitTime = 0.7f;
            //print("reset ball and waiting");
        }
        else
        {
            if (myTeamManager.closestToBall)
                myTeamManager.closestToBall.transform.LookAt(myTeamManager.otherTeam.playersOnTeam[0].transform);
        }
        
        //GameManager.self.baseball.transform.parent = throwingHand.transform;
        GameManager.self.ballScript.rightHand = throwingHand;
        GameManager.self.playerWithBall = currentPlayer;
        GameManager.self.ballScript.BallInHand();

        yield return new WaitForSeconds(waitTime);
        //print("wait time up, throwing");
        anim.SetTrigger("Throw");
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

            if (myTeamManager.role == TeamManager.TeamRole.FIELDING && !GameManager.self.ballScript.held)
            {
                //If pitcher catches ball set new game state
                //if (currentPlayer == myTeamManager.playersOnTeam[0])
                    //GameManager.self.currentGameState = GameManager.GameStates.ReadyToPitch;

                //If catcher catches ball, go to reset ball state
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
        float moveSpeed = 5f * Time.deltaTime;
        int savedIndex = 0;

        //Initially closest fielder should run towards target that ball was hit towards
        GameObject target = GameManager.self.ballScript.target;
        
        //if the ball has hit the ground, start running towards it instead
        if (GameManager.self.ballScript.hitGround)
            target = GameManager.self.baseball;

        Vector3 targetPosition = target.transform.position;

        for (int x = 0; x < myTeamManager.playersOnTeam.Length; x++)
        {
            GameObject player = myTeamManager.playersOnTeam[x];
            float distanceCheck = Vector3.Distance(targetPosition, player.transform.position);

            if (distanceCheck < shortestDist)
            {
                shortestDist = distanceCheck;
                savedIndex = x;
            }
        }

        GameObject goingToCatch = myTeamManager.playersOnTeam[savedIndex];
        myTeamManager.closestToBall = goingToCatch;

        if (currentPlayer == goingToCatch)
            anim.SetBool("Run", true);
        else
        {
            //if this fielder isn't going to catch the ball
            //true if they are in position
            if (ReturnToPosition()) anim.SetBool("Run", false);
        }
            
        //Ball's position with y position equal to the catching player's position to stop them trying to run up
        ///Vector3 ballPosAdjusted = new Vector3(ballPos.x, goingToCatch.transform.position.y, ballPos.z);
        Vector3 ballPosAdjusted = new Vector3(targetPosition.x, goingToCatch.transform.position.y, targetPosition.z);

        if (Vector3.Distance(goingToCatch.transform.position, ballPosAdjusted) > radiusOfSatisfaction && currentPlayer == goingToCatch)
        {

            //Turns instantly towards ball, finding correct rotation to turn smoothly gave wrong direction
            goingToCatch.transform.position = Vector3.MoveTowards(goingToCatch.transform.position, ballPosAdjusted, moveSpeed);
            trans.LookAt(ballPosAdjusted);

            float distanceToBall = Vector3.Distance((GameManager.self.baseball.transform.position), goingToCatch.transform.position);

            if (distanceToBall < 2f)
            {
                GameManager.self.ballScript.BallInHand();
                CatchBall();
                return;
            }
        }
        
    }

    private void figureWhereToThrow()
    {
        PlayerController runner = myTeamManager.otherTeam.playersOnTeam[0].GetComponent<PlayerController>();
        GameObject throwHere = null;

        if ((currentPlayer == myTeamManager.playersOnTeam[0] || GameManager.self.currentGameState == GameManager.GameStates.ResetBall))
            return;

        //if catcher catches the ball, return to pitcher
        if ((currentPlayer == myTeamManager.playersOnTeam[1] || GameManager.self.currentGameState == GameManager.GameStates.ResetBall))
            //&& Vector3.Distance(currentPlayer.transform.position, GameManager.self.baseball.transform.position) < 5f)
        {
            GameManager.self.baseball.GetComponent<BaseballScript>().ReleaseBall(myTeamManager.playersOnTeam[0]);
            return;
        }

        //if runner hasn't yet reached a base, throw to first baseman
        if (!runner.firstBaseVisited && !runner.secondBaseVisited && !runner.thirdBaseVisited)
            throwHere = myTeamManager.playersOnTeam[2];
        //throw to second base
        else if (runner.firstBaseVisited && !runner.secondBaseVisited && !runner.thirdBaseVisited)
            throwHere = myTeamManager.playersOnTeam[3];
        //throw to third base
        else if (runner.firstBaseVisited && runner.secondBaseVisited && !runner.thirdBaseVisited)
            throwHere = myTeamManager.playersOnTeam[4];
        else if (runner.firstBaseVisited && runner.secondBaseVisited && !runner.thirdBaseVisited)
            throwHere = myTeamManager.playersOnTeam[0];
        else
        {
        //    Debug.LogError("I don't know who to throw to");
            
        }

        if (!throwHere)
        {
            //Debug.LogError("No throw target");
            return;
        }

        if (myTeamManager.closestToBall == null)
            myTeamManager.closestToBall = myTeamManager.playersOnTeam[0];

        myTeamManager.closestToBall.transform.LookAt(throwHere.transform);
        
        //Throw ball faster the farther away you are
        float throwForceModifier = Vector3.Distance(trans.position, throwHere.transform.position) * 0.1f;
        //Make sure modifier is greater than 1
        throwForceModifier += (throwForceModifier < 1) ? 1 : 0;
        //Cap modifier at 3x normal speed
        throwForceModifier = (throwForceModifier > 3) ? 3 : throwForceModifier;

        GameManager.self.baseball.GetComponent<BaseballScript>().ReleaseBall(throwHere, throwForceModifier);
        GameManager.self.playerWithBall = null;
    }

    private bool ReturnToPosition() {
        
        int playerIndex = System.Array.IndexOf(myTeamManager.playersOnTeam, currentPlayer);
        Transform correctPosition;

        if (myTeamManager.role == TeamManager.TeamRole.FIELDING)
            correctPosition = GameManager.self.fieldPositions.GetChild(playerIndex);
        else
            correctPosition = GameManager.self.battingPositions.GetChild(playerIndex);

        Vector3 adjustedPosition = new Vector3(correctPosition.position.x, currentPlayer.transform.position.y, correctPosition.position.z);
        if (Vector3.Distance(currentPlayer.transform.position, correctPosition.position) < 1f)
        {
            anim.SetBool("Run", false);
            currentPlayer.transform.position = adjustedPosition;
            //catcher faces pitcher
            if (myTeamManager.role == TeamManager.TeamRole.FIELDING && currentPlayer == myTeamManager.playersOnTeam[1])
                currentPlayer.transform.LookAt(myTeamManager.playersOnTeam[0].transform);
            //fielders face batter, batting team faces pitcher
            currentPlayer.transform.LookAt(myTeamManager.otherTeam.playersOnTeam[0].transform);
            
            //when batter is back in position give them the bat;
            if (myTeamManager.role == TeamManager.TeamRole.BATTING && currentPlayer == myTeamManager.playersOnTeam[0])
                GameManager.self.Reset();
            return true;
        }

        print("Returning " + currentPlayer.name + " to their position");
        currentPlayer.transform.position = Vector3.MoveTowards(currentPlayer.transform.position, correctPosition.position, 5f * Time.deltaTime);
        anim.SetBool("Run", true);
        currentPlayer.transform.LookAt(adjustedPosition);
        //print(currentPlayer.name + " getting back into position");
        return false;
    }

    private void changeBatter()
    {
        if(Vector3.Distance(onDeck.transform.position, homeBase.transform.position) > radiusOfSatisfaction)
            onDeck.transform.position = Vector3.MoveTowards(onDeck.transform.position, homeBase.transform.position, 2.5f);
        if(Vector3.Distance(batters[0].transform.position, onDeck.transform.position) > radiusOfSatisfaction)
            batters[0].transform.position = Vector3.MoveTowards(batters[0].transform.position, onDeck.transform.position, 2.5f);
    }
    

    private void changeRunners()
    {
        if(batters.Count > 0)
        {
            if(runnerThird != null)
            {
                Destroy(runnerThird);
            }

            if(runnerSecond != null)
            {
                runnerThird = runnerSecond;
                runnerThird.name = "Runner on Third";
            }

            if(runnerFirst != null)
            {
                runnerSecond = runnerFirst;
                runnerSecond.name = "Runner on Second";
            }

            runnerFirst = batter;
            runnerFirst.name = "Runner on First";

            batter = onDeck;
            batter.name = "Batting";

            onDeck = batters[0];
            for(int i = 1; i < batters.Count; i++)
            {
                batters[i - 1] = batters[i];
            }

        }
    }

}
