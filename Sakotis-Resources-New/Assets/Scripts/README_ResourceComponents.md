# Resource Components Implementation

This is the implementation of step 1.2 Resource Components from the Implementation Plan for Resources Feature.

## Components

1. **ResourceDefinitionComponent.cs** - ECS component that represents a resource definition
   - Contains the same properties as the ScriptableObject (name, description, uniqueID)
   - Includes a reference to an icon entity for visual representation
   - Includes helper methods for easy creation

2. **ResourceDefinitionAuthoring.cs** - MonoBehaviour for converting resource definitions to ECS
   - Includes Baker class for conversion
   - Handles conversion of the icon sprite to an ECS entity

## Test Components (in Tests folder)

1. **ResourceComponentTests.cs** - Test script for resource components
   - Creates test entities with ResourceDefinitionComponent
   - Verifies that icon entities are properly created and referenced
   - Logs component information to the console

2. **ResourceTestSetup.cs** - Updated test setup for resource system
   - Now handles icon entity creation and reference

## How to Test

1. Create a new Resource Definition asset:
   - Right-click in the Project window
   - Select Create > Resources > Resource Definition
   - Fill in the name, description, and assign a sprite for the icon

2. Set up a test scene:
   - Create an empty GameObject
   - Add the ResourceDefinitionAuthoring component
   - Assign your Resource Definition asset to it
   - OR
   - Add the ResourceTestSetup component
   - Assign your Resource Definition assets to it
   - Configure the test options

3. Enter Play mode to see the resource information logged to the console
   - The logs will show if icon entities were created successfully

## Design Considerations

- The implementation is designed to be used by non-programmers
- All fields have tooltips to explain their purpose
- The system automatically handles icon conversion
- Error messages are clear and helpful
- The code follows ECS best practices

## Next Steps

- Implement Resource Category Definition (step 1.3)
- Implement Transfer System (step 2)
