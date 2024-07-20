using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CloudServices.LeaderBoardsFromPlayerIO;
using UnityEngine.UI;
using PlayerIOClient;
using System.Linq;
using System;
using TMPro;
using Saving;
using Fields;

namespace UserLevels
{
    /// <summary>
    /// Управляет загрузкой и отображением уровней, созданных игроками.
    /// </summary>
    public class UserLevelsController : MonoBehaviour
    {
        private enum SortingType
        {
            /// <summary>
            /// С наивысшим рейтингом
            /// </summary>
            Rating = 0,
            /// <summary>
            /// Новейшие
            /// </summary>
            Newest = 1,
            /// <summary>
            /// Наиболее играемые (запускаемые)
            /// </summary>
            Popular = 2,
            /// <summary>
            /// По количеству звезд в награде.
            /// </summary>
            Reward = 3,
            /// <summary>
            /// Опубликованные самим пользователем.
            /// </summary>
            My = 4,
        }

        private enum SortingOrder
        {
            Direct = 0,
            Reverse = 1
        }

        [Tooltip("Область отображения строк с уровнями игроков.")]
        [SerializeField] private RectTransform contentRectT;

        [SerializeField] private Button ratingButton;
        [SerializeField] private Button newLevelsButton;
        [SerializeField] private Button popularLevelsButton;
        [SerializeField] private Button rewardButton;
        [SerializeField] private Button myLevelsButton;

        [Tooltip("Номера отображаемых уровней в данной сортировке.")]
        [SerializeField] private TextMeshProUGUI levelsIntervalText;

        [Tooltip("Кнопка назад (для предыдущих уровней в данной сортировке).")]
        [SerializeField] private Button backwardButton;

        [Tooltip("Кнопка вперед (для следующих уровней в данной сортировке).")]
        [SerializeField] private Button forwardButton;

        [Tooltip("Изображение пустых звезд, при установке оценки уровню.")]
        [SerializeField] private Image emptyStarsImage;

        [Tooltip("Изображение заполненных звезд, при установке оценки уровню.")]
        [SerializeField] private Image fullStarsImage;

        [Tooltip("Слайдер, отслеживающий нажатия игрока при установке оценки уровню.")]
        [SerializeField] private Slider fullStarsSlider;

        [Tooltip("Кнопка \"Оценить\".")]
        [SerializeField] private Button rateButton;

        



        [Space]

        [Tooltip("Количество отображаемых уровней.")]
        [SerializeField] private int showingLevelsNumber;

        [Tooltip("Интервал между строками уровней.")]
        [SerializeField] private float intervalBetweenLines;

        [Tooltip("Время для перекрашивания кнопки.")]
        [SerializeField] private float crossFadeColorButtonTime;

        [Space]

        [Tooltip("Название таблицы на сервере PIO с информацией о уровнях, но без самих уровней.")]
        [SerializeField] private string tableOnServerPIOWithLevelsInfo;

        [Tooltip("Название таблицы на сервере PIO с самими уровнями в JSON.")]
        [SerializeField] private string tableOnServerPIOWithLevelsThemselves;
        [Space]



        private ClientPlayerIO clientComponent;

        [Tooltip("Секунд для загрузки клиента PIO, если вдруг он не загружен.")]
        [SerializeField] private float secondsTimeoutForLoadingClient;

        private GameController gameController;
        private Color inactiveButtonColor;
        private SortingType currentSortingType = SortingType.My;
        private SortingOrder currentSortingOrder = SortingOrder.Direct;
        private DatabaseObject[] usersLevelsInfo;
        /// <summary>
        ///Индекс первого уровня в отображаемом диапазоне текущей сортировки.
        ///Например, если отображаются уровни с 21-й по 30-й, то индекс будет 20 (т.е. 21 - 1 из-за начала отсчета с 0, как во всех массивах).
        /// </summary>
        private int StartingIndexOfShowingLevel { get; set; } = 0;

