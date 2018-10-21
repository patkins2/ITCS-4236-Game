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

    [SerializeField] private GameObject teamMngrPrefab;
    private GameObject team1, team2;
    public Transform fieldPositions { get; private set; }
    public Transform battingPositions { get; private set; }

    // Use this for initialization
    void Start () {
        team1 = Instantiate(teamMngrPrefab, Vector3.zero, Quaternion.identity);
        team1.name = "Team 1";
        team2 = Instantiate(teamMngrPrefab, Vector3.zero, Quaternion.identity);
        team2.name = "Team 2";

        //Keeps parent Transform of all the batting and fielding positions
        battingPositions = GameObject.Find("Batting Positions").transform;
        fieldPositions = GameObject.Find("Fielding Positions").transform;
    }
}
