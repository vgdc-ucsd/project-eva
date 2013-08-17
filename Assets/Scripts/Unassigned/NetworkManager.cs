using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	public GameObject playerPrefab;

	const int DEFAULT_PORT = 31337;
	const int MAX_CONNECTIONS = 16;

	void Start() {
		DontDestroyOnLoad( this );
	}

	public static void StartServer() {
		bool useNAT = !Network.HavePublicAddress();
		Network.InitializeServer( MAX_CONNECTIONS, DEFAULT_PORT, useNAT );
	}
	
	// Called on the client when they've connected to a server
	void OnConnectedToServer() {
		
	}

	// Called on the server when they've finished initializing.
	void OnServerInitialized() {
		
	}
}
