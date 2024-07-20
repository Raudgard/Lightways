using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fields;
using Saving;
using TMPro;

namespace Modes
{
    public class BlackHolesRiddlesMenu : MonoBehaviour
    {
        [SerializeField] private BlackHolesRiddlesController blackHolesRiddlesController;

        [SerializeField] private Slider sizeOfLevelSlider;
        [SerializeField] private Slider teleportsCountSlider;
        [SerializeField] private Slider blackHolesCountSlider;
        //[SerializeField] private Slider darknessSlider;
        [SerializeField] private RectTransform starsPlace;
        [SerializeField] private TextMeshProUGUI questionMark;


        [Tooltip("Коэффициент, умножаемый на размер уровня по Х, чтобы получить количество сфер в уровне.")]
        [SerializeField] private float spheresCountCoeff;

        // -1 - random, else count.
        private int sizeOfLevel = -1;
        private int teleportsCount = -1;
        // 0 - random, else count.
        private int blackHolesCount = 0;

        private int starsCount;

        /// <summary>
        /// При переходе в меню. Устанавливает ползунки по сохранению и переменные в соответствии с ними.
        /// </summary>
        public void Initialize()
        {
            sizeOfLevelSlider.value = PlayerPrefs.GetInt(SCFPP.ModesSliders.BlackHolesRiddles.levelSize, 0);
            teleportsCountSlider.value = PlayerPrefs.GetInt(SCFPP.ModesSliders.BlackHolesRiddles.teleportsCount, 0);
            blackHolesCountSlider.value = PlayerPrefs.GetInt(SCFPP.ModesSliders.BlackHolesRiddles.blackHolesCount, -1);
        }


        public void OnSizeOfLevelSliderValueChange(float value)
        {
            //Debug.Log($"size value: {value}");
            int intValue = (int)value;
            sizeOfLevel = value == 0 ? -1 : intValue + 5;
            PlayerPrefs.SetInt(SCFPP.ModesSliders.BlackHolesRiddles.levelSize, intValue);
            //Debug.Log($"sizeOfLevel: {sizeOfLevel}");
            var levelSizeMin = GetMinSizeOfLevelProperlyBlackHolesAndTeleportsCount(teleportsCount, blackHolesCount);
            //Debug.Log($"levelSizeMin: {levelSizeMin}");

            if (sizeOfLevel != -1)
            {
                //StartCoroutine(Calc(levelSizeMin));
                while (sizeOfLevel < levelSizeMin)
                {
                    //blackHolesCountSlider.SetValueWithoutNotify(blackHolesCountSlider.value - 1);
                    if (blackHolesCountSlider.value == 1)
                    {
                        if(teleportsCountSlider.value > 1)
                            teleportsCountSlider.value--;
                    }
                    else
                    { 
                        blackHolesCountSlider.value--;
                    }
                    levelSizeMin = GetMinSizeOfLevelProperlyBlackHolesAndTeleportsCount(teleportsCount, blackHolesCount);
                    //Debug.Log($"sizeOfLevel: {sizeOfLevel}, levelSizeMin: {levelSizeMin}");
                }
            }
            starsCount = CountStars(sizeOfLevel, teleportsCount, blackHolesCount);
            ShowStarsCount();
        }


        public void OnTeleportsCountSliderValueChange(float value)
        {
            //Debug.Log($"teleports value: {value}");
            int intValue = (int)value;
            PlayerPrefs.SetInt(SCFPP.ModesSliders.BlackHolesRiddles.teleportsCount, intValue);

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
            starsCount = CountStars(sizeOfLevel, teleportsCount, blackHolesCount);
            ShowStarsCount();
        }

        public void OnBlackHolesCountSliderValueChange(float value)
        {
            //Debug.Log($"black holes value: {value}");
            blackHolesCount = (int)value;
            PlayerPrefs.SetInt(SCFPP.ModesSliders.BlackHolesRiddles.blackHolesCount, blackHolesCount);
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
            starsCount = CountStars(sizeOfLevel, teleportsCount, blackHolesCount);
            ShowStarsCount();
        }

        //public void OnDarknessSliderValueChange(float value)
        //{
        //    //Debug.Log($"darkness value: {value}");
        //    darkness = (int)value;
        //    //Debug.Log($"darkness: {darkness}");
        //    starsCount = CountStars(sizeOfLevel, teleportsCount, blackHolesCount, darkness);
        //    ShowStarsCount();
        //}



        private int GetMinSizeOfLevelProperlyBlackHolesAndTeleportsCount(int teleportsCount, int blackHolesCount)
        {
            int objects = 0;

            if (teleportsCount > 0)
                objects += teleportsCount / 3;

            if (blackHolesCount > 0)
                objects += blackHolesCount;
            //Debug.Log($"objects: {objects}");

            return objects switch
            {
                0 => 6,
                1 => 6,
                2 => 6,
                3 => 6,
                4 => 6,
                5 => 7,
                _ => throw new System.NotImplementedException()
            };
        }

