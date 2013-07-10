#pragma strict

var Health = 200;

function Update () {

	if(Health <= 0) {
	
		Dead();
		
	}

}

function ApplyDamage(damage : int) {

	Health -= damage;

}

function Dead() {

	Destroy(gameObject);

}