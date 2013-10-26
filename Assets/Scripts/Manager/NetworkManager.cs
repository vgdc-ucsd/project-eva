using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player {
	public GameObject avatar;
	public NetworkPlayer playerInfo;
	public float playerHealth;
	public string name;
	public int score;
	public int team;
}

public class NetworkManager : MonoBehaviour {

	public GameObject playerPrefab;

	// NETWORK CONSTANTS
	const int DEFAULT_PORT = 31337;
	const int MAX_CONNECTIONS = 16;
	public List<Player> otherPlayers;
	public Player my;
	public int gameType; // 0 = FFA, 1 = Team DM
	public int redScore;
	public int blueScore;
	
	private int killsToWin;
	private guiGame mainGUI;
	private GameManager gameManager;
	private PlayerWeapons weaponController;
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
		weaponController = myAvatar.GetComponent<PlayerWeapons>();
		
		my.name = mainGUI.id;
	}
	
	public void EnterBattle(int team) {
		my.team = team; // If team is zero, the value in the Player class is still assigned, but not used
		// Team 1 = "Red", Team 2 = "Blue" (placeholders)
		
		// Tell other players we've connected
		networkView.RPC( "GetNewPlayerState", RPCMode.Others, my.playerInfo, my.name, my.team, my.avatar.networkView.viewID, my.avatar.transform.position, my.avatar.transform.rotation );

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
	
	public Player FindPlayerByViewID( NetworkViewID viewID ) {
		if ( viewID == my.avatar.networkView.viewID ) {
			return my;	
		} else {
			return otherPlayers.Find ( (x => x.avatar.networkView.viewID == viewID ) );	
		}
	}
	
	public int FindPlayerIndex( NetworkViewID viewID ) { // cannot find yourself
		return otherPlayers.FindIndex( (x => x.avatar.networkView.viewID == viewID ) );
	}
	
	IEnumerator RestartMatch() {
		yield return new WaitForSeconds(10);	
		
		//kill all players
		gameManager.KillPlayer( my.avatar );
				
		for (int i = 0; i < otherPlayers.Count; i++) {
			gameManager.KillPlayer( otherPlayers[i].avatar );	
		}
		
		//close final scoreboard
		mainGUI.ToggleFinalScoreboard();
		
		//reset everything and respawn
		my.playerHealth = 100;
		my.score = 0;
		gameManager.RespawnPlayer( my.avatar );
		weaponController.WeaponsReset();
		
		for (int i = 0; i < otherPlayers.Count; i++) {
			otherPlayers[i].playerHealth = 100;
			otherPlayers[i].score = 0;
			
			if (gameType == 1) {
				redScore = 0;
				blueScore = 0;
			}
			
			gameManager.RespawnPlayer( otherPlayers[i].avatar );	
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
		gameType = PlayerOptions.host_gameType;
		
		Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(0,true);
	}

	// Called when a player connects (server side)
	void OnPlayerConnected( NetworkPlayer playerInfo ) {
		//Tell the new player the kill limit and gametype
		networkView.RPC("SpecifyGameOptions", playerInfo, killsToWin, gameType);
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
		Player disconnectedPlayer = FindPlayer( player );
		otherPlayers.Remove( disconnectedPlayer );
		mainGUI.UpdateAllPlayers();
		
		Network.RemoveRPCs( player );
		Network.DestroyPlayerObjects( player ); //added to test if this changes "lingering"
		networkView.RPC("RemoveObject", RPCMode.Others, player );
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
		networkView.RPC( "GetCurrentPlayerState", requester, my.playerInfo, my.name, my.team, my.avatar.networkView.viewID, my.avatar.transform.position, my.avatar.transform.rotation );
	}

	[RPC]
	void GetNewPlayerState( NetworkPlayer playerInfo, string playerName, int playerTeam, NetworkViewID avatarID, Vector3 initialPosition, Quaternion initialRotation ) {
		Player newPlayer = new Player();
		newPlayer.playerInfo = playerInfo;
		newPlayer.name = playerName;
		GameObject playerAvatar = NetworkView.Find( avatarID ).gameObject;
		newPlayer.avatar = playerAvatar;
		newPlayer.team = playerTeam;
		
		if (gameType == 1) { // If team DM, assign teams at start of game
			if (newPlayer.team != my.team) {
				newPlayer.avatar.tag = "Player_enemy";
			} else {
				newPlayer.avatar.tag = "Player_ally";
			}
		} else if (gameType == 0) { // If FFA, all other players assigned as enemies
			newPlayer.avatar.tag = 	"Player_enemy";
		}
		
		otherPlayers.Add( newPlayer );
		mainGUI.UpdateAllPlayers();
	}

	[RPC]
	void GetCurrentPlayerState( NetworkPlayer playerInfo, string playerName, int playerTeam, NetworkViewID avatarID, Vector3 initialPosition, Quaternion initialRotation ) {
		Player newPlayer = new Player();
		newPlayer.playerInfo = playerInfo;
		newPlayer.name = playerName;
		GameObject playerAvatar = gameManager.SpawnPlayer( initialPosition, initialRotation );
		playerAvatar.networkView.viewID = avatarID;
		newPlayer.avatar = playerAvatar;
		newPlayer.team = playerTeam;
		
		if (gameType == 1) { // If team DM, assign teams at start of game
			if (newPlayer.team != my.team) {
				newPlayer.avatar.tag = "Player_enemy";
			} else {
				newPlayer.avatar.tag = "Player_ally";
			}
		} else if (gameType == 0) { // If FFA, all other players assigned as enemies
			newPlayer.avatar.tag = 	"Player_enemy";
		}
			
		otherPlayers.Add( newPlayer );
		mainGUI.UpdateAllPlayers();
	}

	[RPC]
	void RemoveObject( NetworkPlayer player ) {
		Player disconnectedPlayer = FindPlayer( player );
		otherPlayers.Remove( disconnectedPlayer );
		
		NetworkViewID id = disconnectedPlayer.avatar.networkView.viewID;
		mainGUI.UpdateAllPlayers();
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
	void SwitchTeam( NetworkPlayer switchingPlayer, int newTeam ) {
		Player switcher = FindPlayer( switchingPlayer );
		
		if (newTeam != switcher.team) {
			switcher.team = newTeam;

			
			mainGUI.UpdateAllPlayers();
		}
	}
	
	[RPC]
	void ReportDeath( NetworkViewID deadPlayerID, NetworkViewID killerID ) {
		Player deadPlayer = FindPlayerByViewID( deadPlayerID );
		Player killerPlayer = FindPlayerByViewID( killerID );
				
		// if killer is same as dead player (ie. suicide), then reduce dead player's score by 1
		if (deadPlayerID == killerID) { 
			deadPlayer.score--;
			
			if (gameType == 1) { // decrement team score if it's a team match
				if (deadPlayer.team == 1) {
					redScore--;
				} else {
					blueScore--;
				}
			}
			
		} else { // it's a normal kill, not a suicide
			
			killerPlayer.score++; // increase killer's score by one
			
			if (gameType == 1) { // increment/decrement team scores if it's a team match
				if (killerPlayer.team == 1) {
					redScore++;
				} else {
					blueScore++;
				}	
				
				if (redScore >= killsToWin || blueScore >= killsToWin) { // Check if a team won, if yes, end and restart the game
					Debug.Log ("Team " + killerPlayer.team + " won!");
					
					//open final scoreboard
					mainGUI.ToggleFinalScoreboard();
				
					//restart the level, respawn players
					StartCoroutine( RestartMatch() );					
				}
			} else {

				if( killerPlayer.score >= killsToWin ) {
					Debug.Log(killerPlayer.name + " won!");
				
					//open final scoreboard
					mainGUI.ToggleFinalScoreboard();
				
					//restart the level, respawn players
					StartCoroutine( RestartMatch() );
				}
			}
		}
	}
	
	[RPC]
	void SpecifyGameOptions( int limit, int type ) {
		killsToWin = limit;
		gameType = type;
	}
}
