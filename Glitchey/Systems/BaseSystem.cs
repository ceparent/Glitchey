using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Core;
using Glitchey.Entities;

namespace Glitchey.Systems
{
    abstract class BaseSystem : IComparable<BaseSystem>
    {
        protected static EntityManager _entityManager;
        public static void LoadManager(EntityManager em)
        {
            _entityManager = em;
        }


        private static int _drawCount = 0;
        public static int _updateCount = 0;
        public BaseSystem()
        {
            Enabled = true;

            UpdateOrder = _updateCount++;
            DrawOrder = _drawCount++;
            _entityManager.EntityListChanged += OnEntityListChanged;

            UpdateEntityList();
        }

        private bool _enabled;
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        private int _updateOrder;
        public int UpdateOrder
        {
            get { return _updateOrder; }
            set { _updateOrder = value; }
        }

        private int _drawOrder;
        public int DrawOrder
        {
            get { return _drawOrder; }
            set { _drawOrder = value; }
        }

        public int CompareTo(BaseSystem other)
        {
            if (this.UpdateOrder == other.UpdateOrder)
                return 0;
            else if (this.UpdateOrder < other.UpdateOrder)
                return -1;
            else
                return 1;
        }

        public virtual void Render() { }
        public virtual void Update() { }


        private void OnEntityListChanged(object sender, EventArgs e)
        {
            UpdateEntityList();
        }
        protected List<Entity> _entities;
        public abstract void UpdateEntityList();
    }
}
