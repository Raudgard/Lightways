using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Achievements
{
    /// <summary>
    /// �����, ���������������� ��� ����������.
    /// </summary>
    public abstract class AchievementType : MonoBehaviour
    {
        [SerializeField] protected Image medal;
        [SerializeField] protected Slider progressBar;
        [SerializeField] protected TextMeshProUGUI progressPercentageText;

        /// <summary>
        /// �������� ���������� ��� ���. ���������� ����� ����  progressBar.value >= 1.
        /// </summary>
        public bool IsReceived => progressBar.value >= 1;

        /// <summary>
        /// ��� ��������� ������ ����������. bool - ��� �������� ��� ���.
        /// </summary>
        public Action<bool> medalActivated;


        private void Start()
        {
            DisableRaycastTargets();
        }


        private void DisableRaycastTargets()
        {
            var graphics = GetComponentsInChildren<Graphic>(true);
            for (int i = 0; i < graphics.Length; i++)
            {
                graphics[i].raycastTarget = false;
            }
        }


        public abstract void EventHappend(params object[] vs);

        /// <summary>
        /// ���������� ��� ������ ��������� ���������� (�� ��� ��������).
        /// </summary>
        protected void AchievementRecieved()
        {
            GameController.Instance.achievementsController.OnAchievementRecieved(gameObject);
        }

    }

    
    


    /// <summary>
    /// ������������ ���� ����� ����������.
    /// </summary>
    public enum Achievement_Type
    {
        /// <summary>
        /// ���������� ����� � ������.
        /// </summary>
        StarsCount = 0,
        /// <summary>
        /// ����������� ������� � ����������.
        /// </summary>
        LightwaysPass = 1,
        /// <summary>
        /// ����������� ������ "��������� ������" �� ���������� �����.
        /// </summary>
        RandomPhotonsPass = 2,
        /// <summary>
        /// ����������� ������ "������� �����" �� ���������� �����.
        /// </summary>
        FasterThanLightPass = 3,
        /// <summary>
        /// ����������� ������ "������� ������ ���" �� ���������� �����.
        /// </summary>
        BlackHolesRiddlesPass = 4,
        /// <summary>
        /// ���������� ���������� ������ � ������ "������� �����".
        /// </summary>
        FasterThanLightSecondsLeft = 5,
        /// <summary>
        /// ���������� "�������� ����".
        /// </summary>
        RoundaboutWay = 6,
        /// <summary>
        /// ���������� "����� �����������".
        /// </summary>
        SuperCurvature = 7,
        /// <summary>
        /// ���������� "����� ������". ���������� ���� ����������.
        /// </summary>
        SuperStar = 8
    }
}