using UnityEngine;
using System.Collections;

public class guiMenu : MonoBehaviour {

	void OnGUI() {
		//Background box
		GUI.Box (new Rect(Screen.width/2-250,Screen.height/2-50,500,90),"Welcome to Project EVA!");
		
		//Create button to load demo scene
		if(GUI.Button (new Rect(Screen.width/2-100,Screen.height/2-25,200,25),"Load demo scene")) {
			Application.LoadLevel (1);
		}
	}
}
