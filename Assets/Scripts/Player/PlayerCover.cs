using UnityEngine;
using System.Collections;

public class PlayerCover : MonoBehaviour {

	//This value is public knowledge
	public bool coverEngaged = false;

	//The max distance you can be from the object, the defaults are just guesses so far
	private float minGrappleDistance = 3f;

	private RaycastHit hit; //hit will contain the location of the hit
	private Vector3 targetCenter;
	private Vector3 hitNorm;
	private Vector3 adjustedVelocity;
	private Ray ray; //ray will be the ray sent out from the center of the screen
	private PlayerEffects effectsController;
	private float radius;
	private float posDiff;
	private Plane tangentPlane;

	// Use this for initialization
	void Start() {
		//cover not engaged by default
		coverEngaged = false;
		effectsController = GetComponent<PlayerEffects>();
	}
	
	// Update is called once per frame
	void Update() {
		if ( Input.GetKeyDown( KeyCode.C ) ) {
			
			//send out a ray
			ray = Camera.main.ScreenPointToRay( new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0 ) );

			if ( !coverEngaged ) {
				//check for a hit
				if ( Physics.Raycast( ray, out hit ) ) {
					radius = Vector3.Distance(transform.position,hit.transform.position);
					
					if( radius <= minGrappleDistance ) {
						coverEngaged = true;
						targetCenter = hit.transform.position;
						effectsController.StartGrapple( targetCenter );
					}
				}
			} else {
				coverEngaged = false;
				effectsController.EndGrapple();
			}
		}
	}
	
	void FixedUpdate() {
		if( coverEngaged ) {
			
			//Get normal vector
			Vector3 distVec = Vector3.Normalize(targetCenter - transform.position);
			
			//Find distance from cover
			posDiff = Mathf.Pow(targetCenter.x - transform.position.x,2) + Mathf.Pow(targetCenter.y - transform.position.y,2) + Mathf.Pow(targetCenter.z - transform.position.z,2);

			//Check if player is outside the sphere
			if( posDiff > Mathf.Pow( radius,2 ) ) {
				
				//Project vector onto plane
				adjustedVelocity = rigidbody.velocity - (Vector3.Dot(rigidbody.velocity,distVec) * distVec);
				rigidbody.velocity = adjustedVelocity;
			}
		}
	}
}
