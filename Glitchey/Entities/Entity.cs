using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Components;

namespace Glitchey.Entities
{
    abstract class Entity
    {
        private static int _idCount = 0;
        public Entity()
        {
            _id = _idCount++;
        }

        public abstract string Name
        {
            get;
        }

        private int _id;
        public int Id
        {
            get { return _id; }
        }
    }
}
