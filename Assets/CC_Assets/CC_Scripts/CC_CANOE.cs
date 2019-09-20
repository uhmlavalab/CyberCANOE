using UnityEngine;

/* 
The main class the user interfaces with to set settings of the CC_CANOE.
Users can also access information about the head and wands by using static methods within this class.
CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: 1.14, August 6th, 2019.
 */

/// <summary> The main class to interface with to retrieve Wand, Head, and CharacterController information. </summary>
public class CC_CANOE : MonoBehaviour {
    public float navigationSpeed = 5.0f;
    public float navigationRotationSpeed = 1.25f;

    public enum WandModel { None, Hand, Axis };
    public WandModel wandModel = WandModel.Hand;
    private WandModel savedWandModel;

    public bool vive = true;
    public bool optitrack = false;

    public Wand simulatorActiveWand;
    public static Wand simActiveWand;
    public DpadType dpadType = DpadType.EightDirections;
    public float centerClickRange = 0.3f;
    public float cameraForwardTilt;

    public enum ShowScreen { None, Innovator, Destiny };
    public ShowScreen showScreen = ShowScreen.None;
    private ShowScreen savedSelScreen;

    private float startTime = 0f;
    private float popupDuration = 3f;

    public bool applyGravity = true;
    public bool kbcont;
    public static bool keyboardControls;

    private static GUIStyle style;
    private string guiDisplay = "";
    private string systemType;

    private static GameObject CC_CANOEOBJ;
    private static GameObject CC_INNOVATOR_SCREENS;
    private static GameObject CC_DESTINY_SCREENS;
    private static GameObject CC_GUI;
    private static GameObject[] CC_WAND;
    private static GameObject CC_HEAD;
    private static CharacterController charController;
    private static System.Random rand = new System.Random(949493);

    private CCaux_OmniNavigator navigator;

    //GLOBAL GET METHODS
    /// <summary>
    /// The transform of the specified wand.
    /// </summary>
    /// <param name="wandNum">Wand number.  Left = 0  Right = 1</param>
    /// <returns>The transform of this wand.</returns>
    public static Transform WandTransform(Wand wand) { return CC_WAND[(int)wand].transform; }

    /// <summary>
    /// The gameobject of the specified wand.
    /// </summary>
    /// <param name="wandNum">Wand number.  Left = 0  Right = 1</param>
    /// <returns>The gameobject of this wand.</returns>
    public static GameObject WandGameObject(Wand wand) { return CC_WAND[(int)wand]; }

    /// <summary>
    /// The collider of the specified wand. (SphereCollider)
    /// </summary>
    /// <param name="wandNum">Wand number.  Left = 0  Right = 1</param>
    /// <returns>The collider of this wand. (SphereCollider)</returns>
    public static SphereCollider WandCollider(Wand wand) { return CC_WAND[(int)wand].GetComponent<SphereCollider>(); }

    /// <summary>
    /// The game object of the head.
    /// </summary>
    /// <returns>The game object of the head.</returns>
    public static GameObject HeadGameObject() { return CC_HEAD; }

    /// <summary>
    /// The game object of the Canoe.
    /// </summary>
    /// <returns>The game object of the Canoe.</returns>
    public static GameObject CanoeGameObject() { return CC_CANOEOBJ; }

    /// <summary>
    /// The character controller attached to the Canoe.
    /// </summary>
    /// <returns>The character controller attached to the Canoe.</returns>
    public static CharacterController CanoeCharacterController() { return charController; }

    /// <summary>
    /// Returns true if the input system is Vive.
    /// </summary>
    public bool isVive() {
        return vive;
    }

    /// <summary>
    /// Returns true if the input system is OptiTrack.
    /// </summary>
    public bool isOptiTrack() {
        return optitrack;
    }

    /// <summary>
    /// Returns name of input system.
    /// </summary>
    public string GetSystemType() {
        return systemType;
    }

