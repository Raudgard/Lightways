using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

namespace Tools
{
    /// <summary>
    /// Отсчитывает время. Изменяет текст в соответствии с оставшимся количеством секунд.
    /// </summary>
    public class TimeCounter : MonoBehaviour
    {
        [Tooltip("Объект TextMeshProUGUI, который отображает таймер.")]
        [SerializeField] private TextMeshProUGUI timer;
        [Tooltip("Значение, до которого идет отсчет.")]
        [SerializeField] private float endTime;
        [Tooltip("Если установлен, то отсчет на убывание. Если нет - на возрастание.")]
        public bool isCountDown = true;

        [Space]
        [Space]

        [Header("Данные о временном промежутке, во время которого за указанное время\n" +
            "цвет будет изменяться от начального к конечному. При убывающем счетчике\n" +
            "устанавливать следующее значение временной метки меньше, чем предыдущее.\n" +
            "При увеличивающемся счетчике - наоборот.")]

        [Space]

        [Tooltip("Временные сегменты цвета.")]
        public List<TimeColorSegments> timeColorSegments;

        /// <summary>
        /// Начальное количество секунд.
        /// </summary>
        private float secondsOnStart;

        /// <summary>
        /// Осталось секунд.
        /// </summary>
        public float currentSeconds { get; private set; }
        private Action endCountingCallback;
        private Coroutine timingCoroutine;


        /// <summary>
        /// Начинает отсчет времени.
        /// </summary>
        /// <param name="secondsCount">Устанавливает таймер на этой количество секунд.</param>
        /// <param name="startColor">Устанавливает начальный цвет счетчика.</param>
        /// <param name="endCountingCallback">Метод, исполняемый по окончанию отсчета.</param>
        public void StartCounting(float secondsCount, Color startColor, Action endCountingCallback)
        {
            this.secondsOnStart = secondsCount;
            timer.color = startColor;
            this.endCountingCallback = endCountingCallback;

            timer.text = secondsCount.ToString();
            if (timeColorSegments == null || timeColorSegments[0] == null)
            {
                Debug.LogError("Необходимо установить минимум один временно-цветовой сегмент для счетчика времени");
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
        /// Останавливает отсчет времени.
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
        /// Отсчитывает время, уменьшая значение.
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
        /// Отсчитывает время, увеличивая значение.
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
        /// При достижении счетчиком указанного финального значения.
        /// </summary>
        private void TimesUp()
        {
            //timer.text = endTime.ToString();
            endCountingCallback?.Invoke();
            timingCoroutine = null;
        }



        /// <summary>
        /// Данные о временном промежутке, во время которого за указанное время цвет будет изменяться от начального к конечному.
        /// При убывающем счетчике устанавливать следующее значение временной метки меньше, чем предыдущее.
        /// При увеличивающемся счетчике - наоборот.
        /// </summary>
        [Serializable]
        public class TimeColorSegments
        {
            [Tooltip("Время начала изменения цвета.")]
            public float timeStamp;
            [Tooltip("Начальный цвет.")]
            public Color startColor;
            [Tooltip("Конечный цвет.")]
            public Color endColor;
            [Tooltip("Время для достижения этого цвета.")] 
            public float timeToChange;
        }
    }

}