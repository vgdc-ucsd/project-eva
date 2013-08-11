using UnityEngine;
using System.Collections;

public class ArmorPickup : MonoBehaviour {

	public float plusArmor = 50.0f;
	public float respawnTime = 30.0f;
	private PlayerHP healthController;
	private float curArmor;
	private float maxArmor;
	
	void OnTriggerEnter(Collider other) {
		GameObject hitObject = other.gameObject;
		if ( hitObject.tag == "Player" && renderer.enabled ) {
			healthController = hitObject.GetComponent<PlayerHP>();
			curArmor = healthController.GetCurrentHP();
			maxArmor = healthController.GetMaxHP();
			
			if ( curArmor < maxArmor ) {
				if ( curArmor + plusArmor >= maxArmor ) {
					healthController.SetCurrentHP( maxArmor );
				} else {
					healthController.SetCurrentHP( curArmor + plusArmor );				
				}
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
