Shader "FX/DirtyLens" {
	Properties {
		_Color ("Main Tint", Color) = (0.5, 0.5, 0.5, 0.5)
		_MainTex ("Texture", 2D) = "white" {}
		_RimPower("Rim Power", Range(0.01, 5.0)) = 1.5
    }
    
    SubShader 
    {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha One
		//AlphaTest Greater .01
		Cull Off Lighting On ZWrite Off
		LOD 200
		
		CGPROGRAM
		#pragma surface surf HalfLambertRim noambient 

		struct Input
		{
			float2 uv_MainTex;
			float3 viewDir;
		};
		
		sampler2D _MainTex;
		fixed4 _Color;
		fixed _RimPower;
		
		
		// The dirty Lens shader is based on a Lambert Rim shader, using the viewDirection
		// and the ligthDirection to calculate the 'rim' of the Alpha channel (instead of using a
		// usual color rim depending on object's normals).
		inline half4 LightingHalfLambertRim (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) 
		{
			//#ifndef USING_DIRECTIONAL_LIGHT
		    lightDir = normalize(-lightDir);	
		    //#endif
    
			// Calculate the half vector
			half3 halfVector = normalize (lightDir + viewDir);
			
			// Difusse Lighting
			half NDotL = max (0, dot (s.Normal, lightDir));
			
			// more dot products
			fixed EdotH = max(0, dot(viewDir, halfVector));
			fixed NdotH = max(0, dot(s.Normal, halfVector));
			fixed NdotE = max(0, dot(s.Normal, viewDir));
			
			//Half Lambert
			fixed4 halfLambert = pow (NDotL * 0.5 + 0.1, 2.0);
			
			// Rim Light
			fixed rimLight = EdotH;
			rimLight = pow(rimLight, _RimPower);
			
			fixed4 finalColor;
			
			finalColor.rgb = (s.Albedo * _LightColor0.rgb) * (halfLambert * atten * 2);
			finalColor.a = rimLight;
			return finalColor;
		}
		
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * _Color.rgb;
			o.Alpha = c.a * _Color.a;
		}
		ENDCG
    }
    Fallback "Particles/Additive"
  }
