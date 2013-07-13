#pragma strict

//delete function Start
//declare variables

//particles emitted when bullet hits
var effect : Transform;

//how much to damage opponent
var damage = 100;

function Update () {

	//hit contains location of hit among other things
	var hit : RaycastHit;
	
	//send out ray straight forward from the middle of screen (half of width and height)
	var ray : Ray = Camera.main.ScreenPointToRay(Vector3(Screen.width * 0.5,Screen.height*0.5,0));
	
	// mouse button 0 = left click
	if ( Input.GetMouseButtonDown(0) ) {
		
		//100=range of ray
		if ( Physics.Raycast(ray,hit,100) ) {
		
			//creates effect at hit.point with the rotation at the normal to where the hit came from
			//(emit outwards)
			var particleClone = Instantiate(effect,hit.point,Quaternion.LookRotation(hit.normal));
			particleClone.gameObject.GetComponent(ParticleEmitter).emit = true;
			Destroy(particleClone.gameObject,2);
	
			//send a message to whatever we hit that we should apply damage
			//ApplyDamage is the function that applies the damage; send damage variable with it
			//does not need to have a receiver - nothing will happen
			hit.transform.SendMessage("ApplyDamage",damage, SendMessageOptions.DontRequireReceiver);
	
		}
	}

}