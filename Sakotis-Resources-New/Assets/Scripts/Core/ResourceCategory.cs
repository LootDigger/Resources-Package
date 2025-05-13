using System;
using UnityEngine;

namespace Resources.Core
{
    [CreateAssetMenu(fileName = "NewResourceCategory", menuName = "Resources/Resource Category")]
    public class ResourceCategory : ScriptableObject
{
    [Tooltip("The name of the category")]
    [SerializeField] private string categoryName;

    [Tooltip("Unique identifier for this category (auto-generated if zero)")]
    [SerializeField] private int uniqueID;

    public string CategoryName => categoryName;
    public int UniqueID => uniqueID;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (uniqueID == 0)
        {
            System.Random random = new System.Random();
            uniqueID = random.Next(1, int.MaxValue);
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
    }
}
