using UnityEngine;
using System.Collections;


public class TriangleBuffer : MonoBehaviour  {

  public Mesh mesh;

  [HideInInspector]
  public int[] values;

  [HideInInspector]
  public int structSize;


  public ComputeBuffer _buffer;
  public int count;

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
      values = new int[ structSize * count ];
    }else{
      print( this );
      print("You have a zero value for count or struct size.....");
    }
  }

  public virtual void AfterCreateBuffer(){}

  void ReleaseBuffer(){
    if( _buffer != null ){ _buffer.Release(); }
  }





  public virtual void GetMesh(){

    if( mesh == null){
      mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
    }

  }

  public virtual void BeforeCreateBuffer(){
    GetMesh();
    print( mesh );
    print( mesh.triangles.Length );
    structSize = 1;
    count = mesh.triangles.Length;
  }

  public virtual void CreateBuffer(){

    values = mesh.triangles;
    _buffer = new ComputeBuffer( count , sizeof(float) );
    _buffer.SetData(values);

  }

}
