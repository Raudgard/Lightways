using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Linq;
using Fields;
using Saving;
using Modes;
using PlayerIOClient;


public class GameController : MonoBehaviour
{
    #region Singleton
    private static GameController instance;
    public static GameController Instance
    {
        get { return instance; }
    }
    #endregion

    [SerializeField] private Settings settings;
    public Options options;
    public LanguageController languageController;
    public Achievements.AchievementsController achievementsController;
    public UserLevels.UserLevelsController userLevelsController;
    public ColorTheme colorTheme;

    public StateData StateData;

    public Settings Settings
    {
        get { return settings; }
    }


    [Space]

    [Tooltip("����������� ������ ��������� ����. ����������� �� ���� ����.")]
    [SerializeField] private List<Tower> spheresHighlighted;

    public List<Tower> SpheresHighlighted => spheresHighlighted;

    [Tooltip("����������� ������ ������������ �����. ����������� �� ���� ����. ��������� ��������� � ������ ������ ����.")]
    [SerializeField] private List<Beam> sendedBeams;

    public Beam LastBeamSended 
    { get
        {
            if (sendedBeams.Count > 0)
                return sendedBeams.Last();
            else return null;
        }
    }

    [Space]

    public List<Tower> finishTowers;

    [Space]

    [SerializeField] private SpriteRenderer backgroundSpriteRenderer;
    [SerializeField] private LogotypeController logotypeController;
    [SerializeField] private UIController uIController;
    private Tutorial tutorial;

    //[SerializeField] private ColorTheme colorTheme;

    public KeysController keysController;
    [SerializeField] private InputHandlers inputHandlers;
    [SerializeField] private SwipeableArea swipeableArea;
    public MusicController musicController;
    public UniversalAdditionalCameraData URPCameraData;
    
    [SerializeField] private Light2D directionalLight;
    public float DirectionalLightIntensity => directionalLight.intensity;
    public void SetDirectionalLightIntensity(float intensity) => directionalLight.intensity = intensity;

    [SerializeField] private RepassingLevelBar repassingLevelBar;
    [SerializeField] private UserLevels.UserLevelCreate userLevelCreateComponent;


    [Space]

    /// <summary>
    /// true - ���� � ������ ����� 2� �������� � ������������ � ������ ������ ������ ������� �������� ������.
    /// </summary>
    public bool waitingForPortalChoose = false;

    [Space]

    public Material purpleBackgroundMaterial;
    public Material greenBackgroundMaterial;
    public Material blueBackgroundMaterial;
    public Material cyanBackgroundMaterial;
    public Material redBackgroundMaterial;

    [Space]

    public Difficulty currentDifficulty;
    public int currentLevelNumber;

    [Tooltip("���������� ����, ������� ��� ��������� (� ������� ����������� �������� ���������).")]
    public int appearedSpheres = 0;


    /// <summary>
    /// �������� �� ������ ���� ������. ����������� � InputHandlers.
    /// </summary>
    public bool IsBeamLaunchAllowed { get; set; } = true;


    public Tower[] ActiveTowers => spheresHighlighted.Where(sphere => sphere.IsActive).ToArray();
    
    [Tooltip("�������������� ������� ����� �� �� ��������� ����. �� ������ �����������. ������ ������ �������� ������.")]
    [SerializeField] private List<Tower> falseActivatedTowers;

    /// <summary>
    /// ����������� �� ����� �� ������ ������ �������.
    /// </summary>
    public Level currentLevel;

    /// <summary>
    /// ���� ��������� �������� ������.
    /// </summary>
    public LevelStateData currentLevelStateData;

    [HideInInspector]
    public bool isStateLoaded = false;

    /// <summary>
    /// ���������� � ����� �������� ������.
    /// </summary>
    public Action onLevelLoaded;

    /// <summary>
    /// ���������� ��� ������ � ������.
    /// </summary>
    public Action onWinLevel;

    /// <summary>
    /// ���������� ��� ������� ����.
    /// </summary>
    public Action onPurchaseGame;



    ///// <summary>
    ///// ������, ���������� ������� �� ���� ������� �� ������ ����������� ������. ������� ����� ���, ��� � ����� ������ �������� ����� � ���� �����������, ����� � �������� � �������.
    ///// ��� ���������� �����, ����� �����������, ������� ������ ����� ����� ��������� �����. � ����� �� �� ��� �������� ��� ����� �� ��� ���������� ������.
    ///// </summary>
    //private int starsRecievedOnLevelLoad;

