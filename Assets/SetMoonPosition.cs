using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMoonPosition : MonoBehaviour {


    public BREATHSETTER bs;

    float radius;

    private Vector3 OG;
	// Use this for initialization
	void Start () {

        OG = transform.position;
	}

	// Update is called once per frame
	void Update () {

        float z =  bs.slow  * 2  + OG.z;
        float y =  bs.slow  * 2  + OG.y;
        float x = OG.x;//Mathf.Sin( bs.regular ) * 10  + OG.z;
        transform.position = new Vector3(x,y,z);

	}
}
