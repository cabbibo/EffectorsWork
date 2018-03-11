using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BREATHSETTER : MonoBehaviour {

    public float regular;
    public float fast;
    public float slow;
    public float lonnng;

    public Vector4 all;

    public float regularSpeed;
    public float fastSpeed;
    public float slowSpeed;
    public float lonnngSpeed;


    private float currentTime;
    private System.DateTime epochStart;

	// Use this for initialization
	void Start () {

        epochStart = System.DateTime.UtcNow;//new System.DateTime(2017, 11, 6, 0, 0, 0, System.DateTimeKind.Utc);
        currentTime = (float)(System.DateTime.UtcNow - epochStart).TotalSeconds;

	}

	// Update is called once per frame
	void FixedUpdate () {

        currentTime = (float)(System.DateTime.UtcNow - epochStart).TotalSeconds;

        regular = Mathf.Sin( 2* Mathf.PI * currentTime / regularSpeed );
        fast = Mathf.Sin( 2* Mathf.PI * currentTime / fastSpeed );
        slow = Mathf.Sin( 2* Mathf.PI * currentTime / slowSpeed );
        lonnng = Mathf.Sin( 2* Mathf.PI * currentTime / lonnngSpeed );

        all=new Vector4( regular , fast , slow , lonnng );

	}

}
