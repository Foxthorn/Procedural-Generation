using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public int iterations;

	public List<GameObject> Rooms = new List<GameObject>();
	public List<GameObject> Corridors = new List<GameObject>();
	public List<GameObject> Ends = new List<GameObject>();
	// Use this for initialization
	void Start () {
		int i = Random.Range(0, Rooms.Count);
		Instantiate(Rooms[i]);
		GenerateMap(Rooms[i]);
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
	
	void GenerateMap(GameObject room)
	{
		var pendingExits = GetExits(room.transform);
		var newExits = new List<Transform>();
		while (iterations > 0)
		{
			newExits.Clear();
			foreach (var exit in pendingExits)
			{
				var newRoom = room;
				if (exit.transform.parent.transform.tag == "Room")
					newRoom = Instantiate(Corridors[Random.Range(0, Corridors.Count)]);
				else
					newRoom = Instantiate(Rooms[Random.Range(0, Rooms.Count)]);
				var ex = GetExits(newRoom.transform);	
				AlignExits(exit, ex[0]);
				for(int i = 0; i < ex.Count; i++)
				{
					if (i != 0)
					{
						newExits.Add(ex[i]);
					}
				}
			}
			pendingExits.Clear();
			pendingExits.AddRange(newExits);
			iterations -= 1;
		}
	}

	float Azimuth(Vector3 vector)
	{
		return Vector3.Angle(Vector3.forward, vector) * Mathf.Sign(vector.x);
	}
}
