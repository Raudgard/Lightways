using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

namespace Tools
{
    /// <summary>
    /// ����������� �����. �������� ����� � ������������ � ���������� ����������� ������.
    /// </summary>
    public class TimeCounter : MonoBehaviour
    {
        [Tooltip("������ TextMeshProUGUI, ������� ���������� ������.")]
        [SerializeField] private TextMeshProUGUI timer;
        [Tooltip("��������, �� �������� ���� ������.")]
        [SerializeField] private float endTime;
        [Tooltip("���� ����������, �� ������ �� ��������. ���� ��� - �� �����������.")]
        public bool isCountDown = true;

        [Space]
        [Space]

        [Header("������ � ��������� ����������, �� ����� �������� �� ��������� �����\n" +
            "���� ����� ���������� �� ���������� � ���������. ��� ��������� ��������\n" +
            "������������� ��������� �������� ��������� ����� ������, ��� ����������.\n" +
            "��� ��������������� �������� - ��������.")]

        [Space]

        [Tooltip("��������� �������� �����.")]
        public List<TimeColorSegments> timeColorSegments;

        /// <summary>
        /// ��������� ���������� ������.
        /// </summary>
        private float secondsOnStart;

        /// <summary>
        /// �������� ������.
        /// </summary>
        public float currentSeconds { get; private set; }
        private Action endCountingCallback;
        private Coroutine timingCoroutine;


        /// <summary>
        /// �������� ������ �������.
        /// </summary>
        /// <param name="secondsCount">������������� ������ �� ���� ���������� ������.</param>
        /// <param name="startColor">������������� ��������� ���� ��������.</param>
        /// <param name="endCountingCallback">�����, ����������� �� ��������� �������.</param>
        public void StartCounting(float secondsCount, Color startColor, Action endCountingCallback)
        {
            this.secondsOnStart = secondsCount;
            timer.color = startColor;
            this.endCountingCallback = endCountingCallback;

            timer.text = secondsCount.ToString();
            if (timeColorSegments == null || timeColorSegments[0] == null)
            {
                Debug.LogError("���������� ���������� ������� ���� ��������-�������� ������� ��� �������� �������");
                return;
            }

            if(timeColorSegments[0].timeStamp >= endTime)
            {
                timingCoroutine = StartCoroutine(CountDown());
            }
            else
            {
                timingCoroutine = StartCoroutine(CountUp());
            }
        }

        /// <summary>
        /// ������������� ������ �������.
        /// </summary>
        public void StopCounting()
        {
            if (timingCoroutine != null)
            {
                StopCoroutine(timingCoroutine);
            }
            MusicController.Instance.TimerTickingStop();
            timingCoroutine = null;
        }

        /// <summary>
        /// ����������� �����, �������� ��������.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CountDown()
        {
            currentSeconds = secondsOnStart;
            int secondsToShowCurrent = int.MinValue;
            int segmentsCounter = 0;
            TimeColorSegments timeColorSegment;
            TimeColorSegments nextTimeColorSegment;

            if (secondsOnStart > timeColorSegments[0].timeStamp)
            {
                while (currentSeconds > timeColorSegments[0].timeStamp)
                {
                    currentSeconds -= Time.deltaTime;

                    int secondsToShow = Mathf.CeilToInt(currentSeconds);
                    if (secondsToShow != secondsToShowCurrent)
                    {
                        timer.text = secondsToShow.ToString();
                        secondsToShowCurrent = secondsToShow;
                        if (secondsToShow == 10)
                        {
                            MusicController.Instance.TimerTickingPlay();
                        }
                    }
                    yield return null;
                }
            }

            while (segmentsCounter < timeColorSegments.Count)
            {
                timeColorSegment = timeColorSegments[segmentsCounter];
                nextTimeColorSegment = timeColorSegments.Count > segmentsCounter + 1 ? timeColorSegments[segmentsCounter + 1] : null;

                float nextTimeStamp;
                float timeInterval;
                if (nextTimeColorSegment != null)
                {
                    timeInterval = timeColorSegment.timeStamp - nextTimeColorSegment.timeStamp;
                    nextTimeStamp = nextTimeColorSegment.timeStamp < endTime ? endTime : nextTimeColorSegment.timeStamp;
                }
                else
                {
                    timeInterval = timeColorSegment.timeStamp - endTime;
                    nextTimeStamp = endTime;
                }
                //Debug.Log($"segmentsCounter: {segmentsCounter}, timeInterval: {timeInterval}, nextTimeStamp:{nextTimeStamp}");


                while (currentSeconds > nextTimeStamp)
                {
                    currentSeconds -= Time.deltaTime;
                    timeInterval -= Time.deltaTime;
                    timer.color = Color.Lerp(timeColorSegment.endColor, timeColorSegment.startColor, timeInterval / timeColorSegment.timeToChange);
                    //Debug.Log($"timer.color: {timer.color}, endColor:{timeColorSegment.endColor}, startColor: {timeColorSegment.startColor}, timeInterval: {timeInterval}, timeToChange: {timeColorSegment.timeToChange}, lerpT: {timeInterval / timeColorSegment.timeToChange}");

                    int secondsToShow = Mathf.CeilToInt(currentSeconds);

                    if (secondsToShow != secondsToShowCurrent)
                    {
                        timer.text = secondsToShow.ToString();
                        secondsToShowCurrent = secondsToShow;
                    }
                    yield return null;
                }

                segmentsCounter++;
            }

            TimesUp();
        }

