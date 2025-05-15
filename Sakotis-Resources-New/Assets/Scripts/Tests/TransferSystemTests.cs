using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using Resources.Core;
using Resources.Components;
using Resources.Systems;

namespace Resources.Tests
{
    public class TransferSystemTests : MonoBehaviour
    {
        [Tooltip("Resource definition to test")]
        public ResourceDefinition resourceDefinition;

        [Tooltip("Amount to transfer")]
        public float transferAmount = 50.0f;

        [Tooltip("Whether to log debug information")]
        public bool logDebugInfo = true;

        private EntityManager entityManager;
        private World defaultWorld;
        private Entity sourceContainer;
        private Entity destinationContainer;
        private bool containersFound = false;
        private string statusMessage = "";

        private void Start()
        {
            defaultWorld = World.DefaultGameObjectInjectionWorld;
            entityManager = defaultWorld.EntityManager;

            if (resourceDefinition == null)
            {
                statusMessage = "No resource definition assigned to test.";
                Debug.LogError(statusMessage);
                return;
            }

            // Find containers for the specified resource
            FindContainers();
        }

        private void FindContainers()
        {
            if (logDebugInfo)
            {
                Debug.Log($"Searching for containers with resource ID: {resourceDefinition.UniqueID}");
            }

            // Query for resource containers with the specified resource ID
            EntityQuery containerQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ResourceContainerComponent>()
            );

            using var containers = containerQuery.ToComponentDataArray<ResourceContainerComponent>(Allocator.Temp);
            using var containerEntities = containerQuery.ToEntityArray(Allocator.Temp);

            // Find two containers for this resource
            int foundCount = 0;
            for (int i = 0; i < containers.Length; i++)
            {
                if (containers[i].ResourceID == resourceDefinition.UniqueID)
                {
                    if (foundCount == 0)
                    {
                        sourceContainer = containerEntities[i];
                        foundCount++;
                    }
                    else if (foundCount == 1)
                    {
                        destinationContainer = containerEntities[i];
                        foundCount++;
                        break;
                    }
                }
            }

            if (foundCount >= 2)
            {
                containersFound = true;
                statusMessage = $"Found {foundCount} containers for {resourceDefinition.ResourceName}";

                if (logDebugInfo)
                {
                    Debug.Log(statusMessage);
                    LogContainerValues();
                }
            }
            else
            {
                containersFound = false;
                statusMessage = $"Not enough containers found for {resourceDefinition.ResourceName}. Found: {foundCount}, need at least 2.";
                Debug.LogWarning(statusMessage);
            }
        }

        private void CreateTransferOrder()
        {
            if (!containersFound)
            {
                statusMessage = "Cannot create transfer: containers not found.";
                Debug.LogWarning(statusMessage);
                return;
            }

            // Check if source has enough resources
            var sourceContainer_data = entityManager.GetComponentData<ResourceContainerComponent>(sourceContainer);
            if (sourceContainer_data.CurrentValue < transferAmount)
            {
                statusMessage = $"Source container doesn't have enough resources. Has: {sourceContainer_data.CurrentValue}, needs: {transferAmount}";
                Debug.LogWarning(statusMessage);
                return;
            }

            if (logDebugInfo)
            {
                Debug.Log($"Creating transfer order for {transferAmount} units using OrderFactorySystem...");
            }

            // Get the OrderFactorySystem
            var orderFactoryHandle = defaultWorld.GetExistingSystem<OrderFactorySystem>();
            if (orderFactoryHandle == default)
            {
                statusMessage = "OrderFactorySystem not found.";
                Debug.LogError(statusMessage);
                return;
            }

            // Get the system state
            var systemState = defaultWorld.Unmanaged.ResolveSystemStateRef(orderFactoryHandle);

            // Create the transfer order using the factory system
            Entity transferOrder = defaultWorld.Unmanaged.GetUnsafeSystemRef<OrderFactorySystem>(orderFactoryHandle)
                .CreateTransferOrder(
                    ref systemState,
                    sourceContainer,
                    destinationContainer,
                    resourceDefinition.UniqueID,
                    transferAmount
                );

            // Update status message
            statusMessage = $"Created transfer order for {transferAmount} units using OrderFactorySystem";

            if (logDebugInfo)
            {
                Debug.Log(statusMessage);
                LogContainerValues();
            }
        }

        private void LogContainerValues()
        {
            if (!logDebugInfo) return;

            var sourceValue = entityManager.GetComponentData<ResourceContainerComponent>(sourceContainer).CurrentValue;
            var destValue = entityManager.GetComponentData<ResourceContainerComponent>(destinationContainer).CurrentValue;

            Debug.Log($"Container values for {resourceDefinition.ResourceName}:");
            Debug.Log($"  Source: {sourceValue}");
            Debug.Log($"  Destination: {destValue}");
        }

        private void OnGUI()
        {
            // Simple UI to show current state and allow manual transfers
            GUILayout.BeginArea(new Rect(10, 10, 300, 250));

            GUILayout.Label($"Resource: {resourceDefinition?.ResourceName ?? "None"}");

            // Display status message
            if (!string.IsNullOrEmpty(statusMessage))
            {
                GUILayout.Label(statusMessage, GUI.skin.box);
            }

            // Display current container values if found
            if (containersFound)
            {
                var sourceValue = entityManager.GetComponentData<ResourceContainerComponent>(sourceContainer).CurrentValue;
                var destValue = entityManager.GetComponentData<ResourceContainerComponent>(destinationContainer).CurrentValue;

                GUILayout.Label($"Source: {sourceValue:F1}");
                GUILayout.Label($"Destination: {destValue:F1}");

                // Transfer amount field
                GUILayout.BeginHorizontal();
                GUILayout.Label("Amount:", GUILayout.Width(60));
                string amountStr = GUILayout.TextField(transferAmount.ToString(), GUILayout.Width(60));
                if (float.TryParse(amountStr, out float newAmount))
                {
                    transferAmount = newAmount;
                }
                GUILayout.EndHorizontal();

                // Button to create a transfer
                if (GUILayout.Button("Create Transfer Order"))
                {
                    CreateTransferOrder();
                }
            }
            else
            {
                GUILayout.Label("No containers found for this resource.", GUI.skin.box);

                // Button to search again for containers
                if (GUILayout.Button("Search for Containers"))
                {
                    FindContainers();
                }
            }

            GUILayout.EndArea();
        }
    }
}
