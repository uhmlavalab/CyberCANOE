using UnityEngine;

/*
The input class for interfacing with the CyberCANOE Wands.

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: 1.3, May 12th, 2017.
 */

/// <summary>
/// Enumerated identifiers for the left or right wand
/// </summary>
public enum Wand
{
    Left = 0,
    Right = 1
}

/// <summary>
/// Enumerated identifiers for wand buttons
/// </summary>
public enum WandButton
{
    X = 0,
    O = 1,
    Shoulder = 2,
    Up = 3,
    Right = 4,
    Down = 5,
    Left = 6
}


/// <summary>
/// Enumerated identifiers for wand axis
/// </summary>
public enum WandAxis
{
    XAxis = 0,
    YAxis = 1,
    Trigger = 2,
}


/// <summary> Input interface for the wand controllers. </summary>
public static class CC_INPUT
{

    /// <summary>
    /// Returns true if the 'button' is held down by the specified wand.   
    /// </summary>
    /// <param name="wand">Wand Controller</param>
    /// <param name="button">Wand Button</param>
    public static bool GetButtonPress(Wand wand, WandButton button)
    {
        if (!StillCurrentFrame())
        {
            UpdateAllStates();
        }

        KeyCode simKey = getSimKey(button);

        if (Input.GetKey(simKey) && CC_CANOE.simActiveWand == wand && CC_CANOE.keyboardControls)
        {
            return true;
        }
        else if (CC_CONFIG.isDestiny() || CC_CONFIG.isInnovator())
        {
            return GetButtonState(wand, button).isDown;
        }
        else
        {
            return false;
        }

    }

    /// <summary>
    /// Returns true at the frame the 'button' starts to press down (not held down) by the specified 'wand'. 
    /// </summary>
    /// <param name="wand">Wand Controller</param>
    /// <param name="button">Wand Button</param>
    public static bool GetButtonDown(Wand wand, WandButton button)
    {
        if (!StillCurrentFrame())
        {
            UpdateAllStates();
        }

        KeyCode simKey = getSimKey(button);

        if (Input.GetKeyDown(simKey) && CC_CANOE.simActiveWand == wand && CC_CANOE.keyboardControls)
        {
            return true;
        }
        else if (CC_CONFIG.isDestiny() || CC_CONFIG.isInnovator())
        {
            return GetButtonState(wand, button).justDown;
        }
        else
        {
            return false;
        }

    }

    /// <summary>
    /// Returns true at the frame the 'button' starts to be released by the specified 'wand'. 
    /// </summary>
    /// <param name="wand">Wand Controller</param>
    /// <param name="button">Wand Button</param>
    public static bool GetButtonUp(Wand wand, WandButton button)
    {

        if (!StillCurrentFrame())
        {
            UpdateAllStates();
        }

        KeyCode simKey = getSimKey(button);

        if (Input.GetKeyUp(simKey) && CC_CANOE.simActiveWand == wand && CC_CANOE.keyboardControls)
        {
            return true;
        }
        else if (CC_CONFIG.isDestiny() || CC_CONFIG.isInnovator())
        {
            return GetButtonState(wand, button).justUp;
        }
        else
        {
            return false;
        }

    }

    /// <summary>
    /// Returns the float number of the 'Axis' for the specified wand. 
    /// </summary>
    /// <param name="wand">Wand Controller</param>
    /// <param name="button">Wand Axis</param>
    public static float GetAxis(Wand wand, WandAxis axis)
    {

        if (!StillCurrentFrame())
        {
            UpdateAllStates();
        }
        KeyCode simAxis1 = getSimAxis1(axis);
        KeyCode simAxis2 = getSimAxis2(axis);

        if (CC_CANOE.simActiveWand == wand && (Input.GetKey(simAxis1) || Input.GetKey(simAxis2)) && CC_CANOE.keyboardControls)
        {
            if (Input.GetKey(simAxis1) && CC_CANOE.keyboardControls)
                return 1.0f;
            else
                return -1.0f;
        }

        else if (CC_CONFIG.isDestiny() || CC_CONFIG.isInnovator())
        {
            return GetAxisState(wand, axis).axisValue;
        }
        else
        {
            return 0.0f;
        }
    }

