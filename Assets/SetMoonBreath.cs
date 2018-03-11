using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMoonBreath : MonoBehaviour {

    public BREATHSETTER breath;

    private Material mat;

	// Use this for initialization
	void Start () {
	       mat = GetComponent<Renderer>().material;
	}

	// Update is called once per frame
	void Update () {
        mat.SetFloat("_BREATH",breath.regular);
    }
}
