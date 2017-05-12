using UnityEngine;
using System.Collections;

/* 
Updates the head's position and rotation by interfacing with CC_TRACKER.

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: 1.3, May 12th, 2017.
 */

/// <summary> Keeps track of head position and rotation. </summary>
public class CC_HEAD : MonoBehaviour
{
    private CC_TRACKER tracker;

    int updateCount;

    void Awake()
    {
        tracker = GameObject.Find("CC_CANOE").GetComponent<CC_TRACKER>();
    }

    void Update()
    {
        //Set the location of the head from the tracker information.
        transform.localPosition = tracker.getHeadPosition();
        transform.localRotation = tracker.getHeadRotation();
    }

}
