using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Presets;

namespace NobleTools
{
    [CanEditMultipleObjects]
    public class ComponentPilot : EditorWindow
    {
        protected static void CopyComponents(GameObject SourceGameObject, List<GameObject> DestinationGameObjects, Dictionary<Component, bool> ComponentsIncludeData)
        {
            if (DestinationGameObjects.Count == 0 || SourceGameObject == null)
            {
                return;
            }
            foreach (GameObject destinationObject in DestinationGameObjects)
            {
                // Ignore cases
                if (destinationObject == null)
                {
                    Debug.LogWarning($"Destination cannot be null!! Skipping");
                    continue;
                }
                if (destinationObject == SourceGameObject)
                {
                    Debug.LogWarning($"Destination: {destinationObject.name} is same as Source: {SourceGameObject.name}!! Skipping");
                    continue;
                }

                foreach (Component sourceComponent in SourceGameObject.GetComponents(typeof(Component)))
                {
                    // Predefine the to be created destinationComponent
                    Component destinationComponent;

                    // Get the typeof Current sourceComponent
                    Type sourceComponentType = sourceComponent.GetType();

                    // Continue incase of transform or the user doesn't want the component to be copied
                    if (!ComponentsIncludeData[sourceComponent])
                        continue;

                    /*
                     * Check if there is a valid component of type sourcecomponent is already present
                     * - If yes, then take the ref of the component to copy the properties
                     * - If no, then add a component and then copy the properties
                     */
                    if (!destinationObject.GetComponent(sourceComponentType))
                        destinationComponent = destinationObject.AddComponent(sourceComponentType);
                    else
                        destinationComponent = destinationObject.GetComponent(sourceComponentType);


                    /*
                     * Check if the Component is Transform
                     * - if Yes, then only preserve the position of the destination object, and Copy the Rotation and Scale (As Implemented in v1.0)
                     */
                    Vector3 cachePosition = Vector3.zero;
                    if(destinationComponent.GetType() == typeof(Transform))
                    {
                        cachePosition = (destinationComponent as Transform).position;
                    }

                    // Create the preset with all the fields intact for the sourceComponent
                    Preset sourcePreset = new Preset(sourceComponent);

                    // Apply the same preset to the destinationComponent
                    sourcePreset.ApplyTo(destinationComponent);

                    if (destinationComponent.GetType() == typeof(Transform))
                        destinationComponent.transform.position = cachePosition;

                }
            }
        }
    }
}
