# Resource Category Implementation

This is the implementation of step 1.3 Resource Category Definition from the Implementation Plan for Resources Feature.

## Components

1. **ResourceCategory.cs** - ScriptableObject that defines a resource category
   - Contains name and uniqueID
   - Automatically generates a unique ID if one is not provided
   - Designer-friendly with tooltips

2. **CategoryReference.cs** - Buffer element that represents a reference to a resource category
   - Simple struct implementing IBufferElementData
   - Contains only the CategoryID (integer)
   - Used in dynamic buffers to store category references

3. **CategoryExtensions.cs** - Extension methods for working with CategoryReference buffers
   - Provides helper methods like BelongsToCategory, AddCategoryUnique, RemoveCategory
   - Makes it easy to work with category buffers in systems and tests

4. **ResourceCategoryComponent.cs** - ECS component that represents a resource category
   - Contains the same properties as the ScriptableObject but in ECS-compatible format
   - Includes helper methods for easy creation

5. **ResourceCategoryAuthoring.cs** - MonoBehaviour for converting resource categories to ECS
   - Includes Baker class for conversion
   - Provides clear error messages for designers

6. **ResourceCategorySystem.cs** - System that processes resource categories
   - Implements ISystem for better performance
   - Demonstrates how to query and use resource categories in ECS

## Updates to Existing Components

1. **ResourceDefinition.cs** - Updated to include a list of categories
   - Allows designers to assign categories to resources

2. **ResourceDefinitionAuthoring.cs** - Updated to handle categories
   - Converts resource categories to ECS components
   - Adds CategoryReference buffer to resource entities

## Test Components (in Tests folder)

1. **ResourceCategoryTests.cs** - Test script for resource categories
   - Creates test entities with ResourceCategoryComponent
   - Creates resource entities with CategoryReference buffers
   - Verifies that categories are properly created and associated with resources
   - Tests category membership using extension methods

2. **ResourceTestSetup.cs** - Updated test setup for resource system
   - Now handles category creation and association using dynamic buffers

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

## Design Considerations

- The implementation is designed to be used by non-programmers
- All fields have tooltips to explain their purpose
- The system automatically generates unique IDs
- Resources can belong to multiple categories (up to 4)
- The code follows ECS best practices

## Next Steps

- Implement Transfer System (step 2)
- Implement Modifier System (step 3)
- Implement Production & Conversion System (step 4)
