// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/hairRenderer" {



    SubShader{
//        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        Cull off
        Pass{

            Blend SrcAlpha OneMinusSrcAlpha // Alpha blending
 
            CGPROGRAM
            #pragma target 4.5
 
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"


 

            #include "Chunks/HairVert.cginc"
            

            StructuredBuffer<HairVert> _vertBuffer;

            //uniform float4x4 worldMat;

            uniform int _VertsPerHair;
            uniform int _TotalVerts;
 			uniform float3 _Color;
            //A simple input struct for our pixel shader step containing a position.
            struct varyings {
                float4 pos      : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 nor      : TEXCOORD0;
                float3 eye      : TEXCOORD2;
                float3 debug    : TEXCOORD3;
                float2 uv       : TEXCOORD4;
            };

            
           

            //Our vertex function simply fetches a point from the buffer corresponding to the vertex index
            //which we transform with the view-projection matrix before passing to the pixel program.
            varyings vert (uint id : SV_VertexID){

                varyings o;

                /*int segID =floor( float(id) / 2);
                int offset = id % 2;
                int idInHair = segID % (_VertsPerHair-1);
                int hairID = int( floor( float(segID) / float(_VertsPerHair) ));*/
               		
                int halfID = id/2;
                int hairID = halfID/(_VertsPerHair-1);

                int offsetID = id % 2;



               	int fID = halfID + offsetID + hairID;//(hairID * ((_VertsPerHair ))) + idInHair + offset;

                HairVert v = _vertBuffer[fID];

                float3 nor;
                if( offsetID + hairID == 0 ){
                	nor = mul( unity_ObjectToWorld , float4(v.norm,0)).xyz;
                	nor = normalize( nor );
                }else{
                	HairVert vDown = _vertBuffer[fID-1];
                	nor = -normalize( v.pos - vDown.pos );
                }

                float3 dif =   - v.pos;

                o.worldPos = v.pos;//mul( worldMat , float4( v.pos , 1.) ).xyz;

                o.eye = _WorldSpaceCameraPos - o.worldPos;

                o.pos = mul (UNITY_MATRIX_VP, float4(o.worldPos,1.0f));


                o.debug = v.debug;//n * .5 + .5;
                o.uv = v.uv;
                o.nor = nor;

            
                return o;

            }
 
            //Pixel function returns a solid color for each point.
            float4 frag (varyings v) : COLOR {



                float3 col = float3(v.uv.x , 0 ,0);//v.nor * .5 + .5;
                return float4( col , 1 );

            }
 
            ENDCG
 
        }
    }
 
    Fallback Off
	
}