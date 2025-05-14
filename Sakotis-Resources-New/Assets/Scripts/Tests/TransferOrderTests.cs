using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using Resources.Core;
using Resources.Components;

namespace Resources.Tests
{
    public class TransferOrderTests : MonoBehaviour
    {
        [Tooltip("Resource definitions to test")]
        public ResourceDefinition[] resourceDefinitions;

        [Tooltip("Whether to log debug information")]
        public bool logDebugInfo = true;

        private EntityManager entityManager;
        private World defaultWorld;

        private void Start()
        {
            defaultWorld = World.DefaultGameObjectInjectionWorld;
            entityManager = defaultWorld.EntityManager;

            if (resourceDefinitions == null || resourceDefinitions.Length == 0)
            {
                Debug.LogWarning("No resource definitions assigned to test.");
                return;
            }

            CreateResourceEntities();
            CreateResourceContainers();
            CreateTransferOrders();
            QueryTransferOrders();
        }

        private void CreateResourceEntities()
        {
            if (logDebugInfo)
            {
                Debug.Log("Creating resource definition entities...");
            }

            foreach (var resourceDef in resourceDefinitions)
            {
                if (resourceDef == null) continue;

                // Check if we already created this resource definition
                EntityQuery query = entityManager.CreateEntityQuery(
                    ComponentType.ReadOnly<ResourceDefinitionComponent>()
                );

                bool alreadyExists = false;
                using var existingResources = query.ToComponentDataArray<ResourceDefinitionComponent>(Allocator.Temp);
                foreach (var res in existingResources)
                {
                    if (res.UniqueID == resourceDef.UniqueID)
                    {
                        alreadyExists = true;
                        break;
                    }
                }

                if (alreadyExists)
                {
                    continue; // Skip if already created
                }

                // Create the resource definition entity
                Entity resourceEntity = entityManager.CreateEntity();
                entityManager.AddComponentData(resourceEntity, ResourceDefinitionComponent.Create(
                    resourceDef.ResourceName,
                    resourceDef.Description,
                    resourceDef.UniqueID
                ));

                if (logDebugInfo)
                {
                    Debug.Log($"Created resource definition for {resourceDef.ResourceName} with ID: {resourceDef.UniqueID}");
                }
            }
        }

        private void CreateResourceContainers()
        {
            if (logDebugInfo)
            {
                Debug.Log("Creating resource containers...");
            }

            foreach (var resourceDef in resourceDefinitions)
            {
                if (resourceDef == null) continue;

                // Create source container
                Entity sourceEntity = entityManager.CreateEntity();
                entityManager.AddComponentData(sourceEntity, ResourceContainerComponent.Create(
                    resourceDef.UniqueID,
                    100.0f // Initial value
                ));

                // Create destination container
                Entity destinationEntity = entityManager.CreateEntity();
                entityManager.AddComponentData(destinationEntity, ResourceContainerComponent.Create(
                    resourceDef.UniqueID,
                    0.0f // Initial value
                ));

                if (logDebugInfo)
                {
                    Debug.Log($"Created source and destination containers for {resourceDef.ResourceName}");
                }
            }
        }

        private void CreateTransferOrders()
        {
            if (logDebugInfo)
            {
                Debug.Log("Creating transfer orders...");
            }

            // Query for resource containers
            EntityQuery containerQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ResourceContainerComponent>()
            );
            
            using var containers = containerQuery.ToComponentDataArray<ResourceContainerComponent>(Allocator.Temp);
            using var containerEntities = containerQuery.ToEntityArray(Allocator.Temp);

            // Group containers by resource ID
            for (int i = 0; i < resourceDefinitions.Length; i++)
            {
                var resourceDef = resourceDefinitions[i];
                if (resourceDef == null) continue;

                Entity sourceEntity = Entity.Null;
                Entity destinationEntity = Entity.Null;

                // Find source and destination containers for this resource
                for (int j = 0; j < containers.Length; j++)
                {
                    if (containers[j].ResourceID == resourceDef.UniqueID)
                    {
                        if (containers[j].CurrentValue > 0 && sourceEntity == Entity.Null)
                        {
                            sourceEntity = containerEntities[j];
                        }
                        else if (containers[j].CurrentValue == 0 && destinationEntity == Entity.Null)
                        {
                            destinationEntity = containerEntities[j];
                        }

                        if (sourceEntity != Entity.Null && destinationEntity != Entity.Null)
                        {
                            break;
                        }
                    }
                }

                if (sourceEntity != Entity.Null && destinationEntity != Entity.Null)
                {
                    // Create transfer order
                    Entity transferEntity = entityManager.CreateEntity();
                    entityManager.AddComponentData(transferEntity, TransferOrderComponent.Create(
                        sourceEntity,
                        destinationEntity,
                        resourceDef.UniqueID,
                        50.0f // Transfer amount
                    ));

                    if (logDebugInfo)
                    {
                        Debug.Log($"Created transfer order for {resourceDef.ResourceName}: 50.0 units");
                    }
                }
                else
                {
                    if (logDebugInfo)
                    {
                        Debug.LogWarning($"Could not create transfer order for {resourceDef.ResourceName}: source or destination not found");
                    }
                }
            }
        }

        private void QueryTransferOrders()
        {
            if (logDebugInfo)
            {
                Debug.Log("Querying transfer orders...");
            }

            EntityQuery transferQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<TransferOrderComponent>()
            );
            
            using var transfers = transferQuery.ToComponentDataArray<TransferOrderComponent>(Allocator.Temp);
            
            if (logDebugInfo)
            {
                Debug.Log($"Found {transfers.Length} transfer orders");
                
                foreach (var transfer in transfers)
                {
                    // Get resource name
                    string resourceName = "Unknown";
                    EntityQuery resourceQuery = entityManager.CreateEntityQuery(
                        ComponentType.ReadOnly<ResourceDefinitionComponent>()
                    );
                    
                    using var resources = resourceQuery.ToComponentDataArray<ResourceDefinitionComponent>(Allocator.Temp);
                    foreach (var resource in resources)
                    {
                        if (resource.UniqueID == transfer.ResourceID)
                        {
                            resourceName = resource.ResourceName.ToString();
                            break;
                        }
                    }
                    
                    Debug.Log($"Transfer order: {transfer.Amount} units of {resourceName} (ID: {transfer.ResourceID})");
                }
            }
        }
    }
}
