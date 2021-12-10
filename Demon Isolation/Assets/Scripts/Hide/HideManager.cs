using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideManager : MonoBehaviour
{
    [HideInInspector] public bool spotOccupied = false;
    public GameObject hideMessageCanvas;
    public GameObject exitMessageCanvas;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            hideMessageCanvas.SetActive(true);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !spotOccupied 
                && other.GetComponent<PlayerController>().hideFlag == PlayerController.hideFlagBits.hidden)
        {
            spotOccupied = true;
            hideMessageCanvas.SetActive(false);
            exitMessageCanvas.SetActive(true);
        }
        if (other.tag == "Player" && spotOccupied
                && other.GetComponent<PlayerController>().hideFlag == PlayerController.hideFlagBits.exitAction)
        {
            exitMessageCanvas.SetActive(false);
        }
        if (other.tag == "Player" && spotOccupied
        && other.GetComponent<PlayerController>().hideFlag == PlayerController.hideFlagBits.dontHide)
        {
            spotOccupied = false;
            hideMessageCanvas.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            hideMessageCanvas.SetActive(false);
            spotOccupied = false;
        }
    }

}
