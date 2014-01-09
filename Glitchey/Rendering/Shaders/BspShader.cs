using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Glitchey.Rendering.Shaders
{
    class BspShader
    {
        public BspShader()
        {
            CreateShaders();
            CreateProgram();
            QueryMatrixLocations();
        }

        string vertexShaderSource = @"
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

        string fragmentShaderSource = @"
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

        private void BindAttribLocations()
        {
            GL.BindAttribLocation(shaderProgramHandle, 0, "vertex_position");
            GL.BindAttribLocation(shaderProgramHandle, 1, "vertex_normal");
            GL.BindAttribLocation(shaderProgramHandle, 2, "texture_coords");
            GL.BindAttribLocation(shaderProgramHandle, 3, "lightmap_coords");
        }


        private void CreateProgram()
        {
            shaderProgramHandle = GL.CreateProgram();

            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);

            BindAttribLocations();

            if (GL.GetError() != ErrorCode.NoError)
                throw new Exception();

            GL.LinkProgram(shaderProgramHandle);

            string programInfoLog;
            GL.GetProgramInfoLog(shaderProgramHandle, out programInfoLog);
        }

        public int vertexShaderHandle;
        public int fragmentShaderHandle;
        public int shaderProgramHandle;

        private void CreateShaders()
        {
            vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertexShaderHandle, vertexShaderSource);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderSource);

            GL.CompileShader(vertexShaderHandle);
            GL.CompileShader(fragmentShaderHandle);
        }

        public int modelviewMatrixLocation;
        public int projectionMatrixLocation;
        public int textureLocation;
        public int lightmapLocation;

        private void QueryMatrixLocations()
        {
            projectionMatrixLocation = GL.GetUniformLocation(shaderProgramHandle, "projection_matrix");
            modelviewMatrixLocation = GL.GetUniformLocation(shaderProgramHandle, "modelview_matrix");
            textureLocation = GL.GetUniformLocation(shaderProgramHandle, "text");
            lightmapLocation = GL.GetUniformLocation(shaderProgramHandle, "lm");
        }
        Matrix4 projectionMatrix, modelviewMatrix;

        public void BindTexture(TextureUnit textureUnit, string UniformName)
        {
            GL.Uniform1(GL.GetUniformLocation(shaderProgramHandle, UniformName), textureUnit - TextureUnit.Texture0);
        }

        public void SetModelviewMatrix(Matrix4 matrix)
        {
            modelviewMatrix = matrix;
            GL.UniformMatrix4(modelviewMatrixLocation, false, ref modelviewMatrix);
        }

        public void SetProjectionMatrix(Matrix4 matrix)
        {
            projectionMatrix = matrix;
            GL.UniformMatrix4(projectionMatrixLocation, false, ref projectionMatrix);
        }

    }
}
