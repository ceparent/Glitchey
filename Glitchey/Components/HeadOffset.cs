using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace Glitchey.Components
{
    interface IHeadOffset
    {
        HeadOffset HeadOffset { get; }
    }

    class HeadOffset : BaseComponent
    {
        public Vector3 Offset { get; set; }

        public HeadOffset(Vector3 offset)
        {
            Offset = offset;
        }
    }
}
