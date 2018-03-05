
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderBuffer : UpdatingBuffer {


  public GameObject[] colliders;

  public override void BeforeCreateBuffer(){

    structSize = 34;
    count = colliders.Length;
    print( count );
    
  }

  public override void AfterCreateBuffer(){
    UpdateBuffer();
    _buffer.SetData( values );

  }

  void AssignColliderValues(Transform t, int id  ){

    int index = id * structSize;

    values[index] = id;
    values[index+1] = 0;

    Matrix4x4 m = t.localToWorldMatrix;
    for( int i = 0; i < 16; i++ ){
      int x = i % 4;
      int y = (int) Mathf.Floor(i / 4);
      values[index + i + 2] = m[x,y];
    }

    m = t.worldToLocalMatrix;
    for( int i = 0; i < 16; i++ ){
      int x = i % 4;
      int y = (int) Mathf.Floor(i / 4);
      values[index + i + 16 + 2] = m[x,y];
    }



  }   

  public override void UpdateBuffer(){
    
    int index = 0;
    for( int i = 0; i < colliders.Length; i++ ){
      AssignColliderValues( colliders[i].transform, i );
    }
  
    _buffer.SetData( values );
  }
}