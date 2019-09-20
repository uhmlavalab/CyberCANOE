using UnityEngine;

using System.Collections;

/*
The input class for interfacing with the CyberCANOE Wands.

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: 1.14, August 6th, 2019.
 */

/// <summary>
/// Enumerated identifiers for the left or right wand.
/// </summary>
public enum Wand {
    Left = 0,
    Right = 1
}

/// <summary>
/// Enumerated identifiers for wand buttons.
/// </summary>
public enum WandButton {
    //Shared Input
    Up, Down, Left, Right,
    //Vive-Only Input
    UpLeft,
    UpRight,
    DownLeft,
    DownRight,
    Center,
    TrackpadClick,
    Menu, Grip, Trigger,
    //OptiTrack-Only Input
    X, O,
    Shoulder,
}

/// <summary>
/// Enumerated identifiers for wand axis.
/// </summary>
public enum WandAxis {
    XAxis = 0,
    YAxis = 1,
    Trigger = 2,
}

/// <summary>
/// Enumerated identifiers for Dpad types.
/// </summary>
public enum DpadType {
    FourDirections = 0,
    EightDirections = 1,
}

/// <summary>
/// Input interface for the wand controllers.
/// </summary>
public static class CC_INPUT {
    private static CC_CANOE canoe = GameObject.Find("CC_CANOE").GetComponent<CC_CANOE>();
    /// <summary>
    /// Returns true if the 'button' is held down by the specified wand. 
    /// <para>Vive: Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight,
    /// Center, TrackpadClick, Menu, Grip, Trigger</para>
    /// <para>OptiTrack: Up, Down, Left, Right, X, O, Shoulder</para>
    /// </summary>
    /// <param name="wand">Wand controller (left/right).</param>
    /// <param name="button"><para>Wand button.</para>
    /// </param>
    public static bool GetButtonPress(Wand wand, WandButton button) {
        if (!stillCurrentFrame()) {
            updateAllStates();
        }

        KeyCode simKey = KeyCode.F12;

        if (canoe.isVive() && IsViveInput(button)) {
            simKey = getViveSimKey(button);
        } else if (canoe.isOptiTrack() && IsOptiTrackInput(button)) {
            simKey = getOptiTrackSimKey(button);
        }

        if (Input.GetKey(simKey) && (CC_CANOE.simActiveWand == wand) && CC_CANOE.keyboardControls) {
            if (isDiagonalDirection(button) && (GetDpadType() != DpadType.EightDirections)) {
                return false;
            }
            return true;
        } else if (CC_CONFIG.IsDestiny() || CC_CONFIG.IsInnovator()) {
            if (IsViveInput(button)) {
                return getViveButtonState(wand, button).isDown;
            }
            return getOptiTrackButtonState(wand, button).isDown;
        }
        return false;
    }

    /// <summary>
    /// Returns true at the frame the 'button' starts to press down (not held down) by the specified 'wand'. 
    /// <para>Vive: Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight,
    /// Center, TrackpadClick, Menu, Grip, Trigger</para>
    /// <para>OptiTrack: Up, Down, Left, Right, X, O, Shoulder</para>
    /// </summary>
    /// <param name="wand">Wand controller (left/right).</param>
    /// <param name="button">Wand button.</param>
    public static bool GetButtonDown(Wand wand, WandButton button) {
        if (!stillCurrentFrame()) {
            updateAllStates();
        }

        KeyCode simKey = KeyCode.F12;

        if (canoe.isVive() && IsViveInput(button)) {
            simKey = getViveSimKey(button);
        } else if (canoe.isOptiTrack() && IsOptiTrackInput(button)) {
            simKey = getOptiTrackSimKey(button);
        }

        if (Input.GetKeyDown(simKey) && (CC_CANOE.simActiveWand == wand) && CC_CANOE.keyboardControls) {
            if (isDiagonalDirection(button) && (GetDpadType() != DpadType.EightDirections)) {
                return false;
            }
            return true;
        } else if (CC_CONFIG.IsDestiny() || CC_CONFIG.IsInnovator()) {
            if (IsViveInput(button)) {
                return getViveButtonState(wand, button).justDown;
            }
            return getOptiTrackButtonState(wand, button).justDown;
        }
        return false;
    }

    /// <summary>
    /// Returns true at the frame the 'button' starts to be released by the specified 'wand'. 
    /// <para>Vive: Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight,
    /// Center, TrackpadClick, Menu, Grip, Trigger</para>
    /// <para>OptiTrack: Up, Down, Left, Right, X, O, Shoulder</para>
    /// </summary>
    /// <param name="wand">Wand controller (left/right).</param>
    /// <param name="button">Wand button.</param>
    public static bool GetButtonUp(Wand wand, WandButton button) {
        if (!stillCurrentFrame()) {
            updateAllStates();
        }

        KeyCode simKey = KeyCode.F12;

        if (canoe.isVive() && IsViveInput(button)) {
            simKey = getViveSimKey(button);
        } else if (canoe.isOptiTrack() && IsOptiTrackInput(button)) {
            simKey = getOptiTrackSimKey(button);
        }

        if (Input.GetKeyUp(simKey) && (CC_CANOE.simActiveWand == wand) && CC_CANOE.keyboardControls) {
            if (isDiagonalDirection(button) && (GetDpadType() != DpadType.EightDirections)) {
                return false;
            }
            return true;
        } else if (CC_CONFIG.IsDestiny() || CC_CONFIG.IsInnovator()) {
            if (IsViveInput(button)) {
                return getViveButtonState(wand, button).justUp;
            }
            return getOptiTrackButtonState(wand, button).justUp;
        }
        return false;
    }

