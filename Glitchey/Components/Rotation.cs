using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace Glitchey.Components
{
    interface IRotation
    {
        Rotation Rotation { get; }
    }
    class Rotation
    {
        public Rotation(float pitch = 0, float yaw = 0, float roll = 0)
        {
            Pitch = pitch;
            Yaw = yaw;
            Roll = roll;
        }

        public Vector3 RotationVector
        {
            get { return new Vector3(Pitch, Yaw, Roll); }
        }

        private float _pitch;
        public float Pitch
        {
            get { return _pitch; }
            set { _pitch = value; }
        }

        private float _yaw;
        public float Yaw
        {
            get { return _yaw; }
            set { _yaw = value; }
        }

        private float _roll;
        public float Roll
        {
            get { return _roll; }
            set { _roll = value; }
        }
    }
}
