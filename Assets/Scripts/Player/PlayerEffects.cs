using UnityEngine;
using System.Collections;

public class PlayerEffects : MonoBehaviour {

	public int trailIntensity = 30;
	public float trailLifetime = 4.0f;
	public float sparkLifetime = 1.0f;

	private GameObject trailGenerator;
	private GameObject currentTrail;
	private TrailRenderer lightTrail;
	private ParticleSystem gunSparks;

	private LineRenderer grappleBeam;


	protected void Awake() {
		trailGenerator = transform.FindChild( "fx_light_trail" ).gameObject;
		lightTrail = trailGenerator.GetComponent<TrailRenderer>();
		grappleBeam = transform.FindChild( "fx_grapple_beam" ).GetComponent<LineRenderer>();
		gunSparks = transform.FindChild( "fx_gun_spark" ).GetComponent<ParticleSystem>();
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
		networkView.RPC( "NetworkLightTrail", RPCMode.Others, currentTrail.transform.position, currentTrail.transform.rotation );
		lightTrail.enabled = false;
	}

	public void StopLightTrail() {
		lightTrail.enabled = false;
		currentTrail = null;
	}

	public void StartGrapple( Vector3 grappleTarget ) {
		grappleBeam.enabled = true;
		grappleBeam.SetPosition( 1, grappleTarget );
		networkView.RPC( "NetworkStartGrappleBeam", RPCMode.Others, grappleTarget );
	}

	public void EndGrapple() {
		grappleBeam.enabled = false;
		networkView.RPC( "NetworkEndGrappleBeam", RPCMode.Others );
	}

	public void TriggerGunSpark( Vector3 position, Vector3 normal ) {
		GameObject newSparks = ( GameObject ) GameObject.Instantiate( gunSparks.gameObject, position, Quaternion.identity );
		newSparks.transform.forward = normal;
		newSparks.GetComponent<ParticleSystem>().enableEmission = true;
		networkView.RPC( "NetworkGunSpark", RPCMode.Others, position, normal );
		Destroy( newSparks, sparkLifetime );
	}

	[RPC]
	public void NetworkLightTrail( Vector3 position, Quaternion rotation ) {
		lightTrail.enabled = true;
		currentTrail = ( GameObject )GameObject.Instantiate( trailGenerator );
		currentTrail.name = "fx_light_trail_clone";
		Destroy( currentTrail, trailLifetime * 2 );
		lightTrail.enabled = false;
	}
	
	[RPC]
	public void NetworkGunSpark( Vector3 position, Vector3 normal ) {
		GameObject newSparks = ( GameObject ) GameObject.Instantiate( gunSparks.gameObject, position, Quaternion.identity );
		newSparks.transform.forward = normal;
		newSparks.GetComponent<ParticleSystem>().enableEmission = true;
		Destroy( newSparks, sparkLifetime );
	}
	
	[RPC]
	public void NetworkStartGrappleBeam( Vector3 grappleTarget ) {
		grappleBeam.enabled = true;
		grappleBeam.SetPosition( 1, grappleTarget );
	}
	
	[RPC]
	public void NetworkEndGrappleBeam() {
		grappleBeam.enabled = false;	
	}
}
