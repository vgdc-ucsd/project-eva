using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	private static bool _inGame = false;
	public static bool inGame {
		get { return _inGame; }
	}

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
		GameObject playerObject = (GameObject) Network.Instantiate( playerPrefab, spawn.position, spawn.rotation, 0 );
		GameObject camera = GameObject.FindGameObjectWithTag( "MainCamera" );
		FollowCamera cameraScript = camera.GetComponent<FollowCamera>();
		cameraScript.target = playerObject;
		cameraScript.enabled = true;

		return playerObject;
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
