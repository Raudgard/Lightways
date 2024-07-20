using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Achievements
{
    /// <summary>
    ///  ласс, отвечающий за достижени€ пройденных уровней в режиме прохождени€.
    /// </summary>
    public class AchivementOfLevelsPassed : AchievementType
    {
        [SerializeField] private Difficulty difficulty;
        [SerializeField] private bool isAchievementForAllLevels;
        [SerializeField] private int requiredToCompleteAchievement;


        

        /// <summary>
        /// ѕри победе в уровне. ѕередаетс€ 2 параметра: difficulty и bool загрузка из сохранени€ или нет.
        /// </summary>
        /// <param name="vs"></param>
        public override void EventHappend(params object[] vs)
        {
            //Debug.Log($"levelType: {GameController.Instance.currentLevelStateData.levelType}");

            if (GameController.Instance.currentLevelStateData.levelType == Saving.LevelType.PathOfLight)
            {
                int levelsPassed;
                var _difficulty = (Difficulty)vs[0];
                bool isLoading = (bool)vs[1];

                if (isAchievementForAllLevels)
                {
                    //Debug.Log($"for all levels.");
                    levelsPassed = GameController.Instance.StateData.levelStateDatas.Where(level => level.StarsRecieved > 0).Count();
                }
                else
                {
                    //Debug.Log($"currentDifficulty: {_difficulty}");
                    levelsPassed = GameController.Instance.StateData.levelStateDatas.Where(level => level.difficulty == _difficulty && level.StarsRecieved > 0).Count();
                }

                if (_difficulty == difficulty || isAchievementForAllLevels)
                {
                    float newProgressBarValue = (float)levelsPassed / requiredToCompleteAchievement;
                    //Debug.Log($"newProgressBarValue: {newProgressBarValue}, old progressBar.value: {progressBar.value}");

                    if (newProgressBarValue > progressBar.value && progressBar.value < 1)
                    {
                        //Debug.Log($"levelsPassed: {levelsPassed}");
                        progressBar.value = newProgressBarValue;
                        progressPercentageText.text = $"{Mathf.FloorToInt(progressBar.value * 100)} %";
                        //Debug.Log($"progressBar.value: {progressBar.value}, progressBar.value * 100: {progressBar.value * 100}, (int)(progressBar.value * 100): {Mathf.FloorToInt(progressBar.value * 100)}");

                        if (levelsPassed >= requiredToCompleteAchievement)
                        {
                            if (!isLoading)
                            {
                                Debug.Log($"Achievement is recieved! AchivementOfLevelsPassed. isAchievementForAllLevels: {isAchievementForAllLevels}, difficulty: {difficulty}.");

                                //ѕо€вление объ€влени€ о получении достижени€.
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
}