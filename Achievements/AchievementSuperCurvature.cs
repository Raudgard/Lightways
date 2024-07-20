using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Saving;

namespace Achievements
{
    /// <summary>
    /// Класс, отвечающий за достижениe "Супер искривление".
    /// </summary>
    public class AchievementSuperCurvature : AchievementType
    {
        [SerializeField] private Achievement_Type achievementType;
        [SerializeField] private int curvaturesRequiredToCompleteAchievement;

        public Achievement_Type Achievement_Type => achievementType;
        public int CurvaturesRequiredToCompleteAchievement => curvaturesRequiredToCompleteAchievement;



        /// <summary>
        /// При отправке луча. Передается 2 параметра: int количество искривлений луча
        /// и bool загрузка из сохранения или нет.
        /// </summary>
        /// <param name="vs"></param>
        public override void EventHappend(params object[] vs)
        {
            int curvatures = (int)vs[0];
            bool isLoading = (bool)vs[1];

            //Debug.Log($"curvatures: {curvatures}, isLoading: {isLoading}");

            float newProgressBarValue = (float)curvatures / curvaturesRequiredToCompleteAchievement;
            //Debug.Log($"newProgressBarValue: {newProgressBarValue}, old progressBar.value: {progressBar.value}");
            if (newProgressBarValue > progressBar.value && progressBar.value < 1)
            {
                progressBar.value = newProgressBarValue;
                progressPercentageText.text = $"{Mathf.FloorToInt(progressBar.value * 100)} %";
                //Debug.Log($"progressBar.value: {progressBar.value}, progressBar.value * 100: {progressBar.value * 100}, (int)(progressBar.value * 100): {Mathf.FloorToInt(progressBar.value * 100)}");

                if (!isLoading)
                {
                    var state = GameController.Instance.StateData.GetAchievementSaveDataOrAddNew(achievementType, curvaturesRequiredToCompleteAchievement);
                    state.mainResult = curvatures;
                    GameController.Instance.SaveState();
                }

                if (curvatures >= curvaturesRequiredToCompleteAchievement)
                {
                    if (!isLoading)
                    {
                        Debug.Log($"Achievement is recieved! achievementType: {achievementType}, curvaturesRequiredToCompleteAchievement: {curvaturesRequiredToCompleteAchievement}, curvatures: {curvatures}");

                        //Появление объявления о получении достижения.
                        AchievementRecieved();

                    }

                    var images = medal.GetComponentsInChildren<Image>();
                    for (int i = 0; i < images.Length; i++)
                    {
                        images[i].color = Color.white;
                    }

                    var Tmpro = medal.GetComponentInChildren<TextMeshProUGUI>();
                    if (Tmpro != null)
                    {
                        Tmpro.color = Color.white;
                    }
                    medalActivated.Invoke(isLoading);
                }
            }
        }
    }
}