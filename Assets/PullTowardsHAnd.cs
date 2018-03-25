using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullTowardsHAnd : MonoBehaviour {

    public GameObject hand1;
    public GameObject hand2;

        private controllerInfo ci1;
    private controllerInfo ci2;

    private Rigidbody rb;

	// Use this for initialization
	void Start () {

        rb = GetComponent<Rigidbody>();
        ci1 = hand1.GetComponent<controllerInfo>();
        ci2 = hand2.GetComponent<controllerInfo>();

	}

	// Update is called once per frame
	void Update () {



        Vector3 dif;

        dif = hand1.transform.position - transform.position;
        //rb.AddForce( dif * 1 * ci1.triggerVal );

        dif = hand2.transform.position - transform.position;
        //rb.AddForce( dif * 1 * ci2.triggerVal );


	}
}