        /// <summary>
        /// Выставляемая оценка.
        /// </summary>
        private int givenRating { get; set; } = 5;

        /// <summary>
        /// Уровень, загруженный в данный момент.
        /// </summary>
        private UserLevelInfo currentLevel { get; set; } = null;

        private void Awake()
        {
            //Debug.Log($"UserLevelsController Awake");
            gameController = GameController.Instance;
            gameController.onWinLevel += OnWinLevel;
            UIController.Instance.onQuitLevel += OnQuitLevel;
        }

        private void OnEnable()
        {
            Debug.Log("UserLevelsController  OnEnable");
            clientComponent = ClientPlayerIO.Instance;
            inactiveButtonColor = gameController.colorTheme.linesAndBackgroundSmokeColorGrey;
            SetSortingTypeToDefault();
            SetActiveColorForButton(null);
            RatingButtonClick();
        }

        public void UserLevelsButtonClick() => gameObject.SetActive(true);

        /// <summary>
        /// Устанавливает поле currentSortingType в значение по умолчанию.
        /// </summary>
        private void SetSortingTypeToDefault()
        {
            currentSortingType = SortingType.My;
        }

        public void BackButtonClick()
        {
            gameObject.SetActive(false);
            SetSortingTypeToDefault();
        }

        public void RatingButtonClick()
        {
            SetActiveColorForButton(ratingButton, crossFadeColorButtonTime);
            SortingButtonClick(SortingType.Rating);
            
        }

        public void NewLevelsButtonClick()
        {
            SetActiveColorForButton(newLevelsButton, crossFadeColorButtonTime);
            SortingButtonClick(SortingType.Newest);

        }

        public void PopularLevelsButtonClick()
        {
            SetActiveColorForButton(popularLevelsButton, crossFadeColorButtonTime);
            SortingButtonClick(SortingType.Popular);

        }


        public void RewardButtonClick()
        {
            SetActiveColorForButton(rewardButton, crossFadeColorButtonTime);
            SortingButtonClick(SortingType.Reward);

        }

        public void MyLevelsButtonClick()
        {
            SetActiveColorForButton(myLevelsButton, crossFadeColorButtonTime);
            SortingButtonClick(SortingType.My);

        }

        /// <summary>
        /// Нажатие на любую кнопку по запросу и сортировке уровней.
        /// </summary>
        /// <param name="sortingType"></param>
        private void SortingButtonClick(SortingType sortingType)
        {
            StartingIndexOfShowingLevel = 0;
            //Debug.Log($"currentSortingType: {currentSortingType}, sortingType: {sortingType}");
            if (currentSortingType != sortingType)
            {
                StartCoroutine(GetLevelsInfoFromServer(sortingType));
            }
            else if (usersLevelsInfo != null)
            {
                usersLevelsInfo = usersLevelsInfo.Reverse().ToArray();
                currentSortingOrder = currentSortingOrder == SortingOrder.Direct ? SortingOrder.Reverse : SortingOrder.Direct;
                BackwardButtonClick();
            }
            else
            {
                throw new Exception();
            }
        }



        /// <summary>
        /// Очищает область показа строк с уровнями игроков от этих строк.
        /// </summary>
        private void ClearContent()
        {
            contentRectT.GetComponentsInChildren<UserLevelLabel>().ToList().ForEach(l => Destroy(l.gameObject));
        }

        

        /// <summary>
        /// Раскрасить одну из 5 кнопок в активный цвет, остальные в неактивный. Если аргумент NULL, то все 5 в неактивный.
        /// </summary>
        /// <param name="button">Раскрашиваемая кнопка. Если аргумент NULL, то все 4 будут раскрашены в неактивный.</param>
        /// <param name="crossFadeTime">Время, за которое происходит перекрашивание.</param>
        private void SetActiveColorForButton(Button button, float crossFadeTime = 0)
        {
            gameController.colorTheme.GetActiveColors(out Color backgroundColor, out _, out _, out _);
            Button[] buttons = new Button[] { ratingButton, newLevelsButton, popularLevelsButton, rewardButton, myLevelsButton };

            foreach (var b in buttons)
            {
                if (b == button)
                {
                    b.GetComponent<Image>().CrossFadeColor(backgroundColor, crossFadeTime, true, false);
                }
                else
                {
                    b.GetComponent<Image>().CrossFadeColor(inactiveButtonColor, crossFadeTime, true, false);
                }
            }
        }





