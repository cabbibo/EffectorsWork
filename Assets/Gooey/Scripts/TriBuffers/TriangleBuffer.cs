using UnityEngine;
using System.Collections;


public class TriangleBuffer : Buffer {

  public Mesh mesh;

  public int[] values;



  public virtual void GetMesh(){

    if( mesh == null){
      mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
    }

  }

  public override void BeforeCreateBuffer(){
    GetMesh();
    print( mesh );
    print( mesh.triangles.Length );
    structSize = 1;
    count = mesh.triangles.Length;
  }

  public override void CreateBuffer(){

    values = mesh.triangles;
    _buffer = new ComputeBuffer( count , sizeof(float) ); 
    _buffer.SetData(values);
    
  }

}