using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetStartHue : MonoBehaviour {


	// Use this for initialization
	void Start () {

        Material m = GetComponent<MeshRenderer>().sharedMaterial;
        float h = m.GetFloat("_HueSize") * Random.Range(0,.999f);
        float bh = m.GetFloat("_BaseHue");

//        print( h + bh );
        GetComponent<MeshRenderer>().material.SetFloat("_BaseHue", h+bh );

	}

	// Update is called once per frame
	void Update () {

	}
}
