using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMoonPosition : MonoBehaviour {


    public BREATHSETTER bs;

    private Vector3 randVec;

    float radius;

    private Vector3 OG;
	// Use this for initialization
	void Start () {
        randVec = new Vector3(Random.value, Random.value, Random.value);
        OG = transform.position;
	}

	// Update is called once per frame
	void Update () {


        Vector3 nV = OG;
        nV = bs.slow * randVec + OG;

        transform.position = nV;
	}
}
