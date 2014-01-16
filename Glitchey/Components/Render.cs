using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Rendering;
using Glitchey.Rendering.Shaders;

namespace Glitchey.Components
{
    interface IRender
    {
        Render Render { get; }
    }
    public enum RenderType { Bsp, Mesh, Model, Billboard }
    class Render : BaseComponent
    {
        private RenderType _type;
        public RenderType RenderType
        {
            get { return _type; }
        }


        public Vertex[] Vertices { get; set; }
        public int VBuffer { get; set; }
        public uint[] Indices { get; set; }
        public int IBuffer { get; set; }
        public int texture { get; set; }
        public BaseShader Shader { get; set; }

        public Render(RenderType type)
        {
            _type = type;
        }
    }
}
