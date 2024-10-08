Shader"Custom/UnlitProjection"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        [Toggle(USE_TEXTURE)] _UseTexture("Use Texture", Float) = 0
        _MainTex ("Main Texture", 2D) = "white" {}
        _ShadowMap ("Shadow Map", 2D) = "white" {}
        _Angle ("Angle", Float) = 0.0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull front 
        LOD 200
 
        Pass
        {
            CGPROGRAM
            #pragma exclude_renderers gles
 
            #pragma vertex v
            #pragma fragment p
            #pragma shader_feature USE_TEXTURE
 
            uniform sampler2D _MainTex;
            uniform sampler2D _ShadowMap;
            uniform float4x4 _ProjectionMatrix;
            uniform float4 _Color;
 
            struct VertexOut
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
                float4 proj : TEXCOORD1;
            } ;
 
            VertexOut v( float4 position : POSITION, float2 uv : TEXCOORD0 )
            {
                VertexOut OUT;
 
                OUT.position =  UnityObjectToClipPos(position);
                OUT.uv = uv;
                OUT.proj = mul( mul( unity_ObjectToWorld, float4(position.xyz, 1)), _ProjectionMatrix );
 
                return OUT;
            }
 
            struct PixelOut
            {
                float4    color : COLOR;
            } ;
 
            PixelOut p(VertexOut IN)
            {
                PixelOut OUT;
 
                float2 ndc = float2(IN.proj.x/IN.proj.w, IN.proj.y/IN.proj.w);
                float2 uv = (1 + float2( ndc.x, ndc.y)) * 0.5;
 
                float4 c = tex2D( _ShadowMap, uv );
 
                if( uv.x < 0 || uv.y < 0 ||
                    uv.x > 1  || uv.y > 1 || c.a <= 0.00f )
                    {
#ifdef USE_TEXTURE
                        c = tex2D(_MainTex, IN.uv);
#else
						c = _Color;
#endif
                    }
 
                OUT.color = c;
 
                return OUT;
            }
 
            ENDCG
        }
 
    }
    FallBack"Diffuse"
}
 