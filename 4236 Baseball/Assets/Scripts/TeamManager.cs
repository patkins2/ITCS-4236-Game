using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * TeamManager handles all of the team-based information and actions
 * - Spawning 9(?) players on the team and assigning them positions
 * - Keeps track of location of each player on the field
 * - Knows which role its team is (fielding or batting)
 * - When batting, rotate players from bench, to batting, to running
 * - When fielding, be able to tell AI fielders which player is closest to 
 *      where the ball will land so they don't all run for it
 * - Give each player a new position for the opposite role when the half-inning ends
 * 
 * TODO 
 * -give dummy character a different material depending on which team they're on
 */
public class TeamManager : MonoBehaviour {

    public GameObject closestToBall = null;

    //Generic player prefab for all positions other than batter and pitcher, 
    // since those are the only ones that need a gameobject attached to them (ball and bat)
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject batterPrefab;
    [SerializeField] private GameObject pitcherPrefab;
    [SerializeField] private GameObject catcherPrefab;
    [SerializeField] private Material blueSurfaceMat;
    [SerializeField] private Material blueJointMat;
    [SerializeField] public TeamManager otherTeam;
    [SerializeField] private GameObject baseballBat;

    private const int maxPlayers = 9;
    [SerializeField] public GameObject[] playersOnTeam = new GameObject[maxPlayers];
	[HideInInspector] public enum TeamRole {FIELDING, BATTING};
    public TeamRole role;

    private void Awake() {
        role = TeamRole.FIELDING;
    }

    // Use this for initialization
    void Start() {
        //Set each team to recognize the other team
        TeamManager[] teams = FindObjectsOfType<TeamManager>();
        otherTeam = name.Equals(teams[0].gameObject.name) ? teams[1] : teams[0];
        
        //Print to make sure each team knows who they are and who they're playing against
        print("This team:" + name);
        print("Other team:" + otherTeam.gameObject.name);

        //check role of other team
        //if undefined, default self to fielding
        if (otherTeam.role == TeamRole.FIELDING) {
            role = TeamRole.BATTING;
        }

        AssignPlayersToPositions();
		
	}

    //Checks the current team's role and then assigns players to their proper positions
    private void AssignPlayersToPositions() {
        print("This team's role is " + role.ToString());
        if (role == TeamRole.FIELDING) {
            
            AssignPositions_Fielding();
        }
        else if (role == TeamRole.BATTING) {
            AssignPositions_Batting();
        }
        else {
            Debug.LogError("Team role is unassigned");
        } 
    }

    //Assigns positions to players on the fielding team
    private void AssignPositions_Fielding() {
        //Loop 9 times, each time creating a player and assigning them to their position to play on the field
        for (int i = 0; i < maxPlayers; i++) {

            //Loop through field positions until an empty position is found
            for (int c = 0; c < GameManager.self.fieldPositions.childCount; c++) {

                FieldPositions position = GameManager.self.fieldPositions.GetChild(c).GetComponent<FieldPositions>();

                //If current position is empty, create a player to fill that position
                if (!position.positionOccupied) {
                    //print("Found empty position: " + position.gameObject.name);
                    GameObject newPlayer = null;

                    if (position.name.Equals("Pitcher")) {
                        newPlayer = CreatePlayer(pitcherPrefab, position.name);
                    }
                    else if (position.name.Equals("Catcher")) {
                        newPlayer = CreatePlayer(catcherPrefab, position.name);
                    }
                    else if (position.name.Equals("First Base")) {
                        newPlayer = CreatePlayer(playerPrefab, position.name);
                    }
                    else if (position.name.Equals("Second Base")) {
                        newPlayer = CreatePlayer(playerPrefab, position.name);
                    }
                    else if (position.name.Equals("Third Base")) {
                        newPlayer = CreatePlayer(playerPrefab, position.name);
                    }
                    else if (position.name.Equals("Short Stop")) {
                        newPlayer = CreatePlayer(playerPrefab, position.name);
                    }
                    else if (position.name.Equals("Left Field")) {
                        newPlayer = CreatePlayer(playerPrefab, position.name);
                    }
                    else if (position.name.Equals("Right Field")) {
                        newPlayer = CreatePlayer(playerPrefab, position.name);
                    }
                    else if (position.name.Equals("Center Field")) {
                        newPlayer = CreatePlayer(playerPrefab, position.name);
                    }
                    else {  //if position is found that isn't a fielding position, error
                        Debug.LogError("Fielding Position called \"" + position.name + "\" not found");
                    }

                    
                    //if new player was created, move them to their playing position
                    if (newPlayer)
                    {
                        position.positionOccupied = true;
                        playersOnTeam[i] = newPlayer;
                        newPlayer.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = blueJointMat;
                        newPlayer.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material = blueSurfaceMat;
                        newPlayer.transform.position = position.transform.position;
                    }
                    break;
                }
            }
            
        }
        GameManager.self.TeamFinishedSpawning();
    }

