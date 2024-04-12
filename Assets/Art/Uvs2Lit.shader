Shader "Custom/LitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        // Add more properties as needed
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1; // Secondary UVs
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1; // Secondary UVs
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = v.uv2; // Pass secondary UVs
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Sample textures using primary and secondary UVs
                fixed4 col = tex2D(_MainTex, i.uv * _MainTex_ST.xy + _MainTex_ST.zw);
                // Use i.uv2 for secondary UV sampling if needed
                fixed4 col2 = tex2D(_SecondTex, i.uv2);
                return col;
            }
            ENDCG
        }
    }
}
