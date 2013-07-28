using UnityEngine;
using System.Collections;

public class PlayerWeapons : MonoBehaviour {
	public GameObject AssaultRifle;
	public GameObject SniperRifle;

	private ARV80_Rifle assaultRifleScript;
	private S107_SniperRifle sniperRifleScript;

	protected void Start() {
		assaultRifleScript = AssaultRifle.GetComponent<ARV80_Rifle>();
		sniperRifleScript = SniperRifle.GetComponent<S107_SniperRifle>();
	}
}