    private int starsTemp;

    /// <summary>
    /// ���������� ����� �� ��������� ������� �� ����������� ������ ��� ����������� ��������� ������. �� ���� ������� ������� ������ true. � ������ ������� ���������� true ����� ����,
    /// ��� ��� �������� ����� ���� ����������.
    /// </summary>
    private bool canReduceStars = true;

    /// <summary>
    /// ������� ���� ����, ���������� � ����������� �������. ���������� ��� ������������� ������� ����, ������������ ���������� �������.
    /// </summary>
    private ConcurrentDictionary<Tower, Vector2> towersPositions;

    /// <summary>
    /// ��� ������ ��� �������������?
    /// </summary>
    public bool IsDevelopVersion => Application.version.EndsWith("d");


    public string PlayerName => PlayerPrefs.GetString(SCFPP.Miscellaneous.playerName, "New player");


    /// <summary>
    /// ���������� ���������� ����� �� ������� ���������. ����� ���������� ����� � ������.
    /// </summary>
    public int StarsCount => StateData.StarsCount;

    /// <summary>
    /// ������� �� ����. ����� ������ ������ ����� �������� ������ �� State �����.
    /// </summary>
    public bool IsGamePurchased => StateData.IsGamePurchased;

    private Client clientPlayerIO = null;

    public Client ClientPlayerIO => clientPlayerIO;

    [SerializeField]
    private bool isDarkLevel = false;

    /// <summary>
    /// ��� ������� ��� �����������, ����������� � ����? ���� ������ ��� ���������� �������� ������� ���� FTL � BHR, ������� ��������� � ���� ��� �����������.
    /// </summary>
    public bool IsStoryLevel { get; set; } = true;
    

    /// <summary>
    /// ��� ����� �������? ��������������� ��� �������� ������.
    /// </summary>
    public bool IsDarkLevel => isDarkLevel;

    /// <summary>
    /// true �� ����� ����������������� �������������� ������, false � ��������� ����� (� ��� ����� � ��� �������� ������).
    /// </summary>
    public bool IsEditingLevel { get; set; } = false;

    /// <summary>
    /// true - �� ����� ������ �������� ������, ��� �������������, �������� � �.�. � ��������� ����� false.
    /// </summary>
    public bool IsCreateLevelMode { get; set; } = false;

    /// <summary>
    /// ������� � �������� �������� (�������� ������� �������� � �.�.)
    /// </summary>
    public bool IsLevelLoading { get; set; } = false;