    //Variables
    private static ButtonState leftX = new ButtonState();
    private static ButtonState leftO = new ButtonState();
    private static ButtonState leftShoulder = new ButtonState();
    private static ButtonState leftDUp = new ButtonState();
    private static ButtonState leftDLeft = new ButtonState();
    private static ButtonState leftDDown = new ButtonState();
    private static ButtonState leftDRight = new ButtonState();
    private static AxisState leftYAxis = new AxisState();
    private static AxisState leftXAxis = new AxisState();
    private static AxisState leftTrigger = new AxisState();
  
    private static ButtonState rightX = new ButtonState();
    private static ButtonState rightO = new ButtonState();
    private static ButtonState rightShoulder = new ButtonState();
    private static ButtonState rightDUp = new ButtonState();
    private static ButtonState rightDLeft = new ButtonState();
    private static ButtonState rightDDown = new ButtonState();
    private static ButtonState rightDRight = new ButtonState();
    private static AxisState rightYAxis = new AxisState();
    private static AxisState rightXAxis = new AxisState();
    private static AxisState rightTrigger = new AxisState();


    private static int savedFrameCount = -1;
    private static bool updateCalled = false;

    private static bool dpadLeftActivated;
    private static bool dpadRightActivated;


    /// <summary>
    /// Inner class to keep track of button states.   
    /// </summary>
    private class ButtonState
    {
        public bool isDown;
        public bool justDown;
        public bool justUp;


        public void UpdateButtonState(Wand wand, WandButton button)
        {
            justDown = false;
            justUp = false;

            if (CC_CONFIG.isDestiny())
            {
                string vrpnName = getDestinyName(wand, button);

                bool currentValue = ClusterInput.GetButton(vrpnName);

                if (!isDown && currentValue)
                {
                    justDown = true;
                }
                if (isDown && !currentValue)
                {
                    justUp = true;
                }
                isDown = currentValue;
            }

            if (CC_CONFIG.isInnovator())
            {
                string wandString = "";

                if (wand == Wand.Left)
                    wandString = "XInput0@";
                else if (wand == Wand.Right)
                    wandString = "XInput1@";

                int vrpnChannel = getInnovatorChannel(button);
                bool currentValue = VRPN.vrpnButton(wandString + CC_CONFIG.controllerIP, vrpnChannel);

                if (wand == Wand.Left || wand == Wand.Right)
                {
                    if (!isDown && currentValue)
                    {
                        justDown = true;
                    }
                    else if (isDown && !currentValue)
                    {
                        justUp = true;
                    }
                    isDown = currentValue;
                }
            }
        }

