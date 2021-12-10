using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    CharacterController selfController;
    public float runSpeed = 5;
    public float walkSpeed = 2;
    float gravity = -9.81f;

    public enum runState
    {
        idle,walking,running
    }
    [HideInInspector] public runState runFlag = runState.idle;

    private void Start()
    {
        selfController = GetComponent<CharacterController>();
    }
    private void Update()
    {
        float forward = Input.GetAxis("Vertical");
        float sideways = Input.GetAxis("Horizontal");
        Vector3 moveVector = forward * transform.forward + sideways * transform.right;
        if (moveVector == Vector3.zero)
            runFlag = runState.idle;
        else
        {
            if ((Input.GetKey(KeyCode.LeftControl)))
            {
                runFlag = runState.walking;
                selfController.Move(walkSpeed * moveVector * Time.deltaTime);
            }
            else
            {
                runFlag = runState.running;
                selfController.Move(runSpeed * moveVector * Time.deltaTime);
            }
        }
        
        //gravity
        selfController.Move(Vector3.up * gravity*Time.deltaTime);
    }
}
