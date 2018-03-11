using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OsscilatePos : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
        transform.position += .08f * Vector3.left * Mathf.Sin( Time.time * 2.11f );
        transform.position += .08f * Vector3.right * Mathf.Sin( .2f * Time.time * 2.11f );
        transform.position += .08f * Vector3.forward * Mathf.Sin( Time.time *  .811f );
	}
}
