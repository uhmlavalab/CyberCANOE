using UnityEngine;
using System.Collections;

/* 
Updates the wand position and rotation by interfacing with CC_TRACKER.

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: October 26th, 2016.
 */

/// <summary> Keeps track of wand position and rotation. </summary>
public class CC_WAND : MonoBehaviour
{
    public Wand wand;
    private CC_TRACKER scr;

    int updateCount;
    void Awake()
    {
        scr = GameObject.Find("CC_CANOE").GetComponent<CC_TRACKER>();
    }

    void Update()
    {
        //Set the location of the wand from the tracker information.
        transform.localPosition = scr.getWandPosition((int)wand);
        transform.localRotation = scr.getWandRotation((int)wand);

    }
}

