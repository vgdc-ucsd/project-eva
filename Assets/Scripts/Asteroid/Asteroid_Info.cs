using UnityEngine;
using System.Collections;

public class Asteroid_Info : MonoBehaviour
{

	//Health
	public float hitPoints = 200.0f;

	void Update()
	{
		if ( hitPoints <= 0 )
		{
			die();
		}
	}

	public void receiveDamage( float damage )
	{
		hitPoints -= damage;
	}

	private void die()
	{
		Destroy( gameObject );
	}
}
