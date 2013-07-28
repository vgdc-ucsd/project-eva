using UnityEngine;
using System.Collections;

public class PlayerWeapons : MonoBehaviour {
	public GameObject AssaultRifle;
	public GameObject SniperRifle;

	private ARV80_Rifle assaultRifleScript;
	private S107_SniperRifle sniperRifleScript;

	private Weapon_Stats currentWeapon;

	protected void Start() {
		assaultRifleScript = AssaultRifle.GetComponent<ARV80_Rifle>();
		sniperRifleScript = SniperRifle.GetComponent<S107_SniperRifle>();
		currentWeapon = assaultRifleScript;
		currentWeapon.enabled = true;
	}

	public float GetWeaponCurrentAmmo() {
		return currentWeapon.GetCurrentAmmo();
	}

	public float GetWeaponSpareAmmo() {
		return currentWeapon.GetSpareAmmo();
	}
}
