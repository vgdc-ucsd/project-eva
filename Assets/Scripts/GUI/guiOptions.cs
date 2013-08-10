using UnityEngine;
using System.Collections;

public class guiOptions : MonoBehaviour {
	
	public GUIStyle titleStyle;
	public GUIStyle	subtitleStyle;
	public GUIStyle buttonStyle;
	public Texture2D background;
	public string pID;
	
	void Awake() {
		if ( ! string.IsNullOrEmpty(PlayerOptions.playerID) ) {
			pID = PlayerOptions.playerID;
		}
	}
	
	void OnGUI() {
		GUI.DrawTexture(new Rect( 20,20,Screen.width-150,Screen.height-150 ), background,ScaleMode.StretchToFill,true,0);
		GUI.Label(new Rect(20,20,100,100),"Options",titleStyle);
		GUI.Label(new Rect(50,100,100,100),"Player ID: ",subtitleStyle);
		
		pID = GUI.TextField(new Rect(160,100,100,20),pID);
		PlayerOptions.playerID = pID;
		
		if ( GUI.Button(new Rect(Screen.width-500,Screen.height-80,400,50),"Back",buttonStyle) ) {
			Application.LoadLevel(0);
		}		
	}
}
