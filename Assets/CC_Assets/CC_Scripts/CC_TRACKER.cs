using UnityEngine;

/*
Keeps track of the head and wand positions and rotations by interfacing over VRPN or through ClusterInput.

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: 1.13, May 17th, 2017.
 */

/// <summary> Retrives information of the head and wand positions and rotations by interfacing with MotiveDirect. </summary>
public class CC_TRACKER : MonoBehaviour
{
    private Vector3 headPosition;
    private Quaternion headRotation;


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
    private Quaternion saveRotation;
    private Quaternion saveHeadRotation;

    void Start()
    {
        if (CC_CONFIG.isDestiny())
        {
            AddDestinyClusterInputs();
        }

        wandPosition = new Vector3[2];
        wandRotation = new Quaternion[2];
        setDefaultPositions();

    }

    void Update()
    {
        if (CC_CONFIG.tracking)
        {
            getTrackerInformation();
        }
        else
        {
            //Mouse movement.
            float mouseX = Input.mousePosition.x / Screen.width;
            float mouseY = Input.mousePosition.y / Screen.height;

            //Wand Movement/Roll.
            wandSimulation(mouseX, mouseY);

            //Head Movement/Roll.
            headSimulation(mouseX, mouseY);

            //Reset simulator positions
            if (Input.GetKeyDown(KeyCode.Return) && CC_CANOE.keyboardControls)
            {
                setDefaultPositions();
            }

        }

    }

    //Set the default positions for the head and wand when in simulatorm ode
    private void setDefaultPositions()
    {
        headPosition = new Vector3(0f, 1.75f, 0f);
        headRotation = Quaternion.identity;
        wandPosition[0] = new Vector3(-0.2f, 1.35f, 1f);
        wandRotation[0] = Quaternion.identity;
        wandPosition[1] = new Vector3(0.2f, 1.35f, 1f);
        wandRotation[1] = Quaternion.identity;

    }

 
    //Inputs for head movements in simulator mode.
    private void headSimulation(float mouseX, float mouseY)
    {

        float same;

        //Turn head.
        if (Input.GetKey(KeyCode.Q) && CC_CANOE.keyboardControls)
        {
            headRotation = Quaternion.AngleAxis(mouseX * headSpan - (headSpan / 2), Vector3.up) * Quaternion.AngleAxis(-(mouseY * headSpan / 2 - (headSpan / 4)), Vector3.right);

        }

        //Move the head left/right and forward/backward.
        if (Input.GetKey(KeyCode.Z) && CC_CANOE.keyboardControls)
        {
            same = headPosition.z;
            headPosition = new Vector3(mouseX * headXSpan - (headXSpan / 2), mouseY * headYSpan / 2 + shoulderHeight, same);
            //Moving the Character Controller with the head movement
            same = CC_CANOE.CanoeCharacterController().center.y;
            CC_CANOE.CanoeCharacterController().center = new Vector3(headPosition.x, same, headPosition.z);
        }

        //Move the head left/right and up/down.
        if (Input.GetKey(KeyCode.C) && CC_CANOE.keyboardControls)
        {
            same = headPosition.y;
            headPosition = new Vector3(mouseX * headXSpan - (headXSpan / 2), same, mouseY * headZSpan / 2);
            //Moving the Character Controller with the head movement
            same = CC_CANOE.CanoeCharacterController().center.y;
            CC_CANOE.CanoeCharacterController().center = new Vector3(headPosition.x, same, headPosition.z);
        }

        //Roll the head
        if (Input.GetKey(KeyCode.E) && CC_CANOE.keyboardControls)
        {
            if (twistHeadStarted == 0)
            {
                twistHeadStarted = 1;
                saveHeadRotation = headRotation;
            }
            else
            {
                headRotation = saveHeadRotation * Quaternion.AngleAxis(-(mouseX * 180 - 90), Vector3.forward);
            }
        }
        else
        {
            twistHeadStarted = 0;
        }

    }


