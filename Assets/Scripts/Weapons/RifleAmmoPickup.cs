using UnityEngine;
using System.Collections;

public class RifleAmmoPickup : MonoBehaviour {
	
	public float plusAmmo = 40;
	private PlayerWeapons weaponController;
	private float curAmmo;
	
	void OnTriggerEnter(Collider other) {
		GameObject hitObject = other.gameObject;
		if ( hitObject.tag == "Player" ) {
			weaponController = hitObject.GetComponent<PlayerWeapons>();
			curAmmo = weaponController.GetWeaponCurrentAmmo();
			weaponController.SetWeaponCurrentAmmo( curAmmo + plusAmmo );
			Destroy(gameObject);
		}
	}
}
