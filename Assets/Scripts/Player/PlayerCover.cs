using UnityEngine;
using System.Collections;

public class PlayerCover : MonoBehaviour {

	//This value is public knowledge
	public bool coverEngaged;

	//The min and max distance you can be from the object
	//The defaults are just guesses so far
	public float minRadius = 5.0f;
	public float maxRadius = 20.0f;


	private RaycastHit hit; //hit will contain the location of the hit
	private Ray ray; //ray will be the ray sent out from the center of the screen

	// Use this for initialization
	void Start() {
		//cover not engaged by default
		coverEngaged = false;
	}
	
	// Update is called once per frame
	void Update() {
		if ( Input.GetKeyDown( KeyCode.C ) ) {
			
			//send out a ray
			ray = Camera.main.ScreenPointToRay( new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0 ) );
			
			//check for a hit
			if ( Physics.Raycast( ray, out hit, maxRadius ) ) {
				Debug.DrawLine( ray.origin, hit.point );
				print("You hit somnething!");

				//TODO: Add a check to see if the object that we are in range of
				// is something we are allowed to lach on to as cover.
				// probably add an on ray collision or something like that

				coverEngaged = !coverEngaged;
			}
		}
	}
}
