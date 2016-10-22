using UnityEngine;
using System.Collections;

public class RotateAround : MonoBehaviour {

	public float speed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround(Vector3.zero, Vector3.up, speed * Time.deltaTime);
	}
}
