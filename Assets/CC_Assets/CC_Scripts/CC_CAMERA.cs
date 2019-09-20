using UnityEngine;

/* 
Manages the cameras of the CyberCANOE.
 
CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: 1.14, August 6th, 2019.
*/

/// <summary> Manages all the cameras for Destiny, Innovator and the Simulator. </summary>
public class CC_CAMERA : MonoBehaviour {
    [HideInInspector]
    public enum SelectedCamera { Simulator, Innovator, Destiny };
    public SelectedCamera selectCamera;
    private SelectedCamera savedSelCam;

    [SerializeField]
    private CC_CAMERARIG m_DestinyCameraRig;
    [HideInInspector]
    public int destinyCameraIndex = 0;

    [HideInInspector]
    public bool enableStereo;
    private bool savedStereo;

    [HideInInspector]
    public int interaxial = 55;
    private float savedInteraxial;

    public Material destinyStereoMaterial;
    public Material innovatorStereoMaterial;

    private Camera[] destinyCameras;
    private Camera innovatorCamera;
    private Camera simCam;
    private GameObject innovatorCameraGroup;
    private GameObject destinyCameraGroup;
    private GameObject simulatorCameraGroup;

    private float savedAspectRatio;
    private bool panOptic;
    private float guiTimeChange;
    private string guiDisplay;
    private GUIStyle style;

    private bool keyboardControls;

    void Start() {
        //Load Settings
        if (CC_CONFIG.ConfigLoaded()) {
            interaxial = (int)CC_CONFIG.interaxial;
            panOptic = CC_CONFIG.panOptic;
            enableStereo = CC_CONFIG.stereo;
        }

        //Get camera groups
        innovatorCameraGroup = transform.Find("CC_INNOVATOR_CAMERAS").gameObject;
        destinyCameraGroup = transform.Find("CC_DESTINY_CAMERAS").gameObject;
        simulatorCameraGroup = transform.Find("CC_SIM_CAMERA").gameObject;

        //Simulator Camera Setup
        simCam = simulatorCameraGroup.GetComponent<Camera>();
        simCam.rect = GetComponent<Camera>().rect;
        simCam.nearClipPlane = GetComponent<Camera>().nearClipPlane;
        simCam.farClipPlane = GetComponent<Camera>().farClipPlane;
        simCam.clearFlags = GetComponent<Camera>().clearFlags;
        simCam.backgroundColor = GetComponent<Camera>().backgroundColor;
        simCam.cullingMask = GetComponent<Camera>().cullingMask;

        //Innovator Camera Setup
        innovatorCamera = innovatorCameraGroup.transform.GetChild(0).gameObject.GetComponent<Camera>();
        innovatorCamera.GetComponent<CC_CAMERASTEREO>().CreateStereoCameras(false);

        //Destiny Camera Setup
        destinyCameras = new Camera[4];
        destinyCameras[0] = destinyCameraGroup.transform.GetChild(0).GetComponent<Camera>();
        destinyCameras[1] = destinyCameraGroup.transform.GetChild(1).GetComponent<Camera>();
        destinyCameras[2] = destinyCameraGroup.transform.GetChild(2).GetComponent<Camera>();
        destinyCameras[3] = destinyCameraGroup.transform.GetChild(3).GetComponent<Camera>();
        for (int i = 0; i < 4; i++) {
            destinyCameras[i].GetComponent<CC_CAMERASTEREO>().CreateStereoCameras(true);
        }

        //Initial Update Cameras
        updateCamerasInteraxials();
        updateCamerasStereo();
        updateCamerasAspectRatio();

        //Save current settings
        savedAspectRatio = GetComponent<Camera>().aspect;
        savedStereo = enableStereo;
        savedInteraxial = interaxial;

        //GUI Setup
        style = new GUIStyle();
        if (CC_CONFIG.IsDestiny() || CC_CONFIG.IsInnovator()) {
            style.fontSize = 100;
        } else {
            style.fontSize = 25;
        }
        style.normal.textColor = Color.white;

        //Set startup camera according to platform
        if (!Application.isEditor) {
            if (CC_CONFIG.IsInnovator()) {
                selectCamera = SelectedCamera.Innovator;
            } else if (CC_CONFIG.IsDestiny()) {
                selectCamera = SelectedCamera.Destiny;
            } else {
                selectCamera = SelectedCamera.Simulator;
            }
        }
        changeCameras();
    }

