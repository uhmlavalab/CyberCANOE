using UnityEngine;

/* 
The main class the user interfaces with to set settings of the CC_CANOE.
Users can also access information about the head and wands by using static methods within this class.

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: 1.3, May 12th, 2017.
 */

/// <summary> The main class to interface with to retrieve Wand, Head, and CharacterController information. </summary>
public class CC_CANOE : MonoBehaviour
{
    public float navigationSpeed = 5.0f;
    public float navigationRotationSpeed = 1.25f;

    public WandModel wandModel;
    public enum WandModel { None, Hand, Axis };
    private WandModel savedWandModel;

    public Wand simulatorActiveWand;
    public static Wand simActiveWand;

    public ShowScreen showScreen;
    public enum ShowScreen { None, Innovator, Destiny };
    private ShowScreen savedSelScreen;

    public bool applyGravity = true;
    public bool kbcont;
    public static bool keyboardControls;

    private static GameObject CC_CANOEOBJ;
    private static GameObject CC_INNOVATOR_SCREENS;
    private static GameObject CC_DESTINY_SCREENS;
    private static GameObject CC_GUI;
    private static GameObject[] CC_WAND;
    private static GameObject CC_HEAD;
    private static CharacterController charController;

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

    void Awake()
    {

        //Load Settings from the XML File
        if (!Application.isEditor)
        {
            CC_CONFIG.loadXMLConfig();
        }

        //Get the GameObjects attached to the canoe.
        CC_WAND = new GameObject[2];
        CC_WAND[0] = transform.FindChild("CC_WAND_LEFT").gameObject;
        CC_WAND[1] = transform.FindChild("CC_WAND_RIGHT").gameObject;
        CC_HEAD = transform.FindChild("CC_HEAD").gameObject;
        charController = GetComponent<CharacterController>();
        CC_CANOEOBJ = gameObject;
        CC_INNOVATOR_SCREENS = transform.FindChild("CC_INNOVATOR_SCREENS").gameObject;
        CC_DESTINY_SCREENS = transform.FindChild("CC_DESTINY_SCREENS").gameObject;
        CC_GUI = transform.FindChild("CC_GUI").gameObject;
        keyboardControls = kbcont;

    }

    void Start()
    {

        //Set the scale of Destiny's screens to account or not account for bezel. 
        if (CC_CONFIG.isDestiny())
        {
            foreach (Transform child in CC_DESTINY_SCREENS.transform)
            {
                child.localScale = new Vector3(0.6797f, 1.208f, 1.0f);
            }
        }
        else
        {
            foreach (Transform child in CC_DESTINY_SCREENS.transform)
            {
                child.localScale = new Vector3(0.701675f, 1.2255f, 1.0f);
            }
        }

        //Set the visibility of the wand models and screens
        changeWandModels();
        changeScreens();

    }

    void Update()
    {
        //Wand select
        if (Input.GetKeyDown(KeyCode.Alpha1) && keyboardControls) simulatorActiveWand = Wand.Left;
        if (Input.GetKeyDown(KeyCode.Alpha2) && keyboardControls) simulatorActiveWand = Wand.Right;
        simActiveWand = simulatorActiveWand;

        //Gravity
        if (applyGravity)
        {
            //SimpleMove applies gravity automatically
            charController.SimpleMove(Vector3.zero);
        }

        // Show and hide the CyberCANOE's screen.
        if (Input.GetKeyDown(KeyCode.Alpha6) && keyboardControls)
        {
            showScreen++;
            if ((int)showScreen == 3) showScreen = 0;
        }
        if (savedSelScreen != showScreen) changeScreens();

        //Change wand models
        if (Input.GetKeyDown(KeyCode.Alpha5) && keyboardControls)
        {
            wandModel++;
            if ((int)wandModel == 3) wandModel = 0;
            changeWandModels();
        }
        if (wandModel != savedWandModel) changeWandModels();

        //Show and hide Simulator Mode help screen.
        if (Input.GetKeyDown(KeyCode.Slash) && keyboardControls)
        {
            CC_GUI.SetActive(!CC_GUI.activeInHierarchy);
        }

        // Press the escape key to quit application
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        keyboardControls = kbcont;
    }

