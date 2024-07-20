using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Fields;

#if UNITY_EDITOR
[CustomEditor(typeof(TowersMatrix))]
public class TowersMatrixEditorScript : Editor
{
    string fileName;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Create matrix via Automatic Level Creator", GUILayout.Width(250), GUILayout.Height(30)))
        {
            TowersMatrix towersMatrix = target as TowersMatrix;
            //Debug.Log("Create matrix...");
            towersMatrix.CreateMatrix();
        }




        GUILayout.Space(10);

        if (GUILayout.Button("Get matrix from screen", GUILayout.Width(250), GUILayout.Height(30)))
        {
            TowersMatrix towersMatrix = target as TowersMatrix;
            Debug.Log("Get matrix");
            towersMatrix.GetMatrixFromScreen();
        }

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Highlight win way", GUILayout.Width(250), GUILayout.Height(30)))
        {
            TowersMatrix towersMatrix = target as TowersMatrix;
            Debug.Log("Enable hightlight winway towers");
            towersMatrix.HighlightWinningTowers(true);
        }

        if (GUILayout.Button("Remove highlight win way", GUILayout.Width(250), GUILayout.Height(30)))
        {
            TowersMatrix towersMatrix = target as TowersMatrix;
            Debug.Log("Disable hightlight winway towers");
            towersMatrix.HighlightWinningTowers(false);
        }


        GUILayout.EndHorizontal();


        GUILayout.Space(30);

        GUILayout.Label("Format: \"Number of Difficulty\"_\"Number of level\". For example: 3_87");
        GUILayout.BeginHorizontal();
        GUILayout.Label("Name of saving file.");
        fileName = GUILayout.TextField(fileName, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / 2));
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        if (GUILayout.Button("Save matrix into file", GUILayout.Width(250), GUILayout.Height(30)))
        {
            if(string.IsNullOrEmpty(fileName))
            {
                Debug.LogWarning("Нужно ввести имя уровня для сохранения");
                return;
            }

            TowersMatrix towersMatrix = target as TowersMatrix;
            Debug.Log($"Saving {fileName} file...");
            towersMatrix.SaveLevelIntoFile(fileName, towersMatrix.levelType, towersMatrix.timeForFTLLevel);


            if (int.TryParse(fileName.Split('_')[1], out int level))
            {
                fileName = $"{fileName[0]}_{++level}";
            }    

        }

        GUILayout.Space(10);

        if (GUILayout.Button("Load matrix from file", GUILayout.Width(250), GUILayout.Height(30)))
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Debug.LogWarning("Нужно ввести имя уровня для загрузки");
                return;
            }

            TowersMatrix towersMatrix = target as TowersMatrix;
            Debug.Log("Loading file...");
            towersMatrix.StartCoroutine(towersMatrix.LoadLevelFromFile(fileName));
        }
    }
}
#endif
