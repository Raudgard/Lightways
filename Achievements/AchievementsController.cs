using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

namespace Achievements
{
    /// <summary>
    /// Класс, отвечающий за отслеживание и публикацию достижений.
    /// </summary>
    public class AchievementsController : MonoBehaviour
    {
        [SerializeField] private Modes.FasterThanLightController fasterThanLightController;

        [Space]

        [SerializeField] private AchivementOfStarNumber[] achivementsOfStarNumber;
        [SerializeField] private AchivementOfLevelsPassed[] achivementOfLevelsPassed;
        [SerializeField] private AchivementOfModeLevelPassed[] achivementOfModeLevelPassed;
        [SerializeField] private AchievementFasterThanLightSecondsLeft[] achievementFasterThanLightSecondsLeft;
        [SerializeField] private AchievementRoundaboutWay achievementRoundaboutWay;
        [SerializeField] private AchievementSuperCurvature achievementSuperCurvature;
        [SerializeField] private AchievementSuperStar achievementSuperStar;

        [Space]

        [SerializeField] private Image labelImage;
        //[SerializeField] private Image labelBorderImage;
        [SerializeField] private GameObject medalPlace;
        [SerializeField] private TextMeshProUGUI congratulationsHeaderText;
        [SerializeField] private TextMeshProUGUI congratulationsText;

        [SerializeField] private TextMeshProUGUI achievementName;

        [Space]

        [SerializeField] private float speedOfAppearing;
        [SerializeField] private float pauseBeforeDisappearing;

        private Coroutine showLabelCoroutine = null;


        IEnumerator Start()
        {
            yield return null;
            yield return null;
            yield return null;

            GameController.Instance.onWinLevel += OnWinLevel;


            while (!GameController.Instance.isStateLoaded)
            {
                yield return null;
            }


            for (int i = 0; i < achivementsOfStarNumber.Length; i++)
            {
                achivementsOfStarNumber[i].medalActivated += OnAchievementMedalActivated;
                achivementsOfStarNumber[i].EventHappend(GameController.Instance.StarsCount, true);
            }

            for (int i = 0; i < achivementOfLevelsPassed.Length; i++)
            {
                achivementOfLevelsPassed[i].medalActivated += OnAchievementMedalActivated;
                achivementOfLevelsPassed[i].EventHappend((Difficulty)i, true);
            }

            for (int i = 0; i < achivementOfModeLevelPassed.Length; i++)
            {
                achivementOfModeLevelPassed[i].medalActivated += OnAchievementMedalActivated;
                var stateData = GameController.Instance.StateData.GetAchievementSaveDataOrAddNew(achivementOfModeLevelPassed[i].Achievement_Type, achivementOfModeLevelPassed[i].StarsRequiredToCompleteAchievement);
                achivementOfModeLevelPassed[i].EventHappend(stateData.achievementType, stateData.mainResult, stateData.resultRequiredToCompleteAchievement, true);
            }

            for (int i = 0; i < achievementFasterThanLightSecondsLeft.Length; i++)
            {
                achievementFasterThanLightSecondsLeft[i].medalActivated += OnAchievementMedalActivated;
                var stateData = GameController.Instance.StateData.GetAchievementSaveDataOrAddNew(achievementFasterThanLightSecondsLeft[i].Achievement_Type, achievementFasterThanLightSecondsLeft[i].StarsMaxRequiredToCompleteAchievement);
                achievementFasterThanLightSecondsLeft[i].EventHappend(stateData.achievementType, stateData.resultRequiredToCompleteAchievement, stateData.mainResult, true);
            }

            achievementRoundaboutWay.medalActivated += OnAchievementMedalActivated;

            var stateDataOfARAW = GameController.Instance.StateData.GetAchievementSaveDataOrAddNew(achievementRoundaboutWay.Achievement_Type, 1);
            if (stateDataOfARAW.mainResult == 1)
            {
                achievementRoundaboutWay.EventHappend(stateDataOfARAW.achievementType, 1, 2, true);
            }
            else
            {
                achievementRoundaboutWay.EventHappend(stateDataOfARAW.achievementType, 1, 1, true);
            }

            achievementSuperCurvature.medalActivated += OnAchievementMedalActivated;
            var stateDataOfASC = GameController.Instance.StateData.GetAchievementSaveDataOrAddNew(achievementSuperCurvature.Achievement_Type, achievementSuperCurvature.CurvaturesRequiredToCompleteAchievement);
            achievementSuperCurvature.EventHappend(stateDataOfASC.mainResult, true);



            //OnAchievementRecieved(achievementSuperStar.gameObject);

        }


        private void OnWinLevel()
        {
            var currentLevelStateData = GameController.Instance.currentLevelStateData;

            for (int i = 0; i < achivementsOfStarNumber.Length; i++)
            {
                achivementsOfStarNumber[i].EventHappend(GameController.Instance.StarsCount, false);
            }

            for (int i = 0; i < achivementOfLevelsPassed.Length; i++)
            {
                achivementOfLevelsPassed[i].EventHappend(GameController.Instance.currentDifficulty, false);
            }

            for (int i = 0; i < achivementOfModeLevelPassed.Length; i++)
            {
                //перечисление Achievement_Type отличается от LevelType на 1. Поэтому плюсую 1.
                achivementOfModeLevelPassed[i].EventHappend(currentLevelStateData.levelType + 1, currentLevelStateData.StarsRecieved, currentLevelStateData.StarsMax, false);
            }

            if (currentLevelStateData.levelType == Saving.LevelType.FasterThanLight)
            {
                achievementRoundaboutWay.EventHappend(Achievement_Type.RoundaboutWay, GameController.Instance.SpheresHighlighted.Count, Matrix.Instance.TowersCount, false);

                var achievement = achievementFasterThanLightSecondsLeft.Where(ach => ach.StarsMaxRequiredToCompleteAchievement == currentLevelStateData.StarsMax).FirstOrDefault();
                achievement.EventHappend(Achievement_Type.FasterThanLightSecondsLeft, currentLevelStateData.StarsMax, Mathf.CeilToInt(fasterThanLightController.timeCounter.currentSeconds), false);
                
                //Debug.Log($"achievement.EventHappend. currentLevelStateData.StarsMax:{currentLevelStateData.StarsMax}, SecondsLeft: {fasterThanLightController.timeCounter.currentSeconds}");
            }

        }


