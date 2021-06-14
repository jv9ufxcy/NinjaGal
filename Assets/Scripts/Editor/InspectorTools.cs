using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Hitbox))]
public class InspectorTools : Editor
{
    public int attackEventIndex = 0;
    public CoreData coreData;
    public CharacterState state;
    

    public override void OnInspectorGUI()
    {
        Hitbox h = (Hitbox)target;
        DrawDefaultInspector();
        if (coreData == null)
        {
            foreach (string guid in AssetDatabase.FindAssets("t: CoreData"))//looks at whole project for assets tagged CoreData
            {
                coreData = AssetDatabase.LoadAssetAtPath<CoreData>(AssetDatabase.GUIDToAssetPath(guid));
            }
        }
        if (GUILayout.Button("Apply Hitbox"))
        {
            state = coreData.characterStates[h.stateIndex];
            for (int i = 0; i < state.attacks.Count; i++)
            {
                AttackEvent atk = state.attacks[i];
                atk.hitBoxPos = h.transform.localPosition;
                atk.hitBoxScale = h.transform.localScale;
            }
            EditorUtility.SetDirty(coreData);
            AssetDatabase.SaveAssets();
        }
    }
}
