using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacePointsByTouch : MonoBehaviour {

  public GameObject[] points;

  void Update(){

    for( int i = 0; i < points.Length; i++ ){

      if( Input.touchCount > i  ){
        Touch t = Input.GetTouch(i);

        Ray ray = Camera.main.ScreenPointToRay(t.position);
        Debug.DrawRay(ray.origin, ray.direction * 20, Color.yellow);

        points[i].transform.position = ray.origin + ray.direction * 20;

      }else{
        points[i].transform.position = new Vector3( 1000000 , 0 , 0 );
      }

    }
  }
}
