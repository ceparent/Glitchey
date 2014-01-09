using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Entities;
using Glitchey.Rendering;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Glitchey.Systems
{
    class CameraSystem : BaseSystem
    {

        public static Matrix4 projectionMatrix;
        public CameraSystem()
        {
            GL.Viewport(0, 0, GameOptions.window_width, GameOptions.window_height);

            float aspectRatio = GameOptions.viewport_width / (float)GameOptions.viewport_height;

            float fov = MathHelper.PiOver3;

            float nearClip = 1.0f;

            float farClip = 5000.0f;

            projectionMatrix  = Matrix4.CreatePerspectiveFieldOfView(fov, aspectRatio, nearClip, farClip);
        }

        public static Matrix4 viewMatrix;

        public override void Update()
        {
            viewMatrix = Matrix4.LookAt(_camera.Position.PositionVec, _camera.Position.PositionVec + RotatedTarget, UpVector);
        }

        public static void Move(Vector3 pMovement, float pSpeed)
        {
            Matrix4 rotMatrix = Matrix4.CreateRotationX(_camera.Rotation.Pitch) * Matrix4.CreateRotationY(_camera.Rotation.Yaw);
            Vector3 rotatedVector = Vector3.Transform(pMovement, rotMatrix);
            _camera.Position.PositionVec += pSpeed * rotatedVector;

        }

        public Vector3 Target
        {
            get { return new Vector3(0, 0, -1); }
        }

        public Vector3 UpVector
        {
            get { return new Vector3(0, 1, 0); }
        }

        public Vector3 RotatedTarget
        {
            get
            {
                Matrix4 rotation = Matrix4.CreateRotationX(_camera.Rotation.Pitch) * Matrix4.CreateRotationY(_camera.Rotation.Yaw);
                return Vector3.Transform(Target, rotation);
            }

        }

        public static Camera _camera;
        public override void UpdateEntityList()
        {
            _entities = new List<Entity>();
            foreach (Entity e in _entityManager.Entities)
            {
                Camera cam = e as Camera;
                _camera = cam;
            }
        }
    }
}
