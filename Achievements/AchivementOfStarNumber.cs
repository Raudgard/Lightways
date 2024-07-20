using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Achievements
{
    /// <summary>
    /// �����, ���������� �� ���������� �� ���������� ��������� �����.
    /// </summary>
    public class AchivementOfStarNumber : AchievementType
    {
        [SerializeField] private int requiredToCompleteAchievement;

        /// <summary>
        /// ��� ������ � ������. ���������� 2 ���������: int �������� � ���������� �� ������ ������ ����� ����������� �����, � bool - �������� �� ���������� ��� ���.
        /// </summary>
        /// <param name="vs"></param>
        public override void EventHappend(params object[] vs)
        {
            int starsCount = (int)vs[0];
            bool isLoading = (bool)vs[1];
            //Debug.Log($"starsCount: {starsCount}");
            float newProgressBarValue = (float)starsCount / requiredToCompleteAchievement;
            //Debug.Log($"newProgressBarValue: {newProgressBarValue}, old progressBar.value: {progressBar.value}");

            if (newProgressBarValue > progressBar.value && progressBar.value < 1)
            {
                progressBar.value = newProgressBarValue;
                progressPercentageText.text = $"{Mathf.FloorToInt(progressBar.value * 100)} %";

                if (starsCount >= requiredToCompleteAchievement)
                {
                    if (!isLoading)
                    {
                        Debug.Log($"Achievement is recieved");
                        //��������� ���������� � ��������� ����������.
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