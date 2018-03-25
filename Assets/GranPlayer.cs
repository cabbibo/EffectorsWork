using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GranPlayer : MonoBehaviour {


public float pitch;
public float offset;
public float amount;


    public AudioSource[] buffer;
    public AudioMixerGroup mixer;
    public AudioClip[] seeds;
    public int maxSize;
    private int current;
    // Use this for initialization
    void Start () {

        current = 0;

        buffer= new AudioSource[maxSize];

        for( int i = 0; i < maxSize; i++ ){
                buffer[i] = gameObject.AddComponent<AudioSource>();
                buffer[i].playOnAwake= false;
                buffer[i].outputAudioMixerGroup = mixer;
        }

    }

    public void Play( float speed , float volume){

//        Debug.Log( bs.regular );
///              print("hi");
      buffer[current].pitch = speed;
        buffer[current].clip=seeds[Random.Range(0,seeds.Length)];
        buffer[current].time = offset * Random.Range(.9f, 1.1f) + .25f;//(20 *(bs.regular+1)+10);
        buffer[current].volume = volume;
      buffer[current].Play();
      current = (current+1) % maxSize;

    }

    void Gran(){

        float r = Random.Range(-1f,3f);

                Play(pitch + r * .1f,.4f);

    }

    // Update is called once per frame
    void FixedUpdate () {
     float r = Random.Range( 0, 1f);

     if( r < amount * 2f ){
      //  Debug.Log("hi");
        Gran();
     }

     for( int i = 0; i< maxSize; i++){
        buffer[i].volume -= .003f;
     }
    }
}
