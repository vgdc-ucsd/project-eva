using UnityEngine;
using System.Collections;

public class Asteroid_Info : MonoBehaviour
{
	public float hitPoints = 50.0f;
	private float collideDamage = 50.0f;

	void Update() {
		if ( hitPoints <= 0 )
		{
			Die();
		}
	}

	public void receiveDamage( float dmgDone ) {
		hitPoints -= dmgDone;
	}
	
	void OnCollisionEnter( Collision other ) {
		GameObject hitObject = other.gameObject;
		
		if ( hitObject.tag == "Player" ) {
			hitObject.SendMessage("ReceiveDamageFromCover",collideDamage);
		}
	}
	
	private void Die() {
		Destroy( gameObject );
	}
}
