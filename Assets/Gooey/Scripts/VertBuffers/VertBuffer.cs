
using UnityEngine;
using System.Collections;


public class VertBuffer : Buffer {

  public Mesh mesh;

  public Vector3[] vertices;
  public Vector3[] normals;
  public Vector2[] uvs;

  public virtual void GetMesh(){

    if( mesh == null){
        print("hi");
      mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
    }

  }

}
