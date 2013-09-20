using UnityEngine;
using System.Collections;

public class PlayerHP : MonoBehaviour {
	
	public float maxHealth = 100.0f;
	private GameManager gameManager;
	private NetworkManager networkManager;
	
	void Start() {
		gameManager = GameObject.FindGameObjectWithTag( Tags.GameController ).GetComponent<GameManager>();
		networkManager = GameObject.FindGameObjectWithTag( Tags.NetworkController ).GetComponent<NetworkManager>();
		networkManager.my.playerHealth = maxHealth;
	}
	
	public float GetCurrentHP() {
		return networkManager.my.playerHealth;
	}
	
	public float GetMaxHP() {
		return maxHealth;
	}
	
	public void SetCurrentHP( float newArmor ) {
		networkManager.my.playerHealth = newArmor;
	}
	
	public void ReceiveDamageFromCover( float damage ) {
		Debug.Log("I'm taking " + damage + " damage");
		networkManager.my.playerHealth -= damage;	
		Debug.Log("Hit by cover! My health is " + networkManager.my.playerHealth );
		
		if( networkManager.my.playerHealth <= 0 ) {
			Debug.Log("I suicided. Respawning...!");
			networkView.RPC( "ReportDeath", RPCMode.All, networkManager.my.playerInfo, networkManager.my.playerInfo );
			
			//Return to full health after respawn
			networkManager.my.playerHealth = maxHealth;
		}
	}
		
	// RPC functions
	//This gets called only on the player who was hit
	[RPC]
	void InflictDamage( float damage, NetworkPlayer playerInfo ) {
		networkManager.my.playerHealth -= damage;
		Debug.Log("I got shot! My health is " + networkManager.my.playerHealth);
		
		if (networkManager.my.playerHealth <= 0) {
			Debug.Log("I died. Respawning...!");
			networkView.RPC("ReportDeath",RPCMode.All, networkManager.my.playerInfo, playerInfo);
			
			//Return to full health after respawn
			networkManager.my.playerHealth = maxHealth;
		}
	}
	
	[RPC]
	void ReportDeath( NetworkPlayer deadPlayer, NetworkPlayer killer ) {
		Player dead = networkManager.FindPlayer( deadPlayer );
		gameManager.KillPlayer( dead.avatar );
		gameManager.RespawnPlayer( dead.avatar );
		
		// if killer is same as dead player (ie. suicide), then reduce dead player's score by 1
		if (deadPlayer == killer) { 
			dead.score--; 
		} else { 
			Player killerPlayer = networkManager.FindPlayer( killer );
			killerPlayer.score++;	
		}
	}
}
