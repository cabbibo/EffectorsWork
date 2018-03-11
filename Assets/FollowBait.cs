using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBait : MonoBehaviour {

    public Rigidbody bait;
    public Rigidbody head;

    private Vector3 delta;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

        delta = bait.position - head.position;
        head.AddForce( delta *100);
	}
}
