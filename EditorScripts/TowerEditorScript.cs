using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(Tower))]
public class TowerEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Create arrows", GUILayout.Width(200), GUILayout.Height(30)))
        {
            Tower tower = target as Tower;
            tower.CreateArrowsFromEditor();
        }



    }

    
}
#endif
