﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Components;
using Glitchey.Rendering;
using Glitchey.Systems;

using System.Drawing;

using BulletSharp;
using OpenTK;

namespace Glitchey.Entities
{
    class GameWorld : Entity, ILevel, IRender, IPhysic
    {
        public GameWorld(string mapName)
        {
            _level = new Level(mapName);
            BspRenderer.BspFile = _level.BspFile;

            _render = new Render(RenderType.Bsp);
            _physic = new Physic(null, 0, CollisionFlags.StaticObject);


            Vector3 position = Vector3.Zero;
            Physic.RigidBody =  PhysicSystem.LocalCreateRigidBody(Physic.Mass, Physic.Flags, position, Physic.Shape);
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

        private Physic _physic;
        public Physic Physic
        {
            get { return _physic; }
        }
    }
}
