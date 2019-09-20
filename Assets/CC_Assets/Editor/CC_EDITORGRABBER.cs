using UnityEngine;
using UnityEditor;

/* 
The CCaux_Grabber custom inspector/editor script.

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: 1.14, August 6th, 2019.
 */

[CustomEditor(typeof(CCaux_Grabber))]
public class CC_EDITORGRABBER : Editor {

    private CCaux_Grabber grabTarget;
    private CC_CANOE canoe;

    private void Awake() {
        grabTarget = (CCaux_Grabber)target;
    }

    public override void OnInspectorGUI() {
        //Disconnect the prefab instance from the original prefab
        bool isPrefabOriginal = (PrefabUtility.GetPrefabParent(target) == null) && (PrefabUtility.GetPrefabObject(target) != null);
        if (!isPrefabOriginal) {
            PrefabUtility.DisconnectPrefabInstance(target);
        }

        EditorGUI.BeginChangeCheck();

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Grabber Settings", EditorStyles.boldLabel);

        bool enableGrabbing = EditorGUILayout.Toggle(new GUIContent("Enable Grabbing", "Toggles grabbing using the specified wand."), grabTarget.enableGrabbing);

        //Disable grabbing if enable grabbin is false
        EditorGUI.BeginDisabledGroup(!enableGrabbing);

        bool grabWithTrigger = EditorGUILayout.Toggle(new GUIContent("Grab With Trigger", "Toggles navigation with trigger. Overrides Grab Button."), grabTarget.grabWithTrigger);

        //Disable grab button if grab with trigger is true
        EditorGUI.BeginDisabledGroup(grabWithTrigger);

        WandButton validGrabButton = grabTarget.grabButton;
        WandButton grabButton = (WandButton)EditorGUILayout.EnumPopup(new GUIContent("Grab Button", "Select button to be used to grab."), grabTarget.grabButton);

        //End grab button disabled group
        EditorGUI.EndDisabledGroup();

        //End grabber disabled group
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(20);

        EditorGUI.EndChangeCheck();

        Undo.RecordObject(grabTarget, "Updated Grabber Settings");
        canoe = GameObject.Find("CC_CANOE").GetComponent<CC_CANOE>();

        //GRABBER STARTS
        grabTarget.enableGrabbing = enableGrabbing;
        grabTarget.grabWithTrigger = grabWithTrigger;

        string location = "CCaux_Grabber on " + grabTarget.gameObject.name + "/\"Grabber Settings\"/\"Grab Button\"";

        //Button logic to keep user from selecting incorrect input for grab button
        if (canoe.isVive() && !CC_INPUT.IsViveInput(grabButton)) {
            if (!CC_INPUT.IsEnumSeparator(grabButton)) {
                if (!CC_INPUT.IsViveInput(validGrabButton)) {
                    Debug.LogError(location + ": Wand button \"" + grabButton + "\" is not Vive input. Defaulting to \"Menu\" button.");
                    grabButton = grabTarget.grabButton = WandButton.Menu;
                } else {
                    Debug.LogError(location + ": Wand button \"" + grabButton + "\" is not Vive input.");
                    grabButton = grabTarget.grabButton = validGrabButton;
                }
            }
            throw new ExitGUIException();
        } else if (canoe.isOptiTrack() && !CC_INPUT.IsOptiTrackInput(grabButton)) {
            if (!CC_INPUT.IsEnumSeparator(grabButton)) {
                if (!CC_INPUT.IsOptiTrackInput(validGrabButton)) {
                    Debug.LogError(location + ": Wand button \"" + grabButton + "\" is not OptiTrack input. Defaulting to \"X\" button.");
                    grabButton = grabTarget.grabButton = WandButton.X;
                } else {
                    Debug.LogError(location + ": Wand button \"" + grabButton + "\" is not OptiTrack input.");
                    grabButton = grabTarget.grabButton = validGrabButton;
                }
            }
            throw new ExitGUIException();
        } else {
            grabTarget.grabButton = grabButton;
        }
        //Button logic end

        //GRABBER ENDS
    }
}
