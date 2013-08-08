using UnityEngine;
using System.Collections;

public class guiGame : MonoBehaviour {

	public Texture2D crosshairImage;
	public Texture2D healthBar;
	public GUIStyle HUDStyle_large;
	public GUIStyle HUDStyle_small;
	public GUIStyle HPCountStyle;
	private PlayerBoost boostController;
	private PlayerWeapons weaponController;
	private PlayerHP healthController;
	private float currentHealth;
	private float maxHealth;
	private float currentAmmo;
	private	float spareAmmo;
	private float currWidth;
	private GameObject MenuMusic;
	private GameObject BattleMusic;
	enum Fade {In, Out};
	float fadeOutTime = 2.0f;
	float fadeInTime = 6.0f;

	protected void Awake() {
		boostController = GetComponent<PlayerBoost>();
		weaponController = GetComponent<PlayerWeapons>();
		healthController = GetComponent<PlayerHP>();
		maxHealth = healthController.GetMaxHP();
	}
	
	void Start() {
		MenuMusic = GameObject.FindGameObjectWithTag("MenuMusic");
		BattleMusic = GameObject.FindGameObjectWithTag("BattleMusic");
		
		if( !MenuMusic.Equals(null) ) {
			StartCoroutine(FadeAudio( MenuMusic, fadeOutTime, Fade.Out));
		}
		
		if( !BattleMusic.Equals(null) ) {
			StartCoroutine(FadeAudio( BattleMusic, fadeInTime, Fade.In));	
		}
	}
		
	IEnumerator FadeAudio (GameObject obj, float timer, Fade fadeType) {
		float start = fadeType == Fade.In? 0.0f : 1.0f;
		float end = fadeType == Fade.In? 1.0f : 0.0f;
		float i = 0.0f;
		float step = 1.0f/timer;
		
		while( i <= 1.0f ) {
			i += step * Time.deltaTime;
			obj.audio.volume = Mathf.Lerp (start, end, i);
			yield return new WaitForSeconds(step * Time.deltaTime);
		}
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
		
		currWidth = 300 * (currentHealth / maxHealth);
		
		GUI.Label(new Rect(0,Screen.height - 75,90,18),"Armor:",HUDStyle_small);
		GUI.Label(new Rect(140,Screen.height - 80,30,30)," " + currentHealth,HUDStyle_large);
		
		GUI.BeginGroup(new Rect(20,Screen.height-50,currWidth,35));;
		GUI.DrawTexture(new Rect(0,0,400,35),healthBar,ScaleMode.StretchToFill);
		GUI.EndGroup();
	}
}