        /// <summary>
        /// Получить объекты из первой таблицы на сервере PIO: объекты с инфой об уровнях без самих уровней.
        /// </summary>
        /// <param name="sortingType"></param>
        /// <returns></returns>
        private IEnumerator GetLevelsInfoFromServer(SortingType sortingType)
        {
            UIController.Instance.ShowInformationInMenu(gameController.languageController.waiting, secondsTimeoutForLoadingClient);
            backwardButton.interactable = false;
            forwardButton.interactable = false;

            if (clientComponent.Client == null)
            {
                clientComponent.GetClient();

                float timePassed = 0;
                while (clientComponent.Client == null && clientComponent.Error == null)
                {
                    yield return null;
                    timePassed += Time.deltaTime;
                    Debug.Log($"timePassed: {timePassed}");
                    if (timePassed > secondsTimeoutForLoadingClient)
                    {
                        Debug.LogWarning($"Time is out: {timePassed} seconds passed.");
                        UIController.Instance.ShowInformationInMenu(gameController.languageController.theResponseWaitingTimeHasBeenExceeded);
                        yield break;
                    }
                }
            }

            if (clientComponent.Client == null)
            {
                Debug.LogWarning($"clientComponent.Client == null!!!");
                currentSortingType = sortingType;
                yield break;
            }

            string index = sortingType switch
            {
                SortingType.Rating => "rating",
                SortingType.Newest => "levelNumber",
                SortingType.Popular => "launched",
                SortingType.Reward => "reward",
                SortingType.My => "levelNumber",

                _ => throw new NotImplementedException()
            };

            clientComponent.Client.BigDB.LoadRange(tableOnServerPIOWithLevelsInfo, index, null, null, null, 1000, usersLevelsInfo =>
            {
                Debug.Log("GetLevelsInfoFromServer");
                this.usersLevelsInfo = sortingType == SortingType.My ? 
                usersLevelsInfo.AsParallel().Where(uli => uli.GetString("userID") == gameController.StateData.UserId).Reverse().ToArray() : 
                usersLevelsInfo;

                BackwardButtonClick();
                UIController.Instance.ShowInformationInMenu(string.Empty, 0);
                currentSortingType = sortingType;
                currentSortingOrder = SortingOrder.Direct;
            },
            er =>
            {
                Debug.Log($"can't load table. er: {er}");
                UIController.Instance.ShowInformationInMenu(er.ToString(), 5);
            });
        }


        /// <summary>
        /// Получить необходимое количество объектов с инфой о уровнях, начиная с указанного индекса.
        /// </summary>
        /// <param name="userLevelsInfo"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private DatabaseObject[] GetUserLevelsInfoNessesaryCount(DatabaseObject[] userLevelsInfo, int startIndex, int count)
        {
            return userLevelsInfo.Skip(startIndex).Take(count).ToArray();
        }


        /// <summary>
        /// Создает строки с кнопками в соответствии с загруженной инфой об этих уровнях с сервера PIO.
        /// </summary>
        /// <param name="userLevelsInfo"></param>
        private void FillContent(DatabaseObject[] userLevelsInfo)
        {
            UserLevelInfo[] levelsInfo = GetLevels(userLevelsInfo);
            Vector2 lineSize = Prefabs.Instance.userLevelLabel.GetComponent<RectTransform>().sizeDelta;
            var verticalSize = lineSize.y + intervalBetweenLines;
            var gorizontalSize = lineSize.x;


            for (int i = 0; i < levelsInfo.Length; i++)
            {
                UserLevelLabel userLevelLabel = Instantiate(Prefabs.Instance.userLevelLabel);

                var rectT = userLevelLabel.GetComponent<RectTransform>();
                rectT.SetParent(contentRectT);
                rectT.localScale = Vector3.one;
                rectT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, verticalSize * i, verticalSize - intervalBetweenLines);
                rectT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, gorizontalSize);
                rectT.transform.position = new Vector3(rectT.transform.position.x, rectT.transform.position.y, -500);


