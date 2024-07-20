using System;

/// <summary>
/// Сохраненное состояние одного уровня.
/// </summary>
[Serializable]
public class SaveData
{
    /// <summary>
    /// Сериализованные данные о расположенных в уровне юнитах.
    /// </summary>
    public string data;

    public float directionalLightIntensity;
    public float sphereLightIntensity;
    public float sphereLightRange;
}
