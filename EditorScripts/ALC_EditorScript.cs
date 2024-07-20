using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Fields;

#if UNITY_EDITOR
[CustomEditor(typeof(AutomaticLevelCreator))]
public class ALC_EditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        AutomaticLevelCreator alc = target as AutomaticLevelCreator;

        if (alc.minSizeX > 0 || alc.maxSizeX > 0) alc.sizeX = alc.sizeY = 0;
        if (alc.sizeX > 0 || alc.sizeY > 0) alc.minSizeX = alc.maxSizeX = 0;

        //if (alc.winTowersCount > 0 && alc.maxTotalTowersInMatrix < alc.winTowersCount) alc.maxTotalTowersInMatrix = alc.winTowersCount;
        //if (alc.minWinTowersCount > 0 || alc.maxWinTowersCount > 0) alc.winTowersCount = 0;
        //if (alc.winTowersCount > 0 ) alc.minWinTowersCount = alc.maxWinTowersCount = 0;


        GUILayout.Label("������ ������� �� X. ���������� � 0 ��� ����. �������.");
        alc.sizeX = EditorGUILayout.IntField("Size X", alc.sizeX);
        GUILayout.Space(10);

        GUILayout.Label("������ ������� �� Y. ������ 2*X. ���������� � 0 ��� ����. ����.");
        alc.sizeY = EditorGUILayout.IntField("Size Y", alc.sizeY);
        GUILayout.Space(10);


        GUILayout.Label("D�� ������� ���������� ������� �������. ����������� ������ �� X.");
        alc.minSizeX = EditorGUILayout.IntField("Min Size X", alc.minSizeX);
        GUILayout.Space(10);

        GUILayout.Label("D�� ������� ���������� ������� �������. ������������ ������ �� X.");
        alc.maxSizeX = EditorGUILayout.IntField("Max Size X", alc.maxSizeX);
        GUILayout.Space(30);
        

        //GUILayout.Label("����. ���-�� ����� � �������. �� ����� ���� ������ winTowersCount.");
        //alc.maxTotalTowersInMatrix = EditorGUILayout.IntField("Max total towers in matrix", alc.maxTotalTowersInMatrix);
        //GUILayout.Space(30);



        GUILayout.Label("���. ���-�� �����. �����. �������. ��� ������� ��������� ��������.");
        alc.minWinTowersCount = EditorGUILayout.IntField("Min win towers count", alc.minWinTowersCount);
        GUILayout.Space(10);

        GUILayout.Label("����. ���-�� �����. �����. �������. ��� ������� ��������� ��������.");
        alc.maxWinTowersCount = EditorGUILayout.IntField("Max win towers count", alc.maxWinTowersCount);
        GUILayout.Space(30);


        GUILayout.Label("����������� ������. ����. �������. �������� 0%, 25%, 50%, 75%, 100%.");
        alc.diagonalsUsing = (DiagonalsUsing)EditorGUILayout.EnumPopup("Diagonals using", alc.diagonalsUsing);
        GUILayout.Space(30);


        GUILayout.Label("���������� ������ �������� �����.");
        alc.falseWinLineCount = EditorGUILayout.IntField("False Win Line Count", alc.falseWinLineCount);
        GUILayout.Space(10);

        GUILayout.Label("���������� ���� � ������ ������ �������� �����.");
        alc.towersInFalseWinLineCount = EditorGUILayout.IntField("Count of towers in each false Win Line", alc.towersInFalseWinLineCount);
        GUILayout.Space(30);


        GUILayout.Label("������������ ���������� ����� � ������ �����.");
        alc.maxTowersInFalseLine = EditorGUILayout.IntField("Max towers in false line", alc.maxTowersInFalseLine);
        GUILayout.Space(30);

        GUILayout.Label("���. ����� ���������� ������� ��� ������ �����.");
        alc.minTotalArrows = EditorGUILayout.IntField("minTotalArrows", alc.minTotalArrows);
        GUILayout.Space(10);

        GUILayout.Label("����. ����� ���������� ������� ��� ������ �����.");
        alc.maxTotalArrows = EditorGUILayout.IntField("False arrows", alc.maxTotalArrows);
        GUILayout.Space(30);

        GUILayout.Label("����� �������� �������� ����� ��� ��������� ����� �� ��� ���������\n�� 1 ������ �����. ���� ������� ���������� ��� �������� �����\n� ������� ����� ����� ����� ������� ��� �� 1 ������ �����.");
        alc.percentOfFalseTowersToCreateNewFalceLine = EditorGUILayout.IntSlider("percent of false towers", alc.percentOfFalseTowersToCreateNewFalceLine, 0, 100);
        GUILayout.Space(50);



        if (GUILayout.Button("Create matrix", GUILayout.Width(250), GUILayout.Height(30)))
        {
            //AutomaticLevelCreator automaticLevelCreator = target as AutomaticLevelCreator;
            ////Debug.Log("Create matrix...");
            //automaticLevelCreator.CreateMatrix();

            FindObjectOfType<TowersMatrix>().CreateMatrix();
        }






    }

}
#endif