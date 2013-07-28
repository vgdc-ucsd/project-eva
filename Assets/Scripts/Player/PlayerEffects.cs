using UnityEngine;
using System.Collections;

public class PlayerEffects : MonoBehaviour {
	
	public int trailIntensity = 30;
	public float trailLifetime = 4.0f;

	private GameObject trailGenerator;
	private GameObject currentTrail;
	private TrailRenderer lightTrail;
	
	private LineRenderer grappleBeam;


	protected void Awake() {
		trailGenerator = transform.FindChild( "fx_light_trail" ).gameObject;
		lightTrail = trailGenerator.GetComponent<TrailRenderer>();
		grappleBeam = transform.FindChild( "fx_grapple_beam" ).GetComponent<LineRenderer>();
	}

	protected void Update() {
		if ( currentTrail != null ) {
			currentTrail.transform.position = gameObject.transform.position;
		}
		grappleBeam.SetPosition( 0, transform.position );
	}

	public void StartLightTrail() {
		lightTrail.enabled = true;
		currentTrail = ( GameObject )GameObject.Instantiate( trailGenerator );
		currentTrail.name = "fx_light_trail_clone";
		Destroy(currentTrail, trailLifetime * 2);
		lightTrail.enabled = false;
	}

	public void StopLightTrail() {
		lightTrail.enabled = false;
		currentTrail = null;
	}

	public void StartGrapple( Vector3 grappleTarget ) {
		grappleBeam.enabled = true;
		grappleBeam.SetPosition( 1, grappleTarget );
	}

	public void EndGrapple() {
		grappleBeam.enabled = false;
	}
}
