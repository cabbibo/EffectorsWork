using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soul : MonoBehaviour {

	private AudioSource audio;

	public List <GameObject> connections;  

	public bool CreatingConnection = false;
	public int maxConnections;

	public GameObject creatingHand;

	// Use this for initialization
	void Start () {

		connections = new List<GameObject> ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void OnTriggerEnter(Collider other){

		if( other.tag == "hand" ){
			for( int i = 0; i < connections.Count; i++ ){
				//connections[i].sendPulse(this.gameObject);
			}
		}


	}




}
