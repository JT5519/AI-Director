using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    EnemyMotor enemyMotor;

    public enum huntFlagBits
    {
        avoid,prowl,hunt
    }
    [HideInInspector] private huntFlagBits huntFlag = huntFlagBits.avoid;

    //public Camera enemyCam;
    private void Start()
    {
        enemyMotor = GetComponent<EnemyMotor>();
    }
    private void Update()
    {
        //test code to click and move enemy
        /*if(Input.GetMouseButton(0))
        {
            Ray ray = enemyCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit,100))
            {
                enemyMotor.MoveToPoint(hit.point);
            }
        }*/     if(huntFlag==huntFlagBits.hunt)
        {

        }
        else if(huntFlag==huntFlagBits.prowl)
        {

        }
        else if(huntFlag==huntFlagBits.avoid)
        {

        }

    }
    public huntFlagBits getHuntState()
    {
        return huntFlag;
    }
    public void setHuntState(huntFlagBits state)
    {
        huntFlag = state;
    }
}
