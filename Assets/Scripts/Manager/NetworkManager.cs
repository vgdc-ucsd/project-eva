using UnityEngine;
using System.Collections.Generic;

public struct Player {
	public GameObject avatar;
	public NetworkPlayer playerInfo;
	public float playerHealth;
	public string name;
	public int score;
}

public class NetworkManager : MonoBehaviour {

	public GameObject playerPrefab;

	// NETWORK CONSTANTS
	const int DEFAULT_PORT = 31337;
	const int MAX_CONNECTIONS = 16;
	public List<Player> otherPlayers;
	public Player my;
	private int killsToWin;
	private guiGame mainGUI;
	private GameManager gameManager;
	private static NetworkManager instance = null;
	
	public static NetworkManager Instance {
		get { return instance; }
	}
	
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
		otherPlayers = new List<Player>();
		gameManager = GameObject.FindGameObjectWithTag( Tags.GameController ).GetComponent<GameManager>();
	}

	public static void StartServer() {
		bool useNAT = !Network.HavePublicAddress();
		Network.InitializeServer( MAX_CONNECTIONS, DEFAULT_PORT, useNAT );
	}

	public static void ConnectToServer(string ip) {
		Network.Connect( ip, DEFAULT_PORT );
	}

	public static void DisconnectFromServer() {
		Network.Disconnect();
	}

	public void EnterGame() {
		GameObject myAvatar = gameManager.SpawnPlayer();
		my = new Player();
		my.avatar = myAvatar;
		my.playerInfo = Network.player;

		gameManager.AssignCamera( myAvatar );	
		mainGUI = myAvatar.GetComponent<guiGame>();
		my.name = mainGUI.id;

		// Tell other players we've connected
		networkView.RPC( "GetNewPlayerState", RPCMode.Others, my.playerInfo, myAvatar.networkView.viewID, myAvatar.transform.position, myAvatar.transform.rotation );

		// Request each other player's state at the time of connection
		networkView.RPC( "RequestIntialPlayerState", RPCMode.OthersBuffered, Network.player );
	}
	
	public Player FindPlayer( NetworkPlayer player ) {
		if( player == my.playerInfo ) { 
			return my; 
		} else {
			return otherPlayers.Find( ( x => x.playerInfo == player ) );
		}
	}

	/////////////////////////
	//	EVENTS
	/////////////////////////

	// Called when the server goes up
	void OnServerInitialized() {
		Debug.Log( "Server Initialized" );
		Network.SetSendingEnabled(0,false);
		Network.isMessageQueueRunning = false;
		Network.SetLevelPrefix ( 2 );
		Application.LoadLevel( 2 );
		
		killsToWin = PlayerOptions.host_killsToWin;
		
		Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(0,true);
	}

	// Called when a player connects (server side)
	void OnPlayerConnected( NetworkPlayer playerInfo ) {
		//Tell the new player the kill limit
		networkView.RPC("SpecifyKillLimit", playerInfo, killsToWin);
	}

	// Called when the player connects (client side)
	void OnConnectedToServer() {	
		Network.SetSendingEnabled(0,false);
		Network.isMessageQueueRunning = false;
		Network.SetLevelPrefix( 2 );
		Application.LoadLevel( 2 );
		Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(0,true);
	}

	// Called when a player disconnects (server side)
	void OnPlayerDisconnected( NetworkPlayer player ) {
		NetworkViewID id = otherPlayers.Find( ( x => x.playerInfo == player ) ).avatar.networkView.viewID;
		Network.RemoveRPCs( player );
		Network.DestroyPlayerObjects( player ); //added to test if this changes "lingering"
		networkView.RPC("RemoveObject", RPCMode.Others, id );
	}
	
	// Called when we disconnect (client side)
	void OnDisconnectedFromServer( NetworkDisconnection info ) {
		if ( info == NetworkDisconnection.LostConnection ) {
		}
		otherPlayers.Clear();
		Network.RemoveRPCs(networkView.viewID);
		Network.Destroy(networkView.viewID);
		Application.LoadLevel( Levels.Main );
	}

	void OnLevelWasLoaded( int levelID ) {
		if ( gameManager.IsGameplayLevel( levelID ) ) {
			gameManager.CreateLevelObjects();
			EnterGame();
		}
	}

	/////////////////////////
	//	RPCS
	/////////////////////////
	[RPC]
	void RequestIntialPlayerState( NetworkPlayer requester ) {
		networkView.RPC( "GetCurrentPlayerState", requester, my.playerInfo, my.avatar.networkView.viewID, my.avatar.transform.position, my.avatar.transform.rotation );
	}

	[RPC]
	void GetNewPlayerState( NetworkPlayer playerInfo, NetworkViewID avatarID, Vector3 initialPosition, Quaternion initialRotation ) {
		Player newPlayer = new Player();
		newPlayer.playerInfo = playerInfo;
		GameObject playerAvatar = NetworkView.Find( avatarID ).gameObject;
		newPlayer.avatar = playerAvatar;
		otherPlayers.Add( newPlayer );
	}

	[RPC]
	void GetCurrentPlayerState( NetworkPlayer playerInfo, NetworkViewID avatarID, Vector3 initialPosition, Quaternion initialRotation ) {
		Player newPlayer = new Player();
		newPlayer.playerInfo = playerInfo;
		GameObject playerAvatar = gameManager.SpawnPlayer( initialPosition, initialRotation );
		playerAvatar.networkView.viewID = avatarID;
		newPlayer.avatar = playerAvatar;
		otherPlayers.Add( newPlayer );
	}

	[RPC]
	void RemoveObject( NetworkViewID id ) {
		Destroy( NetworkView.Find( id ).gameObject );
		Debug.Log( "Object with NetworkViewId " + id.ToString() + " removed" );
	}
	
	[RPC]
	void StopRendering( NetworkPlayer deadplayer ) {
		Player dead = FindPlayer( deadplayer );
		dead.avatar.SetActive(false);
	}
	
	[RPC]
	void ResumeRendering( NetworkPlayer respawnedPlayer ) {
		Player alive = FindPlayer( respawnedPlayer );
		alive.avatar.SetActive(true);
	}
	
	[RPC]
	void ReportDeath( NetworkPlayer deadPlayer, NetworkPlayer killer ) {
		Player dead = FindPlayer( deadPlayer );
		
		// if killer is same as dead player (ie. suicide), then reduce dead player's score by 1
		if (deadPlayer == killer) { 
			dead.score--; 
		} else { 
			Player killerPlayer = FindPlayer( killer );
			killerPlayer.score++;	
			
			if( killerPlayer.score >= killsToWin ) {
				Debug.Log(killerPlayer.name + " wins!");
			}
		}
	}
	
	[RPC]
	void SpecifyKillLimit( int limit ) {
		killsToWin = limit;
	}
}
