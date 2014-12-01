Shader "Custom/CircleShader" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
		_Fraction ("Fraction (Float)", Float) = 0
		_EdgeBlend ("EdgeBlend (Float)", Float) = .02
		_Color ("Tint", Color) = (1.0, 0.6, 0.6, 1.0)
    }

    SubShader {
	ZTest Always // Always on top!
	Pass {
			Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
			uniform float _Fraction;
			uniform float _EdgeBlend;
			uniform fixed4 _Color;

            float4 frag(v2f_img i) : COLOR
			{
				const float PI = 3.14159;
				float4 result = tex2D(_MainTex, i.uv);
				float percentage = (atan2(i.uv.x-0.5 , i.uv.y-0.5) + PI)/(2*PI);

				if(percentage > _Fraction)
				{
					result.a *= 1 - (percentage - _Fraction + _EdgeBlend)/_EdgeBlend;
				}

                return result*_Color;
			}
            ENDCG
        }
    }
}