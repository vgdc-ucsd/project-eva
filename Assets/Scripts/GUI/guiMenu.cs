using UnityEngine;
using System.Collections;

public class guiMenu : MonoBehaviour {
	
	private GameObject logo;
	public GUIStyle myStyle;
	public GUIStyle enterIPtextStyle;
	public GUIStyle enterIPStyle;
	public string joinIP;
	private bool toggleIP = false;
	
	void Awake() {
		logo = GameObject.Find("logo");
		logo.transform.position = Vector3.zero;
		logo.transform.localScale = Vector3.zero;
		logo.guiTexture.pixelInset = new Rect( 0,Screen.height/2-50,512,256 );
	}
	
	void OnGUI() {		
		if( GUI.Button(new Rect(0,Screen.height/2+55,400,50),"Host",myStyle) ){
			Application.LoadLevel(1);
		}
		
		toggleIP = GUI.Toggle(new Rect(0,Screen.height/2+55*2,400,50),toggleIP,"Join",myStyle);
		
		if( toggleIP ) {
			GUI.Label(new Rect(560,Screen.height/2+65,200,50),"Enter the IP of the game you wish to join: ",enterIPtextStyle);
			
			joinIP = GUI.TextField(new Rect(450,Screen.height/2+55*2,425,50),joinIP,enterIPStyle);
			
			if ( GUI.Button( new Rect( 450, Screen.height / 2 + 100 * 2, 425, 50 ), "Join" ) ) {
				NetworkManager.ConnectToServer( joinIP );
			}
		}
		
		if( GUI.Button(new Rect(0,Screen.height/2+55*3,400,50),"Options",myStyle) ){
			Application.LoadLevel(3);
		}
		
		if( GUI.Button(new Rect(0,Screen.height/2+55*4,400,50),"Quit",myStyle) ){
			Application.Quit();
		}	
	}
}
