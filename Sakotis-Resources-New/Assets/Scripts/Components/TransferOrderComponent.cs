using System;
using Unity.Entities;

namespace Resources.Components
{
    [Serializable]
    public struct TransferOrderComponent : IComponentData
    {
        public Entity SourceEntity;
        public Entity DestinationEntity;
        public int ResourceID;
        public float Amount;

        public static TransferOrderComponent Create(Entity sourceEntity, Entity destinationEntity, int resourceID, float amount)
        {
            return new TransferOrderComponent
            {
                SourceEntity = sourceEntity,
                DestinationEntity = destinationEntity,
                ResourceID = resourceID,
                Amount = amount
            };
        }
    }
}
