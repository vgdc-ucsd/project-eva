using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class guiGame : MonoBehaviour {

	public Texture2D crosshairImage;
	public Texture2D healthBar;
	public Texture2D gameMenuBG;
	public Texture2D scoreboardBG;
	public Texture2D gui_background;
	
	public GUIStyle HUDStyle_large;
	public GUIStyle HUDStyle_small;
	public GUIStyle HPCountStyle;
	public GUIStyle GameMenuStyle;
	public GUIStyle ScoreBoardStyle;
	public GUIStyle MyScoreStyle;
	public GUIStyle RedTeamStyle;
	public GUIStyle BlueTeamStyle;
	
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
	private List<Player> redPlayers = new List<Player>();
	private List<Player> bluePlayers = new List<Player>();
	
	private bool isMenuOpen = false;
	private bool isScoreboardOpen = false;
	private bool finalScoreboardOpen = false;
	private bool hudEnabled = false;
	private bool choosingTeam = false;
	private bool switchingTeam = false;
	
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
		
		if (networkManager.gameType == 1) { // If team DM, draw team select buttons
			choosingTeam = true;
		} else { // If FFA, enter the battle immediately as team "zero" (no team selection)
			networkManager.EnterBattle(0);	
			hudEnabled = true;
		}
		UpdateAllPlayers();
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
				weaponController.EnableWeapons();
			} else { 
				isMenuOpen = true; 
				Screen.lockCursor = false;
				movementController.inMenu = true;
				weaponController.DisableWeapons();
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
		if (networkManager.gameType == 0) { // If FFA, just keep track of all the players
			allPlayers.Clear();
			allPlayers = networkManager.otherPlayers;
			allPlayers.Add(networkManager.my);
		} else { // If a team game, need to check who is on what team
			redPlayers.Clear();
			bluePlayers.Clear();
			
			foreach (Player p in networkManager.otherPlayers) {
				if (p.team == 1) {
					redPlayers.Add(p);
				} else { 
					bluePlayers.Add(p);
				}
			}
			
			if (networkManager.my.team == 1) { // Add self to the list
				redPlayers.Add(networkManager.my);
			} else {
				bluePlayers.Add(networkManager.my);
			}
		}
	}
	
	public void ToggleFinalScoreboard() {
		if (!finalScoreboardOpen) {
			finalScoreboardOpen = true;	
		} else { finalScoreboardOpen = false; }
	}
	
	public void ToggleHUD() {
		if (hudEnabled) {
			hudEnabled = false;	
		} else { 
			hudEnabled = true; 
		}			
	}
	
	void OnGUI() {		
		currentAmmo = weaponController.GetWeaponCurrentAmmo();
		spareAmmo = weaponController.GetWeaponSpareAmmo();
		currentHealth = healthController.GetCurrentHP();
		
		if( choosingTeam ) { // true at start of game only
			Screen.lockCursor = false;
			movementController.inMenu = true;
			weaponController.DisableWeapons();
			
			GUI.Label(new Rect(Screen.width/2, Screen.height/2-25,100,10),"Choose your team...",HUDStyle_small);
			GUI.DrawTexture(new Rect(Screen.width/2-320, Screen.height/2-40,640,100), gui_background, ScaleMode.StretchToFill);
			
			if( GUI.Button(new Rect(Screen.width/2-300,Screen.height/2,300,40),"Red Team",RedTeamStyle) ) {
				choosingTeam = false;
				hudEnabled = true;
				Screen.lockCursor = true;
				movementController.inMenu = false;
				weaponController.EnableWeapons();
				networkManager.EnterBattle(1); // Enter the battle in team 1 (red)
			}
			if( GUI.Button(new Rect(Screen.width/2,Screen.height/2,300,40),"Blue Team",BlueTeamStyle) ) {
				choosingTeam = false;
				hudEnabled = true;
				Screen.lockCursor = true;
				movementController.inMenu = false;
				weaponController.EnableWeapons();
				networkManager.EnterBattle(2); // Enter the battle in team 2 (blue)
			}
		}
		
		if( switchingTeam ) { // true if player selects the "switch team" option from the in-game menu
			Screen.lockCursor = false;
			movementController.inMenu = true;
			hudEnabled = false;
			weaponController.DisableWeapons();
			
			GUI.Label(new Rect(Screen.width/2, Screen.height/2-25,100,10),"Choose your team...",HUDStyle_small);
			GUI.DrawTexture(new Rect(Screen.width/2-320, Screen.height/2-40,640,100), gui_background, ScaleMode.StretchToFill);
			
			if( GUI.Button(new Rect(Screen.width/2-300,Screen.height/2,300,40),"Red Team",RedTeamStyle) ) {
				switchingTeam = false;
				hudEnabled = true;
				isMenuOpen = false;
				Screen.lockCursor = true;
				movementController.inMenu = false;
				weaponController.EnableWeapons();
				networkManager.networkView.RPC("SwitchTeam", RPCMode.All, networkManager.my.playerInfo, 1);
			}
			if( GUI.Button(new Rect(Screen.width/2,Screen.height/2,300,40),"Blue Team",BlueTeamStyle) ) {
				switchingTeam = false;
				hudEnabled = true;
				isMenuOpen = false;
				Screen.lockCursor = true;
				movementController.inMenu = false;
				weaponController.EnableWeapons();
				networkManager.networkView.RPC("SwitchTeam", RPCMode.All, networkManager.my.playerInfo, 2);
			}			
		}
		
		if( hudEnabled ) { // true after team selection is complete (in a team match) or immediately at the start of the game (if free-for-all)

			GUI.DrawTexture(new Rect(crosshair_xMin,crosshair_yMin,crosshairImage.width,crosshairImage.height),crosshairImage);
			GUI.Label(new Rect(Screen.width-200,Screen.height-100,200,50),"Boosts: " + boostController.currBoosts,HUDStyle_small);
			GUI.Label(new Rect(Screen.width-200,Screen.height-50,200,50),currentAmmo + " / " + spareAmmo,HUDStyle_large);
		
			currWidth = 300 * (currentHealth / maxHealth);
		
			GUI.Label(new Rect(0,Screen.height - 75,90,18),"Armor:",HUDStyle_small);
			GUI.Label(new Rect(140,Screen.height - 80,30,30)," " + currentHealth,HUDStyle_large);
		
			GUI.BeginGroup(new Rect(20,Screen.height-50,currWidth,35));;
			GUI.DrawTexture(new Rect(0,0,400,35),healthBar,ScaleMode.StretchToFill);
			GUI.EndGroup();
			
		}
			
		if( isMenuOpen ) { // on pressing "escape"
			if (networkManager.gameType == 0) {	// standard menu
				GUI.DrawTexture(new Rect(Screen.width-375,25,350,100),gameMenuBG,ScaleMode.StretchToFill);
			
				if( GUI.Button(new Rect(Screen.width-350,40,300,40),"Exit to Main Menu",GameMenuStyle) ) {
					NetworkManager.DisconnectFromServer();
				}
			} else { // menu with switch team option
				GUI.DrawTexture(new Rect(Screen.width-375,25,350,150),gameMenuBG,ScaleMode.StretchToFill);
			
				if( GUI.Button(new Rect(Screen.width-350,40,300,40),"Exit to Main Menu",GameMenuStyle) ) {
					NetworkManager.DisconnectFromServer();
				}
				
				if( GUI.Button(new Rect(Screen.width-350,100,300,40),"Change Team",GameMenuStyle) ) {
					switchingTeam = true;	
				}				
			}
		}
		
		if( isScoreboardOpen || finalScoreboardOpen ) { // on pressing "tab"
			
			if (networkManager.gameType == 0) { // Free-for-all scoreboard (no team separation)
			
				GUI.DrawTexture(new Rect(50,50,Screen.width-100,Screen.height-150),scoreboardBG,ScaleMode.StretchToFill);
			
				var newList = allPlayers.OrderByDescending(x => x.score).ToList();
			
				for (int i = 0; i < newList.Count; i++) {
					if (networkManager.FindPlayer( newList[i].playerInfo ) == networkManager.my) { // Highlight the name/score in yellow if it's me
						GUI.Label(new Rect(150,100 + (i+1)*50,500,20), newList[i].name + "  " + newList[i].score, MyScoreStyle);
					} else {
						GUI.Label(new Rect(150,100 + (i+1)*50,500,20), newList[i].name + "  " + newList[i].score, ScoreBoardStyle);
					}
				}
			
				if( finalScoreboardOpen ) {
					GUI.Label(new Rect(300, Screen.height-300, 500, 20), newList[0].name + " wins!", ScoreBoardStyle);				
				}
			
			} else { // Team scoreboard
			
				GUI.DrawTexture(new Rect(50,50,Screen.width-100,Screen.height-150),scoreboardBG,ScaleMode.StretchToFill);
			
				var redList = redPlayers.OrderByDescending(x => x.score).ToList();
				var blueList = bluePlayers.OrderByDescending(x => x.score).ToList();
				
				GUI.Label(new Rect(150, 100, 500, 20), "Red score: " + networkManager.redScore, ScoreBoardStyle);
				GUI.Label(new Rect(Screen.width/2+50, 100, 500, 20), "Blue score: " + networkManager.blueScore, ScoreBoardStyle);
				
				for (int r = 0; r < redList.Count; r++) { //print out red team's scores
					
					if (networkManager.FindPlayer( redList[r].playerInfo ) == networkManager.my) {
						GUI.Label(new Rect(150, 150 + (r+1)*50,500,20), redList[r].name + "  " + redList[r].score, MyScoreStyle);
					} else {
						GUI.Label(new Rect(150, 150 + (r+1)*50,500,20), redList[r].name + "  " + redList[r].score, ScoreBoardStyle);
					}					
				}
				for (int b = 0; b < redList.Count; b++) { //print out blue team's scores
					
					if (networkManager.FindPlayer( blueList[b].playerInfo ) == networkManager.my) {
						GUI.Label(new Rect(Screen.width/2+50, 150 + (b+1)*50,500,20), blueList[b].name + "  " + blueList[b].score, MyScoreStyle);
					} else {
						GUI.Label(new Rect(Screen.width/2+50, 150 + (b+1)*50,500,20), blueList[b].name + "  " + blueList[b].score, ScoreBoardStyle);
					}				
				}
				
				if( finalScoreboardOpen ) {
					if (networkManager.redScore > networkManager.blueScore) {
						GUI.Label(new Rect(300, Screen.height-300, 500, 20),"Red team wins!",ScoreBoardStyle);
					} else {
						GUI.Label(new Rect(300, Screen.height-300, 500, 20),"Blue team wins!",ScoreBoardStyle);
					}
				}
			}
		}
	}
}
