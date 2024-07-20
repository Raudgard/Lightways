using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fields;
using Saving;
using TMPro;

namespace Modes
{
    public class FasterThanLightMenu : MonoBehaviour
    {
        [SerializeField] private Slider sizeOfLevelSlider;
        //[SerializeField] private Slider teleportsCountSlider;
        [SerializeField] private Slider blackHolesCountSlider;
        [SerializeField] private Slider darknessSlider;
        [SerializeField] private RectTransform starsPlace;
        [SerializeField] private TextMeshProUGUI questionMark;

        [SerializeField] private FasterThanLightController fasterThanLightController;

        [Tooltip("Коэффициент, умножаемый на площадь уровня (Х * Х * 2), чтобы получить количество телепортов в уровне.")]
        [SerializeField] private float teleportsCountCoeff;
        [Tooltip("Коэффициент, умножаемый на размер уровня по Х, чтобы получить количество сфер в уровне.")]
        [SerializeField] private float spheresCountCoeff;
        [Tooltip("Коэффициент, на который умножается конечное число сфер, если уровень в темноте. ")]
        [SerializeField] private float spheresCountDarknessCoeff;


        // -1 - random, else count.
        private int sizeOfLevel = -1;
        private int teleportsCount = 4;
        private int blackHolesCount = -1;
        // -1 - random. 0 - no darkness. 1 - darkness.
        private int darkness = -1;

        private int starsCount;


        /// <summary>
        /// При переходе в меню. Устанавливает ползунки по сохранению и переменные в соответствии с ними.
        /// </summary>
        public void Initialize()
        {
            sizeOfLevelSlider.value = PlayerPrefs.GetInt(SCFPP.ModesSliders.FasterThanLight.levelSize, 0);
            blackHolesCountSlider.value = PlayerPrefs.GetInt(SCFPP.ModesSliders.FasterThanLight.blackHolesCount, -1);
            darknessSlider.value = PlayerPrefs.GetInt(SCFPP.ModesSliders.FasterThanLight.darkness, -1);
        }


        public void OnSizeOfLevelSliderValueChange(float value)
        {
            //Debug.Log($"size value: {value}");
            int intValue = (int)value;
            sizeOfLevel = value == 0 ? -1 : intValue + 4;
            PlayerPrefs.SetInt(SCFPP.ModesSliders.FasterThanLight.levelSize, intValue);

            //Debug.Log($"sizeOfLevel: {sizeOfLevel}");
            var levelSizeMin = GetMinSizeOfLevelProperlyBlackHolesCount(blackHolesCount);
            //Debug.Log($"levelSizeMin: {levelSizeMin}");

            if (sizeOfLevel != -1)
            {
                //StartCoroutine(Calc(levelSizeMin));
                while (sizeOfLevel < levelSizeMin)
                {
                    //blackHolesCountSlider.SetValueWithoutNotify(blackHolesCountSlider.value - 1);
                    blackHolesCountSlider.value--;
                    levelSizeMin = GetMinSizeOfLevelProperlyBlackHolesCount(blackHolesCount);
                    //Debug.Log($"sizeOfLevel: {sizeOfLevel}, levelSizeMin: {levelSizeMin}");
                }
            }
            SetTeleportsCountProperlyLevelSize(sizeOfLevel);
            starsCount = CountStars(sizeOfLevel, teleportsCount, blackHolesCount, darkness);
            ShowStarsCount();
        }



