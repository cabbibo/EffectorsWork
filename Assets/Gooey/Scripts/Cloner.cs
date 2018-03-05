using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cloner: PhysicsAndRenderBuffer {


  public GameObject cloneOnTo;
  public GameObject toClone;

  public Effector[] effectors;

  // Mesh we want to spawn
  private VertBuffer cloneVertBuffer;
  private TriangleBuffer cloneTriBuffer;


  // Mesh we are cloning onto
  public VertBuffer vertBuffer;
  public TriangleBuffer triBuffer;


  private float[] triAreas;

  struct Basis{

    public float id;

    public Matrix4x4 basis;

    public Vector2 uv;
  
    public Vector3 triIDs;
    public Vector3 triWeights;
    public Vector3 debug;

  }


  // Use this for initialization
  public override void BeforeCreateBuffer() {

    structSize = 1+16+2+3+3+3;

    if( vertBuffer == null ){ vertBuffer = gameObject.GetComponent<VertBuffer>(); }
    if( triBuffer == null ){ triBuffer = gameObject.GetComponent<TriangleBuffer>(); }

    if( cloneVertBuffer == null ){ cloneVertBuffer = toClone.GetComponent<VertBuffer>(); }
    if( cloneTriBuffer == null ){ cloneTriBuffer = toClone.GetComponent<TriangleBuffer>(); }

  }
  
  public override void CreateBuffer(){

    _buffer = new ComputeBuffer( count , structSize * sizeof(float));
    values = new float[ count * structSize ];

    // Used for assigning to our buffer;
    int index = 0;

    float totalArea = 0;

    HelperFunctions.getTriAreas( triBuffer.values , vertBuffer.vertices , out triAreas , out totalArea );


    for (int i = 0; i < count; i++ ){

          int id = i; 

          float randomVal = Random.value;


          HelperFunctions.Point p = HelperFunctions.getVertInfo( id , randomVal, triAreas, triBuffer.values , vertBuffer );
         
          values[index++] = id;
          
          //transform
          for( int j = 0; j < 16; j++){
            values[index++] = 0;
          }

          // uv
          values[index++] = p.uv.x;
          values[index++] = p.uv.y;

          // triIDs
          values[index++] = p.triIDs.x;
          values[index++] = p.triIDs.y;
          values[index++] = p.triIDs.z;

          // triWeights
          values[index++] = p.triWeights.x;
          values[index++] = p.triWeights.y;
          values[index++] = p.triWeights.z;

                 // debug
          values[index++] = 1;
          values[index++] = 0;
          values[index++] = 0;
          
    }

    _buffer.SetData(values);
    

  }


  public override void RenderBuffer(){


      material.SetPass(0);

      material.SetInt( "_VertsPerMesh" , cloneTriBuffer.count );
      material.SetBuffer("_vertBuffer", cloneVertBuffer._buffer );
      material.SetBuffer("_triBuffer", cloneTriBuffer._buffer );
      material.SetBuffer("_basisBuffer", _buffer );

      Graphics.DrawProcedural(MeshTopology.Triangles ,count * cloneTriBuffer.count );

  }


  // Update is called once per frame
  void FixedUpdate () {
  
    physics.SetInt( "_Reset" , 0 );
    physics.SetInt( "_NumMeshes" , count );
    physics.SetFloat( "_DeltaTime" , Time.deltaTime );
    physics.SetFloat( "_Time" , Time.time );

    physics.SetBuffer( _kernel , "basisBuffer"     , _buffer );
    physics.SetBuffer( _kernel , "vertBuffer"     , vertBuffer._buffer );

    physics.Dispatch( _kernel, numGroups,1,1);

    for( int i = 0; i < effectors.Length; i++ ){
      effectors[i].UpdateStep( this );
    }

  }
}