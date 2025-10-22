Shader "Custom/Water2D"
{
    Properties
    {
        _MainTex ("水块纹理", 2D) = "white" {}
        _BaseColor ("基础颜色", Color) = (0.2, 0.7, 0.9, 0.8) // 蓝绿色半透明
        _WaveSpeed ("波动速度", Range(0.5, 3)) = 1.5
        _WaveAmount ("波动幅度", Range(0.01, 0.1)) = 0.03
        _Highlight ("高光强度", Range(0, 0.5)) = 0.2
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha // 半透明混合
        Cull Off // 双面可见

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _BaseColor;
            float _WaveSpeed;
            float _WaveAmount;
            float _Highlight;

            v2f vert (appdata v)
            {
                v2f o;
                // 顶点波动（模拟表面起伏）
                float time = _Time.y * _WaveSpeed;
                v.vertex.y += sin(v.uv.x * 10 + time) * _WaveAmount; // X方向波动
                v.vertex.x += cos(v.uv.y * 8 + time * 1.2) * _WaveAmount * 0.5; // Y方向辅助波动
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _BaseColor;
                // 高光效果（基于UV位置，模拟反光带）
                float highlight = sin(i.uv.x * 15 + _Time.y * _WaveSpeed) * 0.5 + 0.5;
                col.rgb += highlight * _Highlight;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}