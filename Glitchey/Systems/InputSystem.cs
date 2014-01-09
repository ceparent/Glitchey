﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Entities;
using Glitchey.Components;
using OpenTK.Input;
using OpenTK;

using Jitter;

namespace Glitchey.Systems
{
    class InputSystem : BaseSystem
    {
        public InputSystem()
        {
            oldKs = Keyboard.GetState();

            GameEvents.onStateChanged += GameEvents_onStateChanged;
        }

        private void GameEvents_onStateChanged(object sender, StateChangedEventArgs e)
        {
            if (e.State == ScreenState.Game)
                oldMs = Mouse.GetState();
        }

        KeyboardState oldKs;
        MouseState oldMs;
        public override void Update()
        {
            KeyboardState ks = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

            Vector3 left = new Vector3(-1, 0, 0);
            Vector3 right = new Vector3(1, 0, 0);
            Vector3 forward = new Vector3(0, 0, -1);

            Vector3 direction = Vector3.Zero;
            if (ks.IsKeyDown(Key.W))
                direction += forward;
            if (ks.IsKeyDown(Key.S))
                direction -= forward;
            if (ks.IsKeyDown(Key.A))
                direction += left;
            if (ks.IsKeyDown(Key.D))
                direction += right;

            if (direction != Vector3.Zero)
                direction.Normalize();

            CameraSystem.Move(direction, 10);

            float rotSpeed = -0.001f;
            _camera.Rotation.Yaw += (ms.X - oldMs.X) * rotSpeed;
            _camera.Rotation.Pitch += (ms.Y - oldMs.Y) * rotSpeed;

            if (ks.IsKeyDown(Key.Escape) && oldKs.IsKeyUp(Key.Escape))
                GameEvents.ChangeScreenState(ScreenState.Menu);


            oldKs = ks;
            oldMs = ms;
        }
        Camera _camera;
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