        public void UpdateDpadState(Wand wand, WandButton button)
        {
            justDown = false;
            justUp = false;

            if (CC_CONFIG.isDestiny())
            {
                string vrpnName = getDestinyName(wand, button);

                int currentDpadValue = (int)ClusterInput.GetAxis(vrpnName);

                bool activation = false;
                if (wand == Wand.Left) activation = dpadLeftActivated;
                else if (wand == Wand.Right) activation = dpadRightActivated;

                if (currentDpadValue == -1 && !activation)
                {
                    if (wand == Wand.Left) dpadLeftActivated = true;
                    else if (wand == Wand.Right) dpadRightActivated = true;
                }
                else if (activation)
                {
                    int dpadValue = getDpadValue(button);

                    if (!isDown && currentDpadValue == dpadValue)
                    {
                        justDown = true;
                    }
                    else if (isDown && currentDpadValue != dpadValue)
                    {
                        justUp = true;
                    }

                    if (currentDpadValue == dpadValue)
                        isDown = true;
                    else
                        isDown = false;
                }
                else
                {
                    isDown = false;
                    return;
                }
            }

            if (CC_CONFIG.isInnovator())
            {
                string wandString = "";

                if (wand == Wand.Left)
                    wandString = "XInput0@";
                else if (wand == Wand.Right)
                    wandString = "XInput1@";

                int vrpnChannel = getInnovatorChannel(button);
                int currentDpadValue = (int)VRPN.vrpnAnalog(wandString + CC_CONFIG.controllerIP, vrpnChannel);


                bool activation = false;
                if (wand == Wand.Left) activation = dpadLeftActivated;
                else if (wand == Wand.Right) activation = dpadRightActivated;

                if (currentDpadValue == -1 && !activation)
                {
                    if (wand == Wand.Left) dpadLeftActivated = true;
                    else if (wand == Wand.Right) dpadRightActivated = true;
                }
                else if (activation)
                {
                    int dpadValue = getDpadValue(button);

                    if (!isDown && currentDpadValue == dpadValue)
                    {
                        justDown = true;
                    }
                    else if (isDown && currentDpadValue != dpadValue)
                    {
                        justUp = true;
                    }

                    if (currentDpadValue == dpadValue)
                        isDown = true;
                    else
                        isDown = false;
                }
                else
                {
                    isDown = false;
                    return;
                }

            }
        }
    }

    private static ButtonState GetButtonState(Wand wand, WandButton button)
    {
        switch (button)
        {
            case WandButton.X:
                if (wand == Wand.Left) return leftX;
                else if (wand == Wand.Right) return rightX;
                break;
            case WandButton.O:
                if (wand == Wand.Left) return leftO;
                else if (wand == Wand.Right) return rightO;
                break;
            case WandButton.Shoulder:
                if (wand == Wand.Left) return leftShoulder;
                else if (wand == Wand.Right) return rightShoulder;
                break;
            case WandButton.Up:
                if (wand == Wand.Left) return leftDUp;
                else if (wand == Wand.Right) return rightDUp;
                break;
            case WandButton.Right:
                if (wand == Wand.Left) return leftDRight;
                else if (wand == Wand.Right) return rightDRight;
                break;
            case WandButton.Down:
                if (wand == Wand.Left) return leftDDown;
                else if (wand == Wand.Right) return rightDDown;
                break;
            case WandButton.Left:
                if (wand == Wand.Left) return leftDLeft;
                else if (wand == Wand.Right) return rightDLeft;
                break;
            default:
                break;
        }
        throw new System.ArgumentException("Button does not exist.");
    }


    /// <summary>
    /// Inner class to keep track of axis state
    /// </summary>
    private class AxisState
    {
        public float axisValue;

        public void UpdateAxisState(Wand wand, WandAxis axis)
        {
            axisValue = 0.0f;

            if (CC_CONFIG.isDestiny())
            {
                string vrpnName = getDestinyName(wand, axis);

                float currentValue = ClusterInput.GetAxis(vrpnName);

                if (currentValue > 1.0f || currentValue < -1.0f || float.IsNaN(currentValue))
                {
                    axisValue = 0.0f;
                    return;
                }
                else
                {
                    axisValue = currentValue;
                }

            }

            if (CC_CONFIG.isInnovator())
            {
                int vrpnChannel = getInnovatorChannel(axis);

                if (wand == Wand.Left)
                {
                    axisValue = (float)VRPN.vrpnAnalog("XInput0@" + CC_CONFIG.controllerIP, vrpnChannel);
                }
                else if (wand == Wand.Right)
                {
                    axisValue = (float)VRPN.vrpnAnalog("XInput1@" + CC_CONFIG.controllerIP, vrpnChannel);
                }
                else
                {
                    axisValue = 0.0f;
                }

            }
        }

    }

