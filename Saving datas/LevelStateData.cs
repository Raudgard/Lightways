using System;
using UnityEngine;
namespace Saving
{ 
    public enum LevelType
    {
        PathOfLight = 0,
        RandomPhotons = 1,
        FasterThanLight = 2,
        BlackHolesRiddles = 3
    }



    /// <summary>
    /// Состояние прохождения игроком конкретного уровня.
    /// </summary>
    [Serializable]
    public class LevelStateData
    {
        public LevelType levelType;

        public Difficulty difficulty;

        public int levelNumber;

        private int starsRecieved;

        /// <summary>
        /// Получено звезд за этот уровень. Можно только увеличить количество полученных звезд. Уменьшить нельзя.
        /// </summary>
        public int StarsRecieved
        {
            get { return starsRecieved; }
            set
            {
                if (value > starsRecieved)
                    starsRecieved = value;
            }
        }

        private int starsMax;
        /// <summary>
        /// Максимум звезд можно получить за этот уровень.
        /// </summary>
        public int StarsMax { 
            get
            {
                if (levelType == LevelType.PathOfLight)
                    return StarsMaxForStoryLevel();
                else return starsMax;
            }
            private set { starsMax = value; } 
        }

        /// <summary>
        /// Максимум звезд можно получить за эту попытку. Может быть меньше starsMax, если игрок уже запорол прохождение на максисмум, а время сброса еще не прошло.
        /// </summary>
        public int starsMaxOnThisSession;

        /// <summary>
        /// Пропущен ли уровень.
        /// </summary>
        public bool isSkipped;

        /// <summary>
        /// Зафиксированный момент времени, когда игрок получил не максимум звезд за уровень.
        /// </summary>
        public DateTime incompleteTime;

        ///// <summary>
        ///// Конструктор только для типа уровня PathOfLight.
        ///// </summary>
        ///// <param name="difficulty"></param>
        ///// <param name="levelNumber"></param>
        //public LevelStateData(Difficulty difficulty, int levelNumber)
        //{
        //    this.levelType = LevelType.PathOfLight;
        //    StarsRecieved = 0;
        //    isSkipped = false;
        //    this.difficulty = difficulty;
        //    this.levelNumber = levelNumber;

        //    StarsMax = StarsMaxForPathOfLight();

        //    starsMaxOnThisSession = StarsMax;
        //    //Debug.Log($"difficulty: {difficulty}, level: {level}, StarsMax: {StarsMax}");
        //}

        ///// <summary>
        ///// Конструктор для кастомных уровней, которые не будут сохранены.
        ///// </summary>
        ///// <param name="levelType"></param>
        //public LevelStateData(LevelType levelType, int starsMax)
        //{
        //    this.levelType = levelType;
        //    StarsMax = starsMax;
        //    starsMaxOnThisSession = StarsMax;
        //    StarsRecieved = 0;
        //    isSkipped = false;
        //    levelNumber = 0;
        //    difficulty = Difficulty.Very_light;
        //}


        public LevelStateData(LevelType levelType, Difficulty difficulty, int levelNumber, int starsMax)
        {
            this.levelType = levelType;
            this.difficulty = difficulty;
            this.levelNumber = levelNumber;
            if (starsMax == 0)
            {
                StarsMax = StarsMaxForStoryLevel();
            }
            else
            {
                StarsMax = starsMax;
            }

            StarsRecieved = 0;
            isSkipped = false;
            starsMaxOnThisSession = StarsMax;
        }


        /// <summary>
        /// Проверяет вышло ли штрафное время. Если вышло, обнуляет переменные.
        /// </summary>
        public bool HasPenaltyTimeExpired()
        {
            //var time = DateTime.Now - incompleteTime;
            //Debug.Log($"time: {time}, penalty time: {TimeSpan.FromMinutes(GameController.Instance.Settings.penaltyTimeInMinutes)}");
            if (GameController.Instance.IsGamePurchased || DateTime.Now - incompleteTime > TimeSpan.FromMinutes(GameController.Instance.Settings.penaltyTimeInMinutes))
            {
                starsMaxOnThisSession = StarsMax;
                return true;
            }
            else return false;
        }


        private int StarsMaxForStoryLevel()
        {
            return (int)difficulty + 1;
            
            //return difficulty switch
            //{
            //    Difficulty.Very_light => 1,
            //    Difficulty.Light => levelNumber < 101 ? 1 : 2,
            //    Difficulty.Light_shadow => levelNumber < 101 ? 2 : 3,
            //    Difficulty.Dark => levelNumber < 101 ? 3 : 4,
            //    Difficulty.Darkness => levelNumber < 101 ? 4 : 5,
            //    _ => throw new NotImplementedException()
            //};
        }



        /// <summary>
        /// Стирает данные о прохождении. Как будто в уровень не играли.
        /// </summary>
        public void ClearData()
        {
            StarsRecieved = 0;
            isSkipped = false;
            starsMaxOnThisSession = StarsMax;
            incompleteTime = DateTime.MinValue;
        }



        public override string ToString()
        {
            return $"levelType: {levelType}, " +
                    $"difficulty: {difficulty}, " +
                    $"levelNumber: {levelNumber}, " +
                    $"StarsRecieved: {StarsRecieved}, " +
                    $"StarsMax: {StarsMax}, " +
                    $"isSkipped: {isSkipped}, " +
                    $"HasPenaltyTimeExpired: {HasPenaltyTimeExpired()}";
        }

    }
}