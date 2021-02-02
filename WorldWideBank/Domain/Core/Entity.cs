using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldWideBankSample.Utility;

namespace WorldWideBankSample.Domain.Core
{
    public interface IEntity
    {
        long Id { get; }
    }

    public abstract class Entity : IEntity
    {
        public const int NOTSET = -1;

        protected Entity()
        {
            Id = DefaultID;
        }

        public virtual long Id { get; protected set; }

        public static int DefaultID
        {
            get { return -1; }
        }

        protected virtual bool Equals(Entity obj)
        {
            if (obj.Id.Equals(DefaultID) && Id.Equals(DefaultID))
                return false;

            return obj.Id == Id;
        }

        public override bool Equals(object obj)
        {
            var entity = obj.DowncastTo<Entity>();
            if (ReferenceEquals(null, entity)) return false;
            if (ReferenceEquals(this, entity)) return true;

            return Equals(entity);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", GetType().Name, Id);
        }

        public static bool IsThisSelected(IEntity entity, Type selectedItemType, long selectedId)
        {
            return
                selectedItemType != null && entity != null &&
                selectedItemType.IsAssignableFrom(entity.GetType()) &&
                entity.Id == selectedId;
        }


    }
    
}
