using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsAndRenderBuffer : PhysicsUpdateBuffer {

	
  public Material material;

  public override void OnEnable(){

    DoBuffers();
    GetKernelAndGroups();

    // Make a copy of our material
    material = new Material( material );

  }

  // Use this for initialization
  void OnRenderObject() {
    RenderBuffer();
  }

  public virtual void RenderBuffer(){}
  
}
