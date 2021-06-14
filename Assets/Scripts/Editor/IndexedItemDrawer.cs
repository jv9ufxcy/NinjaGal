using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(IndexedItemAttribute))]
public class IndexedItemDrawer : PropertyDrawer
{
    CoreData coreData;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //get the attricbut since it contains the range of the slider
        IndexedItemAttribute indexedItem = attribute as IndexedItemAttribute;
        if (coreData==null)
        {
            foreach (string guid in AssetDatabase.FindAssets("t: CoreData"))//looks at whole project for assets tagged CoreData
            {
                coreData = AssetDatabase.LoadAssetAtPath<CoreData>(AssetDatabase.GUIDToAssetPath(guid));
            }
        }

        switch (indexedItem.type)
        {
            case IndexedItemAttribute.IndexedItemType.SCRIPTS:
                property.intValue = EditorGUI.IntPopup(position, property.intValue, coreData.GetScriptNames(), null);
                break;
            case IndexedItemAttribute.IndexedItemType.STATES:
                property.intValue = EditorGUI.IntPopup(position, property.intValue, coreData.GetStateNames(), null);
                break;
            case IndexedItemAttribute.IndexedItemType.RAW_INPUTS:
                //property.intValue = EditorGUI.Popup(position, property.intValue, coreData.GetRawInputNames(), EditorStyles.miniButtonLeft);
                property.intValue = EditorGUI.IntPopup(position, property.intValue, coreData.GetRawInputNames(), null);
                break;
            case IndexedItemAttribute.IndexedItemType.CHAIN_COMMAND:
                //property.intValue = EditorGUI.IntPopup(position, property.intValue, coreData.GetChainCommandNames(), null);
                break;
            case IndexedItemAttribute.IndexedItemType.COMMAND_STATES:
                property.intValue = EditorGUI.IntPopup(position, property.intValue, coreData.GetCommandStateNames(), null);
                break;
        }
        //base.OnGUI(position, property, label);
    }
}

[CustomPropertyDrawer(typeof(InputCommand))]
public class InputCommandDrawer : PropertyDrawer
{
    public CoreData coreData;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        if (coreData == null)
        {
            foreach (string guid in AssetDatabase.FindAssets("t: CoreData"))
            {
                coreData = AssetDatabase.LoadAssetAtPath<CoreData>(AssetDatabase.GUIDToAssetPath(guid));
            }
        }

        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        position.x -= 100;
        // Calculate rects
        var inputRect = new Rect(position.x, position.y, 100, position.height);
        var stateRect = new Rect(position.x + 105, position.y, 100, position.height);
        var nextAdd = new Rect(position.x + 220, position.y, 20, position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(inputRect, property.FindPropertyRelative("input"), new GUIContent("Input", "tool tip me, father"));
        EditorGUI.PropertyField(stateRect, property.FindPropertyRelative("state"), GUIContent.none);
        //EditorGUI.PropertyField(nextAdd, property.FindPropertyRelative("next"), GUIContent.none, true);

        //if (GUI.Button(nextAdd, "+")) { property.}
        //EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;


        EditorGUI.EndProperty();
    }
}

