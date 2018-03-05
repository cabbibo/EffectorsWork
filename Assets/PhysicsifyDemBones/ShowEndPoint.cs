using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowEndPoint : MonoBehaviour {	


	public Transform bone;

	// Use this for initialization
	void Start () {

		transform.position = bone.transform.TransformPoint( new Vector3(0,1,0) );// + bone.transform.position;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
