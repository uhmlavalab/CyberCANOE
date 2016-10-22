using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/* 
This is an example script that allows the user to draw with the wands and 
choose a color from the color picker quad in the scene. 
Trigger - Draw new line 
Dpad Up - Choose new color

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: October 26th, 2016.
*/

public class CCaux_LineDrawer : MonoBehaviour
{
    public LineRenderer line1, line2, linePointer1, linePointer2;

    private int lineCount1, lineCount2;
    private LineRenderer newLine1, newLine2;
    private Color leftColor, rightcolor;

    void Start()
    {
        lineCount1 = 0;
        lineCount2 = 0;
        leftColor = new Color(1, 0.88f, 0.49f);
        rightcolor = new Color(0.94f, 0.28f, 0.13f);

        //Setup the line pointers.
        linePointer1.SetVertexCount(0);
        linePointer1.SetWidth(0.01f, 0.01f);
        linePointer2.SetVertexCount(0);
        linePointer2.SetWidth(0.01f, 0.01f);

    }

    void LateUpdate()
    {
        //Draw line for left wand.
        if (CC_INPUT.GetAxis(Wand.Left, WandAxis.Trigger) > 0.0f)
        {
            //If lineCount is zero then we are starting a new line.
            if (lineCount1 == 0)
            {
                newLine1 = (LineRenderer)Instantiate(line1, CC_CANOE.WandTransform(Wand.Left).position, CC_CANOE.WandTransform(Wand.Right).rotation);
                lineCount1++;
            }
            else
            {
                //Otherwise we are adding line segments
                newLine1.SetVertexCount(lineCount1);
                newLine1.SetPosition(lineCount1 - 1, CC_CANOE.WandTransform(Wand.Left).position);
                lineCount1++;
            }
        }
        else
        {
            //When the trigger button is released that assumes you are done making
            //that one line sequence and will be starting a new one.
            lineCount1 = 0;
        }

        //Same as above for right wand.
        if (CC_INPUT.GetAxis(Wand.Right, WandAxis.Trigger) > 0.0f)
        {
            if (lineCount2 == 0)
            {

                newLine2 = (LineRenderer)Instantiate(line2, CC_CANOE.WandTransform(Wand.Right).position, CC_CANOE.WandTransform(Wand.Right).rotation);
                lineCount2++;
            }
            else
            {
                newLine2.SetVertexCount(lineCount2);
                newLine2.SetPosition(lineCount2 - 1, CC_CANOE.WandTransform(Wand.Right).position);
                lineCount2++;
            }
        }
        else
        {
            lineCount2 = 0;
        }

        //Color pick for left wand.
        if (CC_INPUT.GetButtonPress(Wand.Left, WandButton.Up))
        {
            getColor(Wand.Left);
        }
        else
        {
            //Erase the line pointer.
            linePointer1.SetVertexCount(0);
        }

        //Color pick for right wand.
        if (CC_INPUT.GetButtonPress(Wand.Right, WandButton.Up))
        {
            getColor(Wand.Right);
        }
        else
        {
            //Erase the line pointer.
            linePointer2.SetVertexCount(0);
        }

    }

    //Get the color from the color picker quad and set color to the wand's line renderer
    private void getColor(Wand wand)
    {
        //The line pointer origin is the Wand's world position.
        Vector3 rayOrigin = CC_CANOE.WandTransform(wand).position;
        //The line pointer direction is the Wand's transform forward direction.
        Vector3 rayDirection = CC_CANOE.WandTransform(wand).forward;

        //Draw the line pointer
        drawLinePointer(wand, rayOrigin, rayDirection);

        //Raycast stuff
        RaycastHit hit;
        Renderer rend;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, 30)) {


            if (hit.transform.gameObject.name.Equals("ColorPicker"))
            {

                rend = hit.transform.GetComponent<Renderer>();
                Texture2D textureMap = rend.material.mainTexture as Texture2D;
                Vector2 pixelUV = hit.textureCoord;

                if (wand == Wand.Left)
                {
                    leftColor = textureMap.GetPixelBilinear(pixelUV.x, pixelUV.y);
                    linePointer1.SetColors(leftColor, leftColor);
                    line1.SetColors(leftColor, leftColor);
                }
                else if (wand == Wand.Right)
                {
                    rightcolor = textureMap.GetPixelBilinear(pixelUV.x, pixelUV.y);
                    linePointer2.SetColors(rightcolor, rightcolor);
                    line2.SetColors(rightcolor, rightcolor);
                }
            }

            
        }

    }

    //Draws the line pointers.
    private void drawLinePointer(Wand wand, Vector3 origin, Vector3 direction)
    {
        if (wand == Wand.Left)
        {
            linePointer1.SetVertexCount(2);
            linePointer1.SetPosition(0, origin);
            linePointer1.SetPosition(1, origin + (direction.normalized * 30));
        }
        else if(wand == Wand.Right)
        {
            linePointer2.SetVertexCount(2);
            linePointer2.SetPosition(0, origin);
            linePointer2.SetPosition(1, origin + (direction.normalized * 30));
        }

    }

}
