using UnityEngine;
using System.Collections;

public class AsteroidGlow : MonoBehaviour {
	
	private GameObject ParentAsteroid;
	private Shader outlineShader;
	private Shader defaultShader;
	
	// Use this for initialization
	void Start () {
		ParentAsteroid = transform.parent.gameObject;
		outlineShader = Shader.Find( "Outlined/Silhouetted Diffuse" );
		defaultShader = Shader.Find( "Diffuse" );
	}
	
	void OnTriggerEnter(Collider other) {
		if( other.tag == "Player" && other.networkView.isMine ) {
			ParentAsteroid.renderer.material.shader = outlineShader;
		}
	}
	
	void OnTriggerExit(Collider other) {
		if( other.tag == "Player" && other.networkView.isMine ) {
			ParentAsteroid.renderer.material.shader = defaultShader;
		}
	}
}
