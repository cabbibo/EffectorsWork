Shader "Custom/clouds2" {

 Properties {

   // _NumberSteps( "Number Steps", Int ) = 20
    _TotalDepth( "Total Depth", float) = .1
    _NoiseSize("Noise Size" , float) = 3
    _HeightMap("Height Map" , 2D) = "white" {}
    _NoiseMap("Noise Map" , 2D) = "white" {}
    _NormalMap("Normal Map" , 2D) = "white" {}

  }



  SubShader {

      // inside SubShader
Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }

    Pass {

// inside Pass
//ZWrite Off
Blend SrcAlpha OneMinusSrcAlpha


      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag


      #include "UnityCG.cginc"
      #include "Chunks/hsv.cginc"


      sampler2D _HeightMap;
      sampler2D _NoiseMap;
      sampler2D _NormalMap;

     // uniform int _NumberSteps;
      uniform float  _TotalDepth;
      uniform float _NoiseSize;
      uniform float3 _MOON;

      struct VertexIn
      {
         float4 position  : POSITION;
         float3 normal    : NORMAL;
         float4 texcoord  : TEXCOORD0;
				 float3 tangent   : TANGENT;
      };

      struct VertexOut {
          float4 pos    : POSITION;
          float3 normal : NORMAL;
          float4 uv     : TEXCOORD0;
          float3 worldPos :TEXCOORD7;
          float3 ro     : TEXCOORD2;
          float3 camPos : TEXCOORD4;
          float3 camTC : TEXCOORD5;
          float3 lightTC : TEXCOORD6;
      };

      float3 localVel;

      float getTerrainInfo( float3 position  ){

      	float2 uv = (float2(position.x, -position.z)  )+ float2(.5,.5);
      	float center = length(tex2Dlod(_HeightMap,float4(uv,0,6)).xyz);

      	return center * 1.;

      }


      VertexOut vert(VertexIn v) {

        VertexOut o;

        o.normal = v.normal;
        o.uv =4* v.texcoord;

        float4 fPos = v.position;

        // Getting the position for actual position
        o.pos = UnityObjectToClipPos(  fPos );

        float3 mPos = mul( unity_ObjectToWorld , v.position );

        	float3 localPos = v.position;
	float3 worldPos = mul(unity_ObjectToWorld, v.position).xyz;
	float3 worldNormal = normalize(mul(unity_ObjectToWorld, float4(v.normal, 0.0)).xyz);
	float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));


	float3 binormal = cross(v.tangent, v.normal);
	float3x3 tbn = float3x3(v.tangent, binormal, v.normal);

  o.worldPos = worldPos;


	// get cam pos in texture (TBN) space
	float3 camPosLocal = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0)).xyz;

  float3 moonLocal = mul(unity_WorldToObject, float4(_MOON, 1.0)).xyz;
	float3 dirToCamLocal = camPosLocal - localPos;
  float3 camPosTexcoord = mul(tbn, dirToCamLocal);
	float3 lightPosTexcoord = mul(tbn, moonLocal - localPos);



        o.ro = v.position * float3(1.,1.,1.); ;//mPos;
        o.camPos = mul( unity_WorldToObject , float4( _WorldSpaceCameraPos  , 1. )).xyz * float3(1.,1.,1.);

        o.camTC = camPosTexcoord;
        o.lightTC = lightPosTexcoord;
        return o;

      }



     // Fragment Shader
      fixed4 frag(VertexOut v) : COLOR {

        float3 ro = v.ro;
        float3 rd = normalize((ro - v.camPos) );

        float3 col = float3( 0.0 , 0.0 , 0.0 );
        //float3 lightDir = -normalize(float3( sin(_Time.y) , 0. , cos(_Time.y)));
        float3 lightDir = normalize(float3(2., 1. , 2.));




        	// height-field offset
      //	float2 uv = (float2(v.ro.x, -v.ro.z)  )+ float2(.5,.5);

	float height = tex2Dlod(_HeightMap, float4(v.uv.x,v.uv.y,0.,6)).r;
	float noise = tex2D(_NoiseMap, v.uv).r;
	// noise += .3 * tex2D(_NoiseMap, 3 * v.uv + 3*float2(_Time.x,0)).r;
	float h = noise * .1 -.1;//-.15;
	float2 newCoords = v.uv + v.camTC* h;

  lightDir = normalize(v.worldPos - _MOON);


      col = tex2D( _NoiseMap , newCoords);

	float3 nor = tex2D(_NormalMap, newCoords );
 // nor += .3 * tex2D(_NormalMap, newCoords * 3  + 3*float2(_Time.x,0) );
  nor = normalize( nor );



	float m = dot( nor * 2 - 1  , lightDir );
	float f = m*m*4;
	col = hsv( col.r * 4 + _Time.x *4 , .3 , 1);//float3(f,f,f) ;//nor * .5 + .5;

        fixed4 color;
        color = fixed4( col , 1);
        return color;
      }

      ENDCG
    }
  }
  FallBack "Diffuse"
}