    //Assigns players on the batting team to their proper positions
    private void AssignPositions_Batting() {
        //Loop 9 times, each time creating a player and their position to play on the field
        for (int i = 0; i < maxPlayers; i++) {

            //Loop through field positions until empty position is found
            for (int c = 0; c < GameManager.self.battingPositions.childCount; c++) {

                FieldPositions position = GameManager.self.battingPositions.GetChild(c).GetComponent<FieldPositions>();

                //If current position is empty, create a player to fill that position
                if (!position.positionOccupied) {
                    //print("Found empty position: " + position.gameObject.name);
                    GameObject newPlayer = null;
                    
                    if (position.name.Equals("Batting")) {
                        newPlayer = CreatePlayer(batterPrefab, position.name);
                    }
                    else if (position.name.Equals("Batter On Deck")) {
                        newPlayer = CreatePlayer(batterPrefab, position.name);
                    }
                    else if (position.name.Equals("Bench")) {
                        newPlayer = CreatePlayer(playerPrefab, position.name);
                        newPlayer.name += " " + (i-1).ToString();   //add a number to the player's name to differentiate the benched players
                    }
                    else {  //if position is found that isn't a batting position, error
                        Debug.LogError("Batting position called \"" + position.name + "\" not found");
                    }

                    //if new player was created, move them to their playing position
                    if (newPlayer)
                    {
                        position.positionOccupied = true;
                        playersOnTeam[i] = newPlayer;
                        newPlayer.transform.position = position.transform.position;
                    }
                    break;
                }
            }
            if (i == playersOnTeam.Length - 1)
            {
                GameManager.self.TeamFinishedSpawning();
            }
        }
    }

    private GameObject CreatePlayer(GameObject playerPrefab, string positionName) {
        GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity, this.transform);
        newPlayer.name = positionName;

        //Access controller script of the new player to assign their position
        PlayerController playerController = newPlayer.GetComponent<PlayerController>();

        //Tells new player what position they play
        //TODO move enum from PlayerController to either GameManager or an enum class
        switch (positionName) {
            case ("Pitcher"):
                playerController.myPosition = PlayerController.Position.PITCHER;
                break;
            case "Catcher":
                playerController.myPosition = PlayerController.Position.CATCHER;
                break;
            case ("First Base"):
                playerController.myPosition = PlayerController.Position.BASEMAN_FIRST;
                break;
            case "Second Base":
                playerController.myPosition = PlayerController.Position.BASEMAN_SECOND;
                break;
            case ("Third Base"):
                playerController.myPosition = PlayerController.Position.BASEMAN_THIRD;
                break;
            case "Short Stop":
                playerController.myPosition = PlayerController.Position.SHORTSTOP;
                break;
            case ("Left Field"):
                playerController.myPosition = PlayerController.Position.OUTFIELD_LEFT;
                break;
            case "Center Field":
                playerController.myPosition = PlayerController.Position.OUTFIELD_CENTER;
                break;
            case ("Right Field"):
                playerController.myPosition = PlayerController.Position.OUTFIELD_RIGHT;
                break;
            case "Batting":
                playerController.myPosition = PlayerController.Position.BATTER;
                break;
            case "Batter On Deck":
                playerController.myPosition = PlayerController.Position.ONDECK;
                break;
            case "Bench":
                playerController.myPosition = PlayerController.Position.BENCH;
                break;
            default:
                playerController.myPosition = PlayerController.Position.INVALID;
                Debug.LogError("Bad Position Assignment");
                break;
        }

        return newPlayer;
    }

    public GameObject SpawnBatter() {
        GameObject batter = null;
        if (role == TeamRole.BATTING)
        {
            batter = CreatePlayer(batterPrefab, "Batting");
            playersOnTeam[0] = batter;
            batter.transform.SetSiblingIndex(0);
            batter.transform.Translate(new Vector3(-1, -1, 0), Space.Self);
        }
            
        return batter;
    }
}
