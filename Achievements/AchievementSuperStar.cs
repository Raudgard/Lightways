using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Saving;

namespace Achievements
{
    /// <summary>
    /// Класс, отвечающий за достижениe "Супер звезда!".
    /// </summary>
    public class AchievementSuperStar : AchievementType
    {
        [SerializeField] private Achievement_Type achievementType;

        public Achievement_Type Achievement_Type => achievementType;


        /// <summary>
        /// При изменении шкалы в любом достижении (и при загрузке). Передается 2 параметра: [AchievementType] массив всех достижений,
        /// и bool загрузка из сохранения или нет.
        /// </summary>
        /// <param name="vs"></param>
        public override void EventHappend(params object[] vs)
        {
            var achievements = (AchievementType[])vs[0]; //все достижения.
            bool isLoading = (bool)vs[1];



            int achivementsRecieved = achievements.Where(ach => ach.IsReceived).Count();
            int achievementsCount = achievements.Length;
            //Debug.Log($"gameObject.name: {gameObject.name}, achievementType: {achievementType}, achivementsRecieved: {achivementsRecieved}, achievementsCount: {achievementsCount}");

            float newProgressBarValue = (float)achivementsRecieved / achievementsCount;
            //Debug.Log($"newProgressBarValue: {newProgressBarValue}, old progressBar.value: {progressBar.value}");
            if (newProgressBarValue > progressBar.value)
            {
                progressBar.value = newProgressBarValue;
                progressPercentageText.text = $"{Mathf.FloorToInt(progressBar.value * 100)} %";
                //Debug.Log($"progressBar.value: {progressBar.value}, progressBar.value * 100: {progressBar.value * 100}, (int)(progressBar.value * 100): {Mathf.FloorToInt(progressBar.value * 100)}");

                if (achivementsRecieved >= achievementsCount)
                {
                    if (!isLoading)
                    {
                        Debug.Log($"Achievement is recieved! achievementType: {achievementType}");

                        var state = GameController.Instance.StateData.GetAchievementSaveDataOrAddNew(achievementType, achievementsCount);
                        state.mainResult = achivementsRecieved;
                        GameController.Instance.SaveState();

                        //Появление объявления о получении достижения.
                        AchievementRecieved();

                    }

                    var images = medal.GetComponentsInChildren<Image>();
                    for (int i = 0; i < images.Length; i++)
                    {
                        images[i].color = Color.white;
                    }
                }
            }
        }
    }
}