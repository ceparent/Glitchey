using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Entities;
using Glitchey.Components;

namespace Glitchey.Core
{

    class EntityManager
    {
        public event EventHandler EntityListChanged;

        Dictionary<int, Entity> _entities;
        List<Entity> _entityList;
        public EntityManager()
        {
            _entities = new Dictionary<int, Entity>();
            _entityList = new List<Entity>();
        }

        public void AddEntity(Entity e)
        {
            _entities.Add(e.Id, e);
            _entityList.Add(e);

            // Adds childrens recursively
            IChildren parent = e as IChildren;
            if (parent != null)
            {
                foreach (Entity c in parent.Childrens.GetChildrenByType<Entity>())
                {
                    AddEntity(c);
                }
            }

            if (EntityListChanged != null)
                EntityListChanged(this, EventArgs.Empty);
        }

        public void RemoveEntity(Entity e)
        {
            _entities.Remove(e.Id);
            _entityList.Remove(e);

            if (EntityListChanged != null)
                EntityListChanged(this, EventArgs.Empty);
        }

        public Entity getEntityById(int id)
        {
            return _entities[id];
        }
        public List<Entity> Entities
        {
            get { return _entityList; }
        }

    }
}
