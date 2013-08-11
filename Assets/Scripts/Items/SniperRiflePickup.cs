using UnityEngine;
using System.Collections;

public class SniperRiflePickup : MonoBehaviour {
	
	private PlayerWeapons weaponController;
	private float respawnTime = 2.0f;
	private float plusAmmo = 4.0f;
	private float curAmmo;
	
	void OnTriggerEnter(Collider other) {
		GameObject hitObject = other.gameObject;
		if( hitObject.tag == "Player" && renderer.enabled ) {
			weaponController = hitObject.GetComponent<PlayerWeapons>();

			if( ! weaponController.HasWeaponCheck("SniperRifle") ) {
				weaponController.AddWeapon("SniperRifle");
				weaponController.SetCurrentWeapon("SniperRifle");
				renderer.enabled = false;
				StartCoroutine( PickupReturn() );
			} else if ( ! weaponController.IsFullCheck("SniperRifle") ) {
				curAmmo = weaponController.GetSniperCurrentAmmo();
				weaponController.SetCurrentWeapon("SniperRifle");
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
