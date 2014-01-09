using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glitchey.Screens
{
    abstract class BaseScreen
    {
        public abstract void Load();
        public abstract void Update();
        public abstract void Render();
        
    }
}
