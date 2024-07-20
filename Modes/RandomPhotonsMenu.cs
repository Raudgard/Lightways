using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fields;
using Saving;
using TMPro;

namespace Modes
{
    public class RandomPhotonsMenu : MonoBehaviour
    {
        [SerializeField] private Slider sizeOfLevelSlider;
        [SerializeField] private Slider teleportsCountSlider;
        [SerializeField] private Slider blackHolesCountSlider;
        [SerializeField] private Slider darknessSlider;
        [SerializeField] private RectTransform starsPlace;
        [SerializeField] private TextMeshProUGUI questionMark;



        // -1 - random, else count.
        private int sizeOfLevel = -1;
        private int teleportsCount = -1;
        private int blackHolesCount = -1;
        // -1 - random. 0 - no darkness. 1 - darkness.
        private int darkness = -1;

        private int starsCount;

        /// <summary>
        /// ѕри переходе в меню. ”станавливает ползунки по сохранению и переменные в соответствии с ними.
        /// </summary>
        public void Initialize()
        {
            sizeOfLevelSlider.value = PlayerPrefs.GetInt(SCFPP.ModesSliders.RandomPhotons.levelSize, 0);
            teleportsCountSlider.value = PlayerPrefs.GetInt(SCFPP.ModesSliders.RandomPhotons.teleportsCount, 0);
            blackHolesCountSlider.value = PlayerPrefs.GetInt(SCFPP.ModesSliders.RandomPhotons.blackHolesCount, -1);
            darknessSlider.value = PlayerPrefs.GetInt(SCFPP.ModesSliders.RandomPhotons.darkness, -1);
        }


        public void OnSizeOfLevelSliderValueChange(float value)
        {
            
            //Debug.Log($"size value: {value}");
            int intValue = (int)value;
            sizeOfLevel = value == 0 ? -1 : intValue + 4;
            PlayerPrefs.SetInt(SCFPP.ModesSliders.RandomPhotons.levelSize, intValue);

            //Debug.Log($"sizeOfLevel: {sizeOfLevel}");
            var levelSizeMin = GetMinSizeOfLevelProperlyBlackHolesAndTeleportsCount(teleportsCount, blackHolesCount);
            //Debug.Log($"levelSizeMin: {levelSizeMin}");

            if (sizeOfLevel != -1)
            {
                //StartCoroutine(Calc(levelSizeMin));
                while (sizeOfLevel < levelSizeMin)
                {
                    //blackHolesCountSlider.SetValueWithoutNotify(blackHolesCountSlider.value - 1);
                    blackHolesCountSlider.value--;
                    levelSizeMin = GetMinSizeOfLevelProperlyBlackHolesAndTeleportsCount(teleportsCount, blackHolesCount);
                    //Debug.Log($"sizeOfLevel: {sizeOfLevel}, levelSizeMin: {levelSizeMin}");
                }
            }
            starsCount = CountStars(sizeOfLevel, teleportsCount, blackHolesCount, darkness);
            ShowStarsCount();
        }


        public void OnTeleportsCountSliderValueChange(float value)
        {
            //Debug.Log($"teleports value: {value}");
            int intValue = (int)value;
            PlayerPrefs.SetInt(SCFPP.ModesSliders.RandomPhotons.teleportsCount, intValue);
            
            if (value == 0)
            {
                teleportsCount = -1;
            }
            else if (value == 1)
            {
                teleportsCount = 0;
            }
            else
            {
                teleportsCount = intValue;
            }

            //Debug.Log($"teleportsCount: {teleportsCount}");
            var levelSizeMin = GetMinSizeOfLevelProperlyBlackHolesAndTeleportsCount(teleportsCount, blackHolesCount);
            //Debug.Log($"levelSizeMin: {levelSizeMin}");


            if (sizeOfLevel != -1)
            {
                //StartCoroutine(Calc(levelSizeMin));
                while (sizeOfLevel < levelSizeMin)
                {
                    sizeOfLevelSlider.value++;
                    levelSizeMin = GetMinSizeOfLevelProperlyBlackHolesAndTeleportsCount(teleportsCount, blackHolesCount);
                    //Debug.Log($"sizeOfLevel: {sizeOfLevel}, levelSizeMin: {levelSizeMin}");
                }
            }
            starsCount = CountStars(sizeOfLevel, teleportsCount, blackHolesCount, darkness);
            ShowStarsCount();
        }