    void LateUpdate()
    {
        //Simulator forward and backward movement
        float curSpeed = 0.0f;
        if (Input.GetKey(KeyCode.W) && keyboardControls) curSpeed += navigationSpeed;
        if (Input.GetKey(KeyCode.S) && keyboardControls) curSpeed -= navigationSpeed;
        Vector3 forward = CC_WAND[(int)Wand.Left].transform.TransformDirection(Vector3.forward);
        charController.Move(forward * curSpeed * Time.deltaTime);

        //Simulator Y-Axis rotation
        float yaw = 0.0f;
        if (Input.GetKey(KeyCode.D) && keyboardControls) yaw += navigationRotationSpeed;
        if (Input.GetKey(KeyCode.A) && keyboardControls) yaw -= navigationRotationSpeed;
        if (GetComponent<CCaux_Navigator>())
            GetComponent<CCaux_Navigator>().yAxisChange += yaw;
        transform.Rotate(new Vector3(0, yaw, 0));

    }


    //Change the wand models
    private void changeWandModels()
    {
        switch (wandModel)
        {
            case WandModel.None:
                CC_WAND[0].transform.FindChild("CC_LEFTHAND_MODEL").gameObject.SetActive(false);
                CC_WAND[1].transform.FindChild("CC_RIGHTHAND_MODEL").gameObject.SetActive(false);
                CC_WAND[0].transform.FindChild("CC_AXIS_MODEL").gameObject.SetActive(false);
                CC_WAND[1].transform.FindChild("CC_AXIS_MODEL").gameObject.SetActive(false);
                savedWandModel = wandModel;
                break;

            case WandModel.Hand:
                CC_WAND[0].transform.FindChild("CC_LEFTHAND_MODEL").gameObject.SetActive(true);
                CC_WAND[1].transform.FindChild("CC_RIGHTHAND_MODEL").gameObject.SetActive(true);
                CC_WAND[0].transform.FindChild("CC_AXIS_MODEL").gameObject.SetActive(false);
                CC_WAND[1].transform.FindChild("CC_AXIS_MODEL").gameObject.SetActive(false);
                savedWandModel = wandModel;
                break;
            case WandModel.Axis:
                CC_WAND[0].transform.FindChild("CC_LEFTHAND_MODEL").gameObject.SetActive(false);
                CC_WAND[1].transform.FindChild("CC_RIGHTHAND_MODEL").gameObject.SetActive(false);
                CC_WAND[0].transform.FindChild("CC_AXIS_MODEL").gameObject.SetActive(true);
                CC_WAND[1].transform.FindChild("CC_AXIS_MODEL").gameObject.SetActive(true);
                savedWandModel = wandModel;
                break;
        }
    }

    //Change the visiblity of the screens.
    private void changeScreens()
    {
        switch (showScreen)
        {
            case ShowScreen.None:
                CC_INNOVATOR_SCREENS.SetActive(false);
                CC_DESTINY_SCREENS.SetActive(false);
                savedSelScreen = showScreen;
                break;
            case ShowScreen.Innovator:
                CC_INNOVATOR_SCREENS.SetActive(true);
                CC_DESTINY_SCREENS.SetActive(false);
                savedSelScreen = showScreen;
                break;
            case ShowScreen.Destiny:
                CC_INNOVATOR_SCREENS.SetActive(false);
                CC_DESTINY_SCREENS.SetActive(true);
                savedSelScreen = showScreen;
                break;
        }
    }

    /// <summary>
    /// Temporary fix for Unity's random not being synced across all nodes
    /// </summary>
    /// <returns>Random int between the range of min and max. Inclusive.</returns>
    public static int RandomRange(int min, int max)
    {
        if (min > max)
            throw new System.ArgumentException("The second argument cannot be less than the first.");

        System.Random rand = new System.Random((int)Time.time);
        return (int)System.Math.Floor(rand.NextDouble() * (max - min + 1) + min);

    }

    /// <summary>
    /// Temporary fix for Unity's random not being synced across all nodes
    /// </summary>
    /// <returns>Random float between the range of min and max. Exclusive.</returns>
    public static float RandomRange(float min, float max)
    {
        if (min > max)
            throw new System.ArgumentException("The second argument cannot be less than the first.");

        System.Random rand = new System.Random((int)Time.time);
        return max - ((System.Math.Abs(max) + System.Math.Abs(min)) * (float)rand.NextDouble());
    }


}