        /// <summary>
        /// Подсчитывает, сколько максимум звезд можно заработать за такой уровень.
        /// </summary>
        /// <param name="sizeOfLevel"></param>
        /// <param name="teleportsCount"></param>
        /// <param name="blackHolesCount"></param>
        /// <returns></returns>
        private int CountStars(int sizeOfLevel, int teleportsCount, int blackHolesCount)
        {
            int starsCount;
            switch (blackHolesCount)
            {
                case 0: starsCount = 0;
                    break;

                case 1:
                    starsCount = 1;
                    if (sizeOfLevel > 9) starsCount++;
                    if (teleportsCount > 2) starsCount++;
                    if (teleportsCount > 4) starsCount++;
                    break;

                case 2:
                    starsCount = 3;
                    if (sizeOfLevel > 8) starsCount++;
                    if (sizeOfLevel > 10) starsCount++;

                    if (teleportsCount > 2) starsCount++;
                    if (teleportsCount > 4) starsCount++;

                    break;

                case 3:
                    starsCount = 4;
                    //if (sizeOfLevel > 10 || teleportsCount > 2) starsCount++;
                    if (sizeOfLevel > 7) starsCount++;
                    if (sizeOfLevel > 9) starsCount++;
                    if (sizeOfLevel > 11) starsCount++;

                    if (teleportsCount > 2) starsCount++;
                    if (teleportsCount > 4) starsCount++;
                    break;

                default: throw new System.NotImplementedException();
            }

            return starsCount > 5 ? 5 : starsCount;
            
            //if (sizeOfLevel > 9) starsCount++;

            //starsCount += teleportsCount / 3;
            //starsCount += blackHolesCount;
            //return starsCount > 5 ? 5 : starsCount;
        }

        private void ShowStarsCount()
        {
            var oldStars = starsPlace.GetComponentsInChildren<Image>();
            foreach (var s in oldStars)
                Destroy(s.gameObject);

            questionMark.gameObject.SetActive(false);

            if (sizeOfLevel == -1 || teleportsCount == -1 || blackHolesCount == 0)
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



        public void GoButtonClick()
        {
            //Debug.Log("Create button click!");
            var gameController = GameController.Instance;
            gameController.KeySpent(1);

            int _teleportsCount;
            int _blackHolesCount;
            //bool _darkness;

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


            if (blackHolesCount == 0)
            {
                _blackHolesCount = Random.Range(1, 4);
            }
            else
            {
                _blackHolesCount = blackHolesCount;
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
                //$"_darkness: {_darkness}, " +
                $"_levelSize: {_levelSize}");


            var alc = ScriptableObject.CreateInstance<AutomaticLevelCreator>();


            alc.sizeX = _levelSize;
            alc.sizeY = _levelSize * 2;
            //alc.minSizeX = int.TryParse(minSizeX.text, out res) ? res : 0;
            //alc.maxSizeX = int.TryParse(maxSizeX.text, out res) ? res : 0;
            int spheresCount = (int)(_levelSize * spheresCountCoeff);
            alc.minWinTowersCount = spheresCount;
            alc.maxWinTowersCount = spheresCount;
            alc.falseWinLineCount = 0;
            alc.towersInFalseWinLineCount = 0;

            alc.maxTowersInFalseLine = 0;
            alc.percentOfFalseTowersToCreateNewFalceLine = 0;
            alc.minTotalArrows = 3;
            alc.maxTotalArrows = 5;

            alc.diagonalsUsing = DiagonalsUsing.Average;
            alc.blackHoles = CreateRandomBlackHolesList(_blackHolesCount);

            alc.teleportsCount = _teleportsCount;
            alc.usesTeleportsCount = Random.Range(1, 7);
            alc.rangeMaxPercentage = _levelSize;
            alc.chanceToUseBlackHole = 100;



            StartCoroutine(TowersMatrix.Instance.ClearHUBs());
            gameController.ClearSendedBeamsList();
            TowersMatrix.Instance.automaticLevelCreator = alc;
            TowersMatrix.Instance.CreateMatrix();
            gameController.SetDirectionalLightIntensity(1);

            gameObject.SetActive(false);


            starsCount = CountStars(_levelSize, _teleportsCount, _blackHolesCount);
            LevelStateData levelStateData = new LevelStateData(LevelType.BlackHolesRiddles, Difficulty.Very_light, 0, starsCount);
            //gameController.LoadCustomLevel(levelStateData);
            gameController.WaitForCreatingMatrixForModesLevel(levelStateData);
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
        /// Проверяет исполнение условий для интерактивности слайдеров. Активирует их, если условия выполнены.
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
            }
        }
    }
}