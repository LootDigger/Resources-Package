using Unity.Entities;
using UnityEngine;
using Resources.Core;
using Resources.Components;

namespace Resources.Tests
{
    public class ResourceTestSetup : MonoBehaviour
    {
        [Tooltip("Resource definitions to test")]
        public ResourceDefinition[] resourceDefinitions;

        [Tooltip("Resource categories to test")]
        public ResourceCategory[] resourceCategories;

        [Tooltip("Whether to create test entities at runtime")]
        public bool createEntitiesAtRuntime = true;

        [Tooltip("Whether to log debug information")]
        public bool logDebugInfo = true;

        private EntityManager entityManager;
        private World defaultWorld;

        private void Start()
        {
            if (!createEntitiesAtRuntime)
                return;

            defaultWorld = World.DefaultGameObjectInjectionWorld;
            entityManager = defaultWorld.EntityManager;

            if (resourceDefinitions == null || resourceDefinitions.Length == 0)
            {
                Debug.LogWarning("No resource definitions assigned to test.");
                return;
            }

            CreateResourceEntities();
            CreateCategoryEntities();
        }

        private void CreateResourceEntities()
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

                if (resourceDef.Categories != null && resourceDef.Categories.Count > 0)
                {
                    var categoryBuffer = entityManager.AddBuffer<CategoryReference>(entity);

                    foreach (var category in resourceDef.Categories)
                    {
                        if (category != null)
                        {
                            categoryBuffer.Add(new CategoryReference(category.UniqueID));
                        }
                    }
                }
            }
        }

        private void CreateCategoryEntities()
        {
            if (resourceCategories == null || resourceCategories.Length == 0)
                return;

            foreach (var category in resourceCategories)
            {
                if (category == null)
                    continue;

                Entity entity = entityManager.CreateEntity();

                var categoryComponent = ResourceCategoryComponent.Create(
                    category.CategoryName,
                    category.UniqueID
                );

                entityManager.AddComponentData(entity, categoryComponent);
            }
        }
    }
}
