using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BasicVertBuffer : VertBuffer {

  public bool rotateMesh;
  public bool scaleMesh;
  public bool translateMesh;

  struct Vert{

    public float used;
    public Vector3 pos;
    public Vector3 vel;
    public Vector3 nor;
    public Vector2 uv;

    public Vector3 targetPos;

    public Vector3 debug;

  };


  public virtual void GetMesh(){

    if( mesh == null){
      mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
    }

  }

  public override void BeforeCreateBuffer(){
    GetMesh();
    vertices = mesh.vertices;
    count = vertices.Length;
    structSize = 1 + 3 + 3 + 3+2 + 3+ 3;
  }


  public override void CreateBuffer(){


    vertices = mesh.vertices;
    uvs = mesh.uv;
    normals = mesh.normals;


    _buffer = new ComputeBuffer( count , structSize * sizeof(float) );
    values = new float[ structSize * count ];

    int index = 0;


    Vector3 t1;
    Vector3 t2;
    for( int i = 0; i < count; i++ ){


      if( scaleMesh == true ){
        t1 = transform.TransformVector( vertices[i] );
        t2 = transform.TransformVector( normals[i]);
      }else{
        t1 = vertices[i];
        t2 = normals[i];
      }



      if( rotateMesh == true ){
        t1 = transform.TransformDirection( t1 );
        t2 = transform.TransformDirection( t2 );
      }


      if( translateMesh == true ){
        t1 = transform.TransformPoint( t1 );
        t2 = transform.TransformPoint( t2 );
      }

      //     t1 = vertices[i];
      //  t2 = normals[i];

      // used
      values[ index++ ] = 1;

      // positions
      values[ index++ ] = t1.x * .99f;
      values[ index++ ] = t1.y * .99f;
      values[ index++ ] = t1.z * .99f;

      // vel
      values[ index++ ] = 0;
      values[ index++ ] = 0;
      values[ index++ ] = 0;

      // normals
      values[ index++ ] = t2.x;
      values[ index++ ] = t2.y;
      values[ index++ ] = t2.z;

      // uvs
      values[ index++ ] = uvs[i].x;
      values[ index++ ] = uvs[i].y;


      // target pos
      values[ index++ ] = t1.x;
      values[ index++ ] = t1.y;
      values[ index++ ] = t1.z;


      // Debug
      values[ index++ ] = 1;
      values[ index++ ] = 0;
      values[ index++ ] = 0;

    }

    _buffer.SetData(values);


  }



}
