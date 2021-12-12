using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class DirectorAI : MonoBehaviour
{
    [Header("Visualise Menace")]
    public Text menaceText;

    [Header("Player Components")]
    public GameObject player;
    public Camera FPSCamera;
    public float ProjectionLimitToMesh = 5f;
    public LayerMask IgnorePlayerMask;

    [Header("Enemy Components")]
    public GameObject enemy;
    public NavMeshAgent agent;
    public EnemyController enemyController;
    public CapsuleCollider enemyCollider;

    private NavMeshPath path;
    private NavMeshHit NavPointClosestToPlayer;
    private float distance = 0;
    private bool enemyVisible;

    //private int MENACE_BAR = 0;

    private void Start()
    {
        path = new NavMeshPath();
        StartCoroutine(PathCheck());
        StartCoroutine(VisibilityCheck());
    }

    IEnumerator PathCheck()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);
        while (true)
        {
            yield return wait;
            NavMesh.SamplePosition(player.transform.position,out NavPointClosestToPlayer, ProjectionLimitToMesh, NavMesh.AllAreas);
            agent.CalculatePath(NavPointClosestToPlayer.position, path);
            distance = CalculatePathLength(enemy.transform.position);

            //Debug.Log("Path Status: "+path.status);
            //Debug.Log("Path Length: "+distance);
        }
    }

    IEnumerator VisibilityCheck()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        while(true)
        {
            yield return wait;
            enemyVisible = VisibilityRayCastCheck();
            Debug.Log("Enemy Visibility: " + enemyVisible);
        }
    }

    private bool VisibilityRayCastCheck()
    {
        //within frustrum
        Plane[] FPSCameraFrustrumPlanes = GeometryUtility.CalculateFrustumPlanes(FPSCamera);
        if (!GeometryUtility.TestPlanesAABB(FPSCameraFrustrumPlanes, enemyCollider.bounds))
            return false;
        
        //visible
        RaycastHit hit;
        Vector3 origin = FPSCamera.transform.position;
        Vector3 enemyTorso = enemy.transform.position;
        enemyTorso.y += enemyCollider.height/2;
        Vector3 direction = enemyTorso - origin;

        if (Physics.Raycast(origin,direction,out hit,100,IgnorePlayerMask) 
                && hit.collider.gameObject==enemy)
            return true;

        //obstacle
        return false;
    }
    private float CalculatePathLength(Vector3 initialPosition)
    {
        float distance = 0;
        Vector3 previousCorner = initialPosition;
        foreach(Vector3 corner in path.corners)
        {
            distance += Vector3.Distance(corner, previousCorner);
            previousCorner = corner;
        }
        return distance;
    }
    /*private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            MENACE_BAR = Mathf.Min(MENACE_BAR+1, 100);
        }
        else if(Input.GetMouseButtonDown(1))
        {
            MENACE_BAR = Mathf.Max(MENACE_BAR - 1, 0);
        }
        menaceText.text = MENACE_BAR.ToString();
    }*/
}
