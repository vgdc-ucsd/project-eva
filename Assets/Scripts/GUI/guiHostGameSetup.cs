using UnityEngine;
using System.Collections;

public class guiHostGameSetup : MonoBehaviour {
	
	public GUIStyle titleStyle;
	public GUIStyle subtitleStyle;
	public GUIStyle map1_buttonStyle;
	public GUIStyle hostReadyStyle;
	public GUIStyle hostNotReadyStyle;
	public Texture2D background;
	private bool mapchoice1 = false;
	
	void OnGUI () {
		
		GUI.DrawTexture(new Rect( 20,20,Screen.width-150,Screen.height-150 ), background,ScaleMode.StretchToFill,true,0);
		GUI.Label(new Rect(80,20,100,100),"Host Game",titleStyle);
		GUI.Label(new Rect(100,100,100,100),"Map Select",subtitleStyle);
		
		mapchoice1 = GUI.Toggle(new Rect(90,150,200,200),mapchoice1,"",map1_buttonStyle);
		
		if ( ! mapchoice1 ){
			//display grayed out launch button
			if( GUI.Button(new Rect(Screen.width-1000,Screen.height-80,400,50),"Launch",hostNotReadyStyle) ){
				//display message
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
