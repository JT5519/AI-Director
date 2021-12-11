using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    EnemyMotor enemyMotor;
    //public Camera enemyCam;

    public enum stateNames
    {
        hunt, prowl, avoid
    }
    public struct State
    {
        public stateNames state;
        public bool isInterruptable;
        public bool isRunning;
        public State(stateNames s, bool i,bool iR=false)
        {
            this.state = s;
            this.isInterruptable = i;
            this.isRunning = iR;
        }
    }

    List<State> stateList = new List<State>(3);
    List<Coroutine> stateRoutineList = new List<Coroutine>(3);
    stateNames currentStateIndex = stateNames.prowl;
    stateNames directorStateIndex = stateNames.prowl; //placeholder for director input
    
    public bool stateChangeFlag = true;

    VisionSense vision;
    
    private void Start()
    {
        enemyMotor = GetComponent<EnemyMotor>();
        
        //creating the states 
        stateList.Add(new State(stateNames.hunt, false));
        stateList.Add(new State(stateNames.prowl, true));
        stateList.Add(new State(stateNames.avoid, true));

        //placeholder
        //stateRoutineList.Add(HuntRoutine);

        vision = GetComponentInChildren<VisionSense>();

        StartCoroutine(ActionCycleRoutine());
    }
    IEnumerator ActionCycleRoutine()
    {
        WaitUntil wait = new WaitUntil(() => stateChangeFlag = true);
        while (true)
        {
            yield return wait;
            stateChoice();
            
            //placeholder code
            if (currentStateIndex == stateNames.hunt)
                StartCoroutine(HuntRoutine());
            else if (currentStateIndex == stateNames.prowl)
                StartCoroutine(ProwlRoutine());
            else if (currentStateIndex == stateNames.avoid)
                StartCoroutine(AvoidRoutine());
        }
    }
    
    IEnumerator HuntRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while(vision.targetInfo!=null)
        {
            enemyMotor.MoveToPoint(vision.targetInfo.position);
            yield return wait;
        }
        stateChangeFlag = true;
    }
    IEnumerator ProwlRoutine()
    {
        stateChangeFlag = true;
        yield return null;
    }
    IEnumerator AvoidRoutine()
    {
        stateChangeFlag = true;
        yield return null;
    }
    /*private void Update()
    {
        //test code to click and move enemy
        if(Input.GetMouseButton(0))
        {
            Ray ray = enemyCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit,100))
            {
                enemyMotor.MoveToPoint(hit.point);
            }
        }

    }*/

    private void stateChoice()
    {
        if (vision.playerVisible)
            currentStateIndex = stateNames.hunt;
        else if (directorStateIndex == stateNames.prowl)
            currentStateIndex = stateNames.prowl;
        else if (directorStateIndex == stateNames.avoid)
            currentStateIndex = stateNames.avoid;
        stateChangeFlag = false;
    }
}

