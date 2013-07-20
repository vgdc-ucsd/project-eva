using UnityEngine;
using System.Collections;

public class ARV80_Rifle : MonoBehaviour {

	//when you first swap to this weapon
	public float SWAP_RATE = 1.0f;
	//firerate
	public float COOLDOWN = 0.1f;
 
	//weapon stats variables
	public float MAX_AMMO = 32.0f;
	public float MAX_SPARE = 50.0f;
	public float RELOAD_RATE = 2.5f;
	public float RANGE = 100000.0f;
	public float DAMAGE = 10.0f;
	public float BULLET_SPREAD = 0.01f;

	//burst fire variables
	public float BURST_FIRE_COOLDOWN = 0.5f;
	public float BURST_FIRE_AMOUNT = 3.0f;
	public float BURST_FIRE_TOGGLERATE = 0.5f;
	public bool BURST_FIRE = false;

	//bullet hit graphic: bullethole etc.
	public GameObject DEBRIS_PREFAB;
	public GameObject EFFECT;

	//mutatable variables
	private float currentSwapRate;
	private float currentCoolDown;
	private float currentReloadRate;
	public float currentAmmo;
	public float currentSpareAmmo;
	private float currentBurstFireCoolDown;
	private float currentBurstFireToggleRate;

	//check variables
	private bool hasAmmo = true;
	private bool isReloading = false;
	private bool isSwapping = true;
	private bool isInitialized = false;

	private RaycastHit hitInfo;

	//access player transform.position
	private GameObject player;

	void Start () {
		player = GameObject.Find( "PlayerAndCamera" );
		TimerInitialization();

		if ( !isInitialized ) {
			WeaponInitialization();
		}
	}

	private void TimerInitialization () {
		currentSwapRate = SWAP_RATE;
		currentCoolDown = COOLDOWN;
		currentReloadRate = 0;
		currentBurstFireCoolDown = BURST_FIRE_COOLDOWN;
		currentBurstFireToggleRate = BURST_FIRE_TOGGLERATE;
	}

	private void WeaponInitialization () {
		currentAmmo = 32.0f;
		currentSpareAmmo = 200.0f;
		isInitialized = true;
	}
	
	void Update () {

		//when swapping to this weapon initiates cooldown for weapon swap
		IsSwappingCheck();

		//changing burstfire
		if ( Input.GetButtonDown( InputConstants.AltFire ) ) {
			BURST_FIRE = BurstFireToggle();
		}

		//initial check for burst fire, ammo, swap cooldown and reload cooldown
		if ( !BURST_FIRE && hasAmmo && currentSwapRate <= 0 && !isReloading ) {
			AutomaticShooting();
		}
		
		//TODO main shooting code block for burstfire
		if ( BURST_FIRE && hasAmmo && currentSwapRate <= 0 && !isReloading ) {
			BurstFireShooting();
		}
		
		//Debug.Log( "Ammo: " + currentAmmo + " Spare: " + currentSpareAmmo );

		//allows you to reload if you run out of ammo and you click once; or if you press R and you don't have max ammo
		if ( ( Input.GetButtonDown( InputConstants.Fire ) && !hasAmmo && currentSpareAmmo > 0) || 
			( Input.GetButtonDown( InputConstants.Reload ) && currentAmmo != MAX_AMMO && currentSpareAmmo > 0) ) {
			Reload();
		}

		IsReloadingCheck();
	}

	/*----------------------------------------------------------------------------------------------------*/

	//check to see if player is in reload animation
	private void IsReloadingCheck () {
		if ( isReloading ) {
			currentReloadRate -= Time.deltaTime;
			if ( currentReloadRate <= 0 ) {
				isReloading = false;
			}
		}
	}

	//handles reloading 
	private void Reload () {
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
	private void IsSwappingCheck () {
		if ( isSwapping ) {
			currentSwapRate -= Time.deltaTime;
			if ( currentSwapRate <= 0 ) {
				isSwapping = false;
			}
		}
	}

	//handles burst fire toggle on off
	private bool BurstFireToggle () {
		if ( !BURST_FIRE ) {
			return true;
		} else {
			return false;
		}
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
			currentCoolDown = COOLDOWN;

			//play shooting sound
			//emit muzzle flare
			//emit bullet tracer image?

			//randomgenerate coordinates to imitate bullet spread, default circle radius is 1.0f
			Vector2 bulletSpreadCircle = Random.insideUnitCircle;

			//adjusted bullet direction with bulletspread
			Vector3 rayDirection = new Vector3(
				player.transform.forward.x + ( BULLET_SPREAD * bulletSpreadCircle.x ) ,
				player.transform.forward.y + ( BULLET_SPREAD * bulletSpreadCircle.y ) ,
				Camera.main.transform.forward.z );

			//creating the bullet, origin is camera
			Ray ray = new Ray( Camera.main.transform.position , rayDirection );


			//returns true if hits collider, false if nothing hit
			if ( Physics.Raycast( ray , out hitInfo , RANGE ) ) {
				//coordinates of hit
				Vector3 hitPoint = hitInfo.point;
				//Debug.Log("Hit Point: " + hitPoint);

				//object hit, null if none hit
				GameObject hitObject = hitInfo.collider.gameObject;
				//Debug.Log("Hit Object: " + hitObject);

				if ( hitObject.tag == "Enemy" ) {
					//damage enemy
					hitObject.transform.SendMessage("receiveDamage",DAMAGE, SendMessageOptions.DontRequireReceiver);
				}

				if ( hitObject.tag == "Cover" ) {
					//damage cover
					//cover.receiveDamage(damage);
				}

				//show bullet hit particles
				if ( DEBRIS_PREFAB != null ) {
					Instantiate( DEBRIS_PREFAB , hitPoint , Quaternion.identity );
				}
				Debug.DrawLine( player.transform.position , hitPoint );
				
				//show bullet hit effect
				if ( EFFECT != null ) {
					GameObject particleClone = Instantiate(EFFECT,hitPoint,Quaternion.LookRotation(hitInfo.normal)) as GameObject;
					Destroy(particleClone.gameObject,2); 	
				}
			}

			currentAmmo--;

			//if you run out of Ammo
			if ( currentAmmo <= 0 ) {
				hasAmmo = false;
			}
		}
	}
}