    //Inputs for wand movements in simulator mode.
    private void wandSimulation(float mouseX, float mouseY)
    {
        //Move wand left/right and up/down.
        if (Input.GetKey(KeyCode.N) && CC_CANOE.keyboardControls)
        {
            wandPosition[(int)CC_CANOE.simActiveWand] = new Vector3(mouseX * wandXSpan - (wandXSpan / 2), mouseY * wandYSpan - (wandYSpan / 2) + shoulderHeight, armDistanceInFront);
        }

        //Move wand left/right and forward/backward.
        if (Input.GetKey(KeyCode.Comma) && CC_CANOE.keyboardControls)
        {
            wandPosition[(int)CC_CANOE.simActiveWand] = new Vector3(mouseX * wandXSpan - (wandXSpan / 2), shoulderHeight, mouseY * wandZSpan);
        }

        //Yaw/pitch wand.
        if (Input.GetKey(KeyCode.Y) && CC_CANOE.keyboardControls)
        {
            if (rotateStarted == 0)
            {
                rotateStarted = 1;
                saveRotation = wandRotation[(int)CC_CANOE.simActiveWand];
            }
            else
            {
                wandRotation[(int)CC_CANOE.simActiveWand] = saveRotation * Quaternion.AngleAxis(mouseX * 180 - 90, Vector3.up) * Quaternion.AngleAxis(-(mouseY * 180 - 90), Vector3.right);

            }
        }
        else
        {
            rotateStarted = 0;
        }

        //Roll Wand.
        if (Input.GetKey(KeyCode.I) && CC_CANOE.keyboardControls)
        {
            if (twistStarted == 0)
            {
                twistStarted = 1;
                saveRotation = wandRotation[(int)CC_CANOE.simActiveWand];
            }
            else
            {
                wandRotation[(int)CC_CANOE.simActiveWand] = saveRotation * Quaternion.AngleAxis(-(mouseX * 180 - 90), Vector3.forward);
            }
        }
        else
        {
            twistStarted = 0;
        }
    }



    //Obtain tracker information
    private void getTrackerInformation()
    {

        if (CC_CONFIG.isDestiny())
        {
            Vector3 position = new Vector3(0, 0, 0);
            Quaternion rotation = Quaternion.identity;

            //Head position and rotation
            position = ClusterInput.GetTrackerPosition("head");
            rotation = ClusterInput.GetTrackerRotation("head");

            if (!float.IsNaN(position.x) && !float.IsNaN(position.y) && !float.IsNaN(position.z))
            {
                if ((position.x < 10 && position.x > -10)
                    && (position.y < 10 && position.y > -10)
                    && (position.z < 10 && position.z > -10))
                {
                    headPosition = convertToLeftHandPosition(position);
                }
            }
            if (!float.IsNaN(rotation.x) && !float.IsNaN(rotation.y) && !float.IsNaN(rotation.z) && !float.IsNaN(rotation.w))
            {
                headRotation = convertToLeftHandRotation(rotation);
            }

            //Left Wand position and rotation
            position = ClusterInput.GetTrackerPosition("leftWand");
            rotation = ClusterInput.GetTrackerRotation("leftWand");

            if (!float.IsNaN(position.x) && !float.IsNaN(position.y) && !float.IsNaN(position.z))
            {
                if ((position.x < 10 && position.x > -10)
                    && (position.y < 10 && position.y > -10)
                    && (position.z < 10 && position.z > -10))
                {
                    wandPosition[(int)Wand.Left] = convertToLeftHandPosition(position);
                }
            }
            if (!float.IsNaN(rotation.x) && !float.IsNaN(rotation.y) && !float.IsNaN(rotation.z) && !float.IsNaN(rotation.w))
            {
                wandRotation[(int)Wand.Left] = convertToLeftHandRotation(rotation);
            }

            //Right Wand position and rotation
            position = ClusterInput.GetTrackerPosition("rightWand");
            rotation = ClusterInput.GetTrackerRotation("rightWand");

            if (!float.IsNaN(position.x) && !float.IsNaN(position.y) && !float.IsNaN(position.z))
            {
                if ((position.x < 10 && position.x > -10)
                    && (position.y < 10 && position.y > -10)
                    && (position.z < 10 && position.z > -10))
                {
                    wandPosition[(int)Wand.Right] = convertToLeftHandPosition(position);
                }
            }
            if (!float.IsNaN(rotation.x) && !float.IsNaN(rotation.y) && !float.IsNaN(rotation.z) && !float.IsNaN(rotation.w))
            {
                wandRotation[(int)Wand.Right] = convertToLeftHandRotation(rotation);
            }

            //Moving the Character Controller with the head movement
            float same = CC_CANOE.CanoeCharacterController().center.y;
            CC_CANOE.CanoeCharacterController().center = new Vector3(headPosition.x, same, headPosition.z);

        }
        else
        {
            if (checkInnovatorTracking())
            {
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
            }
            else
            {
                setDefaultPositions();
            }

        }

    }

