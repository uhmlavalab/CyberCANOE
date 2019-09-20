using UnityEngine;

/*
Keeps track of the head and wand positions and rotations by interfacing over VRPN or through ClusterInput.

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: 1.14, August 6th, 2019.
 */

/// <summary> Retrives information of the head and wand positions and rotations by interfacing with MotiveDirect. </summary>
public class CC_TRACKER : MonoBehaviour {
    private static CC_CANOE canoe;

    private Vector3 headPosition = new Vector3(0f, 1.75f, 0f);
    private Quaternion headRotation = Quaternion.identity;

    private Vector3[] wandPosition;
    private Quaternion[] wandRotation;

    //Simulator Settings
    private float wandXSpan = 2.0f;
    private float wandYSpan = 1.5f;
    private float wandZSpan = 2.0f;
    private float headSpan = 360;
    private float headXSpan = 2;
    private float headYSpan = 1;
    private float headZSpan = 1.83f;
    private float shoulderHeight = 1.5f;
    private float armDistanceInFront = 1.0f;
    private int twistStarted = 0;
    private int twistHeadStarted = 0;
    private int rotateStarted = 0;
    private Quaternion saveRotation = Quaternion.identity;
    private Quaternion saveHeadRotation = Quaternion.identity;

    private KeyCode yaw_pitchKey = KeyCode.Y;
    private KeyCode rollKey = KeyCode.I;
    private KeyCode left_right_up_downKey = KeyCode.N;
    private KeyCode left_right_in_outKey = KeyCode.Less;

    void Start() {
        wandPosition = new Vector3[2];
        wandRotation = new Quaternion[2];
        setDefaultPositions();
        canoe = GameObject.Find("CC_CANOE").GetComponent<CC_CANOE>();
        if (canoe.isVive()) {
            yaw_pitchKey = KeyCode.T;
            rollKey = KeyCode.O;
            left_right_up_downKey = KeyCode.V;
            left_right_in_outKey = KeyCode.Comma;
            if (CC_CONFIG.IsDestiny()) {
                addViveClusterInputs();
            }
        } else if (canoe.isOptiTrack() && CC_CONFIG.IsDestiny()) {
            addOptiTrackClusterInputs();
        }
    }

    void Update() {
        if (CC_CONFIG.tracking) {
            getTrackerInformation();
        } else {
            //Mouse movement.
            float mouseX = Input.mousePosition.x / Screen.width;
            float mouseY = Input.mousePosition.y / Screen.height;

            //Wand Movement/Roll.
            wandSimulation(mouseX, mouseY);

            //Head Movement/Roll.
            headSimulation(mouseX, mouseY);

            //Reset simulator positions
            if (Input.GetKeyDown(KeyCode.Return) && CC_CANOE.keyboardControls) {
                setDefaultPositions();
            }
        }
    }

    /// <summary>
    /// Set the default positions for the head and wand when in simulator mode.
    /// </summary>
    private void setDefaultPositions() {
        headPosition = new Vector3(0f, 1.75f, 0f);
        headRotation = Quaternion.identity;
        wandPosition[0] = new Vector3(-0.2f, 1.35f, 1f);
        wandRotation[0] = Quaternion.identity;
        wandPosition[1] = new Vector3(0.2f, 1.35f, 1f);
        wandRotation[1] = Quaternion.identity;
    }

