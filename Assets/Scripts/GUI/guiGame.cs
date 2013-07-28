using UnityEngine;
using System.Collections;

public class guiGame : MonoBehaviour {

	public Texture2D crosshairImage;
	private PlayerBoost boostController;
	public GUIStyle HUDStyle_large;
	public GUIStyle HUDStyle_small;

	private PlayerWeapons weaponController;
	
	protected void Awake() {
		boostController = GetComponent<PlayerBoost>();
		weaponController = GetComponent<PlayerWeapons>();
	}
		
	void OnGUI() {
		float xMin = Screen.width/2 - ( crosshairImage.width/2 );
		float yMin = Screen.height/2 - ( crosshairImage.height/2 );

		float currentAmmo = weaponController.GetWeaponCurrentAmmo();
		float spareAmmo = weaponController.GetWeaponSpareAmmo();

		GUI.DrawTexture(new Rect(xMin,yMin,crosshairImage.width,crosshairImage.height),crosshairImage);
		GUI.Label(new Rect(Screen.width - 200,Screen.height-100,200,50),"Boosts: " + boostController.currBoosts,HUDStyle_small);
		GUI.Label(new Rect(Screen.width - 200,Screen.height-50,200,50),currentAmmo + " / " + spareAmmo,HUDStyle_large);
	}
}
