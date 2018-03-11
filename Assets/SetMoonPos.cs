using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMoonPos : MonoBehaviour {


    public Transform MOON;
    private Material m;
    // Use this for initialization
	void Start () {

    m = GetComponent<MeshRenderer>().sharedMaterial;
            m.SetVector( "_MOON" , MOON.position);


	}

	// Update is called once per frame
	void Update () {

    m.SetVector( "_MOON" , MOON.position);
	}
}
