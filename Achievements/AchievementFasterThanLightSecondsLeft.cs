using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Saving;

namespace Achievements
{
    /// <summary>
    /// Класс, отвечающий за достижения количества оставшихся секунд при игре в режиме "Faster Than Light".
    /// </summary>
    public class AchievementFasterThanLightSecondsLeft : AchievementType
    {
        [SerializeField] private Achievement_Type achievementType;
        [SerializeField] private int starsMaxRequiredToCompleteAchievement;
        [SerializeField] private int secondsLeftRequiredToCompleteAchievement;


        public Achievement_Type Achievement_Type => achievementType;
        public int SecondsLeftRequiredToCompleteAchievement => secondsLeftRequiredToCompleteAchievement;

        public int StarsMaxRequiredToCompleteAchievement => starsMaxRequiredToCompleteAchievement;


        /// <summary>
        /// При победе в уровне. Передается 4 параметра: параметр Achievement_Type, int количество звезд за этот уровень, int оставшихся секунд,
        /// и bool загрузка из сохранения или нет.
        /// </summary>
        /// <param name="vs"></param>
        public override void EventHappend(params object[] vs)
        {
            //Debug.Log($"levelType: {GameController.Instance.currentLevelStateData.levelType}");
            Achievement_Type _achievementType = (Achievement_Type)vs[0];
            int stars = (int)vs[1];
            int secondsLeft = (int)vs[2];
            bool isLoading = (bool)vs[3];

            //Debug.Log($"_achievementType: {_achievementType}, stars: {stars}, secondsLeft: {secondsLeft}, isLoading: {isLoading}");


            if (_achievementType == achievementType && stars == starsMaxRequiredToCompleteAchievement)
            {
                float newProgressBarValue = (float)secondsLeft / secondsLeftRequiredToCompleteAchievement;
                //Debug.Log($"newProgressBarValue: {newProgressBarValue}, old progressBar.value: {progressBar.value}");
                if (newProgressBarValue > progressBar.value && progressBar.value < 1)
                {
                    progressBar.value = newProgressBarValue;
                    progressPercentageText.text = $"{Mathf.FloorToInt(progressBar.value * 100)} %";
                    //Debug.Log($"progressBar.value: {progressBar.value}, progressBar.value * 100: {progressBar.value * 100}, (int)(progressBar.value * 100): {Mathf.FloorToInt(progressBar.value * 100)}");

                    if (!isLoading)
                    {
                        var state = GameController.Instance.StateData.GetAchievementSaveDataOrAddNew(achievementType, starsMaxRequiredToCompleteAchievement);
                        state.mainResult = secondsLeft;
                        GameController.Instance.SaveState();
                    }

                    if (secondsLeft >= secondsLeftRequiredToCompleteAchievement)
                    {
                        if (!isLoading)
                        {
                            Debug.Log($"Achievement is recieved! achievementType: {achievementType}, starsMaxRequiredToCompleteAchievement: {starsMaxRequiredToCompleteAchievement}, secondsLeft: {secondsLeft}");

                            //Появление объявления о получении достижения.
                            AchievementRecieved();

                        }

                        var images = medal.GetComponentsInChildren<Image>();
                        for (int i = 0; i < images.Length; i++)
                        {
                            images[i].color = Color.white;
                        }
                        medal.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                        medalActivated.Invoke(isLoading);
                    }
                }
            }
        }
    }
}