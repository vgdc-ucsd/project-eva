using UnityEngine;
using System.Collections;

public class guiGame : MonoBehaviour {

	public Texture2D crosshairImage;
	public Texture2D healthBar;
	public GUIStyle HUDStyle_large;
	public GUIStyle HUDStyle_small;
	private PlayerBoost boostController;
	private PlayerWeapons weaponController;
	private PlayerHP healthController;
	private float currentHealth;
	private float maxHealth;
	private float currentAmmo;
	private	float spareAmmo;
	private float currWidth;

	protected void Awake() {
		boostController = GetComponent<PlayerBoost>();
		weaponController = GetComponent<PlayerWeapons>();
		healthController = GetComponent<PlayerHP>();
		maxHealth = healthController.GetMaxHP();
	}
		
	void OnGUI() {
		float crosshair_xMin = Screen.width/2 - ( crosshairImage.width/2 );
		float crosshair_yMin = Screen.height/2 - ( crosshairImage.height/2 );

		currentAmmo = weaponController.GetWeaponCurrentAmmo();
		spareAmmo = weaponController.GetWeaponSpareAmmo();
		currentHealth = healthController.GetCurrentHP();

		GUI.DrawTexture(new Rect(crosshair_xMin,crosshair_yMin,crosshairImage.width,crosshairImage.height),crosshairImage);
		GUI.Label(new Rect(Screen.width - 200,Screen.height-100,200,50),"Boosts: " + boostController.currBoosts,HUDStyle_small);
		GUI.Label(new Rect(Screen.width - 200,Screen.height-50,200,50),currentAmmo + " / " + spareAmmo,HUDStyle_large);
		
		currWidth = 400 * (currentHealth / maxHealth);
		
		GUI.Label(new Rect(0,Screen.height-75,165,18),"Armor Integrity:",HUDStyle_small);
		
		GUI.BeginGroup(new Rect(20,Screen.height-50,currWidth,35));;
		GUI.DrawTexture(new Rect(0,0,400,35),healthBar,ScaleMode.StretchToFill);
		GUI.EndGroup();
	}
}
