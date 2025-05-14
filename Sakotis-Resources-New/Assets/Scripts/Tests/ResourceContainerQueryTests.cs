using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using System.Collections.Generic;
using Resources.Core;
using Resources.Components;

namespace Resources.Tests
{
    /// <summary>
    /// Test script for querying resource containers and their constraints
    /// </summary>
    public class ResourceContainerQueryTests : MonoBehaviour
    {
        [Tooltip("Whether to log debug information")]
        public bool logDebugInfo = true;

        private EntityManager entityManager;
        private World defaultWorld;
        private Dictionary<int, string> resourceNames = new Dictionary<int, string>();

        private void Start()
        {
            defaultWorld = World.DefaultGameObjectInjectionWorld;
            entityManager = defaultWorld.EntityManager;

            // Build resource name dictionary for logging
            BuildResourceNameDictionary();
            
            // Query all containers and their constraints
            QueryAllContainers();
        }

        private void BuildResourceNameDictionary()
        {
            // Query all resource definitions to build a name lookup dictionary
            EntityQuery resourceQuery = entityManager.CreateEntityQuery(typeof(ResourceDefinitionComponent));
            using var resourceDefs = resourceQuery.ToComponentDataArray<ResourceDefinitionComponent>(Allocator.Temp);
            
            foreach (var resourceDef in resourceDefs)
            {
                resourceNames[resourceDef.UniqueID] = resourceDef.ResourceName.ToString();
            }
            
            if (logDebugInfo)
            {
                Debug.Log($"Found {resourceNames.Count} resource definitions for name lookup");
            }
        }

        /// <summary>
        /// Queries all resource containers and logs their current values and constraints
        /// </summary>
        private void QueryAllContainers()
        {
            if (logDebugInfo)
            {
                Debug.Log("Querying all resource containers...");
            }

            // Query all resource containers
            EntityQuery containerQuery = entityManager.CreateEntityQuery(typeof(ResourceContainerComponent));
            
            using var containers = containerQuery.ToComponentDataArray<ResourceContainerComponent>(Allocator.Temp);
            using var containerEntities = containerQuery.ToEntityArray(Allocator.Temp);
            
            if (logDebugInfo)
            {
                Debug.Log($"Found {containers.Length} resource containers");
                
                for (int i = 0; i < containers.Length; i++)
                {
                    LogContainerInfo(containers[i]);
                }
            }
        }
        
        /// <summary>
        /// Gets all resource containers in the world
        /// </summary>
        /// <returns>Array of resource containers</returns>
        public ResourceContainerComponent[] GetAllContainers()
        {
            EntityQuery containerQuery = entityManager.CreateEntityQuery(typeof(ResourceContainerComponent));
            return containerQuery.ToComponentDataArray<ResourceContainerComponent>(Allocator.TempJob).ToArray();
        }
        
        /// <summary>
        /// Gets the current value of a container
        /// </summary>
        /// <param name="resourceID">The resource ID to look for</param>
        /// <returns>The current value, or 0 if not found</returns>
        public float GetContainerCurrentValue(int resourceID)
        {
            EntityQuery containerQuery = entityManager.CreateEntityQuery(typeof(ResourceContainerComponent));
            using var containers = containerQuery.ToComponentDataArray<ResourceContainerComponent>(Allocator.Temp);
            
            foreach (var container in containers)
            {
                if (container.ResourceID == resourceID)
                {
                    return container.CurrentValue;
                }
            }
            
            return 0f; // Not found
        }
        
        /// <summary>
        /// Gets the min and max constraint values for a container
        /// </summary>
        /// <param name="resourceID">The resource ID to look for</param>
        /// <returns>A Vector2 where x is min and y is max, or (float.MinValue, float.MaxValue) if no constraints</returns>
        public Vector2 GetContainerConstraints(int resourceID)
        {
            EntityQuery containerQuery = entityManager.CreateEntityQuery(typeof(ResourceContainerComponent));
            using var containers = containerQuery.ToComponentDataArray<ResourceContainerComponent>(Allocator.Temp);
            
            foreach (var container in containers)
            {
                if (container.ResourceID == resourceID)
                {
                    float minValue = container.GetMinValue(entityManager);
                    float maxValue = container.GetMaxValue(entityManager);
                    return new Vector2(minValue, maxValue);
                }
            }
            
            return new Vector2(float.MinValue, float.MaxValue); // No constraints
        }
        
        /// <summary>
        /// Logs detailed information about a container
        /// </summary>
        private void LogContainerInfo(ResourceContainerComponent container)
        {
            string resourceName = resourceNames.ContainsKey(container.ResourceID)
                ? resourceNames[container.ResourceID]
                : $"Unknown ({container.ResourceID})";
            
            string minValueInfo = container.MinValueResourceID > 0 && resourceNames.ContainsKey(container.MinValueResourceID)
                ? resourceNames[container.MinValueResourceID]
                : "None";
            
            string maxValueInfo = container.MaxValueResourceID > 0 && resourceNames.ContainsKey(container.MaxValueResourceID)
                ? resourceNames[container.MaxValueResourceID]
                : "None";
            
            // Get the actual min/max values
            float minValue = container.GetMinValue(entityManager);
            float maxValue = container.GetMaxValue(entityManager);
            
            string minValueConstraint = container.MinValueResourceID > 0 ? $"{minValue}" : "None";
            string maxValueConstraint = container.MaxValueResourceID > 0 ? $"{maxValue}" : "None";
            
            Debug.Log($"Container for {resourceName} (ID: {container.ResourceID}):");
            Debug.Log($"  Current Value: {container.CurrentValue}");
            Debug.Log($"  Min Constraint: {minValueInfo} (ID: {container.MinValueResourceID}, Value: {minValueConstraint})");
            Debug.Log($"  Max Constraint: {maxValueInfo} (ID: {container.MaxValueResourceID}, Value: {maxValueConstraint})");
            
            // Check if within constraints
            bool isWithinConstraints = container.IsWithinConstraints(entityManager);
            if (isWithinConstraints)
            {
                Debug.Log($"  Status: Within constraints ({minValue} <= {container.CurrentValue} <= {maxValue})");
            }
            else
            {
                if (container.MinValueResourceID > 0 && container.CurrentValue < minValue)
                {
                    Debug.LogWarning($"  Status: Below minimum! ({container.CurrentValue} < {minValue})");
                }
                
                if (container.MaxValueResourceID > 0 && container.CurrentValue > maxValue)
                {
                    Debug.LogWarning($"  Status: Above maximum! ({container.CurrentValue} > {maxValue})");
                }
            }
        }
    }
}
