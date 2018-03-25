Shader "Custom/GooMesh" {

	Properties {
      _NormalMap( "Normal Map" , 2D ) = "white" {}
      _CubeMap( "Cube Map" , Cube ) = "white" {}
      _TexMap( "Tex Map" , 2D ) = "white" {}
      _SizeMultiplier( "Size Multiplier" , float ) = 1
  }
  SubShader{




    Cull off
    Pass{


      CGPROGRAM
      #pragma target 4.5

      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"

      uniform sampler2D _NormalMap;
      uniform sampler2D _TexMap;
      uniform samplerCUBE _CubeMap;


      #include "Chunks/Vert.cginc"


      int _NumVerts;
      int _NumTris;
      StructuredBuffer<Vert> _vertBuffer;
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

        int smallBig = (id / _NumTris);
        id = id % _NumTris;

        varyings o;

        Vert v = _vertBuffer[_triBuffer[id]];
        float3 fPos = v.pos * (1+float(smallBig*4));
				o.pos = mul (UNITY_MATRIX_VP, float4(fPos,1.0f));
				o.worldPos = fPos;
				o.eye = _WorldSpaceCameraPos - o.worldPos;

				o.nor = v.nor;//normalize(v.nor);
				o.uv = v.uv;
        return o;


      }
      //Pixel function returns a solid color for each point.
      float4 frag (varyings v) : COLOR {

        float3 nNor = normalize(cross( ddx( v.worldPos ) , ddy( v.worldPos)));


      	float3 eyeRefl = reflect( v.eye , nNor );

      	float3 cubeCol = texCUBE( _CubeMap , eyeRefl ).xyz;


      	float3 col = normalize(-nNor) * .3 + .7;

      	col = cubeCol * 2 * col;// float3(1,0,0);

        return float4( col , 1. );


      }

      ENDCG

    }
  }

  Fallback Off

}
