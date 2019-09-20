using UnityEngine;
using UnityEditor;

/* 
The CCaux_OmniNavigator custom inspector/editor script.
Unity Editor script that performs miscellaneous tasks before buidling project.

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: 1.14, August 6th, 2019.
 */


[CustomEditor(typeof(CCaux_OmniNavigator))]
public class CC_EDITORNAVIGATOR : Editor {

    private CCaux_OmniNavigator navTarget;
    private CC_CANOE canoe;

    private void Awake() {
        navTarget = (CCaux_OmniNavigator)target;
    }

    public override void OnInspectorGUI() {
        //Get This Target
        navTarget = (CCaux_OmniNavigator)target;
        canoe = GameObject.Find("CC_CANOE").GetComponent<CC_CANOE>();
        //Disconnect the prefab instance from the original prefab
        bool isPrefabOriginal = (PrefabUtility.GetPrefabParent(target) == null) && (PrefabUtility.GetPrefabObject(target) != null);
        if (!isPrefabOriginal) {
            PrefabUtility.DisconnectPrefabInstance(target);
        }

        EditorGUI.BeginChangeCheck();

        //NAVIGATION STARTS
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Navigation Settings", EditorStyles.boldLabel);
        bool enableNavigation = EditorGUILayout.Toggle(new GUIContent("Enable Navigation", "Toggle navigation using navigation input (default is trigger)."), navTarget.enableNavigation);

        //Disable navigation if enable navigation is false
        EditorGUI.BeginDisabledGroup(!enableNavigation);

        bool disableNavigationX = EditorGUILayout.Toggle(new GUIContent("Disable X-Axis Movement", "Toggle movement in the X-axis while navigating."), navTarget.disableNavigationX);
        bool disableNavigationY = EditorGUILayout.Toggle(new GUIContent("Disable Y-Axis Movement", "Toggle movement in the Y-axis while navigating."), navTarget.disableNavigationY);
        bool disableNavigationZ = EditorGUILayout.Toggle(new GUIContent("Disable Z-Axis Movement", "Toggle movement in the Z-axis while navigating."), navTarget.disableNavigationZ);
        CCaux_OmniNavigator.rotationLock lockRotation = (CCaux_OmniNavigator.rotationLock)EditorGUILayout.EnumPopup(new GUIContent("Lock Rotation Around Axis", "Select an axis to lock rotation around."), navTarget.lockRotation);
        Wand wandToUse = (Wand)EditorGUILayout.EnumPopup(new GUIContent("Navigation Wand", "Select wand to use for navigation."), navTarget.wandToUse);
        bool navWithTrigger = EditorGUILayout.Toggle(new GUIContent("Navigate with Trigger", "Toggles navigation with trigger. Overrides Navigation Button."), navTarget.navWithTrigger);

        //Disable navigation button selection if trigger is used for navigation
        EditorGUI.BeginDisabledGroup(navWithTrigger);
        WandButton validNavButton = navTarget.navButton;
        WandButton navButton = (WandButton)EditorGUILayout.EnumPopup(new GUIContent("Navigation Button", "Select button to use for navigation."), navTarget.navButton);

        //End navigation button disabled group
        EditorGUI.EndDisabledGroup();

        float moveSpeed = EditorGUILayout.FloatField(new GUIContent("Movement Speed", "Set navigation movement speed."), navTarget.moveSpeed);
        float rotateSpeed = EditorGUILayout.FloatField(new GUIContent("Rotation Speed", "Set navigation rotation speed."), navTarget.rotateSpeed);
        //NAVIGATION ENDS

        //RESET STARTS
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Reset Settings", EditorStyles.boldLabel);
        WandButton validResetButton = navTarget.resetButton;
        WandButton resetButton = (WandButton)EditorGUILayout.EnumPopup(new GUIContent("Reset Button", "Select button to reset CANOE position."), navTarget.resetButton);
        float resetSpeed = EditorGUILayout.FloatField(new GUIContent("Reset Speed", "Set the CANOE's speed when returning to its reset position."), navTarget.resetSpeed);
        Vector3 resetPosition = EditorGUILayout.Vector3Field(new GUIContent("Reset Position", "Set the CANOE's reset position."), navTarget.resetPosition);
        Vector3 resetRotation = EditorGUILayout.Vector3Field(new GUIContent("Reset Rotation", "Set the CANOE's reset rotation."), navTarget.resetRotation);
        //RESET ENDS

        //MISC STARTS
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Misc Settings", EditorStyles.boldLabel);
        GameObject cursor = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Cursor Object", "Set object to display while navigating."), navTarget.cursor, typeof(Object), true);
        //MISC ENDS

        //End navigation disabled group
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(20);

        EditorGUI.EndChangeCheck();

        Undo.RecordObject(navTarget, "Updated Navigator Settings");
        canoe = GameObject.Find("CC_CANOE").GetComponent<CC_CANOE>();

        //NAVIGATION STARTS
        navTarget.enableNavigation = enableNavigation;
        navTarget.disableNavigationX = disableNavigationX;
        navTarget.disableNavigationY = disableNavigationY;
        navTarget.disableNavigationZ = disableNavigationZ;

        navTarget.lockRotation = lockRotation;
        navTarget.wandToUse = wandToUse;
        navTarget.navWithTrigger = navWithTrigger;

        string location = "CCaux_OmniNavigator/\"Navigation Settings\"/\"Navigation Button\"";

        //Button logic to keep user from selecting incorrect input for navigation button
        if (canoe.isVive() && !CC_INPUT.IsViveInput(navButton)) {
            if (!CC_INPUT.IsEnumSeparator(navButton)) {
                if (!CC_INPUT.IsViveInput(validNavButton)) {
                    Debug.LogError(location + ": Wand button \"" + navButton + "\" is not Vive input. Defaulting to \"Menu\" button.");
                    navButton = navTarget.navButton = WandButton.Menu;
                } else {
                    Debug.LogError(location + ": Wand button \"" + navButton + "\" is not Vive input.");
                    navButton = navTarget.navButton = validNavButton;
                }
            }
            throw new ExitGUIException();
        } else if (canoe.isOptiTrack() && !CC_INPUT.IsOptiTrackInput(navButton)) {
            if (!CC_INPUT.IsEnumSeparator(navButton)) {
                if (!CC_INPUT.IsOptiTrackInput(validNavButton)) {
                    Debug.LogError(location + ": Wand button \"" + navButton + "\" is not OptiTrack input. Defaulting to \"X\" button.");
                    navButton = navTarget.navButton = WandButton.X;
                } else {
                    Debug.LogError(location + ": Wand button \"" + navButton + "\" is not OptiTrack input.");
                    navButton = navTarget.navButton = validNavButton;
                }
            }
            throw new ExitGUIException();
        } else {
            navTarget.navButton = navButton;
        }
        //Button logic end

        navTarget.moveSpeed = moveSpeed;
        navTarget.rotateSpeed = rotateSpeed;
        //NAVIGATION ENDS

        //RESET STARTS
        location = "CCaux_OmniNavigator/\"Reset Settings\"/\"Reset Button\"";

        //Button logic to keep user from selecting incorrect input for reset button
        if (canoe.isVive() && !CC_INPUT.IsViveInput(resetButton)) {
            if (!CC_INPUT.IsEnumSeparator(resetButton)) {
                if (!CC_INPUT.IsViveInput(validResetButton)) {
                    Debug.LogError(location + ": Wand button \"" + resetButton + "\" is not Vive input. Defaulting to \"Menu\" button.");
                    resetButton = navTarget.resetButton = WandButton.Menu;
                } else {
                    Debug.LogError(location + ": Wand button \"" + resetButton + "\" is not Vive input.");
                    resetButton = navTarget.resetButton = validResetButton;
                }
            }
            throw new ExitGUIException();
        } else if (canoe.isOptiTrack() && !CC_INPUT.IsOptiTrackInput(resetButton)) {
            if (!CC_INPUT.IsEnumSeparator(resetButton)) {
                if (!CC_INPUT.IsOptiTrackInput(validResetButton)) {
                    Debug.LogError(location + ": Wand button \"" + resetButton + "\" is not OptiTrack input. Defaulting to \"X\" button.");
                    resetButton = navTarget.resetButton = WandButton.X;
                } else {
                    Debug.LogError(location + ": Wand button \"" + resetButton + "\" is not OptiTrack input.");
                    resetButton = navTarget.resetButton = validResetButton;
                }
            }
            throw new ExitGUIException();
        } else {
            navTarget.resetButton = resetButton;
        }
        //Button logic end

        navTarget.resetSpeed = resetSpeed;
        navTarget.resetPosition = resetPosition;
        navTarget.resetRotation = resetRotation;
        //RESET ENDS

        //MISC STARTS
        navTarget.cursor = cursor;
        //MISC ENDS
    }
}
