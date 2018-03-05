using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceOsscilate : MonoBehaviour {


  private Rigidbody rb;
  private Vector3 startPos;
	// Use this for initialization
	void Start () {
		  rb = GetComponent<Rigidbody>();
      startPos = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
    rb.AddForce( 80 * new Vector3( Mathf.Sin( Time.time * 2.1f ) , Mathf.Cos( Time.time * 2.7f ) , Mathf.Sin( Time.time * 1.8f ))   );
    rb.AddForce( -10f * (transform.position - startPos )   );
	}
}
