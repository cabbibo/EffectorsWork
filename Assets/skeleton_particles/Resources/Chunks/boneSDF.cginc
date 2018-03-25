#include "Chunks/noise.cginc"

float sdCapsule( float3 p, float3 a, float3 b, float r )
{
    float3 pa = p - a, ba = b - a;
    float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
    return length( pa - ba*h ) - r;
}

float sdSphere( float3 p, float s )
{
  return length( p ) - s;
}

float opU( float d1, float d2 )
{
    return min(d1,d2);
}

// checks to see which intersection is closer
// and makes the y of the vec2 be the proper id
float2 opU( float2 d1, float2 d2 ){
    
  return (d1.x<d2.x) ? d1 : d2;
    
}

float smin( float a, float b)
{
    float k = 0.77521;
    float h = clamp( 0.5+0.5*(b-a)/k, 0.0, 1.0 );
    return lerp( b, a, h ) - k*h*(1.0-h);
}

float smoothU( float d1, float d2)
{
    return smin( d1, d2 );
}



float2 map( float3 position ){

  float2 d = float2(100000,-1);
  for( int i = 0; i < _NumBones; i++ ){
    Bone b = _boneBuffer[i];
    float3 sP = b.start.xyz;
    float3 eP = b.end.xyz;

    d = opU(d,float2(sdCapsule(position,sP,eP,length( sP - eP) * .6),i));

  }

  d.x += noise( position  * 10. )  * .1;

  return d;

}


// Calculates the normal by taking a very small distance,
// remapping the function, and getting normal for that
float3 calcNormal( in float3 pos ){
    
  float3 eps = float3( 0.001, 0.0, 0.0 );
  float3 nor = float3(
      map(pos+eps.xyy).x - map(pos-eps.xyy).x,
      map(pos+eps.yxy).x - map(pos-eps.yxy).x,
      map(pos+eps.yyx).x - map(pos-eps.yyx).x );
  return normalize(nor);
    
}