    /// <summary>
    /// ������ �������� ������� ������� ������?
    /// </summary>
    public bool IsUserLevel { get; set; } = false;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this);
        }
        
    }

    private IEnumerator Start()
    {
        Application.targetFrameRate = 60;
        yield return StartCoroutine(LoadState());

        uIController.ShowStars();
        keysController.ChangeKeysCountText();
        tutorial = GetComponent<Tutorial>();

        SetCurrentDifficultyAndLevel();

        onWinLevel += WinLevel;
        //PurchaseGame();
        Debug.Log($"IsGamePurchased: {IsGamePurchased}");

        //Debug.Log($"streamingAssetsPath: {Application.streamingAssetsPath}");

        keysController.CheckInteractivityForModesGoButtons();
        if (clientPlayerIO == null)
        {
            Debug.Log($"client = null. Try to auth");
            CloudServices.LeaderBoardsFromPlayerIO.AuthenticationPlayerIO.AuthenticationUser(StateData.UserId, cl => clientPlayerIO = cl, delegate { });
        }

        options.showBHSizeToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(SCFPP.Options.showBHSize, 0) == 1);
        options.showBHAreaToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(SCFPP.Options.showBHArea, 0) == 1);
    }


    /// <summary>
    /// ����������� ���������� ����� � ������� ��������� �� ��������� ��������, �� �� �����, ��� �� 5!
    /// </summary>
    /// <param name="count"></param>
    public void IncreaseStarsCountBy(int count)
    {
        StateData.StarsCount += count;
        SaveState();
    }

    /// <summary>
    /// ��������� ������ ��������� � ����.
    /// </summary>
    public void SaveState() => SaveLoadUtil.SaveState(StateData);


    public IEnumerator LoadState()
    {
        StateData = SaveLoadUtil.LoadState();
        yield return null;
        isStateLoaded = true;
    }


    public void SkipLevel() => currentLevelStateData.isSkipped = true;
    public bool IsCurrentLevelPassed => currentLevelStateData.StarsRecieved > 0;


    /// <summary>
    /// ������������� ������� ������� ������ �� ����������.
    /// </summary>
    private void SetCurrentDifficultyAndLevel()
    {
        int passedLevel = PlayerPrefs.GetInt(SCFPP.Miscellaneous.levelsPassed, 0);
        currentLevelNumber = passedLevel + 1;
        currentDifficulty = (Difficulty)(PlayerPrefs.GetInt(SCFPP.Miscellaneous.difficultiesPassed, -1) + 1);
    }



    public void TowerActivated(Tower tower)
    {
        //���� ������������� directionalLight == 0, ������ ������� darkness, � ����� ��������� ���������� �� ������ ����, ����� ������ ����� ���� ��������.
        if(IsDarkLevel && !canReduceStars)
        {
            FindSpheresThatWereIlluminatedBySphere(tower);
            //Debug.Log($"highlighted towers: \n{TowersMatrix.Instance.Spheres.Where(t => t.winningWayIndex > -1).Where(t => t.IsIlluminatedAtLeastOnce).ToList().ToStringEverythingInARow()}");

            if (!tower.isFinish && TowersMatrix.Instance.Spheres.Where(t => t.winningWayIndex > -1).All(t => t.IsIlluminatedAtLeastOnce))
            {
                //Debug.Log($"��� �������� ����� ���� ����������!");
                uIController.ShowInfoOnBlack(languageController.allWinningSpheresWereHighlighted, 3);
                canReduceStars = true;
            }
        }

        if (spheresHighlighted.Count == 0)
        {
            spheresHighlighted.Add(tower);
            return;
        }

        if (spheresHighlighted.Last() != tower)
            spheresHighlighted.Add(tower);

        if (tower.isFinish)
        {
            if (IsCreateLevelMode)
            {
                userLevelCreateComponent.BeamHitFinishSphere(tower);
                return;
            }

            if (currentLevelStateData.levelType == LevelType.BlackHolesRiddles)
            {
                GetComponent<BlackHolesRiddlesController>().BeamHitWinSphere();
            }
            else
            {
                onWinLevel.Invoke();
            }

        }
    }

    public void TowerOff(Tower tower)
    {
        if (spheresHighlighted.Contains(tower))
            spheresHighlighted.Remove(tower);
    }

    public void BeamSended(Beam beam)
    {
        sendedBeams.Add(beam);
    }

    

    public void ReturnMove()
    {
        if (sendedBeams.Count == 0)
            return;

        if (waitingForPortalChoose)
            waitingForPortalChoose = false;

        LastBeamSended.ReturnMove();
        TowersMatrix.Instance.Teleports.ToList().ForEach(t => t.ActivateWaveFromTeleport(false));
        //SetActiveForSwipeableArea(true);
    }

    public void BeamWasCanselled(Beam beam)
    {
        if (sendedBeams.Contains(beam))
            sendedBeams.Remove(beam);
        else
            Debug.LogError("��� ����������� � ������");
    }

    public void ClearHighlightedTowers()
    {
        spheresHighlighted.Clear();
    }

    public void ClearSendedBeamsList()
    {
        sendedBeams.Clear();
        sendedBeams = new List<Beam>();
    }

    public void DestroyAllSendedBeams()
    {
        foreach (var beam in sendedBeams)
            if(beam != null) 
                Destroy(beam.gameObject);
    }

    /// <summary>
    /// ��������� ������� �� �����������, ������� �����.
    /// </summary>
    public IEnumerator LoadLevelFromFile(int difficulty, int level, bool hideStarsLabel, bool restartLevel = false)
    {
        IsStoryLevel = true;
        UIController.Instance.SetActiveForScreenCover(true);
        OnLevelLoad();
        yield return StartCoroutine(TowersMatrix.Instance.ClearHUBs());

        UIController.Instance.SetActiveMainMenuObjects(false, hideStarsLabel);
        UIController.Instance.ActivateScreenOtherDeactivate(ActiveScreen.Game);
        //uIController.SetLevelNameToLabel($"{languageController.GetDifficultyName((Difficulty)(difficulty - 1))}: {level}");
        yield return StartCoroutine(TowersMatrix.Instance.LoadLevelFromFile($"{difficulty}_{level}"));
        tutorial.StoryLevelLoading((Difficulty)(difficulty - 1), level);

        //isDarkLevel = DirectionalLightIntensity > 0 ? false : true;
        currentLevelStateData = StateData.GetLevelStateOrAddNew((Difficulty)(difficulty - 1), level, currentLevel.levelType);
        currentLevelStateData.HasPenaltyTimeExpired();
        if (!IsGamePurchased)
        {
            repassingLevelBar.Intitialize();
        }
        SetStarsTempDependingFromPenaltyTimeExpired();

        StartCoroutine(LoadCustomLevel(currentLevelStateData, hideStarsLabel, restartLevel));
        


        //inputHandlers.OnLevelLoad();
        //canReduceStars = !IsDarkLevel;
        //StartCoroutine(LevelObjectsAppearance());
    }


    /// <summary>
    /// ��������� ���� �������. 
    /// </summary>
    /// <param name="levelStateData"></param>
    public IEnumerator LoadCustomLevel(LevelStateData levelStateData, bool hideStarsLabel, bool restartLevel)
    {
        OnLevelLoad();

        if (IsStoryLevel)
        {
            uIController.SetLevelNameToLabel($"{languageController.GetDifficultyName(levelStateData.difficulty)}: {levelStateData.levelNumber}");
        }
        else
        {
            uIController.SetLevelNameToLabel($"{languageController.GetLevelTypeName(levelStateData.levelType)}: {levelStateData.StarsMax}");
        }

        if (restartLevel)
        {
            yield return StartCoroutine(TowersMatrix.Instance.ClearHUBs());
            yield return StartCoroutine(TowersMatrix.Instance.CreatePlayObjectsFromMatrix());
        }

        UIController.Instance.SetActiveMainMenuObjects(false, hideStarsLabel);

        currentLevelStateData = levelStateData;
        starsTemp = currentLevelStateData.starsMaxOnThisSession;
        //repassingLevelBar.Intitialize();
        inputHandlers.OnLevelLoad();
        isDarkLevel = DirectionalLightIntensity > 0 ? false : true;

        canReduceStars = !IsDarkLevel;

        if (levelStateData.levelType == LevelType.BlackHolesRiddles && !GetComponent<BlackHolesRiddlesController>().IsShowingCorrrectPlacing)
        {
            TowersMatrix.Instance.BlackHoles.ToList().ForEach(bh => bh.gameObject.SetActive(false));
        }
        musicController.LevelStartingPlay();
        StartCoroutine(LevelObjectsAppearance());
    }

    /// <summary>
    /// ��������, ����������� ��� �������� ������ ������: �� ����� ��� ������������.
    /// </summary>
    private void OnLevelLoad()
    {
        towersPositions = null;
        IsLevelLoading = true;

        ClearSendedBeamsList();
        //uIController.currentScreen = ActiveScreen.Game;
        URPCameraData.renderPostProcessing = false;
        falseActivatedTowers.Clear();
        falseActivatedTowers = new List<Tower>();
        IsBeamLaunchAllowed = false;
        appearedSpheres = 0;
    }


    private IEnumerator LevelObjectsAppearance()
    {
        SetDirectionalLightIntensity(1);
        UIController.Instance.SetActiveForScreenCover(true);
        var spheres = new List<Tower>(TowersMatrix.Instance.Spheres);
        //Debug.Log($"spheres.Count: {spheres.Count}");
        spheres.ForEach(s => { s.SetActiveForAllVisibleThings(false); s.SetActiveForArrows(false); });
        int countSpheres = TowersMatrix.Instance.Matrix.TowersCount;
        //Debug.Log($"countSpheres: {countSpheres}");

        var blackHoles = new List<BlackHole>(TowersMatrix.Instance.BlackHoles);
        blackHoles.ForEach(bh => bh.SetSizeAreaAndLetterTransparent(true));

        //blackHoles.ForEach(bh => { Debug.Log($"blackHoles.Count: {blackHoles.Count}, bh.transform.position: {bh.transform.position}"); });


        var teleports = new List<Teleport>(TowersMatrix.Instance.Teleports);
        teleports.ForEach(t => t.SetActiveForSpriteRenderer(false));

        float minTime = settings.minTimeBeforeNextTowerAppearance;
        float maxTime = settings.maxTimeBeforeNextTowerAppearance;
        float normalSphereCount = settings.normalCountSphereInMatrixForTimeBeforeNextTowerAppearance;

        if (IsDarkLevel)
        {
            StartCoroutine(FadingDirectionalLight());
        }

        while (spheres.Count > 0)
        {
            var sphere = spheres.GetRandomNext();
            //StartCoroutine(sphere.SphereAppearance());
            sphere.SphereAppearance();
            StartCoroutine(sphere.ArrowsAppearance());
            StartCoroutine(sphere.BackgroundSmokeAppearance());
            if (sphere.IsStart)
            {
                StartCoroutine(WaitingStartSphereForDirectionalLightFading(sphere));
            }

            yield return new WaitForSeconds(UnityEngine.Random.Range(minTime, maxTime) * (normalSphereCount/ countSpheres));
        }

        while (teleports.Count > 0)
        {
            var teleport = teleports.GetRandomNext();
            teleport.Appearance();

            yield return new WaitForSeconds(UnityEngine.Random.Range(minTime, maxTime) * (normalSphereCount / countSpheres));
        }

        while (blackHoles.Count > 0)
        {
            //Debug.Log($"blackHoles.Count: {blackHoles.Count}");
            var bh = blackHoles.GetRandomNext();
            if (bh == null)
            {
                Debug.Log($"bh == NULL!");
                continue;
            }

            bh.Appearance();
            bh.CircleOfAttractionAreaAppearance();
            bh.LettersAppearance();



            yield return new WaitForSeconds(UnityEngine.Random.Range(minTime, maxTime) * (normalSphereCount / countSpheres) * 3);
        }

        var spheresCount = TowersMatrix.Instance.Spheres.Count();

        while (appearedSpheres != spheresCount)
        {
            yield return null;
        }

        //Debug.Log($"TowersMatrix.Instance.Spheres.Length: {TowersMatrix.Instance.Spheres.Length}");
        //TowersMatrix.Instance.Spheres.Single(s => s.IsStart).ActivateStartTower();
        IsBeamLaunchAllowed = true;
        IsLevelLoading = false;
        UIController.Instance.SetActiveForScreenCover(false);

        if (currentLevelStateData.levelType == LevelType.FasterThanLight)
        {
            GetComponent<FasterThanLightController>().Initialize((IsStoryLevel && currentLevel != null) ? currentLevel.timeForFTLlevel : -1);
        }
        else if (currentLevelStateData.levelType == LevelType.BlackHolesRiddles)
        {
            GetComponent<BlackHolesRiddlesController>().Initialize();
        }

        onLevelLoaded?.Invoke();
    }

    /// <summary>
    /// ������� �������� ������� � ALC � ����� ��������� ��������� ������� �� ��������� �������.
    /// </summary>
    /// <param name="levelStateData"></param>
    public void WaitForCreatingMatrixForModesLevel(LevelStateData levelStateData)
    {
        IsStoryLevel = false;
        IsLevelLoading = true;
        UIController.Instance.SetActiveMainMenuObjects(false, false);
        UIController.Instance.ActivateScreenOtherDeactivate(ActiveScreen.Game);
        UIController.Instance.SetActiveForScreenCover(true);

        StartCoroutine(WaitingForCreatingMatrix(levelStateData));
    }

    private IEnumerator WaitingForCreatingMatrix(LevelStateData levelStateData)
    {
        var towersMatrix = TowersMatrix.Instance;
        while (towersMatrix.isCreating)
        {
            yield return null;
        }
        //yield return new WaitForSeconds(1);
        
        StartCoroutine(LoadCustomLevel(levelStateData, true, false));
    }



    /// <summary>
    /// ���������� ������������� Directional Light � 1 �� 0.
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadingDirectionalLight()
    {
        var speed = Settings.directionalLightFadingSpeed;
        while (directionalLight.intensity > 0)
        {
            directionalLight.intensity -= Time.fixedDeltaTime * speed;
            yield return null;
        }
    }

    /// <summary>
    /// �������� ��������� ������ ��������� ����������� ����� �� ������������ ������, ����� ������ ����������.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitingStartSphereForDirectionalLightFading(Tower startSphere)
    {
        var value = Settings.directionalLightIntensityValueWhenStartSphereMustKindling;
        if (IsDarkLevel)
        {
            while (directionalLight.intensity > value)
            {
                yield return null;
            }
        }

        StartCoroutine(startSphere.LightSlowKindlingInStartTower());
    }






    /// <summary>
    /// ������������� ���������� ������������� ���������� �������� ����� � ������ ������ � ����������� �� ��������� ��� ��� ������� ��������.
    /// </summary>
    public void SetStarsTempDependingFromPenaltyTimeExpired()
    {
        starsTemp = IsGamePurchased ? currentLevelStateData.StarsMax : currentLevelStateData.starsMaxOnThisSession;
    }


    /// <summary>
    /// ��� ��������� ������ �����.
    /// </summary>
    /// <param name="tower">������ �����.</param>
    public void FalseTowerActivated(Tower tower)
    {
        if (canReduceStars && !falseActivatedTowers.Contains(tower))
        {
            falseActivatedTowers.Add(tower);

            if (falseActivatedTowers.Count % 2 == 0 && currentLevelStateData.StarsMax > 1 && currentLevelStateData.StarsRecieved < currentLevelStateData.starsMaxOnThisSession)
            {
                //Debug.Log($"minus star! time: {DateTime.Now}");
                starsTemp--;
                if (starsTemp < 1)
                    starsTemp = 1;

                if(currentLevelStateData.HasPenaltyTimeExpired())
                {
                    //Debug.Log($"time is up from previous wrongs: {currentLevelStateData.incompleteTime}");
                    currentLevelStateData.incompleteTime = DateTime.Now;
                }

                currentLevelStateData.starsMaxOnThisSession--;
                if (currentLevelStateData.starsMaxOnThisSession < 1)
                    currentLevelStateData.starsMaxOnThisSession = 1;

                SaveState();
            }
        }
    }




    /// <summary>
    /// ���������� ��� ������.
    /// </summary>
    private void WinLevel()
    {
        print("WIN!!!");

        if(currentLevelStateData.levelNumber == Settings.levelsCountInEachDifficulty)
        {
            uIController.SetActiveForWinLabelNextLevelButton(false);
        }
        else
        {
            uIController.SetActiveForWinLabelNextLevelButton(true);
        }

        if (currentLevelStateData.isSkipped)
        {
            currentLevelStateData.isSkipped = false;
            StateData.levelsSkipped--;
        }


        int starsWin = starsTemp - currentLevelStateData.StarsRecieved;
        Debug.Log($"starsTemp: {starsTemp}, currentLevelStateData.StarsRecieved: {currentLevelStateData.StarsRecieved}, starsWin: {starsWin}");
        currentLevelStateData.StarsRecieved = starsTemp;

        if(IsUserLevel)
        {
            Debug.Log($"currentLevelStateData.levelNumber: {currentLevelStateData.levelNumber}");
            if (StateData.HasThisLevelAlreadyBeenPassed((uint)currentLevelStateData.levelNumber))
            {
                starsWin = 0;
                currentLevelStateData.StarsRecieved = 0;
            }
        }

        IncreaseStarsCountBy(starsWin);
        UIController.Instance.isNeedToTranslateWinStars = starsWin > 0;
        uIController.ShowWinLabel(currentLevelStateData);

        if(IsUserLevel)
        {
            StateData.UserLevelHasBeenCompleted((uint)currentLevelStateData.levelNumber);
            SaveState();
        }

        musicController.WinLevelSoundPlay();

        int winLevelsCount = PlayerPrefs.GetInt(SCFPP.Miscellaneous.winLevelsCount, 0);
        PlayerPrefs.SetInt(SCFPP.Miscellaneous.winLevelsCount, ++winLevelsCount);

        SaveResultInDatabase();
        //uIController.SetInteractibleForNormalLevelsButton();
    }


    /// <summary>
    /// ��������� ������� ���������� ������ � ���� ������ PlayerIO.
    /// </summary>
    public void SaveResultInDatabase()
    {
        if (clientPlayerIO == null)
        {
            CloudServices.LeaderBoardsFromPlayerIO.AuthenticationPlayerIO.AuthenticationUser(StateData.UserId, (cl) =>
            {
                clientPlayerIO = cl;
                clientPlayerIO.BigDB.LoadOrCreate("Stars", clientPlayerIO.ConnectUserId, dbo =>
                {
                    dbo.Set("Name", PlayerName);
                    dbo.Set("StarsNumber", StarsCount);
                    dbo.Save();
                    //currentPlayerDatabaseObject = dbo;
                });
            }, a => { });
        }
    }







    /// <summary>
    /// ������ ��������� �������� ������.
    /// </summary>
    /// <returns></returns>
    public IEnumerator ShowProgressCreatingLevel(int createdObjects, int countOfAllObjects)
    {
        uIController.SetValuesForCreatingLevelBar(createdObjects, countOfAllObjects);
        yield return null;
    }

    public void SetActiveForSwipeableArea(bool active)
    {
        if (active)
        {
            swipeableArea.gameObject.SetActive(true);
            swipeableArea.OnSetActive();
        }
        else
        {
            swipeableArea.SetDeactive();
        }
    }

    /// <summary>
    /// ������� �����, ������� ���� �������� �������� ������������ ������.
    /// </summary>
    /// <param name="towerGlowing">������������ �����.</param>
    private void FindSpheresThatWereIlluminatedBySphere(Tower towerGlowing)
    {
        if (towersPositions == null)
        {
            towersPositions = new ConcurrentDictionary<Tower, Vector2>();
            var towers = TowersMatrix.Instance.Spheres;
            for (int i = 0; i < towers.Length; i++)
            {
                towersPositions.TryAdd(towers[i], towers[i].transform.position);
            }
        }

        ConcurrentDictionary<Tower, float> dict = new ConcurrentDictionary<Tower, float>();
        var towerGlowingPosition = (Vector2)towerGlowing.transform.position;

        towersPositions.AsParallel().ForAll((towerPosition) =>
        {
            dict.TryAdd(towerPosition.Key, (towerGlowingPosition - towerPosition.Value).sqrMagnitude);
        });

        float lightCircleRadiusSqr = towerGlowing.LightRange * towerGlowing.LightRange * settings.coeffSqrForLightRangeSqr;
        //Debug.Log($"towerGlowing.LightRange: {towerGlowing.LightRange}, lightCircleRadiusSqr: {lightCircleRadiusSqr}");

        foreach (var sqrDist in dict)
        {
            //�������� �� 2, ������ ��� �������� ����� 2D ���� �������� ��������� �������� � ������� �������� � 2 ���� �������, ��� ������������ � ��������� �������� outer
            // ��� ��������� inner ������ 0. � � ���� ��� ��� ��� � ����.
            if (sqrDist.Value * 2 < lightCircleRadiusSqr) 
            {
                sqrDist.Key.IsIlluminatedAtLeastOnce = true;
                //Debug.Log($"distSqr: {sqrDist.Value}, sphere: {sqrDist.Key}, lightCircleRadiusSqr: {lightCircleRadiusSqr}");
            }
        }


    }


    /// <summary>
    /// ��������� ���������� ������ ��� ������� �� 1.
    /// </summary>
    public void KeySpent(int spentCount)
    {
        if (IsGamePurchased)
            return;

        StateData.KeysForModesCount = StateData.KeysForModesCount > spentCount ? StateData.KeysForModesCount - spentCount : 0;
        SaveState();
        keysController.ChangeKeysCountText();
        keysController.CheckInteractivityForModesGoButtons();
    }

    /// <summary>
    /// ����������� ���������� ������ ��� �������. �� �� �����, ��� �� 20!
    /// </summary>
    /// <param name="keysCount"></param>
    public void AddKeys(int keysCount)
    {
        StateData.KeysForModesCount += keysCount;
        SaveState();
        keysController.ChangeKeysCountText();
        keysController.CheckInteractivityForModesGoButtons();
    }



    public void ChangePlayerName(string name)
    {
        PlayerPrefs.SetString(SCFPP.Miscellaneous.playerName, name);
    }




    /// <summary>
    /// ��� ������� ����.
    /// </summary>
    public void PurchaseGame()
    {
        StateData.GameWasPurchased();
        AddKeys(1); //��������� 1 ���� �� ������, ���� ���� 0. ����� ������ ��������������.
        SaveState();
        if(onPurchaseGame != null)
            onPurchaseGame.Invoke();
    }


}
