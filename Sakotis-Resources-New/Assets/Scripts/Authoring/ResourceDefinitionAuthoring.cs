using Unity.Entities;
using UnityEngine;
using Resources.Core;
using Resources.Components;

namespace Resources.Authoring
{
    public class ResourceDefinitionAuthoring : MonoBehaviour
{
    [Tooltip("The resource definition to convert to ECS")]
    public ResourceDefinition resourceDefinition;

    public class ResourceDefinitionBaker : Baker<ResourceDefinitionAuthoring>
    {
        public override void Bake(ResourceDefinitionAuthoring authoring)
        {
            if (authoring.resourceDefinition == null)
            {
                Debug.LogWarning($"Resource Definition is null on {authoring.gameObject.name}. No entity will be created.");
                return;
            }

            var entity = GetEntity(TransformUsageFlags.None);

            var resourceComponent = ResourceDefinitionComponent.Create(
                authoring.resourceDefinition.ResourceName,
                authoring.resourceDefinition.Description,
                authoring.resourceDefinition.UniqueID
            );

            if (authoring.resourceDefinition.Icon != null)
            {
                var iconEntity = CreateAdditionalEntity(TransformUsageFlags.None);
                resourceComponent.IconEntity = iconEntity;
            }

            AddComponent(entity, resourceComponent);

            if (authoring.resourceDefinition.Categories != null && authoring.resourceDefinition.Categories.Count > 0)
            {
                var categoryBuffer = AddBuffer<CategoryReference>(entity);

                foreach (var category in authoring.resourceDefinition.Categories)
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
}