        public void OnBlackHolesCountSliderValueChange(float value)
        {
            //Debug.Log($"black holes value: {value}");
            blackHolesCount = (int)value;
            PlayerPrefs.SetInt(SCFPP.ModesSliders.FasterThanLight.blackHolesCount, blackHolesCount);

            //Debug.Log($"blackHolesCount: {blackHolesCount}");
            var levelSizeMin = GetMinSizeOfLevelProperlyBlackHolesCount(blackHolesCount);
            //Debug.Log($"levelSizeMin: {levelSizeMin}");

            if (sizeOfLevel != -1)
            {
                //StartCoroutine(Calc(levelSizeMin));
                while (sizeOfLevel < levelSizeMin)
                {
                    sizeOfLevelSlider.value++;
                    levelSizeMin = GetMinSizeOfLevelProperlyBlackHolesCount(blackHolesCount);
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
            PlayerPrefs.SetInt(SCFPP.ModesSliders.FasterThanLight.darkness, darkness);

            //Debug.Log($"darkness: {darkness}");
            starsCount = CountStars(sizeOfLevel, teleportsCount, blackHolesCount, darkness);
            ShowStarsCount();
        }



        private int GetMinSizeOfLevelProperlyBlackHolesCount(int blackHolesCount)
        {
            int objects = 0;

            //if (teleportsCount > 0)
            //    objects += teleportsCount / 3;

            if (blackHolesCount > -1)
                objects += blackHolesCount;

            return objects switch
            {
                0 => 5,
                1 => 5,
                2 => 6,
                3 => 7,
                4 => 8,
                5 => 10,
                6 => 11,
                //7 => 12,
                _ => throw new System.NotImplementedException()
            };
        }

        /// <summary>
        /// Рассчитывает и устанавливает количество телепортов в зависимости от размера уровня.
        /// </summary>
        /// <param name="sizeOfLevel">Размер уровня по Х.</param>
        private void SetTeleportsCountProperlyLevelSize(int sizeOfLevel)
        {
            if (sizeOfLevel == -1) sizeOfLevel = 5;
            teleportsCount = (int)(sizeOfLevel * sizeOfLevel * 2 * teleportsCountCoeff);
        }







        /// <summary>
        /// Подсчитывает, сколько максимум звезд можно заработать за такой уровень.
        /// </summary>
        /// <param name="sizeOfLevel"></param>
        /// <param name="teleportsCount"></param>
        /// <param name="blackHolesCount"></param>
        /// <param name="darkness"></param>
        /// <returns></returns>
        private int CountStars(int sizeOfLevel, int teleportsCount, int blackHolesCount, int darkness)
        {
            int starsCount = 1;
            if (sizeOfLevel > 9) starsCount++;
            if (blackHolesCount > 2) starsCount++;
            if (blackHolesCount > 4) starsCount++;

            //starsCount += blackHolesCount / 3;
            if (darkness == 1 && sizeOfLevel > 6) starsCount++;
            //Debug.Log($"starsCount: {starsCount}");
            return starsCount;
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



        public void GoButtonClick()
        {
            //Debug.Log("Create button click!");
            var gameController = GameController.Instance;
            gameController.KeySpent(1);

            int _blackHolesCount;
            bool _darkness;


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
                int _minSize = GetMinSizeOfLevelProperlyBlackHolesCount(_blackHolesCount);
                _levelSize = Random.Range(_minSize, 13);
            }
            else
            {
                _levelSize = sizeOfLevel;
            }

            SetTeleportsCountProperlyLevelSize(_levelSize);
            var alc = ScriptableObject.CreateInstance<AutomaticLevelCreator>();

            alc.sizeX = _levelSize;
            alc.sizeY = _levelSize * 2;
            //alc.minSizeX = int.TryParse(minSizeX.text, out res) ? res : 0;
            //alc.maxSizeX = int.TryParse(maxSizeX.text, out res) ? res : 0;
            float darknessCoeff = _darkness ? spheresCountDarknessCoeff : 1f;
            int spheresCount = (int)(_levelSize * spheresCountCoeff * darknessCoeff);
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

            alc.teleportsCount = teleportsCount;
            alc.usesTeleportsCount = 7;
            alc.rangeMaxPercentage = _levelSize;
            alc.chanceToUseBlackHole = 100;

            Debug.Log($"_teleportsCount: {teleportsCount}, " +
                $"_blackHolesCount: {_blackHolesCount}, " +
                $"_darkness: {_darkness}, " +
                $"_levelSize: {_levelSize}" +
                $"spheresCount: {spheresCount}");

            StartCoroutine(TowersMatrix.Instance.ClearHUBs());
            gameController.ClearSendedBeamsList();
            TowersMatrix.Instance.automaticLevelCreator = alc;
            TowersMatrix.Instance.CreateMatrix();
            gameController.SetDirectionalLightIntensity(_darkness ? 0 : 1);

            gameObject.SetActive(false);


            starsCount = CountStars(_levelSize, teleportsCount, _blackHolesCount, _darkness ? 1 : 0);
            LevelStateData levelStateData = new LevelStateData(LevelType.FasterThanLight, Difficulty.Very_light, 0, starsCount);

            //gameController.LoadCustomLevel(levelStateData);
            gameController.WaitForCreatingMatrixForModesLevel(levelStateData);
            //fasterThanLightController.Initialize(-1);
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
                blackHolesCountSlider.interactable = true;
                darknessSlider.interactable = true;
            }
        }
    }
}