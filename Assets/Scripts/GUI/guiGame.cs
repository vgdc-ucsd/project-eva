using UnityEngine;
using System.Collections;

public class guiGame : MonoBehaviour {

	public Texture2D crosshairImage;
	private PlayerBoost boost;
	private ARV80_Rifle ARV;
	public GUIStyle HUDStyle_large;
	public GUIStyle HUDStyle_small;
	
	protected void Awake() {
		boost = GetComponent<PlayerBoost>();
		ARV = GetComponent<ARV80_Rifle>();
	}
		
	void OnGUI() {
		float xMin = Screen.width/2 - ( crosshairImage.width/2 );
		float yMin = Screen.height/2 - ( crosshairImage.height/2 );
		GUI.DrawTexture(new Rect(xMin,yMin,crosshairImage.width,crosshairImage.height),crosshairImage);
		
		//PlayerBoost boost = gameObject.GetComponent<PlayerBoost>();
		//ARV80_Rifle ARVammo = gameObject.GetComponent<ARV80_Rifle>();		
		
		GUI.Label(new Rect(Screen.width - 200,Screen.height-100,200,50),"Boosts: " + boost.currBoosts,HUDStyle_small);
		GUI.Label(new Rect(Screen.width - 200,Screen.height-50,200,50),ARV.currentAmmo + " / " + ARV.currentSpareAmmo,HUDStyle_large);
	}
}
