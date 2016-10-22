using UnityEngine;
using System.Collections;

/* 
The code is based on the Robert Kooima's publication "Generalized Perspective Projection," 2009. http://csc.lsu.edu/~kooima/pdfs/gen-perspective.pdf 

Obtained from https://en.wikibooks.org/wiki/Cg_Programming/Unity/Projection_for_Virtual_Reality

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: October 26th, 2016.
 */

/// <summary> Offaxis camera projection. </summary>
[ExecuteInEditMode]
public class CC_CAMERAOFFSET : MonoBehaviour
{
    public GameObject projectionScreen;
    private Camera cameraComponent;

    void Start()
    {
        cameraComponent = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (null != projectionScreen && null != cameraComponent)
        {
            //Lower left corner of projection screen in world coordinates.
            Vector3 PSLL = projectionScreen.transform.TransformPoint(new Vector3(-0.5f, -0.5f, 0.0f));
            //Lower right corner of projection screen in world coordinates.
            Vector3 PSLR = projectionScreen.transform.TransformPoint(new Vector3(0.5f, -0.5f, 0.0f));
            //Upper left corner of projection screen in world coordinates.
            Vector3 PSUL = projectionScreen.transform.TransformPoint(new Vector3(-0.5f, 0.5f, 0.0f));

            //Eye position.
            Vector3 eye = transform.position;
            //Distance to near clipping plane.
            float nearClipDistance = cameraComponent.nearClipPlane;
            //Distance to far clipping plane.
            float farClipDistance = cameraComponent.farClipPlane;

            //Distances from eye to projection screen corners.
            Vector3 eyeToPSLL = PSLL - eye;
            Vector3 eyeToPSLR = PSLR - eye;
            Vector3 eyeToPSUL = PSUL - eye;

            //Right axis of screen.
            Vector3 psRight = PSLR - PSLL;
            psRight.Normalize();
            //Up axis of screen.
            Vector3 psUp = PSUL - PSLL;
            psUp.Normalize();
            //Normal vector of screen.
            Vector3 psNormal = -Vector3.Cross(psRight, psUp);

            //Distance from eye to screen.
            float d = -Vector3.Dot(eyeToPSLL, psNormal);
            //Distance to left screen edge.
            float l = Vector3.Dot(psRight, eyeToPSLL) * nearClipDistance / d;
            //Ditance to right screen edge.
            float r = Vector3.Dot(psRight, eyeToPSLR) * nearClipDistance / d;
            //Ditance to bottom screen edge.
            float b = Vector3.Dot(psUp, eyeToPSLL) * nearClipDistance / d;
            //Distance to top screen edge.
            float t = Vector3.Dot(psUp, eyeToPSUL) * nearClipDistance / d;

            //Projection matrix. 
            Matrix4x4 p = new Matrix4x4();
            p[0, 0] = 2.0f * nearClipDistance / (r - l);
            p[0, 1] = 0.0f;
            p[0, 2] = (r + l) / (r - l);
            p[0, 3] = 0.0f;

            p[1, 0] = 0.0f;
            p[1, 1] = 2.0f * nearClipDistance / (t - b);
            p[1, 2] = (t + b) / (t - b);
            p[1, 3] = 0.0f;

            p[2, 0] = 0.0f;
            p[2, 1] = 0.0f;
            p[2, 2] = (farClipDistance + nearClipDistance) / (nearClipDistance - farClipDistance);
            p[2, 3] = 2.0f * farClipDistance * nearClipDistance / (nearClipDistance - farClipDistance);

            p[3, 0] = 0.0f;
            p[3, 1] = 0.0f;
            p[3, 2] = -1.0f;
            p[3, 3] = 0.0f;

            //Rotation matrix.
            Matrix4x4 rm = new Matrix4x4();
            rm[0, 0] = psRight.x;
            rm[0, 1] = psRight.y;
            rm[0, 2] = psRight.z;
            rm[0, 3] = 0.0f;

            rm[1, 0] = psUp.x;
            rm[1, 1] = psUp.y;
            rm[1, 2] = psUp.z;
            rm[1, 3] = 0.0f;

            rm[2, 0] = psNormal.x;
            rm[2, 1] = psNormal.y;
            rm[2, 2] = psNormal.z;
            rm[2, 3] = 0.0f;

            rm[3, 0] = 0.0f;
            rm[3, 1] = 0.0f;
            rm[3, 2] = 0.0f;
            rm[3, 3] = 1.0f;

            //Translation matrix.
            Matrix4x4 tm = new Matrix4x4();
            tm[0, 0] = 1.0f;
            tm[0, 1] = 0.0f;
            tm[0, 2] = 0.0f;
            tm[0, 3] = -eye.x;

            tm[1, 0] = 0.0f;
            tm[1, 1] = 1.0f;
            tm[1, 2] = 0.0f;
            tm[1, 3] = -eye.y;

            tm[2, 0] = 0.0f;
            tm[2, 1] = 0.0f;
            tm[2, 2] = 1.0f;
            tm[2, 3] = -eye.z;

            tm[3, 0] = 0.0f;
            tm[3, 1] = 0.0f;
            tm[3, 2] = 0.0f;
            tm[3, 3] = 1.0f;

            //Set matrices.
            cameraComponent.projectionMatrix = p;
            cameraComponent.worldToCameraMatrix = rm * tm;

        }
    }

    public GameObject getProjectionScreen()
    {
        return projectionScreen;
    }

    public void setProjectionScreen(GameObject projectionScreen)
    {
        this.projectionScreen = projectionScreen;
    }


}

