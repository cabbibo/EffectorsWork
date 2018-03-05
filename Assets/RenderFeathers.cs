using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderFeathers : MonoBehaviour {

  public HairOnVertBuffer hair;
  public Material material;

  public int totalVerts;



  // Use this for initialization
  void Start () {

    if( hair == null ){ 
      hair = gameObject.GetComponent<HairOnVertBuffer>();
    }

    material = new Material( material );

    totalVerts = hair.totalHairs * 3 * 2;
    
  }

  void OnRenderObject(){

    material.SetPass(0);

    material.SetBuffer("_vertBuffer", hair._buffer );
    material.SetInt("_NumVertsPerHair" , hair.numVertsPerHair );
    material.SetInt("_TotalHair" , hair.totalHairs * hair.numVertsPerHair  );
    material.SetInt("_TotalVerts" , totalVerts );

    Graphics.DrawProcedural(MeshTopology.Triangles, totalVerts );

  }


  
  // Update is called once per frame
  void Update () {
    
  }


}