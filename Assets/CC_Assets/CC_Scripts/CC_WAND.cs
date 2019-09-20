using UnityEngine;

/* 
Updates the wand position and rotation by interfacing with CC_TRACKER.

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: 1.14, August 6th, 2019.
 */

/// <summary>Keeps track of wand position and rotation. </summary>
public class CC_WAND : MonoBehaviour {
    public Wand wand;
    private CC_TRACKER scr;

    int updateCount;
    void Awake() {
        scr = GameObject.Find("CC_CANOE").GetComponent<CC_TRACKER>();
    }

    void FixedUpdate() {
        //Set the location of the wand from the tracker information.
        transform.localPosition = scr.GetWandPosition((int)wand);
        transform.localRotation = scr.GetWandRotation((int)wand);
    }
}
