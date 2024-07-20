using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

//[RequireComponent(typeof(ClickableObject))]
public class Arrow : MonoBehaviour
{
    public Tower towerOwner;
    public Direction direction;
    public float startPositionPointValue;

    public float speedForwardMoving;
    public float speedReverceMoving;
    public float destinationPointValue;
    public float tresholdForReverceMoving;
    public float zPosition;
    private Vector2 destinationPoint;
    private Vector2 startPoint;

    //private Coroutine hintSizeFlashing = null;
    private bool hintSizeFlashing = false;


    /// <summary>
    /// Необходима подсказка этой стрелкой?
    /// </summary>
    public bool IsNeedToHint { get; set; } = false;


    private void Start()
    {
        TakeStartingPositionsAndRotation();
    }

    public void TakeStartingPositionsAndRotation()
    {
        GetStartPoint();
        transform.localRotation = Quaternion.Euler(0, 0, (int)direction);
        transform.localPosition = destinationPoint = GetDestinationPoint();
    }

    /// <summary>
    /// Срабатывает при нажатии на стрелку.
    /// </summary>
    public void OnClick()
    {
        //print($"Mouse left click on arrow: {gameObject.name}");
        towerOwner.SendLightBeam(direction);
    }

    /// <summary>
    /// Устанавливает положение стрелки между startPoint и destinationPoint, используя значение t. При t = 0, стрелка находится на startPoint,
    /// при t = 1, стрелка находится на destinationPoint. Используется Vector2.Lerp.
    /// </summary>
    /// <param name="t">Значение положения стрелки. Должно быть между 0 и 1.</param>
    public void SetArrowPosition(float t)
    {
        var pos2D = Vector2.Lerp(startPoint, destinationPoint, t);
        transform.localPosition = new Vector3(pos2D.x, pos2D.y, zPosition);
    }


    /// <summary>
    /// Установить стартовую точку.
    /// </summary>
    /// <param name="pointValue">Расстояние от нулевой точки до желаемой точки.</param>
    /// <returns></returns>
    private void GetStartPoint()
    {
        startPoint = new Vector2(0, startPositionPointValue).TurnOnDirection(direction);
    }


    /// <summary>
    /// Получить финальную точку на оси движения стрелки.
    /// </summary>
    /// <param name="pointValue">Расстояние от нулевой точки до желаемой точки.</param>
    /// <returns></returns>
    private Vector2 GetDestinationPoint()
    {
        return new Vector2(0, destinationPointValue).TurnOnDirection(direction);
    }


    public static Arrow Create(Direction direction, Tower owner)
    {
        Arrow arrow = Instantiate(Prefabs.Instance.arrowPrefab);
        arrow.direction = direction;
        arrow.towerOwner = owner;
        return arrow;
    }


    /// <summary>
    /// Выделяет эту стрелку, как правильную.
    /// </summary>
    public bool HintActivate()
    {
        if (!hintSizeFlashing)
        {
            IsNeedToHint = true;
            //hintSizeFlashing = StartCoroutine(SizeFlashing());
            SizeFlashing();
            return true;
        }
        return false;
    }


    private void SizeFlashing()
    {
        hintSizeFlashing = true;
        var _transform = transform;
        Vector3 originalScale = transform.localScale;
        Vector3 maxScale = transform.localScale * GameController.Instance.Settings.maxSizeOnHintSizeFlashing;
        float speed = GameController.Instance.Settings.speedChangingSizeOnHintSizeFlashing;


        UnityTools.ChangeScaleLinearly(_transform, originalScale, maxScale, speed, delegate
        {
            UnityTools.ChangeScaleLinearly(_transform, maxScale, originalScale, speed, delegate
            {
                if (IsNeedToHint)
                {
                    SizeFlashing();
                }
                else
                {
                    hintSizeFlashing = false;
                }
            });
        });
    }

    //private IEnumerator SizeFlashing()
    //{

    //    UnityTools.ChangeScaleLinearly(_transform, Vector3.zero, maxScale, settings.sphereAppereanceIncreaseSpeed, delegate
    //    {
    //        UnityTools.ChangeScaleLinearly(_transform, maxScale, originalScale, settings.sphereAppereanceIncreaseSpeed, null);
    //    });



    //    while (isNeedToHint)
    //    {



    //    }
    //}

}
