using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour {

	public float translateForce = 1.0f;
	public float rotateForce = 0.2f;
	public float mouseSensitivity = 0.08f;
	public float brakesForce = 0.03f;
	public float translateBrakeDeadzone = 0.2f;
	public float rotationBrakeDeadzone = 0.3f;
	private PlayerBoost boostController;
	public bool inMenu = false;

	protected void Awake() {
		if ( !networkView.isMine ) {
			enabled = false;
		}
	}
	
	protected void Start() {
		Screen.lockCursor = true;
		boostController = GetComponent<PlayerBoost>();
	}

	protected void FixedUpdate() {
		if( ! inMenu ) {
			float tX = Input.GetAxis( InputConstants.TranslateX );
			float tY = Input.GetAxis( InputConstants.TranslateY );
			float tZ = Input.GetAxis( InputConstants.TranslateZ );
			float dPitch = Input.GetAxis( InputConstants.Pitch );
			float dYaw = Input.GetAxis( InputConstants.Yaw );
			float dRoll = Input.GetAxis( InputConstants.Roll );

			DoRotation( dPitch, dYaw, dRoll );

			if( Input.GetAxis( InputConstants.Brakes ) != 0 ) {
				DoStop();
			}
			if( Input.GetAxis( InputConstants.Boost ) != 0 ) {
				boostController.DoBoost( transform.forward );
			} else {
				DoTranslation( tX, tY, tZ );
			}
		} else {
			DoStop();
		}
		networkView.RPC( "UpdatePosition", RPCMode.Others, transform.position, transform.rotation );
	}

	private void DoTranslation( float tX, float tY, float tZ ) {
		rigidbody.AddRelativeForce( tX * translateForce, tY * translateForce, tZ * translateForce );
	}

	private void DoRotation( float dP, float dY, float dR ) {
		rigidbody.AddRelativeTorque( 0.0f, 0.0f, dR * rotateForce );
		transform.Rotate( dP * mouseSensitivity, dY * mouseSensitivity, 0, Space.Self );
	}

	private void DoStop() {
		rigidbody.velocity = Vector3.Lerp( rigidbody.velocity, Vector3.zero, brakesForce );
		if( rigidbody.velocity.magnitude < translateBrakeDeadzone ) {
			rigidbody.velocity = Vector3.zero;
		}
		rigidbody.angularVelocity = Vector3.Slerp( rigidbody.angularVelocity, Vector3.zero, brakesForce );
		if( rigidbody.angularVelocity.magnitude < rotationBrakeDeadzone ) {
			rigidbody.angularVelocity = Vector3.zero;
		}
	}

	[RPC]
	void UpdatePosition( Vector3 position, Quaternion rotation ) {
		transform.position = position;
		transform.rotation = rotation;
	}
}
