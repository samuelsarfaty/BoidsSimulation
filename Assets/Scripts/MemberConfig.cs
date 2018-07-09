using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberConfig : MonoBehaviour {

	public float maxFOV = 180;
	public float maxAcceleration;
	public float maxVelocity;

	[Header("Wandering Parameters")]
	public float wanderJitter;
	public float wanderRadius;
	public float wanderDistance;
	public float wanderPriority;

	[Header("Cohesion Parameters")]
	public float cohesionRadius;
	public float cohesionPriority;

	[Header("Alignment Parameters")]
	public float alignmentRadius;
	public float alignmentPriority;

	[Header("Separation Parameters")]
	public float separationRaidus;
	public float separationPriority;

	[Header("Avoidance Parameters")]
	public float avoidanceRadius;
	public float avoidancePriority;

	/* 
		Explanations:
			Priorities: Cohesion and Wander functions in Member class is what defines movemet. Priority defines which action, wander or cohesion, takes precedence over the other

	*/
}
