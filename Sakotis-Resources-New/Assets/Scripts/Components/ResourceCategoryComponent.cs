using System;
using Unity.Entities;
using Unity.Collections;
using Resources.Core;

namespace Resources.Components
{
    [Serializable]
    public struct ResourceCategoryComponent : IComponentData
{
    public FixedString64Bytes CategoryName;
    public int UniqueID;

    public static ResourceCategoryComponent Create(string name, int uniqueID)
    {
        return new ResourceCategoryComponent
        {
            CategoryName = new FixedString64Bytes(name),
            UniqueID = uniqueID
        };
    }
    }
}
