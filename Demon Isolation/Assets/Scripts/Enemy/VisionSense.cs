using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionSense : MonoBehaviour
{
    public float shortRadiusFOV;
    [Range(0, 360)] public float shortAngleFOV;

    public float longRadiusFOV;
    [Range(0, 360)] public float longAngleFOV;

    public LayerMask playerMask;
    public LayerMask obstructionMask;

    private EnemyController myController;

    private bool playerVisibleShort = false;
    private bool playerVisibleLong = false;
    [HideInInspector] public bool playerVisible = false;
    [HideInInspector] public Transform targetInfo = null; 

    private void Start()
    {
        myController = transform.parent.GetComponent<EnemyController>();
        StartCoroutine(ShortFOVRoutine());
        StartCoroutine(LongFOVRoutine());
    }
    IEnumerator ShortFOVRoutine()
    {
        float delay = 0.2f;
        WaitForSeconds wait = new WaitForSeconds(delay);
        while(true)
        {
            yield return wait;
            if (FOVCheck(shortRadiusFOV,shortAngleFOV))
                playerVisibleShort = true;
            else
                playerVisibleShort = false;
            netVisibility();
        }
    }
    IEnumerator LongFOVRoutine()
    {
        float delay = 0.5f;
        WaitForSeconds wait = new WaitForSeconds(delay);
        while (true)
        {
            yield return wait;
            if (FOVCheck(longRadiusFOV,longAngleFOV))
                playerVisibleLong = true;
            else
                playerVisibleLong = false;
            netVisibility();
        }
    }
    private void netVisibility()
    {
        if (playerVisibleLong || playerVisibleShort)
            playerVisible = true;
        else
        {
            playerVisible = false;
            targetInfo = null;
        }
    }
    private bool FOVCheck(float radiusFOV, float angleFOV)
    {
        //out of vision sphere
        Collider[] visibleItems = Physics.OverlapSphere(transform.position, radiusFOV, playerMask);
        if (visibleItems.Length == 0)
            return false;

        //out of FOV
        targetInfo = visibleItems[0].transform;
        Vector3 directionToPlayer = (targetInfo.position - transform.position).normalized;
        float angleWithPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleWithPlayer > angleFOV / 2)
            return false;

        //obstacle
        float distanceToPlayer = Vector3.Distance(targetInfo.position, transform.position);
        if (Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstructionMask))
            return false;
        
        //visible
        return true;
    }

}
