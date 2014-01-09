using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Components;

namespace Glitchey.Entities
{
    class Camera : Entity, IPosition, IRotation
    {

        public Camera()
        {
            _position = new Position();
            _rotation = new Rotation();
        }

        public override string Name
        {
            get { return "Camera"; }
        }

        private Position _position;
        public Position Position
        {
            get { return _position; }
        }

        private Rotation _rotation;
        public Rotation Rotation
        {
            get { return _rotation; }
        }
    }
}