    /// <summary>
    /// Returns the float number of the 'Axis' for the specified wand. 
    /// </summary>
    /// <param name="wand">Wand controller (left/right).</param>
    /// <param name="axis">Wand axis (x-axis, y-axis, trigger).</param>
    public static float GetAxis(Wand wand, WandAxis axis) {
        if (!stillCurrentFrame()) {
            updateAllStates();
        }

        KeyCode simAxis1 = KeyCode.F12;
        KeyCode simAxis2 = KeyCode.F12;

        if (canoe.isVive()) {
            simAxis1 = getViveSimAxis1(axis);
            simAxis2 = getViveSimAxis2(axis);
        } else if (canoe.isOptiTrack()) {
            simAxis1 = getOptiTrackSimAxis1(axis);
            simAxis2 = getOptiTrackSimAxis2(axis);
        }


        if ((CC_CANOE.simActiveWand == wand) && (Input.GetKey(simAxis1) || Input.GetKey(simAxis2)) && CC_CANOE.keyboardControls) {
            if (Input.GetKey(simAxis1) && CC_CANOE.keyboardControls) {
                return 1.0f;
            } else {
                return -1.0f;
            }
        } else if (CC_CONFIG.IsDestiny() || CC_CONFIG.IsInnovator()) {
            return getAxisState(wand, axis).axisValue;
        }
        return 0f;
    }

    //Shared Left ButtonStates
    private static ButtonState leftDUp = new ButtonState();
    private static ButtonState leftDLeft = new ButtonState();
    private static ButtonState leftDDown = new ButtonState();
    private static ButtonState leftDRight = new ButtonState();
    //Shared Right ButtonStates
    private static ButtonState rightDUp = new ButtonState();
    private static ButtonState rightDLeft = new ButtonState();
    private static ButtonState rightDDown = new ButtonState();
    private static ButtonState rightDRight = new ButtonState();

    //Shared Left AxisStates
    private static AxisState leftYAxis = new AxisState();
    private static AxisState leftXAxis = new AxisState();
    private static AxisState leftTriggerAxis = new AxisState();
    //Shared Right AxisStates
    private static AxisState rightYAxis = new AxisState();
    private static AxisState rightXAxis = new AxisState();
    private static AxisState rightTriggerAxis = new AxisState();

    //Vive-Only Left ButtonStates
    private static ButtonState leftDUpLeft = new ButtonState();
    private static ButtonState leftDUpRight = new ButtonState();
    private static ButtonState leftDDownLeft = new ButtonState();
    private static ButtonState leftDDownRight = new ButtonState();
    private static ButtonState leftDCenter = new ButtonState();
    private static ButtonState leftMenu = new ButtonState();
    private static ButtonState leftGrip = new ButtonState();
    private static ButtonState leftTrigger = new ButtonState();
    private static ButtonState leftTrackpadClick = new ButtonState();
    //Vive-Only Right ButtonStates
    private static ButtonState rightDUpLeft = new ButtonState();
    private static ButtonState rightDUpRight = new ButtonState();
    private static ButtonState rightDDownLeft = new ButtonState();
    private static ButtonState rightDDownRight = new ButtonState();
    private static ButtonState rightDCenter = new ButtonState();
    private static ButtonState rightMenu = new ButtonState();
    private static ButtonState rightGrip = new ButtonState();
    private static ButtonState rightTrigger = new ButtonState();
    private static ButtonState rightTrackpadClick = new ButtonState();

    //OptiTrack-Only Left ButtonStates
    private static ButtonState leftX = new ButtonState();
    private static ButtonState leftO = new ButtonState();
    private static ButtonState leftShoulder = new ButtonState();
    //OptiTrack-Only Right ButtonStates
    private static ButtonState rightX = new ButtonState();
    private static ButtonState rightO = new ButtonState();
    private static ButtonState rightShoulder = new ButtonState();

    private static int savedFrameCount = -1;
    private static bool updateCalled = false;

    private static bool dpadLeftActivated = false;
    private static bool dpadRightActivated = false;

    /// <summary>
    /// Inner class to keep track of button states.   
    /// </summary>
    private class ButtonState {
        public bool isDown = false;
        public bool justDown = false;
        public bool justUp = false;