        public void OnBeamSended(int curvaturesNumber)
        {
            if (GameController.Instance.IsCreateLevelMode)
                return;

            achievementSuperCurvature.EventHappend(curvaturesNumber, false);
        }


        private void OnAchievementMedalActivated(bool onLoading)
        {
            var achievements = achivementsOfStarNumber.
                Concat<AchievementType>(achivementOfLevelsPassed).
                Concat(achivementOfModeLevelPassed).
                Concat(achievementFasterThanLightSecondsLeft).
                Append(achievementRoundaboutWay).
                Append(achievementSuperCurvature).
                ToArray();

            achievementSuperStar.EventHappend(achievements, onLoading);

        }

        /// <summary>
        /// При первом получении достижения (не при загрузке). Для появления поздравления.
        /// </summary>
        /// <param name="gameObject">Объект достижения на канвасе.</param>
        public void OnAchievementRecieved(GameObject gameObject)
        {
            StartCoroutine(BeginShowLabels(gameObject));
        }

        /// <summary>
        /// Запускает показ лейбла в то случае, если лейбл не отображается в данный момент, иначе ждет.
        /// Необходимо для показа нескольких лейблов подряд в случае одновременного получения немкольких достижений.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private IEnumerator BeginShowLabels(GameObject gameObject)
        {
            while (showLabelCoroutine != null)
            {
                yield return null;
            }
            
            showLabelCoroutine = StartCoroutine(ShowAchievementRecievedLabel(gameObject));
        }

        /// <summary>
        /// Плавное отображение одного лейбла с поздравлениями.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        private IEnumerator ShowAchievementRecievedLabel(GameObject gameObject)
        {
            labelImage.gameObject.SetActive(true);
            string _achievementName = gameObject.GetComponentsInChildren<TextMeshProUGUI>().Where(go => go.CompareTag("Achievement name")).FirstOrDefault().text;
            Image medal = gameObject.GetComponentsInChildren<Image>().Where(go => go.CompareTag("Medal Image")).FirstOrDefault();

            Debug.Log($"achievementName: {_achievementName}");

            achievementName.text = _achievementName;

            var oldMedalObjects = medalPlace.GetComponentsInChildren<Component>(true);
            for (int i = 1; i < oldMedalObjects.Length; i++)
            {
                Destroy(oldMedalObjects[i].gameObject);
            }


            var newMedal = Instantiate(medal, medalPlace.transform);
            var rectTransformOfNewMedal = newMedal.GetComponent<RectTransform>();
            rectTransformOfNewMedal.anchoredPosition = Vector2.zero;


            yield return null;

            Color colorTransparent = new Color(1, 1, 1, 0);

            var medalImages = medalPlace.GetComponentsInChildren<Image>(true);
            var medalTexts = medalPlace.GetComponentsInChildren<TextMeshProUGUI>(true);

            float t = 0;


            while (t < 1)
            {
                t += Time.deltaTime * speedOfAppearing;



                labelImage.color = Color.Lerp(colorTransparent, Color.white, t);
                //labelBorderImage.color = Color.Lerp(colorTransparent, Color.white, t);
                congratulationsHeaderText.color = Color.Lerp(colorTransparent, Color.white, t);
                congratulationsText.color = Color.Lerp(colorTransparent, Color.white, t);
                achievementName.color = Color.Lerp(colorTransparent, Color.white, t);

                foreach (var mi in medalImages)
                {
                    mi.color = Color.Lerp(colorTransparent, Color.white, t);
                }

                foreach (var mt in medalTexts)
                {
                    mt.color = Color.Lerp(colorTransparent, Color.white, t);
                }

                yield return null;
            }

            labelImage.color = Color.white;
            //labelBorderImage.color = Color.white;
            congratulationsHeaderText.color = Color.white;
            congratulationsText.color = Color.white;
            achievementName.color = Color.white;

            foreach (var mi in medalImages)
            {
                mi.color = Color.white;
            }

            foreach (var mt in medalTexts)
            {
                mt.color = Color.white;
            }

            yield return new WaitForSeconds(pauseBeforeDisappearing);

            t = 1;

            while (t > 0)
            {
                t -= Time.deltaTime * speedOfAppearing;

                labelImage.color = Color.Lerp(colorTransparent, Color.white, t);
                //labelBorderImage.color = Color.Lerp(colorTransparent, Color.white, t);
                congratulationsHeaderText.color = Color.Lerp(colorTransparent, Color.white, t);
                congratulationsText.color = Color.Lerp(colorTransparent, Color.white, t);
                achievementName.color = Color.Lerp(colorTransparent, Color.white, t);

                foreach (var mi in medalImages)
                {
                    mi.color = Color.Lerp(colorTransparent, Color.white, t);
                }

                foreach (var mt in medalTexts)
                {
                    mt.color = Color.Lerp(colorTransparent, Color.white, t);
                }

                yield return null;
            }

            labelImage.gameObject.SetActive(false);

            showLabelCoroutine = null;
        }
    }
}