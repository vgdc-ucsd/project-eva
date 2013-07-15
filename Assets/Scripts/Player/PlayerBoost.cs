using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerEffects))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerBoost : MonoBehaviour {

	public float boostForce = 5.0f;
	public int maxBoosts = 3;
	public float boostDuration = 1.0f;
	public float rechargeTime = 25.0f;

	public int currBoosts;
	private float currBoostTime;
	private float currRechargeTime;
	private bool isBoosting = false;
	private bool boostKeyLifted = true;

	private PlayerEffects effectsController;

	protected void Awake() {
		effectsController = GetComponent<PlayerEffects>();
		currBoosts = maxBoosts;
		currBoostTime = boostDuration;
	}

	protected void Update() {
		if( isBoosting ) {
			currBoostTime -= Time.deltaTime;
			if( currBoostTime <= 0 ) {
				StopBoost();
			}
		} else {
			currBoostTime = boostDuration;
		}

		DoRechargeTimer();

		if( Input.GetAxis( InputConstants.Boost ) == 0 ) {
			boostKeyLifted = true;
		}
	}

	private void DoRechargeTimer() {
		currRechargeTime -= Time.deltaTime;
		if( currRechargeTime <= 0 ) {
			currBoosts = Mathf.Min( currBoosts + 1, maxBoosts );
			currRechargeTime = rechargeTime;
		}
	}

	public void DoBoost( Vector3 facing ) {
		if( CanBoost() ) {
			isBoosting = true;
			boostKeyLifted = false;

			effectsController.StartLightTrail();

			rigidbody.velocity = new Vector3( facing.x * boostForce, facing.y * boostForce, facing.z * boostForce );
		}
	}

	private void StopBoost() {
		rigidbody.velocity = rigidbody.velocity.normalized;
		currBoosts--;
		isBoosting = false;
		effectsController.StopLightTrail();
	}

	private bool CanBoost() {
		return currBoostTime > 0
				&& currBoosts > 0
				&& boostKeyLifted;
	}
}
