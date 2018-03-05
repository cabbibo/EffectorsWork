using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;

public class RotateCameraByGyro : MonoBehaviour {

  public float speedL;
  public float speedU;
  public float lerpBack;

  public GameObject objectToMove;

  public GameObject center;

  public Vector3 up;
  public Vector3 left;
  public Vector3 startPoint;


  void Start(){
    Screen.orientation = ScreenOrientation.Landscape;
    Input.gyro.enabled = true;
    startPoint = objectToMove.transform.position;
  }
	
	// Update is called once per frame
	void Update () {
		GyroModifyCamera();
	}


  // The Gyroscope is right-handed.  Unity is left handed.
  // Make the necessary change to the camera.
  void GyroModifyCamera()
  {


      Quaternion q = GyroToUnity(Input.gyro.attitude);
      transform.rotation = q;

      Vector3 e = q.eulerAngles;


      float leftVal = ((e.x - 180 ) / 180) * speedL;
      objectToMove.transform.position += leftVal * left;


      float upVal = ((e.y - 180 ) / 180) * speedU;
      objectToMove.transform.position += upVal * up;

      objectToMove.transform.position = Vector3.Lerp(objectToMove.transform.position,startPoint, lerpBack);

      //objectToMove.transform.position = new Vector3( Mathf.Cos( angle ) * radius , center.transform.position.y , Mathf.Sin( angle ) * radius);

      objectToMove.transform.LookAt( center.transform );




      //print( q );
  }

  private static Quaternion GyroToUnity(Quaternion q)
  {
      return new Quaternion(q.x, q.y, -q.z, -q.w);
  }


}
