using UnityEngine;
using System.Collections;

/* 
This is an example script that allows the user to move around the scene.
Forward movement is based off the orientation of the Right Wand.
You point the right wand in the direction you want to move and pull the trigger.

Right Wand Joystick - Pitch and yaw

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: 1.3, May 12th, 2017.
*/

public class CCaux_Navigator : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float lookSpeed = 50.0f;
    public float maxPitch = 20.0f;
    private float xAxisChange = 0.0f;
    [HideInInspector]
    public float yAxisChange = 0.0F;

    private CharacterController charCont;

    void Start()
    {
        //Get the character controller from CC_CANOE.
        charCont = CC_CANOE.CanoeCharacterController();
    }

    void Update()
    {
        //Directions obtained from CC_CANOE class. We use the Right wand and Head for orientation.
        //Where the right wand is pointed is the direction the player will move
        Vector3 forwardDir = CC_CANOE.WandGameObject(Wand.Right).transform.forward;
        Vector3 rightDir = CC_CANOE.HeadGameObject().transform.right;
        rightDir = Vector3.Normalize(Vector3.ProjectOnPlane(rightDir, Vector3.up));
        Vector3 upDir = Vector3.up;

        //The input from the trigger axis of the left wand
        float forward = CC_INPUT.GetAxis(Wand.Right, WandAxis.Trigger);

        //Move the CharacterController attached to the CC_CANOE. 
        Vector3 movement = forwardDir * forward;
        charCont.Move(movement * Time.deltaTime * moveSpeed);

        //The input from the X and Y axis of the right wand joystick.
        yAxisChange += CC_INPUT.GetAxis(Wand.Right, WandAxis.XAxis) * Time.deltaTime * lookSpeed;
        yAxisChange = yAxisChange % 360.0f;
        xAxisChange += CC_INPUT.GetAxis(Wand.Right, WandAxis.YAxis) * Time.deltaTime * lookSpeed;
        xAxisChange = Mathf.Clamp(xAxisChange, -maxPitch, maxPitch);

        //Change the direction the CharacterController is facing.
        charCont.transform.rotation = Quaternion.identity;
        charCont.transform.RotateAround(CC_CANOE.CanoeGameObject().transform.position, Vector3.up, yAxisChange);
        charCont.transform.RotateAround(CC_CANOE.CanoeGameObject().transform.position, rightDir, xAxisChange);
       
   
    }

}