        /// <summary>
        /// Updates ButtonState depending on incoming and current values.
        /// <para>Vive: Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight,
        /// Center, TrackpadClick, Menu, Grip, Trigger</para>
        /// <para>OptiTrack: Up, Down, Left, Right, X, O, Shoulder</para>
        /// </summary>
        /// <param name="wand"> Wand controller (left/right).</param>
        /// <param name="button">Wand button.</param>
        public void UpdateButtonState(Wand wand, WandButton button) {
            justDown = false;
            justUp = false;

            if (canoe.isVive() && IsViveInput(button)) {
                string vrpnName = "";
                bool currentValue = false;

                vrpnName = getViveName(wand, button);

                currentValue = ClusterInput.GetButton(vrpnName);

                if (!isDown && currentValue) {
                    justDown = true;
                }
                if (isDown && !currentValue) {
                    justUp = true;
                }


                isDown = currentValue;
            } else if (canoe.isOptiTrack() && IsOptiTrackInput(button)) {
                string vrpnName = getOptiTrackName(wand, button);

                bool currentValue = ClusterInput.GetButton(vrpnName);

                if (!isDown && currentValue) {
                    justDown = true;
                }
                if (isDown && !currentValue) {
                    justUp = true;
                }
                isDown = currentValue;
            } else if (CC_CONFIG.IsInnovator()) {
                string wandString = "";

                if (wand == Wand.Left)
                    wandString = "XInput0@";
                else if (wand == Wand.Right)
                    wandString = "XInput1@";

                int vrpnChannel = getInnovatorChannel(button);
                bool currentValue = VRPN.vrpnButton(wandString + CC_CONFIG.controllerIP, vrpnChannel);

                if (wand == Wand.Left || wand == Wand.Right) {
                    if (!isDown && currentValue) {
                        justDown = true;
                    } else if (isDown && !currentValue) {
                        justUp = true;
                    }
                    isDown = currentValue;
                }
            }
        }

        /// <summary>
        /// Updates ButtonStates of Dpad input.
        /// <para>Vive Dpad: Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight, Center</para>
        /// <para>OptiTrack Dpad: Up, Down, Left, Right</para>
        /// </summary>
        /// <param name="wand">Wand controller (left/right).</param>
        /// <param name="button">Wand button.</param>
        public void UpdateDpadState(Wand wand, WandButton button) {
            justDown = false;
            justUp = false;

            if (!isDpad(button)) { throw new System.ArgumentException("Button is not Dpad button."); }

            if (canoe.isVive()) {

                //VRPN name of axis entries
                string vrpnName = "";

                //Gets x-axis trackpad value
                vrpnName = getAxisName(wand, WandAxis.XAxis);
                float x = ClusterInput.GetAxis(vrpnName);

                //Gets y-axis trackpad value
                vrpnName = getAxisName(wand, WandAxis.YAxis);
                float y = ClusterInput.GetAxis(vrpnName);

                //Get trackpad clicked value
                vrpnName = getViveName(wand, WandButton.TrackpadClick);
                bool trackpadClicked = ClusterInput.GetButton(vrpnName);

                //Gets current Dpad value
                int currentDpadValue = getDpadDirection(canoe.dpadType, x, y);
                //Casts WandButton to int to compare to current Dpad
                int dpadValue = (int)button;

                if (!isDown && (currentDpadValue == dpadValue) && trackpadClicked) {
                    justDown = true;
                } else if (isDown && !trackpadClicked) {
                    justUp = true;
                } else if (isDown && (currentDpadValue != dpadValue) && trackpadClicked) {
                    justUp = true;
                }

                if ((currentDpadValue == dpadValue) && trackpadClicked) {
                    isDown = true;
                } else {
                    isDown = false;
                }

            } else if (canoe.isOptiTrack()) {
                string vrpnName = getOptiTrackName(wand, button);

                int currentDpadValue = (int)ClusterInput.GetAxis(vrpnName);

                bool activation = false;
                if (wand == Wand.Left) activation = dpadLeftActivated;
                else if (wand == Wand.Right) activation = dpadRightActivated;

                if (currentDpadValue == -1 && !activation) {
                    if (wand == Wand.Left) dpadLeftActivated = true;
                    else if (wand == Wand.Right) dpadRightActivated = true;
                } else if (activation) {
                    int dpadValue = getDpadValue(button);

                    if (!isDown && currentDpadValue == dpadValue) {
                        justDown = true;
                    } else if (isDown && currentDpadValue != dpadValue) {
                        justUp = true;
                    }

                    if (currentDpadValue == dpadValue)
                        isDown = true;
                    else
                        isDown = false;
                } else {
                    isDown = false;
                    return;
                }
            } else if (CC_CONFIG.IsInnovator()) {
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

                if (currentDpadValue == -1 && !activation) {
                    if (wand == Wand.Left) dpadLeftActivated = true;
                    else if (wand == Wand.Right) dpadRightActivated = true;
                } else if (activation) {
                    int dpadValue = getDpadValue(button);

                    if (!isDown && currentDpadValue == dpadValue) {
                        justDown = true;
                    } else if (isDown && currentDpadValue != dpadValue) {
                        justUp = true;
                    }

                    if (currentDpadValue == dpadValue)
                        isDown = true;
                    else
                        isDown = false;
                } else {
                    isDown = false;
                    return;
                }
            }
        }
    }

