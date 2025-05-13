using System;
using System.Collections.Generic;
using UnityEngine;

namespace Resources.Core
{
    [CreateAssetMenu(fileName = "NewResource", menuName = "Resources/Resource Definition")]
    public class ResourceDefinition : ScriptableObject
{
    [Tooltip("The name of the resource")]
    [SerializeField] private string resourceName;

    [Tooltip("A description of the resource")]
    [TextArea(3, 5)]
    [SerializeField] private string description;

    [Tooltip("The icon representing this resource")]
    [SerializeField] private Sprite icon;

    [Tooltip("Unique identifier for this resource (auto-generated if zero)")]
    [SerializeField] private int uniqueID;

    [Tooltip("Categories this resource belongs to")]
    [SerializeField] private List<ResourceCategory> categories = new List<ResourceCategory>();

    public string ResourceName => resourceName;
    public string Description => description;
    public Sprite Icon => icon;
    public int UniqueID => uniqueID;
    public IReadOnlyList<ResourceCategory> Categories => categories;

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
