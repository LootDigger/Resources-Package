using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using Resources.Components;

namespace Resources.Systems
{
    [BurstCompile]
    public partial struct OrderDestructionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<OrderDestructionTag>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var destructionQuery = SystemAPI.QueryBuilder()
                .WithAll<OrderDestructionTag>()
                .Build();

            var entitiesToDestroy = destructionQuery.ToEntityArray(Allocator.Temp);

            foreach (var entity in entitiesToDestroy)
            {
                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            entitiesToDestroy.Dispose();
        }
    }
}
