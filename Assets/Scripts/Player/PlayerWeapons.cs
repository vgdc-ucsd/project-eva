using UnityEngine;
using System.Collections;

public class PlayerWeapons : MonoBehaviour {
	
	public GameObject AssaultRifle;
	public GameObject SniperRifle;
	
	private Weapon_Stats[] weaponSlots;  // player's weapon slots
	private int totalWeapons = 3; // number of weapons in the game
	private int availWeapons = 0; // number of weapons player owns
	
	//Start with weapon 0
	private int currentWeapon = 0;
	private int nextWeapon = 0;
	
	private ARV80_Rifle assaultRifleScript;
	private S107_SniperRifle sniperRifleScript;
	private Weapon_Stats currentWeaponScript;
	
	protected void Awake() {
		weaponSlots = new Weapon_Stats[totalWeapons];
		assaultRifleScript = AssaultRifle.GetComponent<ARV80_Rifle>();
		sniperRifleScript = SniperRifle.GetComponent<S107_SniperRifle>();
		
		// Populate list of weapons
		for (int i=0; i<totalWeapons; ++i) {
			weaponSlots[i] = null;
		}
		weaponSlots[0] = assaultRifleScript;
		availWeapons = 1;
		
		// Set initial weapon
		currentWeaponScript = weaponSlots[0];
		currentWeaponScript.enabled = true;
	}
	
	void Update() {
		if( Input.GetAxis("SwitchWeapon") > 0 && availWeapons > 1 ) {
			nextWeapon = currentWeapon + 1;
			
			if ( nextWeapon >= totalWeapons) {
				nextWeapon = 0;
			}
			while ( weaponSlots[nextWeapon] == null ) {
				++nextWeapon;
			}
		} else if( Input.GetAxis("SwitchWeapon") < 0 && availWeapons > 1 ) {
			nextWeapon = currentWeapon - 1;
			
			if ( nextWeapon < 0 ) { 
				nextWeapon = totalWeapons - 1;
			}
			
			while ( weaponSlots[nextWeapon] == null) {
				--nextWeapon;
			}
		} else {
			nextWeapon = currentWeapon;	
		}
		
		if ( nextWeapon != currentWeapon ) {
			
			if( currentWeapon == 2 ) {
				sniperRifleScript.ZoomToggle();		
			}
			currentWeapon = nextWeapon;
			currentWeaponScript.enabled = false;
			currentWeaponScript = weaponSlots[currentWeapon];
			currentWeaponScript.enabled = true;
		}
	}
	
	//These getters correspond to the currently equipped weapon
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
	
	//These getters are for specific weapons (to query ammo counts of weapons not currently equipped)
	//New ones will need to be written as weapons are implemented
	public float GetSniperCurrentAmmo() {
		return sniperRifleScript.GetCurrentAmmo();
	}
	
	public float GetSniperSpareAmmo() {
		return sniperRifleScript.GetSpareAmmo();
	}	
	
	public float GetSniperMaxAmmo() {
		return sniperRifleScript.GetMaxAmmo();
	}
	
	public float GetSniperMaxSpare() {
		return sniperRifleScript.GetMaxSpare();	
	}
	
	//Overloaded method for when a specific weapon's stats are requested
	public bool IsFullCheck( string weapon ) {
		
		if( weapon == "SniperRifle" ) {
			
			if( GetSniperCurrentAmmo() == GetSniperMaxAmmo() && GetSniperSpareAmmo() == GetSniperMaxSpare() ) {
				return true;
			} else { return false; }				
		} else { return false; }
	}
	
	public bool HasWeaponCheck( string weapon ) {
		
		if( weapon == "AssaultRifle" ) {
			
			if( weaponSlots[0] == null ) {
				return false;
			} else { return true; }
			
		} else if( weapon == "SniperRifle" ) {
			
			if( weaponSlots[2] == null ) {
				return false;	
			} else { return true; }
			
		} else { return false; }
	}
	
	public void SetWeaponCurrentAmmo( float ammo ) {
		currentWeaponScript.SetCurrentAmmo( ammo );
	}
		
	public void AddWeapon( string weapon ) {
	
		if( weapon == "SniperRifle" ) {
			weaponSlots[2] = sniperRifleScript;
			availWeapons++;	
		}	
	}	
	
	public void SetCurrentWeapon( string weapon ) {
		
		if( weapon == "AssaultRifle" ) {
			currentWeapon = 0;
			currentWeaponScript.enabled = false;
			currentWeaponScript = weaponSlots[currentWeapon];
			currentWeaponScript.enabled = true;
		} else if( weapon == "SniperRifle" ) {
			currentWeapon = 2;
			currentWeaponScript.enabled = false;
			currentWeaponScript = weaponSlots[currentWeapon];
			currentWeaponScript.enabled = true;
		}
	}		
}
