using UnityEngine;
using System.Collections;

public class CQB22_Shotgun : MonoBehaviour {

	//when you first swap to this weapon
	public float SWAP_RATE = 1.0f;
	//firerate
	public float COOLDOWN = 1.0f;

	//weapon stats variables
	public float MAX_AMMO = 4.0f;
	public float MAX_SPARE = 24.0f;
	public float SHOTGUN_PELLETS = 10.0f;
	public float RELOAD_RATE = 3.0f;
	public float RANGE = 100.0f;
	public float DAMAGE = 5.0f;
	public float BULLET_SPREAD = 2.0f;
	public float BULLET_CIRCLE_RADIUS = 7.0f;
	public bool bulletRicochet = false;

	public GameObject DEBRIS_PREFAB;

	private float currentSwapRate;
	private float currentCoolDown;
	private float currentReloadRate;
	private float currentAmmo;
	private float currentSpareAmmo;

	private bool hasAmmo = true;
	private bool isReloading = false;
	private bool isSwapping = true;
	private bool isInitialized = false;

	private RaycastHit hitInfo;
	private RaycastHit reflectedHitInfo;

	private GameObject player;

	void Start() {
		player = GameObject.Find( "PlayerAndCamera" );
		TimerInitialization();

		if ( !isInitialized ) {
			WeaponInitialization();
		}
	}

	private void WeaponInitialization() {
		currentAmmo = 4.0f;
		currentSpareAmmo = 24.0f;
		isInitialized = true;
	}

	private void TimerInitialization() {
		currentSwapRate = SWAP_RATE;
		currentCoolDown = COOLDOWN;
		currentReloadRate = 0;
	}

	// Update is called once per frame
	void Update() {

		IsSwappingCheck();

		if ( Input.GetButtonDown( InputConstants.AltFire ) ) {
			bulletRicochet = BulletRicochetToggle();
		}

		if ( !bulletRicochet && hasAmmo && currentSwapRate <= 0 && !isReloading ) {
			Fire();
		}

		if ( bulletRicochet && hasAmmo && currentSwapRate <= 0 && !isReloading ) {
			RicochetFire();
		}

		Debug.Log( "Ammo: " + currentAmmo + " Spare: " + currentSpareAmmo );

		if ( ( Input.GetButtonDown( InputConstants.Fire ) && !hasAmmo && currentSpareAmmo > 0 ) ||
			( Input.GetButtonDown( InputConstants.Reload ) && currentAmmo != MAX_AMMO && currentSpareAmmo > 0 ) ) {
			Reload();
		}

		IsReloadingCheck();
	}

	/*-----------------------------------------------------------------------------------------------------*/

	private void Fire() {
		currentCoolDown -= Time.deltaTime;

		if ( Input.GetButtonDown( InputConstants.Fire ) && currentCoolDown <= 0 ) {
			currentCoolDown = COOLDOWN;

			//play shooting sound
			//muzzle flare
			//bullet tracer

			for ( int i = 0; i < SHOTGUN_PELLETS; i++ ) {
				Vector2 shotgunRadius = Random.insideUnitCircle * BULLET_CIRCLE_RADIUS;

				Vector3 rayDirection = new Vector3(
					player.transform.forward.x + ( BULLET_SPREAD * shotgunRadius.x ),
					player.transform.forward.y + ( BULLET_SPREAD * shotgunRadius.y ),
					Camera.main.transform.forward.z );

				//creating the bullet, origin is camera
				Ray ray = new Ray( Camera.main.transform.position, rayDirection );


				//returns true if hits collider, false if nothing hit
				if ( Physics.Raycast( ray, out hitInfo, RANGE ) ) {
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

					if ( DEBRIS_PREFAB != null ) {
						Instantiate( DEBRIS_PREFAB, hitPoint, Quaternion.identity );
					}
				}
			}
			currentAmmo--;

			if ( currentAmmo <= 0 ) {
				hasAmmo = false;
			}
		}
	}

	private void RicochetFire() {
		//NEED BULLET RICOCHET
		currentCoolDown -= Time.deltaTime;

		if ( Input.GetButtonDown( InputConstants.Fire ) && currentCoolDown <= 0 ) {
			currentCoolDown = COOLDOWN;

			//play shooting sound
			//muzzle flare
			//bullet tracer

			for ( int i = 0; i < SHOTGUN_PELLETS; i++ ) {
				Vector2 shotgunRadius = Random.insideUnitCircle * BULLET_CIRCLE_RADIUS;

				Vector3 rayDirection = new Vector3(
					player.transform.forward.x + ( BULLET_SPREAD * shotgunRadius.x ),
					player.transform.forward.y + ( BULLET_SPREAD * shotgunRadius.y ),
					Camera.main.transform.forward.z );

				//creating the bullet, origin is camera
				Ray ray = new Ray( Camera.main.transform.position, rayDirection );


				//returns true if hits collider, false if nothing hit
				if ( Physics.Raycast( ray, out hitInfo, RANGE ) ) {

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

					if ( DEBRIS_PREFAB != null ) {
						Instantiate( DEBRIS_PREFAB, hitPoint, Quaternion.identity );
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



	//check to see if player is in reload animation
	private void IsReloadingCheck() {
		if ( isReloading ) {
			currentReloadRate -= Time.deltaTime;
			if ( currentReloadRate <= 0 ) {
				isReloading = false;
			}
		}
	}

	//handles reloading 
	private void Reload() {
		//play reload animation
		//play reload sound? etc.
		currentReloadRate = RELOAD_RATE;

		if ( ( currentSpareAmmo - ( MAX_AMMO - currentAmmo ) ) < 0 ) {
			currentAmmo = currentSpareAmmo;
			currentSpareAmmo = 0;
		} else {
			currentSpareAmmo -= ( MAX_AMMO - currentAmmo );
			currentAmmo += ( MAX_AMMO - currentAmmo );
		}

		hasAmmo = true;
		isReloading = true;
	}

	//check to see if player swapped to this weapon
	private void IsSwappingCheck() {
		if ( isSwapping ) {
			currentSwapRate -= Time.deltaTime;
			if ( currentSwapRate <= 0 ) {
				isSwapping = false;
			}
		}
	}

	//handles ricochet fire toggle on off
	private bool BulletRicochetToggle() {
		if ( !bulletRicochet ) {
			currentCoolDown = COOLDOWN;
			return true;
		} else {
			currentCoolDown = COOLDOWN;
			return false;
		}
	}
}
