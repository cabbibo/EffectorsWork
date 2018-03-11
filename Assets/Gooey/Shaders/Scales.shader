Shader "Custom/Scales" {
  Properties {
      _NormalMap( "Normal Map" , 2D ) = "white" {}
      _TexMap( "TEX MAP" , 2D ) = "white" {}
      _CubeMap( "Cube Map" , Cube ) = "white" {}
		  _Color1 ("Tip Color", Color) = (1,.4,.2,1)
      _Color2 ("Tail Color", Color) = (1,0,0,1)
		  _Color3 ("Tail Color", Color) = (1,0,0,1)
        }

        SubShader{
           Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
          Cull Off
          Pass{

            Lighting On
            Tags {"LightMode" = "ForwardBase"}



          //  Blend SrcAlpha OneMinusSrcAlpha // Alpha blending

            CGPROGRAM
            #pragma target 4.5

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"


            #include "Chunks/HairVert.cginc"
            #include "Chunks/hsv.cginc"

            StructuredBuffer<HairVert> _vertBuffer;


            uniform int _TubeWidth;
            uniform int _NumVertsPerHair;
            uniform int _TubeLength;
            uniform int _TotalHair;
            uniform int _TotalVerts;

            uniform sampler2D _NormalMap;
            uniform sampler2D _TexMap;
            uniform samplerCUBE _CubeMap;


            uniform float3 _Color2;
            uniform float3 _Color1;
            uniform float3 _Color3;



        //A simple input struct for our pixel shader step containing a position.
        struct varyings {
          float4 pos      : SV_POSITION;
          float3 worldPos : TEXCOORD4;
          float3 nor      : TEXCOORD0;
          float3 eye      : TEXCOORD2;
          float3 debug    : TEXCOORD3;
          float3 col      : TEXCOORD5;
          float2 uv       : TEXCOORD1;

           half3 tspace0 : TEXCOORD6; // tangent.x, bitangent.x, normal.x
           half3 tspace1 : TEXCOORD7; // tangent.y, bitangent.y, normal.y
           half3 tspace2 : TEXCOORD8; // tangent.z, bitangent.z, normal.z

        };




        //Our vertex function simply fetches a point from the buffer corresponding to the vertex index
        //which we transform with the view-projection matrix before passing to the pixel program.
        varyings vert (uint id : SV_VertexID){

          varyings o;

          if( id < _TotalVerts ){


            float bID = floor( id / 6 );//floor(float((float(id) / (3*2))));// / float( _TubeWidth * _TubeLength-1 );
            float idInTri = fmod( float(id) , 3 );
            float idInTri2 = fmod( floor(float(id)/3) , 2 );
            uint tri  = id % 6;

            // from getRibbonID
            float featherID = floor( id / 6) * 2;


            if( featherID +1 < _TotalHair ){




          HairVert v1 = _vertBuffer[ featherID ];
          HairVert v2 = _vertBuffer[ featherID+1 ];

          float3 tan = normalize(v1.debug);

           float3 viewDir = UNITY_MATRIX_IT_MV[2].xyz;

          float3 dir = v1.pos - v2.pos;

          float3 left = normalize(cross(normalize(dir), tan));


          float3 nor = normalize(cross( left, dir ));
          //float3 left = normalize(cross( normalize(dir),viewDir));

  				float l = length( dir );



  					float3 mid = v2.pos + dir * .3;

            float3 f1 =  v1.pos;//left + v1.pos;
            float3 f2 =  -left * l * .4 + mid;
            float3 f3 =  left * l * .4 + mid;
            float3 f4 =  v2.pos;

            float3 n1 = nor;
            float3 n2 = nor;
            float3 n3 = nor;
            float3 n4 = nor;

            float2 uv1 = float2(0.5,0);
            float2 uv2 = float2(1.0,.7);
            float2 uv3 = float2(-.0,.7);
            float2 uv4 = float2(.5,1);


            float3 finalPos; float3 finalNor; float2 finalUV;

            if( tri == 0){
              finalPos = f1;
              finalNor = n1;
              finalUV = uv1;
            }else if( tri == 1 ){
              finalPos = f2;
              finalNor = n2;
              finalUV = uv2;
            }else if( tri == 2 ){
              finalPos = f4;
              finalNor = n4;
              finalUV = uv4;
            }else if( tri == 3 ){
              finalPos = f1;
              finalNor = n1;
              finalUV = uv1;
            }else if( tri == 4 ){
              finalPos = f4;
              finalNor = n4;
              finalUV = uv4;
            }else if( tri == 5 ){
              finalPos = f3;
              finalNor = n3;
              finalUV = uv3;
            }else{}



 					o.tspace0 = half3(dir.x, left.x, nor.x);
          o.tspace1 = half3(dir.y, left.y, nor.y);
          o.tspace2 = half3(dir.z, left.z, nor.z);


        //o.col =  finalCol;


          float3 fPos = float3( 0 , 0 , 0);

          o.worldPos = finalPos;

          o.pos = mul (UNITY_MATRIX_VP, float4(o.worldPos,1.0f));
          o.nor = finalNor;
          o.uv = finalUV;
          o.debug = float3(featherID,0,0);//_WorldSpaceLightPos0.xyz;

          o.eye = _WorldSpaceCameraPos - o.worldPos;

//          TRANSFER_VERTEX_TO_FRAGMENT(o);
        }

        }

          return o;

        }

        //Pixel function returns a solid color for each point.
        float4 frag (varyings v) : COLOR {

        //  float  atten = LIGHT_ATTENUATION(i);

          float3 x = ddx(v.worldPos);
          float3 y = ddy(v.worldPos);

          float3 nor= cross( normalize(x),normalize(y));

          float2 fUV = float2(1,1) - v.uv;

          float4 tCol = tex2D( _TexMap , fUV);
       // sample the normal map, and decode from the Unity encoding
         half3 tnormal = UnpackNormal(tex2D(_NormalMap, fUV));
          // transform normal from tangent to world space
          half3 worldNormal;
          worldNormal.x = dot(v.tspace0, tnormal);
          worldNormal.y = dot(v.tspace1, tnormal);
          worldNormal.z = dot(v.tspace2, tnormal);

          nor = worldNormal;


          float m = dot( v.debug , nor );


          float3 col;// =  _Color1;

          //if( m < .2){ col = _Color2; }

          if( length(tCol.xyz) >1.4){
          	//discard;
          }

         // col = lerp( _Color3 , col ,  fUV.x);


        float3 eyeRefl = reflect( v.eye , nor );

        float3 cubeCol = texCUBE( _CubeMap , eyeRefl ).xyz;


        //col = float3(v.uv.y, 0, 0);//v.nor * .5 + .5;//float3(1,1,1);
        col =  normalize(eyeRefl) * .5 + .5; //col2 * (fRefl * .5+.5);// * lerp( col1 , col2 , v.uv.x); //v.nor * .5 + .5;//float3( v.uv.x , v.uv.y , 0 );

        col = v.uv.y *v.uv.y;
        col =  normalize(eyeRefl) * .5 + .5;
        col *= cubeCol * 3.4;
        //col *= (float3(1,1,1)-tCol.xyz) * 4*hsv(tCol.x * 1+ _Time.y * .5 + v.debug.x * .1,.6,2 );

//col = v.uv.y *v.uv.y;

        col *= tCol.xyz * tCol.xyz * tCol.xyz;

         	//fCol = float3(1,1,1);
          return float4( col , 1);
        }

        ENDCG

      }
    }

    Fallback Off

  }