        /// <summary>
        /// ����������� �����, ���������� ��������.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CountUp()
        {
            currentSeconds = secondsOnStart;
            int secondsToShowCurrent = int.MaxValue;
            int segmentsCounter = 0;
            TimeColorSegments timeColorSegment;
            TimeColorSegments nextTimeColorSegment;

            if (secondsOnStart < timeColorSegments[0].timeStamp)
            {
                while (currentSeconds < timeColorSegments[0].timeStamp)
                {
                    currentSeconds += Time.deltaTime;

                    int secondsToShow = Mathf.FloorToInt(currentSeconds);
                    if (secondsToShow != secondsToShowCurrent)
                    {
                        timer.text = secondsToShow.ToString();
                        secondsToShowCurrent = secondsToShow;
                    }
                    yield return null;
                }
            }

            while (segmentsCounter < timeColorSegments.Count)
            {
                timeColorSegment = timeColorSegments[segmentsCounter];
                nextTimeColorSegment = timeColorSegments.Count > segmentsCounter + 1 ? timeColorSegments[segmentsCounter + 1] : null;

                float nextTimeStamp;
                float timeInterval;
                if (nextTimeColorSegment != null)
                {
                    timeInterval = nextTimeColorSegment.timeStamp - timeColorSegment.timeStamp;
                    nextTimeStamp = nextTimeColorSegment.timeStamp > endTime ? endTime : nextTimeColorSegment.timeStamp;
                }
                else
                {
                    timeInterval = endTime - timeColorSegment.timeStamp;
                    nextTimeStamp = endTime;
                }


                while (currentSeconds < nextTimeStamp)
                {
                    currentSeconds += Time.deltaTime;
                    timeInterval -= Time.deltaTime;
                    timer.color = Color.Lerp(timeColorSegment.endColor, timeColorSegment.startColor, timeInterval / timeColorSegment.timeToChange);
                    //Debug.Log($"timer.color: {timer.color}, endColor:{timeColorSegment.endColor}, startColor: {timeColorSegment.startColor}, timeInterval: {timeInterval}, timeToChange: {timeColorSegment.timeToChange}, lerpT: {timeInterval / timeColorSegment.timeToChange}");

                    int secondsToShow = Mathf.FloorToInt(currentSeconds);

                    if (secondsToShow != secondsToShowCurrent)
                    {
                        timer.text = secondsToShow.ToString();
                        secondsToShowCurrent = secondsToShow;
                    }
                    yield return null;
                }

                segmentsCounter++;
            }

            TimesUp();
        }

        /// <summary>
        /// ��� ���������� ��������� ���������� ���������� ��������.
        /// </summary>
        private void TimesUp()
        {
            //timer.text = endTime.ToString();
            endCountingCallback?.Invoke();
            timingCoroutine = null;
        }



        /// <summary>
        /// ������ � ��������� ����������, �� ����� �������� �� ��������� ����� ���� ����� ���������� �� ���������� � ���������.
        /// ��� ��������� �������� ������������� ��������� �������� ��������� ����� ������, ��� ����������.
        /// ��� ��������������� �������� - ��������.
        /// </summary>
        [Serializable]
        public class TimeColorSegments
        {
            [Tooltip("����� ������ ��������� �����.")]
            public float timeStamp;
            [Tooltip("��������� ����.")]
            public Color startColor;
            [Tooltip("�������� ����.")]
            public Color endColor;
            [Tooltip("����� ��� ���������� ����� �����.")] 
            public float timeToChange;
        }
    }

}