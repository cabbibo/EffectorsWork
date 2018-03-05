using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HelperFunctions{

  public static Vector3 GetRandomPointInTriangle( int seed, Vector3 v1 , Vector3 v2 , Vector3 v3 ){
   
    /* Triangle verts called a, b, c */

    Random.InitState(seed* 14145);
    float r1 = Random.value;

    Random.InitState(seed* 19247);
    float r2 = Random.value;
    //float r3 = Random.value;

    return (1 - Mathf.Sqrt(r1)) * v1 + (Mathf.Sqrt(r1) * (1 - r2)) * v2 + (Mathf.Sqrt(r1) * r2) * v3;
     
    ///return (r1 * v1 + r2 * v2 + r3 * v3) / (r1 + r2 + r3);
  }

  public static float AreaOfTriangle( Vector3 v1 , Vector3 v2 , Vector3 v3 ){
     Vector3 v = Vector3.Cross(v1-v2, v1-v3);
     float area = v.magnitude * 0.5f;
     return area;
  }


  public static Vector3 ToV3( Vector4 parent)
  {
     return new Vector3(parent.x, parent.y, parent.z);
  }

  public static float getRandomFloatFromSeed( int seed ){
    Random.InitState(seed);
    return Random.value;
  }

  public static int getTri(float randomVal, float[] triAreas){


    int triID = 0;
    float totalTest = 0;
    for( int i = 0; i < triAreas.Length; i++ ){

      totalTest += triAreas[i];
      if( randomVal <= totalTest){
        triID = i;
        break;
      }

    }

    return triID;

  }


  public static void getTriAreas( int[] triValues, Vector3[] vertices , out float[] triAreas , out float totalArea ){

    totalArea = 0;
    triAreas = new float[triValues.Length/3 ];
    
    for( int i = 0; i < triValues.Length/3; i++ ){
      int tri0 = i * 3;
      int tri1 = tri0 + 1;
      int tri2 = tri0 + 2;
      tri0 = triValues[tri0];
      tri1 = triValues[tri1];
      tri2 = triValues[tri2];
      float area = HelperFunctions.AreaOfTriangle( vertices[tri0] , vertices[tri1] , vertices[tri2] );
      triAreas[i] = area;
      totalArea += area;
    }

    for( int i = 0; i < triAreas.Length; i++ ){
      triAreas[i] /= totalArea;
    }

  }


  public struct Point{

    public Vector3 uv;
    public Vector3 pos;
    public Vector3 nor;
  
    public Vector3 triIDs;
    public Vector3 triWeights;

  }

  public static Point getVertInfo( int seed , float randomVal , float[] triAreas , int[] triValues,  VertBuffer vertBuffer ){

    int tri0 = 3 * HelperFunctions.getTri( randomVal , triAreas );
    int tri1 = tri0 + 1;
    int tri2 = tri0 + 2;

    tri0 = triValues[tri0];
    tri1 = triValues[tri1];
    tri2 = triValues[tri2];

    Vector3 pos = HelperFunctions.GetRandomPointInTriangle( seed , vertBuffer.vertices[ tri0 ] , vertBuffer.vertices[ tri1 ]  , vertBuffer.vertices[ tri2 ]  );
    
    float a0 = HelperFunctions.AreaOfTriangle( pos , vertBuffer.vertices[tri1] , vertBuffer.vertices[tri2] );
    float a1 = HelperFunctions.AreaOfTriangle( pos , vertBuffer.vertices[tri0] , vertBuffer.vertices[tri2] );
    float a2 = HelperFunctions.AreaOfTriangle( pos , vertBuffer.vertices[tri0] , vertBuffer.vertices[tri1] );
    float aTotal = a0 + a1 + a2;

    float p0 = a0 / aTotal;
    float p1 = a1 / aTotal;
    float p2 = a2 / aTotal;




    Vector3 nor     = vertBuffer.normals[tri0]  * p0 + vertBuffer.normals[tri1]  * p1 + vertBuffer.normals[tri2]  * p2;
    nor = nor.normalized;
//          Vector3 color   = tri0.color  * p0 + tri1.color  * p1 + tri2.color  * p2;

    Vector2 uv      = vertBuffer.uvs[tri0] * p0 + vertBuffer.uvs[tri1] * p1 + vertBuffer.uvs[tri2] * p2;


    Point p = new Point();

    p.uv = uv;
    p.nor = nor;
    p.triWeights = new Vector3( p0 , p1 , p2 );
    p.triIDs = new Vector3( tri0 , tri1 , tri2 );

    return p;

  }

}