    private static AxisState GetAxisState(Wand wand, WandAxis axis)
    {
        switch (axis)
        {
            case WandAxis.XAxis:
                if (wand == Wand.Left) return leftXAxis;
                else if (wand == Wand.Right) return rightXAxis;
                break;
            case WandAxis.YAxis:
                if (wand == Wand.Left) return leftYAxis;
                else if (wand == Wand.Right) return rightYAxis;
                break;
            case WandAxis.Trigger:
                if (wand == Wand.Left) return leftTrigger;
                else if (wand == Wand.Right) return rightTrigger;
                break;
            default:
                break;
        }
        throw new System.ArgumentException("Axis does not exist.");
    }

    //Check to see if this button is a dpad button
    private static bool isDpad(WandButton button)
    {
        return (button == WandButton.Up || button == WandButton.Right || button == WandButton.Down || button == WandButton.Left);
    }

    //Get the buttons equivalent simulator key
    private static KeyCode getSimKey(WandButton button)
    {
        switch (button)
        {
            case WandButton.X:
                return KeyCode.G;
            case WandButton.O:
                return KeyCode.L;
            case WandButton.Shoulder:
                return KeyCode.B;
            case WandButton.Up:
                return KeyCode.UpArrow;
            case WandButton.Right:
                return KeyCode.RightArrow;
            case WandButton.Down:
                return KeyCode.DownArrow;
            case WandButton.Left:
                return KeyCode.LeftArrow;
            default:
                break;
        }
        throw new System.ArgumentException("Simulator key does not exist.");
    }

    //Get the axis equivalent simulator key
    private static KeyCode getSimAxis1(WandAxis axis)
    {
        switch (axis)
        {
            case WandAxis.XAxis:
                return KeyCode.K;
            case WandAxis.YAxis:
                return KeyCode.U;
            case WandAxis.Trigger:
                return KeyCode.Space;
            default:
                break;
        }
        throw new System.ArgumentException("Simulator axis does not exist.");
    }

    //Get the axis equivalent simulator key
    private static KeyCode getSimAxis2(WandAxis axis)
    {
        switch (axis)
        {
            case WandAxis.XAxis:
                return KeyCode.H;
            case WandAxis.YAxis:
                return KeyCode.J;
            case WandAxis.Trigger:
                return KeyCode.Space;
            default:
                break;
        }
        throw new System.ArgumentException("Simulator axis does not exist.");
    }

    //XInput uses an Axis for the dpad, but we want to treat the dpad directions as buttons.
    //This returns the value for the corresponding button so we can compare them to see which button is pressed
    private static int getDpadValue(WandButton button)
    {
        switch (button)
        {
            case WandButton.Up:
                return 0;
            case WandButton.Right:
                return 90;
            case WandButton.Down:
                return 180;
            case WandButton.Left:
                return 270;
            default:
                break;
        }
        throw new System.ArgumentException("Dpad button does not exist.");

    }

    //Get the corresponding Destiny Cluster Input name for the corresponding button 
    //These value was set within CC_TRACKER
    private static string getDestinyName(Wand wand, WandButton button)
    {
        switch (button)
        {
            case WandButton.X:
                if (wand == Wand.Left) return "leftX";
                else if (wand == Wand.Right) return "rightX";
                break;
            case WandButton.O:
                if (wand == Wand.Left) return "leftO";
                else if (wand == Wand.Right) return "rightO";
                break;
            case WandButton.Shoulder:
                if (wand == Wand.Left) return "leftShoulder";
                else if (wand == Wand.Right) return "rightShoulder";
                break;
            case WandButton.Up:
                if (wand == Wand.Left) return "leftDpad";
                else if (wand == Wand.Right) return "rightDpad";
                break;
            case WandButton.Right:
                if (wand == Wand.Left) return "leftDpad";
                else if (wand == Wand.Right) return "rightDpad";
                break;
            case WandButton.Down:
                if (wand == Wand.Left) return "leftDpad";
                else if (wand == Wand.Right) return "rightDpad";
                break;
            case WandButton.Left:
                if (wand == Wand.Left) return "leftDpad";
                else if (wand == Wand.Right) return "rightDpad";
                break;
            default:
                break;
        }
        throw new System.ArgumentException("Destiny button does not exist.");
    }

