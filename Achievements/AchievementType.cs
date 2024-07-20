using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Achievements
{
    /// <summary>
    /// Класс, интерпретирующий вид достижений.
    /// </summary>
    public abstract class AchievementType : MonoBehaviour
    {
        [SerializeField] protected Image medal;
        [SerializeField] protected Slider progressBar;
        [SerializeField] protected TextMeshProUGUI progressPercentageText;

        /// <summary>
        /// Получено достижение или нет. Возвращает всего лишь  progressBar.value >= 1.
        /// </summary>
        public bool IsReceived => progressBar.value >= 1;

        /// <summary>
        /// При активации медали достижения. bool - при загрузке или нет.
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
        /// Происходит при первом получении достижения (не при загрузке).
        /// </summary>
        protected void AchievementRecieved()
        {
            GameController.Instance.achievementsController.OnAchievementRecieved(gameObject);
        }

    }

    
    


    /// <summary>
    /// Перечисление всех видов достижений.
    /// </summary>
    public enum Achievement_Type
    {
        /// <summary>
        /// Количество звезд у игрока.
        /// </summary>
        StarsCount = 0,
        /// <summary>
        /// Прохождение уровней в сложностях.
        /// </summary>
        LightwaysPass = 1,
        /// <summary>
        /// Прохождение режима "Случайные фотоны" на количество звезд.
        /// </summary>
        RandomPhotonsPass = 2,
        /// <summary>
        /// Прохождение режима "Быстрее света" на количество звезд.
        /// </summary>
        FasterThanLightPass = 3,
        /// <summary>
        /// Прохождение режима "Загадки черных дыр" на количество звезд.
        /// </summary>
        BlackHolesRiddlesPass = 4,
        /// <summary>
        /// Количество оставшихся секунд в режиме "Быстрее света".
        /// </summary>
        FasterThanLightSecondsLeft = 5,
        /// <summary>
        /// Достижение "Обходной путь".
        /// </summary>
        RoundaboutWay = 6,
        /// <summary>
        /// Достижение "Супер искривление".
        /// </summary>
        SuperCurvature = 7,
        /// <summary>
        /// Достижение "Супер звезда". Достижение всех достижений.
        /// </summary>
        SuperStar = 8
    }
}