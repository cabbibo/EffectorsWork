using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionList : MonoBehaviour {

    public List<RopeConnector> toList;
    public List<RopeConnector> fromList;
    public BreathPlayer player;


	// Use this for initialization
	void Awake() {

        toList = new List<RopeConnector>();
        fromList = new List<RopeConnector>();

	}

    public void Fire(float power,float pitch,float dir){

        player.Play(pitch, power);


        foreach ( RopeConnector rope in toList){
                    rope.Fire( power, pitch , -1 );
        }


        foreach ( RopeConnector rope in fromList){
            rope.Fire( power, pitch , 1 );
        }
    }

	// Update is called once per frame
	void Update () {

	}
}
