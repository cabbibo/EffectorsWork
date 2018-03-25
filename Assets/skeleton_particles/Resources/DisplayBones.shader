Shader "Custom/DisplayBones" {
	Properties {

    }
  SubShader{

  	


    Cull off
    Pass{


      CGPROGRAM
      #pragma target 5.0

      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"







    struct Bone{

        float3 start;
        float3 end;
        float   size;

    };
      StructuredBuffer<Bone> _boneBuffer;

      //A simple input struct for our pixel shader step containing a position.
      struct varyings {
          float4 pos 			: SV_POSITION;
          float3 nor 			: TEXCOORD0;
          float2 uv  			: TEXCOORD1;
          //float2 suv 			: TEXCOORD2;
          //float3 col 			: TEXCOORD3;
          //float  lamb 		: TEXCOORD4;
          float3 eye      : TEXCOORD5;
          float3 worldPos : TEXCOORD6;
          float3 debug    : TEXCOORD7;

      };


      varyings vert (uint id : SV_VertexID){

        varyings o;

        int bID = id / 6;

        int tri = id % 6;

       	Bone b = _boneBuffer[bID];

       	float3 fPos = b.start;//mul(float4(0,0,0,1),b.bindPose ).xyz;

        float3 fPos2 =b.end; 
        
        float3 dir = fPos2- fPos;

        float3 up = cross( UNITY_MATRIX_IT_MV[2].xyz , normalize(dir))  * .01;

        float3 p1 = fPos + up;
        float3 p2 = fPos - up;
        float3 p3 = fPos2 + up;
        float3 p4 = fPos2 - up;


				

				if( tri == 0 ){ fPos = p1; }
				if( tri == 1 ){ fPos = p2; }
				if( tri == 2 ){ fPos = p4; }


        if( tri == 3 ){ fPos = p1; }
        if( tri == 4 ){ fPos = p4; }
        if( tri == 5 ){ fPos = p3; }


				o.pos = mul (UNITY_MATRIX_VP, float4(fPos,1.0f));
				o.worldPos = fPos;
				o.eye = _WorldSpaceCameraPos - o.worldPos;
	
				o.nor = float3(0,0,0);
				o.uv = float2(0,0);
        o.debug = float3(1,0,0);

        return o;


      }
      //Pixel function returns a solid color for each point.
      float4 frag (varyings v) : COLOR {
      	float3 col = v.debug;

        return float4( col , 1. );


      }

      ENDCG

    }
  }

  Fallback Off
  
}