    void Awake() {
        //Load Settings from the XML File
        if (!Application.isEditor) {
            CC_CONFIG.LoadXMLConfig();
        }

        CC_CONFIG.LoadXMLConfig();

        //Get the GameObjects attached to the canoe.
        CC_WAND = new GameObject[2];
        CC_WAND[0] = transform.Find("CC_WAND_LEFT").gameObject;
        CC_WAND[1] = transform.Find("CC_WAND_RIGHT").gameObject;
        CC_HEAD = transform.Find("CC_HEAD").gameObject;
        charController = GetComponent<CharacterController>();
        CC_CANOEOBJ = gameObject;
        CC_INNOVATOR_SCREENS = transform.Find("CC_INNOVATOR_SCREENS").gameObject;
        CC_DESTINY_SCREENS = transform.Find("CC_DESTINY_SCREENS").gameObject;
        if (isVive()) {
            CC_GUI = transform.Find("CC_GUI_VIVE").gameObject;
            systemType = "vive";
        } else if (isOptiTrack()) {
            CC_GUI = transform.Find("CC_GUI_OPTITRACK").gameObject;
            systemType = "optitrack";
        }
        keyboardControls = kbcont;
        navigator = gameObject.GetComponent<CCaux_OmniNavigator>();

        // Only checks and sets vrpn
        if (ClusterNetwork.isMasterOfCluster) {
            StartCoroutine(oneTimeServerVrpnSet());
        }
        return;
    }

    /// <summary>
    /// Initial call to VRPN to allow data retrieval later
    /// in the program.
    /// </summary>
    private static System.Collections.IEnumerator oneTimeServerVrpnSet() {
        yield return new WaitForEndOfFrame();
        //Trackpad button entries
        VRPN.vrpnButton("CC_FLAT_WAND0@" + CC_CONFIG.serverIP, 32);
        VRPN.vrpnButton("CC_FLAT_WAND1@" + CC_CONFIG.serverIP, 32);
        //Trackpad axis entries
        VRPN.vrpnAnalog("CC_FLAT_WAND0@" + CC_CONFIG.serverIP, 0);
        VRPN.vrpnAnalog("CC_FLAT_WAND0@" + CC_CONFIG.serverIP, 1);
        VRPN.vrpnAnalog("CC_FLAT_WAND1@" + CC_CONFIG.serverIP, 0);
        VRPN.vrpnAnalog("CC_FLAT_WAND1@" + CC_CONFIG.serverIP, 1);
        //Trigger button entries
        VRPN.vrpnButton("CC_FLAT_WAND0@" + CC_CONFIG.serverIP, 33);
        VRPN.vrpnButton("CC_FLAT_WAND1@" + CC_CONFIG.serverIP, 33);
        //Trigger axis entry
        VRPN.vrpnAnalog("CC_FLAT_WAND0@" + CC_CONFIG.serverIP, 2);
        VRPN.vrpnAnalog("CC_FLAT_WAND1@" + CC_CONFIG.serverIP, 2);
    }

    void Start() {
        //Set the scale of Destiny's screens to account or not account for bezel.
        if (CC_CONFIG.IsDestiny()) {
            foreach (Transform child in CC_DESTINY_SCREENS.transform) {
                child.localScale = new Vector3(0.6797f, 1.208f, 1.0f);
            }
        } else {
            foreach (Transform child in CC_DESTINY_SCREENS.transform) {
                child.localScale = new Vector3(0.701675f, 1.2255f, 1.0f);
            }
        }

        //Set the visibility of the wand models and screens
        CC_WAND[0].transform.Find("CC_AXIS_MODEL").gameObject.SetActive(false);
        CC_WAND[1].transform.Find("CC_AXIS_MODEL").gameObject.SetActive(false);

        CC_INNOVATOR_SCREENS.SetActive(false);
        CC_DESTINY_SCREENS.SetActive(false);

        //GUI Setup
        style = new GUIStyle();
        if (CC_CONFIG.IsDestiny() || CC_CONFIG.IsInnovator()) {
            style.fontSize = 100;
        } else {
            style.fontSize = 25;
        }
        style.normal.textColor = Color.white;
    }

    void Update() {

        if (!navigator.doNav && Input.GetKeyDown(KeyCode.Alpha6) && keyboardControls) {
            UpdateWandModels();
        }

        //Wand select
        if (Input.GetKeyDown(KeyCode.Alpha1) && keyboardControls) {
            simulatorActiveWand = Wand.Left;
            startTime = Time.time;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && keyboardControls) {
            simulatorActiveWand = Wand.Right;
            startTime = Time.time;
        }
        simActiveWand = simulatorActiveWand;

        //Gravity
        if (applyGravity) {
            //SimpleMove applies gravity automatically
            charController.SimpleMove(Vector3.zero);
        }

        // Show and hide the CyberCANOE's screen.
        if (Input.GetKeyDown(KeyCode.Alpha6) && keyboardControls) {
            changeScreens();
            startTime = Time.time;
        }

        //Change wand models
        if (Input.GetKeyDown(KeyCode.Alpha5) && keyboardControls && !navigator.doNav) {
            wandModel++;
            wandModel = (WandModel)((int)wandModel % 3);
            UpdateWandModels();
            startTime = Time.time;
        }

        //Show and hide Simulator Mode help screen.
        if (Input.GetKeyDown(KeyCode.Slash) && keyboardControls) {
            CC_GUI.SetActive(!CC_GUI.activeInHierarchy);
        }

        // Press the escape key to quit application
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        keyboardControls = kbcont;

        //Call ONLY works in Update, tested in other locations and failed
        if (ClusterNetwork.isMasterOfCluster) {
            StartCoroutine(endOfFrameForMaster());
        }
    }

