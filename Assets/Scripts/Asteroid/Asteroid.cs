using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]

public class Asteroid : MonoBehaviour {

	public float massScalar = 5;
	public float velocityScalar = 5;

	// Use this for initialization
	protected void Start () {
		RandomizeSize();
		SetAngularVelocity();
	}

	private void RandomizeSize() {
		float newScale = Random.value * massScalar;
		float newMass = rigidbody.mass * newScale;
		rigidbody.mass = newMass;
		transform.localScale.Set( newScale, newScale, newScale );
	}

	private void SetAngularVelocity() {
		float newVelocityX = Random.value * velocityScalar;
		float newVelocityY = Random.value * velocityScalar;
		float newVelocityZ = Random.value * velocityScalar;
		rigidbody.angularVelocity.Set( newVelocityX, newVelocityY, newVelocityZ );
	}
}
