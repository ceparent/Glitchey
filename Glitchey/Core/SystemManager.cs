using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Systems;

namespace Glitchey.Core
{
    class SystemManager
    {
        private List<BaseSystem> _systems;
        public SystemManager(EntityManager em)
        {
            _systems = new List<BaseSystem>();

            // BaseSystem loads the manager (static)
            BaseSystem.LoadManager(em);
        }

        public void AddSystem(BaseSystem system)
        {
            _systems.Add(system);
            _systems.Sort();
        }

        public void RemoveSystem(BaseSystem system)
        {
            _systems.Remove(system);
        }

        public void Update()
        {
            foreach (BaseSystem system in _systems)
            {
                if(system.Enabled)
                    system.Update();
            }
        }

        public void Render()
        {
            foreach (BaseSystem system in _systems)
            {
                if (system.Enabled)
                    system.Render();
            }
        }


    }
}
