# Resource Category Implementation with Dynamic Buffers

This is the updated implementation of step 1.3 Resource Category Definition from the Implementation Plan for Resources Feature, using ECS Dynamic Buffers for a more flexible and expandable approach.

## Components

1. **ResourceCategory.cs** - ScriptableObject that defines a resource category
   - Contains name and uniqueID (now using integer instead of string)
   - Automatically generates a unique ID if one is not provided
   - Designer-friendly with tooltips

2. **CategoryReference.cs** - Buffer element that represents a reference to a resource category
   - Simple struct implementing IBufferElementData
   - Contains only the CategoryID (integer)
   - Used in dynamic buffers to store category references
   - No buffer capacity constraints for maximum flexibility

3. **CategoryExtensions.cs** - Extension methods for working with CategoryReference buffers
   - Provides helper methods like BelongsToCategory, AddCategoryUnique, RemoveCategory
   - Makes it easy to work with category buffers in systems and tests

4. **ResourceCategoryComponent.cs** - ECS component that represents a resource category
   - Contains the same properties as the ScriptableObject but in ECS-compatible format
   - Uses integer for UniqueID

5. **ResourceCategoryAuthoring.cs** - MonoBehaviour for converting resource categories to ECS
   - Includes Baker class for conversion
   - Provides clear error messages for designers

6. **ResourceCategorySystem.cs** - System that processes resource categories
   - Updated to use DynamicBuffer<CategoryReference> instead of ResourceCategoriesComponent
   - Demonstrates how to query and use resource categories in ECS

## Updates to Existing Components

1. **ResourceDefinition.cs** - Updated to include a list of categories and use integer for UniqueID
   - Allows designers to assign categories to resources

2. **ResourceDefinitionAuthoring.cs** - Updated to use DynamicBuffer<CategoryReference>
   - Adds CategoryReference buffer to resource entities
   - No longer limited to a fixed number of categories

## Benefits of the Dynamic Buffer Approach

1. **No Size Limitations**: Resources can belong to any number of categories with no artificial constraints
2. **Performance**: Optimized for iteration and cache-friendly access
3. **Flexibility**: Categories can be added or removed at runtime
4. **ECS Best Practices**: Uses DynamicBuffer which is designed for this purpose
5. **Future-Proof**: Follows ECS best practices for collections
6. **Expandable**: Easy to add more functionality through extension methods
7. **Fully Generic**: Implementation places no restrictions on how many categories a resource can have

## How to Test

1. Create new Resource Category assets:
   - Right-click in the Project window
   - Select Create > Resources > Resource Category
   - Fill in the name (a unique ID will be generated automatically)

2. Assign categories to resources:
   - Open a Resource Definition asset
   - Add categories to the Categories list

3. Set up a test scene:
   - Create an empty GameObject
   - Add the ResourceCategoryTests component
   - Assign your Resource Definition and Resource Category assets to it
   - Configure the test options

4. Enter Play mode to see the category information logged to the console
   - The logs will show which resources belong to which categories

## Usage Examples

### Adding a category to a resource at runtime:

```csharp
// Get the category buffer
var categoryBuffer = entityManager.GetBuffer<CategoryReference>(resourceEntity);

// Add a category
categoryBuffer.Add(new CategoryReference(categoryID));

// Or use the extension method to add only if it doesn't exist
categoryBuffer.AddCategoryUnique(categoryID);
```

### Checking if a resource belongs to a category:

```csharp
// Get the category buffer
var categoryBuffer = entityManager.GetBuffer<CategoryReference>(resourceEntity);

// Check if it belongs to a specific category
bool belongsToCategory = categoryBuffer.BelongsToCategory(categoryID);
```

### Removing a category:

```csharp
// Get the category buffer
var categoryBuffer = entityManager.GetBuffer<CategoryReference>(resourceEntity);

// Remove a category
bool removed = categoryBuffer.RemoveCategory(categoryID);
```

### Querying resources by category:

```csharp
// Query all resources with categories
EntityQuery resourceQuery = SystemAPI.QueryBuilder()
    .WithAll<ResourceDefinitionComponent>()
    .WithAll<CategoryReference>()
    .Build();

// Process only resources in a specific category
foreach (var (resourceDef, categoryBuffer, entity) in
         SystemAPI.Query<ResourceDefinitionComponent, DynamicBuffer<CategoryReference>>()
                  .WithEntityAccess())
{
    if (categoryBuffer.BelongsToCategory(targetCategoryID))
    {
        // Process this resource
    }
}
```

## Next Steps

- Implement Transfer System (step 2)
- Implement Modifier System (step 3)
- Implement Production & Conversion System (step 4)
