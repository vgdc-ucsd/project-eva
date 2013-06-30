using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {

	public float smooth = 1.0f;
	private Transform standardPos;

	protected void Start () {
		standardPos = GameObject.Find( "PlayerCameraPos" ).transform;
	}

	protected void Update () {
		transform.position = standardPos.position;
		transform.localRotation = standardPos.parent.rotation;
	}
}
