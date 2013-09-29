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
		
	// RPC functions
	[RPC]
	void IncreaseHPfromPickup( float plusHealth ) {
		float curHP = GetCurrentHP();
		if ( curHP < maxHealth ) {
			if ( curHP + plusHealth >= maxHealth ) {
				networkManager.my.playerHealth = maxHealth;
			} else {
				networkManager.my.playerHealth = curHP + plusHealth;				
			}
		}
	}
	
	[RPC]
	void InflictDamage( float damage, NetworkViewID attackerID ) {
		networkManager.my.playerHealth -= damage;
		Player attackerPlayer = networkManager.FindPlayerByViewID( attackerID );
		Debug.Log("Shot by " + attackerPlayer.name);
		
		if (networkManager.my.playerHealth <= 0) {
			Debug.Log("I died. Respawning...!");
			
			networkManager.networkView.RPC("StopRendering", RPCMode.Others, networkManager.my.playerInfo);
			networkManager.networkView.RPC("ReportDeath", RPCMode.All, networkManager.my.avatar.networkView.viewID, attackerID);	

			gameManager.KillPlayer( networkManager.my.avatar );
			networkManager.my.playerHealth = maxHealth;
			gameManager.RespawnPlayer( networkManager.my.avatar );
		}
	}
	
	[RPC]
	void InflictDamageFromCover( float damage ) {
		Debug.Log("I'm taking " + damage + " damage");
		networkManager.my.playerHealth -= damage;	
		Debug.Log("Hit by cover! My health is " + networkManager.my.playerHealth );
		
		if( networkManager.my.playerHealth <= 0 ) {
			Debug.Log("I suicided. Respawning...!");
			gameManager.KillPlayer( networkManager.my.avatar );
			networkManager.my.playerHealth = maxHealth;
			gameManager.RespawnPlayer( networkManager.my.avatar );
			
			networkManager.networkView.RPC( "ReportDeath", RPCMode.All, networkManager.my.playerInfo, networkManager.my.playerInfo );
		}		
	}
}
