using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Tools
{
    /// <summary>
    /// ����� � ���������� ���������������� ��������.
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
        /// �������� ������� ������� �������.
        /// </summary>
        /// <param name="transform">��������� ����������� �������.</param>
        /// <param name="scaleFrom">������, �� �������� ���� ���������.</param>
        /// <param name="scaleTo">������, � �������� ���� ���������.</param>
        /// <param name="speed">�������� ���������.</param>
        /// <param name="callback">�������, ����������� ����� ��������� ���������. �� ����� �������� ��� ����������� ������� �� ����� ���������.</param>
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
        ///// ��������� ������� ������� �������. ���������� �� FPS.
        ///// </summary>
        ///// <param name="transform">��������� ������������ �������.</param>
        ///// <param name="scaleTarget">������, �� �������� ���� ����������. ���������� ��������� ����������, ������ ��� �� ������ ����� scale � ������� transform.</param>
        ///// <param name="speed">�������� ����������.</param>
        ///// <param name="callback">�������, ����������� ����� ��������� ����������. ����� �������� � ��� ����������� ������� �� ����� ����������.</param>
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