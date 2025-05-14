using Unity.Entities;
using UnityEngine;
using Resources.Core;
using Resources.Components;

namespace Resources.Authoring
{
    public class ResourceContainerAuthoring : MonoBehaviour
    {
        [Tooltip("The resource definition for this container")]
        public ResourceDefinition resourceDefinition;

        [Tooltip("Initial value of the resource")]
        public float initialValue = 0;

        [Tooltip("Resource that defines the minimum value")]
        public ResourceDefinition minValueResource;

        [Tooltip("Resource that defines the maximum value")]
        public ResourceDefinition maxValueResource;

        public class ResourceContainerBaker : Baker<ResourceContainerAuthoring>
        {
            public override void Bake(ResourceContainerAuthoring authoring)
            {
                if (authoring.resourceDefinition == null)
                {
                    Debug.LogWarning($"Resource Definition is null on {authoring.gameObject.name}. No container entity will be created.");
                    return;
                }

                var entity = GetEntity(TransformUsageFlags.None);

                int minValueResourceID = 0;
                int maxValueResourceID = 0;

                // Get resource IDs for min/max values if provided
                if (authoring.minValueResource != null)
                {
                    minValueResourceID = authoring.minValueResource.UniqueID;
                }

                if (authoring.maxValueResource != null)
                {
                    maxValueResourceID = authoring.maxValueResource.UniqueID;
                }

                // Create and add the container component
                var containerComponent = ResourceContainerComponent.Create(
                    authoring.resourceDefinition.UniqueID,
                    authoring.initialValue,
                    minValueResourceID,
                    maxValueResourceID
                );

                AddComponent(entity, containerComponent);
            }
        }
    }
}