    /// <summary>
    /// Inputs for head movements in simulator mode.
    /// </summary>
    /// <param name="mouseX">Mouse x-position.</param>
    /// <param name="mouseY">Mouse y-position.</param>
    private void headSimulation(float mouseX, float mouseY) {

        float same;

        //Turn head.
        if (Input.GetKey(KeyCode.Q) && CC_CANOE.keyboardControls) {
            headRotation = Quaternion.AngleAxis(mouseX * headSpan - (headSpan / 2), Vector3.up) * Quaternion.AngleAxis(-(mouseY * headSpan / 2 - (headSpan / 4)), Vector3.right);
        }

        //Move the head left/right and forward/backward.
        if (Input.GetKey(KeyCode.Z) && CC_CANOE.keyboardControls) {
            same = headPosition.z;
            headPosition = new Vector3(mouseX * headXSpan - (headXSpan / 2), mouseY * headYSpan / 2 + shoulderHeight, same);
            //Moving the Character Controller with the head movement
            same = CC_CANOE.CanoeCharacterController().center.y;
            CC_CANOE.CanoeCharacterController().center = new Vector3(headPosition.x, same, headPosition.z);
        }

        //Move the head left/right and up/down.
        if (Input.GetKey(KeyCode.C) && CC_CANOE.keyboardControls) {
            same = headPosition.y;
            headPosition = new Vector3(mouseX * headXSpan - (headXSpan / 2), same, mouseY * headZSpan / 2);
            //Moving the Character Controller with the head movement
            same = CC_CANOE.CanoeCharacterController().center.y;
            CC_CANOE.CanoeCharacterController().center = new Vector3(headPosition.x, same, headPosition.z);
        }

        //Roll the head
        if (Input.GetKey(KeyCode.E) && CC_CANOE.keyboardControls) {
            if (twistHeadStarted == 0) {
                twistHeadStarted = 1;
                saveHeadRotation = headRotation;
            } else {
                headRotation = saveHeadRotation * Quaternion.AngleAxis(-(mouseX * 180 - 90), Vector3.forward);
            }
        } else {
            twistHeadStarted = 0;
        }
    }


    /// <summary>
    /// Inputs for wand movements in simulator mode.
    /// </summary>
    /// <param name="mouseX">Mouse x-position.</param>
    /// <param name="mouseY">Mouse y-position.</param>
    private void wandSimulation(float mouseX, float mouseY) {
        //Move wand left/right and up/down.
        if (Input.GetKey(left_right_up_downKey) && CC_CANOE.keyboardControls) {
            wandPosition[(int)CC_CANOE.simActiveWand] = new Vector3(mouseX * wandXSpan - (wandXSpan / 2), mouseY * wandYSpan - (wandYSpan / 2) + shoulderHeight, armDistanceInFront);
        }

        //Move wand left/right and forward/backward.
        if (Input.GetKey(left_right_in_outKey) && CC_CANOE.keyboardControls) {
            wandPosition[(int)CC_CANOE.simActiveWand] = new Vector3(mouseX * wandXSpan - (wandXSpan / 2), shoulderHeight, mouseY * wandZSpan);
        }

        //Yaw/pitch wand.
        if (Input.GetKey(yaw_pitchKey) && CC_CANOE.keyboardControls) {
            if (rotateStarted == 0) {
                rotateStarted = 1;
                saveRotation = wandRotation[(int)CC_CANOE.simActiveWand];
            } else {
                wandRotation[(int)CC_CANOE.simActiveWand] = saveRotation * Quaternion.AngleAxis(mouseX * 180 - 90, Vector3.up) * Quaternion.AngleAxis(-(mouseY * 180 - 90), Vector3.right);

            }
        } else {
            rotateStarted = 0;
        }

        //Roll Wand.
        if (Input.GetKey(rollKey) && CC_CANOE.keyboardControls) {
            if (twistStarted == 0) {
                twistStarted = 1;
                saveRotation = wandRotation[(int)CC_CANOE.simActiveWand];
            } else {
                wandRotation[(int)CC_CANOE.simActiveWand] = saveRotation * Quaternion.AngleAxis(-(mouseX * 180 - 90), Vector3.forward);
            }
        } else {
            twistStarted = 0;
        }
    }

