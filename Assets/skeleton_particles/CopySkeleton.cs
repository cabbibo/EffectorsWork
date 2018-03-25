using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopySkeleton : MonoBehaviour {

	public Transform myBase;
	public Transform otherBase;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		percolateCopy(myBase,otherBase);
		
	}

	void percolateCopy( Transform t , Transform tToCopy ){
		t.localPosition = tToCopy.localPosition;
		t.localRotation = tToCopy.localRotation;
		t.localScale = tToCopy.localScale;
		for( int i = 0; i < t.childCount; i++ ){
			percolateCopy(t.GetChild(i), tToCopy.GetChild(i));
		}

	}
}
