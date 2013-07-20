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
	public Texture2D crosshairImage;


	private PlayerBoost boostController;
	private PlayerCover coverController;

	protected void Awake() {
		Screen.lockCursor = true;
		boostController = GetComponent<PlayerBoost>();
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
			boostController.DoBoost( transform.forward );
		} else {
			doTranslation( tX, tY, tZ );
		}
	}

	private void doTranslation( float tX, float tY, float tZ ) {
		if( coverController.coverEngaged ) {
			coverController.GetRelativeMovement( ref tX, ref tY, ref tZ );
		}
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
	
	void OnGUI() {
		
		float xMin = Screen.width/2 - ( crosshairImage.width/2 );
		float yMin = Screen.height/2 - ( crosshairImage.height/2 );
		GUI.DrawTexture(new Rect(xMin,yMin,crosshairImage.width,crosshairImage.height),crosshairImage);
		GUI.Label(new Rect(Screen.width - 100,Screen.height-50,100,50),"Boosts: " + boostController.currBoosts);	
	}
}
