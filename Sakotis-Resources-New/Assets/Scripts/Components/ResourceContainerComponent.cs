using System;
using Unity.Entities;
using Unity.Collections;
using Resources.Core;

namespace Resources.Components
{
    [Serializable]
    public struct ResourceContainerComponent : IComponentData
    {
        public int ResourceID;
        public float CurrentValue;
        public int MinValueResourceID;
        public int MaxValueResourceID;

        public static ResourceContainerComponent Create(int resourceID, float initialValue)
        {
            return new ResourceContainerComponent
            {
                ResourceID = resourceID,
                CurrentValue = initialValue,
                MinValueResourceID = 0,
                MaxValueResourceID = 0
            };
        }

        public static ResourceContainerComponent Create(int resourceID, float initialValue,
            int minValueResourceID, int maxValueResourceID)
        {
            return new ResourceContainerComponent
            {
                ResourceID = resourceID,
                CurrentValue = initialValue,
                MinValueResourceID = minValueResourceID,
                MaxValueResourceID = maxValueResourceID
            };
        }

        // Utility methods for working with min/max values

        /// <summary>
        /// Gets the minimum value for this container from the specified EntityManager
        /// </summary>
        public float GetMinValue(EntityManager entityManager)
        {
            if (MinValueResourceID <= 0)
                return float.MinValue; // No minimum constraint

            return GetContainerValueByResourceID(entityManager, MinValueResourceID);
        }

        /// <summary>
        /// Gets the maximum value for this container from the specified EntityManager
        /// </summary>
        public float GetMaxValue(EntityManager entityManager)
        {
            if (MaxValueResourceID <= 0)
                return float.MaxValue; // No maximum constraint

            return GetContainerValueByResourceID(entityManager, MaxValueResourceID);
        }

        /// <summary>
        /// Checks if the current value is within the min/max constraints
        /// </summary>
        public bool IsWithinConstraints(EntityManager entityManager)
        {
            float minValue = GetMinValue(entityManager);
            float maxValue = GetMaxValue(entityManager);

            return CurrentValue >= minValue && CurrentValue <= maxValue;
        }

        /// <summary>
        /// Gets a container's current value by its resource ID
        /// </summary>
        private float GetContainerValueByResourceID(EntityManager entityManager, int resourceID)
        {
            if (resourceID <= 0)
                return 0f;

            // Query for containers with the specified resource ID
            EntityQuery containerQuery = entityManager.CreateEntityQuery(typeof(ResourceContainerComponent));
            using var containers = containerQuery.ToComponentDataArray<ResourceContainerComponent>(Allocator.Temp);

            // Find the container with the matching resource ID
            foreach (var container in containers)
            {
                if (container.ResourceID == resourceID)
                {
                    return container.CurrentValue;
                }
            }

            // If no container is found, return a default value
            return 0f;
        }
    }
}
