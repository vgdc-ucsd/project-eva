using UnityEngine;
using System.Collections;

public class PlayerWeapons : MonoBehaviour {
	
	public GameObject AssaultRifle;
	public GameObject SniperRifle;
	public int currentWeapon = 1;
	public int numWeapons = 2;
	private int? nextWeapon = null;
	private ARV80_Rifle assaultRifleScript;
	private S107_SniperRifle sniperRifleScript;
	private Weapon_Stats currentWeaponScript;

	protected void Start() {
		assaultRifleScript = AssaultRifle.GetComponent<ARV80_Rifle>();
		sniperRifleScript = SniperRifle.GetComponent<S107_SniperRifle>();
		
		if( currentWeapon == 1 ) {
			currentWeaponScript = assaultRifleScript;
		} else if ( currentWeapon == 2 ) {
			currentWeaponScript = sniperRifleScript;
		}
		currentWeaponScript.enabled = true;
	}
	
	void Update() {
		
		if( Input.GetAxis("SwitchWeapon") > 0 ) {	
			nextWeapon = currentWeapon + 1;
			
			if( nextWeapon <= numWeapons ) {
				currentWeapon = nextWeapon.Value;
			} else {
				currentWeapon = 1;
			}
		} else if( Input.GetAxis("SwitchWeapon") < 0 ) {
			nextWeapon = currentWeapon - 1;
			
			if( nextWeapon.Value > 0 ) {
				currentWeapon = nextWeapon.Value;
			} else {
				currentWeapon = numWeapons;
			}				
		} else {
			nextWeapon = null;	
		}
		
		if ( nextWeapon.HasValue ) {
			if( currentWeapon == 1 ) {
				currentWeaponScript.enabled = false;
				currentWeaponScript = assaultRifleScript;
			} else if ( currentWeapon == 2 ) {
				currentWeaponScript.enabled = false;
				currentWeaponScript = sniperRifleScript;
			}
			currentWeaponScript.enabled = true;
		}
	}

	public float GetWeaponCurrentAmmo() {
		return currentWeaponScript.GetCurrentAmmo();
	}

	public float GetWeaponSpareAmmo() {
		return currentWeaponScript.GetSpareAmmo();
	}
	
	public float GetWeaponMaxAmmo() {
		return currentWeaponScript.GetMaxAmmo();
	}
	
	public float GetWeaponMaxSpare() {
		return currentWeaponScript.GetMaxSpare();
	}
	
	public bool IsFullCheck() {
		if( GetWeaponCurrentAmmo() == GetWeaponMaxAmmo() && GetWeaponSpareAmmo() == GetWeaponMaxSpare() ) {
			return true;
		} else { return false; }
	}
	
	public void SetWeaponCurrentAmmo( float ammo ) {
		currentWeaponScript.SetCurrentAmmo( ammo );
	}
}
