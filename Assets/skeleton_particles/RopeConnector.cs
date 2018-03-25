using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeConnector : MonoBehaviour {


	public GameObject From;
	public GameObject To;

	public int numberPoints;
	public int smoothedNumberPoints;
	public int numberSides;

	public Transform hand1;
	public Transform hand2;

	private Rigidbody fromRB;
	private Rigidbody toRB;

	public controllerInfo cInfo1;
	public controllerInfo cInfo2;


	public float ropeLength;

	public ComputeShader physics;
	public ComputeShader constraintShader;
	public ComputeShader gatherShader;

	  public bool calcOut;
  public ComputeBuffer outBuffer;
  public float[] outFloats;


  public ComputeBuffer finalBuffer;
  public float[] finalFloats;

	private Vector3 oPosition;
	private Vector3 velocity;
	private Vector3 oVelocity;

	public float travelTime = 1;
	private float fireStartTime;
	private float firing;
	private float direction;
	private float power;
	private Vector4 fireInfo;
	private float pitch;


	//public Attractors attractors;

	struct Vert{
	    public Vector3 pos;
	    public Vector3 oPos;
	    public Vector3 ooPos;
	    public Vector3 nor;
	    public Vector2 uv;
	    public float id;
	    public float width;
	    public float cap; // 0 for no cap, 1 for start , 2 for end
	    public Vector3 extraInfo;
	    public Vector3 debug;
	};

	private int vertStructSize = 3+3+3+3+2+1+1+1+3+3;


	public int nrThreads = 256;
	private int nrGroups;

	//public int numVertsPerHair = 18;

	/*public float hairLength = .1f;
	public float distBetweenHairs { get { return hairLength / numVertsPerHair; }}*/



	private int _kernelPhysics;
	private int _kernelConstraint;
	public ComputeBuffer _buffer;

	private float[] vertValues;

	public Material pointMat;
	public Material lineMat;
	public Material meshMat;

	private Rigidbody rb;


	// Use this for initialization
	void OnEnable() {

		lineMat = new Material( lineMat);
		meshMat = new Material( meshMat);


		_buffer = new ComputeBuffer( numberPoints , vertStructSize * sizeof(float));
		vertValues = new float[ numberPoints * vertStructSize ];


    outBuffer = new ComputeBuffer(nrThreads, 4 * sizeof(float));
    outFloats = new float[nrThreads * 4];

    finalBuffer = new ComputeBuffer(1, 4 * sizeof(float));
    finalFloats = new float[4];




		//print( nrGroups);
		nrGroups = (numberPoints+(nrThreads-1))/nrThreads;


		int index = 0;


		for( int i = 0; i < numberPoints; i++ ){



			// positions
			vertValues[ index++ ] = 0;
			vertValues[ index++ ] = 0;
			vertValues[ index++ ] = 0;

			// vel
			vertValues[ index++ ] = 0;
			vertValues[ index++ ] = 0;
			vertValues[ index++ ] = 0;

			//acc
			vertValues[ index++ ] = 0;
			vertValues[ index++ ] = 0;
			vertValues[ index++ ] = 0;


			// normals
			vertValues[ index++ ] = 0;
			vertValues[ index++ ] = 0;
			vertValues[ index++ ] = 0;

			// uvs
			vertValues[ index++ ] = i;//(float)j/(float)numVertsPerHair;//(float)j/((float)numVertsPerHair);
			vertValues[ index++ ] = 0;//(float)i/(float)totalHairs;//(float)i/((float)totalHairs);




			vertValues[ index++ ] = i;
			vertValues[ index++ ] = 0;


			// CAP
			if( i == 0){
				vertValues[ index++ ] = 1; // Start Point
		    }else if( i == numberPoints - 1 ){
		    	vertValues[ index++ ] = 2; // End Point
	    	}else{
	    		vertValues[ index++ ] = 0;
	    	}


			// extraInfo
			vertValues[ index++ ] = ropeLength/(float)numberPoints;
			vertValues[ index++ ] = 0;
			vertValues[ index++ ] = 0;


			// Debug
			vertValues[ index++ ] = 1;
			vertValues[ index++ ] = 0;
			vertValues[ index++ ] = 0;



		}

		_buffer.SetData(vertValues);


		Reset();

	}

	void OnDisable(){
		if( _buffer != null ){ _buffer.Release(); }
	}

	void OnRenderObject(){

		//lineMat.SetPass(0);
	//
		//lineMat.SetInt( "_numberPoints" , numberPoints );
		//lineMat.SetBuffer("_vertBuffer", _buffer );
	//
  		//Graphics.DrawProcedural( MeshTopology.Lines , (numberPoints-1)*2 );


		meshMat.SetPass(0);
		meshMat.SetInt( "_NumberPoints" , numberPoints );
		meshMat.SetInt( "_TotalNumberPoints" , smoothedNumberPoints );
		meshMat.SetInt( "_NumSides" , numberSides );
		meshMat.SetBuffer("vertBuffer", _buffer );
		meshMat.SetVector( "_FireInfo" , fireInfo );

  		Graphics.DrawProcedural( MeshTopology.Triangles , (smoothedNumberPoints-1) * 6 * numberSides );




	}

	private void assignTransform(){

	    Matrix4x4 m = transform.localToWorldMatrix;

	    float[] matrixFloats = new float[]
			{
			m[0,0], m[1, 0], m[2, 0], m[3, 0],
			m[0,1], m[1, 1], m[2, 1], m[3, 1],
			m[0,2], m[1, 2], m[2, 2], m[3, 2],
			m[0,3], m[1, 3], m[2, 3], m[3, 3]
			};

	    physics.SetFloats("transform", matrixFloats);

	  }


	// Update is called once per frame
	void LateUpdate () {

		Dispatch();

		Vector3 dif = From.transform.position - To.transform.position;

		if( dif.magnitude > ropeLength * 1.5f){
			From.GetComponent<Rigidbody>().AddForce( -dif );
			To.GetComponent<Rigidbody>().AddForce( dif );
		}




	}

	public void Fire(float p,  float pit, float d){
		fireStartTime = Time.time;
		firing = 1;
		power = p;
		direction = d;
		pitch = pit;
	}

	public void FireNext(float p, float pit , float d){

		float nextPower = p - .4f;
		travelTime = nextPower;

		if( nextPower > 0 ){
			if( d == -1){
				From.GetComponent<ConnectionList>().Fire(nextPower, pit,d);
			}else{
				To.GetComponent<ConnectionList>().Fire(nextPower,pit,d);
			}
		}
	}

	public void Reset(){
		physics.SetInt( "_Reset" , 1 );
		Dispatch();
		physics.SetInt( "_Reset" , 0 );
	}

	void Dispatch(){

		assignTransform();

		if( firing == 1 ){
			float t = Time.time - fireStartTime;

			if( t > travelTime ){
				firing = 0;
				FireNext( power,pitch, direction);
			}

			float val = Mathf.Clamp(t/travelTime,0,1);

			if(direction == -1){
				val = 1-val;
			}

			fireInfo = new Vector4( val, power,firing,direction);
		}




    if( calcOut == true ){

        physics.SetBuffer( 0 , "outBuffer"     , outBuffer );
    }


		physics.SetFloat( "_DeltaTime" , Time.deltaTime );
		physics.SetFloat( "_Time" , Time.time );




		physics.SetVector( "_To" , To.transform.position );
		physics.SetVector( "_From" , From.transform.position );

		physics.SetVector( "_Hand1", hand1.position);
		physics.SetVector( "_Hand2", hand2.position);

		physics.SetFloat( "_Trigger1", cInfo1.triggerVal);
		physics.SetFloat( "_Trigger2", cInfo2.triggerVal);


		physics.SetBuffer(0 , "vertBuffer"     , _buffer );

		physics.SetInt( "_NumVerts" , numberPoints );
		physics.SetInt( "_NumGroups", nrGroups );

		physics.Dispatch(0, nrGroups,1,1 );


		constraintShader.SetInt( "_PassID" , 0 );
		constraintShader.SetInt( "_NumGroups", nrGroups );

		constraintShader.SetBuffer( 0 , "vertBuffer"     , _buffer );
		constraintShader.Dispatch( 0 , (int)Mathf.Ceil((float)nrGroups/2) , 1 , 1 );

		constraintShader.SetInt( "_PassID" , 1 );
		constraintShader.SetInt( "_NumGroups", nrGroups );

		constraintShader.SetBuffer( 0 , "vertBuffer"     , _buffer );
		constraintShader.Dispatch( 0 , (int)Mathf.Ceil((float)nrGroups/2) , 1 , 1 );

		if( calcOut == true ){


	      gatherShader.SetBuffer( 0 , "floatBuffer" , outBuffer );
	      gatherShader.SetBuffer( 0 , "gatherBuffer" , finalBuffer );

	      gatherShader.Dispatch( 0, 1 , 1 , 1 );

	      finalBuffer.GetData(finalFloats);

	    }



	}

}
