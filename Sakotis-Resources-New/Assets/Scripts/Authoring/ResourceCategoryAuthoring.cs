using Unity.Entities;
using UnityEngine;
using Resources.Core;
using Resources.Components;

namespace Resources.Authoring
{
    public class ResourceCategoryAuthoring : MonoBehaviour
{
    [Tooltip("The resource category to convert to ECS")]
    public ResourceCategory resourceCategory;

    public class ResourceCategoryBaker : Baker<ResourceCategoryAuthoring>
    {
        public override void Bake(ResourceCategoryAuthoring authoring)
        {
            if (authoring.resourceCategory == null)
            {
                Debug.LogWarning($"Resource Category is null on {authoring.gameObject.name}. No entity will be created.");
                return;
            }

            var entity = GetEntity(TransformUsageFlags.None);

            var categoryComponent = ResourceCategoryComponent.Create(
                authoring.resourceCategory.CategoryName,
                authoring.resourceCategory.UniqueID
            );

            AddComponent(entity, categoryComponent);
        }
    }
    }
}
