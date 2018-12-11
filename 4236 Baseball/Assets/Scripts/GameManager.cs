using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * GameManager handles everything not addressed by smaller scope scripts
 *  - Starting and ending the baseball game
 *  - Keep track of strikes, outs, innings, etc.
 *  - Knows when a run is scored
 *  - Music
 * 
 */
public class GameManager : MonoBehaviour {

    #region Singleton
    public static GameManager self;
    private void Awake() {
        if (self == null)
            self = this;
        else
            Destroy(gameObject);
    }
    #endregion

    public enum GameStates
    {
        Pregame, ReadyToPitch, Pitching, BallPitched,
        ResetBall, BallInPlay
    }
    public GameStates currentGameState = GameStates.Pregame;

    public enum Bases
    {
        First, Second, Third, Home
    }

    //Increment when each team makes its last player
    //Used to know when the game can begin
    public int numTeamsCreated = 0;

    public Transform battingPositions { get; private set; }
    public Transform fieldPositions { get; private set; }
    public Vector3[] basePositions = new Vector3[4];
    public GameObject playerWithBall;
    public GameObject strikeZone { get; private set; }
    public GameObject baseball { get; set; }
    public GameObject bat;
    public BaseballScript ballScript;
    //public Vector3 initialBatRotation = new Vector3(155, -98,-88);

    [HideInInspector] public Rigidbody batRB;

    [SerializeField] private GameObject teamMngrPrefab;

    private GameObject team1, team2;
    private bool returningBallToPitcher = false;
    private bool batterReady = false;

    // Use this for initialization
    void Start () {
        team1 = Instantiate(teamMngrPrefab, Vector3.zero, Quaternion.identity);
        team1.name = "Team 1";
        team2 = Instantiate(teamMngrPrefab, Vector3.zero, Quaternion.identity);
        team2.name = "Team 2";

        //Keeps parent Transform of all the batting and fielding positions
        battingPositions = GameObject.Find("Batting Positions").transform;
        fieldPositions = GameObject.Find("Fielding Positions").transform;
        //first, second, third, home
        basePositions[0] = fieldPositions.GetChild(2).position;
        basePositions[1] = fieldPositions.GetChild(3).position;
        basePositions[2] = fieldPositions.GetChild(4).position;
        basePositions[3] = new Vector3(fieldPositions.GetChild(1).position.x, fieldPositions.GetChild(1).position.y, 0);

        strikeZone = GameObject.FindGameObjectWithTag("StrikeZone");
    }

    private void Update() {
        Reset();
    }

    public void TeamFinishedSpawning() {
        numTeamsCreated++;
        if (numTeamsCreated >= 2)
        {
            currentGameState = GameStates.ReadyToPitch;
            //ballScript = baseball.GetComponent<BaseballScript>();
        }
    }

    public void Reset() {

        if (playerWithBall != null)
            baseball.transform.localPosition = Vector3.zero;

        if ( currentGameState == GameStates.ResetBall)
        {
            //print("GM resetting");
            //ResetBatter();
            ResetTeam();
            StartCoroutine(ResetBall());
        }
    }

    //Called by the batter's player scipt when he has returned to position
    public void ResetBatter() {
        PlayerController batter = team1.GetComponent<TeamManager>().playersOnTeam[0].GetComponent<PlayerController>();
        Destroy(batter.gameObject);
        batter = team1.GetComponent<TeamManager>().SpawnBatter().GetComponent<PlayerController>();
        //team1.GetComponent<TeamManager>().playersOnTeam[0] = batter;
        batter.firstBaseVisited = false;
        batter.secondBaseVisited = false;
        batter.thirdBaseVisited = false;
        batterReady = true;
    }

    private void ResetTeam() {
        team1.GetComponent<TeamManager>().closestToBall = null;
        team2.GetComponent<TeamManager>().closestToBall = null;
        
        foreach (GameObject player in team2.GetComponent<TeamManager>().playersOnTeam)
        {
            //if this player has the ball, don't idle, they need to throw it back to pitcher
            if (player == playerWithBall)
                continue;

            if (player == team2.GetComponent<TeamManager>().playersOnTeam[1])
                player.GetComponent<Animator>().Play("Catcher Idle");
            else
                player.GetComponent<Animator>().Play("Idle");
        }
    }

    private IEnumerator ResetBall() {
        if (baseball)
        {
            GameObject pitcher = team2.GetComponent<TeamManager>().playersOnTeam[0];
            Vector3 pitcherPos = pitcher.transform.position;
            Vector3 catcherPos = team2.GetComponent<TeamManager>().playersOnTeam[0].transform.position;
            //ball is reset by players unless hit out of bounds
            if ((baseball.transform.position.y < -5f && !ballScript.held))
            {
                yield return new WaitForSeconds(0.4f);
                Rigidbody ballRB = baseball.GetComponent<Rigidbody>();
                ballRB.velocity = Vector3.zero;
                GameObject hand = team2.GetComponent<TeamManager>().playersOnTeam[0].GetComponent<PlayerController>().throwingHand;
                baseball.transform.parent = hand.transform;
                baseball.transform.localPosition = Vector3.zero;
                ballRB.constraints = RigidbodyConstraints.None;
                ballRB.useGravity = false;
                ballRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                ballScript.held = true;
                
                ballScript.target = strikeZone;
            }
            Animator pwbAnim = playerWithBall.GetComponent<Animator>();
            //print("Throwing?: " + pwbAnim.GetCurrentAnimatorStateInfo(0).IsName("Throw"));
            //print("Catching?: " + pwbAnim.GetCurrentAnimatorStateInfo(0).IsName("Catch"));
            if (playerWithBall != pitcher && ballScript.held && !returningBallToPitcher)// && 
                //(pwbAnim.GetCurrentAnimatorStateInfo(0).IsName("Throw") || pwbAnim.GetCurrentAnimatorStateInfo(0).IsName("Catch")))
            {
                //print(playerWithBall.name + " has the ball");
                //baseball.transform.parent = pitcher.GetComponent<PlayerController>().throwingHand.transform;
                pwbAnim.SetTrigger("Throw");
                //playerWithBall = pitcher;
                //baseball.transform.localPosition = Vector3.zero;
                returningBallToPitcher = true;
            }

            if (playerWithBall == pitcher && batterReady)
            {
                currentGameState = GameStates.ReadyToPitch;
                returningBallToPitcher = false;
                batterReady = false;
                ballScript.hitGround = false;
            }
                
        }
    }

}
