using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Components;

using BulletSharp;
using OpenTK;

namespace Glitchey.Entities
{
    class Player : Entity, IPosition, IRotation, IPhysic, IHeadOffset
    {

        public Player(Vector3 position)
        {
            _position = new Position(position);
            _rotation = new Rotation();

            float height = 50;
            float width = 25;
            _headOffset = new Components.HeadOffset(new Vector3(0, height / 2, 0));
            CollisionShape shape = new CapsuleShape(width, height);
            float mass = 1f;
            _physic = new Physic(shape,mass, CollisionFlags.CharacterObject);
        }

        public override string Name
        {
            get { return "Player"; }
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

        private Physic _physic;
        public Physic Physic
        {
            get { return _physic; }
        }

        private HeadOffset _headOffset;
        public HeadOffset HeadOffset
        {
            get { return _headOffset; }
        }

    }
}
