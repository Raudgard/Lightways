using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;
using UnityEngine.Serialization;


namespace UITools
{
    /// <summary>
    /// Прикрепляется к UI объекту, который необходимо передвинуть на сцену. Предоставляет делегат для добавления метода, срабатывающего в момент отпускания объекта на сцену.
    /// </summary>
    public class DragAndDropObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [Tooltip("Сamera rendering UI objects in a scene.")]
        public Camera cameraUI;

        [Serializable]
        public class OnPointerDownEvent : UnityEvent<PointerEventData> { }

        // Event delegates triggered on click.
        /// <summary>
        /// OnInvoke(PointerEventData pointerEventData)
        /// </summary>
        [FormerlySerializedAs("onPointerDownEvent")]
        [SerializeField]
        [Tooltip("Срабатывает во время нажатия на кнопку мыши либо на экран смартфона.")]
        private OnPointerDownEvent onPointerDownEvent = new OnPointerDownEvent();


        [Serializable]
        public class OnDragEvent : UnityEvent<PointerEventData, float> { }

        // Event delegates triggered on dragging object.
        /// <summary>
        /// OnInvoke(PointerEventData pointerEventData)
        /// </summary>
        [FormerlySerializedAs("onDragEvent")]
        [SerializeField]
        private OnDragEvent onDragEvent = new OnDragEvent();


        [Serializable]
        public class OnEndDragEvent : UnityEvent<PointerEventData> { }

        // Event delegates triggered on end drag object.
        /// <summary>
        /// OnInvoke(PointerEventData pointerEventData)
        /// </summary>
        [FormerlySerializedAs("onEndDragEvent")]
        [SerializeField]
        private OnEndDragEvent onEndDragEvent = new OnEndDragEvent();

        /// <summary>
        /// Вектор, составляющий разницу между положением передвигаемого объекта и точкой, где за объект "взялся" пользователь (точку нажатия).
        /// </summary>
        private Vector2 shiftVector;

        /// <summary>
        /// Соотношение размера камеры, отображающей объекты UI к размеру экрана.
        /// </summary>
        private float verticalRatio;


        public void OnDrag(PointerEventData eventData)
        {
            //Debug.Log($"OnDrag. eventData.position: {eventData.position}, eventData.delta: {eventData.delta}, eventData.scrollDelta: {eventData.scrollDelta}");
            

            //float shift = cameraUI.orthographicSize * 2 / Screen.height;
            Vector2 pos = eventData.position * verticalRatio - shiftVector;
            gameObject.transform.position = new Vector3(pos.x, pos.y, gameObject.transform.position.z);
            //Debug.Log($"eventData.position: {eventData.position}, shiftVector: {shiftVector}, gO position: {gameObject.transform.position}, shift: {verticalRatio}");

            onDragEvent.Invoke(eventData, verticalRatio);
        }

        public void OnDrop(PointerEventData eventData)
        {
            //Debug.Log($"OnDrop. eventData.position: {eventData.position}, eventData.delta: {eventData.delta}, eventData.scrollDelta: {eventData.scrollDelta}");
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //Debug.Log($"OnEndDrag. eventData.position: {eventData.position}, eventData.delta: {eventData.delta}, eventData.scrollDelta: {eventData.scrollDelta}");
            onEndDragEvent.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //Debug.Log($"OnPointerDown. eventData.position: {eventData.position}, eventData.delta: {eventData.delta}, eventData.scrollDelta: {eventData.scrollDelta}");
            if (cameraUI == null)
            {
                Debug.LogError("First you have to assign a camera.");
                return;
            }

            verticalRatio = cameraUI.orthographicSize * 2 / Screen.height;
            shiftVector = eventData.position * verticalRatio - (Vector2)gameObject.transform.position ;
            onPointerDownEvent.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //Debug.Log($"OnPointerUp. eventData.position: {eventData.position}, eventData.delta: {eventData.delta}, eventData.scrollDelta: {eventData.scrollDelta}");
        }

    }
}