using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Glitchey.Rendering.Shaders
{
    class LightmapShader : BaseShader
    {
        protected override void LoadShaders()
        {
            vertexShaderSource = @"
            #version 140

            uniform mat4 modelview_matrix;            
            uniform mat4 projection_matrix;

            // Vertex data
            attribute vec3 vertex_position;
            attribute vec3 vertex_normal;
            attribute vec2 texture_coords;
            attribute vec2 lightmap_coords;

            out vec2 texCoordV;
            out vec2 lmCoord;
            void main(void)
            {
                texCoordV = texture_coords;
                lmCoord = lightmap_coords;
                gl_Position =  projection_matrix *  modelview_matrix * vec4( vertex_position, 1 );
            }";

            fragmentShaderSource = @"
            #version 140
            uniform sampler2D text;
            uniform sampler2D lm;
            
            in vec2 texCoordV;
            in vec2 lmCoord;
            
            out vec4 out_frag_color;
            void main(void)
            {
                out_frag_color = texture(lm, lmCoord) *  texture(text, texCoordV) * 4;
            }";
        }

        protected override void BindAttribLocations()
        {
            GL.BindAttribLocation(shaderProgramHandle, 0, "vertex_position");
            GL.BindAttribLocation(shaderProgramHandle, 1, "vertex_normal");
            GL.BindAttribLocation(shaderProgramHandle, 2, "texture_coords");
            GL.BindAttribLocation(shaderProgramHandle, 3, "lightmap_coords");
        }

        public int textureLocation;
        public int lightmapLocation;

        protected override void QueryUniformLocations()
        {
            textureLocation = GL.GetUniformLocation(shaderProgramHandle, "text");
            lightmapLocation = GL.GetUniformLocation(shaderProgramHandle, "lm");
        }


    }
}
