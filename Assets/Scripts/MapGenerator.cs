using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public int iterations;

	public List<GameObject> Rooms = new List<GameObject>();
	public List<GameObject> Corridors = new List<GameObject>();
	public List<GameObject> Ends = new List<GameObject>();

	List<Transform> newExits = new List<Transform>();
	List<Transform> pendingExits = new List<Transform>();
	List<GameObject> map = new List<GameObject>();
	GameObject newRoom;
	int numRoomsPlaced;
	// Use this for initialization
	void Start () {
		// Random.seed = 2;
		int i = Random.Range(0, Rooms.Count);
		Instantiate(Rooms[i]);
		pendingExits = GetExits(Rooms[i].transform);
		map.Add(Rooms[i]);
	}

	void Update()
	{
		CheckCollisions();
		if (iterations > 0)
			GenerateMap();
	}

	List<Transform> GetExits(Transform room)
	{
		List<Transform> exits = new List<Transform>();
		foreach (Transform child in room)
		{
			if (child.tag == "Exit")
				exits.Add(child);
		}
		return exits;
	}

	void AlignExits(Transform oldExit, Transform newExit)
	{
		var	newModule = newExit.transform.parent;
		var forwardVectorToMatch = -oldExit.transform.forward;
		var correctiveRotation = Azimuth(forwardVectorToMatch) - Azimuth(newExit.transform.forward);
		newModule.RotateAround(newExit.transform.position, Vector3.up, correctiveRotation);
		var correctiveTranslation = oldExit.transform.position - newExit.transform.position;
		newModule.transform.position += correctiveTranslation;
	}
	
	void GenerateMap()
	{
		newExits.Clear();
		foreach (var exit in pendingExits)
		{
			if (exit.transform.parent.transform.tag == "Room")
				newRoom = Instantiate(Corridors[Random.Range(0, Corridors.Count)]);
			else
				newRoom = Instantiate(Rooms[Random.Range(0, Rooms.Count)]);
			var exits = GetExits(newRoom.transform);	
			map.Add(newRoom);
			AlignExits(exit, exits[0]);
			numRoomsPlaced += 1;
			for(int i = 0; i < exits.Count; i++)
			{
				if (i != 0)
				{
					newExits.Add(exits[i]);
				}
			}
		}
		pendingExits.Clear();
		iterations -= 1;
	}

	void RemoveExits(int index)
	{
		foreach(Transform child in map[index].transform)
		{
			if (child.tag == "Exit")
			{
				newExits.Remove(child);
			}
		}
	}

	void CheckCollisions()
	{
		List<int> indices = new List<int>();
		for(int i = 0; i <= numRoomsPlaced; i++)
		{
			var index = map.Count - 1 - i;
			foreach(Transform child in map[index].transform)
			{
				if (child.tag != "Exit")
				{
					var script = child.gameObject.GetComponent<CollisionCheck>();
					if (script.colliding)
					{
						indices.Add(index);
						RemoveExits(index);
					}
				}
			}
		}
		foreach(int i in indices)
		{
			Destroy(map[i]);
			map.Remove(map[i]);
		}
		pendingExits.AddRange(newExits);
		numRoomsPlaced = 0;	
	}

	float Azimuth(Vector3 vector)
	{
		return Vector3.Angle(Vector3.forward, vector) * Mathf.Sign(vector.x);
	}
}
