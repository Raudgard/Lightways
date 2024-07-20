using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fields;
using UnityEngine.UI;
using TMPro;
using Tools;

namespace Modes
{
    public class FasterThanLightController : MonoBehaviour
    {
        [SerializeField] private GameObject loseLevelLabel;
        [SerializeField] private Button tryAgainButton;
        [SerializeField] private TextMeshProUGUI loseTextTMP;
        //[SerializeField] private TextMeshProUGUI timer;
        public TimeCounter timeCounter;



        [Tooltip("Добавка в секундах на каждую 1 сферу в уровне.")]
        [SerializeField] private float secondsFor1Sphere;

        [Tooltip("Добавка в секундах на каждый 1 телепорт в уровне.")]
        [SerializeField] private float secondsFor1Teleport;

        [Tooltip("Добавка в секундах на каждое 1 использование телепортов (пара вход-выход считается за 1 использование).")]
        [SerializeField] private float secondsFor1TeleportUsing;

        [Tooltip("Добавка в секундах на каждое 1 искривление луча из-за черной дыры.")]
        [SerializeField] private float secondsFor1BlackHoleUsing;

        [Tooltip("Коэффициент за темноту.")]
        [SerializeField] private float coeffForDarkness;

        [Tooltip("Пенальти в секундах за каждую следующую звезду после 1-й.")]
        [SerializeField] private float secondsPenaltyForStars;

        /// <summary>
        /// Секунд для прохождения уровня.
        /// </summary>
        //private int secondsCount;

        //private Coroutine timingCoroutine;

        //public float SecondsLeft { get; private set; }



        private void Start()
        {
            GameController.Instance.onWinLevel += StopAndInactiveTimer;
            UIController.Instance.onQuitLevel += StopAndInactiveTimer;
            UIController.Instance.onRestartLevelButtonClick += StopAndInactiveTimer;

        }

        /// <summary>
        /// Передача контроля скрипту при запуске уровня.
        /// </summary>
        /// <param name="secondsCount">Количество секунд, отведенное для прохождения уровня. Если установлено -1, то будет высчитано по формуле.</param>
        public void Initialize(int secondsCount)
        {
            //timer.color = Color.white;
            UIController.Instance.SetActiveForWinLabelNextLevelButton(GameController.Instance.IsStoryLevel);
            StartCoroutine(WaitingForLevelToBeCreated(secondsCount));
        }

        /// <summary>
        /// Ожидание создания уровня.
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitingForLevelToBeCreated(int secondsCount)
        {
            //var towerMatrix = TowersMatrix.Instance;
            var gameController = GameController.Instance;
            while (gameController.IsLevelLoading)
            {
                yield return null;
            }

            timeCounter.gameObject.SetActive(true);
            int seconds = secondsCount == -1 ? GetSecondsCount() : secondsCount;
            timeCounter.StartCounting(seconds, Color.white, LoseLevel);

            //TimerStart();
        }

        private int GetSecondsCount()
        {
            var towersMatrix = TowersMatrix.Instance;
            int spheresCount = towersMatrix.Spheres.Length;
            int teleportsCount = towersMatrix.Teleports.Length;
            int teleportsUsedCount = towersMatrix.Matrix.TeleportsUsedCount;
            int blackHolesUsedCount = towersMatrix.Matrix.BlackHolesUsedCount;
            float _darknessCoeff = GameController.Instance.IsDarkLevel ? coeffForDarkness : 1;
            float _penalty = secondsPenaltyForStars * (GameController.Instance.currentLevelStateData.StarsMax - 1);

            return (int)((spheresCount * secondsFor1Sphere +
            teleportsCount * secondsFor1Teleport +
            teleportsUsedCount * secondsFor1TeleportUsing +
            blackHolesUsedCount * secondsFor1BlackHoleUsing) *
            _darknessCoeff - _penalty);

            //Debug.Log($"spheresCount: {spheresCount}, " +
            //    $"seconds from spheresCount: {spheresCount * secondsFor1Sphere} ");
            //Debug.Log($"teleportsCount: {teleportsCount}, " +
            //$"seconds from teleportsCount: {teleportsCount * secondsFor1Teleport}");
            //Debug.Log($"teleportsUsedCount: {teleportsUsedCount}, " +
            //$"seconds from teleportsUsedCount: {teleportsUsedCount * secondsFor1TeleportUsing}");
            //Debug.Log($"blackHolesUsedCount: {blackHolesUsedCount}, " +
            //$"seconds from blackHolesUsedCount: {blackHolesUsedCount * secondsFor1BlackHoleUsing}");
            //Debug.Log($"_darknessCoeff: {_darknessCoeff}");
            //Debug.Log($"_penalty: {_penalty}");

            //Debug.Log($"secondsCount: {secondsCount}");
        }

        private void SetTimerLabelInactive()
        {
            timeCounter.gameObject.SetActive(false);
        }


        private void LoseLevel()
        {
            Debug.Log("Lose level!");
            loseLevelLabel.SetActive(true);
            tryAgainButton.gameObject.SetActive(GameController.Instance.IsStoryLevel);
            loseTextTMP.text = GameController.Instance.languageController.LightIsFasterString;
            UIController.Instance.SetInteractibleForShowCorrectPlacementButton();
            UIController.Instance.currentScreen = ActiveScreen.LoseLabel;
            SetTimerLabelInactive();
            GameController.Instance.musicController.LoseLevelSoundPlay();
            UIController.Instance.SetActiveForMinusOneStarGOOnLoseLabel(false);
        }

        ///// <summary>
        ///// Нажатие на кнопку "попробовать еще раз" при поражении в уровне на прохождение.
        ///// </summary>
        //public void TryAgainButtonClick()
        //{

        //}

        private void StopAndInactiveTimer()
        {
            timeCounter.StopCounting();
            SetTimerLabelInactive();
        }

    }
}