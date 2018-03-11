// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Backdrop" {

  Properties {

    _NumberSteps( "Number Steps", Int ) = 3
    _TotalDepth( "Total Depth", Float ) = 0.16
    _NoiseSize( "Noise Size" , Float ) = 0.6
    _HueSize( "Hue Size" , Float ) = 2.45
    _BaseHue( "Base Hue" , Float ) = 1.67
    _Saturation( "Saturation" , Float ) = 1
    _NoiseSpeed( "Noise Speed" , Float ) = 0.8

  }

  SubShader {

    //Tags { "RenderType"="Transparent" "Queue" = "Transparent" }

    Tags { "RenderType"="Opaque" }
    LOD 200

    Pass {

      //Blend SrcAlpha OneMinusSrcAlpha // Alpha blending


      CGPROGRAM

      #pragma vertex vert
      #pragma fragment frag

      // Use shader model 3.0 target, to get nicer looking lighting
      #pragma target 3.0

      #include "UnityCG.cginc"


      uniform int _NumberSteps;
      uniform float _TotalDepth;

      uniform float _NoiseSize;
      uniform float _NoiseSpeed;

      uniform float _Saturation;

      uniform float _HueSize;
      uniform float _BaseHue;
      uniform float3 _MOON;


      struct VertexIn{
         float4 position  : POSITION;
         float3 normal    : NORMAL;
         float4 texcoord  : TEXCOORD0;
         float4 tangent   : TANGENT;
      };


      struct VertexOut {
          float4 pos        : POSITION;
          float3 normal     : NORMAL;
          float4 uv         : TEXCOORD0;
          float3 ro         : TEXCOORD1;
          float3 rd         : TEXCOORD2;
          float3 camPos     : TEXCOORD3;
          float3 lightPos : TEXCOORD4;
      };



      float3 hsv(float h, float s, float v){
        return lerp( float3( 1.0,1,1 ), clamp(( abs( frac(h + float3( 3.0, 2.0, 1.0 ) / 3.0 )
                             * 6.0 - 3.0 ) - 1.0 ), 0.0, 1.0 ), s ) * v;
      }


            // Taken from https://www.shadertoy.com/view/4ts3z2
            // By NIMITZ  (twitter: @stormoid)
            // good god that dudes a genius...

            float tri( float x ){
              return abs( frac(x) - .5 );
            }

            float3 tri3( float3 p ){

              return float3(
                  tri( p.z + tri( p.y * 1. ) ),
                  tri( p.z + tri( p.x * 1. ) ),
                  tri( p.y + tri( p.x * 1. ) )
              );

            }

            float triNoise3D( float3 p, float spd , float time){

              float z  = 1.4;
                float rz =  0.;
              float3  bp =   p;

                for( float i = 0.; i <= 3.; i++ ){

                float3 dg = tri3( bp * 2. );
                p += ( dg + time * .1 * spd );

                bp *= 1.8;
                    z  *= 1.5;
                    p  *= 1.2;

                float t = tri( p.z + tri( p.x + tri( p.y )));
                rz += t / z;
                bp += 0.14;

                }

                return rz;

            }



      VertexOut vert(VertexIn v) {

        VertexOut o;

        o.normal = v.normal;

        o.uv = v.texcoord;

        o.camPos = mul( unity_WorldToObject , float4( _WorldSpaceCameraPos , 1. )).xyz;
        //o.lightPos = mul( unity_WorldToObject , float4( _WorldSpaceCameraPos , 1. )).xyz;
        o.lightPos = mul( unity_WorldToObject , _MOON ).xyz ;

        // Getting the position for actual position
        o.pos = UnityObjectToClipPos(  v.position );

        float3 mPos = mul( unity_ObjectToWorld , v.position );

        o.ro = v.position;
        o.rd = normalize( o.camPos - v.position.xyz );



        return o;

      }

      // Fragment Shader
      fixed4 frag(VertexOut o) : COLOR {


                float3 oldNorm = o.normal;

        float3 ro           = o.ro;
        float3 rd           = o.rd;

        float3 col = float3( 0.0 , 0.0 , 0.0 );

        float3 p;



        float acc = 0.;

        float3 lightDir = ro - o.lightPos;

        float m = max(-dot( normalize( lightDir ), o.normal),0);

        //float3 col = float3( 0, 0,0);
        col = hsv( 0 * _HueSize + _BaseHue + .4*1 + m *_HueSize, .6,m*m);



        for( int i = 0; i < _NumberSteps; i++ ){

          p = ro + -rd * float( i ) * _TotalDepth / _NumberSteps;

          float v = float( i )/ _NumberSteps;
          float val = triNoise3D( p * _NoiseSize  + float3(float(i)* .3,0,0), _NoiseSpeed , _Time.y );
           val += triNoise3D( p * _NoiseSize * .4 + float3(float(i)* .3,0,0), _NoiseSpeed , _Time.y );


          if( val < 1 * abs(m)){
            acc = val;
            col = hsv( _HueSize + _BaseHue + val*.2+ .4* v + m*_HueSize , .6,m *m);
            break;
          }
        }



        //col = float3( m, m,m);

        //col /= _NumberSteps;
        //acc /= _NumberSteps;

        float3 bw = float3( acc ,acc ,acc);

        //col = o.normal * .5 + .5;
        //col *= bw;

       // col = lerp( bw * bw * 4,col,_Saturation);

            fixed4 color;
        color = fixed4( col , 1. );
        return color;

      }

      ENDCG
    }
  }
  FallBack "Diffuse"
}
