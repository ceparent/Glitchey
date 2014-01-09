using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace Glitchey.Components
{
    interface IPosition
    {
        Position Position { get; }
    }

    class Position:BaseComponent
    {
        public Position(Vector3 pos)
        {
            PositionVec = pos;
        }
        public Position()
        {
            PositionVec = Vector3.Zero;
        }
        public Vector3 PositionVec { get; set; }
    }
}
