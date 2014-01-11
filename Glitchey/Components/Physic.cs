using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BulletSharp;
using OpenTK;

namespace Glitchey.Components
{
    interface IPhysic
    {
        Physic Physic { get; }
    }

    class Physic : BaseComponent
    {

        public Physic(CollisionShape shape, float mass, CollisionFlags flags)
        {
            _shape = shape;
            _mass = mass;
            _flags = flags;
        }

        private float _mass;
        public float Mass
        {
            get { return _mass; }
        }
        private CollisionFlags _flags;
        public CollisionFlags Flags
        {
            get { return _flags; }
        }

        private CollisionShape _shape;
        public CollisionShape Shape
        {
            get { return _shape; }
        }

        private RigidBody _rigidBody;
        public RigidBody RigidBody
        {
            get { return _rigidBody; }
            set { _rigidBody = value; }
        }
    }
}