    /// <summary>
    /// Obtain tracker information.
    /// </summary>
    private void getTrackerInformation() {
        if (CC_CONFIG.IsDestiny()) {
            Vector3 position = new Vector3(0, 0, 0);
            Quaternion rotation = Quaternion.identity;

            if (canoe.isVive()) {

                //Update head ClusterInput entry
                position = ClusterInput.GetTrackerPosition("head");
                rotation = ClusterInput.GetTrackerRotation("head");
                //Angle puck so flat section is bottom
                //Change "Camera Forward Tilt" in editor to change tilt if hat/helmet is angled
                rotation *= Quaternion.AngleAxis(90 - canoe.cameraForwardTilt, Vector3.left);
                rotation *= Quaternion.AngleAxis(180, Vector3.up);
                convertHeadTracking(position, rotation);

                //Update left controller ClusterInput entries
                position = ClusterInput.GetTrackerPosition("leftWand");
                rotation = ClusterInput.GetTrackerRotation("leftWand");
                convertWandTracking(Wand.Left, position, rotation);

                //Update right controller ClusterInput entries
                position = ClusterInput.GetTrackerPosition("rightWand");
                rotation = ClusterInput.GetTrackerRotation("rightWand");
                convertWandTracking(Wand.Right, position, rotation);

                //Moving the Character Controller with the head movement
                float same = CC_CANOE.CanoeCharacterController().center.y;
                CC_CANOE.CanoeCharacterController().center = new Vector3(headPosition.x, same, headPosition.z);

            } else if (canoe.isOptiTrack()) {
                //Head position and rotation
                position = ClusterInput.GetTrackerPosition("head");
                //position = new Vector3(position.x, position.y, -position.z);
                rotation = ClusterInput.GetTrackerRotation("head");
                convertHeadTracking(position, rotation);

                //Left wand position and rotation
                position = ClusterInput.GetTrackerPosition("leftWand");
                rotation = ClusterInput.GetTrackerRotation("leftWand");
                convertWandTracking(Wand.Left, position, rotation);

                //Right wand position and rotation
                position = ClusterInput.GetTrackerPosition("rightWand");
                rotation = ClusterInput.GetTrackerRotation("rightWand");
                convertWandTracking(Wand.Right, position, rotation);

                //Moving the Character Controller with the head movement
                float same = CC_CANOE.CanoeCharacterController().center.y;

                CC_CANOE.CanoeCharacterController().center = new Vector3(headPosition.x, same, headPosition.z);
            }
        } else {
            if (checkInnovatorTracking()) {
                headPosition = convertToLeftHandPosition(VRPN.vrpnTrackerPos("CC_FLAT_HEAD@" + CC_CONFIG.trackerIP, 0));
                headRotation = convertToLeftHandRotation(VRPN.vrpnTrackerQuat("CC_FLAT_HEAD@" + CC_CONFIG.trackerIP, 0));

                //Moving the Character Controller with the head movement
                float same = CC_CANOE.CanoeCharacterController().center.y;
                CC_CANOE.CanoeCharacterController().center = new Vector3(headPosition.x, same, headPosition.z);

                //Wands rotation and position.
                wandPosition[0] = convertToLeftHandPosition(VRPN.vrpnTrackerPos("CC_FLAT_WAND0@" + CC_CONFIG.trackerIP, 0));
                wandPosition[1] = convertToLeftHandPosition(VRPN.vrpnTrackerPos("CC_FLAT_WAND1@" + CC_CONFIG.trackerIP, 0));
                wandRotation[0] = convertToLeftHandRotation(VRPN.vrpnTrackerQuat("CC_FLAT_WAND0@" + CC_CONFIG.trackerIP, 0));
                wandRotation[1] = convertToLeftHandRotation(VRPN.vrpnTrackerQuat("CC_FLAT_WAND1@" + CC_CONFIG.trackerIP, 0));
            } else {
                setDefaultPositions();
            }
        }
    }

