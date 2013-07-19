using UnityEngine;
using System.Collections;

public class WeaponDebris_Selfdestruct : MonoBehaviour {

	public float destroyTimer = 1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		destroyTimer -= Time.deltaTime;

		if ( destroyTimer <= 0 ) {
			Destroy( gameObject );
		}
	}
}
