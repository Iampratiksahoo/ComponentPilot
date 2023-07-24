using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NobleTools
{
    public class CoPilotMenu : ComponentPilot
    {
        private static GameObject _sourceGameObject;

        [MenuItem("CONTEXT/Component/Copy All Components")]
        public static void GetSourceGameObject(MenuCommand menuCommand)
        {
            _sourceGameObject = (menuCommand.context as Component).gameObject;
        }

        [MenuItem("CONTEXT/Component/Paste All Components")]
        public static void CopyToDestination(MenuCommand menuCommand)
        {
            GameObject destinationGameObject = (menuCommand.context as Component).gameObject;

            Dictionary<Component, bool> ComponentsIncludeData = new Dictionary<Component, bool>();
            foreach (Component sourceComponent in _sourceGameObject.GetComponents(typeof(Component)))
            {

                if (ComponentsIncludeData.ContainsKey(sourceComponent))
                    ComponentsIncludeData[sourceComponent] = true;
                else
                    ComponentsIncludeData.Add(sourceComponent, true);
            }

            CopyComponents(_sourceGameObject, new List<GameObject>() { destinationGameObject }, ComponentsIncludeData);

        }

        [MenuItem("CONTEXT/Component/Paste All Components", true)]
        static bool ValidateSourceGameObject()
        {
            if(_sourceGameObject == null) return false;
            return true;
        }
    }
}