    /// <summary>
    /// <para>Returns Vive ButtonState based on left/right wand and WandButton.</para>
    /// Buttons:
    /// Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight,
    /// Center, TrackpadClick, Menu, Grip, Trigger
    /// </summary>
    /// <param name="wand">Wand controller (left/right).</param>
    /// <param name="button">Wand button.</param>
    private static ButtonState getViveButtonState(Wand wand, WandButton button) {
        switch (button) {
            case WandButton.Up:
                if (wand == Wand.Left) { return leftDUp; } else if (wand == Wand.Right) { return rightDUp; }
                break;
            case WandButton.Down:
                if (wand == Wand.Left) { return leftDDown; } else if (wand == Wand.Right) { return rightDDown; }
                break;
            case WandButton.Left:
                if (wand == Wand.Left) { return leftDLeft; } else if (wand == Wand.Right) { return rightDLeft; }
                break;
            case WandButton.Right:
                if (wand == Wand.Left) { return leftDRight; } else if (wand == Wand.Right) { return rightDRight; }
                break;
            case WandButton.UpLeft:
                if (wand == Wand.Left) { return leftDUpLeft; } else if (wand == Wand.Right) { return rightDUpLeft; }
                break;
            case WandButton.UpRight:
                if (wand == Wand.Left) { return leftDUpRight; } else if (wand == Wand.Right) { return rightDUpRight; }
                break;
            case WandButton.DownLeft:
                if (wand == Wand.Left) { return leftDDownLeft; } else if (wand == Wand.Right) { return rightDDownLeft; }
                break;
            case WandButton.DownRight:
                if (wand == Wand.Left) { return leftDDownRight; } else if (wand == Wand.Right) { return rightDDownRight; }
                break;
            case WandButton.Center:
                if (wand == Wand.Left) { return leftDCenter; } else if (wand == Wand.Right) { return rightDCenter; }
                break;
            case WandButton.TrackpadClick:
                if (wand == Wand.Left) { return leftTrackpadClick; } else if (wand == Wand.Right) { return rightTrackpadClick; }
                break;
            case WandButton.Menu:
                if (wand == Wand.Left) { return leftMenu; } else if (wand == Wand.Right) { return rightMenu; }
                break;
            case WandButton.Grip:
                if (wand == Wand.Left) { return leftGrip; } else if (wand == Wand.Right) { return rightGrip; }
                break;
            case WandButton.Trigger:
                if (wand == Wand.Left) { return leftTrigger; } else if (wand == Wand.Right) { return rightTrigger; }
                break;
            default:
                if (!CC_CANOE.keyboardControls) {
                    throw new System.ArgumentException("Non-Vive input used or button does not exist");
                }
                break;
        }
        throw new System.ArgumentException("Vive button does not exist");
    }

    /// <summary>
    /// <para>Returns OptiTrack ButtonState based on left/right wand and WandButton.</para>
    /// Buttons: Up, Down, Left, Right, X, O, Shoulder
    /// </summary>
    /// <param name="wand">Wand controller (left/right).</param>
    /// <param name="button">Wand button.</param>
    private static ButtonState getOptiTrackButtonState(Wand wand, WandButton button) {

        switch (button) {
            case WandButton.Up:
                if (wand == Wand.Left) { return leftDUp; } else if (wand == Wand.Right) { return rightDUp; }
                break;
            case WandButton.Down:
                if (wand == Wand.Left) { return leftDDown; } else if (wand == Wand.Right) { return rightDDown; }
                break;
            case WandButton.Left:
                if (wand == Wand.Left) { return leftDLeft; } else if (wand == Wand.Right) { return rightDLeft; }
                break;
            case WandButton.Right:
                if (wand == Wand.Left) { return leftDRight; } else if (wand == Wand.Right) { return rightDRight; }
                break;
            case WandButton.X:
                if (wand == Wand.Left) { return leftX; } else if (wand == Wand.Right) { return rightX; }
                break;
            case WandButton.O:
                if (wand == Wand.Left) { return leftO; } else if (wand == Wand.Right) { return rightO; }
                break;
            case WandButton.Shoulder:
                if (wand == Wand.Left) { return leftShoulder; } else if (wand == Wand.Right) { return rightShoulder; }
                break;
            default:
                if (!CC_CANOE.keyboardControls) {
                    throw new System.ArgumentException("Non-OptiTrack input used or button does not exist");
                }
                break;
        }
        throw new System.ArgumentException("OptiTrack button does not exist.");
    }

    /// <summary>
    /// Inner class to keep track of axis state
    /// </summary>
    private class AxisState {
        public float axisValue;

        /// <summary>
        /// Updates AxisState of joystick/trackpad.
        /// </summary>
        /// <param name="wand">Wand controller (left/right).</param>
        /// <param name="axis">Wand axis (x-axis, y-axis, trigger).</param>
        public void UpdateAxisState(Wand wand, WandAxis axis) {

            axisValue = 0.0f;

            if (CC_CONFIG.IsDestiny()) {

                string vrpnName = getAxisName(wand, axis);

                float currentValue = 0f;

                if (canoe.isVive()) {
                    currentValue = ClusterInput.GetAxis(vrpnName);
                    if ((currentValue > 1.0f) || (currentValue < -1.0f) || float.IsNaN(currentValue)) {
                        axisValue = 0.0f;
                    } else {
                        axisValue = currentValue;
                    }
                    return;
                } else if (canoe.isOptiTrack()) {
                    currentValue = ClusterInput.GetAxis(vrpnName);
                    if ((currentValue > 1.0f) || (currentValue < -1.0f) || float.IsNaN(currentValue)) {
                        axisValue = 0.0f;
                    } else {
                        axisValue = currentValue;
                    }
                    return;
                }
            }

            if (CC_CONFIG.IsInnovator()) {

                int vrpnChannel = getInnovatorChannel(axis);

                if (wand == Wand.Left) {
                    axisValue = (float)VRPN.vrpnAnalog("XInput0@" + CC_CONFIG.controllerIP, vrpnChannel);
                } else if (wand == Wand.Right) {
                    axisValue = (float)VRPN.vrpnAnalog("XInput1@" + CC_CONFIG.controllerIP, vrpnChannel);
                } else {
                    axisValue = 0.0f;
                }
            }
        }
    }

