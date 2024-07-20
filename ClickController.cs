//ClickController for Android and Windows by Elwood. version 1.0.0.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Fields;
using UnityEngine.Events;
using System;

using UnityEngine.EventSystems;
using UnityEngine.Serialization;

/// <summary>
/// Отлавливает нажатия клавиш, прикосновения к экрану и свайпы и передает их в эвенты.
/// </summary>
[AddComponentMenu("Handlers/Click Controller")]
[DisallowMultipleComponent]
public class ClickController : MonoBehaviour
{
    [Serializable]
    public class OnTouchOrLeftMouseClickEvent : UnityEvent<Vector2> { }

    // Event delegates triggered on click.
    /// <summary>
    /// OnInvoke(Vector2 clickPosition)
    /// </summary>
    [FormerlySerializedAs("onTouchOrLeftMouseClick")]
    [SerializeField]
    private OnTouchOrLeftMouseClickEvent onTouchOrLeftMouseClick = new OnTouchOrLeftMouseClickEvent();


    [Serializable]
    public class OnOneFingerSwipeEvent : UnityEvent<Vector2, Vector2> { }

    // Event delegates triggered when swiping.
    /// <summary>
    /// OnInvoke(Vector2 beganPosition, Vector2 deltaPosition)
    /// </summary>
    [FormerlySerializedAs("onOneFingerSwipeEvent")]
    [SerializeField]
    private OnOneFingerSwipeEvent onOneFingerSwipeEvent = new OnOneFingerSwipeEvent();



    [Serializable]
    public class OnTwoFingersSwipeEvent : UnityEvent<float, Vector2> { }

    // Event delegates triggered when swiping.
    /// <summary>
    /// OnInvoke(float deltaPositions, Vector2 touchDeltaPosition). deltaPosition - разбегание между пальцами, touchDeltaPosition - вектор движения точки посередине между пальцами.
    /// </summary>
    [FormerlySerializedAs("onTwoFingersSwipeEvent")]
    [SerializeField]
    private OnTwoFingersSwipeEvent onTwoFingersSwipeEvent = new OnTwoFingersSwipeEvent();


    [Serializable]
    public class OnEndOneFingerSwipingEvent : UnityEvent<Vector2, Vector2> { }

    // Event delegates triggered when swiping.
    /// <summary>
    /// OnInvoke(Vector2 beganPosition, Vector2 endPosition).
    /// </summary>
    [FormerlySerializedAs("onEndOneFingerSwipingEvent")]
    [SerializeField]
    private OnEndOneFingerSwipingEvent onEndOneFingerSwipingEvent = new OnEndOneFingerSwipingEvent();


    [Serializable]
    public class OnDoubleClickEvent : UnityEvent<Vector2> { }

    // Event delegates triggered when swiping.
    /// <summary>
    /// OnInvoke(Vector2 firstClickPosition).
    /// </summary>
    [FormerlySerializedAs("onDoubleClickEvent")]
    [SerializeField]
    private OnDoubleClickEvent onDoubleClickEvent = new OnDoubleClickEvent();




    public enum SwipeType
    {
        None,
        OneFingerOrLeftMouseClick,
        TwoFingerOrRightMouseClick
    }

    [Space]

    private float mouseX;
    private float mouseY;
    private float mouseWheel;
    private Vector2 beganTouchPosition;

    [Space]

    [Tooltip("Тип сдвигания экрана.")]
    public SwipeType swipeType;

    [Tooltip("Квадратичный размер свайпа, который нужно преодолеть, чтобы свайп засчитался.")]
    [SerializeField] private float swipeTresholdSqr;

    [Space]

    public float twoFingersCoeff;
    public float swipeCoeff;
    public float zoomCoeffForWin;

    [Tooltip("Максимальное время между первым и вторым кликом, чтобы засчитался даблклик.")]
    public float doubleClickTreshold;
    [Tooltip("Расстояние в пикселях сверху экрана. Если движение начинается в нем, то это движение не засчитывается. Чтобы пользователь мог смахивать сверху, и это не учитывалось игрой.")]
    public int deadSpaceTop;



    private bool isPreviousMovedOneFinger;//если перед отпусканием была фаза Move, тогда - true.
    private bool isTwoFingersTouch = false;//нужен для того, чтобы при завершении действия двумя пальцами не срабатывало действие с одним пальцем.
    private float previousClickTime;//время предыдущего клика.
    private bool isBegunIntoDeadSpace;//движение начиналось в "мертвой" зоне




