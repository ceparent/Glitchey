using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glitchey.Components
{
    interface IMove
    {
        Move Move { get; }
    }

    class Move
    {
        public Move(float speed)
        {
            Speed = speed;
        }

        public float Speed { get; set; }
    }
}
