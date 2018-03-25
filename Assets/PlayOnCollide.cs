using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnCollide : MonoBehaviour {

    public BreathPlayer player;
    public ConnectionList list;
	// Use this for initialization
	void Start () {
        list = GetComponent<ConnectionList>();

	}

	// Update is called once per frame
	void Update () {

	}

    public void OnCollisionEnter( Collision o){

        //print("ASDAS");
       // print( o.impulse.magnitude);

        //player.Play(Random.Range(.3f ,1f));
        list.Fire(1,o.collider.attachedRigidbody.velocity.magnitude,0);
    }
}