    private void Update()
    {
#if UNITY_ANDROID

        float touchCount = Input.touchCount;
        if(touchCount == 0)
        {
            isTwoFingersTouch = false;
        }
        else if (touchCount == 1)
        {
            if (isTwoFingersTouch)
                return;

            var touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                CheckForBegunIntoDeadSpace(touch);

                if (isBegunIntoDeadSpace)
                    return;

                beganTouchPosition = touch.position;

                #region DoubleClick section
                var timeOfClick = Time.realtimeSinceStartup;
                if (timeOfClick - previousClickTime < doubleClickTreshold)
                {
                    onDoubleClickEvent.Invoke(beganTouchPosition);
                }
                previousClickTime = timeOfClick;
                #endregion
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (isBegunIntoDeadSpace)
                    return;

                onOneFingerSwipeEvent.Invoke(beganTouchPosition, -touch.deltaPosition * swipeCoeff);
                isPreviousMovedOneFinger = true;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (isBegunIntoDeadSpace)
                {
                    isBegunIntoDeadSpace = false;
                    return;
                }

                if (isPreviousMovedOneFinger)
                {
                    isPreviousMovedOneFinger = false;
                    onEndOneFingerSwipingEvent.Invoke(beganTouchPosition, touch.position);
                }
                else
                {
                    onTouchOrLeftMouseClick.Invoke(touch.position);
                }
            }
        }
        else if (touchCount == 2)
        {
            isTwoFingersTouch = true;

            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
            Vector2 prevTouchDelta = touch0PrevPos - touch1PrevPos;
            Vector2 touchDelta = touch0.position - touch1.position;
            Vector2 middlePointDeltaPosition = new Vector2((touch0PrevPos.x + touch1PrevPos.x) / 2, (touch0PrevPos.y + touch1PrevPos.y) / 2) - new Vector2((touch0.position.x + touch1.position.x) / 2, (touch0.position.y + touch1.position.y) / 2);

            float prevTouchDeltaMagnitude = prevTouchDelta.magnitude;
            float touchDeltaMagnitude = touchDelta.magnitude;

            float twoFingersDelta = prevTouchDeltaMagnitude - touchDeltaMagnitude;


            if (twoFingersDelta > 0.01f || twoFingersDelta < -0.01f)
            {
                onTwoFingersSwipeEvent.Invoke(twoFingersDelta * twoFingersCoeff, middlePointDeltaPosition * swipeCoeff);
            }
            //isPreviousMovedOneFinger = true;
        }

#endif


#if UNITY_EDITOR_WIN

        if(Input.GetMouseButtonDown(0))
        {
            beganTouchPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            mouseY = Input.GetAxis("Mouse Y");
            mouseX = Input.GetAxis("Mouse X");
            Vector2 deltaPosition = new Vector2(-mouseX, -mouseY) * Camera.main.orthographicSize / 10;
            onOneFingerSwipeEvent.Invoke(beganTouchPosition, deltaPosition);
        }



        if (Input.GetMouseButtonUp(0))
        {
            if (Input.GetMouseButton(1))
            {
                return;
            }

            #region DoubleClick section
            var timeOfClick = Time.realtimeSinceStartup;
            if (timeOfClick - previousClickTime < doubleClickTreshold)
            {
                onDoubleClickEvent.Invoke(beganTouchPosition);
            }
            previousClickTime = timeOfClick;
            #endregion

            Vector2 endTouchPosition = Input.mousePosition;
            Vector2 swipeVector = endTouchPosition - beganTouchPosition;

            if (swipeVector.sqrMagnitude < swipeTresholdSqr)
            {
                onTouchOrLeftMouseClick.Invoke(Input.mousePosition);
            }
            else
            {
                onEndOneFingerSwipingEvent.Invoke(beganTouchPosition, Input.mousePosition);
            }
        }


        if (Input.GetMouseButton(1))
        {
            //print("Mouse button 1 was pressed!");
            mouseY = Input.GetAxis("Mouse Y");
            mouseX = Input.GetAxis("Mouse X");
            Vector2 deltaPosition = new Vector2(-mouseX, -mouseY) * Camera.main.orthographicSize / 10;

            //onOneFingerSwipeEvent.Invoke(deltaPosition);
            onTwoFingersSwipeEvent.Invoke(0, deltaPosition);
        }

        mouseWheel = Input.GetAxis("Mouse ScrollWheel");

        if (mouseWheel < -0.01f || mouseWheel > 0.01f)
        {
            onTwoFingersSwipeEvent.Invoke(mouseWheel * zoomCoeffForWin, Vector2.zero);
        }
#endif
        
    }

    /// <summary>
    /// Проверяет, находится ли нажатие в мертвой зоне. Если да, переключает isBegunIntoDeadSpace в true.
    /// </summary>
    /// <param name="touch"></param>
    private void CheckForBegunIntoDeadSpace(Touch touch)
    {
        //Debug.Log($"touch position from top: {Screen.height - touch.position.y}");
        isBegunIntoDeadSpace = Screen.height - touch.position.y - deadSpaceTop < 0;
    }

}