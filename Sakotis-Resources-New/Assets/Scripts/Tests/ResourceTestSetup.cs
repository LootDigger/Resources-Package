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

        [Tooltip("Whether to create resource containers")]
        public bool createContainers = false;

        [Tooltip("Initial values for resource containers")]
        public float[] containerInitialValues;

        [Tooltip("Minimum value resources for containers")]
        public ResourceDefinition[] minValueResources;

        [Tooltip("Maximum value resources for containers")]
        public ResourceDefinition[] maxValueResources;

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

            if (createContainers)
            {
                CreateResourceContainers();
            }
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

        private void CreateResourceContainers()
        {
            if (logDebugInfo)
            {
                Debug.Log("Creating resource containers...");
            }

            for (int i = 0; i < resourceDefinitions.Length; i++)
            {
                var resourceDef = resourceDefinitions[i];
                if (resourceDef == null) continue;

                // Get min/max value resource IDs if available
                int minValueResourceID = 0;
                int maxValueResourceID = 0;

                if (minValueResources != null && i < minValueResources.Length && minValueResources[i] != null)
                {
                    minValueResourceID = minValueResources[i].UniqueID;
                }

                if (maxValueResources != null && i < maxValueResources.Length && maxValueResources[i] != null)
                {
                    maxValueResourceID = maxValueResources[i].UniqueID;
                }

                // Create the container entity
                Entity containerEntity = entityManager.CreateEntity();
                float initialValue = (containerInitialValues != null && i < containerInitialValues.Length)
                    ? containerInitialValues[i]
                    : 0f;

                entityManager.AddComponentData(containerEntity, ResourceContainerComponent.Create(
                    resourceDef.UniqueID,
                    initialValue,
                    minValueResourceID,
                    maxValueResourceID
                ));

                if (logDebugInfo)
                {
                    string minResourceName = minValueResourceID > 0 && minValueResources != null && i < minValueResources.Length
                        ? minValueResources[i].ResourceName
                        : "None";

                    string maxResourceName = maxValueResourceID > 0 && maxValueResources != null && i < maxValueResources.Length
                        ? maxValueResources[i].ResourceName
                        : "None";

                    Debug.Log($"Created container for {resourceDef.ResourceName} with initial value: {initialValue}, " +
                              $"Min: {minResourceName} (ID: {minValueResourceID}), Max: {maxResourceName} (ID: {maxValueResourceID})");
                }
            }
        }
    }
}
