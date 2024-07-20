using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Achievements;
using System.Security.Cryptography;
using System.Text;

namespace Saving
{
    /// <summary>
    /// Данные о состоянии прохождения игры игроком.
    /// </summary>
    [Serializable]
    public class StateData
    {
        private CryptedData stars;
        private CryptedData gamePurchased;
        private CryptedData keysForModes;
        private CryptedData userId;

        public int levelsSkipped;
        /// <summary>
        /// Время последнего прибавления ключей.
        /// </summary>
        public DateTime timeLastAddingKeys;

        /// <summary>
        /// Время следующего прибавления ключей.
        /// </summary>
        public DateTime timeNextAddingKeys;


        /// <summary>
        /// Получает и сохраняет количество звезд. С шифрованием. Разрешает увеличение звезд не более, чем на 5.
        /// </summary>
        public int StarsCount
        {
            get
            {
                var starsString = Crypto.Decrypt(stars.data, stars.key, stars.IV);
                if (int.TryParse(starsString, out int res))
                    return res;
                else throw new Exception("Can't get stars from encrypted file.");
            }

            set
            {
                //Debug.Log($"value: {value}");
                if (value - StarsCount > 5)
                {
                    throw new Exception("Attempt to increase the stars by more than 5.");
                }

                stars.data = Crypto.Encrypt(value.ToString(), out stars.key, out stars.IV);
            }
        }


        public bool IsGamePurchased
        {
            get
            {
                //var purchasedString = Crypto.Decrypt(gamePurchased, keyForPurchasing, IVForPurchasing);
                var purchasedString = Crypto.Decrypt(gamePurchased.data, gamePurchased.key, gamePurchased.IV);
                return purchasedString == "Game was purchased";
            }
        }

        /// <summary>
        /// Получает и сохраняет количество ключей для модов. С шифрованием. Разрешает увеличение ключей не более, чем на 20.
        /// </summary>
        public int KeysForModesCount
        {
            get
            {
                var keysString = Crypto.Decrypt(keysForModes.data, keysForModes.key, keysForModes.IV);
                if (int.TryParse(keysString, out int res))
                    return res;
                else throw new Exception("Can't get modes keys from encrypted file.");
            }

            set
            {
                //Debug.Log($"value: {value}");
                if (value - KeysForModesCount > 20)
                {
                    throw new Exception("Attempt to increase modes keys by more than 20.");
                }

                keysForModes.data = Crypto.Encrypt(value.ToString(), out keysForModes.key, out keysForModes.IV);
            }
        }


        public string UserId
        {
            get
            {
                string _userId = Crypto.Decrypt(userId.data, userId.key, userId.IV);
                if(_userId == "New player")
                {
                    //Debug.Log($"if(_userId == New player)");
                    SetRandomUserId();
                    SaveLoadUtil.SaveState(this);
                    //Debug.Log($"new userId: {Crypto.Decrypt(userId.data, userId.key, userId.IV)}");

                    return Crypto.Decrypt(userId.data, userId.key, userId.IV);
                }
                else
                {
                    //Debug.Log($"if is NOT(_userId == New player)");
                    return _userId;
                }

            }
            set
            {
                userId = new CryptedData();
                userId.data = Crypto.Encrypt(value.ToString(), out userId.key, out userId.IV);
            }
        }

        //public string _UserId => Crypto.Decrypt(userId.data, userId.key, userId.IV);

        /// <summary>
        /// Состояния пройденных игроком уровней.
        /// </summary>
        public List<LevelStateData> levelStateDatas;

        /// <summary>
        /// Состояния некоторых достижений.
        /// </summary>
        public List<AchievementSaveData> achievementSaveDatas;

        /// <summary>
        /// Номера уровней, созданных другими игроками, которые данный игрок прошел.
        /// </summary>
        public List<uint> userLevelsNumbersPassed;

        /// <summary>
        /// Номера уровней, созданных другими игроками, которые данный игрок уже оценил.
        /// </summary>
        public List<uint> userLevelsNumbersRated;


        public StateData()
        {
            stars = new CryptedData()
            {
                data = Crypto.Encrypt("0", out var key, out var IV),
                key = key,
                IV = IV
            };

            gamePurchased = new CryptedData()
            {
                data = Crypto.Encrypt("Game was NOT purchased", out key, out IV),
                key = key,
                IV = IV,
            };

            keysForModes = new CryptedData()
            {
                data = Crypto.Encrypt("10", out key, out IV),
                key = key,
                IV = IV,
            };

            userId = new CryptedData()
            {
                data = Crypto.Encrypt("New player", out key, out IV),
                key = key,
                IV = IV,
            };


            levelStateDatas = new List<LevelStateData>();
            achievementSaveDatas = new List<AchievementSaveData>();
            userLevelsNumbersPassed = new List<uint>();
            userLevelsNumbersRated = new List<uint>();
        }

        /// <summary>
        /// Возвращает сохраненное состояние уровня либо новое пустое состояние. Только для запроса сохраненных состояний.
        /// </summary>
        /// <param name="difficulty"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public LevelStateData GetLevelState(Difficulty difficulty, int level)
        {
            //var state = levelStateDatas.AsParallel().Where(lvl => lvl.difficulty == difficulty && lvl.levelNumber == level).FirstOrDefault();
            var states = levelStateDatas.AsParallel().Where(lvl => lvl.difficulty == difficulty && lvl.levelNumber == level);
            LevelStateData state;
            state = states.Where(lvl => lvl.levelType == LevelType.FasterThanLight).FirstOrDefault();
            if (state != null)
            {
                return state;
            }

            state = states.Where(lvl => lvl.levelType == LevelType.BlackHolesRiddles).FirstOrDefault();
            if (state != null)
            {
                return state;
            }

            state = states.FirstOrDefault();
            if (state != null)
            {
                return state;
            }
            else
            {
                return new LevelStateData(LevelType.PathOfLight, difficulty, level, 0);
            }
        }