    /// <summary>
    /// Obtains the correct trackpad and trigger button values directly from VRPN and sets
    /// ClusterInput entries to correct value. Implemented because initial values
    /// from ClusterInput are incorrect.
    /// </summary>
    private static System.Collections.IEnumerator endOfFrameForMaster() {
        yield return new WaitForEndOfFrame();
        //Trackpad button updates
        ClusterInput.SetButton("leftTrackpad", VRPN.vrpnButton("CC_FLAT_WAND0@" + CC_CONFIG.serverIP, 32));
        ClusterInput.SetButton("rightTrackpad", VRPN.vrpnButton("CC_FLAT_WAND1@" + CC_CONFIG.serverIP, 32));
        //Trackpad axis updates
        ClusterInput.SetAxis("leftXAxis", (float)VRPN.vrpnAnalog("CC_FLAT_WAND0@" + CC_CONFIG.serverIP, 0));
        ClusterInput.SetAxis("leftYAxis", (float)VRPN.vrpnAnalog("CC_FLAT_WAND0@" + CC_CONFIG.serverIP, 1));
        ClusterInput.SetAxis("rightXAxis", (float)VRPN.vrpnAnalog("CC_FLAT_WAND1@" + CC_CONFIG.serverIP, 0));
        ClusterInput.SetAxis("rightYAxis", (float)VRPN.vrpnAnalog("CC_FLAT_WAND1@" + CC_CONFIG.serverIP, 1));
        //Trigger button updates
        ClusterInput.SetButton("leftTrigger", VRPN.vrpnButton("CC_FLAT_WAND0@" + CC_CONFIG.serverIP, 33));
        ClusterInput.SetButton("rightTrigger", VRPN.vrpnButton("CC_FLAT_WAND1@" + CC_CONFIG.serverIP, 33));
        //Trigger axis updates
        ClusterInput.SetAxis("leftTriggerAxis", (float)VRPN.vrpnAnalog("CC_FLAT_WAND0@" + CC_CONFIG.serverIP, 2));
        ClusterInput.SetAxis("rightTriggerAxis", (float)VRPN.vrpnAnalog("CC_FLAT_WAND1@" + CC_CONFIG.serverIP, 2));
    }

    void LateUpdate() {
        //Simulator forward and backward movement
        float curSpeed = 0.0f;
        if (Input.GetKey(KeyCode.W) && keyboardControls) curSpeed += navigationSpeed;
        if (Input.GetKey(KeyCode.S) && keyboardControls) curSpeed -= navigationSpeed;
        Vector3 forward = Vector3.zero;
        forward = CC_WAND[(int)Wand.Left].transform.TransformDirection(Vector3.forward);
        charController.Move(forward * curSpeed * Time.deltaTime);

        //Simulator Y-Axis rotation
        float yaw = 0.0f;
        if (Input.GetKey(KeyCode.D) && keyboardControls) { yaw += navigationRotationSpeed; }
        if (Input.GetKey(KeyCode.A) && keyboardControls) { yaw -= navigationRotationSpeed; }
        if (GetComponent<CCaux_Navigator>()) {
            GetComponent<CCaux_Navigator>().yAxisChange += yaw;
        }
        transform.Rotate(new Vector3(0, yaw, 0));
    }

    /// <summary>
    /// Deactivates wand models when navigating.
    /// </summary>
    public void DeactivateModels() {
        CC_WAND[0].transform.Find("CC_LEFTHAND_MODEL").gameObject.SetActive(false);
        CC_WAND[1].transform.Find("CC_RIGHTHAND_MODEL").gameObject.SetActive(false);
        CC_WAND[0].transform.Find("CC_AXIS_MODEL").gameObject.SetActive(false);
        CC_WAND[1].transform.Find("CC_AXIS_MODEL").gameObject.SetActive(false);
    }

