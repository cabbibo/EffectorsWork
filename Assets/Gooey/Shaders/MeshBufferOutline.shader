// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/MeshBufferOutline" {

	Properties {
      _Color("Color" , Color ) = (1,.4,.2,1)
  }
  SubShader{

  	


    Cull Front
			ZWrite Off
			//ZTest Always

    Pass{


      CGPROGRAM
      #pragma target 4.5

      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"

      uniform float3 _Color;
      uniform float _Outline;

      #include "Chunks/SkinnedVert.cginc"


      StructuredBuffer<SkinnedVert> _vertBuffer;
      StructuredBuffer<int> _triBuffer;

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


        float fID = float(_triBuffer[id]);

        SkinnedVert v = _vertBuffer[_triBuffer[id]];

 				float3 viewDir = UNITY_MATRIX_IT_MV[2].xyz;
        float3 fPos = v.pos;// + v.nor * .03 - viewDir * .1; 
   

				o.pos = mul (UNITY_MATRIX_VP, float4(fPos,1.0f));


				float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.nor);
				float2 offset = TransformViewToProjection(norm.xy);
 
				o.pos.xy += offset * o.pos.z * _Outline;
				o.worldPos = fPos;
				o.eye = _WorldSpaceCameraPos - o.worldPos ;
	
				o.nor = v.nor;//normalize(v.nor);
				o.uv = v.uv;
        o.debug = _WorldSpaceLightPos0.xyz;
        return o;


      }
      //Pixel function returns a solid color for each point.
      float4 frag (varyings v) : COLOR {


      	float3 col =  _Color; 

        return float4( col , 1. );


      }

      ENDCG

    }
  }

  Fallback Off
  
}