        /// <summary>
        /// Возвращает сохраненное состояние уровня либо новое пустое состояние с добавлением его в список состояний.
        /// </summary>
        /// <param name="difficulty"></param>
        /// <param name="level"></param>
        /// <param name="levelType"></param>
        /// <returns></returns>
        public LevelStateData GetLevelStateOrAddNew(Difficulty difficulty, int level, LevelType levelType)
        {
            var state = levelStateDatas.AsParallel().Where(lvl => lvl.difficulty == difficulty && lvl.levelNumber == level && lvl.levelType == levelType).FirstOrDefault();
            //var state = levelStateDatas.Where(lvl => lvl.difficulty == difficulty && lvl.levelNumber == level).FirstOrDefault();

            if (state != null)
            {
                //Debug.Log($"state != null. State: {state}");
                return state;
            }
            else
            {
                if (levelStateDatas == null)
                    levelStateDatas = new List<LevelStateData>();
                state = new LevelStateData(levelType, difficulty, level, 0);
                levelStateDatas.Add(state);
                //Debug.Log($"state == null. State: {state}");
                return state;
            }
        }


        public AchievementSaveData GetAchievementSaveDataOrAddNew(Achievement_Type achievement_Type, int resultRequiredToCompleteAchievement)
        {
            var state = achievementSaveDatas.AsParallel().Where(ach => ach.achievementType == achievement_Type && ach.resultRequiredToCompleteAchievement == resultRequiredToCompleteAchievement).FirstOrDefault();

            if (state != null)
                return state;
            else
            {
                if (achievementSaveDatas == null)
                    achievementSaveDatas = new List<AchievementSaveData>();
                state = new AchievementSaveData()
                {
                    achievementType = achievement_Type,
                    resultRequiredToCompleteAchievement = resultRequiredToCompleteAchievement,
                    mainResult = 0
                };

                achievementSaveDatas.Add(state);
                return state;
            }
        }




        /// <summary>
        /// Возвращает номер последнего пройденного уровня в данной сложности.
        /// </summary>
        /// <param name="difficulty"></param>
        /// <returns></returns>
        public int GetLastPassedLevelOnDifficulty(Difficulty difficulty)
        {
            var passedLevels = levelStateDatas.Where(l => l.difficulty == difficulty).Where(l => l.StarsRecieved > 0);
            //Debug.Log($"difficulty: {difficulty}, passedLevels: {passedLevels.Count()}");
            if (passedLevels.Count() == 0)
                return 0;

            var max = passedLevels.Max(l => l.levelNumber);
            //Debug.Log($"max: {max}");
            return max;
        }


        public void GameWasPurchased()
        {
            gamePurchased.data = Crypto.Encrypt("Game was purchased".ToString(), out gamePurchased.key, out gamePurchased.IV);
        }



        public void SetRandomUserId()
        {
            Debug.Log("SetRandomUserId!!!!!!!!!!!!!!");
            int unixTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(new System.Random().Next(0, 1000000).ToString())).ComputeHash(Encoding.UTF8.GetBytes(unixTime + "" + unixTime));
            string _userId = unixTime + "" + toHexString(hmac);
            UserId = _userId.Substring(0, 50);
        }

        private static string toHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Проходил ли игрок уровень, созданный другим игроком. Поиск по номеру уровня.
        /// </summary>
        /// <param name="levelNumber"></param>
        /// <returns></returns>
        public bool HasThisLevelAlreadyBeenPassed(uint levelNumber)
        {
            if (userLevelsNumbersPassed == null) return false;
            return userLevelsNumbersPassed.Contains(levelNumber);
        }

        /// <summary>
        /// Данный игрок прошел уровень другого игрока. Номер уровня добавляется в список.
        /// </summary>
        /// <param name="levelNumber"></param>
        public void UserLevelHasBeenCompleted(uint levelNumber)
        {
            if(userLevelsNumbersPassed == null)
            {
                userLevelsNumbersPassed = new List<uint>();
            }

            if (!userLevelsNumbersPassed.Contains(levelNumber))
            {
                userLevelsNumbersPassed.Add(levelNumber);
            }
        }


        /// <summary>
        /// Оценивал ли игрок уровень, созданный другим игроком. Поиск по номеру уровня.
        /// </summary>
        /// <param name="levelNumber"></param>
        /// <returns></returns>
        public bool HasThisLevelAlreadyBeenRated(uint levelNumber)
        {
            if (userLevelsNumbersRated == null) return false;
            return userLevelsNumbersRated.Contains(levelNumber);
        }

        /// <summary>
        /// Данный игрок оценил уровень другого игрока. Номер уровня добавляется в список.
        /// </summary>
        /// <param name="levelNumber"></param>
        public void UserLevelHasBeenRated(uint levelNumber)
        {
            if (userLevelsNumbersRated == null)
            {
                userLevelsNumbersRated = new List<uint>();
            }

            if (!userLevelsNumbersRated.Contains(levelNumber))
            {
                userLevelsNumbersRated.Add(levelNumber);
            }
        }


    }

    /// <summary>
    /// Для хранения зашифрованных данных.
    /// </summary>
    [Serializable]
    public class CryptedData
    {
        public byte[] data;
        public byte[] key;
        public byte[] IV;
    }
}
