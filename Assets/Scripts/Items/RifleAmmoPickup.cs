using UnityEngine;
using System.Collections;

public class RifleAmmoPickup : MonoBehaviour {
	
	public float plusAmmo = 40.0f;
	public float respawnTime = 30.0f;
	private PlayerWeapons weaponController;
	private float curAmmo;
	
	void OnTriggerEnter(Collider other) {
		GameObject hitObject = other.gameObject;
		if ( hitObject.tag == "Player" && renderer.enabled ) {
			weaponController = hitObject.GetComponent<PlayerWeapons>();
			curAmmo = weaponController.GetWeaponCurrentAmmo();
			
			if ( ! weaponController.IsFullCheck() ) {
				weaponController.SetWeaponCurrentAmmo( curAmmo + plusAmmo );
				renderer.enabled = false;
				StartCoroutine( PickupReturn() );
			}
		}
	}
	
	IEnumerator PickupReturn() {
		yield return new WaitForSeconds( respawnTime );
		renderer.enabled = true;
	}
}
