Shader "Custom/pointsAlongRope" {

	Properties {
_Texture( "Texture" , 2D ) = "white" {}
    }




  SubShader{
    Cull off
    // inside SubShader
Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }

// inside Pass
ZWrite Off
Blend SrcAlpha OneMinusSrcAlpha
    Pass{


      CGPROGRAM
      #pragma target 5.0

      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"
      #include "Chunks/hsv.cginc"


      uniform sampler2D _Texture;


      struct Vert{
         float3 pos;
         float3 vel;
         float2 uv;
         float life;
         float3 debug;
      };

      StructuredBuffer<Vert> _vertBuffer;

      //A simple input struct for our pixel shader step containing a position.
      struct varyings {
          float4 pos 			: SV_POSITION;
          float2 uv  			: TEXCOORD1;
          float3 eye      : TEXCOORD5;
          float3 worldPos : TEXCOORD6;
          float3 debug    : TEXCOORD7;

      };


      varyings vert (uint id : SV_VertexID){

        varyings o;

        int bID = id / 6;

        int tri = id % 6;

       	Vert v = _vertBuffer[bID];

       	float3 fPos = float3(0,0,0);

       	fPos = v.pos;

				float3 up = UNITY_MATRIX_IT_MV[0].xyz;
				float3 ri = UNITY_MATRIX_IT_MV[1].xyz;

				float size = .006 * clamp( v.life, 0.1,1);

        float2 fUV;
				if( tri == 0 || tri == 5 ){ fPos -= ri * size; fUV = float2(0,0);}
				if( tri == 1 || tri == 4 ){ fPos += ri * size; fUV = float2(1,1);}
				if( tri == 2 ){ fPos += up * size; fUV = float2(0,1);}
        if( tri == 3 ){ fPos -= up * size; fUV = float2(1,0);}

				o.pos = mul (UNITY_MATRIX_VP, float4(fPos,1.0f));
				o.worldPos = fPos;
				o.eye = normalize(_WorldSpaceCameraPos - o.worldPos);

				o.uv = fUV;
				o.debug =normalize(v.vel);




        return o;


      }
      //Pixel function returns a solid color for each point.
      float4 frag (varyings v) : COLOR {


        /*if( length( v.eye) < .3 ){
          discard;
        }*/
        float4 tCol = tex2D(_Texture, float2( v.uv));
      	float3 col = tCol.xyz;

       float m = dot( v.debug , v.eye );
        col *= v.debug * .5 + .5;
        col *= 2;
        col =lerp(col,hsv(m,1,1), m*m*m*m);

        return float4( col ,tCol.w);


      }

      ENDCG

    }
  }

  Fallback Off

}
