using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulConnector : MonoBehaviour {

	public GameObject connectionPrefab;

	private AudioSource audio;

	public bool CreatingConnection = false;
	public GameObject[] connections;

	public bool insideSoul = false;
	public bool triggerDownInside = false;

	public int maxConnections;

	public GameObject connection;

	// Use this for initialization
	void Start () {

		audio = GetComponent<AudioSource>();

		EventManager.OnTriggerDown += OnTriggerDown;
    	EventManager.OnTriggerUp += OnTriggerUp;
    	EventManager.StayTrigger += StayTrigger;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void OnTriggerUp( GameObject controller){
		triggerDownInside = false;

		print( connection );
		if( connection != null ){
			Destroy(connection);
			connection = null;
		}


	}

	void OnTriggerDown( GameObject controller){

		if( controller.GetComponent<controllerInfo>().interactionTip == this.gameObject && insideSoul == true ){
			audio.Play();
			triggerDownInside = true;
		}

	}

	void StayTrigger( GameObject controller){

	}

	void OnTriggerEnter(Collider other){

		if( other.tag == "soul" ){
			audio.Play();
			insideSoul = true;

			if( connection != null ){

				Soul s = other.gameObject.GetComponent<Soul>();
				RopeConnector  r = connection.GetComponent<RopeConnector>();

				bool canMake = true;
				for (var i = 0; i < s.connections.Count; i++) {
					RopeConnector oRC = s.connections[i].GetComponent<RopeConnector>();

					if( oRC.From == r.From  && oRC.To == other.gameObject ){
						canMake = false;
					}

					if( oRC.From == r.To  && oRC.To == other.gameObject ){
						canMake = false;
					}

					if( oRC.From == other.gameObject  && oRC.To == r.From ){
						canMake = false;
					}

					if( oRC.From == other.gameObject  && oRC.To == r.To ){
						canMake = false;
					}
					
				}
				

				if( r.From != other.gameObject && canMake == true){
					r.To = other.gameObject;
					s.connections.Add( connection );
					r.From.GetComponent<Soul>().connections.Add(connection);
					connection = null;
				}

			}
		}
	}

	void OnTriggerExit(Collider other){

		if( other.tag == "soul" ){


			
			if( triggerDownInside == true ){
				audio.Play();
				CreateConnection(other.gameObject);
			}
		}

	}

	void CreateConnection(GameObject other){
		
		GameObject c = Instantiate(connectionPrefab);
		RopeConnector  rope = c.GetComponent<RopeConnector>();
		
		rope.From = other;
		rope.To = this.gameObject;

		rope.Reset();

		print( c );

		connection = c;

	}
}
