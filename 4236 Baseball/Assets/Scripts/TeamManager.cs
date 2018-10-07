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

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private TeamManager otherTeam;
    [SerializeField] private GameObject baseballBat;

    private Transform fieldPositions;
    private int maxPlayers = 9;
	[HideInInspector] public enum TeamRole {fielding, batting};
    public TeamRole role;

    private void Awake() {
        role = TeamRole.fielding;
    }
    // Use this for initialization
    void Start() {

        TeamManager[] teams = FindObjectsOfType<TeamManager>();
        otherTeam = name.Equals(teams[0].gameObject.name) ? teams[1] : teams[0];
    
        print("This team:" + name);
        print("Other team:" + teams[0].gameObject.name);

        //check role of other team
        //if undefined, default to fielding
        if (otherTeam.role == TeamRole.fielding) {
            role = TeamRole.batting;
            fieldPositions = GameObject.Find("Batting Positions").transform;
        }
        else
            fieldPositions = GameObject.Find("Fielding Positions").transform;

		for (int i = 0; i < maxPlayers; i++) {

            //Instantiate player prefab
            GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity, this.transform);
            newPlayer.name = "Player " + (i + 1);

            //Loop through field positions until empty position is found
            for (int c = 0; c < fieldPositions.childCount; c++) {
                FieldPositions position = fieldPositions.GetChild(c).GetComponent<FieldPositions>();
                if (position.name.Equals("Batting")){
                    position.isBatter = true;
                    Instantiate(baseballBat);
                }

                //If empty position is found, move player to position location and break;
                if (!position.positionOccupied) {
                    print("Adding player to position: " + position.gameObject.name);
                    
                    position.positionOccupied = true;
                    newPlayer.transform.position = position.transform.position;
                    break;
                }
            }
            //Assign new player to empty position
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
