	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class PhysicsifySkinnedMesh : MonoBehaviour {

	  public GameObject bonePrefab;
	  public GameObject connectionPrefab;
	  public GameObject Base;

	  //public C

	  public GameObject BaseConnection;

	  public SkinnedMeshRenderer skinnedMesh;

	  public Transform[] bones;
	  public List<GameObject> PhysicsSkeleton;


		// Use this for initialization
		void Start () {

	    skinnedMesh = GetComponent<SkinnedMeshRenderer>();

	    bones = skinnedMesh.bones;

	    Transform baseBone = bones[0];
	    
	    Vector3 dir = baseBone.position - Base.transform.position;
	  	Vector3 fPos = Base.transform.position;// + dir * .5f;

	  	GameObject connection = Instantiate( connectionPrefab , fPos , Quaternion.identity );

	  	connection.GetComponent<SpringJoint>().connectedBody = Base.GetComponent<Rigidbody>();
	  	connection.GetComponent<SpringJoint>().connectedAnchor = new Vector3(0 ,0, 0 );
	  	connection.GetComponent<SpringJoint>().anchor = new Vector3(0 , 0 , 0 );
	  	connection.GetComponent<Collider>().enabled = false;

	  	BoneLinker bl = baseBone.gameObject.GetComponent<BoneLinker>();

			connection.transform.localScale = new Vector3( bl.BoneWidth , dir.magnitude , bl.BoneWidth  );

	  	connection.GetComponent<ConnectionLinker>().Length = dir.magnitude;
	 		//connection.GetComponent<ConnectionLinker>().Kid = baseBone;
	 		//connection.GetComponent<ConnectionLinker>().Dad = Base.transform;

	 		
	    bl.Connection = connection;

	    BaseConnection = connection;

	    recurseDemBones( baseBone , connection );


	  }

	  void recurseDemBones( Transform bone , GameObject dadConnection ){

	  	BoneLinker dadBL = bone.gameObject.GetComponent<BoneLinker>();
	  	Debug.Log( "Recurse");
			Debug.Log( bone );



			if( bone.childCount > 0 ){

				GameObject connection = CreateConnection( bone.GetChild(0) , bone , dadConnection );	
				dadBL.Connection = connection;


				foreach (Transform child in bone ){
					
					connection.GetComponent<ConnectionLinker>().Kiddies.Add( child );

		  		//BoneLinker bl = child.gameObject.AddComponent<BoneLinker>();
		  		
					recurseDemBones( child , connection );

		  	}

			}

	  	
	  	

	  }


	  GameObject CreateConnection( Transform kid , Transform dad, GameObject dadConnection ){

	  	Vector3 dir = kid.position - dad.position;
	  	Vector3 fPos = kid.position + dir * .5f;

	  	GameObject connection = Instantiate( connectionPrefab , fPos , Quaternion.identity );

			ConnectionLinker dadCL = dadConnection.GetComponent<ConnectionLinker>();
			BoneLinker dadBL = dad.GetComponent<BoneLinker>();

	  	connection.GetComponent<SpringJoint>().connectedBody = dadConnection.GetComponent<Rigidbody>();
	  	connection.GetComponent<SpringJoint>().connectedAnchor = new Vector3(0 , dadCL.Length * .5f, 0 );
	  	connection.GetComponent<SpringJoint>().anchor = new Vector3(0 , dir.magnitude * -.5f , 0 );

	  	connection.transform.localScale = new Vector3( dadBL.BoneWidth , dir.magnitude * .5f , dadBL.BoneWidth );
	 
	 		connection.GetComponent<ConnectionLinker>().Length = dir.magnitude;

	 		return connection;
	  	
	  }


	  /*void physicsDemBones(){

	    PhysicsSkeleton = new GameObject[ bones.Length ];

	    for( int i = 0; i < bones.Length; i++ ){
	      Transform b = bones[i];

	      GameObject bone = Instantiate( bonePrefab , new Vector3(0, 0, 0), Quaternion.identity);
	      
	      bone.transform.position = b.position;
	      bone.transform.rotation = b.rotation;
	      bone.transform.localScale = b.localScale * .01f;

	      PhysicsSkeleton[i] = bone;
	      //bone.transform.parent = b;

	      for( int j = 0; j < bones.Length; j++ ){

	        if( b.transform.parent == bones[j]){
	          bone.GetComponent<SpringJoint>().connectedBody = PhysicsSkeleton[j].GetComponent<Rigidbody>();
	        }
	      }

	      print( b.transform.parent );


	//      print( g );


	    }
	  }*/

	  /*void recursiveUpdate( ConnectionLinker bone ){
	  	foreach( Transform child in bone ){

	  	}
	  }*/
		
		// Update is called once per frame
		void FixedUpdate () {


			for( int i = 0; i < bones.Length; i++ ){
				GameObject c = bones[i].GetComponent<BoneLinker>().Connection;

				Vector3 fPos = new Vector3(0, -c.GetComponent<ConnectionLinker>().Length * .5f , 0 );
				bones[i].position = c.transform.position + c.transform.TransformVector(fPos);
				bones[i].rotation = c.transform.rotation;
			}

			
		}
	}
