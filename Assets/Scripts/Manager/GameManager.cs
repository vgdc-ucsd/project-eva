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
	public GameObject healthPickupPrefab;
	public GameObject coverAsteroidPrefab;
	
	private static List<Transform> spawnPoints;
	private static GameManager instance = null;
	public static GameManager Instance {
		get { return instance; }
	}
	private NetworkManager networkManager;
	
	void Awake() {
		if( instance != null && instance != this ) {
			Destroy (this.gameObject);
			return;
		} else {
			instance = this;
		}
		DontDestroyOnLoad(gameObject);
	}
	
	void Start() {
		spawnPoints = new List<Transform>();
		networkManager = GameObject.FindGameObjectWithTag( Tags.NetworkController ).GetComponent<NetworkManager>();
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
		
		deadPlayer.SetActive(false);
		
		guiGame mainGUI = deadPlayer.GetComponent<guiGame>();
		mainGUI.ToggleHUD();
	}
	
	public void RespawnPlayer( GameObject deadPlayer ) {
		Debug.Log( "Respawning in 3 seconds..." );
		
		//Stop the player from moving
		deadPlayer.rigidbody.velocity = Vector3.zero;
		deadPlayer.rigidbody.angularVelocity = Vector3.zero;
		
		//Begin wait sequence, then respawn
		StartCoroutine( RespawnTimer( deadPlayer ) );
	}
	
	IEnumerator RespawnTimer( GameObject deadPlayer ) {
		yield return new WaitForSeconds( respawnTime );
		
		Transform spawn = spawnPoints[Random.Range( 0, spawnPoints.Count )];
		GameObject camera = GameObject.FindGameObjectWithTag( "MainCamera" );

		deadPlayer.transform.position = spawn.position;
		deadPlayer.transform.rotation = spawn.rotation;
	
		guiGame mainGUI = deadPlayer.GetComponent<guiGame>();
		mainGUI.ToggleHUD();

		networkManager.networkView.RPC("ResumeRendering", RPCMode.Others, networkManager.my.playerInfo);
		deadPlayer.SetActive(true);
		
		PlayerWeapons weaponController = deadPlayer.GetComponent<PlayerWeapons>();
		weaponController.WeaponsReset();
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
	
	public List<Transform> CollectCurrentHealthLocations() {
		List<Transform> healthPickupPoints = new List<Transform>();
		GameObject[] hpPickupObjects = GameObject.FindGameObjectsWithTag( "HealthPickup" );
			
		foreach ( GameObject loc in hpPickupObjects ) {
			healthPickupPoints.Add( loc.transform );
			Debug.Log( "HP pickup added" );
		}
		return healthPickupPoints;
	}
	
	public List<Transform> CollectCurrentCoverLocations() {
		List<Transform> coverSpawnPoints = new List<Transform>();
		GameObject[] coverObjects = GameObject.FindGameObjectsWithTag( "Cover" );
			
		foreach ( GameObject loc in coverObjects ) {
			coverSpawnPoints.Add( loc.transform );
			Debug.Log( "Cover added" );
		}
		return coverSpawnPoints;
	}	

	void OnLevelWasLoaded( int levelID ) {
		if ( IsGameplayLevel( levelID ) ) {
			spawnPoints = CollectCurrentLevelSpawns();
		}
	}
	
	public void CreateLevelObjects() {
		List<Transform> healthPickupPoints = CollectCurrentHealthLocations();
		List<Transform> coverSpawnPoints = CollectCurrentCoverLocations();
		
		foreach( Transform pickup in healthPickupPoints ) {
			Instantiate(healthPickupPrefab, pickup.position, pickup.rotation);
		}
	
		foreach( Transform cover in coverSpawnPoints ) {
			Instantiate(coverAsteroidPrefab, cover.position, cover.rotation);
		}	
	}
}
