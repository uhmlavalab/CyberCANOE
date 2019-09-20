using UnityEditor;
using UnityEditor.Build;

/* 
This class performs actions before the executable build is complete.

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: 1.14, August 6th, 2019.
 */

/// <summary> Resets player settings before executable build is complete. </summary>
public class BeforeBuild : IPreprocessBuild {

    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildTarget target, string path) {
        PlayerSettings.defaultIsFullScreen = false;
        PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
        PlayerSettings.resizableWindow = true;
    }
}
