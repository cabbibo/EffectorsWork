Shader "Custom/pointBasicRender" {

  SubShader{

  	


    Cull off
    Pass{


      CGPROGRAM
      #pragma target 4.5

      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"

			struct Vert{
			   float distanceUp;
			   float3 pos;
			   float3 oPos;
			   float3 nor;
			   float2 uv;
			   float3 debug;
			};


			uniform int _TotalVerts;

      StructuredBuffer<Vert> _vertBuffer;

      //A simple input struct for our pixel shader step containing a position.
      struct varyings {
          float4 pos 			: SV_POSITION;

      };


      varyings vert (uint id : SV_VertexID){

        varyings o;

        int tri =id % 3;
      	int fID = id / 3;

        if( fID < _TotalVerts ){

	        Vert v = _vertBuffer[fID];

	        float3 fPos = v.pos;

	        if( tri == 0 ){ fPos += float3( 1 , 0 , 0) * .1; }
	        if( tri == 1 ){ fPos += float3( 0 , 1 , 0) * .1; }

					o.pos = mul (UNITY_MATRIX_VP, float4(fPos,1.0f));
	        

        }

        return o;
	      


      }
      //Pixel function returns a solid color for each point.
      float4 frag (varyings v) : COLOR {

      	float3 col = float3(1,1,1);// + float3( .6 , 0,0);// v.nor * .5 + .5;

        return float4( col , 1. );


      }

      ENDCG

    }
  }

  Fallback Off
  
}
