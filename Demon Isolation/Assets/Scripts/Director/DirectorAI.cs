using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DirectorAI : MonoBehaviour
{
    [Header("Player Components")]
    public GameObject player;
    public Camera FPSCamera;
    public float ProjectionLimitToMesh = 5f;
    public LayerMask IgnorePlayerMask;
    public NavMeshObstacle playerObstacle;

    [Header("Enemy Components")]
    public GameObject enemy;
    public NavMeshAgent agent;
    public EnemyController enemyController;
    public CapsuleCollider enemyCollider;
    public VisionSense enemyVision;

    [Header("House Components")]
    public float QuadrantSize = 17;
    public Transform[] QuadrantPOI;
    public Transform[] CornerPOI;
    private enum QuadrantIndex
    {
        northwest,north,northeast,west,center,east,soutwest,south,southeast,none
    }

    private NavMeshPath path;
    private NavMeshHit NavPointClosestToPlayer;
    
    [Header("Menace Parameters")]
    [SerializeField] float maxPositiveDistCoeff = 5;
    [SerializeField] float negativeDistCoeff = -5/6f;
    [SerializeField] float visibilityCoefficient = 2;

    [SerializeField] float activeHuntCoefficient = 5;
    [SerializeField] float passiveHuntCoefficient = 2;
    [SerializeField] float prowlCoefficient = 0;

    [SerializeField] float menaceCutoff = 75;
    [SerializeField] float relaxCutoff = 25;

    [Header("Director Statistics")]
    [Tooltip("Menace has range: 0 to 100. Player is considered menaced if menace > 75. Once menaced, player is considered relaxed once menace < 25")]
    [Range(0,100)] [SerializeField] private float menace = 0;
    [Tooltip("Menace State. Menaced or Relaxed")]
    [SerializeField] private bool isMenaced = false;
    [Tooltip("Distance between enemy and player. This is path length, not straight line shortest distance.")]
    [SerializeField] private float distance = 0;
    [Tooltip("If enemy is visible to the player or not. True when enemy in camera frustrum and raycast from player hits enemy.")]
    [SerializeField] private bool enemyVisible = false;

    [SerializeField] private float effectiveDistanceCoeff = 0;
    [SerializeField] private float effectiveVisiblityCoeff = 0;
    [SerializeField] private float effectiveHuntCoeff = 0;
    [SerializeField] private float totalCoeff = 0;

    private void Start()
    {
        path = new NavMeshPath();
        StartCoroutine(PathCheck());
        StartCoroutine(VisibilityCheck());
        StartCoroutine(MenaceHandler());
    }
    IEnumerator MenaceHandler()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);

        while (true)
        {
            yield return wait;

            float deltaMenace = CalculateMenaceDelta();

            menace = Mathf.Clamp(menace + deltaMenace, 0,100);
            if (menace > menaceCutoff)
                isMenaced = true;
            else if (menace < relaxCutoff)
                isMenaced = false;

            SetDirectorStateIndex();
        }
    }
    private void SetDirectorStateIndex()
    {
        if (isMenaced)
        {
            enemyController.directorStateIndex = EnemyController.stateNames.avoid;
            enemyController.directorPOI = FindFarthestQuadrant();
        }
        else
        {
            enemyController.directorStateIndex = EnemyController.stateNames.prowl;
            enemyController.directorPOI = FindNearestQuadrant();
        }
    }
    private Transform FindFarthestQuadrant()
    {
        float maxSquareDistance = float.NegativeInfinity;
        Transform farthestPOI = null;

        Vector3 playerPos = player.transform.position;

        foreach(Transform currentPOI in CornerPOI)
        {
            float squareMag = Vector3.SqrMagnitude(playerPos - currentPOI.position);
            if (squareMag > maxSquareDistance)
            {
                farthestPOI = currentPOI;
                maxSquareDistance = squareMag;
            }
        }

        return farthestPOI;
    }
    private Transform FindNearestQuadrant()
    {
        float minSquareDistance = float.PositiveInfinity;
        Transform nearestPOI = null;

        Vector3 playerPos = player.transform.position;

        foreach(Transform currentPOI in QuadrantPOI)
        {
            float squareMag = Vector3.SqrMagnitude(playerPos - currentPOI.position);
            if (squareMag < QuadrantSize*QuadrantSize * 0.25f)
                return currentPOI;
            else if(squareMag < minSquareDistance)
            {
                nearestPOI = currentPOI;
                minSquareDistance = squareMag;
            }
        }

        return nearestPOI;
    }
    private float CalculateMenaceDelta()
    {
        if (enemyVision.shortRadiusFOV < distance && distance < enemyVision.longRadiusFOV)
            effectiveDistanceCoeff = (1 / distance) * enemyVision.shortRadiusFOV * maxPositiveDistCoeff;
        else if (distance <= enemyVision.shortRadiusFOV)
            effectiveDistanceCoeff = maxPositiveDistCoeff;
        else
            effectiveDistanceCoeff = negativeDistCoeff;

        if (enemyVisible)
            effectiveVisiblityCoeff = visibilityCoefficient * effectiveDistanceCoeff / maxPositiveDistCoeff;
        else
            effectiveVisiblityCoeff = 0;

        if (enemyController.currentStateIndex == EnemyController.stateNames.activeHunt)
            effectiveHuntCoeff = activeHuntCoefficient;
        else if (enemyController.currentStateIndex == EnemyController.stateNames.passiveHunt)
            effectiveHuntCoeff = passiveHuntCoefficient;
        else if (enemyController.currentStateIndex == EnemyController.stateNames.prowl)
            effectiveHuntCoeff = prowlCoefficient;
        else
            effectiveHuntCoeff = 0;

        totalCoeff = effectiveDistanceCoeff + effectiveVisiblityCoeff + effectiveHuntCoeff;
        return totalCoeff;
    }
    IEnumerator PathCheck()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);
        while (true)
        {
            yield return wait;
            NavMesh.SamplePosition(player.transform.position,out NavPointClosestToPlayer, ProjectionLimitToMesh, NavMesh.AllAreas);

            if (agent.CalculatePath(NavPointClosestToPlayer.position, path))
                distance = CalculatePathLength(enemy.transform.position);
            else
                distance = float.PositiveInfinity;

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
            //Debug.Log("Enemy Visibility: " + enemyVisible);
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
