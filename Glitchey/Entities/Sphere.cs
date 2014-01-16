using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Components;
using Glitchey.Rendering;
using Glitchey.Rendering.Shaders;

using OpenTK;
using BulletSharp;
using Glitchey.Systems;


namespace Glitchey.Entities
{
    class Sphere : Entity, IPosition, IRender, IPhysic
    {

        public override string Name
        {
            get { return "Sphere"; }
        }

        public Sphere(Vector3 position, float radius, string texture)
        {
            _position = new Position(position);
            LoadMesh(radius, texture);
        }

        private static TextureShader shader = new TextureShader();
        private void LoadMesh(float radius, string texture)
        {
            byte rings = (byte)(GameOptions.GetVariable("r_tesselation") * 2);
            byte segments = rings;
            var elements = DrawHelper.CalculateSphereElements(radius, radius, segments, rings);
            var vertices = DrawHelper.CalculateSphereVertices(radius, radius, segments, rings);

            var Ibuffer = DrawHelper.CreateIBuffer(elements);
            var Vbuffer = DrawHelper.CreateVBuffer(vertices);

            _render = new Render(RenderType.Mesh)
            {
                IBuffer = Ibuffer,
                Indices = elements,
                VBuffer = Vbuffer,
                Vertices = vertices,
                texture = Content.LoadTexture(texture),
                Shader = shader
            };

            SphereShape shape = new SphereShape(radius);
            _physic = new Physic(shape, radius / 20, CollisionFlags.None);

            Vector3 position = _position.PositionVec;
            Physic.RigidBody = PhysicSystem.LocalCreateRigidBody(Physic.Mass, Physic.Flags, position, Physic.Shape);
        }

        private Position _position;
        public Position Position
        {
            get { return _position; }
        }
        private Physic _physic;
        public Physic Physic
        {
            get { return _physic; }
        }
        private Render _render;
        public Render Render
        {
            get { return _render; }
        }

    }
}
