using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Systems;
using OpenTK.Graphics.OpenGL4;

namespace Glitchey.Core
{
    class SystemManager : IDisposable
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

                if (GL.GetError() != ErrorCode.NoError)
                    throw new Exception();
            }
        }



        public void Dispose()
        {
            foreach (BaseSystem sys in _systems)
            {
                sys.Dispose();
            }
        }
    }
}
