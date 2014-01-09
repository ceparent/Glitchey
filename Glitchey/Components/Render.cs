using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glitchey.Components
{
    interface IRender
    {
        Render Render { get; }
    }
    public enum RenderType { Bsp, Model, Billboard }
    class Render : BaseComponent
    {
        private RenderType _type;
        public RenderType RenderType
        {
            get { return _type; }
        }

        public Render(RenderType type)
        {
            _type = type;
        }
    }
}
