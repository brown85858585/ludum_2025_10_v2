Shader "Projector/Additive" { 
   Properties { 
      _ShadowTex ("Cookie", 2D) = ""
      _FalloffTex ("FallOff", 2D) = ""
   } 
   Subshader { 
      Pass { 
         ZWrite off 
         Fog { Color (1, 1, 1) } 
         ColorMask RGB 
         Blend One One 
         SetTexture [_ShadowTex] { 
            combine texture, ONE - texture 
         } 
         SetTexture [_FalloffTex] { 
            constantColor (0,0,0,0) 
            combine previous lerp (texture) constant 
         } 
      }
   }
}