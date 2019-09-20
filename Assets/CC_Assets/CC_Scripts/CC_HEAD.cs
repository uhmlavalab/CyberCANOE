using UnityEngine;

/* 
Updates the head's position and rotation by interfacing with CC_TRACKER.

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: 1.14, August 6th, 2019.
 */

/// <summary> Keeps track of head position and rotation. </summary>
public class CC_HEAD : MonoBehaviour {
    private CC_TRACKER tracker;
    int updateCount;

    void Awake() {
        tracker = GameObject.Find("CC_CANOE").GetComponent<CC_TRACKER>();
    }

    void FixedUpdate() {
        //Set the location of the head from the tracker information.
        transform.localPosition = tracker.GetHeadPosition();
        transform.localRotation = tracker.GetHeadRotation();
    }
}
