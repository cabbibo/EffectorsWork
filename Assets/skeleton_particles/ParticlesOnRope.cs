using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesOnRope : MonoBehaviour {

	public int numberParticles;
	private RopeConnector rope;

	public ComputeShader physics;
	public Material particleMat;

	private int _kernel;
	public ComputeBuffer _buffer;


	public controllerInfo cInfo1;
	public controllerInfo cInfo2;

	struct Vert{
	    public Vector3 pos;
	    public Vector3 vel;
	    public Vector2 uv;
	    public float life;
	    public Vector3 debug;
	};

	private int vertStructSize = 3+3+2+1+3;

	public int nrThreads = 256;
	private int nrGroups;


	private float[] vertValues;

	// Use this for initialization
	void Start () {

		rope = GetComponent<RopeConnector>();

		particleMat = new Material( particleMat);


		_buffer = new ComputeBuffer( numberParticles , vertStructSize * sizeof(float));
		vertValues = new float[ numberParticles * vertStructSize ];


		//print( nrGroups);
		nrGroups = (numberParticles+(nrThreads-1))/nrThreads;



		int index = 0;


		for( int i = 0; i < numberParticles; i++ ){



			// positions
			vertValues[ index++ ] = 0;
			vertValues[ index++ ] = 0;
			vertValues[ index++ ] = 0;

			// vel
			vertValues[ index++ ] = 0;
			vertValues[ index++ ] = 0;
			vertValues[ index++ ] = 0;



			// uv
			vertValues[ index++ ] = i;
			vertValues[ index++ ] = (float)i/(float)numberParticles;

			// life
			vertValues[ index++ ] = -1 + Random.Range(0, .99f);

			// debug
			vertValues[ index++ ] = 1;
			vertValues[ index++ ] = 0;
			vertValues[ index++ ] = 0;

		}

		_buffer.SetData(vertValues);


	}

	void OnRenderObject(){

		particleMat.SetPass(0);

		particleMat.SetInt( "_numberParticles" , numberParticles );
		particleMat.SetBuffer("_vertBuffer", _buffer );

  		Graphics.DrawProcedural( MeshTopology.Triangles , numberParticles * 6 );

	}



	// Update is called once per frame
	void FixedUpdate () {
		Dispatch();
	}

	void Dispatch(){

		physics.SetInt( "_Reset" , 0 );

		physics.SetFloat( "_DeltaTime" , Time.deltaTime );
		physics.SetFloat( "_Time" , Time.time );

		physics.SetBuffer(0 , "vertBuffer"     , _buffer );
		physics.SetBuffer(0 , "ropeBuffer"     , rope._buffer );

				physics.SetVector( "_Hand1", cInfo1.position);
		physics.SetVector( "_Hand2", cInfo2.position);

		physics.SetFloat( "_Trigger1", cInfo1.triggerVal);
		physics.SetFloat( "_Trigger2", cInfo2.triggerVal);



		physics.SetInt( "_NumVerts" , numberParticles );
		physics.SetInt( "_NumPointsOnRope" , rope.numberPoints );
		physics.SetInt( "_NumGroups", nrGroups );

		physics.Dispatch(0, nrGroups,1,1 );

	}
}
