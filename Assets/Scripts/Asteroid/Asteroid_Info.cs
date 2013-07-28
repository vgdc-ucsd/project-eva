using UnityEngine;
using System.Collections;

public class Asteroid_Info : MonoBehaviour
{
	public float hitPoints = 50.0f;
	private float collideDamage = 10.0f;

	void Update() {
		if ( hitPoints <= 0 )
		{
			die();
		}
	}

	public void receiveDamage( float damage ) {
		hitPoints -= damage;
	}
	
	void OnCollisionEnter(Collision other) {
		GameObject hitObject = other.gameObject;
		
		if ( hitObject.tag == "Player" ) {
			hitObject.SendMessage("receiveDamage", collideDamage );
		}
		
	}
	
	private void die() {
		Destroy( gameObject );
	}
}
