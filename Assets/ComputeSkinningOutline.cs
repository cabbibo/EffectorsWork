using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeSkinningOutline : MonoBehaviour {

  public ComputeSkinning meshToSkin;
  public Material material;
  public Color color;
  public float outline;


  private Material mat;

	// Use this for initialization
	void Start () {
		mat = new Material( material );
    if( meshToSkin == null ){ meshToSkin = this.GetComponent<ComputeSkinning>(); }
	}
	

  void OnRenderObject(){

      mat.SetPass(0);

      mat.SetBuffer("_vertBuffer", meshToSkin.vertBuffer._buffer);
      mat.SetBuffer("_triBuffer", meshToSkin.triBuffer._buffer);
      mat.SetColor("_Color", color);
      mat.SetFloat("_Outline", outline);
    
      Graphics.DrawProcedural(MeshTopology.Triangles, meshToSkin.triBuffer.count );

  }

}
