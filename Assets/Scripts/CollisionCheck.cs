using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour {

	public bool colliding = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter (Collider col)
	{
		if (transform.tag == "Exit")
		{
			if (col.transform.tag == "Exit")
				colliding = true;
		}
		else
		{
			if (col.transform.tag == "Corridor" || col.transform.tag == "Room")
				colliding = true;
		}
	}
}
