    using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NobleTools
{
    public class CoPilotWindow : ComponentPilot
    {

        [SerializeField] GameObject         _sourceGameObject;
        [SerializeField] List<GameObject>   _destinationGameObjects = new List<GameObject>();

        private Vector2 _scrollPos = Vector2.zero;
        private Dictionary<Component, bool> _components = new Dictionary<Component, bool>();
        bool _shouldCopyEverything = true;

        SerializedObject _componentPilotSerializedObject;

        [MenuItem("Tools/Noble Tools/Component Pilot")]
        public static void ShowWindow()
        {
            GetWindow(typeof(CoPilotWindow), false, "Component Pilot");
        }

        private void OnEnable()
        {
            _componentPilotSerializedObject = new SerializedObject(this);
        }

        private void OnGUI()
        {
            // Update the Serialized object, to save changes if any
            _componentPilotSerializedObject.Update();

            /*
             * SOURCE GAME OBJECT
             * - THIS IS THE GAME OBJECT FROM WHICH ALL THE COMPONENTS ARE COPIED TO THE DESTINATION GAMEOBJECT(s)
             * - THIS FIELD CANNOT BE EMPTY
             */


            #region SourceGameObject
            GUIContent sourceGameObjectGUIContent = new(
                "Source GameObject",
                "Game Object from which you want to copy all the components to the Destination Game Object(s)");

            /*_sourceGameObject = EditorGUILayout.ObjectField(
                sourceGameObjectGUIContent,
                _sourceGameObject,
                typeof(GameObject),
                false)
                as GameObject;*/

            EditorGUILayout.PropertyField(_componentPilotSerializedObject.FindProperty(nameof(_sourceGameObject)), sourceGameObjectGUIContent);

            if (_sourceGameObject != null)
            {
                foreach (Component sourceComponent in _sourceGameObject.GetComponents(typeof(Component)))
                {
                    if (!_components.ContainsKey(sourceComponent))
                        _components.Add(sourceComponent, true);

                }
            }
            else
            {
                _components.Clear();
            }
            #endregion


            /*
             * COMPONENT SELECTION
             * - THIS PROVIDES A LIST OF COMPONENTS TO CHOOSE FROM
             * - THESE COMPONENTS ARE THE COMPONENTS ATTACHED TO THE SOURCE GAME OBJECT
             * - THIS IS TO PROVIDE THE USER WITH THE LIBERTY TO CHOOSE WHICH ELEMENTS ARE NEEDED TO BE COPIED TO THE DESTINATION GAME OBJECT(s)
             */

            #region ComponentSelectionRegion

            if (_sourceGameObject == null)
                GUI.enabled = false;

            GUIContent dropdownGUIContent = new(
            "Component(s)",
            "Select the components which you want to copy from the Source GameObject");

            //GUIStyle style = GUI.skin.button;

            if (EditorGUILayout.DropdownButton(dropdownGUIContent, FocusType.Keyboard, GUILayout.ExpandWidth(true)))
            {
                // Create a Generic menu inside the dropdown
                GenericMenu componentGenericMenu = new GenericMenu();

                componentGenericMenu.AddItem(new GUIContent($"Everything"), _shouldCopyEverything, () => ToggleComponent(null));
                // Map all the source objects components to the dropdown
                foreach (Component sourceComponent in _sourceGameObject.GetComponents(typeof(Component)))
                {
                    // Strip the component name
                    string componentName = sourceComponent.GetType().ToString().Split(".")[1];

                    if (!_components.ContainsKey(sourceComponent))
                        _components.Add(sourceComponent, true);

                    componentGenericMenu.AddItem(new GUIContent($"{componentName}"), _components[sourceComponent], () => ToggleComponent(sourceComponent));
                }
                componentGenericMenu.ShowAsContext();
            }

            GUI.enabled = true;

            #endregion


            /*
             * DESTINATION GAME OBJECT
             * - THIS IS THE GAME OBJECT(s) TO WHICH ALL THE COMPONENTS ARE COPIED FROM THE SORUCE GAMEOBJECT
             * - THERE SHOULD BE ATLEAST ONE DESTINATION GAME OBJECT INORDER FOR THE TOOL TO RUN (OFC)
             */

            #region DestinationGameObject

            GUIContent destinationGameObjectLabelGUIContent = new(
            "Destination GameObject(s)",
            "Game Object(s) to which you want to copy all the components from the Source Game Objects");


            // Start a ScrollView incase the count grows more than the window can accomodate
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(true));

            EditorGUILayout.PropertyField(_componentPilotSerializedObject.FindProperty(nameof(_destinationGameObjects)), destinationGameObjectLabelGUIContent);

            EditorGUILayout.EndScrollView();
            #endregion


            #region SubmitButton

            if (_sourceGameObject == null || _destinationGameObjects.Count == 0)
                GUI.enabled = false;

            if (GUILayout.Button("Copy Components"))
            {
                CopyComponents(_sourceGameObject, _destinationGameObjects, _components);
            }

            GUI.enabled = true;
            #endregion

            //EditorGUILayout.LabelField("", GUI.skin.horizontalScrollbar);

            #region ClearDestinationListButton

            if (_destinationGameObjects.Count == 0)
                GUI.enabled = false;

            if (GUILayout.Button(new GUIContent("Clear Destination List", "Clears all the contents of Destination GameObject List")))
            {
                _destinationGameObjects.Clear();
            }

            GUI.enabled = true;

            #endregion

            #region ClearAllButton

            if (_sourceGameObject == null && _destinationGameObjects.Count == 0)
                GUI.enabled = false;

            if (GUILayout.Button(new GUIContent("Clear All", "Clears all the contents")))
            {
                _sourceGameObject = null;
                _destinationGameObjects.Clear();
            }

            GUI.enabled = true;

            #endregion


            GUILayout.FlexibleSpace();
            _componentPilotSerializedObject.ApplyModifiedProperties();
        }

        private void ToggleComponent(Component component)
        {
            // In case of Everything is selected
            if (component == null)
            {
                _shouldCopyEverything = true;
                foreach (Component comp in _components.Keys.ToList())
                    _components[comp] = true;
            }
            else
            {
                _components[component] = !_components[component];


                int count = 0;
                foreach (Component comp in _components.Keys.ToList())
                    if (_components[comp])
                        ++count;

                    _shouldCopyEverything = count == _components.Count;
            }
        }
    }
}