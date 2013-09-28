using UnityEngine;
using System.Collections;

public class Asteroid_Info : MonoBehaviour
{
	public float hitPoints = 50.0f;
	private float collideDamage = 5.0f;

	void Update() {
		if ( hitPoints <= 0 ) {
			Die();
		}
	}

	public void receiveDamage( float dmgDone ) {
		//hitPoints -= dmgDone;
		//Design decision: asteroids will be invulnerable
	}
	
	void OnCollisionEnter( Collision other ) {
		GameObject hitObject = other.gameObject;
		
		if ( hitObject.tag == "Player" ) {
			hitObject.networkView.RPC("InflictDamageFromCover",hitObject.networkView.owner,collideDamage);
		}
	}
	
	private void Die() {
		Destroy( gameObject );
	}
}
