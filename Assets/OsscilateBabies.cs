using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OsscilateBabies : MonoBehaviour {

public float osscilateVal;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {	
    transform.Rotate(Vector3.right * Mathf.Sin(Time.time* osscilateVal * 10)* 20 * Time.deltaTime);
	}

}
