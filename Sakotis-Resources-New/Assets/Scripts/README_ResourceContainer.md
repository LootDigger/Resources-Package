# Resource Container Implementation

This is the implementation of step 1.4 Resource Container from the Implementation Plan for Resources Feature.

## Components

1. **ResourceContainerComponent.cs** - ECS component that represents a resource container
   - Contains current value, resource ID, and references to min/max value resource IDs
   - Min and max values are represented as resource IDs, not simple integers or entity references
   - Includes helper methods for easy creation

2. **ResourceContainerAuthoring.cs** - MonoBehaviour for converting containers to ECS
   - Includes fields for initial value and references to resource definitions
   - Allows designers to specify min/max value resources
   - Includes Baker class for conversion to ECS components

## Test Components (in Tests folder)

1. **ResourceContainerTests.cs** - Test script for creating resource containers
   - Creates test entities with ResourceContainerComponent
   - Creates resource definition entities for all resources (main, min, max)
   - Verifies that containers are properly created and linked to resources by ID

2. **ResourceContainerQueryTests.cs** - Test script for querying resource containers
   - Gets all containers in the world
   - Returns current values of containers by resource ID
   - Returns min/max constraint values of containers
   - Provides detailed logging of container constraints

3. **ResourceTestSetup.cs** - Updated test setup for resource system
   - Now handles container creation and min/max value resources by ID

## How to Test

1. Create Resource Definition assets for your resources and min/max values:
   - Right-click in the Project window
   - Select Create > Resources > Resource Definition
   - Fill in the name, description, and assign a sprite for the icon

2. Set up a test scene with both test components:
   - Create an empty GameObject named "ContainerCreator"
   - Add the ResourceContainerTests component
   - Assign your Resource Definition assets to it
   - Configure the test options (initial values, min/max resources)

   - Create another empty GameObject named "ContainerQuery"
   - Add the ResourceContainerQueryTests component
   - This will query and display information about all containers

3. Enter Play mode to see the container information logged to the console
   - The ResourceContainerTests logs will show the containers being created
   - The ResourceContainerQueryTests logs will show detailed information about all containers:
     - Current values
     - Min/max constraint values
     - Whether values are within constraints

## Design Considerations

- The implementation is designed to be used by non-programmers
- All fields have tooltips to explain their purpose
- Min and max values are represented as resource IDs, allowing for shared min/max values across containers
- The ResourceContainerComponent includes utility methods for working with min/max constraints:
  - `GetMinValue(EntityManager)`: Gets the minimum value for this container
  - `GetMaxValue(EntityManager)`: Gets the maximum value for this container
  - `IsWithinConstraints(EntityManager)`: Checks if the current value is within min/max constraints
- The code follows ECS best practices

## Usage Examples

### Creating a resource container at runtime:

```csharp
// Create the resource definition entities
Entity goldEntity = entityManager.CreateEntity();
entityManager.AddComponentData(goldEntity, ResourceDefinitionComponent.Create(
    "Gold",
    "Precious metal",
    12345
));

Entity minGoldEntity = entityManager.CreateEntity();
entityManager.AddComponentData(minGoldEntity, ResourceDefinitionComponent.Create(
    "Min Gold",
    "Minimum gold value",
    54321
));

Entity maxGoldEntity = entityManager.CreateEntity();
entityManager.AddComponentData(maxGoldEntity, ResourceDefinitionComponent.Create(
    "Max Gold",
    "Maximum gold value",
    98765
));

// Create the container entity
Entity containerEntity = entityManager.CreateEntity();
entityManager.AddComponentData(containerEntity, ResourceContainerComponent.Create(
    12345,  // Resource ID
    50.0f,  // Initial value
    54321,  // Min value resource ID
    98765   // Max value resource ID
));
```

### Querying resource containers:

```csharp
using System;

// Query all resource containers
EntityQuery containerQuery = entityManager.CreateEntityQuery(typeof(ResourceContainerComponent));
var containers = containerQuery.ToComponentDataArray<ResourceContainerComponent>(Allocator.Temp);

// Query containers for a specific resource
EntityQuery specificQuery = entityManager.CreateEntityQuery(
    ComponentType.ReadOnly<ResourceContainerComponent>()
);

// Filter for a specific resource ID
var specificContainers = new List<ResourceContainerComponent>();
using var allContainers = specificQuery.ToComponentDataArray<ResourceContainerComponent>(Allocator.Temp);
foreach (var container in allContainers)
{
    if (container.ResourceID == 12345)
    {
        specificContainers.Add(container);
    }
}

// Working with min/max constraints
foreach (var container in allContainers)
{
    // Get min/max values
    float minValue = container.GetMinValue(entityManager);
    float maxValue = container.GetMaxValue(entityManager);

    // Check if within constraints
    bool isWithinConstraints = container.IsWithinConstraints(entityManager);

    // Enforce constraints if needed
    if (!isWithinConstraints)
    {
        var updatedContainer = container;
        updatedContainer.CurrentValue = Math.Clamp(container.CurrentValue, minValue, maxValue);
        // Update the container in the EntityManager
    }
}
```

## Next Steps

- Implement Transfer System (step 2)
- Implement Modifier System (step 3)
- Implement Production & Conversion System (step 4)
