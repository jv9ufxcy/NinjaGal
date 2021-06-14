using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterScriptEditorWindow : EditorWindow
{
    [MenuItem("Window/Character Script Editor")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(CharacterScriptEditorWindow), false, "Character Script Editor");
    }
    CoreData coreData;
    int currentScriptIndex;


    Vector2 scrollView;
    private void OnGUI()
    {
        if (coreData == null)
        {
            foreach (string guid in AssetDatabase.FindAssets("t: CoreData"))
            {
                coreData = AssetDatabase.LoadAssetAtPath<CoreData>(AssetDatabase.GUIDToAssetPath(guid));
            }
        }
        scrollView = GUILayout.BeginScrollView(scrollView);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Script Index : " + currentScriptIndex.ToString());
        currentScriptIndex = EditorGUILayout.Popup(currentScriptIndex, coreData.GetScriptNames());
        if (GUILayout.Button("New Character Script"))
        {
            coreData.characterScripts.Add(new CharacterScript());
            currentScriptIndex = coreData.characterScripts.Count - 1;
        }
        EditorGUILayout.EndHorizontal();
        CharacterScript currentScript = coreData.characterScripts[currentScriptIndex];
        currentScript.name = EditorGUILayout.TextField("Name : ", currentScript.name);
        int deleteParam = -1;
        for (int p = 0; p < currentScript.parameters.Count; p++)
        {
            ScriptParameters currentParam = currentScript.parameters[p];
            EditorGUILayout.BeginHorizontal();

            currentParam.name = EditorGUILayout.TextField("Parameter Name : ", currentParam.name);
            if (GUILayout.Button("x", GUILayout.Width(25))) { deleteParam = p; }

            EditorGUILayout.EndHorizontal();
            currentParam.val = EditorGUILayout.FloatField("Default Value : ", currentParam.val);
        }
        if(deleteParam>-1) { currentScript.parameters.RemoveAt(deleteParam); }
        if (GUILayout.Button("+", GUILayout.Width(25)))
        {
            currentScript.parameters.Add(new ScriptParameters());
        }
        GUILayout.EndScrollView();
        EditorUtility.SetDirty(coreData);
    }
}
