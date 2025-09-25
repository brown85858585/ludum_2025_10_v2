Shader "Self-Illumin/VertexLitOffset1" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Spec Color", Color) = (1,1,1,1)
	_Shininess ("Shininess", Range (0.1, 1)) = 0.7
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Illum ("Illumin (A)", 2D) = "white" {}
	_EmissionLM ("Emission (Lightmapper)", Float) = 0
}

// ------------------------------------------------------------------
// Dual texture cards

SubShader {
	LOD 100
	Ztest Always
	Tags { "Queue" = "Overlay+11" "RenderType"="Overlay" "IgnoreProjector" = "True"}
	
	Pass {
		Name "BASE"
		Tags {"LightMode" = "Vertex"}
		Material {
			Diffuse [_Color]
			Shininess [_Shininess]
			Specular [_SpecColor]
		}
		SeparateSpecular On
		Lighting On
		SetTexture [_Illum] {
			constantColor [_Color]
			combine constant lerp (texture) previous
		}
		SetTexture [_MainTex] {
			Combine texture * previous, texture*primary
		}
	}
}

Fallback "VertexLit"
}
