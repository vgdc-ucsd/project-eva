using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class PlayerEffects : MonoBehaviour {
	
	public int trailIntensity = 30;

	private ParticleSystem lightTrail;

	protected void Awake() {
		lightTrail = GetComponent<ParticleSystem>();
		lightTrail.emissionRate = 0;
	}

	public void StartLightTrail() {
		lightTrail.emissionRate = trailIntensity;
	}

	public void StopLightTrail() {
		lightTrail.emissionRate = 0;
	}
}
