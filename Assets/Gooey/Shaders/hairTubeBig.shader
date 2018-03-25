Shader "Custom/hairTubeBig" {
  Properties {
      _NormalMap( "Normal Map" , 2D ) = "white" {}
      _CubeMap( "Cube Map" , Cube ) = "white" {}
		    _Color1 ("Tip Color", Color) = (1,.4,.2,1)
        _Color2 ("Tail Color", Color) = (1,0,0,1)
        _Color3 ("lightColor", Color) = (1,0,0,1)
        _Color4 ("ReflectColor", Color) = (1,0,0,1)
        }

        SubShader{
          //        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
          Cull Front
          Pass{

            Lighting On
            Tags {"LightMode" = "ForwardBase"}



            Blend SrcAlpha OneMinusSrcAlpha // Alpha blending

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

            uniform float3 _MOON;
            uniform sampler2D _NormalMap;
            uniform sampler2D _TexMap;
            uniform samplerCUBE _CubeMap;


            uniform float3 _Color2;
            uniform float3 _Color1;
            uniform float3 _Color3;
            uniform float3 _Color4;



        //A simple input struct for our pixel shader step containing a position.
        struct varyings {
          float4 pos      : SV_POSITION;
          float3 worldPos : TEXCOORD4;
          float3 nor      : TEXCOORD0;
          float3 eye      : TEXCOORD2;
          float3 debug    : TEXCOORD3;
          float3 col      : TEXCOORD5;
          float2 uv       : TEXCOORD1;
          LIGHTING_COORDS(6,7)
        };



        float3 cubicCurve( float t , float3  c0 , float3 c1 , float3 c2 , float3 c3 ){

          float s  = 1. - t;

          float3 v1 = c0 * ( s * s * s );
          float3 v2 = 3. * c1 * ( s * s ) * t;
          float3 v3 = 3. * c2 * s * ( t * t );
          float3 v4 = c3 * ( t * t * t );

          float3 value = v1 + v2 + v3 + v4;

          return value;

        }

        float3 cubicFromValue( in float val , in float bladeID , out float3 upPos , out float3 doPos ){

          //float3 upPos;
          //float3 doPos;


          float3 p0 = float3( 0. , 0. , 0. );
          float3 v0 = float3( 0. , 0. , 0. );
          float3 p1 = float3( 0. , 0. , 0. );
          float3 v1 = float3( 0. , 0. , 0. );

          float3 p2 = float3( 0. , 0. , 0. );



          float base = val * (float(_NumVertsPerHair)-1.);
          float baseUp   = floor( base );
          float baseDown = ceil( base );
          float amount = base - baseUp;

          int bladeBase = _NumVertsPerHair * int( bladeID );

          if( baseUp == 0. ){

            p0 = _vertBuffer[ int( baseUp )        + bladeBase ].pos;
            p1 = _vertBuffer[ int( baseDown )      + bladeBase ].pos;
            p2 = _vertBuffer[ int( baseDown + 1. ) + bladeBase ].pos;


            v1 = .5 * ( p2 - p0 );

            }else if( baseDown == float(_NumVertsPerHair-1) ){

              p0 = _vertBuffer[ int( baseUp )      + bladeBase ].pos;
              p1 = _vertBuffer[ int( baseDown )    + bladeBase ].pos;
              p2 = _vertBuffer[ int( baseUp - 1. ) + bladeBase ].pos;

              v0 = .5 * ( p1 - p2 );

              }else{

                p0 = _vertBuffer[ int( baseUp )   + bladeBase ].pos;
                p1 = _vertBuffer[ int( baseDown ) + bladeBase ].pos;


                float3 pMinus;

                pMinus = _vertBuffer[ int( baseUp - 1. )   + bladeBase ].pos;
                p2 =     _vertBuffer[ int( baseDown + 1. ) + bladeBase ].pos;

                v1 = .5 * ( p2 - p0 );
                v0 = .5 * ( p1 - pMinus );

              }


              float3 c0 = p0;
              float3 c1 = p0 + v0/3.;
              float3 c2 = p1 - v1/3.;
              float3 c3 = p1;




              float3 pos = cubicCurve( amount , c0 , c1 , c2 , c3 );

              upPos = cubicCurve( amount  + .1 , c0 , c1 , c2 , c3 );
              doPos = cubicCurve( amount  - .1 , c0 , c1 , c2 , c3 );

              return pos;


            }

            float3 straightFromVal( in float val , in float bladeID , out float3 upPos , out float3 doPos ){


            }

            float getRadius( float row ){
              return  10 * pow((1 - row ),.5) + (sin( row * 10 )+1) * .03 + row *row* row * .2 + (sin( row * 100)+1) * ( 1-row) * .02;
            }
        //Our vertex function simply fetches a point from the buffer corresponding to the vertex index
        //which we transform with the view-projection matrix before passing to the pixel program.
        varyings vert (uint id : SV_VertexID){

          varyings o;


            float bID = floor( id / 6 );//floor(float((float(id) / (3*2))));// / float( _TubeWidth * _TubeLength-1 );
            float idInTri = fmod( float(id) , 3 );
            float idInTri2 = fmod( floor(float(id)/3) , 2 );
            uint tri  = id % 6;




            // from getRibbonID
            float bladeID = floor( id / ((_TubeWidth * (_TubeLength-1)) * 3 * 2));
            float remainID = id - bladeID * (_TubeWidth * (_TubeLength-1) * 3 * 2);
            float row = floor(remainID / (_TubeWidth * 3 * 2));
            float triID = floor( id / 6 );

            float col = triID  % _TubeWidth;
            float colU = (triID +1 ) % _TubeWidth;







            // Rebuilding the mesh in the vert shader

            float3 upPos; float3 doPos;

            float r1 = row / float(_TubeLength);
            float r2 = (row+1) / float(_TubeLength);

            float3 pos1 = cubicFromValue( r1 , bladeID , upPos , doPos );
            float3 nor1 = normalize( upPos - doPos );

            float3 pos2 = cubicFromValue( r2 , bladeID , upPos , doPos );
            float3 nor2 = normalize( upPos - doPos );


            float3 x1 = normalize( cross( nor1 , float3( 1, 0 , 0)));
            float3 z1 = normalize( cross( nor1 , x1 ));

            float3 x2 = normalize( cross( nor2 , float3( 1, 0 , 0)));
            float3 z2 = normalize( cross( nor2 , x2 ));

            float angle =  col/_TubeWidth * 2 * 3.14159;
            float angleU = (col+1)/_TubeWidth * 2 * 3.14159;

            float bladeNormed = float( bladeID)/ float( _TotalHair);

            float lengthModulator = (sin( float( bladeID) * 16135. ) + 2)/2;

            float fr1 = r1;// + (1/float(_TubeLength));
            float fr2= r2;// + (1/float(_TubeLength));

            float radius  = .04 * (1-fr1 * fr1) * (fr1 * fr1);// * getRadius( fr1 );
            float radiusU = .04 * (1-fr2 * fr2) * (fr2 * fr2);// * getRadius( r2 );

            //radius *= lengthModulator;
            //radiusU *= lengthModulator;

            float3 f1 = radius * sin(angle ) * x1   + radius * cos( angle  ) * z1 + pos1;
            float3 f2 = radius * sin(angleU) * x1   + radius * cos( angleU ) * z1 + pos1;
            float3 f3 = radiusU * sin(angle ) * x2   + radiusU * cos( angle  ) * z2 + pos2;
            float3 f4 = radiusU * sin(angleU) * x2   + radiusU * cos( angleU ) * z2 + pos2;

            float3 n1 = normalize(f1 - pos1);
            float3 n2 = normalize(f2 - pos1);
            float3 n3 = normalize(f3 - pos2);
            float3 n4 = normalize(f4 - pos2);

            float2 uv1 = float2(r1,(col)/_TubeWidth);
            float2 uv2 = float2(r1,(col+1)/_TubeWidth);
            float2 uv3 = float2(r2,(col)/_TubeWidth);
            float2 uv4 = float2(r2,(col+1)/_TubeWidth);


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





          int bladeBase = _NumVertsPerHair * int( bladeID );
          HairVert baseVert = _vertBuffer[ bladeBase ];
        //o.col =  finalCol;


          float3 fPos = float3( 0 , 0 , 0);

          o.worldPos = 20 * finalPos;

          o.pos = mul (UNITY_MATRIX_VP, float4(o.worldPos,1.0f));
          o.nor = finalNor;
          o.uv = finalUV;
          o.debug = _WorldSpaceLightPos0.xyz;

          o.eye = _WorldSpaceCameraPos - o.worldPos;

          TRANSFER_VERTEX_TO_FRAGMENT(o);

          return o;

        }

        //Pixel function returns a solid color for each point.
        float4 frag (varyings v) : COLOR {

//          float  atten = LIGHT_ATTENUATION(i);


          float m = dot( v.debug , v.nor );


          float3 col =  lerp(_Color1 , _Color2, v.uv.x);

          //if( m < .2){ col = _Color2; }


          float3 lightDir = -(_MOON - v.worldPos);

          float match = -dot( normalize( lightDir) , v.nor);


        float3 eyeRefl = reflect( v.eye , v.nor );

        float3 cubeCol = texCUBE( _CubeMap , eyeRefl ).xyz;

          float refl = clamp( dot( normalize(lightDir), normalize(eyeRefl)),0,1);//v.nor;

          float lightV = clamp( match*match * match *30,0,1);
          col = lerp( col , _Color3 , lightV);
          col = lerp(col, _Color4, clamp( 100000*pow(refl,41) ,0,1));

          col = v.nor * .5 + .5;
          col *= lightV;

                    refl *= pow( refl , 10 );
          col = hsv(refl,1,refl);// * .5 + .5;
          col += lightV * v.nor * .2 + .8;


        // col *= cubeCol;
        //col = float3(1,1,1);
        //col *= cubeCol * (v.nor * .5 + .5) * 2; //col2 * (fRefl * .5+.5);// * lerp( col1 , col2 , v.uv.x); //v.nor * .5 + .5;//float3( v.uv.x , v.uv.y , 0 );

         	//fCol = float3(1,1,1);
          return float4( col , 1.);
        }

        ENDCG

      }
    }

    Fallback Off

  }
