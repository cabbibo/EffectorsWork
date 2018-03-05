using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsUpdateBuffer : Buffer {

  public ComputeShader physics;

  public int numThreads;
  public int numGroups;
  public int _kernel;

	// Use this for initialization
	public override void OnEnable() {
		DoBuffers();
	}

  public void GetKernelAndGroups(){
    numGroups = (count+(numThreads-1))/numThreads;
    _kernel = physics.FindKernel("CSMain");
  }
	

   // Update is called once per frame
  void FixedUpdate () {

    physics.SetInt( "_Count" , count );
    physics.SetFloat( "_DeltaTime" , Time.deltaTime );
    physics.SetFloat( "_Time" , Time.time );

    DoPhysics();

  }

  public virtual void DoPhysics(){}

}
