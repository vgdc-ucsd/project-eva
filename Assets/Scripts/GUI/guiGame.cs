using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class guiGame : MonoBehaviour {

	public Texture2D crosshairImage;
	public Texture2D healthBar;
	public Texture2D gameMenuBG;
	public Texture2D scoreboardBG;
	public GUIStyle HUDStyle_large;
	public GUIStyle HUDStyle_small;
	public GUIStyle HPCountStyle;
	public GUIStyle GameMenuStyle;
	public GUIStyle ScoreBoardStyle;
	public GUIStyle MyScoreStyle;
	public string id;
	private PlayerBoost boostController;
	private PlayerWeapons weaponController;
	private PlayerHP healthController;
	private PlayerMovement movementController;
	private float currentHealth;
	private float maxHealth;
	private float currentAmmo;
	private	float spareAmmo;
	
	private float currWidth;
	private GameObject MenuMusic;
	private GameObject BattleMusic;
	private NetworkManager networkManager;
	private List<Player> allPlayers;
	
	private bool isMenuOpen = false;
	private bool isScoreboardOpen = false;
	private bool finalScoreboardOpen = false;
	private bool hudEnabled = true;
	
	enum Fade {In, Out};
	float fadeOutTime = 2.0f;
	float fadeInTime = 6.0f;
	float crosshair_xMin;
	float crosshair_yMin;

	protected void Awake() {
		if( !networkView.isMine ) {
			enabled = false;
		}
		
		if( string.IsNullOrEmpty(PlayerOptions.playerID) ) {
			id = "Jane Doe";	
		} else {
			id = PlayerOptions.playerID;	
		}
		
		boostController = GetComponent<PlayerBoost>();
		weaponController = GetComponent<PlayerWeapons>();
		healthController = GetComponent<PlayerHP>();
		movementController = GetComponent<PlayerMovement>();
		maxHealth = healthController.GetMaxHP();
	}
	
	void Start() {
		MenuMusic = GameObject.FindGameObjectWithTag("MenuMusic");
		BattleMusic = GameObject.FindGameObjectWithTag("BattleMusic");
		networkManager = GameObject.FindGameObjectWithTag( Tags.NetworkController ).GetComponent<NetworkManager>();
		
		if( !MenuMusic.Equals(null) ) {
			StartCoroutine(FadeAudio( MenuMusic, fadeOutTime, Fade.Out));
		}
		
		if( !BattleMusic.Equals(null) ) {
			StartCoroutine(FadeAudio( BattleMusic, fadeInTime, Fade.In));	
		}
				
		crosshair_xMin = Screen.width/2 - ( crosshairImage.width/2 );
		crosshair_yMin = Screen.height/2 - ( crosshairImage.height/2 );
		
		allPlayers = networkManager.otherPlayers;
		allPlayers.Add(networkManager.my);
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
	
	void Update() {
		
		if ( Input.GetButtonDown("GameMenu") ) {
			
			if( isMenuOpen ) { 
				isMenuOpen = false;
				Screen.lockCursor = true;
				movementController.inMenu = false;
			} else { 
				isMenuOpen = true; 
				Screen.lockCursor = false;
				movementController.inMenu = true;
			}
		}
		
		if ( Input.GetButtonDown("Scoreboard") && !finalScoreboardOpen ) {
		
			if( isScoreboardOpen ) { 
				isScoreboardOpen = false;
			} else { 
				isScoreboardOpen = true; 
			}
		}
	}
	
	public void UpdateAllPlayers() {
		allPlayers = networkManager.otherPlayers;
		allPlayers.Add(networkManager.my);
	}
	
	public void ToggleFinalScoreboard() {
		if (!finalScoreboardOpen) {
			finalScoreboardOpen = true;	
		} else { finalScoreboardOpen = false; }
	}
	
	public void ToggleHUD() {
		if (hudEnabled) {
			hudEnabled = false;	
		} else { hudEnabled = true; }			
	}
	
	void OnGUI() {		
		currentAmmo = weaponController.GetWeaponCurrentAmmo();
		spareAmmo = weaponController.GetWeaponSpareAmmo();
		currentHealth = healthController.GetCurrentHP();
		
		if( hudEnabled ) {

			GUI.DrawTexture(new Rect(crosshair_xMin,crosshair_yMin,crosshairImage.width,crosshairImage.height),crosshairImage);
			GUI.Label(new Rect(Screen.width-200,Screen.height-100,200,50),"Boosts: " + boostController.currBoosts,HUDStyle_small);
			GUI.Label(new Rect(Screen.width-200,Screen.height-50,200,50),currentAmmo + " / " + spareAmmo,HUDStyle_large);
			GUI.Label(new Rect(10,20,100,20),id,HUDStyle_small);
			GUI.Label(new Rect(10,40,100,20),"Score: " + networkManager.my.score,HUDStyle_small);
		
			currWidth = 300 * (currentHealth / maxHealth);
		
			GUI.Label(new Rect(0,Screen.height - 75,90,18),"Armor:",HUDStyle_small);
			GUI.Label(new Rect(140,Screen.height - 80,30,30)," " + currentHealth,HUDStyle_large);
		
			GUI.BeginGroup(new Rect(20,Screen.height-50,currWidth,35));;
			GUI.DrawTexture(new Rect(0,0,400,35),healthBar,ScaleMode.StretchToFill);
			GUI.EndGroup();
			
		}
			
		if( isMenuOpen ) {
			GUI.DrawTexture(new Rect(Screen.width-375,25,350,100),gameMenuBG,ScaleMode.StretchToFill);
			
			if( GUI.Button(new Rect(Screen.width-350,40,300,40),"Exit to Main Menu",GameMenuStyle) ) {
				NetworkManager.DisconnectFromServer();
			}
		}
		
		if( isScoreboardOpen || finalScoreboardOpen ) {
			GUI.DrawTexture(new Rect(50,50,Screen.width-100,Screen.height-150),scoreboardBG,ScaleMode.StretchToFill);
			
			var newList = allPlayers.OrderByDescending(x => x.score).ToList();
			
			for (int i = 0; i < newList.Count; i++) {
				if (networkManager.FindPlayer( newList[i].playerInfo ) == networkManager.my) {
					GUI.Label(new Rect(150,100 + (i+1)*50,500,20), newList[i].name + "  " + newList[i].score, MyScoreStyle);
				} else {
					GUI.Label(new Rect(150,100 + (i+1)*50,500,20), newList[i].name + "  " + newList[i].score, ScoreBoardStyle);
				}
			}
			
			if ( finalScoreboardOpen ) {
				GUI.Label(new Rect(300,Screen.height - 300,500,20), newList[0].name + " wins!", ScoreBoardStyle);				
			}
		}
	}
}
