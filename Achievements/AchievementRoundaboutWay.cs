using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Saving;

namespace Achievements
{
    /// <summary>
    /// �����, ���������� �� ���������e "�������� ����".
    /// </summary>
    public class AchievementRoundaboutWay : AchievementType
    {
        [SerializeField] private Achievement_Type achievementType;

        public Achievement_Type Achievement_Type => achievementType;


        /// <summary>
        /// ��� ������ � ������. ���������� 4 ���������: �������� Achievement_Type, int ���������� ��������� ����, int ����� ���������� ���� � ������,
        /// � bool �������� �� ���������� ��� ���.
        /// </summary>
        /// <param name="vs"></param>
        public override void EventHappend(params object[] vs)
        {
            //Debug.Log($"levelType: {GameController.Instance.currentLevelStateData.levelType}");
            Achievement_Type _achievementType = (Achievement_Type)vs[0];
            int spheresLighted = (int)vs[1]; //���������� ��������� ����.
            int spheresNumber = (int)vs[2]; //����� ���������� ���� � ������.
            bool isLoading = (bool)vs[3];

            //Debug.Log($"gameObject.name: {gameObject.name}, _achievementType: {_achievementType}, spheresLighted: {spheresLighted}, spheresNumber: {spheresNumber}");


            if (_achievementType == achievementType && spheresLighted < spheresNumber)
            {
                float newProgressBarValue = 1;
                //Debug.Log($"newProgressBarValue: {newProgressBarValue}, old progressBar.value: {progressBar.value}");
                if (newProgressBarValue > progressBar.value)
                {
                    progressBar.value = newProgressBarValue;
                    progressPercentageText.text = $"{Mathf.FloorToInt(progressBar.value * 100)} %";
                    //Debug.Log($"progressBar.value: {progressBar.value}, progressBar.value * 100: {progressBar.value * 100}, (int)(progressBar.value * 100): {Mathf.FloorToInt(progressBar.value * 100)}");

                    if (!isLoading)
                    {
                        Debug.Log($"Achievement is recieved! achievementType: {achievementType}");
                        
                        var state = GameController.Instance.StateData.GetAchievementSaveDataOrAddNew(achievementType, 1);
                        state.mainResult = 1;
                        GameController.Instance.SaveState();

                        //��������� ���������� � ��������� ����������.
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