        public void OnBlackHolesCountSliderValueChange(float value)
        {
            //Debug.Log($"black holes value: {value}");
            blackHolesCount = (int)value;
            PlayerPrefs.SetInt(SCFPP.ModesSliders.RandomPhotons.blackHolesCount, blackHolesCount);

            //Debug.Log($"blackHolesCount: {blackHolesCount}");
            var levelSizeMin = GetMinSizeOfLevelProperlyBlackHolesAndTeleportsCount(teleportsCount, blackHolesCount);
            //Debug.Log($"levelSizeMin: {levelSizeMin}");

            if (sizeOfLevel != -1)
            {
                //StartCoroutine(Calc(levelSizeMin));
                while (sizeOfLevel < levelSizeMin)
                {
                    sizeOfLevelSlider.value++;
                    levelSizeMin = GetMinSizeOfLevelProperlyBlackHolesAndTeleportsCount(teleportsCount, blackHolesCount);
                    //Debug.Log($"sizeOfLevel: {sizeOfLevel}, levelSizeMin: {levelSizeMin}");
                }
            }
            starsCount = CountStars(sizeOfLevel, teleportsCount, blackHolesCount, darkness);
            ShowStarsCount();
        }

        public void OnDarknessSliderValueChange(float value)
        {
            //Debug.Log($"darkness value: {value}");
            darkness = (int)value;
            PlayerPrefs.SetInt(SCFPP.ModesSliders.RandomPhotons.darkness, darkness);
            //Debug.Log($"darkness: {darkness}");
            starsCount = CountStars(sizeOfLevel, teleportsCount, blackHolesCount, darkness);
            ShowStarsCount();
        }



        private int GetMinSizeOfLevelProperlyBlackHolesAndTeleportsCount(int teleportsCount, int blackHolesCount)
        {
            int objects = 0;

            if (teleportsCount > 0)
                objects += teleportsCount / 3;

            if (blackHolesCount > -1)
                objects += blackHolesCount;

            return objects switch
            {
                0 => 5,
                1 => 5,
                2 => 5,
                3 => 6,
                4 => 6,
                5 => 7,
                6 => 8,
                7 => 9,
                _ => throw new System.NotImplementedException()
            };
        }

        /// <summary>
        /// ѕодсчитывает, сколько максимум звезд можно заработать за такой уровень.
        /// </summary>
        /// <param name="sizeOfLevel"></param>
        /// <param name="teleportsCount"></param>
        /// <param name="blackHolesCount"></param>
        /// <param name="darkness"></param>
        /// <returns></returns>
        private int CountStars(int sizeOfLevel, int teleportsCount, int blackHolesCount, int darkness)
        {
            int starsCount = 1;

            if (sizeOfLevel > 7) starsCount++;
            if (sizeOfLevel > 10) starsCount++;

            if (teleportsCount > 3) starsCount++;
            //if (blackHolesCount > 2) starsCount++;
            starsCount += blackHolesCount / 2;

            if (darkness == 1 && sizeOfLevel > 5) starsCount++;
            //Debug.Log($"starsCount: {starsCount}");
            return starsCount > 5 ? 5 : starsCount;
        }

        private void ShowStarsCount()
        {
            var oldStars = starsPlace.GetComponentsInChildren<Image>();
            foreach (var s in oldStars)
                Destroy(s.gameObject);

            questionMark.gameObject.SetActive(false);

            if (sizeOfLevel == -1 || teleportsCount == -1 || blackHolesCount == -1 || darkness == -1)
            {
                var star = Instantiate(Prefabs.Instance.starEmptyForUI);
                star.SetParent(starsPlace);
                star.localScale = new Vector3(0.7f, 0.7f, 1);
                star.anchoredPosition = Vector2.zero;
                questionMark.gameObject.SetActive(true);
                return;
            }

            var stars = new RectTransform[starsCount];
            for (int i = 0; i < starsCount; i++)
            {
                stars[i] = Instantiate(Prefabs.Instance.starEmptyForUI);
                stars[i].SetParent(starsPlace);
                stars[i].localScale = new Vector3(0.7f, 0.7f, 1);
            }

            if (stars.Length == 1)
            {
                stars[0].anchoredPosition = Vector2.zero;
            }
            else if (stars.Length == 2)
            {
                stars[0].anchoredPosition = new Vector2(-60f, 0);
                stars[1].anchoredPosition = new Vector2(60f, 0);
            }
            else if (stars.Length == 3)
            {
                stars[0].anchoredPosition = new Vector2(-120f, 0);
                stars[1].anchoredPosition = Vector2.zero;
                stars[2].anchoredPosition = new Vector2(120f, 0);
            }
            else if (stars.Length == 4)
            {
                stars[0].anchoredPosition = new Vector2(-180f, 0);
                stars[1].anchoredPosition = new Vector2(-60f, 0);
                stars[2].anchoredPosition = new Vector2(60f, 0);
                stars[3].anchoredPosition = new Vector2(180f, 0);
            }
            else
            {
                stars[0].anchoredPosition = new Vector2(-240f, 0);
                stars[1].anchoredPosition = new Vector2(-120f, 0);
                stars[2].anchoredPosition = Vector2.zero;
                stars[3].anchoredPosition = new Vector2(120f, 0);
                stars[4].anchoredPosition = new Vector2(240f, 0);
            }

        }



