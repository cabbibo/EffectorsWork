using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;


public class AcceleratorOssiclate : MonoBehaviour {


    void Start(){
      Input.gyro.enabled = true;
    }
    void Update () 
    {
        //transform.Translate(Input.acceleration.x, 0, -Input.acceleration.z);
        GyroModifyCamera();

        
       
    }




    // The Gyroscope is right-handed.  Unity is left handed.
    // Make the necessary change to the camera.
    void GyroModifyCamera()
    {


        Quaternion q = GyroToUnity(Input.gyro.attitude);
        transform.rotation = q;

        //print( q );
    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
}