    /// <summary>
    /// Add the Destiny ClusterInput settings for the correct syncing through all clients and masters.
    /// This uses VRPN servers that are launched on Kanaloa 1.
    /// </summary>
    private static void AddDestinyClusterInputs()
    {

        ClusterInput.AddInput("head", "CC_FLAT_HEAD", CC_CONFIG.trackerIP, 0, ClusterInputType.Tracker);
        ClusterInput.AddInput("leftWand", "CC_FLAT_WAND0", CC_CONFIG.trackerIP, 0, ClusterInputType.Tracker);
        ClusterInput.AddInput("rightWand", "CC_FLAT_WAND1", CC_CONFIG.trackerIP, 0, ClusterInputType.Tracker);

        ClusterInput.AddInput("leftXAxis", "XInput0", CC_CONFIG.controllerIP, 0, ClusterInputType.Axis);
        ClusterInput.AddInput("leftYAxis", "XInput0", CC_CONFIG.controllerIP, 1, ClusterInputType.Axis);
        ClusterInput.AddInput("leftTriggerAxis", "XInput0", CC_CONFIG.controllerIP, 5, ClusterInputType.Axis);
        ClusterInput.AddInput("leftDpad", "XInput0", CC_CONFIG.controllerIP, 4, ClusterInputType.Axis);
        ClusterInput.AddInput("leftX", "XInput0", CC_CONFIG.controllerIP, 0, ClusterInputType.Button);
        ClusterInput.AddInput("leftO", "XInput0", CC_CONFIG.controllerIP, 1, ClusterInputType.Button);
        ClusterInput.AddInput("leftShoulder", "XInput0", CC_CONFIG.controllerIP, 4, ClusterInputType.Button);

        ClusterInput.AddInput("rightXAxis", "XInput1", CC_CONFIG.controllerIP, 0, ClusterInputType.Axis);
        ClusterInput.AddInput("rightYAxis", "XInput1", CC_CONFIG.controllerIP, 1, ClusterInputType.Axis);
        ClusterInput.AddInput("rightTriggerAxis", "XInput1", CC_CONFIG.controllerIP, 5, ClusterInputType.Axis);
        ClusterInput.AddInput("rightDpad", "XInput1", CC_CONFIG.controllerIP, 4, ClusterInputType.Axis);
        ClusterInput.AddInput("rightX", "XInput1", CC_CONFIG.controllerIP, 0, ClusterInputType.Button);
        ClusterInput.AddInput("rightO", "XInput1", CC_CONFIG.controllerIP, 1, ClusterInputType.Button);
        ClusterInput.AddInput("rightShoulder", "XInput1", CC_CONFIG.controllerIP, 4, ClusterInputType.Button);

    }

    //Check to see if innovator tracking is valid.
    private bool checkInnovatorTracking()
    {
        if (VRPN.vrpnTrackerPos("CC_FLAT_HEAD@" + CC_CONFIG.trackerIP, 0) == new Vector3(-505, -505, -505))
        {
            return false;
        }
        return true;
    }

    public Vector3 getHeadPosition()
    {
        return headPosition;
    }

    public Quaternion getHeadRotation()
    {
        return headRotation;
    }

    public Vector3 getWandPosition(int x)
    {
        return wandPosition[x];
    }

    public Quaternion getWandRotation(int x)
    {
        return wandRotation[x];
    }

    private Vector3 convertToLeftHandPosition(Vector3 p)
    {
        return new Vector3(p.x, p.y, -p.z);
    }

    private Quaternion convertToLeftHandRotation(Quaternion q)
    {
        return new Quaternion(-q.x, -q.y, q.z, q.w);
    }

}
