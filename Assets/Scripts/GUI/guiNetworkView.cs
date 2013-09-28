using UnityEngine;
using System.Collections;

public class guiNetworkView : MonoBehaviour {

	public bool show = true;
	public GUIStyle networkViewStyle;

	protected void OnGUI() {
		
		if( !show ) return;

		Vector2 dimensions = new Vector2( 250, 100 );
		Vector2 position = new Vector2( Screen.width - dimensions.x - 20, 20 );
		GUI.Box( new Rect( position.x, position.y, dimensions.x, dimensions.y ), "Network Status", networkViewStyle );
		GUI.Label( new Rect( position.x + 10, position.y + 15, dimensions.x, dimensions.y ), DetermineConnectivity() );
		GUI.Label( new Rect( position.x + 10, position.y + 30, dimensions.x, dimensions.y ), DetermineNAT() );
		if( Network.isServer ) {
			// Display server info
			GUI.Label( new Rect( position.x + 10, position.y + 45, dimensions.x, dimensions.y ), ShowConnections() );
		} else if(Network.isClient) {
			// Display clienty things
		}
	}

	private string DetermineConnectivity() {
		string baseString = "Connected: ";
		
		if( Network.isClient ) {
			return baseString + "as Client";
		} else if( Network.isServer ) {
			return baseString + "as Server";
		} else {
			return "Not connected";
		}
	}

	private string DetermineNAT() {
		string baseString = "Using NAT? ";
		if( !Network.HavePublicAddress() ) {
			return baseString + "Yes";
		} else {
			return baseString + "No";
		}
	}

	private string ShowConnections() {
		return "Clients: " + Network.connections.Length;
	}
}
