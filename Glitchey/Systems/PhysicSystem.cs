﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Entities;
using Glitchey.Components;
using Glitchey.Levels;
using Glitchey.Rendering;

using OpenTK;
using BulletSharp;
using OpenTK.Graphics.OpenGL4;

namespace Glitchey.Systems
{
    class PhysicSystem : BaseSystem
    {
        GameWorld _gameWorld;


        CollisionConfiguration CollisionConf;
        CollisionDispatcher Dispatcher;
        BroadphaseInterface Broadphase;
        ConstraintSolver Solver;

        public static DynamicsWorld World;

        public PhysicSystem()
        {
            InitializePhysics();
            SetUpWorld(1f);
            UpdateEntityList();

            GameOptions.ValueChanged += GameOptions_ValueChanged;

            

            World.DebugDrawer = new DebugDrawer();
            World.DebugDrawer.DebugMode = DebugDrawModes.DrawWireframe;
            isDrawingDebug = GameOptions.GetVariable("phys_debug") == 1;


            
        }

        void GameOptions_ValueChanged(object sender, OptionsChangedArgs e)
        {
            switch (e.Variable)
            {
                case "phys_debug":
                    isDrawingDebug = e.Value == 1;
                    break;
            }
        }

        DateTime lastStep = DateTime.Now;
        public override void Update()
        {
            DateTime now = DateTime.Now;
            World.StepSimulation((float)(now - lastStep).TotalMilliseconds);
            lastStep = now;

            foreach (IPhysic physic in _entities)
            {
                IPosition position = physic as IPosition;
                if (physic.Physic.RigidBody == null || position == null)
                    continue;

                Matrix4 motion;
                physic.Physic.RigidBody.GetWorldTransform(out motion);

                position.Position.PositionVec = Vector3.Transform(Vector3.Zero, motion);
            }
            
        }

        DateTime lastDebugDrawUpdate = DateTime.MinValue;
        Vector3[] debugVertices;
        bool isDrawingDebug;
        public override void Render()
        {

            if (!isDrawingDebug)
                return;

            if (DateTime.Now - lastDebugDrawUpdate > TimeSpan.FromSeconds(10))
            {
                DebugDrawer drawer = World.DebugDrawer as DebugDrawer;
                drawer.LineVertices = new List<Vector3>();
                World.DebugDrawWorld();
                debugVertices = drawer.LineVertices.ToArray();
                lastDebugDrawUpdate = DateTime.Now;
            }


            GameRenderer.RenderDebugLines(debugVertices);


        }

        private HashSet<RigidBody> _bodies = new HashSet<RigidBody>();
        public override void UpdateEntityList()
        {
            _entities = new List<Entity>();
            foreach (Entity e in _entityManager.Entities)
            {
                IPhysic phys = e as IPhysic;
                if (phys != null)
                {
                    _entities.Add(e);
                    if (World != null && !_bodies.Contains(phys.Physic.RigidBody))
                    {
                        World.AddRigidBody(phys.Physic.RigidBody);
                        _bodies.Add(phys.Physic.RigidBody);
                    }
                        
                    if (e is GameWorld)
                        _gameWorld = (GameWorld)e;
                }
            }
        }

        private void InitializePhysics()
        {
            // collision configuration contains default setup for memory, collision setup
            CollisionConf = new DefaultCollisionConfiguration();
            Dispatcher = new CollisionDispatcher(CollisionConf);

            Broadphase = new DbvtBroadphase();
            Solver = new SequentialImpulseConstraintSolver();

            CollisionShapes = new AlignedCollisionShapeArray();

            World = new DiscreteDynamicsWorld(Dispatcher, Broadphase, Solver, CollisionConf);

            int grav = GameOptions.GetVariable("sv_gravity");
            World.Gravity = new Vector3(0, -grav, 0);
        }

        private void SetUpWorld(float scaling)
        {

            if (_gameWorld == null)
                return;

            AddBsp(scaling);
        }

        private void AddBsp(float scaling)
        {
            BspFile bsp = _gameWorld.Level.BspFile;

            foreach (leaf leaf in bsp.Leaves)
            {
                bool isValidBrush = false;
                for (int b = 0; b < leaf.N_LeafBrushes; b++)
                {
                    AlignedVector3Array planeEquations = new AlignedVector3Array();

                    int brushId = bsp.LeafBrushes[leaf.LeafBrush + b].Brush;
                    brush brush = bsp.Brushes[brushId];

                    if (brush.Texture != -1)
                    {
                        int flags = bsp.Textures[brush.Texture].Contents;

                        int solidFlag = 1;
                        if ((flags & solidFlag) == solidFlag)
                        {
                            brush.Texture = -1;

                            for (int p = 0; p < brush.N_BrushSides; p++)
                            {
                                int sideId = brush.BrushSide + p;
                                brushside brushSide = bsp.BrusheSides[sideId];

                                int planeId = brushSide.Plane;
                                plane plane = bsp.Planes[planeId];

                                Vector4 planeEq = new Vector4(plane.Normal[0], plane.Normal[2], -plane.Normal[1], scaling * -plane.Dist);
                                planeEquations.Add(planeEq);
                                isValidBrush = true;
                            }

                            if (isValidBrush)
                            {
                                AlignedVector3Array vertices;
                                GeometryUtil.GetVerticesFromPlaneEquations(planeEquations, out vertices);


                                const bool isEntity = false;
                                Vector3 entityTarget = Vector3.Zero;

                                AddConvexVerticesCollider(vertices, isEntity, CollisionFlags.StaticObject, entityTarget);

                            }
                        }


                    }


                }
            }
        }


        public static AlignedCollisionShapeArray CollisionShapes { get; private set; }
        private  void AddConvexVerticesCollider(AlignedVector3Array vertices, bool isEntity, CollisionFlags flags, Vector3 entityTargLocation)
        {
            if (vertices.Count == 0)
                return;

            float mass = 0.0f;

            CollisionShape shape = new ConvexHullShape(vertices);
            CollisionShapes.Add(shape);

            var body = LocalCreateRigidBody(mass, flags, entityTargLocation, shape);
            World.AddRigidBody(body);
        }

        public static RigidBody LocalCreateRigidBody(float mass, CollisionFlags flags, Vector3 startPos,  CollisionShape shape)
        {
            bool isDynamic = (mass != 0.0f);

            Vector3 localInertia = Vector3.Zero;
            if (isDynamic)
                shape.CalculateLocalInertia(mass, out localInertia);

            DefaultMotionState motion = new DefaultMotionState(Matrix4.Identity);

           
            RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(mass, motion, shape, localInertia);
            RigidBody body = new RigidBody(rbInfo);
            body.CollisionFlags = flags;

            if (flags == CollisionFlags.CharacterObject)
                body.AngularFactor = Vector3.Zero;

            body.Translate(startPos);
            
            rbInfo.Dispose();
            
            return body;
        }


        public override void Dispose()
        {
            World.Dispose();
            World = null;
            CollisionShapes.Dispose();
            CollisionShapes = null;
        }
    }
}
