using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeStarsAndConnections : MonoBehaviour {


    public GranPlayer player;
    public int numberStars;
    public int numberRopes;

    public float starSize;
    public float size;
    public Vector3 center;

    public GameObject starPrefab;
    public GameObject ropePrefab;

    public GameObject[] stars;
    public Rigidbody[] bodies;
    public GameObject[] ropes;


	// Use this for initialization
	void Start () {
        stars = new GameObject[numberStars];
        bodies = new Rigidbody[numberStars];
        ropes = new GameObject[numberRopes];
        for( int i = 0; i < numberStars; i++ ){
            Vector3 position = Random.insideUnitSphere + new Vector3(0,1,0);
            stars[i] = Instantiate( starPrefab , position , Quaternion.identity);
            stars[i].transform.localScale = new Vector3( starSize , starSize , starSize);
            bodies[i] = stars[i].GetComponent<Rigidbody>();
        }


        /*for( int i = 0; i < numberRopes; i++){
         ropes[i] = Instantiate( ropePrefab , Vector3.zero , Quaternion.identity);

         GameObject From = stars[Random.Range(0,numberStars)];
         GameObject To =  stars[Random.Range(0,numberStars)];
         ropes[i].GetComponent<RopeConnector>().From = From;
         ropes[i].GetComponent<RopeConnector>().To = To;

         From.GetComponent<ConnectionList>().fromList.Add(ropes[i].GetComponent<RopeConnector>());
         To.GetComponent<ConnectionList>().toList.Add(ropes[i].GetComponent<RopeConnector>());

        }*/

        int index = 0;
        for( int i = 0; i < numberStars-1; i++){
            for( int j = i+1; j < numberStars; j++ ){

         ropes[index] = Instantiate( ropePrefab , Vector3.zero , Quaternion.identity);

         GameObject From = stars[i];
         GameObject To =  stars[j];
         ropes[index].GetComponent<RopeConnector>().From = From;
         ropes[index].GetComponent<RopeConnector>().To = To;

         From.GetComponent<ConnectionList>().fromList.Add(ropes[i].GetComponent<RopeConnector>());
         To.GetComponent<ConnectionList>().toList.Add(ropes[i].GetComponent<RopeConnector>());
         index += 1;
     }
        }

        numberRopes = index;

        Destroy(ropePrefab);
        Destroy(starPrefab);

	}

	// Update is called once per frame
	void Update () {

        for( int i = 0; i < numberStars; i++ ){

            Rigidbody star1 = bodies[i];
 Vector3 dif = star1.position - center;

        star1.AddForce( -dif*4 );



            for( int j = 0; j < numberStars; j++){
                Rigidbody star2 = bodies[j];

                dif = star1.position - star2.position;

                star1.AddForce( .8f * -dif.normalized * ( dif.magnitude -size));
                star2.AddForce( .8f * dif.normalized * ( dif.magnitude -size));
            }
        }

        float m = 1000;
        float id = 1000;
        for( int i = 0; i < numberRopes; i++ ){
            RopeConnector r = ropes[i].GetComponent<RopeConnector>();

            if( r.finalFloats[0] < m){
                m = r.finalFloats[0];
                id = r.finalFloats[1];

            }
        }

        if( m < 1000 ){
            player.amount = .0003f / m;
            player.pitch = .0003f / m;
            player.offset = .01f / m;
        }

	}
}