                userLevelLabel.name = $"User level button  {levelsInfo[i].levelName}";

                //userLevelLabel.Initialize(levels[i].levelName, levels[i].userName, levels[i].rating, levels[i].launched, levels[i].passed, levels[i].rated, levels[i].level, UserLevelButtonClickHandler);
                userLevelLabel.Initialize(levelsInfo[i], UserLevelButtonClickHandler);
            }
        }

        /// <summary>
        /// Превращает объекты с сервера PIO в массив объектов с инфой об уровнях (кроме самого уровня в JSON).
        /// </summary>
        /// <param name="levelObjects"></param>
        /// <returns></returns>
        private UserLevelInfo[] GetLevels(params DatabaseObject[] levelObjects)
        {
            UserLevelInfo[] userLevelInfos = new UserLevelInfo[levelObjects.Length];

            for (int i = 0; i < userLevelInfos.Length; i++)
            {
                userLevelInfos[i] = new UserLevelInfo()
                {
                    rating = levelObjects[i].GetFloat("rating"),
                    launched = levelObjects[i].GetUInt("launched"),
                    passed = levelObjects[i].GetUInt("passed"),
                    rated = levelObjects[i].GetUInt("rated"),
                    levelNumber = levelObjects[i].GetUInt("levelNumber"),
                    reward = levelObjects[i].GetUInt("reward"),
                    levelName = levelObjects[i].GetString("LevelName"),
                    userName = levelObjects[i].GetString("userName"),
                    userID = levelObjects[i].GetString("userID"),
                };
            }

            return userLevelInfos;
        }

        /// <summary>
        /// Загружает уровень с свервера PIO и запускает его.
        /// </summary>
        private void UserLevelButtonClickHandler(uint levelNumber, string levelName, uint rewardStars)
        {
            if (clientComponent.Client == null)
            {
                UIController.Instance.ShowInformationInMenu(gameController.languageController.Thereisnoconnectiontotheremoteserver);
                return;
            }

            UIController.Instance.ShowInformationInMenu(gameController.languageController.waiting, secondsTimeoutForLoadingClient);

            Debug.Log("GetLevelFromServer");
            object[] _levelNumber = new object[] { levelNumber };
            clientComponent.Client.BigDB.LoadSingle(tableOnServerPIOWithLevelsThemselves, "levelNumber", _levelNumber, userLevel =>
            {
                StartCoroutine(LoadingAndStartingLevel(userLevel, levelNumber, rewardStars, levelName));
            },
            er =>
            {
                Debug.Log($"can't load table. er: {er}");
                UIController.Instance.ShowInformationInMenu(er.ToString(), secondsTimeoutForLoadingClient);
            });

        }


        private IEnumerator LoadingAndStartingLevel(DatabaseObject userLevel, uint levelNumber, uint rewardStars, string levelName)
        {
            //Debug.Log($"Level LOADED! level: {userLevel}");

            string levelString = userLevel.GetString("level");

            Level level = SaveLoadUtil.LoadLevelFromString(levelString);
            LevelStateData levelStateData = new LevelStateData(LevelType.RandomPhotons, Difficulty.Very_light, 0, (int)rewardStars);
            levelStateData.levelNumber = (int)levelNumber;

            TowersMatrix.Instance.Matrix = level.matrix;
            TowersMatrix.Instance.Matrix.AddPointsOfBlackHolesInfluence();
            UIController.Instance.SetActiveMainMenuObjects(false);
            yield return StartCoroutine(TowersMatrix.Instance.CreatePlayObjectsFromMatrix());
            Debug.Log($"level.directionalLightIntensity: {level.directionalLightIntensity}");
            GameController.Instance.SetDirectionalLightIntensity(level.directionalLightIntensity);
            GameController.Instance.IsStoryLevel = false;
            yield return StartCoroutine(GameController.Instance.LoadCustomLevel(levelStateData, true, false));
            UIController.Instance.SetLevelNameToLabel(levelName);
            UIController.Instance.currentScreen = ActiveScreen.Game;

            gameObject.SetActive(false);
            UIController.Instance.SetActiveForModesMenu(false);
            gameController.IsUserLevel = true;
            Debug.Log($"IsUserLevel: {GameController.Instance.IsUserLevel}");

            UIController.Instance.ShowInformationInMenu(string.Empty, 0);
            currentLevel = GetLevels(usersLevelsInfo.FirstOrDefault(l => l.GetUInt("levelNumber") == levelNumber))[0];
            Debug.Log($"currentLevel: {currentLevel}");
            currentSortingOrder = SortingOrder.Reverse;
            AddOneLaunchedToUserLevel(levelNumber);
        }

        /// <summary>
        /// Добавляет один запуск к уровню, созданному игроком.
        /// </summary>
        /// <param name="levelNumber"></param>
        private void AddOneLaunchedToUserLevel(uint levelNumber)
        {
            if (clientComponent.Client != null)
            {
                object[] _levelNumber = new object[] { levelNumber };
                clientComponent.Client.BigDB.LoadSingle(tableOnServerPIOWithLevelsInfo, "levelNumber", _levelNumber, userLevel =>
                {
                    uint launched = userLevel.GetUInt("launched");
                    userLevel.Set("launched", ++launched);
                    userLevel.Save();
                    Debug.Log("Level launched +1");
                },
                er =>
                {
                    Debug.Log($"can't adding lauched. er: {er}");
                });
            }
        }


        /// <summary>
        /// Нажимает кнопку назад и запускает формирование и отображение нового списка кнопок.
        /// </summary>
        public void BackwardButtonClick()
        {
            if(StartingIndexOfShowingLevel != 0)
                StartingIndexOfShowingLevel -= showingLevelsNumber;

            CountNecessaryLevelsAndFillContent();
        }

        /// <summary>
        /// Нажимает кнопку вперед и запускает формирование и отображение нового списка кнопок.
        /// </summary>
        public void ForwardButtonClick()
        {
            StartingIndexOfShowingLevel += showingLevelsNumber;
            CountNecessaryLevelsAndFillContent();
        }

        


        public void RateSliderValueChanged(float value)
        {
            //Debug.Log($"RateSliderValueChanged: value: {value}");

            if (value >= 0 && value < 1) givenRating = 1;
            else if (value >= 1 && value < 2) givenRating = 2;
            else if (value >= 2 && value < 3) givenRating = 3;
            else if (value >= 3 && value < 4) givenRating = 4;
            else if (value >= 4 && value <= 5) givenRating = 5;
            else throw new NotImplementedException(); 

            fullStarsImage.fillAmount = (float)givenRating / 5;
        }

        public void RateButtonClick()
        {
            Debug.Log($"RateButtonClick! givenRating: {givenRating}");

            if (clientComponent.Client != null)
            {
                object[] _levelNumber = new object[] { currentLevel.levelNumber };
                clientComponent.Client.BigDB.LoadSingle(tableOnServerPIOWithLevelsInfo, "levelNumber", _levelNumber, userLevel =>
                {
                    uint rated = userLevel.GetUInt("rated");
                    float rating = userLevel.GetFloat("rating");
                    float oldRatingSum = rated * rating;
                    userLevel.Set("rated", ++rated);
                    float ratingNew = (oldRatingSum + givenRating) / rated;
                    userLevel.Set("rating", ratingNew);
                    userLevel.Save();
                    Debug.Log("Level rated +1");
                    UIController.Instance.ShowInformationInMenu(gameController.languageController.scoreIsSet, secondsTimeoutForLoadingClient);
                    gameController.StateData.UserLevelHasBeenRated(currentLevel.levelNumber);
                    SetInteractibleForRateButton(false);
                    emptyStarsImage.gameObject.SetActive(false);
                    fullStarsImage.gameObject.SetActive(false);
                },
                er =>
                {
                    Debug.Log($"can't adding passed. er: {er}");
                    UIController.Instance.ShowInformationInMenu(er.ToString(), secondsTimeoutForLoadingClient);
                });
            }

        }

        public void RateButtonAreaClick()
        {
            Debug.Log($"RateButtonAreaClick! ");
            if(currentLevel != null && currentLevel.userID == gameController.StateData.UserId)
            {
                UIController.Instance.ShowInformationInMenu(gameController.languageController.youCantRateYourOwnLevel);
                return;
            }

            if (currentLevel != null && gameController.StateData.HasThisLevelAlreadyBeenRated(currentLevel.levelNumber))
            {
                UIController.Instance.ShowInformationInMenu(gameController.languageController.youAlreadyRatedThisLevel);
            }
        }

        private void OnWinLevel()
        {
            if (!gameController.IsUserLevel)
                return;

            Debug.Log("Win level in User level Controller");
            RateSliderValueChanged(5);

            AddOnePassedToUserLevel(currentLevel.levelNumber);
        }

        /// <summary>
        /// Добавить одно прохождение к уровню, созданному игроком.
        /// </summary>
        /// <param name="levelNumber"></param>
        private void AddOnePassedToUserLevel(uint levelNumber)
        {
            if (clientComponent.Client != null)
            {
                object[] _levelNumber = new object[] { levelNumber };
                clientComponent.Client.BigDB.LoadSingle(tableOnServerPIOWithLevelsInfo, "levelNumber", _levelNumber, userLevel =>
                {
                    uint passed = userLevel.GetUInt("passed");
                    userLevel.Set("passed", ++passed);
                    userLevel.Save();
                    Debug.Log("Level passed +1");
                },
                er =>
                {
                    Debug.Log($"can't adding passed. er: {er}");
                });
            }
        }

        /// <summary>
        /// Проверка на активность элементов оценки после победы в уровне.
        /// </summary>
        public void CheckForRatingActivity()
        {
            //Делает неактивной кнопку Оценить в случае, если игрок пытается оценить свой собственный уровень или этот уровень он уже оценивал.
            bool active = currentLevel != null && currentLevel.userID != gameController.StateData.UserId && !gameController.StateData.HasThisLevelAlreadyBeenRated(currentLevel.levelNumber);
            SetInteractibleForRateButton(active);
            emptyStarsImage.gameObject.SetActive(active);
            fullStarsImage.gameObject.SetActive(active);
        }


        private void SetInteractibleForRateButton(bool interactable) => rateButton.interactable = interactable;






        private void CountNecessaryLevelsAndFillContent()
        {
            DatabaseObject[] countedUsersLevelsInfo = GetUserLevelsInfoNessesaryCount(usersLevelsInfo, StartingIndexOfShowingLevel, showingLevelsNumber);
            ClearContent();
            FillContent(countedUsersLevelsInfo);
            backwardButton.interactable = StartingIndexOfShowingLevel != 0;
            forwardButton.interactable = (StartingIndexOfShowingLevel + countedUsersLevelsInfo.Length) < usersLevelsInfo.Length;

            levelsIntervalText.text = $"{StartingIndexOfShowingLevel + 1} - {StartingIndexOfShowingLevel + showingLevelsNumber}";
        }


        private void OnQuitLevel()
        {
            GameController.Instance.IsUserLevel = false;
            Debug.Log($"IsUserLevel: {GameController.Instance.IsUserLevel} ");
            currentLevel = null;
        }



    }
}