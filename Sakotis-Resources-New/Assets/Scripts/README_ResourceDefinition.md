# Resource Definition Implementation

This is the implementation of step 1.1 Resource Definition from the Implementation Plan for Resources Feature.

## Components

1. **ResourceDefinition.cs** - ScriptableObject that defines resource properties
   - Contains name, description, sprite, and uniqueID
   - Automatically generates a unique ID if one is not provided
   - Designer-friendly with tooltips and text areas

2. **ResourceDefinitionComponent.cs** - ECS component that represents a resource definition
   - Contains the same properties as the ScriptableObject but in ECS-compatible format
   - Includes helper methods for easy creation

3. **ResourceDefinitionAuthoring.cs** - MonoBehaviour for converting resource definitions to ECS
   - Includes Baker class for conversion
   - Provides clear error messages for designers

4. **ResourceDefinitionSystem.cs** - System that processes resource definitions
   - Implements ISystem for better performance
   - Demonstrates how to query and use resource definitions in ECS

## Test Components (in Tests folder)

1. **ResourceDefinitionTests.cs** - Simple test script for resource definitions
   - Logs resource information to the console

2. **ResourceTestSetup.cs** - Test setup for resource system
   - Demonstrates how to create resource entities at runtime
   - Provides options for debugging

## How to Test

1. Create a new Resource Definition asset:
   - Right-click in the Project window
   - Select Create > Resources > Resource Definition
   - Fill in the name, description, and optionally assign a sprite

2. Set up a test scene:
   - Create an empty GameObject
   - Add the ResourceDefinitionAuthoring component
   - Assign your Resource Definition asset to it
   - OR
   - Add the ResourceTestSetup component
   - Assign your Resource Definition assets to it
   - Configure the test options

3. Enter Play mode to see the resource information logged to the console

## Design Considerations

- The implementation is designed to be used by non-programmers
- All fields have tooltips to explain their purpose
- The system automatically generates unique IDs
- Error messages are clear and helpful
- The code follows ECS best practices

## Next Steps

- Implement Resource Container (step 1.2)
- Implement Resource Categories (step 1.3)
- Implement Transfer System (step 2)
