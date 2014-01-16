using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Glitchey.Rendering.Shaders
{
    abstract class BaseShader
    {
        public BaseShader()
        {
            LoadShaders();
            CreateShaders();
            CreateProgram();
            QueryMatrixLocations();
            QueryUniformLocations();
        }

        public int vertexShaderHandle;
        public int fragmentShaderHandle;
        public int shaderProgramHandle;

        protected string vertexShaderSource { get; set; }
        protected string fragmentShaderSource { get; set; }

        protected abstract void BindAttribLocations();
        protected abstract void LoadShaders();

        protected abstract void QueryUniformLocations();

        private void QueryMatrixLocations()
        {
            projectionMatrixLocation = GL.GetUniformLocation(shaderProgramHandle, "projection_matrix");
            modelviewMatrixLocation = GL.GetUniformLocation(shaderProgramHandle, "modelview_matrix");
        }

        protected void CreateShaders()
        {
            vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertexShaderHandle, vertexShaderSource);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderSource);

            GL.CompileShader(vertexShaderHandle);
            GL.CompileShader(fragmentShaderHandle);
        }

        protected void CreateProgram()
        {
            shaderProgramHandle = GL.CreateProgram();

            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);

            BindAttribLocations();

            if (GL.GetError() != ErrorCode.NoError)
                throw new Exception();

            GL.LinkProgram(shaderProgramHandle);

            if (GL.GetError() != ErrorCode.NoError)
                throw new Exception();
            string programInfoLog;
            GL.GetProgramInfoLog(shaderProgramHandle, out programInfoLog);
        }

        public void BindTexture(TextureUnit textureUnit, string UniformName)
        {
            GL.Uniform1(GL.GetUniformLocation(shaderProgramHandle, UniformName), textureUnit - TextureUnit.Texture0);
        }

        Matrix4 projectionMatrix, modelviewMatrix;

        public int modelviewMatrixLocation;
        public int projectionMatrixLocation;

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
