using Unity.Entities;

namespace Resources.Components
{
    public struct CategoryReference : IBufferElementData
{
    public int CategoryID;

    public CategoryReference(int categoryID)
    {
        CategoryID = categoryID;
    }
    }
}
