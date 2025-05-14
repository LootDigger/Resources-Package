using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using System.Collections.Generic;
using Resources.Core;
using Resources.Components;

namespace Resources.Tests
{
    public class ResourceContainerTests : MonoBehaviour
    {
        [Tooltip("Resource definitions to test")]
        public ResourceDefinition[] resourceDefinitions;

        [Tooltip("Minimum value resource definitions")]
        public ResourceDefinition[] minValueResources;

        [Tooltip("Maximum value resource definitions")]
        public ResourceDefinition[] maxValueResources;

        [Tooltip("Initial values for each resource container")]
        public float[] initialValues;

        [Tooltip("Whether to log debug information")]
        public bool logDebugInfo = true;

        private EntityManager entityManager;
        private World defaultWorld;
        private Dictionary<int, string> resourceNames = new Dictionary<int, string>();

        private void Start()
        {
            defaultWorld = World.DefaultGameObjectInjectionWorld;
            entityManager = defaultWorld.EntityManager;

            if (resourceDefinitions == null || resourceDefinitions.Length == 0)
            {
                Debug.LogWarning("No resource definitions assigned to test.");
                return;
            }

            // Store resource names for logging
            foreach (var resource in resourceDefinitions)
            {
                if (resource != null)
                {
                    resourceNames[resource.UniqueID] = resource.ResourceName;
                }
            }

            if (minValueResources != null)
            {
                foreach (var resource in minValueResources)
                {
                    if (resource != null && !resourceNames.ContainsKey(resource.UniqueID))
                    {
                        resourceNames[resource.UniqueID] = resource.ResourceName;
                    }
                }
            }

            if (maxValueResources != null)
            {
                foreach (var resource in maxValueResources)
                {
                    if (resource != null && !resourceNames.ContainsKey(resource.UniqueID))
                    {
                        resourceNames[resource.UniqueID] = resource.ResourceName;
                    }
                }
            }

            CreateResourceDefinitions();
            CreateResourceContainers();
        }

        private void CreateResourceDefinitions()
        {
            if (logDebugInfo)
            {
                Debug.Log("Creating resource definition entities...");
            }

            // Create resource definition entities for all resources
            CreateResourceDefinitionEntities(resourceDefinitions);
            CreateResourceDefinitionEntities(minValueResources);
            CreateResourceDefinitionEntities(maxValueResources);
        }

        private void CreateResourceDefinitionEntities(ResourceDefinition[] definitions)
        {
            if (definitions == null) return;

            foreach (var resourceDef in definitions)
            {
                if (resourceDef == null) continue;

                // Check if we already created this resource definition
                EntityQuery query = entityManager.CreateEntityQuery(
                    ComponentType.ReadOnly<ResourceDefinitionComponent>()
                );

                // Use a different approach since ResourceDefinitionComponent is not a shared component
                bool alreadyExists = false;
                using var existingResources = query.ToComponentDataArray<ResourceDefinitionComponent>(Allocator.Temp);
                foreach (var res in existingResources)
                {
                    if (res.UniqueID == resourceDef.UniqueID)
                    {
                        alreadyExists = true;
                        break;
                    }
                }

                if (alreadyExists)
                {
                    continue; // Skip if already created
                }

                // Create the resource definition entity
                Entity resourceEntity = entityManager.CreateEntity();
                entityManager.AddComponentData(resourceEntity, ResourceDefinitionComponent.Create(
                    resourceDef.ResourceName,
                    resourceDef.Description,
                    resourceDef.UniqueID
                ));

                if (logDebugInfo)
                {
                    Debug.Log($"Created resource definition for {resourceDef.ResourceName} with ID: {resourceDef.UniqueID}");
                }
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
                float initialValue = (initialValues != null && i < initialValues.Length) ? initialValues[i] : 0f;

                entityManager.AddComponentData(containerEntity, ResourceContainerComponent.Create(
                    resourceDef.UniqueID,
                    initialValue,
                    minValueResourceID,
                    maxValueResourceID
                ));

                if (logDebugInfo)
                {
                    string minResourceName = minValueResourceID > 0 && resourceNames.ContainsKey(minValueResourceID)
                        ? resourceNames[minValueResourceID]
                        : "None";

                    string maxResourceName = maxValueResourceID > 0 && resourceNames.ContainsKey(maxValueResourceID)
                        ? resourceNames[maxValueResourceID]
                        : "None";

                    Debug.Log($"Created container for {resourceDef.ResourceName} with initial value: {initialValue}, " +
                              $"Min: {minResourceName} (ID: {minValueResourceID}), Max: {maxResourceName} (ID: {maxValueResourceID})");
                }
            }
        }
    }
}
