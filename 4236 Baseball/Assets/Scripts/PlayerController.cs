using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * PlayerController keeps all information about the player and holds all the logic for their actions
 * 
 */ 
public class PlayerController : MonoBehaviour {

    [SerializeField] private float minSpeed, maxSpeed, minPower, maxPower;
    [SerializeField] private float runSpeed, swingPower;

    private GameObject currentPlayer;
    [SerializeField] private GameObject batter;
    [SerializeField] private GameObject pitcher;


    private GameObject firstBase;
    private GameObject secondBase;
    private GameObject thirdBase;

    private Transform trans;
    private GameObject baseball;
    private bool running;
    private bool firstBaseVisited, secondBaseVisited, thirdBaseVisited;
    private GameObject bat;
    private Animator animator;
    private float radiusOfSatisfaction;
	// Use this for initialization
	void Start () {
        radiusOfSatisfaction = 1.0f;
        currentPlayer = this.gameObject;
        trans = currentPlayer.transform;
        running = false;
        firstBaseVisited = secondBaseVisited = thirdBaseVisited = false;
        runSpeed = Random.Range(minSpeed, maxSpeed);
        swingPower = Random.Range(minPower, maxPower);
        
        Transform[] fieldingPositions = GameObject.Find("Stadium").GetComponentsInChildren<Transform>();
        foreach(Transform position in fieldingPositions)
        {
            if(position.name.Equals("First Base"))
            {
                firstBase = position.gameObject;
            }
            if(position.name.Equals("Second Base"))
            {
                secondBase = position.gameObject;
            }
            if(position.name.Equals("Third Base"))
            {
                thirdBase = position.gameObject;
            }
        }

        Vector3 relativePos;
        
        if (currentPlayer.name.Equals("Batting"))
        {
            relativePos = pitcher.transform.position - trans.position;
        }
        else
        {
            relativePos = batter.transform.position - trans.position;
        }
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        trans.rotation = rotation;
       
        animator = GetComponent<Animator>();
        
        if (currentPlayer.name.Equals("Pitcher"))
        {
            Transform[] components = currentPlayer.GetComponentsInChildren<Transform>();
            foreach (Transform component in components)
            {
                if (component.name.Equals("Baseball"))
                {
                    
                    baseball = component.gameObject;
                }
            }
        }else if (currentPlayer.name.Equals("Batting"))
        {
            Transform[] components = currentPlayer.GetComponentsInChildren<Transform>();
            foreach(Transform component in components)
            {
                if (component.name.Equals("Bat"))
                {
                    bat = component.gameObject;
                }
                
            }
            animator.Play("Baseball Walk In");

        }else if(currentPlayer.name.Equals("Batter On Deck"))
        {
            animator.Play("Batter On Deck");
        }
   
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(1))
        {
            if (currentPlayer.name.Equals("Pitcher"))
            {
                Debug.Log("Pitching");
                animator.SetTrigger("Pitch");
                pitch();
                
            }
            if (currentPlayer.name.Equals("Batting"))
            {
                Debug.Log("Batter");
                animator.SetTrigger("Swing");
                
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
        animator.ResetTrigger("Pitch");
        
    }

    public void hit()
    {
        bat.GetComponent <BatScript> ().ReleaseBat();
        animator.ResetTrigger("Swing");
    }

    public void run()
    {
        animator.SetTrigger("Run");
        running = true;
    }

    private void runToBase()
    {
        Quaternion baseRotation;
        Vector3 basePosition;
        float step = 5f * Time.deltaTime;
        if (!firstBaseVisited && !secondBaseVisited && !thirdBaseVisited)
        {
            Debug.Log("Running to first");
            baseRotation = firstBase.transform.rotation;
            basePosition = firstBase.transform.position;
            
        }else if(firstBaseVisited && !secondBaseVisited && !thirdBaseVisited)
        {
            Debug.Log("Running to second");
            baseRotation = secondBase.transform.rotation;
            basePosition = secondBase.transform.position;
        }else if(firstBaseVisited && secondBaseVisited && !thirdBaseVisited)
        {
            Debug.Log("Running to third");
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
            animator.SetTrigger("No Run");
            animator.ResetTrigger("Run");
            running = false;
        }
        
    }
    
   
}
