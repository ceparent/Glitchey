using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Entities;
using Glitchey.Rendering;
using Glitchey.Components;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Glitchey.Systems
{
    class CameraSystem : BaseSystem
    {

        public static Matrix4 projectionMatrix;
        public CameraSystem()
        {
            GL.Viewport(0, 0, GameOptions.GetVariable("w_width"), GameOptions.GetVariable("w_height"));

            float aspectRatio = GameOptions.GetVariable("w_width") / (float)GameOptions.GetVariable("w_height");

            float fov = MathHelper.PiOver3;

            float nearClip = 1.0f;

            float farClip = 5000.0f;

            projectionMatrix  = Matrix4.CreatePerspectiveFieldOfView(fov, aspectRatio, nearClip, farClip);
        }

        public static Matrix4 viewMatrix;
        public override void Update()
        {
            viewMatrix = Matrix4.LookAt(Camera.Position.PositionVec + Camera.HeadOffset.Offset, Camera.Position.PositionVec + Camera.HeadOffset.Offset + RotatedTarget, UpVector);
        }

        public static void Move(Vector3 pMovement, float pSpeed)
        {
            Matrix4 rotMatrix = Matrix4.CreateRotationX(Camera.Rotation.Pitch) * Matrix4.CreateRotationY(Camera.Rotation.Yaw);
            Vector3 rotatedVector = Vector3.Transform(pMovement, rotMatrix);

            Camera.Physic.RigidBody.Activate();
            Camera.Position.PositionVec += pSpeed * rotatedVector;
            Camera.Physic.RigidBody.Translate(pSpeed * rotatedVector);
        }

        public static void MoveWithoutY(Vector3 pMovement, float pSpeed)
        {
            Matrix4 rotMatrix = Matrix4.CreateRotationX(Camera.Rotation.Pitch) * Matrix4.CreateRotationY(Camera.Rotation.Yaw);
            Vector3 rotatedVector = Vector3.Transform(pMovement, rotMatrix);

            if (rotatedVector == Vector3.Zero)
                return;

            rotatedVector.Y = 0;
            rotatedVector.Normalize();

            Camera.Physic.RigidBody.Activate();
            Camera.Position.PositionVec += pSpeed * rotatedVector;
            Camera.Physic.RigidBody.Translate(pSpeed * rotatedVector);
        }

        public static Vector3 Target
        {
            get { return new Vector3(0, 0, -1); }
        }

        public static Vector3 UpVector
        {
            get { return new Vector3(0, 1, 0); }
        }

        public static Vector3 RotatedTarget
        {
            get
            {
                Matrix4 rotation = Matrix4.CreateRotationX(Camera.Rotation.Pitch) * Matrix4.CreateRotationY(Camera.Rotation.Yaw);
                return Vector3.Transform(Target, rotation);
            }

        }

        public static Player Camera
        {
            get { return _cameras[ActiveCamera]; }
        }

        public static List<Player> _cameras;
        public static int ActiveCamera = 0;
        public override void UpdateEntityList()
        {
            _cameras = new List<Entities.Player>();
            _entities = new List<Entity>();
            foreach (Entity e in _entityManager.Entities)
            {
                Player cam = e as Player;
                if(cam != null)
                    _cameras.Add(cam);
            }
        }

        public override void Dispose()
        {
            _cameras = null;
        }
    }
}
