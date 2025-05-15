using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using Resources.Components;

namespace Resources.Systems
{
    [BurstCompile]
    public partial struct OrderFactorySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // This system doesn't run automatically - it's used as a service
        }

        public Entity CreateTransferOrder(ref SystemState state, Entity sourceEntity, Entity destinationEntity, int resourceID, float amount)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            // Create the order entity
            var orderEntity = ecb.CreateEntity();

            // Add transfer order component
            ecb.AddComponent(orderEntity, TransferOrderComponent.Create(
                sourceEntity,
                destinationEntity,
                resourceID,
                amount
            ));

            // Play the command buffer immediately
            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            return orderEntity;
        }
    }
}
