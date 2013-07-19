using UnityEngine;
using System.Collections;

public class S107_SniperRifle : MonoBehaviour {

	//when you first swap to this weapon
	public float SWAP_RATE = 1.0f;

	//No actual shot cooldown for Sniper
	public float SNIPER_SHOT_COOLDOWN = 1.0f;

	//weapon stats variables
	public float MAX_AMMO = 1.0f;
	public float MAX_SPARE = 2.0f;
	public float RELOAD_RATE = 4.0f;
	public float RANGE = 100000.0f;
	public float DAMAGE = 75.0f;
	public float BULLET_SPREAD = 0.01f;
	public float BULLET_SPREAD_CIRCLE = 0.25f; //default is 1

	//burst fire variables
	public float ZOOM_TOGGLERATE = 0.5f;
	public float REG_CAMVIEW = 60.0f; //original camera view
	public float ZOOM_MULTIPLIER = 0.14285f; //Zoom 7x (1/7)
	public bool zoom = false;

	//bullet hit graphic: bullethole etc.
	public GameObject DEBRIS_PREFAB;

	//mutatable variables
	private float currentSwapRate;
	//private float currentCoolDown;
	private float currentReloadRate;
	private float currentAmmo;
	private float currentSpareAmmo;
	private float currentCoolDown;

	//check variables
	private bool hasAmmo = true;
	private bool isReloading = false;
	private bool isSwapping = true;
	private bool isInitialized = false;
	private bool isOnCoolDown = false;
	private bool shotFired = false;

	private RaycastHit hitInfo;

	//access player transform.position
	private GameObject player;

	void Start() {
		player = GameObject.Find( "PlayerAndCamera" );
		TimerInitialization();

		if ( !isInitialized ) {
			WeaponInitialization();
		}
	}

	private void TimerInitialization() {
		currentSwapRate = SWAP_RATE;
		currentReloadRate = 0;
		currentCoolDown = SNIPER_SHOT_COOLDOWN;
	}

	private void WeaponInitialization() {
		currentAmmo = 1.0f;
		currentSpareAmmo = 2.0f;
		isInitialized = true;
	}

	void Update() {

		//when swapping to this weapon initiates cooldown for weapon swap
		IsSwappingCheck();
		IsReloadingCheck();

		//changing burstfire
		if ( Input.GetButtonDown( InputConstants.AltFire ) ) {
			zoom = ZoomToggle();
		}

		//initial check for burst fire, ammo, swap cooldown and reload cooldown
		if ( hasAmmo && currentSwapRate <= 0 && !isReloading ) {
			Fire();
		}

		Debug.Log( "Ammo: " + currentAmmo + " Spare: " + currentSpareAmmo );

		//allows you to reload if you run out of ammo and you click once; or if you press R and you don't have max ammo
		if ( ( Input.GetButtonDown( InputConstants.Fire ) && !hasAmmo && currentSpareAmmo > 0 && isOnCoolDown ) ||
			( Input.GetButtonDown( InputConstants.Reload ) && currentAmmo != MAX_AMMO && currentSpareAmmo > 0 && isOnCoolDown ) ) {
			Reload();
		}

		if ( shotFired ) {
			isOnCoolDown = true;
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

		//currentSpareAmmo -= ( maxAmmo - currentAmmo );
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

	//handles burst fire toggle on off
	private bool ZoomToggle() {
		if ( !zoom ) {
			Camera.main.fieldOfView = Camera.main.fieldOfView * ZOOM_MULTIPLIER;
			return true;
		} else {
			Camera.main.fieldOfView = REG_CAMVIEW;
			return false;
		}
	}


	//handles shooting
	private void Fire() {
		//play shooting sound
		//emit muzzle flare
		//emit bullet tracer image?

		if ( Input.GetButtonDown( InputConstants.Fire ) ) {
			//randomgenerate coordinates to imitate bullet spread, default circle radius is 1.0f
			Vector2 bulletSpreadCircle = Random.insideUnitCircle * BULLET_SPREAD_CIRCLE;

			//adjusted bullet direction with bulletspread
			Vector3 rayDirection = new Vector3(
				player.transform.forward.x + ( BULLET_SPREAD * bulletSpreadCircle.x ),
				player.transform.forward.y + ( BULLET_SPREAD * bulletSpreadCircle.y ),
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

				//show bullet hit particles
				if ( DEBRIS_PREFAB != null ) {
					Instantiate( DEBRIS_PREFAB, hitPoint, Quaternion.identity );
				}
				//Debug.DrawLine( player.transform.position , hitPoint );
			}

			currentAmmo--;
			shotFired = true;

			//if you run out of Ammo
			if ( currentAmmo <= 0 ) {
				hasAmmo = false;
			}
		}
	}
}