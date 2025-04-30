Shader "Custom/DiagonalMask"
{Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Progress ("Wipe Progress", Range(0,1)) = 0
        _Slope ("Slope", Float) = 1.0
        _Feather ("Feather Range", Range(0,0.2)) = 0.05
       
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float _Progress;
            float _Slope;
            float _Feather;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // 斜線公式，這次以進度拉整個畫面
                float cutLine = _Slope * (uv.x - _Progress);

                // distance 是從當前uv到cutline的距離
                float distance = uv.y - cutLine;

                // 加羽化
                float alpha = saturate(distance / _Feather);

                fixed4 col = tex2D(_MainTex, uv);

                col.a *= alpha;
                return col;
            }
            ENDCG
        }
    }
}