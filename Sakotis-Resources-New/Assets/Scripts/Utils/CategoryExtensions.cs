using Unity.Entities;
using Resources.Components;

namespace Resources.Utils
{
    public static class CategoryExtensions
{
    public static bool BelongsToCategory(this DynamicBuffer<CategoryReference> buffer, int categoryID)
    {
        foreach (var category in buffer)
        {
            if (category.CategoryID == categoryID)
                return true;
        }
        return false;
    }

    public static void AddCategoryUnique(this DynamicBuffer<CategoryReference> buffer, int categoryID)
    {
        if (!buffer.BelongsToCategory(categoryID))
        {
            buffer.Add(new CategoryReference(categoryID));
        }
    }

    public static bool RemoveCategory(this DynamicBuffer<CategoryReference> buffer, int categoryID)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i].CategoryID == categoryID)
            {
                buffer.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public static int GetCategoryCount(this DynamicBuffer<CategoryReference> buffer)
    {
        return buffer.Length;
    }

    public static void ClearCategories(this DynamicBuffer<CategoryReference> buffer)
    {
        buffer.Clear();
    }
    }
}
