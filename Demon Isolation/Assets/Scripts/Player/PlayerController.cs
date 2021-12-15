using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PlayerController : MonoBehaviour
{
    PlayerMotor playerMotor;
    public  enum hideFlagBits
    {
        dontHide,beginHide,hideAction,hidden,exitHide,exitAction
    }
    [HideInInspector] public  hideFlagBits hideFlag = hideFlagBits.dontHide; //1
    [HideInInspector] public Transform currentHideSpot;

    public enum visibiltyFlagBits
    {
        visible,invisible
    }
    [HideInInspector] public visibiltyFlagBits visibilityFlag = visibiltyFlagBits.visible; //2

    Coroutine routineController;

    NavMeshObstacle selfObstacle;

    private void Start()
    {
        playerMotor = gameObject.GetComponent<PlayerMotor>();
        selfObstacle = GetComponent<NavMeshObstacle>();
    }
    private void Update()
    {
        if (hideFlag == hideFlagBits.beginHide)
        {
            bool isOccupied = currentHideSpot.GetComponentInChildren<HideManager>().spotOccupied;
            if (!isOccupied)
            {
                hideFlag = hideFlagBits.hideAction;
                routineController = StartCoroutine(playerEnterHideSpot());
            }
        }
        if (hideFlag == hideFlagBits.exitHide)
        {
            bool isOccupied = currentHideSpot.GetComponentInChildren<HideManager>().spotOccupied;
            if (isOccupied)
            {
                hideFlag = hideFlagBits.exitAction;
                routineController = StartCoroutine(playerExitHideSpot());
            }
        }
    }
    IEnumerator playerEnterHideSpot()
    {
        playerMotor.enabled = false;
        Vector3 playerInitialPosition = transform.position;
        float slerpPercent = 0f;
        float slerpSpeed = 0.5f;
        while (transform.position != currentHideSpot.position && slerpPercent < 1f)
        {
            transform.position = Vector3.Slerp(playerInitialPosition, currentHideSpot.position, slerpPercent);
            slerpPercent += slerpSpeed * Time.deltaTime;
            yield return null;
        }

        visibilityFlag = visibiltyFlagBits.invisible;
        hideFlag = hideFlagBits.hidden;
        gameObject.layer = LayerMask.NameToLayer("Invisible");
        selfObstacle.enabled = false;
    }
    IEnumerator playerExitHideSpot()
    {
        float slerpPercent = 0f;
        float slerpSpeed = 0.5f;
        while (Vector3.Distance(transform.position,currentHideSpot.position)<1f && slerpPercent<1f)
        {
            transform.position = Vector3.Slerp(currentHideSpot.position,currentHideSpot.position-currentHideSpot.forward, slerpPercent);
            slerpPercent += slerpSpeed * Time.deltaTime;
            yield return null;
        }

        playerMotor.enabled = true;

        visibilityFlag = visibiltyFlagBits.visible;
        hideFlag = hideFlagBits.dontHide;
        gameObject.layer = LayerMask.NameToLayer("Player");
        selfObstacle.enabled = true;
    }
}
