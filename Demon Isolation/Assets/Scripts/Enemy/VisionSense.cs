using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionSense : MonoBehaviour
{
    public float extremeShortRadiusFOV;
    [Range(0, 360)] public float extremeShortAngleFOV;

    public float shortRadiusFOV;
    [Range(0, 360)] public float shortAngleFOV;

    public float longRadiusFOV;
    [Range(0, 360)] public float longAngleFOV;

    public LayerMask playerMask;
    public LayerMask obstructionMask;

    private bool playerVisibleExtremeShort = false;
    private bool playerVisibleShort = false;
    private bool playerVisibleLong = false;
    [HideInInspector] public bool playerVisible = false;
    Transform tempTargetInfo = null;
    [HideInInspector] public Transform targetInfo = null; 

    private void Start()
    {
        StartCoroutine(ExtremeShortFOVRoutine());
        StartCoroutine(ShortFOVRoutine());
        StartCoroutine(LongFOVRoutine());
    }
    IEnumerator ExtremeShortFOVRoutine()
    {
        float delay = 0.1f;
        WaitForSeconds wait = new WaitForSeconds(delay);
        while(true)
        {
            yield return wait;
            if (OptimisedFOVCheck(extremeShortRadiusFOV))
            {
                playerVisibleExtremeShort = true;
                targetInfo = tempTargetInfo;
            }
            else
                playerVisibleExtremeShort = false;
            netVisibility();
        }
    }
    IEnumerator ShortFOVRoutine()
    {
        float delay = 0.2f;
        WaitForSeconds wait = new WaitForSeconds(delay);
        while(true)
        {
            yield return wait;
            if (FOVCheck(shortRadiusFOV, shortAngleFOV))
            {
                playerVisibleShort = true;
                targetInfo = tempTargetInfo;
            }
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
            {
                playerVisibleLong = true;
                targetInfo = tempTargetInfo;
            }
            else
                playerVisibleLong = false;
            netVisibility();
        }
    }
    private void netVisibility()
    {
        if (playerVisibleLong || playerVisibleShort || playerVisibleExtremeShort)
        {
            playerVisible = true;
        }
        else
        {
            playerVisible = false;
            targetInfo = null;
        }
    }
    private bool OptimisedFOVCheck(float radiusFOV)
    {
        Collider[] visibleItems = Physics.OverlapSphere(transform.position, radiusFOV, playerMask);
        if (visibleItems.Length == 0)
            return false;

        tempTargetInfo = visibleItems[0].transform;
        return true;
    }
    private bool FOVCheck(float radiusFOV, float angleFOV)
    {
        //out of vision sphere OR player Invisible
        Collider[] visibleItems = Physics.OverlapSphere(transform.position, radiusFOV, playerMask);
        if (visibleItems.Length == 0)
            return false;

        //out of FOV
        tempTargetInfo = visibleItems[0].transform;
        Vector3 directionToPlayer = (tempTargetInfo.position - transform.position).normalized;
        float angleWithPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleWithPlayer > angleFOV / 2)
            return false;

        //obstacle
        float distanceToPlayer = Vector3.Distance(tempTargetInfo.position, transform.position);
        if (Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstructionMask))
            return false;
        
        //visible
        return true;
    }

}