    /// <summary>
    /// Add the Destiny ClusterInput settings for Vive for the correct syncing through all clients and masters.
    /// This uses VRPN servers that are launched on Kanaloa 1.
    /// </summary>
    private static void addViveClusterInputs() {
        //Head and wand ClusterInput entries added
        ClusterInput.AddInput("head", "CC_FLAT_HEAD", CC_CONFIG.serverIP, 0, ClusterInputType.Tracker);
        ClusterInput.AddInput("leftWand", "CC_FLAT_WAND0", CC_CONFIG.serverIP, 0, ClusterInputType.Tracker);
        ClusterInput.AddInput("rightWand", "CC_FLAT_WAND1", CC_CONFIG.serverIP, 0, ClusterInputType.Tracker);

        //Left wand axis ClusterInput entries added
        ClusterInput.AddInput("leftXAxis", "CC_FLAT_WAND0", CC_CONFIG.serverIP, 0, ClusterInputType.CustomProvidedInput);
        ClusterInput.AddInput("leftYAxis", "CC_FLAT_WAND0", CC_CONFIG.serverIP, 1, ClusterInputType.CustomProvidedInput);
        ClusterInput.AddInput("leftTriggerAxis", "CC_FLAT_WAND0", CC_CONFIG.serverIP, 2, ClusterInputType.CustomProvidedInput);

        //Left wand button ClusterInput entries added
        ClusterInput.AddInput("leftMenu", "CC_FLAT_WAND0", CC_CONFIG.serverIP, 1, ClusterInputType.Button);
        ClusterInput.AddInput("leftGrip", "CC_FLAT_WAND0", CC_CONFIG.serverIP, 2, ClusterInputType.Button);
        ClusterInput.AddInput("leftTrackpad", "CC_FLAT_WAND0", CC_CONFIG.serverIP, 32, ClusterInputType.CustomProvidedInput);
        ClusterInput.AddInput("leftTrigger", "CC_FLAT_WAND0", CC_CONFIG.serverIP, 33, ClusterInputType.CustomProvidedInput);

        //Right wand axis ClusterInput entries added
        ClusterInput.AddInput("rightXAxis", "CC_FLAT_WAND1", CC_CONFIG.serverIP, 0, ClusterInputType.CustomProvidedInput);
        ClusterInput.AddInput("rightYAxis", "CC_FLAT_WAND1", CC_CONFIG.serverIP, 1, ClusterInputType.CustomProvidedInput);
        ClusterInput.AddInput("rightTriggerAxis", "CC_FLAT_WAND1", CC_CONFIG.serverIP, 2, ClusterInputType.CustomProvidedInput);

        //Right wand button ClusterInput entries added
        ClusterInput.AddInput("rightMenu", "CC_FLAT_WAND1", CC_CONFIG.serverIP, 1, ClusterInputType.Button);
        ClusterInput.AddInput("rightGrip", "CC_FLAT_WAND1", CC_CONFIG.serverIP, 2, ClusterInputType.Button);
        ClusterInput.AddInput("rightTrackpad", "CC_FLAT_WAND1", CC_CONFIG.serverIP, 32, ClusterInputType.CustomProvidedInput);
        ClusterInput.AddInput("rightTrigger", "CC_FLAT_WAND1", CC_CONFIG.serverIP, 33, ClusterInputType.CustomProvidedInput);
    }

    /// <summary>
    /// Add the Destiny ClusterInput settings for OptiTrack for the correct syncing through all clients and masters.
    /// This uses VRPN servers that are launched on Kanaloa 1.
    /// </summary>
    private static void addOptiTrackClusterInputs() {
        //Head entry added
        ClusterInput.AddInput("head", "CC_FLAT_HEAD", CC_CONFIG.trackerIP, 0, ClusterInputType.Tracker);
        //Wand entries added
        ClusterInput.AddInput("leftWand", "CC_FLAT_WAND0", CC_CONFIG.trackerIP, 0, ClusterInputType.Tracker);
        ClusterInput.AddInput("rightWand", "CC_FLAT_WAND1", CC_CONFIG.trackerIP, 0, ClusterInputType.Tracker);

        //Left axis entries added
        ClusterInput.AddInput("leftXAxis", "XInput0", CC_CONFIG.controllerIP, 0, ClusterInputType.Axis);
        ClusterInput.AddInput("leftYAxis", "XInput0", CC_CONFIG.controllerIP, 1, ClusterInputType.Axis);
        ClusterInput.AddInput("leftTriggerAxis", "XInput0", CC_CONFIG.controllerIP, 5, ClusterInputType.Axis);
        ClusterInput.AddInput("leftDpad", "XInput0", CC_CONFIG.controllerIP, 4, ClusterInputType.Axis);
        //Left button entries added
        ClusterInput.AddInput("leftX", "XInput0", CC_CONFIG.controllerIP, 0, ClusterInputType.Button);
        ClusterInput.AddInput("leftO", "XInput0", CC_CONFIG.controllerIP, 1, ClusterInputType.Button);
        ClusterInput.AddInput("leftShoulder", "XInput0", CC_CONFIG.controllerIP, 4, ClusterInputType.Button);

        //Right axis entries added
        ClusterInput.AddInput("rightXAxis", "XInput1", CC_CONFIG.controllerIP, 0, ClusterInputType.Axis);
        ClusterInput.AddInput("rightYAxis", "XInput1", CC_CONFIG.controllerIP, 1, ClusterInputType.Axis);
        ClusterInput.AddInput("rightTriggerAxis", "XInput1", CC_CONFIG.controllerIP, 5, ClusterInputType.Axis);
        ClusterInput.AddInput("rightDpad", "XInput1", CC_CONFIG.controllerIP, 4, ClusterInputType.Axis);
        //Right button entries added
        ClusterInput.AddInput("rightX", "XInput1", CC_CONFIG.controllerIP, 0, ClusterInputType.Button);
        ClusterInput.AddInput("rightO", "XInput1", CC_CONFIG.controllerIP, 1, ClusterInputType.Button);
        ClusterInput.AddInput("rightShoulder", "XInput1", CC_CONFIG.controllerIP, 4, ClusterInputType.Button);
    }

