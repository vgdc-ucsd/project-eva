using UnityEngine;
using System.Collections;

public class guiOptions : MonoBehaviour {
	
	public GUIStyle titleStyle;
	public GUIStyle buttonStyle;
	public Texture2D background;
	
	void OnGUI () {
		
		GUI.DrawTexture(new Rect( 20,20,Screen.width-150,Screen.height-150 ), background,ScaleMode.StretchToFill,true,0);
		GUI.Label(new Rect(100,20,100,100),"Options",titleStyle);
		
		if ( GUI.Button(new Rect(Screen.width-500,Screen.height-80,400,50),"Back",buttonStyle) ) {
			Application.LoadLevel(0);
		}		
	}
}
