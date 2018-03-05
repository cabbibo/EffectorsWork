using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigureSkinnedMesh : MonoBehaviour {


	  public GameObject bonePrefab;
	  public GameObject connectionPrefab;
	  public GameObject Base;

	  //public C

	  public GameObject BaseConnection;

	  public SkinnedMeshRenderer skinnedMesh;

	  public Transform[] bones;
	  public List<GameObject> PhysicsSkeleton;


	  private int recursionCount = 0;

		// Use this for initialization
		void Start () {

	    skinnedMesh = GetComponent<SkinnedMeshRenderer>();

	    bones = skinnedMesh.bones;

	    Transform baseBone = bones[0];
	    
	    Vector3 dir = baseBone.position - Base.transform.position;

	  	Base.GetComponent<ConnectionLinker>().Length = dir.magnitude;

	  	GameObject connection = CreateConnection( baseBone.transform , Base.transform , Base , true );
	    //bl.Connection = connection;

	    BaseConnection = connection;

	    //connection.transform.parent = Base.transform;

	    recurseDemBones( baseBone , connection , 0 );


	  }

	  void recurseDemBones( Transform bone , GameObject grandDadBone , int recursionCount ){


			if( bone.childCount > 0 ){


				GameObject connection = new GameObject();//Instantiate( connectionPrefab , new Vector3(0,0,0) , Quaternion.identity ); 					
				
				bool found = false;
				foreach (Transform child in bone ){
					if( child.gameObject.tag == "bone"){
						Destroy( connection );
						connection = CreateConnection( child , bone , grandDadBone , false );	 

						found = true;
						break;
					}
				}

				if( found == false ){
					Debug.Log("NOOOO");
					connection =Instantiate( connectionPrefab , new Vector3(0,0,0) , Quaternion.identity ); 
				}

				int i = 0;
				foreach (Transform child in bone ){
					if( child.gameObject.tag == "bone"){
						connection.GetComponent<ConnectionLinker>().Kiddies.Add( child );	
						recurseDemBones( child , connection , recursionCount + 1 );
					}
		  	}

			}





	  	
	  	

	  }


	  GameObject CreateConnection( Transform kid , Transform dad, GameObject grandDadBone , bool first  ){

	  	Vector3 dir = kid.position - dad.position;
	  	Vector3 fPos = dad.position;

	  	//fPos = new Vector3( 0,0,0);
	  	GameObject connection = Instantiate( connectionPrefab , fPos , dad.rotation );

			ConnectionLinker dadCL = grandDadBone.GetComponent<ConnectionLinker>();

	  	connection.GetComponent<ConfigurableJoint>().connectedBody = grandDadBone.GetComponent<Rigidbody>();

	  	//print( dadCL.Length );

	  	if( first == true ){
	  		connection.GetComponent<ConfigurableJoint>().connectedAnchor = new Vector3(0 ,0 , 0 );
	  		connection.GetComponent<Rigidbody>().isKinematic = true;
	  	}else{
				connection.GetComponent<ConfigurableJoint>().connectedAnchor = new Vector3(0 ,2 , 0 );
	  	}
	  	connection.GetComponent<ConfigurableJoint>().anchor = new Vector3(0 , 0 , 0 );

//	  	print( "MAg");
//	  	print( dir.magnitude );

	  	//print( "connected");
	  	//print( dadCL.Length );
	  	connection.transform.localScale = new Vector3(dir.magnitude * .5f , dir.magnitude * .5f , dir.magnitude * .5f );
	 
	 		connection.GetComponent<ConnectionLinker>().Length = dir.magnitude;
	 		dad.gameObject.GetComponent<BoneLinker>().DadConnection = grandDadBone;
	 		dad.gameObject.GetComponent<BoneLinker>().Connection = connection;

//	 		print( grandDadBone );

	 	
	 
	 		//connection.transform.parent = dad;
	 		//connection.transform.rotation = dad.rotation;//Quaternion.LookRotation(dir);

	 		return connection;
	  	
	  }

		
		// Update is called once per frame
		void FixedUpdate () {


			for( int i = 0; i < bones.Length; i++ ){
				GameObject c = bones[i].GetComponent<BoneLinker>().Connection;

				//Vector3 fPos = new Vector3(0, -c.GetComponent<ConnectionLinker>().Length * .5f , 0 );
				bones[i].position = c.transform.position;// + c.transform.TransformVector(fPos);
				bones[i].rotation = c.transform.rotation;
			}

			
		}


	}
