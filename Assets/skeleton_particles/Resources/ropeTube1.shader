Shader "Custom/ropeTube1" {
		Properties {
			_MainTex   ("baseTexture", 2D) = "white" {}
			_TextureMap ("bladeTexture", 2D) = "white" {}
			_NormalMap ("normal", 2D) = "white" {}
			_CubeMap ("cube", Cube) = "white" {}
		}

    SubShader{
//        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        Cull Off
        Pass{

            Blend SrcAlpha OneMinusSrcAlpha // Alpha blending
 
            CGPROGRAM
            #pragma target 5.0
 
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
            #include "Chunks/uvNormalMap.cginc"
            #include "Chunks/noise.cginc"
 
  struct Vert{
    float3 pos;
    float3 oPos;
    float3 ooPos;
    float3 nor;
    float2 uv;
    float id;
    float width;
    float cap; // 0 for no cap, 1 for start , 2 for end
    float3 extraInfo;
    float3 debug;
  };





            StructuredBuffer<Vert> vertBuffer;
         

            uniform int _NumberPoints;
            uniform int _TotalNumberPoints;
            uniform int _NumSides;

            uniform sampler2D _MainTex;
            uniform sampler2D _NormalMap;
            uniform sampler2D _TextureMap;
            uniform sampler2D _AudioMap;
            uniform samplerCUBE _CubeMap;


            //A simple input struct for our pixel shader step containing a position.
            struct varyings {
                float4 pos      : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 nor      : TEXCOORD0;
                float3 eye      : TEXCOORD2;
                float3 debug    : TEXCOORD3;
                float3 col 			: TEXCOORD5;
                float2 uv       : TEXCOORD4;
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


float3 cubicFromValue( in float val , out float3 upPos , out float3 doPos , out float3 vel ){



  float3 p0 = float3( 0. , 0. , 0. );
  float3 v0 = float3( 0. , 0. , 0. );
  float3 p1 = float3( 0. , 0. , 0. );
  float3 v1 = float3( 0. , 0. , 0. );

  float3 p2 = float3( 0. , 0. , 0. );

  float base = val * (float(_NumberPoints-1));
  float baseUp   = floor( base );
  float baseDown = ceil( base );
  float amount = base - baseUp;

  float nAmount = amount / ( baseDown - baseUp );


  Vert vUp = vertBuffer[ int( baseUp )   ];
  Vert vDown = vertBuffer[ int( baseDown )   ];

  p0 = vUp.pos;
  p1 = vDown.pos;


  vel = lerp( vUp.pos - vUp.oPos , vDown.pos - vDown.oPos , nAmount );


  float3 pMinus;

  int bUU = int( baseUp - 1. );


  int bDD = int( baseDown + 1. );
  //if( bDD >= _NumberPoints ){ bDD -= _NumberPoints; }
	if( baseUp == 0. ){
	     p2 = vertBuffer[ int( baseDown + 1. )  ].pos;


	     v1 = .5 * ( p2 - p0 );

	 }else if( baseDown == float(_NumberPoints-1) ){

	     p2 = vertBuffer[ int( baseUp - 1. )  ].pos;

	     v0 = .5 * ( p1 - p2 );

	 }else{

	     float3 pMinus;

	     pMinus = vertBuffer[ int( baseUp - 1. )   ].pos;
	     p2 =     vertBuffer[ int( baseDown + 1. ) ].pos;

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



			float getRadius( float row , float3 vel){
					return	.01;//.01 + length(vel)  * clamp( .5 - abs( row - .5) , 0 , 1)  * (sin( row * 30. + _Time.y) + 3.);
			}
            //Our vertex function simply fetches a point from the buffer corresponding to the vertex index
            //which we transform with the view-projection matrix before passing to the pixel program.
            varyings vert (uint id : SV_VertexID){

                varyings o;


                float bID = floor( id / 6 );//floor(float((float(id) / (3*2))));// / float( _NumLeafs * _LeafResolution-1 );
                float idInTri = fmod( float(id) , 3 );
                float idInTri2 = fmod( floor(float(id)/3) , 2 );
                uint tri  = id % 6;

                float row = floor( bID  / _NumSides);

                
                float col = bID  % _NumSides;
                float colU = (bID +1 ) % _NumSides;


                float3 upPos; float3 doPos;

                float r1 = row / float(_TotalNumberPoints);
                float r2 = (row+1) / float(_TotalNumberPoints);


                float3 vel1;
                float3 pos1 = cubicFromValue( r1  , upPos , doPos , vel1 );
                float3 nor1 = normalize( upPos - doPos );

                float3 vel2;
                float3 pos2 = cubicFromValue( r2  , upPos , doPos ,vel2 );
                float3 nor2 = normalize( upPos - doPos ); 

                float3 x1 = normalize( cross( nor1 , float3( 1, 0 , 0)));
                float3 z1 = normalize( cross( nor1 , x1 ));

                float3 x2 = normalize( cross( nor2 , float3( 1, 0 , 0)));
                float3 z2 = normalize( cross( nor2 , x2 ));

				float angle =  col/_NumSides * 2 * 3.14159;
              	float angleU = (col+1)/_NumSides * 2 * 3.14159;



              	float3 aCol1 = tex2Dlod( _AudioMap , float4( r1 * .2 , 0. ,0. ,0.)).xyz;
              	float3 aCol2 = tex2Dlod( _AudioMap , float4( r2 * .2, 0. ,0. ,0.)).xyz;

              	float radius  =  getRadius( r1 , vel1 );// * (clamp( length( aCol1 ) , 0 , .2) + .1);
              	float radiusU =  getRadius( r2 , vel2 );// * (clamp( length( aCol2 ) , 0 , .2) + .1);

              	//angle += 1.1 * noise( r1 * 100 );
              	//radius = .03;
              	//radiusU = .03;
              //radius  += .01 * noise(  r1 * 100) + .01 * noise(r1 * 10 );
              //radiusU += .01 * noise(  r2 * 100) + .01 * noise(r2 * 10 );
              	//float3 f1 = radius * sin(angle ) * x1   + radius * cos( angle  ) * z1 + pos1;
              	//float3 f2 = radius * sin(angleU) * x1   + radius * cos( angleU ) * z1 + pos1;
              	//float3 f3 = radiusU * sin(angle ) * x2   + radiusU * cos( angle  ) * z2 + pos2;
              	//float3 f4 = radiusU * sin(angleU) * x2   + radiusU * cos( angleU ) * z2 + pos2;


              	float3 f1 = vel1 + radius * sin( angle ) * x1  + radius * cos( angle ) * z1 + pos1;
              	float3 f2 = vel1 + radius * sin( angleU ) * x1  + radius * cos( angleU ) * z1 + pos1;
              	float3 f3 = vel2 + radiusU * sin( angle ) * x2  + radiusU * cos( angle ) * z2 + pos2;
              	float3 f4 = vel2 + radiusU * sin( angleU ) * x2  + radiusU * cos( angleU ) * z2 + pos2;


              	float3 n = normalize(cross( normalize(f1-f2) ,  normalize( f3-f2)));

              	//float3 n1 = n; //normalize(f1 - pos1);
              	//float3 n2 = n; //normalize(f2 - pos1);
              	//float3 n3 = n; //normalize(f3 - pos2);
              	//float3 n4 = n; //normalize(f4 - pos2);
//
				float3 n1 = normalize(f1 - pos1);
				float3 n2 = normalize(f2 - pos1);
				float3 n3 = normalize(f3 - pos2);
				float3 n4 = normalize(f4 - pos2);

              	float2 uv1 = float2(r1,(col)/_NumSides);
              	float2 uv2 = float2(r1,(col+1)/_NumSides);
              	float2 uv3 = float2(r2,(col)/_NumSides);
				float2 uv4 = float2(r2,(col+1)/_NumSides);


				float3 finalCol; float3 finalPos; float3 finalNor; float2 finalUV;



 	             	if( tri == 0){
 	             		finalPos = f1;
 	             		finalCol = aCol1;
 	             		finalNor = n1;
 	             		finalUV = uv1;

              	}else if( tri == 1 ){
              		finalPos = f2;
              		finalCol = aCol1;
 	             		finalNor = n2;
 	             		finalUV = uv2;
              	}else if( tri == 2 ){
              		finalPos = f4;
              		finalCol = aCol2;
 	             		finalNor = n4;
 	             		finalUV = uv4;
              	}else if( tri == 3 ){
              		finalPos = f1;
              		finalCol = aCol1;
 	             		finalNor = n1;
 	             		finalUV = uv1;
              	}else if( tri == 4 ){
              		finalPos = f4;
              		finalCol = aCol2;
 	             		finalNor = n4;
 	             		finalUV = uv4;
              	}else if( tri == 5 ){
              		finalPos = f3;
              		finalCol = aCol2;
 	             		finalNor = n3;
 	             		finalUV = uv3;
              	}else{}


				o.col =  finalCol;

              

              float3 fPos = float3( 0 , 0 , 0);

              o.worldPos = finalPos;

              o.pos = mul (UNITY_MATRIX_VP, float4(o.worldPos,1.0f));
              o.nor = finalNor;
              o.uv = finalUV;
              o.debug = float3( 1 , 0 , 0 );


              // o.debug = float3( 1 , 0 , 0);// * nor;//o.worldPos - og.pos;

               o.eye = _WorldSpaceCameraPos - o.worldPos;
              // o.uv = v.uv;
              // o.nor = v.nor;
                return o;

            }
 
            //Pixel function returns a solid color for each point.
            float4 frag (varyings v) : COLOR {
	

								float3 fNorm = uvNormalMap( _NormalMap , v.pos ,  v.uv  * float2( 1. , .2), v.nor , 10.1 , .6 );
  
              	float3 fRefl = reflect( -v.eye , fNorm );
                float3 cubeCol = texCUBE(_CubeMap, normalize( fRefl ) ).rgb;


                float3 col1 = lerp( cubeCol , cubeCol * float3( 0.3 , 4.6 , 2. ) , clamp(v.uv.x *3 ,0,1) ); 
                float3 col2 = lerp( cubeCol * float3( 0.3 , 4.6 , 2. ) , cubeCol * float3( 2  , .6 , .4) , clamp((v.uv.x  -.8) * 10 ,0,1) ); 



            	float3 fCol = (v.col * 2.+ .1) * lerp( col1 , col2 , v.uv.x); //v.nor * .5 + .5;//float3( v.uv.x , v.uv.y , 0 );
              fCol = fRefl * .5 + .5;

              float m = dot( -normalize(v.eye) , v.nor );

              fCol *= pow( (1+m), 10.);

              fCol = v.nor * .5 + .5;

              //fCol = float3( 1., 0., 0.);
                return float4( fCol , 1.);
            }
 
            ENDCG
 
        }
    }
 
    Fallback Off
	
}