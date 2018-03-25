using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GooMesh : MonoBehaviour {

public GranPlayer player;

  public BasicVertBuffer vertBuffer;
  public BasicTriangleBuffer triBuffer;

  public ComputeShader computeShader;
  public ComputeShader gatherShader;

  public Material material;

  public controllerInfo cInfo1;
  public controllerInfo cInfo2;

  public bool calcOut;
  public ComputeBuffer outBuffer;
  public float[] outFloats;


  public ComputeBuffer finalBuffer;
  public float[] finalFloats;
  //public int vertCount;

  public bool showMesh = true;


  private int numThreads = 256;
  private int numGroups;


  private int _kernel;
  private int vertCount;

  public int Set = 0;

  private Material mat;

private float[] transformValues ;
  // Use this for initialization
  void Start () {

    outBuffer = new ComputeBuffer(numThreads, 4 * sizeof(float));
    outFloats = new float[numThreads * 4];

    finalBuffer = new ComputeBuffer(1, 4 * sizeof(float));
    finalFloats = new float[4];





    transformValues = new float[16];

    if( vertBuffer == null ){ vertBuffer = this.GetComponent<BasicVertBuffer>(); }
    if( triBuffer == null ){ triBuffer = this.GetComponent<BasicTriangleBuffer>(); }

    vertCount = vertBuffer.count;
    _kernel = computeShader.FindKernel("CSMain");

    numGroups = (vertCount+(numThreads-1))/numThreads;

    SetBegin();

    mat = new Material( material );

  }


  void OnRenderObject(){

    if( showMesh == true){
       /// print("SHOW FIRE");
      mat.SetPass(0);

      mat.SetBuffer("_vertBuffer", vertBuffer._buffer);
      mat.SetBuffer("_triBuffer", triBuffer._buffer);

      mat.SetInt("_NumTris" , triBuffer.count);
      mat.SetInt("_NumVerts" , vertBuffer.count);

      Graphics.DrawProcedural(MeshTopology.Triangles, triBuffer.count *2);
    }

  }


  void SetBindPoses(){

  }

  void SetBegin(){

    Set = 1;
    Dispatch();
    Set = 0;


  }

  void Dispatch(){

    computeShader.SetInt( "_Set" , Set );
    computeShader.SetInt( "_NumVerts" , vertBuffer.count );

    Matrix4x4 m = transform.localToWorldMatrix;
     for( int i = 0; i < 16; i++ ){
      int x = i % 4;
      int y = (int) Mathf.Floor(i / 4);
      transformValues[i] = m[x,y];
      ///   print( transformValues[i]);
    }

            computeShader.SetVector( "_Hand1", cInfo1.interactionTip.transform.position);
        computeShader.SetVector( "_Hand2",cInfo2.interactionTip.transform.position);

        computeShader.SetFloat( "_Trigger1", cInfo1.triggerVal);
        computeShader.SetFloat( "_Trigger2", cInfo2.triggerVal);






    computeShader.SetFloats( "_transform" , transformValues );

    if( calcOut == true ){

        computeShader.SetBuffer( _kernel , "outBuffer"     , outBuffer );
    }

    if( vertBuffer._buffer != null ){

        computeShader.SetBuffer( _kernel , "vertBuffer"     , vertBuffer._buffer );
        computeShader.Dispatch( _kernel, numGroups,1,1 );
      }


    if( calcOut == true ){


        gatherShader.SetBuffer( 0 , "floatBuffer" , outBuffer );
      gatherShader.SetBuffer( 0 , "gatherBuffer" , finalBuffer );

      gatherShader.Dispatch( 0, 1 , 1 , 1 );

      finalBuffer.GetData(finalFloats);

//      print( finalFloats[0]);

      Vector3 f = new Vector3( finalFloats[0] , finalFloats[1],finalFloats[2]);
      player.amount = f.magnitude * .3f;
      player.pitch  = f.magnitude * 2 + .2f;
    }



  }

  // Update is called once per frame
  void FixedUpdate () {

    Dispatch();


  }
}
