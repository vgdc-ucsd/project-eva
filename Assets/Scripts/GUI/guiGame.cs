using UnityEngine;
using System.Collections;

public class guiGame : MonoBehaviour {

	public Texture2D crosshairImage;
	
	void OnGUI() {
		float xMin = Screen.width/2 - ( crosshairImage.width/2 );
		float yMin = Screen.height/2 - ( crosshairImage.height/2 );
		GUI.DrawTexture(new Rect(xMin,yMin,crosshairImage.width,crosshairImage.height),crosshairImage);
		
		PlayerBoost boost = gameObject.GetComponent<PlayerBoost>();
		ARV80_Rifle ARVammo = gameObject.GetComponent<ARV80_Rifle>();
		
		GUI.Label(new Rect(Screen.width - 100,Screen.height-50,100,50),"Boosts: " + boost.currBoosts);
		GUI.Label(new Rect(25,Screen.height-50,200,50),"Ammunition: " + ARVammo.currentAmmo + " / " + ARVammo.currentSpareAmmo);
	}
}