    void Update() {
        //Change cameras
        if (Input.GetKeyDown(KeyCode.Alpha0) && CC_CANOE.keyboardControls) {
            selectCamera++;
            if ((int)selectCamera >= 3) {
                selectCamera = 0;
            }
        }
        if (savedSelCam != selectCamera) {
            changeCameras();
        }

        //Change interaxial
        if (Input.GetKeyDown(KeyCode.Equals) && CC_CANOE.keyboardControls) {
            interaxial++;
        }
        if (Input.GetKeyDown(KeyCode.Minus) && CC_CANOE.keyboardControls) {
            interaxial--;
        }
        if (savedInteraxial != interaxial) {
            updateCamerasInteraxials();
        }

        //Enable/disable stereoscopic.
        if (Input.GetKeyDown(KeyCode.Alpha9) && CC_CANOE.keyboardControls) {
            enableStereo = !enableStereo;
        }
        if (savedStereo != enableStereo) {
            updateCamerasStereo();
        }

        //Enable/disable Panoptic for Destiny.
        if (Input.GetKeyDown(KeyCode.Alpha8) && CC_CANOE.keyboardControls) {
            panOptic = !panOptic;
            guiTimeChange = Time.time;
            guiDisplay = "panopticGUI";
        }

        //Checkes for changes to the aspect ratio for innovator and upadtes camera's accordingly.
        if (savedAspectRatio != GetComponent<Camera>().aspect) {
            updateCamerasAspectRatio();
        }

        //Change the Destiny Camera Index
        if (Input.GetKeyDown(KeyCode.LeftBracket) && CC_CANOE.keyboardControls) {
            if (destinyCameraIndex < 7) {
                destinyCameraIndex++;
                guiTimeChange = Time.time;
                guiDisplay = "cameraIndex";
            }
        }
        if (Input.GetKeyDown(KeyCode.RightBracket) && CC_CANOE.keyboardControls) {
            if (destinyCameraIndex > 0) {
                destinyCameraIndex--;
                guiTimeChange = Time.time;
                guiDisplay = "cameraIndex";
            }
        }
    }

    void LateUpdate() {
        setDestinyPerspective();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (selectCamera == SelectedCamera.Destiny) {

            if (enableStereo) {
                destinyStereoMaterial.SetTexture("leftTopLeft", destinyCameras[0].GetComponent<CC_CAMERASTEREO>().GetLeftRenderTexture());
                destinyStereoMaterial.SetTexture("leftBottomLeft", destinyCameras[1].GetComponent<CC_CAMERASTEREO>().GetLeftRenderTexture());
                destinyStereoMaterial.SetTexture("leftTopRight", destinyCameras[2].GetComponent<CC_CAMERASTEREO>().GetLeftRenderTexture());
                destinyStereoMaterial.SetTexture("leftBottomRight", destinyCameras[3].GetComponent<CC_CAMERASTEREO>().GetLeftRenderTexture());

                destinyStereoMaterial.SetTexture("rightTopLeft", destinyCameras[0].GetComponent<CC_CAMERASTEREO>().GetRightRenderTexture());
                destinyStereoMaterial.SetTexture("rightBottomLeft", destinyCameras[1].GetComponent<CC_CAMERASTEREO>().GetRightRenderTexture());
                destinyStereoMaterial.SetTexture("rightTopRight", destinyCameras[2].GetComponent<CC_CAMERASTEREO>().GetRightRenderTexture());
                destinyStereoMaterial.SetTexture("rightBottomRight", destinyCameras[3].GetComponent<CC_CAMERASTEREO>().GetRightRenderTexture());

                destinyStereoMaterial.SetFloat("resX", Screen.width);
                destinyStereoMaterial.SetFloat("resY", Screen.height);

                Graphics.Blit(destination, destinyStereoMaterial, 0);

            } else {
                destinyStereoMaterial.SetTexture("centerTopLeft", destinyCameras[0].GetComponent<CC_CAMERASTEREO>().GetCenterRenderTexture());
                destinyStereoMaterial.SetTexture("centerBottomLeft", destinyCameras[1].GetComponent<CC_CAMERASTEREO>().GetCenterRenderTexture());
                destinyStereoMaterial.SetTexture("centerTopRight", destinyCameras[2].GetComponent<CC_CAMERASTEREO>().GetCenterRenderTexture());
                destinyStereoMaterial.SetTexture("centerBottomRight", destinyCameras[3].GetComponent<CC_CAMERASTEREO>().GetCenterRenderTexture());

                destinyStereoMaterial.SetFloat("resX", Screen.width);
                destinyStereoMaterial.SetFloat("resY", Screen.height);

                Graphics.Blit(destination, destinyStereoMaterial, 1);
            }
        } else if (selectCamera == SelectedCamera.Innovator) {
            if (enableStereo) {
                innovatorStereoMaterial.SetFloat("InterlaceValue", Screen.height);

                innovatorStereoMaterial.SetTexture("LeftTex", innovatorCamera.GetComponent<CC_CAMERASTEREO>().GetLeftRenderTexture());
                innovatorStereoMaterial.SetTexture("RightTex", innovatorCamera.GetComponent<CC_CAMERASTEREO>().GetRightRenderTexture());

                Graphics.Blit(destination, innovatorStereoMaterial, 0);

            } else {
                Graphics.Blit(innovatorCamera.GetComponent<CC_CAMERASTEREO>().GetCenterRenderTexture(), destination);
            }
        } else {
            Graphics.Blit(source, destination);
        }
    }

