using System;

/// <summary>
/// ����������� ��������� ������ ������.
/// </summary>
[Serializable]
public class SaveData
{
    /// <summary>
    /// ��������������� ������ � ������������� � ������ ������.
    /// </summary>
    public string data;

    public float directionalLightIntensity;
    public float sphereLightIntensity;
    public float sphereLightRange;
}
