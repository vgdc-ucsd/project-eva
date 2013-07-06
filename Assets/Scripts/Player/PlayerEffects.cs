using UnityEngine;
using System.Collections;

public class PlayerEffects : MonoBehaviour {
	
	public int trailIntensity = 30;

	private ParticleSystem lightTrail;

	protected void Awake() {
		lightTrail = GetComponent<ParticleSystem>();
		lightTrail.emissionRate = 0;
	}

	public void startLightTrail() {
		lightTrail.emissionRate = trailIntensity;
	}

	public void stopLightTrail() {
		lightTrail.emissionRate = 0;
	}
}
