using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [Header("Base Icons")]
    [SerializeField] private Image firstBaseIcon;
    [SerializeField] private Image secondBaseIcon;
    [SerializeField] private Image thirdBaseIcon;

    [Header("Strikes")]
    [SerializeField] private RawImage strike1;
    [SerializeField] private RawImage strike2;
    [SerializeField] private RawImage strike3;

    [Header("Outs")]
    [SerializeField] private RawImage out1;
    [SerializeField] private RawImage out2;
    [SerializeField] private RawImage out3;

    [Header("Team Scores")]
    [SerializeField] private Image team1;
    [SerializeField] private Image team2;
    [SerializeField] private Image currentInning;
    //TODO Add final (total) scores for each team

    //[Header("Score Boxes")]
    private Image[] inningScores = new Image[18];
    private int currentInningIndex = 0;
    private Text currentInningText;

    // Use this for initialization
    void Start () {
        GetScoreBoxes();
        currentInning.transform.position = inningScores[currentInningIndex].transform.position;
        currentInningText = currentInning.GetComponentInChildren<Text>();
        currentInningText.text = "0";
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0))
            AddStrike();
        if (Input.GetMouseButtonDown(1)) { IncrementScore(); }
            //increase score for current inning by 1
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

    public void AddStrike() {
        if (!strike1.IsActive())
            strike1.gameObject.SetActive(true);
        else if (!strike2.IsActive())
            strike2.gameObject.SetActive(true);
        else if (!strike3.IsActive())
            strike3.gameObject.SetActive(true);
        if (strike3.IsActive())
            StartCoroutine(NextOut());
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

        if (out3.IsActive())
            StartCoroutine(NextInning());
    }
    public void ClearOuts() {
        out1.gameObject.SetActive(false);
        out2.gameObject.SetActive(false);
        out3.gameObject.SetActive(false);
    }


    public void IncrementScore() {
        int newScore = int.Parse(currentInningText.text) + 1;
        currentInningText.text = newScore.ToString();
        inningScores[currentInningIndex].GetComponentInChildren<Text>().text = currentInningText.text;
        //TODO update final score with new total
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

    //testing called when 3 strikes are active -> add out and clear strikes
    private IEnumerator NextOut() {
        
        yield return new WaitForSeconds(0.5f);
        AddOut();
        ClearStrikes();
    }

    //testing called when 3 outs are active -> go to next inning on scoreboard and clear outs
    private IEnumerator NextInning() {
        if (currentInningIndex < 9)
            currentInningIndex += 9;
        else if (currentInningIndex >= 9)
            currentInningIndex -= 8;

        yield return new WaitForSeconds(0.5f);
        ClearOuts();
        currentInningText.text = "0";
        currentInning.transform.position = inningScores[currentInningIndex].transform.position;
    }
}
