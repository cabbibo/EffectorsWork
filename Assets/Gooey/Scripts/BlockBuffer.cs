
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBuffer : MonoBehaviour {


  public ComputeBuffer _buffer;
  public int numberBlocks;

  public GameObject[] Blocks;
  private float[] inValues;



  // Use this for initialization
  void Start () {
    numberBlocks = Blocks.Length;
    CreateBuffer();   
  }


  void CreateBuffer(){

    _buffer = new ComputeBuffer( Blocks.Length, 32 * sizeof(float));
    inValues = new float[ Blocks.Length * 32 ];

  }

  void AssignTransBuffer(Transform t, int id  ){

    int index = id * 32;
    Matrix4x4 m = t.localToWorldMatrix;

    for( int i = 0; i < 16; i++ ){
      int x = i % 4;
      int y = (int) Mathf.Floor(i / 4);
      inValues[index + i] = m[x,y];
    }

    m = t.worldToLocalMatrix;

    for( int i = 0; i < 16; i++ ){
      int x = i % 4;
      int y = (int) Mathf.Floor(i / 4);
      inValues[index + i + 16] = m[x,y];
    }


  }   
  
  // Update is called once per frame
  void FixedUpdate () {

    int index = 0;
    for( int i = 0; i < Blocks.Length; i++ ){
      AssignTransBuffer( Blocks[i].transform, i );
    }

    _buffer.SetData(inValues);
    
  }
}