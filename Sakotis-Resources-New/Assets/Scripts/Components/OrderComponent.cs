using System;
using Unity.Entities;

namespace Resources.Components
{
    [Serializable]
    public struct OrderComponent : IComponentData
    {
        public int OrderTypeID;
        public float Priority;
        public bool IsProcessed;
        public Entity OwnerEntity;
        
        public static OrderComponent Create(int orderTypeID, float priority = 0, Entity ownerEntity = default)
        {
            return new OrderComponent
            {
                OrderTypeID = orderTypeID,
                Priority = priority,
                IsProcessed = false,
                OwnerEntity = ownerEntity
            };
        }
    }
}
