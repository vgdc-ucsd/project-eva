using UnityEngine;
using System.Collections;

public class Weapon_Stats : MonoBehaviour {

	protected float swapRate;
	protected float reloadRate;
	protected float coolDown;
	protected float maxAmmo;
	protected float maxSpare;
	protected float range;
	protected float damage;
	protected float bulletSpread;
	protected float bulletCircleRadius;

	protected GameObject player;
	protected RaycastHit hitInfo;

	protected bool isReloading;
	protected bool hasAmmo;
	protected bool isSwapping;
	protected bool altFire;

	protected float currentAmmo;
	protected float currentReloadRate;
	protected float currentSpareAmmo;
	protected float currentSwapRate;
	protected float currentCoolDown;

	public float GetCurrentAmmo() {
		return currentAmmo;
	}

	public float GetSpareAmmo() {
		return currentSpareAmmo;
	}

	protected void WeaponAwake() {
		isReloading = false;
		hasAmmo = true;
		isSwapping = true;
		altFire = false;

		currentAmmo = maxAmmo;
		currentReloadRate = reloadRate;
		currentSpareAmmo = maxSpare;
		currentSwapRate = swapRate;
		currentCoolDown = coolDown;
	}

	protected void WeaponStart() {
		currentCoolDown = coolDown;
		currentReloadRate = reloadRate;
		currentSwapRate = swapRate;
		isSwapping = true;
	}

	protected void IsReloadingCheck () {
		if ( isReloading ) {
			currentReloadRate -= Time.deltaTime;
			if ( currentReloadRate <= 0 ) {
				isReloading = false;
			}
		}
	}

	protected void IsSwappingCheck () {
		if ( isSwapping ) {
			currentSwapRate -= Time.deltaTime;
			if ( currentSwapRate <= 0 ) {
				isSwapping = false;
			}
		}
	}

	protected bool AltFireToggle () {
		if ( !altFire ) {
			return true;
		} else {
			return false;
		}
	}

	protected void Reload () {
		//play reload animation
		//play reload sound? etc.
		currentReloadRate = reloadRate;

		//currentSpareAmmo -= ( maxAmmo - currentAmmo );
		if ( ( currentSpareAmmo - ( maxAmmo - currentAmmo ) ) < 0 ) {
			currentAmmo += currentSpareAmmo;
			currentSpareAmmo = 0;
		} else {
			currentSpareAmmo -= ( maxAmmo - currentAmmo );
			currentAmmo += ( maxAmmo - currentAmmo );
		}

		hasAmmo = true;
		isReloading = true;
	}
}
