using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

/* 
The CC_CANOE custom inspector/editor script.
Unity Editor script that performs miscellaneous tasks before buidling project.
Must click the "CLICK HERE BEFORE BUIDLING" button on the CC_CANOE GameObject before building your project.

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: 1.3, May 12th, 2017.
 */

[CustomEditor(typeof(CC_CANOE))]
public class CC_EDITORCANOE : Editor
{
    private CC_CAMERA.SelectedCamera selCamera;
    private bool enableStereo;
    private int destCameraIndex;
    private int interaxial;

    public override void OnInspectorGUI()
    {
        //Get This Target
        CC_CANOE canoeTarget = (CC_CANOE)target;

        //Disconnect the CC_CANOE prefab instance from the original prefab
        bool isPrefabOriginal = PrefabUtility.GetPrefabParent(target) == null && PrefabUtility.GetPrefabObject(target) != null;
        if (!isPrefabOriginal)
            PrefabUtility.DisconnectPrefabInstance(target);

        //CAMERA STARTS
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Camera Settings", EditorStyles.boldLabel);
        selCamera = (CC_CAMERA.SelectedCamera)EditorGUILayout.EnumPopup(new GUIContent("Selected Camera", "Select which camera to use. Keyboard Shortcut: 'P'"), GameObject.Find("CC_HEAD").GetComponent<CC_CAMERA>().selectCamera);
        enableStereo = EditorGUILayout.Toggle(new GUIContent("Enable Stereo", "Enable/Disable stereoscopic. Keyboard Shortcut: '9'"), GameObject.Find("CC_HEAD").GetComponent<CC_CAMERA>().enableStereo);
        interaxial = EditorGUILayout.IntField(new GUIContent("Interaxial", "Interaxial distance in millimeters.Keyboard Shortcut: '-' and '+'"), GameObject.Find("CC_HEAD").GetComponent<CC_CAMERA>().interaxial);
        destCameraIndex = (int)EditorGUILayout.Slider(new GUIContent("Destiny Camera Index", "Change the view to different cameras of Destiny. Keyboard Shortcut: '[' and ']'"), GameObject.Find("CC_HEAD").GetComponent<CC_CAMERA>().destinyCameraIndex, 0, 7);

        //Camera ENDS

        //WANDS STARTS
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Wand Settings", EditorStyles.boldLabel);
        canoeTarget.simulatorActiveWand = (Wand)EditorGUILayout.EnumPopup(new GUIContent("Selected Wand", "The currently active simulator wand."), canoeTarget.simulatorActiveWand);
        canoeTarget.wandModel = (CC_CANOE.WandModel)EditorGUILayout.EnumPopup(new GUIContent("Wand Model", "The wand model you wish to be visible. Keyboard Shortcut: '5'"), canoeTarget.wandModel);
        //WAND ENDS

        //NAV STARTS
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Simulator Navigation Settings", EditorStyles.boldLabel);
        canoeTarget.navigationSpeed = EditorGUILayout.FloatField(new GUIContent("Movement Speed", "Navigation speed of the simulator controls. (Only affects WASD key movement)"), canoeTarget.navigationSpeed);
        canoeTarget.navigationRotationSpeed = EditorGUILayout.FloatField(new GUIContent("Rotational Speed", "Rotational speed of the simulator controls. (Only affects WASD key movement)"), canoeTarget.navigationRotationSpeed);
        //NAV ENDS

        //MISC STARTS
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Misc Settings", EditorStyles.boldLabel);
        canoeTarget.showScreen = (CC_CANOE.ShowScreen)EditorGUILayout.EnumPopup(new GUIContent("Show Screen", "Enable/Disable the visibility of the CyberCANOE's Screens. Typically you want this set to none unless you are debugging in the editor. Keyboard Shortcut: '6'"), canoeTarget.showScreen);
        canoeTarget.applyGravity = EditorGUILayout.Toggle(new GUIContent("Apply Gravity", "Enable or disable gravity the Canoe/Player experiences. Does not affect other objects in the scene."), canoeTarget.applyGravity);
        canoeTarget.kbcont = EditorGUILayout.Toggle(new GUIContent("Keyboard Controls", "Enable or disable the keyboard controls. This affects the simualtor, camera, and canoe controls."), canoeTarget.kbcont);
        //MISC ENDS

        GUILayout.Space(20);
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.fontSize = 12;
        style.fontStyle = FontStyle.Bold;
        if (GUILayout.Button("CLICK HERE BEFORE BUILDING", style, GUILayout.Height(25)))
        {
            BeforeBuild();
        }

        if (GUI.changed)
        {
            try
            {
                GameObject.Find("CC_HEAD").GetComponent<CC_CAMERA>().selectCamera = selCamera;
                GameObject.Find("CC_HEAD").GetComponent<CC_CAMERA>().enableStereo = enableStereo;
                GameObject.Find("CC_HEAD").GetComponent<CC_CAMERA>().destinyCameraIndex = destCameraIndex;
                GameObject.Find("CC_HEAD").GetComponent<CC_CAMERA>().interaxial = interaxial;
            }
            catch (System.NullReferenceException)
            {
                Debug.Log("CC_HEAD can't be found. CC_HEAD should be a child of CC_CANOE.");
            }
        }

    } 

    private void BeforeBuild()
    {
        GameObject.Find("CC_CANOE").GetComponent<CC_CANOE>().showScreen = CC_CANOE.ShowScreen.None;
        PlayerSettings.defaultIsFullScreen = false;
        PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
        PlayerSettings.resizableWindow = true;
    }

}
