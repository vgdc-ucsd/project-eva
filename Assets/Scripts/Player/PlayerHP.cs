using UnityEngine;
using System.Collections;

public class PlayerHP : MonoBehaviour {
	
	public float playerHealth;
	public float maxHealth = 100.0f;
	
	public float GetCurrentHP() {
		return playerHealth;
	}
	
	public float GetMaxHP() {
		return maxHealth;
	}
	
	public void SetCurrentHP( float newArmor ) {
		playerHealth = newArmor;
	}
	
	void Start() {
		playerHealth = maxHealth;
	}
	
	void Update() {
		if ( playerHealth <= 0 ) {
			Die();
		}
	}

	public void receiveDamage( float damage ) {
		playerHealth -= damage;
	}

	private void Die(){
		Debug.Log ("OH SHIT. You died.");
		Screen.lockCursor = false;
		Application.LoadLevel(0);
	}
}