    /// <summary>
    /// Converts wand position and rotation to using convertToLeftHandPosition.
    /// </summary>
    /// <param name="wand">Wand controller (left/right).</param>
    /// <param name="position">Position obtained from VRPN.</param>
    /// <param name="rotation">Rotation obtained from VRPN.</param>
    private void convertWandTracking(Wand wand, Vector3 position, Quaternion rotation) {
        if (!float.IsNaN(position.x) && !float.IsNaN(position.y) && !float.IsNaN(position.z)) {
            if ((position.x < 10) && (position.x > -10)
                && (position.y < 10) && (position.y > -10)
                && (position.z < 10) && (position.z > -10)) {
                wandPosition[(int)wand] = convertToLeftHandPosition(position);
            }
        }
        if (!float.IsNaN(rotation.x) && !float.IsNaN(rotation.y) && !float.IsNaN(rotation.z) && !float.IsNaN(rotation.w)) {
            wandRotation[(int)wand] = convertToLeftHandRotation(rotation);
        }
    }

    /// <summary>
    /// Converts head position and rotation to using convertToLeftHandPosition.
    /// </summary>
    /// <param name="wand">Wand controller (left/right).</param>
    /// <param name="position">Position obtained from VRPN.</param>
    /// <param name="rotation">Rotation obtained from VRPN.</param>
    private void convertHeadTracking(Vector3 position, Quaternion rotation) {
        if (!float.IsNaN(position.x) && !float.IsNaN(position.y) && !float.IsNaN(position.z)) {
            if ((position.x < 10) && (position.x > -10)
                && (position.y < 10) && (position.y > -10)
                && (position.z < 10) && (position.z > -10)) {
                headPosition = convertToLeftHandPosition(position);
            }
        }
        if (!float.IsNaN(rotation.x) && !float.IsNaN(rotation.y) && !float.IsNaN(rotation.z) && !float.IsNaN(rotation.w)) {
            headRotation = convertToLeftHandRotation(rotation);
        }
    }

    /// <summary>
    /// Check to see if innovator tracking is valid.
    /// </summary>
    private bool checkInnovatorTracking() {
        if (VRPN.vrpnTrackerPos("CC_FLAT_HEAD@" + CC_CONFIG.trackerIP, 0) == new Vector3(-505, -505, -505)) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Returns head position.
    /// </summary>
    public Vector3 GetHeadPosition() {
        return headPosition;
    }

    /// <summary>
    /// Returns head rotation.
    /// </summary>
    public Quaternion GetHeadRotation() {
        return headRotation;
    }

    /// <summary>
    /// Returns position of wand.
    /// </summary>
    /// <param name="wandEnum">Wand value cast as int.</param>
    public Vector3 GetWandPosition(int wandEnum) {
        return wandPosition[wandEnum];
    }

    /// <summary>
    /// Returns rotation of wand.
    /// </summary>
    /// <param name="wandEnum">Wand value cast as int</param>
    public Quaternion GetWandRotation(int wandEnum) {
        return wandRotation[wandEnum];
    }

    /// <summary>
    /// Sets and returns position with correctly inverted values.
    /// </summary>
    /// <param name="position">Original vector to be converted.</param>
    private Vector3 convertToLeftHandPosition(Vector3 position) {
        return new Vector3(position.x, position.y, -position.z);
    }

    /// <summary>
    /// Sets and returns rotation with correctly inverted values.
    /// </summary>
    /// <param name="quaternion">Original quaternion to be converted.</param>
    private Quaternion convertToLeftHandRotation(Quaternion quaternion) {
        return new Quaternion(-quaternion.x, -quaternion.y, quaternion.z, quaternion.w);
    }

    /// <summary>
    /// Returns true if computer is master.
    /// </summary>
    public bool IsMaster() {
        return (CC_CAMERA.GetCameraIndex() == 0);
    }
}
