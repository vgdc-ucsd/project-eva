using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour {

	public float plusHealth = 50.0f;
	public float respawnTime = 5.0f;
	private PlayerHP serverHealthController;
	
	void OnTriggerEnter(Collider other) {
		GameObject hitObject = other.gameObject;
		
		if ( hitObject.tag == "Player" && renderer.enabled ) {
			NetworkPlayer plusHPtarget = other.networkView.owner;
			other.networkView.RPC("IncreaseHPfromPickup", plusHPtarget, plusHealth );
			
			renderer.enabled = false;
			StartCoroutine( PickupReturn() );	
		}
	}
	
	IEnumerator PickupReturn() {
		yield return new WaitForSeconds( respawnTime );
		renderer.enabled = true;
	}
}
