using UnityEngine;
using System.Collections;

public class S107_SniperRifle : Weapon_Stats {
	public float regCamView; //original camera view
	public float zoomMultiplier; //Zoom 7x (1/7)

	private PlayerEffects effectsController;
	private NetworkManager networkManager;
	private NetworkViewID myViewID;
	private bool isOnCoolDown;
	private bool shotFired;

	void Awake() {
		player = transform.parent.gameObject;
		effectsController = transform.parent.GetComponent<PlayerEffects>();
		networkManager = GameObject.FindGameObjectWithTag( Tags.NetworkController ).GetComponent<NetworkManager>();

		swapRate = 1.0f;
		reloadRate = 4.0f;
		coolDown = 1.0f;
		maxAmmo = 1.0f;
		maxSpare = 4.0f;
		range = 100000.0f;
		damage = 75.0f;
		bulletSpread = 0.01f;
		bulletCircleRadius = 0.5f;

		regCamView = 60.0f;
		zoomMultiplier = 0.14285f;

		WeaponAwake();

		shotFired = false;
	}

	void Start() {
		myViewID = networkManager.my.avatar.networkView.viewID;
		
		Camera.main.fieldOfView = regCamView;
		WeaponStart();
	}

	void Update() {

		//when swapping to this weapon initiates cooldown for weapon swap
		IsSwappingCheck();
		IsReloadingCheck();

		//changing burstfire
		if ( Input.GetButtonDown( InputConstants.AltFire ) ){
			altFire = ZoomToggle();
		}

		//initial check for burst fire, ammo, swap cooldown and reload cooldown
		if ( hasAmmo && currentSwapRate <= 0 && !isReloading ) {
			Fire();
		}

		//allows you to reload if you run out of ammo and you click once; or if you press R and you don't have max ammo
		if ( ( Input.GetButtonDown( InputConstants.Fire ) && !hasAmmo && currentSpareAmmo > 0 && isOnCoolDown ) ||
			( Input.GetButtonDown( InputConstants.Reload ) && currentAmmo != maxAmmo && currentSpareAmmo > 0 && isOnCoolDown ) ) {
			Reload();
		}

		if ( shotFired ) {
			isOnCoolDown = true;
		}
	}

	//handles burst fire toggle on off
	public bool ZoomToggle() {
		if ( !altFire ) {
			Camera.main.fieldOfView = Camera.main.fieldOfView * zoomMultiplier;
			return true;
		} else {
			Camera.main.fieldOfView = regCamView;
			return false;
		}
	}


	//handles shooting
	private void Fire() {

		if ( Input.GetButtonDown( InputConstants.Fire ) ) {
			//randomgenerate coordinates to imitate bullet spread, default circle radius is 1.0f
			Vector2 bulletSpreadCircle = Random.insideUnitCircle * bulletCircleRadius;

			//adjusted bullet direction with bulletspread
			Vector3 rayDirection = new Vector3(
				player.transform.forward.x + ( bulletSpread * bulletSpreadCircle.x ),
				player.transform.forward.y + ( bulletSpread * bulletSpreadCircle.y ),
				Camera.main.transform.forward.z );

			//creating the bullet, origin is camera
			Ray ray = new Ray( Camera.main.transform.position, rayDirection );

			//returns true if hits collider, false if nothing hit
			if ( Physics.Raycast( ray, out hitInfo, range ) ) {
				Vector3 hitPoint = hitInfo.point;

				//object hit, null if none hit
				GameObject hitObject = hitInfo.collider.gameObject;
				
				if ( hitObject.tag == "Player" ) {
					NetworkPlayer hitPlayer = hitInfo.collider.networkView.owner;
					transform.parent.networkView.RPC ("InflictDamage", hitPlayer, damage, myViewID);
				}

				if ( hitObject.tag == "Cover" ) {
					hitObject.SendMessage("receiveDamage", damage);
				}

				//show bullet hit particles
				effectsController.TriggerGunSpark( hitPoint, hitInfo.normal );
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
