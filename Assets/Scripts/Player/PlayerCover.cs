using UnityEngine;
using System.Collections;

public class PlayerCover : MonoBehaviour {

	//This value is public knowledge
	public bool coverEngaged = false;

	//The max distance you can be from the object
	//The defaults are just guesses so far
	public float maxRadius = 20.0f;


	private RaycastHit hit; //hit will contain the location of the hit
	private Vector3 targetCenter;
	private Ray ray; //ray will be the ray sent out from the center of the screen
	private SpringJoint attachJoint;

	private PlayerEffects effectsController;

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
				if ( Physics.Raycast( ray, out hit, maxRadius ) ) {
					coverEngaged = true;
					targetCenter = hit.transform.position;
					effectsController.StartGrapple( targetCenter );
					attachJoint = gameObject.AddComponent<SpringJoint>();
					attachJoint.anchor = targetCenter;
					attachJoint.connectedBody = hit.rigidbody;
					attachJoint.spring = 4;
					attachJoint.damper = 8;
					attachJoint.minDistance = 0;
					attachJoint.maxDistance = .5f;
				}
			} else {
				coverEngaged = false;
				effectsController.EndGrapple();
				Destroy (attachJoint);
			}
		}
	}
}
