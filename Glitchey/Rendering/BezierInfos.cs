using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Levels;

namespace Glitchey.Rendering
{
    class BezierInfos
    {
        public face Face { get; set; }
        public int StartIndex { get; set; }
        public int NbIndex { get; set; }
        public List<vertex[]> Controls { get; set; }
        public int Start { get; set; }
        public int End { get; set; }

        public BezierInfos(face f)
        {
            Face = f;
            Controls = new List<vertex[]>();

        }
    }
}
