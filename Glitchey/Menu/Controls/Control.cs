using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glitchey.Menu.Controls
{
    abstract class Control
    {
        public abstract void Update(bool clicked, float x, float y);
        public abstract void Render();
    }
}
