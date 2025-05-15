using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using Resources.Components;

namespace Resources.Systems
{
    [UpdateBefore(typeof(OrderDestructionSystem))]
    [BurstCompile]
    public partial struct TransferSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ResourceContainerComponent>();
            state.RequireForUpdate<TransferOrderComponent>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var entityManager = state.EntityManager;

            var transferQuery = SystemAPI.QueryBuilder()
                .WithAll<TransferOrderComponent>()
                .WithNone<OrderDestructionTag>()
                .Build();

            var transferEntities = transferQuery.ToEntityArray(Allocator.Temp);

            foreach (var transferEntity in transferEntities)
            {
                var transferOrder = SystemAPI.GetComponent<TransferOrderComponent>(transferEntity);

                if (!entityManager.Exists(transferOrder.SourceEntity) ||
                    !entityManager.Exists(transferOrder.DestinationEntity))
                {
                    ecb.AddComponent<OrderDestructionTag>(transferEntity);
                    continue;
                }

                if (!entityManager.HasComponent<ResourceContainerComponent>(transferOrder.SourceEntity) ||
                    !entityManager.HasComponent<ResourceContainerComponent>(transferOrder.DestinationEntity))
                {
                    ecb.AddComponent<OrderDestructionTag>(transferEntity);
                    continue;
                }

                var sourceContainer = entityManager.GetComponentData<ResourceContainerComponent>(transferOrder.SourceEntity);
                var destContainer = entityManager.GetComponentData<ResourceContainerComponent>(transferOrder.DestinationEntity);

                if (sourceContainer.ResourceID != transferOrder.ResourceID ||
                    destContainer.ResourceID != transferOrder.ResourceID)
                {
                    ecb.AddComponent<OrderDestructionTag>(transferEntity);
                    continue;
                }

                if (sourceContainer.CurrentValue < transferOrder.Amount)
                {
                    ecb.AddComponent<OrderDestructionTag>(transferEntity);
                    continue;
                }

                float newSourceValue = sourceContainer.CurrentValue - transferOrder.Amount;
                float newDestValue = destContainer.CurrentValue + transferOrder.Amount;

                float sourceMinValue = sourceContainer.GetMinValue(entityManager);
                float destMaxValue = destContainer.GetMaxValue(entityManager);

                if (newSourceValue < sourceMinValue || newDestValue > destMaxValue)
                {
                    ecb.AddComponent<OrderDestructionTag>(transferEntity);
                    continue;
                }

                sourceContainer.CurrentValue = newSourceValue;
                destContainer.CurrentValue = newDestValue;

                entityManager.SetComponentData(transferOrder.SourceEntity, sourceContainer);
                entityManager.SetComponentData(transferOrder.DestinationEntity, destContainer);

                ecb.AddComponent<OrderDestructionTag>(transferEntity);
            }

            ecb.Playback(entityManager);
            ecb.Dispose();
            transferEntities.Dispose();
        }
    }
}
