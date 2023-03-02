// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sprites/Custom/MinnaShader"
{
	Properties
	{
		_SwitchColor ("Switch Color", Color) = (0,0,0,0)
		_TargetColor ("Target Color", Color) = (0,0,0,0)
		_Treshold("Treshold", Float) = 0
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}



		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord  : TEXCOORD0;
			};

			fixed4 _Color;
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				return OUT;
			}
				
			sampler2D _MainTex;
			fixed4 _SwitchColor;
			fixed4 _TargetColor;
			float _Treshold;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, IN.texcoord);
				if (distance(c.r, _SwitchColor.r) < _Treshold && distance(c.g, _SwitchColor.g) < _Treshold && distance(c.b, _SwitchColor.b) < _Treshold){
					c.rgb = _TargetColor.rgb;
				}
				
				return c;
			}
		ENDCG
		}
	}
}
