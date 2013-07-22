using UnityEngine;
using System.Collections;

public class ARV80_Rifle : Weapon_Stats {

	public float burstFireCoolDown;
	public float burstFireAmount;
	public float burstFireToggleRate;

	public GameObject debris;

	private float currentBurstFireCoolDown;
	private float currentBurstFireToggleRate;

	void Awake () {
		player = GameObject.Find( "PlayerAndCamera" );

		burstFireCoolDown = 0.5f;
		burstFireAmount = 3.0f;
		burstFireToggleRate = 0.5f;

		swapRate = 1.0f;
		reloadRate = 2.5f;
		coolDown = 0.1f;
		maxAmmo = 32.0f;
		maxSpare = 200.0f;
		range = 10000.0f;
		damage = 10.0f;
		bulletSpread = 0.01f;
		bulletCircleRadius = 1.0f;

		isReloading = false;
		hasAmmo = true;
		isSwapping = true;
		altFire = false;

		currentAmmo = maxAmmo;
		currentReloadRate = reloadRate;
		currentSpareAmmo = maxSpare;
		currentCoolDown = coolDown;
		currentSwapRate = swapRate;
	}

	void Start () {
		currentBurstFireCoolDown = burstFireCoolDown;
		currentBurstFireToggleRate = burstFireToggleRate;

		isReloading = false;
		isSwapping = true;
	}

	void Update () {

		//when swapping to this weapon initiates cooldown for weapon swap
		IsSwappingCheck();

		//changing burstfire
		if ( Input.GetButtonDown( InputConstants.AltFire ) ) {
			altFire = AltFireToggle();
		}

		//initial check for burst fire, ammo, swap cooldown and reload cooldown
		if ( !altFire && hasAmmo && currentSwapRate <= 0 && !isReloading ) {
			AutomaticShooting();
		}

		//TODO main shooting code block for burstfire
		if ( altFire && hasAmmo && currentSwapRate <= 0 && !isReloading ) {
			BurstFireShooting();
		}

		Debug.Log( "Ammo: " + currentAmmo + " Spare: " + currentSpareAmmo );

		//allows you to reload if you run out of ammo and you click once; or if you press R and you don't have max ammo
		if ( ( Input.GetButtonDown( InputConstants.Fire ) && !hasAmmo && currentSpareAmmo > 0 ) ||
			( Input.GetButtonDown( InputConstants.Reload ) && currentAmmo != maxAmmo && currentSpareAmmo > 0 ) ) {
			Reload();
		}

		IsReloadingCheck();
	}

	//handles burst fire shooting
	private void BurstFireShooting () {
		//TODO
	}

	//handles automatic shooting
	private void AutomaticShooting () {
		//firerate
		currentCoolDown -= Time.deltaTime;

		//main shooting code block for automatic
		if ( Input.GetButton( InputConstants.Fire ) && currentCoolDown <= 0 ) {
			currentCoolDown = coolDown;

			//play shooting sound
			//emit muzzle flare
			//emit bullet tracer image?

			//randomgenerate coordinates to imitate bullet spread, default circle radius is 1.0f
			Vector2 bulletSpreadCircle = Random.insideUnitCircle;

			//adjusted bullet direction with bulletspread
			Vector3 rayDirection = new Vector3(
				player.transform.forward.x + ( bulletSpread * bulletSpreadCircle.x ) ,
				player.transform.forward.y + ( bulletSpread * bulletSpreadCircle.y ) ,
				Camera.main.transform.forward.z );

			//creating the bullet, origin is camera
			Ray ray = new Ray( Camera.main.transform.position , rayDirection );


			//returns true if hits collider, false if nothing hit
			if ( Physics.Raycast( ray , out hitInfo , range ) ) {
				//coordinates of hit
				Vector3 hitPoint = hitInfo.point;
				//Debug.Log("Hit Point: " + hitPoint);

				//object hit, null if none hit
				GameObject hitObject = hitInfo.collider.gameObject;
				//Debug.Log("Hit Object: " + hitObject);

				if ( hitObject.tag == "Enemy" ) {
					//damage enemy
					//enemy.receiveDamage(damage);
				}

				if ( hitObject.tag == "Cover" ) {
					//damage cover
					//cover.receiveDamage(damage);
				}

				//show bullet hit particles
				if ( debris != null ) {
					Instantiate( debris , hitPoint , Quaternion.identity );
				}
				Debug.DrawLine( player.transform.position , hitPoint );
			}

			currentAmmo--;

			//if you run out of Ammo
			if ( currentAmmo <= 0 ) {
				hasAmmo = false;
			}
		}
	}
}
