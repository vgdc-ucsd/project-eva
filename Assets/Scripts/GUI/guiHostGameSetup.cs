using UnityEngine;
using System;
using System.Collections;
using System.Text.RegularExpressions;

public class guiHostGameSetup : MonoBehaviour {
	
	public GUIStyle titleStyle;
	public GUIStyle subtitleStyle;
	public GUIStyle map1_buttonStyle;
	public GUIStyle hostReadyStyle;
	public GUIStyle hostNotReadyStyle;
	public GUIStyle textStyle;
		
	public Texture2D background;
	private bool mapchoice1 = false;
	private string text;
	private string[] gameTypes;
	private int gameTypeSelect;
	
	void Awake() {
		if (PlayerOptions.host_killsToWin == 0) {
			text = "5";
			PlayerOptions.host_killsToWin = 5; //default kills to win
		} else {
			text = PlayerOptions.host_killsToWin.ToString();	
		}
	}
	
	void Start() {
		gameTypes = new string[2];
		gameTypes[0] = "Free-for-all";
		gameTypes[1] = "Team Deathmatch";
		
		gameTypeSelect = PlayerOptions.host_gameType;
	}
	
	void OnGUI() {
		
		GUI.DrawTexture(new Rect( 20,20,Screen.width-150,Screen.height-150 ), background,ScaleMode.StretchToFill,true,0);
		GUI.Label(new Rect(80,20,100,100),"Host Game",titleStyle);
		GUI.Label(new Rect(100,100,100,100),"Map Select",subtitleStyle);
		GUI.Label(new Rect(120,400,100,100),"Game Options",subtitleStyle);
		GUI.Label(new Rect(100,455,100,100),"Kills to Win",textStyle);
	
		text = GUI.TextField(new Rect(200,450,50,20),text);
		text = Regex.Replace(text,@"[^0-9]", ""); //Only allow integers
		
		GUI.Label(new Rect(100,500,100,100),"Game Type",textStyle);
		gameTypeSelect = GUI.SelectionGrid(new Rect(115,525,350,20), gameTypeSelect, gameTypes, 2);
		PlayerOptions.host_gameType = gameTypeSelect;
		
		if( string.Equals(text,"0") ) { //Do not let user set value to zero
			text = "1";	
		}
		
		if( !string.IsNullOrEmpty(text) ) { //only parse and set new value if the string is not null
			PlayerOptions.host_killsToWin = Int32.Parse(text); //sets new value in PlayerOptions
		}
			
		mapchoice1 = GUI.Toggle(new Rect(90,150,200,200),mapchoice1,"",map1_buttonStyle);
		
		if( !mapchoice1 ) {
			if( GUI.Button(new Rect(Screen.width-1000,Screen.height-80,400,50),"Launch",hostNotReadyStyle) ){
				//display grayed out launch button
			}
		} else {
			if( GUI.Button(new Rect(Screen.width-1000,Screen.height-80,400,50),"Launch",hostReadyStyle) ){
				NetworkManager.StartServer();
			}
		}
		
		if ( GUI.Button(new Rect(Screen.width-500,Screen.height-80,400,50),"Back",hostReadyStyle) ) {
			Application.LoadLevel(0);
		}
	}
}
