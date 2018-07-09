using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

	public Transform memberPrefab;
	public Transform enemyPrefab;

	public int numberOfEnemies;
	public int numberOfMembers;

	public List<Member> members;
	public List<Enemy> enemies;

	public float bounds;
	public float spawnRadius;
	public bool spawnRight;

	// Use this for initialization
	void Start () {
		members = new List<Member> ();
		enemies = new List<Enemy> ();

		//Spawn (memberPrefab, numberOfMembers);
		SpawnOnSide(memberPrefab, numberOfMembers, spawnRight);
		Spawn (enemyPrefab, numberOfEnemies);

		members.AddRange (FindObjectsOfType<Member> ());
		enemies.AddRange (FindObjectsOfType<Enemy> ());
	}

	void Spawn(Transform prefab, int count){
		for (int i = 0; i < count; i++) {
			Instantiate (prefab, new Vector3 (Random.Range (-spawnRadius, spawnRadius), Random.Range (-spawnRadius, spawnRadius), 0), Quaternion.identity);
		}
	}

	void SpawnOnSide(Transform prefab, int count, bool rightSide){// Spawn function but to spanw members on the side of the screen
		if (rightSide) {
			for (int i = 0; i < count; i++) {
				//Instantiate (prefab, new Vector3 (Random.Range (spawnRadius, bounds), Random.Range (-spawnRadius, spawnRadius), 0), Quaternion.identity);
				Instantiate (prefab, new Vector3 (Random.Range (spawnRadius, bounds), Random.Range (-bounds, bounds), 0), Quaternion.identity); //spawns on Y according to bounds and not radius
			}
		} else {
			for (int i = 0; i < count; i++) {
				//Instantiate (prefab, new Vector3 (Random.Range (-spawnRadius, -bounds), Random.Range (-spawnRadius, spawnRadius), 0), Quaternion.identity);
				Instantiate (prefab, new Vector3 (Random.Range (-spawnRadius, -bounds), Random.Range (-bounds, bounds), 0), Quaternion.identity); //spawns on Y according to bounds and not radius
			}
		}
	}

	public List<Member> GetNeighbors(Member member, float radius)
	{
		List<Member> neighborsFound = new List<Member> ();

		foreach (var otherMember in members) 
		{
			if (otherMember == member) 
			{
				continue;
			}

			if (Vector3.Distance (member.transform.position, otherMember.transform.position) <= radius) 
			{
					neighborsFound.Add (otherMember);
			}
		}

		return neighborsFound;
	}

	public List<Enemy> GetEnemies(Member member, float radius)
	{
		List <Enemy> returnEnemies = new List<Enemy> ();

		foreach (var enemy in enemies) {
			if (Vector3.Distance (member.position, enemy.position) <= radius) {
				returnEnemies.Add (enemy);
			}
		}

		return returnEnemies;
	}
}