    /// <summary>
    /// Update the wand models.
    /// </summary>
    public void UpdateWandModels() {

        switch (wandModel) {
            case WandModel.None:
                CC_WAND[0].transform.Find("CC_LEFTHAND_MODEL").gameObject.SetActive(false);
                CC_WAND[1].transform.Find("CC_RIGHTHAND_MODEL").gameObject.SetActive(false);
                CC_WAND[0].transform.Find("CC_AXIS_MODEL").gameObject.SetActive(false);
                CC_WAND[1].transform.Find("CC_AXIS_MODEL").gameObject.SetActive(false);
                guiDisplay = "wandModelNone";
                break;
            case WandModel.Hand:
                CC_WAND[0].transform.Find("CC_LEFTHAND_MODEL").gameObject.SetActive(true);
                CC_WAND[1].transform.Find("CC_RIGHTHAND_MODEL").gameObject.SetActive(true);
                CC_WAND[0].transform.Find("CC_AXIS_MODEL").gameObject.SetActive(false);
                CC_WAND[1].transform.Find("CC_AXIS_MODEL").gameObject.SetActive(false);
                guiDisplay = "wandModelHand";
                break;
            case WandModel.Axis:
                CC_WAND[0].transform.Find("CC_LEFTHAND_MODEL").gameObject.SetActive(false);
                CC_WAND[1].transform.Find("CC_RIGHTHAND_MODEL").gameObject.SetActive(false);
                CC_WAND[0].transform.Find("CC_AXIS_MODEL").gameObject.SetActive(true);
                CC_WAND[1].transform.Find("CC_AXIS_MODEL").gameObject.SetActive(true);
                guiDisplay = "wandModelAxis";
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Change the visibility of the screens.
    /// </summary>
    private void changeScreens() {

        showScreen++;
        showScreen = (ShowScreen)((int)showScreen % 3);

        switch (showScreen) {
            case ShowScreen.None:
                CC_INNOVATOR_SCREENS.SetActive(false);
                CC_DESTINY_SCREENS.SetActive(false);
                guiDisplay = "showScreenNone";
                break;
            case ShowScreen.Innovator:
                CC_INNOVATOR_SCREENS.SetActive(true);
                CC_DESTINY_SCREENS.SetActive(false);
                guiDisplay = "showScreenInnovator";
                break;
            case ShowScreen.Destiny:
                CC_INNOVATOR_SCREENS.SetActive(false);
                CC_DESTINY_SCREENS.SetActive(true);
                guiDisplay = "showScreenDestiny";
                break;
        }
    }

    /// <summary>
    /// Displays the information to the screen.
    /// </summary>
    void OnGUI() {

        if ((Time.time - startTime) < popupDuration) {
            Rect textRect = new Rect();
            string canoePrefix = "CC_CANOE - ";
            string orientation = "";

            if (CC_CONFIG.IsInnovator() || CC_CONFIG.IsDestiny()) {
                textRect = new Rect(10f, Screen.height - 115, 200, 100);
            } else {
                textRect = new Rect(10f, Screen.height - 30, 200, 100);
            }

            if (simulatorActiveWand == Wand.Left) {
                orientation = "Left";
            } else if (simulatorActiveWand == Wand.Right) {
                orientation = "Right";
            }

            if (CC_CONFIG.IsInnovator() || CC_CONFIG.IsDestiny()) {
                if (guiDisplay.Contains("wandModel")) {
                    GUI.Label(textRect, canoePrefix + "Wand Model: " + guiDisplay.Substring(9), style);
                }
            } else {
                if (guiDisplay.Contains("wandModel")) {
                    GUI.Label(textRect, canoePrefix + orientation + " Simulator Wand, Model: " + guiDisplay.Substring(9), style);
                } else if (guiDisplay.Contains("showScreen")) {
                    GUI.Label(textRect, canoePrefix + "CANOE Model: " + guiDisplay.Substring(10), style);
                }
            }
        }
    }

    /// <summary>
    /// Temporary fix for Unity's random not being synced across all nodes
    /// </summary>
    /// <returns>Random int between the range of min and max. Inclusive.</returns>
    public static int RandomRange(int min, int max) {
        if (min > max) {
            throw new System.ArgumentException("The second argument cannot be less than the first.");
        }
        return (int)System.Math.Floor(rand.NextDouble() * (max - min + 1) + min);
    }

    /// <summary>
    /// Temporary fix for Unity's random not being synced across all nodes
    /// </summary>
    /// <returns>Random float between the range of min and max. Exclusive.</returns>
    public static float RandomRange(float min, float max) {
        if (min > max) {
            throw new System.ArgumentException("The second argument cannot be less than the first.");
        }
        return max - ((System.Math.Abs(max) + System.Math.Abs(min)) * (float)rand.NextDouble());
    }
}
