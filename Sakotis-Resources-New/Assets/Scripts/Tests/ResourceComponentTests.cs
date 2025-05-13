using Unity.Entities;
using UnityEngine;
using Resources.Core;
using Resources.Components;

namespace Resources.Tests
{
    public class ResourceComponentTests : MonoBehaviour
    {
        [Tooltip("Resource definitions to test")]
        public ResourceDefinition[] resourceDefinitions;

        [Tooltip("Whether to log debug information")]
        public bool logDebugInfo = true;

        private EntityManager entityManager;
        private World defaultWorld;

        private void Start()
        {
            defaultWorld = World.DefaultGameObjectInjectionWorld;
            entityManager = defaultWorld.EntityManager;

            if (resourceDefinitions == null || resourceDefinitions.Length == 0)
            {
                Debug.LogWarning("No resource definitions assigned to test.");
                return;
            }

            TestResourceDefinitionComponents();
        }

        private void TestResourceDefinitionComponents()
        {
            foreach (var resourceDef in resourceDefinitions)
            {
                if (resourceDef == null)
                    continue;

                Entity entity = entityManager.CreateEntity();

                var resourceComponent = ResourceDefinitionComponent.Create(
                    resourceDef.ResourceName,
                    resourceDef.Description,
                    resourceDef.UniqueID
                );

                if (resourceDef.Icon != null)
                {
                    Entity iconEntity = entityManager.CreateEntity();
                    resourceComponent.IconEntity = iconEntity;
                }

                entityManager.AddComponentData(entity, resourceComponent);
            }

            EntityQuery query = entityManager.CreateEntityQuery(typeof(ResourceDefinitionComponent));
            var resources = query.ToComponentDataArray<ResourceDefinitionComponent>(Unity.Collections.Allocator.Temp);
            resources.Dispose();
        }
    }
}