        public void CreateButtonClick()
        {
            //Debug.Log("Create button click!");
            var gameController = GameController.Instance;
            gameController.KeySpent(1);
            
            int _teleportsCount;
            int _blackHolesCount;
            bool _darkness;

            if (teleportsCount == -1)
            {
                if (Calculations.IsChanceOn(50))
                {
                    _teleportsCount = Random.Range(2, 6);
                }
                else
                    _teleportsCount = 0;
            }
            else
            {
                _teleportsCount = teleportsCount;
            }


            if (blackHolesCount == -1)
            {
                if (Calculations.IsChanceOn(50))
                {
                    _blackHolesCount = Random.Range(1, 7);
                }
                else
                    _blackHolesCount = 0;
            }
            else
            {
                _blackHolesCount = blackHolesCount;
            }

            if (darkness == -1)
            {
                _darkness = Random.Range(0, 2) > 0;
            }
            else
            {
                _darkness = darkness > 0;
            }

            int _levelSize;
            if (sizeOfLevel == -1)
            {
                int _minSize = GetMinSizeOfLevelProperlyBlackHolesAndTeleportsCount(_teleportsCount, _blackHolesCount);
                _levelSize = Random.Range(_minSize, 13);
            }
            else
            {
                _levelSize = sizeOfLevel;
            }

            Debug.Log($"_teleportsCount: {_teleportsCount}, " +
                $"_blackHolesCount: {_blackHolesCount}, " +
                $"_darkness: {_darkness}, " +
                $"_levelSize: {_levelSize}");


            var alc = ScriptableObject.CreateInstance<AutomaticLevelCreator>();


            alc.sizeX = _levelSize;
            alc.sizeY = _levelSize * 2;
            //alc.minSizeX = int.TryParse(minSizeX.text, out res) ? res : 0;
            //alc.maxSizeX = int.TryParse(maxSizeX.text, out res) ? res : 0;
            alc.minWinTowersCount = _levelSize + 1;
            alc.maxWinTowersCount = _levelSize + 1;
            alc.falseWinLineCount = _levelSize / 3;
            alc.towersInFalseWinLineCount = _levelSize < 11 ? 3 : 4;

            alc.maxTowersInFalseLine = _levelSize < 11 ? 2 : 3;
            alc.percentOfFalseTowersToCreateNewFalceLine = _levelSize < 11 ? 5 : 10;
            alc.minTotalArrows = 3;
            alc.maxTotalArrows = 5;

            alc.diagonalsUsing = DiagonalsUsing.Average;
            alc.blackHoles = CreateRandomBlackHolesList(_blackHolesCount);

            alc.teleportsCount = _teleportsCount;
            alc.usesTeleportsCount =  Random.Range(1, 8 - _blackHolesCount);
            alc.rangeMaxPercentage = _levelSize;
            alc.chanceToUseBlackHole = _teleportsCount > 0 ? 100 : 65;



            StartCoroutine(TowersMatrix.Instance.ClearHUBs());
            gameController.ClearSendedBeamsList();
            TowersMatrix.Instance.automaticLevelCreator = alc;
            TowersMatrix.Instance.CreateMatrix();
            gameController.SetDirectionalLightIntensity(_darkness ? 0 : 1);



            starsCount = CountStars(_levelSize, _teleportsCount, _blackHolesCount, _darkness ? 1 : 0);
            LevelStateData levelStateData = new LevelStateData(LevelType.RandomPhotons, Difficulty.Very_light, 0, starsCount);

            gameController.WaitForCreatingMatrixForModesLevel(levelStateData);
            gameObject.SetActive(false);
        }


        


        private List<BlackHoleInfo.BlackHoleSize> CreateRandomBlackHolesList(int count)
        {
            List<BlackHoleInfo.BlackHoleSize> res = new List<BlackHoleInfo.BlackHoleSize>();
            for (int i = 0; i < count; i++)
            {
                res.Add((BlackHoleInfo.BlackHoleSize)(new System.Random().Next(1, 5) * 45));
            }
            return res;
        }


        /// <summary>
        /// ѕровер€ет исполнение условий дл€ интерактивности слайдеров. јктивирует их, если услови€ выполнены.
        /// </summary>
        public void CheckForSlidersInteractible()
        {
            Debug.Log("CheckForSliderInteractible");
            var gameController = GameController.Instance;
            if (gameController.IsGamePurchased || gameController.IsDevelopVersion)
            {
                sizeOfLevelSlider.interactable = true;
                teleportsCountSlider.interactable = true;
                blackHolesCountSlider.interactable = true;
                darknessSlider.interactable = true;
            }
        }


       



    }
}