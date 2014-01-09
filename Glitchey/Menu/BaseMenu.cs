using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glitchey.Menu
{
    abstract class BaseMenu
    {

        public abstract void Load();
        public abstract void Render();
        public abstract void Click(bool click, float x, float y);

    }
}
