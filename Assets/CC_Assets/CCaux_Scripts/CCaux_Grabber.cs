using UnityEngine;

/* 
This is an example script that allows the user to grab objects.
This script should be attached to the CC_WAND_LEFT or CC_WAND_RIGHT game objects under the CC_CANOE game object.

Down Button - Grab object

CyberCANOE Virtual Reality API for Unity3D
(C) 2016 Ryan Theriot, Jason Leigh, Laboratory for Advanced Visualization & Applications, University of Hawaii at Manoa.
Version: 1.14, August 6th, 2019.
 */

public class CCaux_Grabber : MonoBehaviour {

    public bool enableGrabbing;
    public WandButton grabButton;
    public bool grabWithTrigger;

    [HideInInspector]
    public string wandName = "";

    private Wand wand;
    private bool grab;
    private GameObject currentObject = null;
    private GameObject grabbedObject = null;
    private Transform grabbedObjectParent = null;
    private bool wasKinematic = false;


    void Start() {
        //Get the wand component attached to this GameObject
        wand = GetComponent<CC_WAND>().wand;

        wandName = gameObject.name;
    }

    void Update() {
        //Set grab setting
        grab = false;
        if (enableGrabbing) {
            if (grabWithTrigger) {
                if (CC_INPUT.GetAxis(wand, WandAxis.Trigger) > 0.0f)
                    grab = true;
                else
                    grab = false;
            } else {
                grab = CC_INPUT.GetButtonPress(wand, grabButton);
            }
        }

        if (grab) {
            if (grabbedObject == null) {

                if (currentObject != null) {
                    grabbedObject = currentObject;

                    // If object had a rigidbody, grabbed save the rigidbody's kinematic state
                    // so it can be restored on release of the object
                    Rigidbody body = null;
                    body = grabbedObject.GetComponent<Rigidbody>();
                    if (body != null) {
                        wasKinematic = body.isKinematic;
                        body.isKinematic = true;
                    }

                    // Save away to original parentage of the grabbed object
                    grabbedObjectParent = grabbedObject.transform.parent;

                    // Make the grabbed object a child of the wand
                    grabbedObject.transform.parent = CC_CANOE.WandGameObject(wand).transform;
                    currentObject = null;

                    // Disable collision between yourself and the grabbed object so that the grabbed object
                    // does not apply its physics to you and push you off the world
                    Physics.IgnoreCollision(CC_CANOE.CanoeCharacterController(), grabbedObject.GetComponent<Collider>(), true);

                }
            }
        } else {

            if (grabbedObject != null) {

                // Restore the original parentage of the grabbed object
                grabbedObject.transform.parent = grabbedObjectParent;

                // If object had a rigidbody, restore its kinematic state
                Rigidbody body = null;
                body = grabbedObject.GetComponent<Rigidbody>();
                if (body != null) {
                    body.isKinematic = wasKinematic;
                }

                //Re-enstate collision between self and object
                Physics.IgnoreCollision(CC_CANOE.CanoeCharacterController(), grabbedObject.GetComponent<Collider>(), false);

                grabbedObject = null;
                currentObject = null;
            }
        }
    }

    void OnTriggerEnter(Collider collision) {
        if (grabbedObject == null)
            currentObject = collision.gameObject;
    }

    void OnTriggerExit(Collider collision) {
        currentObject = null;
    }
}
