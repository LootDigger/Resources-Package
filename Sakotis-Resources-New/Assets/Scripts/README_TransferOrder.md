# Transfer Order Implementation

This is the implementation of step 2.1 Transfer Order Components from the Implementation Plan for Resources Feature.

## Components

1. **TransferOrderComponent.cs** - ECS component that represents a transfer order
   - Contains source entity, destination entity, resource ID, and amount to transfer
   - Designed to be created programmatically rather than through authoring components
   - Includes helper methods for easy creation

## Test Components (in Tests folder)

1. **TransferOrderTests.cs** - Test script for creating and querying transfer orders
   - Creates test entities with TransferOrderComponent
   - Creates resource definition entities and container entities
   - Demonstrates how to create transfer orders at runtime
   - Shows how to query transfer orders and their associated resources

## How to Test

1. Create a new empty GameObject in your scene
2. Add the TransferOrderTests component to it
3. Assign some Resource Definition assets to the component
4. Enter Play mode to see the transfer orders created and logged to the console

## Usage Examples

### Creating a transfer order at runtime:

```csharp
// Create a transfer order
Entity transferEntity = entityManager.CreateEntity();
entityManager.AddComponentData(transferEntity, TransferOrderComponent.Create(
    sourceEntity,      // Entity containing a ResourceContainerComponent
    destinationEntity, // Entity containing a ResourceContainerComponent
    resourceID,        // The ID of the resource to transfer
    50.0f              // Amount to transfer
));
```

### Querying transfer orders:

```csharp
// Query all transfer orders
EntityQuery transferQuery = entityManager.CreateEntityQuery(
    ComponentType.ReadOnly<TransferOrderComponent>()
);

using var transfers = transferQuery.ToComponentDataArray<TransferOrderComponent>(Allocator.Temp);

foreach (var transfer in transfers)
{
    // Process each transfer order
    Debug.Log($"Transfer order: {transfer.Amount} units of resource {transfer.ResourceID}");
}
```

## Next Steps

- Implement Transfer System (step 2.2)
- Implement Modifier System (step 3)
- Implement Production & Conversion System (step 4)
