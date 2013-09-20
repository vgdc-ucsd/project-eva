using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour {

	private static bool _inGame = false;
	public static bool inGame {
		get { return _inGame; }
	}
	public float respawnTime = 3.0f;

	public GameObject playerPrefab;
	private static List<Transform> spawnPoints;

	void Start() {
		DontDestroyOnLoad( this );
		spawnPoints = new List<Transform>();
	}

	// Instantiate a player at a random spawn point
	public GameObject SpawnPlayer() {
		Debug.Log( "Spawning new player" );

		Transform spawn = spawnPoints[Random.Range( 0, spawnPoints.Count )];
		GameObject playerObject = ( GameObject )Network.Instantiate( playerPrefab, spawn.position, spawn.rotation, 0 );
		GameObject camera = GameObject.FindGameObjectWithTag( "MainCamera" );
		
		FollowCamera cameraScript = camera.GetComponent<FollowCamera>();
		cameraScript.target = playerObject;
		cameraScript.enabled = true;

		return playerObject;
	}
	
	// Kill a player
	public void KillPlayer( GameObject deadPlayer ) {
		Debug.Log( "Killing player" );		
		GameObject camera = GameObject.FindGameObjectWithTag( "MainCamera" );
		
		FollowCamera cameraScript = camera.GetComponent<FollowCamera>();
		cameraScript.enabled = false;
		
		deadPlayer.SetActive(false);
		
		guiGame mainGUI = deadPlayer.GetComponent<guiGame>();
		mainGUI.enabled = false;
	}
	
	public void RespawnPlayer( GameObject deadPlayer ) {
		Debug.Log( "Respawning in 3 seconds..." );
		
		//Stop the player from moving
		deadPlayer.rigidbody.velocity = Vector3.zero;
		
		//Begin wait sequence, then respawn
		StartCoroutine( RespawnTimer( deadPlayer ) );
	}
	
	IEnumerator RespawnTimer( GameObject deadPlayer ) {
		yield return new WaitForSeconds( respawnTime );
		
		Transform spawn = spawnPoints[Random.Range( 0, spawnPoints.Count )];
		GameObject camera = GameObject.FindGameObjectWithTag( "MainCamera" );

		deadPlayer.transform.position = spawn.position;
		deadPlayer.transform.rotation = spawn.rotation;
		
		FollowCamera cameraScript = camera.GetComponent<FollowCamera>();
		cameraScript.enabled = true;
		
		deadPlayer.SetActive(true);
		
		guiGame mainGUI = deadPlayer.GetComponent<guiGame>();
		mainGUI.enabled = true;	
	}

	public GameObject SpawnPlayer( Vector3 position, Quaternion rotation ) {
		Debug.Log( "Spawning player at position " + position.ToString() + rotation.ToString() );

		GameObject playerObject = ( GameObject )Instantiate( playerPrefab, position, rotation );
		return playerObject;
	}

	public void AssignCamera( GameObject target ) {
		Debug.Log( "Targeting " + target.ToString() + " with main camera" );
		FollowCamera camera = GameObject.FindGameObjectWithTag( Tags.MainCamera ).GetComponent<FollowCamera>();
		camera.target = target;
	}

	// Determine if the level we just loaded is a gameplay level
	public bool IsGameplayLevel( int levelID ) {
		switch ( levelID ) {
			// ADD MORE LEVELS AS WE MAKE THEM			
			case 2: return true;

			default: return false;
		}
	}
	
	public List<Transform> CollectCurrentLevelSpawns() {
		List<Transform> spawnPoints = new List<Transform>();
		GameObject[] levelSpawns = GameObject.FindGameObjectsWithTag( "Respawn" );
		
		foreach ( GameObject spawnPoint in levelSpawns ) {
			spawnPoints.Add( spawnPoint.transform );
			Debug.Log( "Spawn Added" );
		}
		return spawnPoints;
	}

	void OnLevelWasLoaded( int levelID ) {
		if ( IsGameplayLevel( levelID ) ) {
			spawnPoints = CollectCurrentLevelSpawns();
		}
	}
}
