using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Member : MonoBehaviour {

	public Vector3 position;
	public Vector3 velocity;
	public Vector3 acceleration;


	public Level level;
	public MemberConfig conf;

	private Vector3 wanderTarget;
	private Vector3 push = new Vector3 (1,0,0); //add push after Combine

	void Start()
	{
		level = FindObjectOfType<Level> ();
		conf = FindObjectOfType<MemberConfig> ();

		position = transform.position;
		velocity = new Vector3 (Random.Range (-3, 3), Random.Range (-3, 3), 0);
	}

	void Update ()
	{
		acceleration = Combine () + push;
		acceleration = Vector3.ClampMagnitude (acceleration, conf.maxAcceleration);
		velocity = velocity + acceleration * Time.deltaTime;
		velocity = Vector3.ClampMagnitude (velocity, conf.maxVelocity);

		position = position + velocity * Time.deltaTime;

		WrapAround (ref position, -level.bounds, level.bounds);

		transform.position = position;
	}

	protected Vector3 Wander() //Makes object wander by adding a random vector nearby and then moving the object towards that vector //TODO modify here probably to make sideways movement.
	{
		float jitter = conf.wanderJitter * Time.deltaTime; //get jitter
		wanderTarget += new Vector3 (RandomBinomial () * jitter, RandomBinomial () * jitter, 0); //create random vector //0 on x?
		wanderTarget = wanderTarget.normalized; //bring vector back to unit circle

		wanderTarget *= conf.wanderRadius; //make radius the same size as wonder circle. i.e. we create a random vector within the wandering cirlce.

		Vector3 targetInLocalSpace = wanderTarget + new Vector3 (conf.wanderDistance, conf.wanderDistance, 0);
		Vector3 targetInWorldSpace = transform.TransformPoint (targetInLocalSpace);

		targetInWorldSpace -= this.position;

		return targetInWorldSpace.normalized;
	}

	protected Vector3 WanderWithDirection(bool rightToLeft) //Makes object wander by adding a random vector nearby and then moving the object towards that vector //TODO modify here probably to make sideways movement.
	{
		float jitter = conf.wanderJitter * Time.deltaTime; //get jitter

		if (rightToLeft) {
			wanderTarget += new Vector3 (RandomBinomialNegative() * jitter, 0 * jitter, 0); //create random vector
		} else {
			wanderTarget += new Vector3 (RandomBinomialPositive() * jitter, RandomBinomial() * jitter, 0); //create random vector
		}

		wanderTarget = wanderTarget.normalized; //bring vector back to unit circle

		wanderTarget *= conf.wanderRadius; //make radius the same size as wonder circle. i.e. we create a random vector within the wandering cirlce.

		Vector3 targetInLocalSpace = wanderTarget + new Vector3 (conf.wanderDistance, conf.wanderDistance, 0);
		Vector3 targetInWorldSpace = transform.TransformPoint (targetInLocalSpace);

		targetInWorldSpace -= this.position;

		return targetInWorldSpace.normalized;
	}

	Vector3 Cohesion() //Finds objects in the specified vecinity and adjusts position based on average of neighbors in radius
	{
		Vector3 cohesionVector = new Vector3 ();
		int countMembers = 0;

		var neighbors = level.GetNeighbors (this, conf.cohesionRadius);
		if (neighbors.Count == 0) {
			return cohesionVector;
		}

		foreach (var member in neighbors) 
		{
			if (isInFOV (member.position)) 
			{
				cohesionVector += member.position;
				countMembers++;
			}
		}
		if (countMembers == 0) 
		{
			return cohesionVector;
		}

		cohesionVector /= countMembers;
		cohesionVector = cohesionVector - this.position;
		cohesionVector = Vector3.Normalize (cohesionVector);

		return cohesionVector;
	}

	Vector3 Alignment()
	{
		Vector3 alignVector = new Vector3 ();
		var members = level.GetNeighbors (this, conf.alignmentRadius);

		if (members.Count == 0) {
			return alignVector;
		}

		foreach (var member in members) {
			if (isInFOV (member.position)) {
				alignVector += member.velocity;
			}
		}

		return alignVector.normalized;
	}

	Vector3 Separation()
	{
		Vector3 separateVector = new Vector3 ();
		var members = level.GetNeighbors (this, conf.separationRaidus);

		if (members.Count == 0) {
			return separateVector;
		}

		foreach (var member in members) {
			if (isInFOV (member.position)) {
				Vector3 movingTowards = this.position - member.position;
				if (movingTowards.magnitude > 0) {
					separateVector += movingTowards.normalized / movingTowards.magnitude;
				}
			}
		}

		return separateVector.normalized;

	}

	Vector3 Avoidance()
	{
		Vector3 avoidVector = new Vector3 ();
		var enemyList = level.GetEnemies (this, conf.avoidanceRadius);

		if (enemyList.Count == 0) {
			return avoidVector;
		}

		foreach (var enemy in enemyList) {
			avoidVector += RunAway (enemy.position);
		}

		return avoidVector.normalized;
	}

	Vector3 RunAway(Vector3 target)
	{
		Vector3 neededVelocity = (position - target).normalized * conf.maxVelocity;
		return neededVelocity - velocity;
	}

	virtual protected Vector3 Combine(){ //Uses both cohesion and wander multiplied by their priorities to guide object's behavior.
		Vector3 finalVec = conf.cohesionPriority * Cohesion () + 
			//conf.wanderPriority * Wander () +
			conf.wanderPriority * WanderWithDirection (level.rightToLeft) + //TODO test this function but return to previous for normal behavior
			conf.alignmentPriority * Alignment() + 
			conf.separationPriority * Separation() + 
			conf.avoidancePriority * Avoidance();
		
		return finalVec;
	}

	void WrapAround(ref Vector3 vector, float min, float max)
	{
		vector.x = WrapAroundFloat (vector.x, min, max);
		vector.y = WrapAroundFloat (vector.y, min, max);
		vector.z = WrapAroundFloat (vector.z, min, max);
	}


	float WrapAroundFloat(float value, float min, float max) //Checks if object leaves the bounds and makes it appear on the other side
	{
		if (value > max) {
			value = min;
		} else if (value < min) {
			value = max;
		}

		return value;
	}

	float RandomBinomial()
	{
		return Random.Range (0f, 1f) - Random.Range (0f, 1f);
	}

	float RandomBinomialPositive() //Copied from above, return only positive values for wandering
	{
		return Random.Range (0f, 1f);
	}

	float RandomBinomialNegative() //Copied from above, return only positive values for wandering
	{
		return Random.Range (0f, -1f);
	}

	bool isInFOV(Vector3 vec)
	{
		return Vector3.Angle (this.velocity, vec - this.position) <= conf.maxFOV;
	}
}
