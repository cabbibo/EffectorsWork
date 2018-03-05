using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buffer : MonoBehaviour {

  public ComputeBuffer _buffer;
  public int count;

  [HideInInspector]
  public int structSize;

  [HideInInspector]
  public float[] values;


  public virtual void OnEnable(){
    DoBuffers();
  }

  public void DoBuffers(){
    BeforeCreateBuffer();
    SetUpBuffers();
    CreateBuffer();
    AfterCreateBuffer();
  }

  public virtual void OnDisable(){
    ReleaseBuffer();
  }


  public virtual void SetUpBuffers(){

    print( count );
    print( structSize );
    if( count > 0 && structSize > 0){
      _buffer = new ComputeBuffer( count , structSize * sizeof(float) );  
      values = new float[ structSize * count ];   
    }else{
      print( this );
      print("You have a zero value for count or struct size.....");
    }
  }
  
  public virtual void CreateBuffer(){} 

  public virtual void BeforeCreateBuffer(){} 
  public virtual void AfterCreateBuffer(){} 

  void ReleaseBuffer(){
    if( _buffer != null ){ _buffer.Release(); }
  }


}
