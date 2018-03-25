using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayHairs : MonoBehaviour {

    public GranPlayer player;
    private HairOnVertBuffer hairs;

	// Use this for initialization
	void Start () {
        hairs = GetComponent<HairOnVertBuffer>();
	}

	// Update is called once per frame
	void Update () {
        print(  .01f / hairs.finalFloats[0]);

        player.amount = .003f / hairs.finalFloats[0];
        player.offset = .03f / hairs.finalFloats[0];

        player.pitch = .003f / hairs.finalFloats[0];

	}
}
