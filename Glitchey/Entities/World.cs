using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Components;

using System.Drawing;

namespace Glitchey.Entities
{
    class World : Entity, ILevel, IRender
    {
        public World(string mapName)
        {
            _level = new Level(mapName);
            _render = new Render(RenderType.Bsp);
        }

        public override string Name
        {
            get { return "World"; }
        }

        private Level _level;
        public Level Level
        {
            get { return _level; }
        }

        private Render _render;
        public Render Render
        {
            get { return _render; }
        }
    }
}
