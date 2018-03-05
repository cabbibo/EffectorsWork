using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatingBuffer : Buffer {

	// Use this for initialization
	void FixedUpdate() {
	  UpdateBuffer();
    _buffer.SetData(values);
	}

  public virtual void UpdateBuffer(){

  }
}
