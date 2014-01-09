using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Entities;
using Glitchey.Components;
using Glitchey.Rendering;

namespace Glitchey.Systems
{
    class RenderingSystem : BaseSystem
    {

        public override void Render()
        {
            foreach (IRender r in _entities)
            {
                switch (r.Render.RenderType)
                {
                    case RenderType.Bsp:
                        GameRenderer.RenderWorld(r as World);
                        break;
                    case RenderType.Model:
                        break;
                    case RenderType.Billboard:
                        break;
                    default:
                        break;
                }
            }

            //GameRenderer.ApplyLights();
        }

        public override void UpdateEntityList()
        {
            _entities = new List<Entities.Entity>();
            foreach (Entity e in _entityManager.Entities)
            {
                if (e is IRender)
                    _entities.Add(e);
            }
        }
    }
}
