using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour {

	public float translateForce = 1.0f;
	public float rotateForce = 1.0f;
	public float mouseSensitivity = 1.0f;
	public float brakesForce = 0.03f;
	public float translateBrakeDeadzone = 0.2f;
	public float rotationBrakeDeadzone = 0.3f;
	public float boostForce = 2.0f;
	public int maxBoosts = 3;
	public float boostDuration = 1.0f;

	private int currBoosts;
	private float currBoostTimer;

	private bool isBoosting = false;
	private bool stoppedBoosting = true;

	private PlayerEffects effectsController;

	protected void Awake() {
		Screen.lockCursor = true;
		effectsController = GetComponent<PlayerEffects>();
		currBoosts = maxBoosts;
		currBoostTimer = boostDuration;
	}

	protected void Update() {
		if( isBoosting ) {
			currBoostTimer -= Time.deltaTime;
			if( currBoostTimer <= 0 ) {
				stopBoost();
			}
		} else {
			currBoostTimer = boostDuration;
		}
	}

	protected void FixedUpdate() {
		float tX = Input.GetAxis( InputConstants.TranslateX );
		float tY = Input.GetAxis( InputConstants.TranslateY );
		float tZ = Input.GetAxis( InputConstants.TranslateZ );
		float dPitch = Input.GetAxis( InputConstants.Pitch );
		float dYaw = Input.GetAxis( InputConstants.Yaw );
		float dRoll = Input.GetAxis( InputConstants.Roll );

		doRotation( dPitch, dYaw, dRoll );

		if( Input.GetAxis( InputConstants.Brakes ) != 0 ) {
			doStop();
		}

		if( Input.GetAxis( InputConstants.Boost ) != 0 ) {
			doBoost( transform.forward );
		} else {
			doTranslation( tX, tY, tZ );
			stoppedBoosting = true;
		}
	}

	private void doTranslation( float tX, float tY, float tZ ) {
		rigidbody.AddRelativeForce( tX * translateForce, tY * translateForce, tZ * translateForce );
	}

	private void doRotation( float dP, float dY, float dR ) {
		rigidbody.AddRelativeTorque( 0.0f, 0.0f, dR * rotateForce );
		transform.Rotate( dP * mouseSensitivity, dY * mouseSensitivity, 0, Space.Self );
	}

	private void doStop() {
		rigidbody.velocity = Vector3.Lerp( rigidbody.velocity, Vector3.zero, brakesForce );
		if( rigidbody.velocity.magnitude < translateBrakeDeadzone ) {
			rigidbody.velocity = Vector3.zero;
		}
		rigidbody.angularVelocity = Vector3.Slerp( rigidbody.angularVelocity, Vector3.zero, brakesForce );
		if( rigidbody.angularVelocity.magnitude < rotationBrakeDeadzone ) {
			rigidbody.angularVelocity = Vector3.zero;
		}
	}

	private void doBoost( Vector3 facing ) {
		if( !canBoost() ) return;
		isBoosting = true;
		stoppedBoosting = false;

		effectsController.startLightTrail();

		rigidbody.velocity = new Vector3( facing.x * boostForce, facing.y * boostForce, facing.z * boostForce );
	}

	private void stopBoost() {
		rigidbody.velocity = rigidbody.velocity.normalized;
		currBoosts--;
		isBoosting = false;
		effectsController.stopLightTrail();
	}

	private bool canBoost() {
		return currBoostTimer > 0
				&& currBoosts > 0
				&& stoppedBoosting;
	}
}
