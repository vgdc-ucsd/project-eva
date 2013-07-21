using UnityEngine;
using System.Collections;

public class CQB22_Shotgun : Weapon_Stats {

	public float shotgunPellets;

	public GameObject debris;
	void Awake () {

		player = GameObject.Find( "PlayerAndCamera" );

		shotgunPellets = 10.0f;
		swapRate = 1.0f;
		reloadRate = 2.5f;
		coolDown = 0.5f;
		maxAmmo = 4.0f;
		maxSpare = 24.0f;
		range = 100000.0f;
		damage = 5.0f;
		bulletSpread = 0.01f;
		bulletCircleRadius = 5.0f;

		WeaponAwake();
	}

	void Start() {
		WeaponStart();
	}

	// Update is called once per frame
	void Update() {

		IsSwappingCheck();

		if ( Input.GetButtonDown( InputConstants.AltFire ) ) {
			altFire = AltFireToggle();
		}

		if ( !altFire && hasAmmo && currentSwapRate <= 0 && !isReloading ) {
			Fire();
		}

		if ( altFire && hasAmmo && currentSwapRate <= 0 && !isReloading ) {
			RicochetFire();
		}

		Debug.Log( "Ammo: " + currentAmmo + " Spare: " + currentSpareAmmo );

		if ( ( Input.GetButtonDown( InputConstants.Fire ) && !hasAmmo && currentSpareAmmo > 0 ) ||
			( Input.GetButtonDown( InputConstants.Reload ) && currentAmmo != maxAmmo && currentSpareAmmo > 0 ) ) {
			Reload();
		}

		IsReloadingCheck();
	}

	/*-----------------------------------------------------------------------------------------------------*/

	private void Fire() {
		currentCoolDown -= Time.deltaTime;

		if ( Input.GetButtonDown( InputConstants.Fire ) && currentCoolDown <= 0 ) {
			currentCoolDown = coolDown;

			//play shooting sound
			//muzzle flare
			//bullet tracer

			for ( int i = 0; i < shotgunPellets; i++ ) {
				Vector2 shotgunRadius = Random.insideUnitCircle * bulletCircleRadius;

				Vector3 rayDirection = new Vector3(
					player.transform.forward.x + ( bulletSpread * shotgunRadius.x ),
					player.transform.forward.y + ( bulletSpread * shotgunRadius.y ),
					Camera.main.transform.forward.z );

				//creating the bullet, origin is camera
				Ray ray = new Ray( Camera.main.transform.position, rayDirection );


				//returns true if hits collider, false if nothing hit
				if ( Physics.Raycast( ray, out hitInfo, range ) ) {
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

					if ( debris != null ) {
						Instantiate( debris, hitPoint, Quaternion.identity );
					}
				}
			}
			currentAmmo--;

			if ( currentAmmo <= 0 ) {
				hasAmmo = false;
			}
		}
	}

	private void RicochetFire () {
		//NEED BULLET RICOCHET
		currentCoolDown -= Time.deltaTime;

		if ( Input.GetButtonDown( InputConstants.Fire ) && currentCoolDown <= 0 ) {
			currentCoolDown = coolDown;

			//play shooting sound
			//muzzle flare
			//bullet tracer

			for ( int i = 0; i < shotgunPellets; i++ ) {
				Vector2 shotgunRadius = Random.insideUnitCircle * bulletCircleRadius;

				Vector3 rayDirection = new Vector3(
					player.transform.forward.x + ( bulletSpread * shotgunRadius.x ) ,
					player.transform.forward.y + ( bulletSpread * shotgunRadius.y ) ,
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

					if ( debris != null ) {
						Instantiate( debris , hitPoint , Quaternion.identity );
					}

					/*

					Debug.Log( reflectedRay );

					Ray reflectedRay = new Ray( reflect.position , reflect.forward );

					if ( Physics.Raycast( reflectedRay , out reflectedHitInfo , RANGE ) ) {
						Vector3 reflectedHitPoint = reflectedHitInfo.point;

						GameObject reflectedHitObject = reflectedHitInfo.collider.gameObject;

						if ( hitObject.tag == "Enemy" ) {
							//damage enemy
							//enemy.receiveDamage(damage);
						}

						if ( hitObject.tag == "Cover" ) {
							//damage cover
							//cover.receiveDamage(damage);
						}

						if ( DEBRIS_PREFAB != null ) {
							Instantiate( DEBRIS_PREFAB , reflectedHitPoint , Quaternion.identity );
						}
					}
				}
					  */
				}
				currentAmmo--;

				if ( currentAmmo <= 0 ) {
					hasAmmo = false;
				}
			}
		}
	}
}
