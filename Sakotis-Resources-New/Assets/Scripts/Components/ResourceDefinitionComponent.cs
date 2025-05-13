using System;
using Unity.Entities;
using Unity.Collections;
using Resources.Core;

namespace Resources.Components
{
    [Serializable]
    public struct ResourceDefinitionComponent : IComponentData
{
    public FixedString64Bytes ResourceName;
    public FixedString128Bytes Description;
    public int UniqueID;
    public Entity IconEntity;

    public static ResourceDefinitionComponent Create(string name, string description, int uniqueID)
    {
        return new ResourceDefinitionComponent
        {
            ResourceName = new FixedString64Bytes(name),
            Description = new FixedString128Bytes(description),
            UniqueID = uniqueID,
            IconEntity = Entity.Null
        };
    }
    }
}