    /// <summary>
    /// Change which camera is in use.
    /// </summary>
    private void changeCameras() {
        switch (selectCamera) {
            case SelectedCamera.Simulator:
                simulatorCameraGroup.SetActive(true);
                innovatorCameraGroup.SetActive(false);
                destinyCameraGroup.SetActive(false);
                savedSelCam = selectCamera;
                break;
            case SelectedCamera.Innovator:
                simulatorCameraGroup.SetActive(false);
                innovatorCameraGroup.SetActive(true);
                destinyCameraGroup.SetActive(false);
                savedSelCam = selectCamera;
                break;
            case SelectedCamera.Destiny:
                simulatorCameraGroup.SetActive(false);
                innovatorCameraGroup.SetActive(false);
                destinyCameraGroup.SetActive(true);
                savedSelCam = selectCamera;
                break;
        }
        guiTimeChange = Time.time;
        guiDisplay = "cameraGUI";
    }

    /// <summary>
    /// Set the perspective of the Destiny camera rig.
    /// </summary>
    private void setDestinyPerspective() {
        if (!CC_CONFIG.IsDestiny()) {
            m_DestinyCameraRig.UpdateCameraPerspective(destinyCameras, destinyCameraIndex, panOptic);
        } else {
            int cameraIndex = GetCameraIndex();

            if (cameraIndex == -1) return;

            m_DestinyCameraRig.UpdateCameraPerspective(destinyCameras, cameraIndex, panOptic);
        }
    }

    /// <summary>
    /// Update each camera's aspect ratio.
    /// </summary>
    private void updateCamerasAspectRatio() {
        innovatorCamera.GetComponent<CC_CAMERASTEREO>().UpdateScreenAspect(false);
        m_DestinyCameraRig.UpdateCameraScreenAspect(destinyCameras);
        savedAspectRatio = GetComponent<Camera>().aspect;
    }

    /// <summary>
    /// Disables or enables center, left, or right cameras depending on if stereo is enabled.
    /// </summary>
    private void updateCamerasStereo() {
        if (enableStereo) {
            innovatorCamera.GetComponent<CC_CAMERASTEREO>().DisableCenterCamera();
            m_DestinyCameraRig.UpdateCameraStereo(destinyCameras, enableStereo);
        } else {
            innovatorCamera.GetComponent<CC_CAMERASTEREO>().EnableCenterCamera();
            m_DestinyCameraRig.UpdateCameraStereo(destinyCameras, enableStereo);
        }

        savedStereo = enableStereo;
        guiTimeChange = Time.time;
        guiDisplay = "stereoGUI";
    }

    /// <summary>
    /// Updates each camera's interaxial setting.
    /// </summary>
    private void updateCamerasInteraxials() {
        GameObject head = gameObject;
        innovatorCamera.GetComponent<CC_CAMERASTEREO>().UpdateInteraxial(head, (float)interaxial / 1000);
        m_DestinyCameraRig.UpdateCameraInteraxials(destinyCameras, (float)interaxial / 1000);

        savedInteraxial = interaxial;
        guiTimeChange = Time.time;
        guiDisplay = "interaxialGUI";
    }

    /// <summary>
    /// Displays the information to the screen.
    /// </summary>
    void OnGUI() {

        string value = (interaxial).ToString();

        string camPrefix = "CC_CAMERA - ";

        if (Time.time - guiTimeChange < 3) {
            Rect textRect = new Rect();
            if (CC_CONFIG.IsInnovator() || CC_CONFIG.IsDestiny()) {
                textRect = new Rect(15, 10/*Screen.height - 115*/, 200, 100);
            } else {
                textRect = new Rect(5, 10/*Screen.height - 30*/, 200, 100);
            }

            if (guiDisplay.Equals("interaxialGUI")) {
                GUI.Label(textRect, camPrefix + "Interaxial: " + value + "mm", style);
            } else if (guiDisplay.Equals("stereoGUI")) {
                GUI.Label(textRect, camPrefix + "Stereo: " + savedStereo, style);
            } else if (guiDisplay.Equals("panopticGUI")) {
                GUI.Label(textRect, camPrefix + "Panoptic: " + panOptic, style);
            } else if (guiDisplay.Equals("cameraGUI")) {
                GUI.Label(textRect, camPrefix + "Camera Type: " + selectCamera.ToString(), style);
            } else if (guiDisplay.Equals("cameraIndex")) {
                GUI.Label(textRect, camPrefix + "Camera Index: MAUI " + (destinyCameraIndex + 1), style);
            }
        }
    }

    /// <summary>
    /// Returns the camera index of this client.
    /// </summary>
    public static int GetCameraIndex() {
        // load from command line
        string cmdIndex = getCmdArguments("-client");
        // if there is an index given from the command line (slave node)
        if (cmdIndex != null) {
            return int.Parse(cmdIndex);
        }
        // if node is master node and option has been chosen to integrate it
        else return 0;
    }

    /// <summary>
    /// Receives and returns command line arguments.
    /// </summary>
    /// <param name="arg">Command line arguments.</param>
    private static string getCmdArguments(string arg) {
        string[] arguments = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < arguments.Length; i++) {
            if (arguments[i] == arg) {
                if (i + 1 < arguments.Length) {
                    return arguments[i + 1];
                }
            }
        }
        // default to null
        return null;
    }
}
