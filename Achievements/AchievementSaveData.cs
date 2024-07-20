using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Achievements;

/// <summary>
/// �����, ���������� ������ ��� ���������� ��������� ����������.
/// </summary>
[Serializable]
public class AchievementSaveData
{
    public Achievement_Type achievementType;

    /// <summary>
    /// ���������, ����������� ��� ��������� ����� ����������. ��� ������ ���������� ������ ���. ������, ���������� ������, ����� ����������� ���� � ��.
    /// </summary>
    public int resultRequiredToCompleteAchievement;

    ///// <summary>
    ///// ���������� �����, ����������� ��������� �� ���������� �������. �� ��� ���� ����������.
    ///// </summary>
    //public int starsMax;

    /// <summary>
    /// �������� ��������� ����������.
    /// ��� ���������� � ������� - ��� ���������� ��������� �����.
    /// ��� ���������� ���������� - ��� ���������� ���������� ������.
    /// ��� ���������� ����������� - ��� ������������ ���������� ����������� ������ ����.
    /// ��� ���������� ���� "�������� / �� ��������": 1 - ��������, 0 - �� ��������.
    /// </summary>
    public int mainResult;


}
