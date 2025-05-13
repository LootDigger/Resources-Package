using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using System.Collections.Generic;
using Resources.Core;
using Resources.Components;
using Resources.Utils;

namespace Resources.Tests
{
    public class ResourceCategoryTests : MonoBehaviour
    {
        [Tooltip("Resource definitions to test")]
        public ResourceDefinition[] resourceDefinitions;

        [Tooltip("Resource categories to test")]
        public ResourceCategory[] resourceCategories;

        [Tooltip("Category to filter resources by (optional)")]
        public ResourceCategory filterByCategory;

        [Tooltip("Whether to log debug information")]
        public bool logDebugInfo = true;

        private EntityManager entityManager;
        private World defaultWorld;
        private Dictionary<int, string> categoryNames = new Dictionary<int, string>();

        private void Start()
        {
            defaultWorld = World.DefaultGameObjectInjectionWorld;
            entityManager = defaultWorld.EntityManager;

            if ((resourceDefinitions == null || resourceDefinitions.Length == 0) &&
                (resourceCategories == null || resourceCategories.Length == 0))
            {
                Debug.LogWarning("No resource definitions or categories assigned to test.");
                return;
            }

            SetupEntities();
            QueryResourcesByCategory();
        }

        private void SetupEntities()
        {
            // Create category entities and build lookup dictionary
            if (resourceCategories != null)
            {
                foreach (var category in resourceCategories)
                {
                    if (category == null) continue;

                    // Store category name for lookup
                    categoryNames[category.UniqueID] = category.CategoryName;

                    // Create entity
                    Entity entity = entityManager.CreateEntity();
                    entityManager.AddComponentData(entity, ResourceCategoryComponent.Create(
                        category.CategoryName,
                        category.UniqueID
                    ));
                }
            }

            // Create resource entities with categories
            if (resourceDefinitions != null)
            {
                foreach (var resourceDef in resourceDefinitions)
                {
                    if (resourceDef == null) continue;

                    Entity entity = entityManager.CreateEntity();
                    entityManager.AddComponentData(entity, ResourceDefinitionComponent.Create(
                        resourceDef.ResourceName,
                        resourceDef.Description,
                        resourceDef.UniqueID
                    ));

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
        }

        private void QueryResourcesByCategory()
        {
            // Get all resources with categories
            EntityQuery resourceQuery = entityManager.CreateEntityQuery(
                typeof(ResourceDefinitionComponent),
                ComponentType.ReadOnly<CategoryReference>()
            );

            // Get entities and components
            using var entities = resourceQuery.ToEntityArray(Allocator.Temp);
            using var resourceDefs = resourceQuery.ToComponentDataArray<ResourceDefinitionComponent>(Allocator.Temp);

            if (logDebugInfo)
            {
                Debug.Log($"Found {entities.Length} resources with categories");

                // Display all resources and their categories
                for (int i = 0; i < entities.Length; i++)
                {
                    var entity = entities[i];
                    var resourceDef = resourceDefs[i];
                    var categoryBuffer = entityManager.GetBuffer<CategoryReference>(entity);

                    string categoryList = "";
                    for (int c = 0; c < categoryBuffer.Length; c++)
                    {
                        int categoryID = categoryBuffer[c].CategoryID;
                        string categoryName = categoryNames.ContainsKey(categoryID) ? categoryNames[categoryID] : $"Unknown ({categoryID})";
                        categoryList += (c > 0 ? ", " : "") + categoryName;
                    }

                    Debug.Log($"Resource: {resourceDef.ResourceName} belongs to categories: {categoryList}");
                }

                // Filter by specific category if provided
                if (filterByCategory != null)
                {
                    Debug.Log($"Filtering resources by category: {filterByCategory.CategoryName}");

                    for (int i = 0; i < entities.Length; i++)
                    {
                        var entity = entities[i];
                        var resourceDef = resourceDefs[i];
                        var categoryBuffer = entityManager.GetBuffer<CategoryReference>(entity);

                        if (categoryBuffer.BelongsToCategory(filterByCategory.UniqueID))
                        {
                            Debug.Log($"Resource in category {filterByCategory.CategoryName}: {resourceDef.ResourceName}");
                        }
                    }
                }
            }
        }
    }
}
