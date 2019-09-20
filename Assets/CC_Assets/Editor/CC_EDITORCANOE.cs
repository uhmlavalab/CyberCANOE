using UnityEngine;
using UnityEditor;

/* 
The CC_CANOE custom inspector/editor script.
Unity Editor script that performs miscellaneous tasks before buidling project.

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: 1.14, August 6th, 2019.
 */

[CustomEditor(typeof(CC_CANOE))]
[CanEditMultipleObjects]
public class CC_EDITORCANOE : Editor {

    [SerializeField]
    private CC_CAMERA.SelectedCamera selCamera;
    [HideInInspector]
    private bool enableStereo;
    [HideInInspector]
    private int destCameraIndex;
    [HideInInspector]
    private int interaxial;
    [HideInInspector]
    private float centerClickRange = 0.5f;
    [HideInInspector]
    private DpadType dpadType = DpadType.EightDirections;

    private CC_CANOE canoeTarget;

    private void Awake() {
        canoeTarget = (CC_CANOE)target;
    }

    public override void OnInspectorGUI() {
        //Get This Target
        canoeTarget = (CC_CANOE)target;
        //Disconnect the CC_CANOE prefab instance from the original prefab
        bool isPrefabOriginal = (PrefabUtility.GetPrefabParent(target) == null) && (PrefabUtility.GetPrefabObject(target) != null);
        if (!isPrefabOriginal) {
            PrefabUtility.DisconnectPrefabInstance(target);
        }

        EditorGUI.BeginChangeCheck();

        //TRACKING STARTS
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Tracking Settings", EditorStyles.boldLabel);
        bool vive = EditorGUILayout.Toggle(new GUIContent("Vive Tracking", "Use the HTC Vive tracking system."), canoeTarget.vive);
        bool optitrack = EditorGUILayout.Toggle(new GUIContent("OptiTrack Tracking", "Use the OptiTrack tracking system."), canoeTarget.optitrack);
        //TRACKING ENDS

        //CAMERA STARTS
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Camera Settings", EditorStyles.boldLabel);
        selCamera = (CC_CAMERA.SelectedCamera)EditorGUILayout.EnumPopup(new GUIContent("Selected Camera", "Select which camera to use. Keyboard Shortcut: 'P'"), GameObject.Find("CC_HEAD").GetComponent<CC_CAMERA>().selectCamera);
        enableStereo = EditorGUILayout.Toggle(new GUIContent("Enable Stereo", "Enable/Disable stereoscopic. Keyboard Shortcut: '9'"), GameObject.Find("CC_HEAD").GetComponent<CC_CAMERA>().enableStereo);
        interaxial = EditorGUILayout.IntField(new GUIContent("Interaxial", "Interaxial distance in millimeters.Keyboard Shortcut: '-' and '+'"), GameObject.Find("CC_HEAD").GetComponent<CC_CAMERA>().interaxial);
        destCameraIndex = (int)EditorGUILayout.Slider(new GUIContent("Destiny Camera Index", "Change the view to different cameras of Destiny. Keyboard Shortcut: '[' and ']'"), GameObject.Find("CC_HEAD").GetComponent<CC_CAMERA>().destinyCameraIndex, 0, 7);
        float cameraForwartTilt = EditorGUILayout.Slider(new GUIContent("Camera Forward Tilt", "Change the foward tilt of the camera."), canoeTarget.cameraForwardTilt, -90f, 90f);
        //CAMERA ENDS

        //WANDS STARTS
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Wand Settings", EditorStyles.boldLabel);
        Wand simulatorActiveWand = (Wand)EditorGUILayout.EnumPopup(new GUIContent("Selected Wand", "The currently active simulator wand."), canoeTarget.simulatorActiveWand);
        CC_CANOE.WandModel wandModel = (CC_CANOE.WandModel)EditorGUILayout.EnumPopup(new GUIContent("Wand Model", "The wand model you wish to be visible. Keyboard Shortcut: '5'"), canoeTarget.wandModel);
        if (canoeTarget.isVive()) {
            dpadType = (DpadType)EditorGUILayout.EnumPopup(new GUIContent("Dpad Type", "The Dpad configuration (4 directions or 8 directions) you wish to use."), canoeTarget.dpadType);
            centerClickRange = EditorGUILayout.Slider(new GUIContent("Center Click Range", "Change the range of the Dpad center click."), canoeTarget.centerClickRange, 0.1f, 0.9f);
        }
        //WAND ENDS

        //SIM NAV STARTS
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Simulator Navigation Settings", EditorStyles.boldLabel);
        float navigationSpeed = EditorGUILayout.FloatField(new GUIContent("Movement Speed", "Navigation speed of the simulator controls. (Only affects WASD key movement)"), canoeTarget.navigationSpeed);
        float navigationRotationSpeed = EditorGUILayout.FloatField(new GUIContent("Rotational Speed", "Rotational speed of the simulator controls. (Only affects WASD key movement)"), canoeTarget.navigationRotationSpeed);
        //SIM NAV ENDS

        //MISC STARTS
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Misc Settings", EditorStyles.boldLabel);
        CC_CANOE.ShowScreen showScreen = (CC_CANOE.ShowScreen)EditorGUILayout.EnumPopup(new GUIContent("Show Screen", "Enable/Disable the visibility of the CyberCANOE's Screens. Typically you want this set to none unless you are debugging in the editor. Keyboard Shortcut: '6'"), canoeTarget.showScreen);
        bool applyGravity = EditorGUILayout.Toggle(new GUIContent("Apply Gravity", "Enable or disable gravity the Canoe/Player experiences. Does not affect other objects in the scene."), canoeTarget.applyGravity);
        bool kbcont = EditorGUILayout.Toggle(new GUIContent("Keyboard Controls", "Enable or disable the keyboard controls. This affects the simualtor, camera, and canoe controls."), canoeTarget.kbcont);

        //MISC ENDS

        EditorGUI.EndChangeCheck();

        GUILayout.Space(20);
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.fontSize = 12;
        style.fontStyle = FontStyle.Bold;

        if (GUI.changed) {
            try {
                Undo.RecordObject(canoeTarget, "Updated Canoe Settings");
                //TRACKING STARTS
                if (vive && !canoeTarget.vive) {
                    canoeTarget.vive = true;
                    canoeTarget.optitrack = false;
                } else if (optitrack && !canoeTarget.optitrack) {
                    canoeTarget.vive = false;
                    canoeTarget.optitrack = true;
                }
                //TRACKING ENDS

                //CAMERA STARTS
                GameObject.Find("CC_HEAD").GetComponent<CC_CAMERA>().selectCamera = selCamera;
                GameObject.Find("CC_HEAD").GetComponent<CC_CAMERA>().enableStereo = enableStereo;
                GameObject.Find("CC_HEAD").GetComponent<CC_CAMERA>().destinyCameraIndex = destCameraIndex;
                GameObject.Find("CC_HEAD").GetComponent<CC_CAMERA>().interaxial = interaxial;
                canoeTarget.cameraForwardTilt = cameraForwartTilt;
                //CAMERA ENDS

                //WAND STARTS
                canoeTarget.simulatorActiveWand = simulatorActiveWand;
                canoeTarget.wandModel = wandModel;
                canoeTarget.dpadType = dpadType;
                canoeTarget.centerClickRange = centerClickRange;
                //WAND ENDS

                //SIM NAV STARTS
                canoeTarget.navigationSpeed = navigationSpeed;
                canoeTarget.navigationRotationSpeed = navigationRotationSpeed;
                //SIM NAV ENDS

                //MISC STARTS
                canoeTarget.showScreen = showScreen;
                canoeTarget.applyGravity = applyGravity;
                canoeTarget.kbcont = kbcont;
                //MISC ENDS
            }
            catch (System.NullReferenceException) {
                Debug.Log("CC_HEAD can't be found. CC_HEAD should be a child of CC_CANOE.");
            }
        }
    }
}
