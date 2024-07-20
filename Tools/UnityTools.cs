using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Tools
{
    /// <summary>
    /// Класс с различными вспомогательными методами.
    /// </summary>
    public class UnityTools : MonoBehaviour
    {
        #region Singleton
        private static UnityTools instance;
        //public static UnityTools Instance
        //{
        //    get { return instance; }
        //}
        #endregion

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }



        public static void ExecuteWithDelay(Action action, int framesDelay)
        {
            instance.StartCoroutine(instance.Executing(action, framesDelay));
        }

        public static void ExecuteWithDelay(Action action, float secondsDelay)
        {
            instance.StartCoroutine(instance.Executing(action, secondsDelay));
        }


        private IEnumerator Executing(Action action, int framesDelay)
        {
            for (int i = 0; i < framesDelay; i++)
            {
                yield return null;
            }
            action.Invoke();
        }

        private IEnumerator Executing(Action action, float secondsDelay)
        {
            yield return new WaitForSeconds(secondsDelay);
            action.Invoke();
        }


        /// <summary>
        /// Изменяет масштаб объекта линейно.
        /// </summary>
        /// <param name="transform">Трансформ изменяемого объекта.</param>
        /// <param name="scaleFrom">Размер, от которого идет изменение.</param>
        /// <param name="scaleTo">Размер, к которому идет изменение.</param>
        /// <param name="speed">Скорость изменения.</param>
        /// <param name="callback">Делегат, исполняемый после окончания изменения. Не будет выполнен при уничтожении объекта во время изменения.</param>
        public static void ChangeScaleLinearly(Transform transform, Vector3 scaleFrom, Vector3 scaleTo, float speed, Action callback)
        {
            instance.StartCoroutine(instance.ChangingScale(transform, scaleFrom, scaleTo, speed, callback));
        }


        private IEnumerator ChangingScale(Transform transform, Vector3 scaleFrom, Vector3 scaleTo, float speed, Action callback)
        {
            var maxScaleX = scaleTo.x;
            float t = 0;

            while (t < 1)
            {
                if (transform == null)
                {
                    Debug.LogWarning("Transform is null! Breaking");
                    yield break;
                }

                transform.localScale = Vector3.Lerp(scaleFrom, scaleTo, t);
                t += speed * Time.deltaTime;
                yield return null;
            }

            if (transform != null)
            {
                transform.localScale = scaleTo;
            }

            callback?.Invoke();
        }



        ///// <summary>
        ///// Уменьшает масштаб объекта линейно. Независимо от FPS.
        ///// </summary>
        ///// <param name="transform">Трансформ уменьшаемого объекта.</param>
        ///// <param name="scaleTarget">Размер, от которого идет уменьшение. Передается отдельным параметром, потому что не всегда равен scale у данного transform.</param>
        ///// <param name="speed">Скорость уменьшения.</param>
        ///// <param name="callback">Делегат, исполняемый после окончания уменьшения. Будет выполнен и при уничтожении объекта во время уменьшения.</param>
        //public static void DecreaseScaleLinearly(Transform transform, Vector3 scaleTarget, float speed, Action callback)
        //{
        //    instance.StartCoroutine(instance.DecreasingScale(transform, scaleTarget, speed, callback));
        //}


        //private IEnumerator DecreasingScale(Transform transform, Vector3 scaleTarget, float speed, Action callback)
        //{
        //    var minScaleX = scaleTarget.x;
        //    Vector3 reduction;
        //    if (transform != null)
        //    {
        //        reduction = new Vector3(Mathf.Abs(scaleTarget.x - transform.localScale.x) * speed, Mathf.Abs(scaleTarget.y - transform.localScale.y) * speed, Mathf.Abs(scaleTarget.z - transform.localScale.z) * speed);
        //    }
        //    else
        //    {
        //        reduction = new Vector3(speed, speed, speed);
        //    }

        //    while (transform != null && transform.localScale.x > minScaleX)
        //    {
        //        transform.localScale -= reduction;
        //        yield return null;
        //    }

        //    if (transform != null)
        //    {
        //        transform.localScale = scaleTarget;
        //    }

        //    callback?.Invoke();
        //}







        //public static void SmoothTransition<T>(T value, T startValue, T endValue, float speed, Action callback)
        //{
        //    instance.StartCoroutine(instance.SmoothTransisting<T>(value, startValue, endValue, speed, callback));
        //}

        //public async static void SmoothTransition<T>(T value, T startValue, T endValue, float speed)
        //{

        //}


        //private IEnumerator SmoothTransisting<T>(T value, T startValue, T endValue, float speed, Action callback)
        //{
        //    float t = 0;
        //    var originalColor = backgroundSmoke.color;
        //    float a = 0;

        //    while (backgroundSmoke.color.a < 1)
        //    {
        //        backgroundSmoke.color = new Color(originalColor.r, originalColor.g, originalColor.b, a);
        //        t += Time.fixedDeltaTime;
        //        a += t * speed;
        //        yield return null;
        //    }

        //    yield return null;
        //    callback.Invoke();
        //}


    }
}