    //Get the corresponding Destiny Cluster Input name for the corresponding axis 
    //This value was set within CC_TRACKER
    private static string getDestinyName(Wand wand, WandAxis axis)
    {
        switch (axis)
        {
            case WandAxis.XAxis:
                if (wand == Wand.Left) return "leftXAxis";
                else if (wand == Wand.Right) return "rightXAxis";
                break;
            case WandAxis.YAxis:
                if (wand == Wand.Left) return "leftYAxis";
                else if (wand == Wand.Right) return "rightYAxis";
                break;
            case WandAxis.Trigger:
                if (wand == Wand.Left) return "leftTriggerAxis";
                else if (wand == Wand.Right) return "rightTriggerAxis";
                break;

            default:
                break;
        }
        throw new System.ArgumentException("Destiny axis does not exist.");
    }

    //Get the corresponding Innovator VRPN channel for the button 
    private static int getInnovatorChannel(WandButton button)
    {
        switch (button)
        {
            case WandButton.X:
                return 0;
            case WandButton.O:
                return 1;
            case WandButton.Shoulder:
                return 4;
            case WandButton.Up:
                return 4;
            case WandButton.Right:
                return 4;
            case WandButton.Down:
                return 4;
            case WandButton.Left:
                return 4;
            default:
                break;
        }
        throw new System.ArgumentException("Innovator button does not exist.");

    }

    //Get the corresponding Innovator VRPN channel for the axis 
    private static int getInnovatorChannel(WandAxis axis)
    {
        switch (axis)
        {
            case WandAxis.XAxis:
                return 0;
            case WandAxis.YAxis:
                return 1;
            case WandAxis.Trigger:
                return 5;
            default:
                break;
        }
        throw new System.ArgumentException("Innovator axis does not exist.");

    }

    //Updates all the button and axis states
    private static void UpdateAllStates()
    {
        if (updateCalled) return;

        leftX.UpdateButtonState(Wand.Left, WandButton.X);
        rightX.UpdateButtonState(Wand.Right, WandButton.X);

        leftO.UpdateButtonState(Wand.Left, WandButton.O);
        rightO.UpdateButtonState(Wand.Right, WandButton.O);

        leftShoulder.UpdateButtonState(Wand.Left, WandButton.Shoulder);
        rightShoulder.UpdateButtonState(Wand.Right, WandButton.Shoulder);

        leftDUp.UpdateDpadState(Wand.Left, WandButton.Up);
        rightDUp.UpdateDpadState(Wand.Right, WandButton.Up);

        leftDLeft.UpdateDpadState(Wand.Left, WandButton.Left);
        rightDLeft.UpdateDpadState(Wand.Right, WandButton.Left);

        leftDDown.UpdateDpadState(Wand.Left, WandButton.Down);
        rightDDown.UpdateDpadState(Wand.Right, WandButton.Down);

        leftDRight.UpdateDpadState(Wand.Left, WandButton.Right);
        rightDRight.UpdateDpadState(Wand.Right, WandButton.Right);

        leftXAxis.UpdateAxisState(Wand.Left, WandAxis.XAxis);
        rightXAxis.UpdateAxisState(Wand.Right, WandAxis.XAxis);

        leftYAxis.UpdateAxisState(Wand.Left, WandAxis.YAxis);
        rightYAxis.UpdateAxisState(Wand.Right, WandAxis.YAxis);

        leftTrigger.UpdateAxisState(Wand.Left, WandAxis.Trigger);
        rightTrigger.UpdateAxisState(Wand.Right, WandAxis.Trigger);

        updateCalled = true;

    }

    //Checks to see if we are still in the current frame so we can correctly set button states
    private static bool StillCurrentFrame()
    {

        int currFrame = Time.frameCount;

        if (savedFrameCount == currFrame)
        {
            savedFrameCount = currFrame;
            return true;
        }
        else
        {
            updateCalled = false;
            savedFrameCount = currFrame;
            return false;
        }

    }

}
