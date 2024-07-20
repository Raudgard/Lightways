using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Tools
{

    public class CameraShake : MonoBehaviour
    {
        [SerializeField] private Transform _transform;

        [Min(0f)]
        [SerializeField] private float duration;
        [SerializeField] private float amplitude;
        [Min(0.01f)]
        [SerializeField] private float frequency;
        [Range(0, 1)]
        [SerializeField] private float decreaseAmplitude;

        private Coroutine shakingCoroutine = null;


        /// <summary>
        /// Создает эффект тряски камеры посредством быстрого и хаотичного ее перемещения относительно исходной точки БЕЗ изменения координаты Z.
        /// </summary>
        /// <param name="duration">Длительность тряски в секундах.</param>
        /// <param name="amplitude">Амплитуда тряски (насколько далеко камера будет перемещаться от исходной точки.</param>
        /// <param name="frequency">Частота перемещения (в секундах между каждым перемещением камеры).</param>
        /// <param name="decreaseAmplitude">Уменьшение амплитуды в течение тяски. Значение от 0 до 1, где 0 - уменьшение амплитуды до 0 в конце тряски, 1 - амплитуда остается одинаковой в течение всего времени тряски.</param>
        /// <returns></returns>
        public void Shake2D(float duration = 1, float amplitude = 1, float frequency = 0.02f, float decreaseAmplitude = 0)
        {
            if(decreaseAmplitude < 0 || decreaseAmplitude > 1)
            {
                Debug.LogWarning("DecreaseAmlitude value must be between 0 and 1. Clamp it.");
                decreaseAmplitude = Mathf.Clamp01(decreaseAmplitude);
            }

            if (shakingCoroutine != null)
            {
                StopCoroutine(shakingCoroutine);
                shakingCoroutine = StartCoroutine(Shaking2D(duration, amplitude, frequency, decreaseAmplitude));
            }
        }

        [Button]
        /// <summary>
        /// Создает эффект тряски камеры посредством быстрого и хаотичного ее перемещения относительно исходной точки БЕЗ изменения координаты Z.
        /// </summary>
        public void Shake2D()
        {
            if (shakingCoroutine != null)
            {
                StopCoroutine(shakingCoroutine);
            }
            shakingCoroutine = StartCoroutine(Shaking2D(duration, amplitude, frequency, decreaseAmplitude));
        }


        private IEnumerator Shaking2D(float duration, float amplitude, float frequency, float decreaseAmplitude)
        {
            Vector3 originPosition = _transform.position;
            float timeLeft = duration;
            float currentAmplitude;
            float endAmplitude = decreaseAmplitude * amplitude;

            while (timeLeft > 0)
            {
                currentAmplitude = Mathf.Lerp(endAmplitude, amplitude, timeLeft / duration);
                //Debug.Log($"currentAmplitude: {currentAmplitude}");

                var randomAdd = (Vector3)(Random.insideUnitCircle * currentAmplitude);
                _transform.position = originPosition + randomAdd;

                timeLeft -= frequency;
                yield return new WaitForSeconds(frequency);
            }

            _transform.position = originPosition;
            shakingCoroutine = null;
        }



        /// <summary>
        /// Создает эффект тряски камеры посредством быстрого и хаотичного ее перемещения относительно исходной точки во всех трех измерениях.
        /// </summary>
        /// <param name="duration">Длительность тряски в секундах.</param>
        /// <param name="amplitude">Амплитуда тряски (насколько далеко камера будет перемещаться от исходной точки.</param>
        /// <param name="frequency">Частота перемещения (в секундах между каждым перемещением камеры).</param>
        /// <param name="decreaseAmplitude">Уменьшение амплитуды в течение тяски. Значение от 0 до 1, где 0 - уменьшение амплитуды до 0 в конце тряски, 1 - амплитуда остается одинаковой в течение всего времени тряски.</param>
        /// <returns></returns>
        public void Shake3D(float duration = 1, float amplitude = 1, float frequency = 0.02f, float decreaseAmplitude = 0)
        {
            if (decreaseAmplitude < 0 || decreaseAmplitude > 1)
            {
                Debug.LogWarning("DecreaseAmlitude value must be between 0 and 1. Clamp it.");
                decreaseAmplitude = Mathf.Clamp01(decreaseAmplitude);
            }

            if (shakingCoroutine != null)
            {
                StopCoroutine(shakingCoroutine);
                shakingCoroutine = StartCoroutine(Shaking3D(duration, amplitude, frequency, decreaseAmplitude));
            }
        }

        [Button]
        /// <summary>
        /// Создает эффект тряски камеры посредством быстрого и хаотичного ее перемещения относительно исходной точки во всех трех измерениях.
        /// </summary>
        public void Shake3D()
        {
            if (shakingCoroutine != null)
            {
                StopCoroutine(shakingCoroutine);
            }
            shakingCoroutine = StartCoroutine(Shaking3D(duration, amplitude, frequency, decreaseAmplitude));
        }

        private IEnumerator Shaking3D(float duration, float amplitude, float frequency, float decreaseAmplitude)
        {
            Vector3 originPosition = _transform.position;
            float timeLeft = duration;
            float currentAmplitude;
            float endAmplitude = decreaseAmplitude * amplitude;

            while (timeLeft > 0)
            {
                currentAmplitude = Mathf.Lerp(endAmplitude, amplitude, timeLeft / duration);
                //Debug.Log($"currentAmplitude: {currentAmplitude}");

                var randomAdd = Random.insideUnitSphere * currentAmplitude;
                _transform.position = originPosition + randomAdd;

                timeLeft -= frequency;
                yield return new WaitForSeconds(frequency);
            }

            _transform.position = originPosition;
            shakingCoroutine = null;
        }


    }
}