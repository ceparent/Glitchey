using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Glitchey.Rendering.Shaders
{
    class TextureShader : BaseShader
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

            out vec2 texCoordV;
            void main(void)
            {
                texCoordV = texture_coords;
                gl_Position =  projection_matrix *  modelview_matrix * vec4( vertex_position, 1 );
            }";

            fragmentShaderSource = @"
            #version 140
            uniform sampler2D text;
            
            in vec2 texCoordV;
            
            out vec4 out_frag_color;
            void main(void)
            {
                out_frag_color = vec4(texture(text, texCoordV).rgb * 0.7 , 1);
            }";
        }

        protected override void BindAttribLocations()
        {
            GL.BindAttribLocation(shaderProgramHandle, 0, "vertex_position");
            GL.BindAttribLocation(shaderProgramHandle, 1, "vertex_normal");
            GL.BindAttribLocation(shaderProgramHandle, 2, "texture_coords");
        }

        public int textureLocation;
        protected override void QueryUniformLocations()
        {
            textureLocation = GL.GetUniformLocation(shaderProgramHandle, "text");
        }


    }
}
