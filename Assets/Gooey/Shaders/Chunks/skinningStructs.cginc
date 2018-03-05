struct SkinnedVert{

  float  used;
  float3 pos;
  float3 vel;
  float3 nor;
  float3 tan;
  float2 uv;

  float3 targetPos;

  float3 bindPos;
  float3 bindNor;
  float3 bindTan;
  float4 boneWeights;
  float4 boneIDs;
  float3 debug;
  
};