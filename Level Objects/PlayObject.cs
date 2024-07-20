using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fields;


/// <summary>
/// Основной объект, от которого наследуют игровые объекты.
/// </summary>
public class PlayObject : MonoBehaviour
{
    public PlayObject playObject { get { return this; } }

    [Tooltip("Фактические входящие направления света, заполняемые во время игры")]
    public List<Beam> inputBeams = new List<Beam>();
    public List<Beam> outputBeams = new List<Beam>();

    private TMPro.TextMeshPro coordinates;

    public virtual void OnLightBeamHit(Beam beam) 
    {
        inputBeams.Add(beam);

    }

    public virtual void OnLightBeamLeft(Beam beam)
    {
        inputBeams.Remove(beam);
    }

    /// <summary>
    /// Выводит/убирает координаты объекты чуть ниже самого объекта. Для отладки.
    /// </summary>
    public void VisualizeCoordinates(bool on)
    {
        if (coordinates == null)
        {
            if (on)
            {
                coordinates = Instantiate(Prefabs.Instance.playObjectCoordinates);
                coordinates.transform.SetParent(transform);
                coordinates.transform.localPosition = new Vector3(0, coordinates.transform.localScale.x / -1.3f, -5);
                coordinates.text = $"{(int)transform.position.x};{(int)transform.position.y}";
            }
        }
        else if(on)
            coordinates.gameObject.SetActive(true);
        else
            coordinates.gameObject.SetActive(false);
    }
}
