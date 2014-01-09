using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Levels;

namespace Glitchey.Components
{
    interface ILevel
    {
        Level Level { get; }
    }

    class Level
    {
        private BspFile _bsp;
        public BspFile BspFile
        {
            get { return _bsp; }
            set { _bsp = value; }
        }


        public Level(string mapName)
        {
            _bsp = new BspFile("Content/" + mapName);
        }
    }
}
