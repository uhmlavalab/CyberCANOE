using UnityEngine;

/* 
This is an example script that allows the user to draw with the wands and 
choose a color from the color picker quad in the scene. 
Trigger - Draw new line 
Dpad Up - Choose new color

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: 1.13, May 17th, 2017.
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
        linePointer1.positionCount = 0;
        linePointer1.startWidth = 0.01f;
        linePointer1.endWidth = 0.01f;
        linePointer2.positionCount = 0;
        linePointer2.startWidth = 0.01f;
        linePointer2.endWidth = 0.01f;

    }

    void LateUpdate()
    {
        //Draw line for left wand.
        if (CC_INPUT.GetButtonPress(Wand.Left, WandButton.Down))
        {
            //If lineCount is zero then we are starting a new line.
            if (lineCount1 == 0)
                newLine1 = Instantiate(line1, CC_CANOE.WandTransform(Wand.Left).position, CC_CANOE.WandTransform(Wand.Right).rotation);

            lineCount1++;
            newLine1.positionCount = lineCount1;
            newLine1.SetPosition(lineCount1 - 1, CC_CANOE.WandTransform(Wand.Left).position);
        }
        else
        {
            lineCount1 = 0;
        }

        //Draw line for right wand.
        if (CC_INPUT.GetButtonPress(Wand.Right, WandButton.Down))
        {
            //If lineCount is zero then we are starting a new line.
            if (lineCount2 == 0)
                newLine2 = Instantiate(line2, CC_CANOE.WandTransform(Wand.Right).position, CC_CANOE.WandTransform(Wand.Right).rotation);

            lineCount2++;
            newLine2.positionCount = lineCount2;
            newLine2.SetPosition(lineCount2 - 1, CC_CANOE.WandTransform(Wand.Right).position);
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
            linePointer1.positionCount = 0;
        }

        //Color pick for right wand.
        if (CC_INPUT.GetButtonPress(Wand.Right, WandButton.Up))
        {
            getColor(Wand.Right);
        }
        else
        {
            //Erase the line pointer.
            linePointer2.positionCount = 0;
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

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, 30))
        {
            if (hit.transform.gameObject.name.Equals("ColorPicker"))
            {

                rend = hit.transform.GetComponent<Renderer>();
                Texture2D textureMap = rend.material.mainTexture as Texture2D;
                Vector2 pixelUV = hit.textureCoord;

                if (wand == Wand.Left)
                {
                    leftColor = textureMap.GetPixelBilinear(pixelUV.x, pixelUV.y);
                    linePointer1.startColor = leftColor;
                    linePointer1.endColor = leftColor;
                    line1.startColor = leftColor;
                    line1.endColor = leftColor;
                }
                else if (wand == Wand.Right)
                {
                    rightcolor = textureMap.GetPixelBilinear(pixelUV.x, pixelUV.y);
                    linePointer2.startColor = rightcolor;
                    linePointer2.endColor = rightcolor;
                    line2.startColor = rightcolor;
                    line2.endColor = rightcolor;
                }
            }
        }
    }

    //Draws the line pointers.
    private void drawLinePointer(Wand wand, Vector3 origin, Vector3 direction)
    {
        if (wand == Wand.Left)
        {
            linePointer1.positionCount = 2;
            linePointer1.SetPosition(0, origin);
            linePointer1.SetPosition(1, origin + (direction.normalized * 30));
        }
        else if (wand == Wand.Right)
        {
            linePointer2.positionCount = 2;
            linePointer2.SetPosition(0, origin);
            linePointer2.SetPosition(1, origin + (direction.normalized * 30));
        }
    }
}
