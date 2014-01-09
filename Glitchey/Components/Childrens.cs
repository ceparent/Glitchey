using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Entities;
using Glitchey.Core;

namespace Glitchey.Components
{
    interface IChildren
    {
        Childrens Childrens { get; }
    }
    class Childrens
    {
        public Childrens()
        {
            _childrens = new List<Entity>();
        }
        public Childrens(List<Entity> childrens)
        {
            _childrens = childrens;
        }

        public void AddChildren(Entity children, EntityManager em)
        {
            _childrens.Add(children);
            em.AddEntity(children);
        }

        private List<Entity> _childrens;
        public List<T> GetChildrenByType<T>() where T : class
        {
            List<T> entities = new List<T>();

            foreach (Entity e in _childrens)
            {
                T entity = e as T;
                if (entity != null)
                    entities.Add(entity);
            }
            return entities;
        }

    }
}
