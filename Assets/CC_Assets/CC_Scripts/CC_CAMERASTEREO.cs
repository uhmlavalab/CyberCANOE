using UnityEngine;
using System.Collections;

/* 
Turns the camera into a stereoscopic capable camera.
The centerCamera is the camera attached to this gameobject. 
The leftCamera and rightCamera are created at the start by calling the function createStereoCameras().
Center, Left, and Right all point at the same projection scree.n but 
the left and right cameras are offsetted according to the global interaxial distance.

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: October 26th, 2016.
 */

/// <summary> Turns camera into a stereoscopic capabale camera. </summary>
public class CC_CAMERASTEREO : MonoBehaviour
{
    private Camera leftCamera;
    private Camera rightCamera;
    private Camera centerCamera;

    private RenderTexture centerCameraRT;
    private RenderTexture leftCameraRT;
    private RenderTexture rightCameraRT;

    public void createStereoCameras(bool isDestiny)
    {
        //Create two new GameObjects
        GameObject leftCameraOBJ = new GameObject("LeftCamera");
        GameObject rightCameraOBJ = new GameObject("RightCamera");

        //Add GUILayer and FlareLayer. Default Unity cameras have both of these.
        leftCameraOBJ.AddComponent<GUILayer>();
        rightCameraOBJ.AddComponent<GUILayer>();
        leftCameraOBJ.AddComponent<FlareLayer>();
        rightCameraOBJ.AddComponent<FlareLayer>();

        //Set them as children to this transform.
        leftCameraOBJ.transform.SetParent(transform);
        rightCameraOBJ.transform.SetParent(transform);

        //Setup each camera according to the main camera's settings
        Camera mainCamera = GameObject.Find("CC_HEAD").GetComponent<Camera>();
        centerCamera = GetComponent<Camera>();
        centerCamera.nearClipPlane = mainCamera.nearClipPlane;
        centerCamera.farClipPlane = mainCamera.farClipPlane;
        centerCamera.clearFlags = mainCamera.clearFlags;
        centerCamera.backgroundColor = mainCamera.backgroundColor;
        centerCamera.cullingMask = mainCamera.cullingMask;

        leftCamera = leftCameraOBJ.GetComponent<Camera>();
        leftCamera.rect = centerCamera.rect;
        leftCamera.nearClipPlane = centerCamera.nearClipPlane;
        leftCamera.farClipPlane = centerCamera.farClipPlane;
        leftCamera.clearFlags = centerCamera.clearFlags;
        leftCamera.backgroundColor = centerCamera.backgroundColor;
        leftCamera.cullingMask = centerCamera.cullingMask;

        rightCamera = rightCameraOBJ.GetComponent<Camera>();
        rightCamera.rect = centerCamera.rect;
        rightCamera.nearClipPlane = centerCamera.nearClipPlane;
        rightCamera.farClipPlane = centerCamera.farClipPlane;
        rightCamera.clearFlags = centerCamera.clearFlags;
        rightCamera.backgroundColor = centerCamera.backgroundColor;
        rightCamera.cullingMask = centerCamera.cullingMask;

        //Give each camera a RenderTexture to draw to
        if (isDestiny)
        {
            leftCameraRT = new RenderTexture(Screen.width/2, Screen.height/2, 24);
            rightCameraRT = new RenderTexture(Screen.width/2, Screen.height/2, 24);
            centerCameraRT = new RenderTexture(Screen.width/2, Screen.height/2, 24);
            leftCamera.targetTexture = leftCameraRT;
            rightCamera.targetTexture = rightCameraRT;
            centerCamera.targetTexture = centerCameraRT;
        }
        else
        {

            //Add a CC_CAMERAOFFSET script to both new camera gameobjects. 
            leftCameraOBJ.AddComponent<CC_CAMERAOFFSET>();
            rightCameraOBJ.AddComponent<CC_CAMERAOFFSET>();

            //Set the projection screen of each camera.
            GameObject projectionScreen;
            projectionScreen = GetComponent<CC_CAMERAOFFSET>().getProjectionScreen();
            leftCameraOBJ.GetComponent<CC_CAMERAOFFSET>().setProjectionScreen(projectionScreen);
            rightCameraOBJ.GetComponent<CC_CAMERAOFFSET>().setProjectionScreen(projectionScreen);

            leftCameraRT = new RenderTexture(Screen.width, Screen.height, 24);
            rightCameraRT = new RenderTexture(Screen.width, Screen.height, 24);
            centerCameraRT = new RenderTexture(Screen.width, Screen.height, 24);
            leftCamera.targetTexture = leftCameraRT;
            rightCamera.targetTexture = rightCameraRT;
            centerCamera.targetTexture = centerCameraRT;
        }

    }

    public RenderTexture getCenterRenderTexture()
    {
        return centerCameraRT;
    }

    public RenderTexture getRightRenderTexture()
    {
        return rightCameraRT;
    }

    public RenderTexture getLeftRenderTexture()
    {
        return leftCameraRT;
    }

    //When the screen aspect ratio changes we need to update all the RenderTextures dimensions.
    //First we have to set the TargetTexture to null on each camera, you can't release unless you do.
    //Release the RenderTexture from resources.
    //Create a new RenderTextures and set them as the new TargetTexture on each camera.
    public void updateScreenAspect(bool isDestiny)
    {
        if (isDestiny)
        {
            leftCamera.targetTexture = null;
            leftCameraRT.Release();
            leftCameraRT = new RenderTexture(Screen.width/2, Screen.height/2, 24);

            rightCamera.targetTexture = null;
            rightCameraRT.Release();
            rightCameraRT = new RenderTexture(Screen.width/2, Screen.height/2, 24);

            centerCamera.targetTexture = null;
            centerCameraRT.Release();
            centerCameraRT = new RenderTexture(Screen.width/2, Screen.height/2, 24);
        }
        else
        {
            leftCamera.targetTexture = null;
            leftCameraRT.Release();
            leftCameraRT = new RenderTexture(Screen.width, Screen.height, 24);

            rightCamera.targetTexture = null;
            rightCameraRT.Release();
            rightCameraRT = new RenderTexture(Screen.width, Screen.height, 24);

            centerCamera.targetTexture = null;
            centerCameraRT.Release();
            centerCameraRT = new RenderTexture(Screen.width, Screen.height, 24);
        }


        leftCamera.targetTexture = leftCameraRT;
        rightCamera.targetTexture = rightCameraRT;
        centerCamera.targetTexture = centerCameraRT;
    }


    //Changes the position of the stereo cameras when the interaxial is inc/dec.
    public void updateInteraxial(GameObject head, float interaxial)
    {
        leftCamera.transform.position = head.transform.position + (head.transform.right * (-interaxial / 2));
        rightCamera.transform.position = head.transform.position + (head.transform.right * (interaxial / 2));
    }


    //Disable the center camera and enable the left and right camera.
    public void disableCenterCamera()
    {
        centerCamera.enabled = false;
        leftCamera.GetComponent<Camera>().enabled = true;
        rightCamera.GetComponent<Camera>().enabled = true;
    }

    //Enable the center camera and disable the left and right camera.
    public void enableCenterCamera()
    {
        centerCamera.enabled = true;
        leftCamera.GetComponent<Camera>().enabled = false;
        rightCamera.GetComponent<Camera>().enabled = false;
    }

}



