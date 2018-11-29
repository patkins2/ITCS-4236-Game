using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [Header("Base Icons")]
    [SerializeField] Image firstBaseIcon;
    [SerializeField] Image secondBaseIcon;
    [SerializeField] Image thirdBaseIcon;

    [Header("Score Boxes")]
    [SerializeField] Image[] inningScores = new Image[18];

    [Header("Team Scores")]
    [SerializeField] Image team1;
    [SerializeField] Image team2;

    [Header("Strikes")]
    [SerializeField] RawImage strike1;
    [SerializeField] RawImage strike2;
    [SerializeField] RawImage strike3;

    [Header("Outs")]
    [SerializeField] RawImage out1;
    [SerializeField] RawImage out2;
    [SerializeField] RawImage out3;

    // Use this for initialization
    void Start () {
        GetScoreBoxes();
        
    }

    private void Update() {
        if (Input.GetMouseButtonDown(1))
            AddOut();
        if (Input.GetMouseButtonDown(0))
            AddStrike();
    }
    private void GetScoreBoxes() {
        int i = 0;
        foreach (Transform childImage in team1.transform)
        {
            if (childImage.GetComponent<Image>())
            {
                //print("found image");
                inningScores[i++] = childImage.GetComponent<Image>();
            }
            else
                print("not image on T1");
        }
        foreach (Transform childImage in team2.transform)
        {
            if (childImage.GetComponent<Image>())
            {
                //print("found image");
                inningScores[i++] = childImage.GetComponent<Image>();
            }
            else
                print("not image on T2");
        }
        if (i != 18)
            Debug.LogError("Invalid number of score boxes in scoreboard");
    }
    public void PlayerOnBase(int baseNum) {
        switch (baseNum)
        {
            //first base
            case 1:
                firstBaseIcon.color = Color.yellow;
                break;
            //second base
            case 2:
                secondBaseIcon.color = Color.yellow;
                break;
            //third base
            case 3:
                thirdBaseIcon.color = Color.yellow;
                break;
            default:
                Debug.LogError("Player on invalid base number: " + baseNum);
                break;
        }
    }

    public void PlayerOffBase(int baseNum) {
        switch (baseNum)
        {
            //first base
            case 1:
                firstBaseIcon.color = Color.white;
                break;
            //second base
            case 2:
                secondBaseIcon.color = Color.white;
                break;
            //third base
            case 3:
                thirdBaseIcon.color = Color.white;
                break;
            default:
                Debug.LogError("Player leaving invalid base number: " + baseNum);
                break;
        }
    }

    public void IncrementScore(int teamNum, int score) {

    }

    public void AddStrike() {
        if (!strike1.IsActive())
            strike1.gameObject.SetActive(true);
        else if (!strike2.IsActive())
            strike2.gameObject.SetActive(true);
        else if (!strike3.IsActive())
            strike3.gameObject.SetActive(true);
    }

    public void ClearStrikes() {
        strike1.gameObject.SetActive(false);
        strike2.gameObject.SetActive(false);
        strike3.gameObject.SetActive(false);
    }

    public void AddOut() {
        if (!out1.IsActive())
            out1.gameObject.SetActive(true);
        else if (!out2.IsActive())
            out2.gameObject.SetActive(true);
        else if (!out3.IsActive())
            out3.gameObject.SetActive(true);
        else
        {
            Debug.LogError("Too many outs");
            return;
        }
        //ClearStrikes(); //have gamemanager check and call to clear strikes when there is an out
    }

    public void ClearOuts() {
        out1.gameObject.SetActive(false);
        out2.gameObject.SetActive(false);
        out3.gameObject.SetActive(false);
    }
}
