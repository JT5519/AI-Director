using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    EnemyMotor enemyMotor;
    NavMeshAgent agent;
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
        public float agentSpeedInState;
        public State(stateNames s, bool i,bool iR=false,float aS=5)
        {
            this.state = s;
            this.isInterruptable = i;
            this.isRunning = iR;
            this.agentSpeedInState = aS;
        }
    }

    List<State> stateList = new List<State>(3);
    public delegate IEnumerator RoutineDelegate();
    List<RoutineDelegate> stateRoutineList = new List<RoutineDelegate>(3);
    stateNames currentStateIndex = stateNames.prowl;
    
    stateNames directorStateIndex = stateNames.prowl; //placeholder for director input
    [SerializeField] Transform directorPOI;
    
    [HideInInspector] public bool stateChangeFlag = true;
    
    VisionSense vision;

    Coroutine InterruptCheckerRoutineController;

    [SerializeField] private LayerMask POIMask;

    private void Start()
    {
        enemyMotor = GetComponent<EnemyMotor>();
        agent = GetComponent<NavMeshAgent>();

        //creating the states 
        stateList.Add(new State(stateNames.hunt, false,false,10));
        stateList.Add(new State(stateNames.prowl, true));
        stateList.Add(new State(stateNames.avoid, true));

        stateRoutineList.Add(HuntRoutine);
        stateRoutineList.Add(ProwlRoutine);
        stateRoutineList.Add(AvoidRoutine);

        vision = GetComponentInChildren<VisionSense>();

        StartCoroutine(ActionCycleRoutine());
    }
    IEnumerator ActionCycleRoutine()
    {
        WaitUntil wait = new WaitUntil(() => stateChangeFlag == true);
        while (true)
        {
            yield return wait;
            StateChoice();
            StateInit();
            Coroutine currentRoutine = StartCoroutine(stateRoutineList[(int)currentStateIndex]());
            try
            {
                StopCoroutine(InterruptCheckerRoutineController);
            }
            catch(NullReferenceException)
            {
                //Do nothing, the coroutine was already terminated
            }
            if(stateList[(int)currentStateIndex].isInterruptable)
                InterruptCheckerRoutineController = StartCoroutine(InterruptTestRoutine(currentRoutine));
        }
    }
    IEnumerator InterruptTestRoutine(Coroutine currentRoutine)
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (true)
        {
            yield return wait;
            if(InterruptAlert())
            {
                StopCoroutine(currentRoutine);
                stateChangeFlag = true;
                yield break;
            }
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
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        yield return wait;
        enemyMotor.MoveToPoint(directorPOI.position);

        yield return new WaitUntil(NavMeshPathCompletionTest);
        Queue<Vector3> POIs = FindVicinityPOIs();
        Debug.Log(POIs.Count);
        while(POIs.Count!=0)
        {
            yield return wait;
            enemyMotor.MoveToPoint(POIs.Dequeue());
            yield return new WaitUntil(NavMeshPathCompletionTest);
        }

        stateChangeFlag = true;
    }
    IEnumerator AvoidRoutine()
    {
        yield return null;
        stateChangeFlag = true;
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

    private void StateInit()
    {
        agent.speed = stateList[(int)currentStateIndex].agentSpeedInState;
    }
    private void StateChoice()
    {
        if (vision.playerVisible)
            currentStateIndex = stateNames.hunt;
        else if (directorStateIndex == stateNames.prowl)
            currentStateIndex = stateNames.prowl;
        else if (directorStateIndex == stateNames.avoid)
            currentStateIndex = stateNames.avoid;
        stateChangeFlag = false;
    }
    private bool InterruptAlert()
    {
        return vision.playerVisible;
    }
    private Queue<Vector3> FindVicinityPOIs()
    {
        Queue<Vector3> POIQueue = new Queue<Vector3>();
        
        Vector3 boxBounds = new Vector3(5, 2, 5);
        Vector3 boxCenter = transform.position;
        boxCenter.y += GetComponent<CapsuleCollider>().height / 2;

        Collider[] POICollidersArray = Physics.OverlapBox(boxCenter, boxBounds / 2, Quaternion.identity, POIMask);
        
        foreach(Collider colliders in POICollidersArray)
            POIQueue.Enqueue(colliders.transform.position);
        
        return POIQueue;
    }
    private bool NavMeshPathCompletionTest()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    return true;
            }
        }
        return false;
    }
}

