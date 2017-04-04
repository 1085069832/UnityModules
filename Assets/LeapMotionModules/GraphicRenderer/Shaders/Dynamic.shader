﻿Shader "Leap Motion/Graphic Renderer/Defaults/Dynamic" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
  }
  SubShader {
    Tags {"Queue"="Geometry" "RenderType"="Opaque" }

    Cull Off

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag

      #pragma shader_feature _ GRAPHIC_RENDERER_CYLINDRICAL GRAPHIC_RENDERER_SPHERICAL
      #pragma shader_feature _ GRAPHIC_RENDERER_VERTEX_NORMALS
      #pragma shader_feature _ GRAPHIC_RENDERER_VERTEX_UV_0
      #pragma shader_feature _ GRAPHIC_RENDERER_VERTEX_UV_1
      #pragma shader_feature _ GRAPHIC_RENDERER_VERTEX_UV_2
      #pragma shader_feature _ GRAPHIC_RENDERER_VERTEX_COLORS
      #pragma shader_feature _ GRAPHIC_RENDERER_MOVEMENT_TRANSLATION GRAPHIC_RENDERER_MOVEMENT_FULL
      #pragma shader_feature _ GRAPHIC_RENDERER_TINTING
      #pragma shader_feature _ GRAPHIC_RENDERER_BLEND_SHAPES
      #include "Assets/LeapMotionModules/GraphicRenderer/Resources/DynamicRenderer.cginc"
      #include "UnityCG.cginc"

      sampler2D _MainTex;
      
      v2f_graphic_dynamic vert (appdata_graphic_dynamic v) {
        return ApplyDynamicGraphics(v);
      }
      
      fixed4 frag (v2f_graphic_dynamic i) : SV_Target {
        fixed4 color = fixed4(1,1,1,1);

#ifdef GRAPHIC_RENDERER_VERTEX_NORMALS
        color *= dot(normalize(i.normal.xyz), float3(0, 0, 1));
#endif

#ifdef GRAPHIC_RENDERER_VERTEX_UV_0
        color *= tex2D(_MainTex, i.uv0);
#endif

#ifdef GRAPHICS_HAVE_COLOR
        color *= i.color;
#endif

        return color;
      }
      ENDCG
    }
  }
}