    /// <summary>
    /// Returns AxisState of wand axis.
    /// </summary>
    /// <param name="wand">Wand controller (left/right).</param>
    /// <param name="axis">Wand axis (x-axis, y-axis, trigger).</param>
    private static AxisState getAxisState(Wand wand, WandAxis axis) {
        switch (axis) {
            case WandAxis.XAxis:
                if (wand == Wand.Left) { return leftXAxis; } else if (wand == Wand.Right) { return rightXAxis; }
                break;
            case WandAxis.YAxis:
                if (wand == Wand.Left) { return leftYAxis; } else if (wand == Wand.Right) { return rightYAxis; }
                break;
            case WandAxis.Trigger:
                if (wand == Wand.Left) { return leftTriggerAxis; } else if (wand == Wand.Right) { return rightTriggerAxis; }
                break;
            default:
                break;
        }
        throw new System.ArgumentException("Axis does not exist.");
    }

    /// <summary>
    /// <para>Returns true if input is OptiTrack input.</para>
    /// Buttons: Up, Down, Left, Right, X, O, Shoulder
    /// </summary>
    /// <param name="button">Wand button.</param>
    public static bool IsOptiTrackInput(WandButton button) {
        switch (button) {
            case WandButton.Up:
            case WandButton.Down:
            case WandButton.Left:
            case WandButton.Right:
            case WandButton.X:
            case WandButton.O:
            case WandButton.Shoulder:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// <para>Returns true if the button is a diagonal direction on the Dpad.</para>
    /// Diagonals: UpLeft, UpRight, DownLeft, DownRight
    /// </summary>
    /// <param name="button"></param>
    private static bool isDiagonalDirection(WandButton button) {
        return ((button == WandButton.UpLeft) || (button == WandButton.UpRight)
            || (button == WandButton.DownLeft) || (button == WandButton.DownRight));
    }

    /// <summary>
    /// <para>Returns true if input is Vive input.</para>
    /// Buttons: 
    /// Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight,
    /// Center, TrackpadClick, Menu, Grip, Trigger
    /// </summary>
    /// <param name="button">Wand button.</param>
    public static bool IsViveInput(WandButton button) {
        switch (button) {
            case WandButton.Up:
            case WandButton.Down:
            case WandButton.Left:
            case WandButton.Right:
            case WandButton.UpLeft:
            case WandButton.UpRight:
            case WandButton.DownLeft:
            case WandButton.DownRight:
            case WandButton.Center:
            case WandButton.TrackpadClick:
            case WandButton.Menu:
            case WandButton.Grip:
            case WandButton.Trigger:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Returns true if the input is a seperator value in the WandButton enumeration.
    /// </summary>
    public static bool IsEnumSeparator(WandButton button) {
        return !(IsViveInput(button) || IsOptiTrackInput(button));
    }

    /// <summary>
    /// <para>Returns true if the button is a Dpad button.</para>
    /// Buttons: Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight, Center
    /// </summary>
    /// <param name="button">Wand button.</param>
    private static bool isDpad(WandButton button) {
        return ((button == WandButton.Up) || (button == WandButton.Down) || (button == WandButton.Left) || (button == WandButton.Right)
           || (button == WandButton.UpLeft) || (button == WandButton.UpRight) || (button == WandButton.DownLeft) || (button == WandButton.DownRight)
           || (button == WandButton.Center));
    }

    /// <summary>
    /// <para>Get the buttons equivalent Vive simulator key.</para>
    /// Buttons: 
    /// Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight,
    /// Center, TrackpadClick, Menu, Grip, Trigger
    /// </summary>
    /// <param name="button">Wand button.</param>
    private static KeyCode getViveSimKey(WandButton button) {
        switch (button) {
            case WandButton.Up:
                return KeyCode.U;
            case WandButton.Down:
                return KeyCode.N;
            case WandButton.Left:
                return KeyCode.H;
            case WandButton.Right:
                return KeyCode.K;
            case WandButton.UpLeft:
                return KeyCode.Y;
            case WandButton.UpRight:
                return KeyCode.I;
            case WandButton.DownLeft:
                return KeyCode.B;
            case WandButton.DownRight:
                return KeyCode.M;
            case WandButton.Center:
                return KeyCode.J;
            case WandButton.TrackpadClick:
                return KeyCode.F;
            case WandButton.Menu:
                return KeyCode.L;
            case WandButton.Grip:
                return KeyCode.G;
            case WandButton.Trigger:
                return KeyCode.Space;
            default:
                break;
        }
        throw new System.ArgumentException("Vive simulator key does not exist.");
    }

    /// <summary>
    /// <para>Get the buttons equivalent OptiTrack simulator key.</para>
    /// Buttons: 
    /// Up, Down, Left, Right, X, O, Shoulder
    /// </summary>
    /// <param name="button">Wand button.</param>
    private static KeyCode getOptiTrackSimKey(WandButton button) {
        switch (button) {
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
        throw new System.ArgumentException("OptiTrack simulator key does not exist.");
    }

    /// <summary>
    /// Get the positive axis equivalent Vive simulator key.
    /// </summary>
    /// <param name="axis">Wand axis (x-axis, y-axis, trigger).</param>
    private static KeyCode getViveSimAxis1(WandAxis axis) {
        switch (axis) {
            case WandAxis.XAxis:
                return KeyCode.RightArrow;
            case WandAxis.YAxis:
                return KeyCode.UpArrow;
            case WandAxis.Trigger:
                return KeyCode.Space;
        }
        throw new System.ArgumentException("Vive simulator axis does not exist.");
    }

    /// <summary>
    /// Get the negative axis equivalent Vive simulator key.
    /// </summary>
    /// <param name="axis">Wand axis (x-axis, y-axis, trigger).</param>
    private static KeyCode getViveSimAxis2(WandAxis axis) {
        switch (axis) {
            case WandAxis.XAxis:
                return KeyCode.LeftArrow;
            case WandAxis.YAxis:
                return KeyCode.DownArrow;
            case WandAxis.Trigger:
                return KeyCode.Space;
        }
        throw new System.ArgumentException("Vive simulator axis does not exist.");
    }


    /// <summary>
    /// Get the positive axis equivalent OptiTrack simulator key.
    /// </summary>
    /// <param name="axis">Wand axis (x-axis, y-axis, trigger).</param>
    private static KeyCode getOptiTrackSimAxis1(WandAxis axis) {
        switch (axis) {
            case WandAxis.XAxis:
                return KeyCode.K;
            case WandAxis.YAxis:
                return KeyCode.U;
            case WandAxis.Trigger:
                return KeyCode.Space;
            default:
                break;
        }
        throw new System.ArgumentException("OptiTrack simulator axis does not exist.");
    }

    /// <summary>
    /// Get the negative axis equivalent OptiTrack simulator key.
    /// </summary>
    /// <param name="axis">Wand axis (x-axis, y-axis, trigger).</param>
    private static KeyCode getOptiTrackSimAxis2(WandAxis axis) {
        switch (axis) {
            case WandAxis.XAxis:
                return KeyCode.H;
            case WandAxis.YAxis:
                return KeyCode.J;
            case WandAxis.Trigger:
                return KeyCode.Space;
            default:
                break;
        }
        throw new System.ArgumentException("OptiTrack simulator axis does not exist.");
    }

    /// <summary>
    /// Returns Dpad type, either four or eight directions.
    /// </summary>
    public static DpadType GetDpadType() {
        return canoe.dpadType;
    }

    /// <summary>
    /// Sets the Dpad type to either four directions or eight directions. 
    /// </summary>
    /// <param name="type">Selected Dpad type.</param>
    public static void SetDpadType(DpadType type) {
        if (type == DpadType.FourDirections) {
            canoe.dpadType = DpadType.FourDirections;
        } else {
            canoe.dpadType = DpadType.EightDirections;
        }
    }

    /// <summary>
    /// Returns range for Dpad center click.
    /// </summary>
    public static float GetCenterRange() {
        return canoe.centerClickRange;
    }

    /// <summary>
    /// Sets the range of Dpad center click.
    /// </summary>
    /// <param name="newRange">New range</param>
    public static void SetCenterRange(float newRange) {
        if (Mathf.Abs(newRange) < 1) {
            canoe.centerClickRange = Mathf.Abs(newRange);
        } else {
            throw new System.ArgumentException("Center range must be less that 1");
        }
    }

    /// <summary>
    /// Returns the enum value of the position clicked on the Dpad.
    /// </summary>
    /// <param name="dpadType">Dpad type.</param>
    /// <param name="x">Value of X-axis.</param>
    /// <param name="y">Value of the Y-axis.</param>
    private static int getDpadDirection(DpadType dpadType, float x, float y) {
        float distFromCenter = Mathf.Sqrt((x * x) + (y * y));
        if (distFromCenter <= canoe.centerClickRange) {
            return (int)WandButton.Center;
        } else if (dpadType == DpadType.FourDirections) {
            float deg = Mathf.Atan2(y, x) * (180f / Mathf.PI);
            if ((deg >= 45) && (deg <= 135)) {
                return (int)WandButton.Up;
            }
            if ((deg >= -135) && (deg <= -45)) {
                return (int)WandButton.Down;
            }
            if ((Mathf.Abs(deg) > 135) && (Mathf.Abs(deg) <= 180)) {
                return (int)WandButton.Left;
            }
            if ((Mathf.Abs(deg) >= 0) && (Mathf.Abs(deg) < 45)) {
                return (int)WandButton.Right;
            }
        } else if (dpadType == DpadType.EightDirections) {
            float deg = Mathf.Atan2(y, x) * (180 / Mathf.PI);
            if ((deg >= 22.5) && (deg < 67.5)) {
                return (int)WandButton.UpRight;
            }
            if ((deg >= 67.5) && (deg < 112.5)) {
                return (int)WandButton.Up;
            }
            if ((deg >= 112.5) && (deg < 157.5)) {
                return (int)WandButton.UpLeft;
            }
            if ((deg >= -157.5) && (deg < -112.5)) {
                return (int)WandButton.DownLeft;
            }
            if ((deg >= -112.5) && (deg < -67.5)) {
                return (int)WandButton.Down;
            }
            if ((deg >= -67.5) && (deg < -22.5)) {
                return (int)WandButton.DownRight;
            }
            if ((Mathf.Abs(deg) >= 157.5) && (Mathf.Abs(deg) <= 180)) {
                return (int)WandButton.Left;
            }
            if ((Mathf.Abs(deg) >= 0) && (Mathf.Abs(deg) < 22.5)) {
                return (int)WandButton.Right;
            }
        }
        return -1;
    }

    /// <summary>
    /// Returns Destiny channel for button input.
    /// </summary>
    /// <param name="button">Wand button.   </param>
    private static int getDestinyChannel(WandButton button) {
        switch (button) {
            case WandButton.Menu:
                return 1;
            case WandButton.Grip:
                return 2;
            case WandButton.TrackpadClick:
                return 32;
            case WandButton.Trigger:
                return 33;
            default:
                break;
        }
        throw new System.ArgumentException("Vive button does not exist.");
    }

    /// <summary>
    /// XInput uses an Axis for the Dpad, but we want to treat the Dpad directions as buttons.
    /// This returns the value for the corresponding button so we can compare them to see which button is pressed.
    /// </summary>
    /// <param name="button">Wand button.</param>
    private static int getDpadValue(WandButton button) {
        switch (button) {
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

    /// <summary>
    /// <para>Get the corresponding OptiTrack Cluster Input name for the corresponding button. These values set withint CC_TRACKER.</para>
    /// Buttons: Up, Down, Left, Right, X, O, Shoulder
    /// </summary>
    /// <param name="wand">Wand controller (left/right).</param>
    /// <param name="button">Wand button.</param>
    private static string getOptiTrackName(Wand wand, WandButton button) {
        switch (button) {
            case WandButton.Up:
                if (wand == Wand.Left) { return "leftDpad"; } else if (wand == Wand.Right) { return "rightDpad"; }
                break;
            case WandButton.Down:
                if (wand == Wand.Left) { return "leftDpad"; } else if (wand == Wand.Right) { return "rightDpad"; }
                break;
            case WandButton.Left:
                if (wand == Wand.Left) { return "leftDpad"; } else if (wand == Wand.Right) { return "rightDpad"; }
                break;
            case WandButton.Right:
                if (wand == Wand.Left) { return "leftDpad"; } else if (wand == Wand.Right) { return "rightDpad"; }
                break;
            case WandButton.X:
                if (wand == Wand.Left) { return "leftX"; } else if (wand == Wand.Right) { return "rightX"; }
                break;
            case WandButton.O:
                if (wand == Wand.Left) { return "leftO"; } else if (wand == Wand.Right) { return "rightO"; }
                break;
            case WandButton.Shoulder:
                if (wand == Wand.Left) { return "leftShoulder"; } else if (wand == Wand.Right) { return "rightShoulder"; }
                break;
            default:
                break;
        }
        throw new System.ArgumentException("OptiTrack button does not exist.");
    }

    /// <summary>
    /// <para>Get the corresponding Vive Cluster Input name for the corresponding button. These values set withint CC_TRACKER.</para>
    /// Buttons: 
    /// Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight,
    /// Center, TrackpadClick, Menu, Grip, Trigger
    /// </summary>
    /// <param name="wand">Wand controller (left/right).</param>
    /// <param name="button">Wand button.</param>
    private static string getViveName(Wand wand, WandButton button) {
        switch (button) {
            case WandButton.Up:
            case WandButton.Right:
            case WandButton.Down:
            case WandButton.Left:
            case WandButton.UpLeft:
            case WandButton.UpRight:
            case WandButton.DownLeft:
            case WandButton.DownRight:
            case WandButton.Center:
            case WandButton.TrackpadClick:
                if (wand == Wand.Left) { return "leftTrackpad"; } else if (wand == Wand.Right) { return "rightTrackpad"; }
                break;
            case WandButton.Menu:
                if (wand == Wand.Left) { return "leftMenu"; } else if (wand == Wand.Right) { return "rightMenu"; }
                break;
            case WandButton.Grip:
                if (wand == Wand.Left) { return "leftGrip"; } else if (wand == Wand.Right) { return "rightGrip"; }
                break;
            case WandButton.Trigger:
                if (wand == Wand.Left) { return "leftTrigger"; } else if (wand == Wand.Right) { return "rightTrigger"; }
                break;
            default:
                break;
        }
        throw new System.ArgumentException("Vive button does not exist.");
    }

    /// <summary>
    /// <para>Get the corresponding Cluster Input name for the corresponding axis. This value was set within CC_TRACKER.</para>
    /// </summary>
    /// <param name="wand">Wand controller (left/right).</param>
    /// <param name="axis">Wand axes (x-axis, y-axis, trigger).</param>
    private static string getAxisName(Wand wand, WandAxis axis) {
        switch (axis) {
            case WandAxis.XAxis:
                if (wand == Wand.Left) { return "leftXAxis"; } else if (wand == Wand.Right) { return "rightXAxis"; }
                break;
            case WandAxis.YAxis:
                if (wand == Wand.Left) { return "leftYAxis"; } else if (wand == Wand.Right) { return "rightYAxis"; }
                break;
            case WandAxis.Trigger:
                if (wand == Wand.Left) { return "leftTriggerAxis"; } else if (wand == Wand.Right) { return "rightTriggerAxis"; }
                break;
            default:
                break;
        }
        throw new System.ArgumentException("Destiny axis does not exist.");
    }

    /// <summary>
    /// Get the corresponding Innovator VRPN channel for the button.
    /// </summary>
    /// <param name="button">Wand button.</param>
    private static int getInnovatorChannel(WandButton button) {
        switch (button) {
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

    /// <summary>
    /// Get the corresponding Innovator VRPN channel for the axis.
    /// </summary>
    /// <param name="axis">Wand axis (x-axis, y-axis, trigger).</param>
    private static int getInnovatorChannel(WandAxis axis) {
        switch (axis) {
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

    /// <summary>
    /// Updates all the button and axis states.
    /// </summary>
    private static void updateAllStates() {

        if (updateCalled) { return; }

        /* Axes updates. */
        leftXAxis.UpdateAxisState(Wand.Left, WandAxis.XAxis);
        rightXAxis.UpdateAxisState(Wand.Right, WandAxis.XAxis);

        leftYAxis.UpdateAxisState(Wand.Left, WandAxis.YAxis);
        rightYAxis.UpdateAxisState(Wand.Right, WandAxis.YAxis);

        leftTriggerAxis.UpdateAxisState(Wand.Left, WandAxis.Trigger);
        rightTriggerAxis.UpdateAxisState(Wand.Right, WandAxis.Trigger);

        /* D-Pad updates. */
        leftDUp.UpdateDpadState(Wand.Left, WandButton.Up);
        rightDUp.UpdateDpadState(Wand.Right, WandButton.Up);

        leftDLeft.UpdateDpadState(Wand.Left, WandButton.Left);
        rightDLeft.UpdateDpadState(Wand.Right, WandButton.Left);

        leftDDown.UpdateDpadState(Wand.Left, WandButton.Down);
        rightDDown.UpdateDpadState(Wand.Right, WandButton.Down);

        leftDRight.UpdateDpadState(Wand.Left, WandButton.Right);
        rightDRight.UpdateDpadState(Wand.Right, WandButton.Right);

        if (canoe.isVive()) {
            leftMenu.UpdateButtonState(Wand.Left, WandButton.Menu);
            rightMenu.UpdateButtonState(Wand.Right, WandButton.Menu);

            leftGrip.UpdateButtonState(Wand.Left, WandButton.Grip);
            rightGrip.UpdateButtonState(Wand.Right, WandButton.Grip);

            leftTrigger.UpdateButtonState(Wand.Left, WandButton.Trigger);
            rightTrigger.UpdateButtonState(Wand.Right, WandButton.Trigger);

            leftTrackpadClick.UpdateButtonState(Wand.Left, WandButton.TrackpadClick);
            rightTrackpadClick.UpdateButtonState(Wand.Right, WandButton.TrackpadClick);

            leftDUpLeft.UpdateDpadState(Wand.Left, WandButton.UpLeft);
            rightDUpLeft.UpdateDpadState(Wand.Right, WandButton.UpLeft);

            leftDUpRight.UpdateDpadState(Wand.Left, WandButton.UpRight);
            rightDUpRight.UpdateDpadState(Wand.Right, WandButton.UpRight);

            leftDDownLeft.UpdateDpadState(Wand.Left, WandButton.DownLeft);
            rightDDownLeft.UpdateDpadState(Wand.Right, WandButton.DownLeft);

            leftDDownRight.UpdateDpadState(Wand.Left, WandButton.DownRight);
            rightDDownRight.UpdateDpadState(Wand.Right, WandButton.DownRight);

            leftDCenter.UpdateDpadState(Wand.Left, WandButton.Center);
            rightDCenter.UpdateDpadState(Wand.Right, WandButton.Center);
        } else if (canoe.isOptiTrack()) {
            leftX.UpdateButtonState(Wand.Left, WandButton.X);
            rightX.UpdateButtonState(Wand.Right, WandButton.X);

            leftO.UpdateButtonState(Wand.Left, WandButton.O);
            rightO.UpdateButtonState(Wand.Right, WandButton.O);

            leftShoulder.UpdateButtonState(Wand.Left, WandButton.Shoulder);
            rightShoulder.UpdateButtonState(Wand.Right, WandButton.Shoulder);
        }
        updateCalled = true;
    }

    /// <summary>
    /// Checks to see if we are still in the current frame so we can correctly set button states.
    /// </summary>
    private static bool stillCurrentFrame() {

        int currFrame = Time.frameCount;

        if (savedFrameCount == currFrame) {
            savedFrameCount = currFrame;
            return true;
        } else {
            updateCalled = false;
            savedFrameCount = currFrame;
            return false;
        }
    }
}
