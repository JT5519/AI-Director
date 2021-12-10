using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHide : MonoBehaviour
{
    PlayerController playerController;
    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.parent.tag == "hidespot" && Input.GetKey(KeyCode.E)
            && playerController.hideFlag == PlayerController.hideFlagBits.dontHide)
        {
            playerController.hideFlag = PlayerController.hideFlagBits.beginHide;
            playerController.currentHideSpot = other.transform;
        }
        if(other.transform.parent.tag == "hidespot" && Input.GetKey(KeyCode.E) 
            && playerController.hideFlag == PlayerController.hideFlagBits.hidden)
        {
            playerController.hideFlag = PlayerController.hideFlagBits.exitHide;
        }
    }
}
