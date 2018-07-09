using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberConfig : MonoBehaviour {

	public float maxFOV = 180;
	public float maxAcceleration;
	public float maxVelocity;

	//Wandering variables
	public float wanderJitter;
	public float wanderRadius;
	public float wanderDistance;
	public float wanderPriority;

	//Cohesion
	public float cohesionRadius;
	public float cohesionPriority;

	//Alignment
	public float alignmentRadius;
	public float alignmentPriority;

	//Separation
	public float separationRaidus;
	public float separationPriority;

	//Avoidance
	public float avoidanceRadius;
	public float avoidancePriority;

	/* 
		Explanations:
			Priorities: Cohesion and Wander functions in Member class is what defines movemet. Priority defines which action, wander or cohesion, takes precedence over the other

	*/
}
