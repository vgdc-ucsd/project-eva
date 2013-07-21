using UnityEngine;
using System.Collections;

public class guiMenu : MonoBehaviour {
	
	private GameObject logo;
	private Vector2 pos = new Vector2(0,450);
	private Vector2 size = new Vector2(512,256);
	public GUIStyle myStyle;
	
	void Start() {
		logo = GameObject.Find("logo");
		logo.transform.position = Vector3.zero;
		logo.transform.localScale = Vector3.zero;
		logo.guiTexture.pixelInset = new Rect( pos.x,pos.y,size.x,size.y );
	}
	
	void OnGUI() {		
		//Create button to load demo scene
		if( GUI.Button(new Rect(0,Screen.height/2,400,50),"Launch demo scene",myStyle) ){
			Application.LoadLevel (1);
		}
		
		if( GUI.Button(new Rect(0,Screen.height/2+55,400,50),"Options",myStyle) ){
			//Go to options screen
		}
	}
}
