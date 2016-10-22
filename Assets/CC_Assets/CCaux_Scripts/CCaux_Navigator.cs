using UnityEngine;
using System.Collections;

/* 
This is an example script that allows the user to move around the scene.
All the controls are based off the orientation of the Left Wand.
You point the left wand in the direction you want to move.

Left Wand Joystick - Forward/Backward and Yaw 
Right Wand Joystick - Pitch and Roll

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: October 26th, 2016.
*/

public class CCaux_Navigator : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float lookSpeed = 50.0f;

    private CharacterController charCont;

    void Start()
    {
        //Get the character controller from CC_CANOE.
        charCont = CC_CANOE.CanoeCharacterController();
    }

    void Update()
    {
        //Directions obtained from CC_CANOE class. We use the Left wand for orientation.
        //Where the left wand is pointed where the player will move and rotate around
        Vector3 forwardDir = CC_CANOE.WandGameObject(Wand.Left).transform.forward;
        Vector3 rightDir = CC_CANOE.WandGameObject(Wand.Left).transform.right;
        Vector3 upDir = CC_CANOE.WandGameObject(Wand.Left).transform.up;

        //The input from the trigger axis of the left wand
        float forward = CC_INPUT.GetAxis(Wand.Left, WandAxis.YAxis);

        //Move the CharacterController attached to the CC_CANOE. 
        Vector3 movement = forwardDir * forward;
        charCont.Move(movement * Time.deltaTime * moveSpeed);

        //The input from the X and Y axis of the right wand joystick.
        float yaw = CC_INPUT.GetAxis(Wand.Left, WandAxis.XAxis) * Time.deltaTime * lookSpeed;
        float pitch = CC_INPUT.GetAxis(Wand.Right, WandAxis.YAxis) * Time.deltaTime * lookSpeed;
        float roll = CC_INPUT.GetAxis(Wand.Right, WandAxis.XAxis) * Time.deltaTime * lookSpeed;

        //Change the direction the CharacterController is facing.
        charCont.transform.RotateAround(charCont.transform.position, rightDir, pitch);
        charCont.transform.RotateAround(charCont.transform.position, upDir, yaw);
        charCont.transform.RotateAround(charCont.transform.position, forwardDir, -roll);
    }
}
