﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Entities;
using Glitchey.Components;
using Glitchey.Rendering;

using OpenTK.Graphics.OpenGL;

namespace Glitchey.Systems
{
    class RenderingSystem : BaseSystem
    {
        public RenderingSystem()
        {
        }

        public override void Update()
        {
            
            if (BspRenderer.BspFile != null)
                BspRenderer.LoadIndexBuffer();
        }

        public override void Render()
        {
            if (GL.GetError() != ErrorCode.NoError)
                throw new Exception();

            foreach (IRender r in _entities)
            {
                switch (r.Render.RenderType)
                {
                    case RenderType.Bsp:
                        
                        GameRenderer.RenderWorld(r as GameWorld);

                        break;
                    case RenderType.Mesh:
                        GameRenderer.RenderMesh(r);
                        break;
                    case RenderType.Model:
                        break;
                    case RenderType.Billboard:
                        break;
                    default:
                        break;
                }
            }

            if (GL.GetError() != ErrorCode.NoError)
                throw new Exception();

        }

        
        public override void UpdateEntityList()
        {
            _entities = new List<Entities.Entity>();
            foreach (Entity e in _entityManager.Entities)
            {
                if (e is IRender)
                {
                    _entities.Add(e);
                }
            }
        }

        public override void Dispose()
        {
            BspRenderer.DisposeData();
        }
    }
}
