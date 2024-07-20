using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Saving;

namespace Achievements
{
    /// <summary>
    /// Класс, отвечающий за достижения прохождений уровней в режимах.
    /// </summary>
    public class AchivementOfModeLevelPassed : AchievementType
    {
        [SerializeField] private Achievement_Type achievementType;
        [SerializeField] private int starsRequiredToCompleteAchievement;

        public Achievement_Type Achievement_Type => achievementType;
        public int StarsRequiredToCompleteAchievement => starsRequiredToCompleteAchievement;


        /// <summary>
        /// При победе в уровне. Передается 4 параметра: параметр Achievement_Type, int количество набранных звезд, int максимальное за этот уровень количество звезд,
        /// и bool загрузка из сохранения или нет.
        /// </summary>
        /// <param name="vs"></param>
        public override void EventHappend(params object[] vs)
        {
            //Debug.Log($"levelType: {GameController.Instance.currentLevelStateData.levelType}");
            Achievement_Type _achievementType = (Achievement_Type)vs[0];
            int starsScored = (int)vs[1];
            int starsMax = (int)vs[2];
            bool isLoading = (bool)vs[3];

            //Debug.Log($"_achievementType: {_achievementType}, starsScored: {starsScored}, starsMax: {starsMax}");


            if (_achievementType == achievementType && starsMax == starsRequiredToCompleteAchievement)
            {
                float newProgressBarValue = (float)starsScored / starsMax;
                //Debug.Log($"newProgressBarValue: {newProgressBarValue}, old progressBar.value: {progressBar.value}");
                if (newProgressBarValue > progressBar.value)
                {
                    progressBar.value = newProgressBarValue;
                    progressPercentageText.text = $"{Mathf.FloorToInt(progressBar.value * 100)} %";
                    //Debug.Log($"progressBar.value: {progressBar.value}, progressBar.value * 100: {progressBar.value * 100}, (int)(progressBar.value * 100): {Mathf.FloorToInt(progressBar.value * 100)}");

                    if (!isLoading)
                    {
                        var state = GameController.Instance.StateData.GetAchievementSaveDataOrAddNew(achievementType, starsRequiredToCompleteAchievement);
                        state.mainResult = starsScored;
                        GameController.Instance.SaveState();
                    }

                    if (starsScored == starsMax)
                    {
                        if(!isLoading)
                        {
                            Debug.Log($"Achievement is recieved! achievementType: {achievementType}, starsRequiredToCompleteAchievement: {starsRequiredToCompleteAchievement}");

                            //Появление объявления о получении достижения.
                            AchievementRecieved();

                        }

                        var images = medal.GetComponentsInChildren<Image>();
                        for (int i = 0; i < images.Length; i++)
                        {
                            images[i].color = Color.white;
                        }
                            
                        medalActivated.Invoke(isLoading);
                    }
                }
            }
        }
    }
}