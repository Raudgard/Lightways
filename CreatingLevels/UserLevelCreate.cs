using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using TMPro;
using System.Linq;
using Tools;
using Fields;
using System.Collections;
using Saving;
using UnityEngine.Events;

namespace UserLevels
{
    public class UserLevelCreate : MonoBehaviour
    {
        private enum ActiveUnit
        {
            Sphere,
            SphereStart,
            SphereFinish,
            Teleport,
            BlackHoleSmall,
            BlackHoleMedium,
            BlackHoleLarge,
            BlackHoleSuperMassive
        }


        [SerializeField] private Slider sizeSlider;
        [SerializeField] private GameObject levelObjectsPlace;
        [SerializeField] private GameObject createLevelMenu;
        [SerializeField] private GameObject createdLevelsLabel;
        [Tooltip("����� ����� ������. ������������ ������ ������� ShowTextInputDialog.")]
        [SerializeField] private GameObject textInputLabel;
        [SerializeField] private GameObject darkCover;
        [SerializeField] private GameObject winWaysCountLabel;
        [Tooltip("��������� \"��� �������\".")]
        [SerializeField] private GameObject howToCreateLabel;
        [SerializeField] private TextMeshProUGUI winWaysCountTMP;
        [SerializeField] private TMP_InputField textInputField;
        [SerializeField] private UniversalAdditionalCameraData URPCameraData;
        [SerializeField] private InputHandlers inputHandlers;
        [SerializeField] private Button returnButton;
        [SerializeField] private Button menuButton;
        [Tooltip("������ OK �� ������ ����� ������.")]
        [SerializeField] private Button okButtonOnTextInputDialog;
        [Tooltip("������ ���������� ����� ����������� ������.")]
        [SerializeField] private Button publishButtonWhenPassingLevel;
        [Tooltip("������ ������ ����� ����������� ������.")]
        [SerializeField] private Button backButtonWhenPassingLevel;

        [SerializeField] private LevelAnalyzer analyzer;



        [Space]

        [SerializeField] private Image sphereButtonBorder;
        [SerializeField] private Image TeleportButtonBorder;
        [SerializeField] private Image BlackHoleButtonBorder;
        [SerializeField] private Image sphereImage;
        [SerializeField] private Image BlackHoleImage;
        [SerializeField] private TextMeshProUGUI BlackHoleLetter;
        [SerializeField] private GameObject optionsLabel;
        public Toggle inDarkneesToggle;
        [SerializeField] private Button playLevelButton;

        [Space]

        [SerializeField] private Sprite sphereSprite;
        [SerializeField] private Sprite sphereStartSprite;
        [SerializeField] private Sprite sphereFinishSprite;

        [Space]

        [SerializeField] private Sprite smallBH;
        [SerializeField] private Sprite mediumBH;
        [SerializeField] private Sprite largeBH;
        [SerializeField] private Sprite superMassiveBH;

        [Space]


        [SerializeField] private string levelName = string.Empty;

        public string LevelName { get { return levelName; } private set { levelName = value; } }

        [Space]

        [Tooltip("������ ����� ��������� � ��������� ����� � �� ��������� ������.")]
        public float secondsAfterFinish;

        [Tooltip("������ ��� ����������. ���� ���������, �� ����������� ������� ����������.")]
        public float secondsTimeoutWhenPublishing;

        private Matrix Matrix { get; set; }
        private int xSize;
        private int ySize;

        private ActiveUnit activeUnit = ActiveUnit.Sphere;
        private ActiveUnit BHTempActiveUnit = ActiveUnit.BlackHoleSmall;
        private ActiveUnit sphereTempActiveUnit = ActiveUnit.Sphere;


        private Rect optionsLabelRect;
        private Rect menuButtonRect;

        ///// <summary>
        ///// ������, ������� ��, �.�. �� 8 ���� ������ ������ ��.
        ///// </summary>
        //private Dictionary<BlackHoleInfo, (int X, int Y)[]> lockedSells = new Dictionary<BlackHoleInfo, (int X, int Y)[]>();

        /// <summary>
        /// ������, ������� ��, �.�. �� 8 ���� ������ ������ ��.
        /// </summary>
        private (int X, int Y)[] LockedSells
        {
            get
            {
                var res = new List<(int X, int Y)>();
                var blackHoles = TowersMatrix.Instance.BlackHoles;
                foreach (var bh in blackHoles)
                {
                    int X = (int)bh.transform.position.x;
                    int Y = (int)bh.transform.position.y;

                    res.Add((X, Y + 1));
                    res.Add((X - 1, Y + 1));
                    res.Add((X - 1, Y));
                    res.Add((X - 1, Y - 1));
                    res.Add((X, Y - 1));
                    res.Add((X + 1, Y - 1));
                    res.Add((X + 1, Y));
                    res.Add((X + 1, Y + 1));
                }
                return res.ToArray();
            }
        }




        //private bool IsMarkingSphereAsStart { get; set; } = false;
        //private bool IsMarkingSphereAsFinish { get; set; } = false;
        private bool IsLevelMenuButtonClick { get; set; } = false;

        

        private GameController gameController;
        private Coroutine analyzingCoroutine = null;
        private Tower hittenFinishTower = null;
        private bool successfulAddedInFirstTableOnPIO = false;
        private bool successfulAddedInSecondTableOnPIO = false;


        public UnityAction actionOnOkButtonTextInputFieldClick;


        private void Start()
        {
            gameController = GameController.Instance;

            SetRects();
            //SetTextToEditLevelButton();
            UIController.Instance.onQuitLevel += OnQuitLevel;
            UIController.Instance.onLevelMenuOpen += delegate { optionsLabel.SetActive(false); };

            textInputField.onValidateInput += ValidateLevelName;

            //levelInputField.onValidateInput += delegate (string input, int charIndex, char addedChar) { return SCFPP.ValidateDigit(addedChar); };

        }


        /// <summary>
        /// ������������� ������������� �������, ������� � ������� ������� ������������� ��������.
        /// </summary>
        private void SetRects()
        {
            var corners = new Vector3[4];
            optionsLabel.GetComponent<RectTransform>().GetWorldCorners(corners);
            optionsLabelRect = new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);

            corners = new Vector3[4];
            menuButton.GetComponent<RectTransform>().GetWorldCorners(corners);
            menuButtonRect = new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
        }


        

        /// <summary>
        /// ��������, ����������� ����� ������� ������ �������� ���� �������� �������������� ������.
        /// </summary>
        public IEnumerator EnterCreateLevelMode()
        {
            gameController.IsLevelLoading = true;
            gameController.IsBeamLaunchAllowed = false;
            gameController.IsCreateLevelMode = true;
            gameController.SetDirectionalLightIntensity(1);
            UIController.Instance.SetActiveMainMenuObjects(false);
            UIController.Instance.SetActiveForSkipLevelButtonGameObject(false);
            createLevelMenu.SetActive(false);
            levelObjectsPlace.SetActive(true);
            returnButton.gameObject.SetActive(false);
            UIController.Instance.ActivateScreenOtherDeactivate(ActiveScreen.Game);

            playLevelButton.gameObject.SetActive(true);

            URPCameraData.renderPostProcessing = false;
            var towersMatrix = TowersMatrix.Instance;
            yield return StartCoroutine(towersMatrix.CreatePlayObjectsFromMatrix());


            //lockedSells.Clear();
            //lockedSells = new Dictionary<BlackHoleInfo, (int X, int Y)[]>();
            //var blackHoles = Matrix.BlackHoles;
            //foreach (var bh in blackHoles)
            //    AddLockedSells(bh);


            //towersMatrix.Spheres.Where(s => s.IsStart).ToList().ForEach(s => s.ActivateStartTower());
            towersMatrix.Spheres.Where(s => s.isFinish).ToList().ForEach(s => s.GetComponentInChildren<ParticleSystem>().Play());
            towersMatrix.Teleports.ToList().ForEach(t => t.GetComponentInChildren<ParticleSystem>().Play());

            var spheres = TowersMatrix.Instance.Spheres.Where(s => s.IsStart);
            //Debug.Log($"count: {TowersMatrix.Instance.Spheres.Count()}, start sphere count: {TowersMatrix.Instance.Spheres.Where(s => s.IsStart).Count()}");

            if (spheres.Count() > 0) spheres.SingleOrDefault().TurnOnOffLight(true);

            //gameController.directionalLight.intensity = 1;
            gameController.IsEditingLevel = true;
            SetTextToEditLevelButton();
            inputHandlers.OnLevelLoad();

            UnityTools.ExecuteWithDelay(() => { GameController.Instance.IsLevelLoading = false; }, 3);
        }


        

        public void SphereUnitButtonClick()
        {
            //Debug.Log("Sphere Unit Button Click");

            sphereButtonBorder.gameObject.SetActive(true);
            TeleportButtonBorder.gameObject.SetActive(false);
            BlackHoleButtonBorder.gameObject.SetActive(false);

            SetNextSphereUnit();
        }

        private void SetNextSphereUnit()
        {
            switch (activeUnit)
            {
                case ActiveUnit.BlackHoleSmall:
                case ActiveUnit.BlackHoleMedium:
                case ActiveUnit.BlackHoleLarge:
                case ActiveUnit.BlackHoleSuperMassive:
                case ActiveUnit.Teleport:
                    activeUnit = sphereTempActiveUnit;
                    break;

                case ActiveUnit.Sphere:
                    SetActiveUnit(ActiveUnit.SphereStart);
                    break;

                case ActiveUnit.SphereStart:
                    SetActiveUnit(ActiveUnit.SphereFinish);
                    break;

                case ActiveUnit.SphereFinish:
                    SetActiveUnit(ActiveUnit.Sphere);
                    break;

                default: throw new System.NotImplementedException();
            }
        }


        public void TeleportUnitButtonClick()
        {
            //Debug.Log("Teleport Unit Button Click");

            sphereButtonBorder.gameObject.SetActive(false);
            TeleportButtonBorder.gameObject.SetActive(true);
            BlackHoleButtonBorder.gameObject.SetActive(false);

            activeUnit = ActiveUnit.Teleport;
        }


        public void BlackHoleUnitButtonClick()
        {
            //Debug.Log("Black Hole Unit Button Click");

            sphereButtonBorder.gameObject.SetActive(false);
            TeleportButtonBorder.gameObject.SetActive(false);
            BlackHoleButtonBorder.gameObject.SetActive(true);

            SetNextBlackHoleUnit();
        }



        private void SetNextBlackHoleUnit()
        {
            switch(activeUnit)
            {
                case ActiveUnit.Sphere:
                case ActiveUnit.SphereStart:
                case ActiveUnit.SphereFinish:
                case ActiveUnit.Teleport:
                    activeUnit = BHTempActiveUnit;
                    break;

                case ActiveUnit.BlackHoleSmall:
                    SetActiveUnit(ActiveUnit.BlackHoleMedium);
                    break;

                case ActiveUnit.BlackHoleMedium:
                    SetActiveUnit(ActiveUnit.BlackHoleLarge);
                    break;

                case ActiveUnit.BlackHoleLarge:
                    SetActiveUnit(ActiveUnit.BlackHoleSuperMassive);
                    break;

                case ActiveUnit.BlackHoleSuperMassive:
                    SetActiveUnit(ActiveUnit.BlackHoleSmall);
                    break;

                default: throw new System.NotImplementedException();
            }
        }


        private void SetActiveUnit(ActiveUnit _activeUnit)
        {
            switch (_activeUnit)
            {
                case ActiveUnit.Sphere:
                    sphereImage.sprite = sphereSprite;
                    activeUnit = sphereTempActiveUnit = _activeUnit;
                    break;

                case ActiveUnit.SphereStart:
                    sphereImage.sprite = sphereStartSprite;
                    activeUnit = sphereTempActiveUnit = _activeUnit;
                    break;

                case ActiveUnit.SphereFinish:
                    sphereImage.sprite = sphereFinishSprite;
                    activeUnit = sphereTempActiveUnit = _activeUnit;
                    break;

                case ActiveUnit.Teleport:
                    activeUnit = _activeUnit;
                    break;

                case ActiveUnit.BlackHoleSmall:
                    BlackHoleImage.sprite = smallBH;
                    BlackHoleLetter.text = "S";
                    activeUnit = BHTempActiveUnit = _activeUnit;
                    break;

                case ActiveUnit.BlackHoleMedium:
                    BlackHoleImage.sprite = mediumBH;
                    BlackHoleLetter.text = "M";
                    activeUnit = BHTempActiveUnit = _activeUnit;
                    break;

                case ActiveUnit.BlackHoleLarge:
                    BlackHoleImage.sprite = largeBH;
                    BlackHoleLetter.text = "L";
                    activeUnit = BHTempActiveUnit = _activeUnit;
                    break;

                case ActiveUnit.BlackHoleSuperMassive:
                    BlackHoleImage.sprite = superMassiveBH;
                    BlackHoleLetter.text = "SM";
                    activeUnit = BHTempActiveUnit = _activeUnit;
                    break;

                default: throw new System.NotImplementedException();
            }

        }



        public void OnClick(Vector2 touchOrLeftMouseClickPosition)
        {
            if (!gameController.IsEditingLevel ||
                gameController.IsLevelLoading ||
                createdLevelsLabel.activeSelf ||
                UIController.Instance.currentScreen != ActiveScreen.Game ||
                IsClickIntoUnitPlace(touchOrLeftMouseClickPosition) ||
                IsClickOnMenuButton(touchOrLeftMouseClickPosition) ||
                IsLevelMenuButtonClick)
            {
                return;
            }

            if (optionsLabel.activeSelf)
            {
                if (IsClickOnOptionsLabels(touchOrLeftMouseClickPosition))
                    return;
                else
                {
                    optionsLabel.SetActive(false);
                    return;
                }
            }

            Vector2 nearestWholeCoordinate = Calculations.NearestWholeCoordinate(touchOrLeftMouseClickPosition);
            (int X, int Y) coordinate = ((int)nearestWholeCoordinate.x, (int)nearestWholeCoordinate.y);
            //Debug.Log($"nearestWholeCoordinate: {coordinate}");
            if (IsThereUnitInMatrix(coordinate.X, coordinate.Y, out MatrixUnit unit))
            {
                //Debug.Log($"� ��� ����� ���� ������: {unit}");
                OnObjectClick(unit);
            }
            else
            {
                //Debug.Log($"� ��� ����� ������� ���.");
                if (IsPositionAllowed(coordinate.X, coordinate.Y))
                {
                    Debug.Log($"������� ���������.");
                    CreateObject(coordinate.X, coordinate.Y, out _);
                }
                else
                {
                    Debug.Log($"������� ���������!");

                }
            }
        }



        /// <summary>
        /// ���������, ��������� �� ���������� ����� ��� ������ ��� ������ (����� ������).
        /// </summary>
        /// <returns></returns>
        private bool IsClickIntoUnitPlace(Vector2 clickCoordinate)
        {
            return clickCoordinate.y < levelObjectsPlace.GetComponent<RectTransform>().rect.height;
        }

        /// <summary>
        /// ���������, ��������� �� ���������� ����� ��� �������� �����.
        /// </summary>
        /// <returns></returns>
        private bool IsClickOnOptionsLabels(Vector2 clickCoordinate)
        {
            return optionsLabelRect.Contains(clickCoordinate);
        }

        /// <summary>
        /// ���������, ��������� �� ���������� ����� ��� ������� ����.
        /// </summary>
        /// <returns></returns>
        private bool IsClickOnMenuButton(Vector2 clickCoordinate)
        {
            return menuButtonRect.Contains(clickCoordinate);
        }


        /// <summary>
        /// ���� �� �� ������ ������ � ������� �� ���� ����������� ������?
        /// </summary>
        /// <param name="X">����������� � ����������.</param>
        /// <param name="Y">����������� Y ����������.</param>
        /// <param name="matrixUnit">����������� �� ���� ����� ������ � �������.</param>
        /// <returns></returns>
        private bool IsThereUnitInMatrix(int X, int Y, out MatrixUnit matrixUnit)
        {
            matrixUnit = null;
            if (X < 0 || X > Matrix.SizeX - 1 || Y < 0 || Y > Matrix.SizeY - 1)
            {
                return false;
            }

            matrixUnit = Matrix[X, Y];
            if (matrixUnit != null)
            {
                return true;
            }
            else return false;
        }



        private bool IsPositionAllowed(int X, int Y)
        {
            if (X < 0 || X >= Matrix.SizeX || Y < 0 || Y >= Matrix.SizeY)
                return false;

            List<(int X, int Y)> directPointsToCheck = new List<(int X, int Y)>();
            List<(int X, int Y)> cornersPointsToCheck = new List<(int X, int Y)>();

            directPointsToCheck.Add((X, Y));

            if (activeUnit == ActiveUnit.BlackHoleLarge ||
                activeUnit == ActiveUnit.BlackHoleMedium ||
                activeUnit == ActiveUnit.BlackHoleSmall ||
                activeUnit == ActiveUnit.BlackHoleSuperMassive)
            {
                directPointsToCheck.Add((X, Y + 1));
                directPointsToCheck.Add((X - 1, Y));
                directPointsToCheck.Add((X, Y - 1));
                directPointsToCheck.Add((X + 1, Y));

                cornersPointsToCheck.Add((X + 1, Y + 1));
                cornersPointsToCheck.Add((X + 1, Y - 1));
                cornersPointsToCheck.Add((X - 1, Y + 1));
                cornersPointsToCheck.Add((X - 1, Y - 1));

                if (X == 0)
                {
                    directPointsToCheck.Remove((X - 1, Y));
                    cornersPointsToCheck.Remove((X - 1, Y - 1));
                    cornersPointsToCheck.Remove((X - 1, Y + 1));
                }

                if (X == Matrix.SizeX - 1)
                {
                    directPointsToCheck.Remove((X + 1, Y));
                    cornersPointsToCheck.Remove((X + 1, Y - 1));
                    cornersPointsToCheck.Remove((X + 1, Y + 1));
                }

                if (Y == 0)
                {
                    directPointsToCheck.Remove((X, Y - 1));
                    cornersPointsToCheck.Remove((X - 1, Y - 1));
                    cornersPointsToCheck.Remove((X + 1, Y - 1));
                }

                if (Y == Matrix.SizeY - 1)
                {
                    directPointsToCheck.Remove((X, Y + 1));
                    cornersPointsToCheck.Remove((X - 1, Y + 1));
                    cornersPointsToCheck.Remove((X + 1, Y + 1));
                }
            }

            IEnumerable<(int x, int y)> cells = Matrix.GetAllUnits().Select(u => (u.X, u.Y));
            //Debug.Log($"1. cells.Count: {cells.Count()}");

            //var _lockedSells = lockedSells.SelectMany(bh => bh.Value);
            //Debug.Log($"2. _lockedSells.Count: {_lockedSells.Count()}");

            //var checkingCells = cells.Concat(_lockedSells);
            var checkingCells = cells.Concat(LockedSells);

            //Debug.Log($"3. checkingCells.Count: {checkingCells.Count()}");
            return !(checkingCells.Intersect(directPointsToCheck).Count() > 0) && !(cells.Intersect(cornersPointsToCheck).Count() > 0);
        }


        private PlayObject CreateObject(int X, int Y, out MatrixUnit matrixUnit)
        {
            PlayObject playObject;

            switch (activeUnit)
            {
                case ActiveUnit.Sphere:
                    matrixUnit = Matrix.AddTower(X, Y, -1, false);
                    playObject = TowersMatrix.Instance.CreatePlayObject<Tower>(matrixUnit); 
                    break;

                case ActiveUnit.SphereStart:
                    matrixUnit = Matrix.AddTower(X, Y, -1, false);
                    playObject = TowersMatrix.Instance.CreatePlayObject<Tower>(matrixUnit);
                    OnObjectClick(matrixUnit);
                    break;

                case ActiveUnit.SphereFinish:
                    matrixUnit = Matrix.AddTower(X, Y, -1, false);
                    playObject = TowersMatrix.Instance.CreatePlayObject<Tower>(matrixUnit);
                    OnObjectClick(matrixUnit);
                    break;

                case ActiveUnit.Teleport:
                    matrixUnit = Matrix.AddTeleport(X, Y);
                    playObject = TowersMatrix.Instance.CreatePlayObject<Teleport>(matrixUnit);
                    playObject.GetComponentInChildren<ParticleSystem>().Play();
                    break;

                case ActiveUnit.BlackHoleSmall:
                    var blackHoleInfo = Matrix.AddBlackHole(X, Y, BlackHoleInfo.BlackHoleSize.Small);
                    playObject = TowersMatrix.Instance.CreatePlayObject<BlackHole>(blackHoleInfo);
                    matrixUnit = blackHoleInfo;
                    //AddLockedSells(blackHoleInfo);
                        break;

                case ActiveUnit.BlackHoleMedium:
                    blackHoleInfo = Matrix.AddBlackHole(X, Y, BlackHoleInfo.BlackHoleSize.Medium);
                    playObject = TowersMatrix.Instance.CreatePlayObject<BlackHole>(blackHoleInfo);
                    matrixUnit = blackHoleInfo;
                    //AddLockedSells(blackHoleInfo); 
                    break;

                case ActiveUnit.BlackHoleLarge:
                    blackHoleInfo = Matrix.AddBlackHole(X, Y, BlackHoleInfo.BlackHoleSize.Large);
                    playObject = TowersMatrix.Instance.CreatePlayObject<BlackHole>(blackHoleInfo);
                    matrixUnit = blackHoleInfo;
                    //AddLockedSells(blackHoleInfo); 
                    break;

                case ActiveUnit.BlackHoleSuperMassive:
                    blackHoleInfo = Matrix.AddBlackHole(X, Y, BlackHoleInfo.BlackHoleSize.Supermassive);
                    playObject = TowersMatrix.Instance.CreatePlayObject<BlackHole>(blackHoleInfo);
                    matrixUnit = blackHoleInfo;
                    //AddLockedSells(blackHoleInfo); 
                    break;

                default: throw new System.NotImplementedException();
            }

            return playObject;
        }

        private void OnObjectClick(MatrixUnit matrixUnit)
        {
            switch (matrixUnit)
            {
                case TowerInfo sphereInfo:
                    var sphere = TowersMatrix.Instance.Spheres.Single(s => (int)s.transform.position.x == sphereInfo.X && (int)s.transform.position.y == sphereInfo.Y);
                    if (activeUnit == ActiveUnit.SphereStart)
                    {
                        if (sphere.isFinish || sphere.IsStart)
                        {
                            return;
                        }

                        var otherSphereInfoes = Matrix.GetAllTowers().Where(s => s.IsStart).ToList();
                        foreach (var s in otherSphereInfoes)
                        {
                            s.winningWayIndex = -1;
                        }

                        var otherSpheres = TowersMatrix.Instance.Spheres.Where(s => s.IsStart).ToList();
                        foreach (var s in otherSpheres)
                        {
                            s.winningWayIndex = -1;
                            s.TurnOnOffLight(false);
                        }

                        sphereInfo.winningWayIndex = 0;
                        sphere.winningWayIndex = 0;
                        sphere.TurnOnOffLight(true);
                    }
                    else if (activeUnit == ActiveUnit.SphereFinish)
                    {
                        if (sphere.isFinish || sphere.IsStart)
                        {
                            return;
                        }

                        sphereInfo.winningWayIndex = 10;
                        sphere.winningWayIndex = 10;
                        sphereInfo.isFinish = sphere.isFinish = true;
                        TowersMatrix.Instance.SetTowerFinish(sphere);
                        sphere.TurnOnOffLight(false);
                        var directions = sphere.directions;
                        foreach (var d in directions)
                        {
                            sphere.DeleteDirectionAndArrow(d);
                        }
                        sphere.GetComponentInChildren<ParticleSystem>().Play();
                    }
                    else
                    {
                        DeleteUnitAndObject(sphereInfo);
                    }

                    break;

                case TeleportInfo teleportInfo:
                    DeleteUnitAndObject(teleportInfo);
                    break;

                case BlackHoleInfo blackHoleInfo:
                    DeleteUnitAndObject(blackHoleInfo);
                    break;

            }
        }

        /// <summary>
        /// ������� ���� �� ������� � ��������������� ��� ������ �� �����.
        /// </summary>
        /// <param name="unit"></param>
        private void DeleteUnitAndObject(MatrixUnit unit)
        {
            switch (unit)
            {
                case TowerInfo sphereInfo:
                    var sphere = TowersMatrix.Instance.Spheres.Single(s => (int)s.transform.position.x == sphereInfo.X && (int)s.transform.position.y == sphereInfo.Y);
                    var finishTowersList = GameController.Instance.finishTowers;
                    if (finishTowersList.Contains(sphere))
                        finishTowersList.Remove(sphere);
                    break;

                case TeleportInfo teleportInfo:
                    break;

                case BlackHoleInfo blackHoleInfo:
                    //lockedSells.Remove(blackHoleInfo);
                    break;

                default: throw new System.NotImplementedException();
            }

            TowersMatrix.Instance.DeletePlayObject(unit.X, unit.Y);
            Matrix.DeleteMatrixUnit(unit.X, unit.Y);
        }

        //private void AddLockedSells(BlackHoleInfo blackHole)
        //{
        //    int X = blackHole.X;
        //    int Y = blackHole.Y;
        //    var pointsToAdd = new (int x, int y)[8]
        //    {
        //        (X, Y + 1),
        //        (X - 1, Y + 1),
        //        (X - 1, Y),
        //        (X - 1, Y - 1),
        //        (X, Y - 1),
        //        (X + 1, Y - 1),
        //        (X + 1, Y),
        //        (X + 1, Y + 1),
        //    };

        //    lockedSells.Add(blackHole, pointsToAdd);
        //}






        #region ButtonHandlers

        public void NewButtonClick()
        {
            //Debug.Log("Create button click");
            xSize = (int)sizeSlider.value;
            ySize = xSize * 2;
            Matrix = TowersMatrix.Instance.Matrix = new Matrix(xSize, ySize);
            //Debug.Log($"Create matrix size: ({xSize}, {ySize}");
            LevelName = string.Empty;
            UIController.Instance.SetLevelNameToLabel(string.Empty);
            SetActiveUnit(ActiveUnit.BlackHoleSmall);
            SetActiveUnit(ActiveUnit.SphereFinish);
            SphereUnitButtonClick();

            StartCoroutine(EnterCreateLevelMode());
        }


        public void LoadButtonClick()
        {
            //saveLevelMode = false;
            createdLevelsLabel.SetActive(true);
            SetActiveUnit(ActiveUnit.BlackHoleSmall);
            SetActiveUnit(ActiveUnit.SphereFinish);
            SphereUnitButtonClick();
        }


        public void LoadLevel(Level level, string levelName)
        {
            inDarkneesToggle.isOn = level.directionalLightIntensity > 0 ? true : false;
            Matrix = TowersMatrix.Instance.Matrix = level.matrix;
            StartCoroutine(EnterCreateLevelMode());
            createdLevelsLabel.SetActive(false);
            LevelName = textInputField.text = levelName;
            UIController.Instance.SetLevelNameToLabel(levelName);
            //LevelNumber = levelNumber;
        }

        public void CreatedLevelsLabelBackButtonClick()
        {
            createdLevelsLabel.SetActive(false);
        }

        public void OptionsButtonClick() => optionsLabel.SetActive(!optionsLabel.activeSelf);

        public void OnEndOneFingerSwiping(Vector2 beganPosition, Vector2 endPosition)
        {
            if (!gameController.IsEditingLevel ||
                gameController.IsLevelLoading ||
                createdLevelsLabel.activeSelf ||
                UIController.Instance.currentScreen != ActiveScreen.Game ||
                IsLevelMenuButtonClick)
            {
                return;
            }

            //if (!gameController.IsCreateLevelMode)
            //    return;

            var beganIntoMatrixCoordinate = Calculations.NearestWholeCoordinate(beganPosition);
            var allSpheresInfo = Matrix.GetAllTowers(false);
            var sphereInfo = allSpheresInfo.Where(s => s.X == (int)beganIntoMatrixCoordinate.x && s.Y == beganIntoMatrixCoordinate.y).SingleOrDefault();
            //Debug.Log($"sphereInfo: {sphereInfo}, beganIntoMatrixCoordinate: {beganIntoMatrixCoordinate}, allSpheresInfo Count: {allSpheresInfo.Count()}");

            if (sphereInfo == null || sphereInfo.isFinish)
                return;


            Vector2 swipeVector = endPosition - beganPosition;
            float signedAngle = Vector2.SignedAngle(swipeVector, Vector2.up);
            Direction direction;

            if (signedAngle > -22.5f && signedAngle <= 22.5f)
                direction = Direction.Up;
            else if (signedAngle > -67.5f && signedAngle <= -22.5f)
                direction = Direction.UpLeft;
            else if (signedAngle > -112.5f && signedAngle <= -67.5f)
                direction = Direction.Left;
            else if (signedAngle > -157.5f && signedAngle <= -112.5f)
                direction = Direction.DownLeft;
            else if (signedAngle > 157.5f || signedAngle <= -157.5f)
                direction = Direction.Down;
            else if (signedAngle > 112.5f && signedAngle <= 157.5f)
                direction = Direction.DownRight;
            else if (signedAngle > 67.5f && signedAngle <= 112.5f)
                direction = Direction.Right;
            else if (signedAngle > 22.5f && signedAngle <= 67.5f)
                direction = Direction.UpRight;
            else throw new System.NotImplementedException();

            //Debug.Log($"OnEndOneFingerSwiping: beganPos: {beganPosition}, endPos: {endPosition}, direction: {direction}");

            var sphere = TowersMatrix.Instance.Spheres.Single(s => (Vector2)s.transform.position == beganIntoMatrixCoordinate);

            if (sphereInfo.directions.Contains(direction))
            {
                sphereInfo.directions.Remove(direction);
                sphere.DeleteDirectionAndArrow(direction);
            }
            else
            {
                sphereInfo.directions.Add(direction);
                sphere.CreateDirectionAndArrow(direction);
            }

            
        }

        public void PlayLevelButtonClick()
        {
            if (TowersMatrix.Instance.Spheres.Where(s => s.IsStart).Count() == 0)
            {
                UIController.Instance.ShowInformationLabel(gameController.languageController.thereIsNoStartingSphere, 2, true);
                UIController.Instance.BackButtonClick();
                return;
            }

            if (gameController.IsEditingLevel)
            {
                StartCoroutine(EnterPlayLevelMode());
            }
            else
            {
                EnterEditLevelMode();

            }
            SetTextToEditLevelButton();

            UIController.Instance.BackButtonClick();

        }

        private IEnumerator EnterPlayLevelMode()
        {
            gameController.IsEditingLevel = false;
            gameController.IsBeamLaunchAllowed = true;
            gameController.ClearHighlightedTowers();
            gameController.DestroyAllSendedBeams();
            gameController.ClearSendedBeamsList();
            var towersMatrix = TowersMatrix.Instance;
            towersMatrix.GetMatrixFromScreen();

            yield return StartCoroutine(towersMatrix.CreatePlayObjectsFromMatrix());

            //towersMatrix.Spheres.ToList().ForEach(s => Debug.Log($"s: {(s.transform.position.x, s.transform.position.y)}"));
            towersMatrix.Spheres.Single(s => s.IsStart).ActivateStartTower();
            towersMatrix.Spheres.Where(s => s.isFinish).ToList().ForEach(s => s.GetComponentInChildren<ParticleSystem>().Play());
            towersMatrix.Teleports.ToList().ForEach(t => t.GetComponentInChildren<ParticleSystem>().Play());


            levelObjectsPlace.SetActive(false);
            returnButton.gameObject.SetActive(true);
            gameController.SetDirectionalLightIntensity(inDarkneesToggle.isOn ? 0 : 1);
        }

        /// <summary>
        /// ��������, ����������� ��� �������� � ����� �������������� ������ �� ������ ������������ ������.
        /// </summary>
        private void EnterEditLevelMode()
        {
            gameController.IsEditingLevel = true;
            gameController.IsBeamLaunchAllowed = false;
            gameController.DestroyAllSendedBeams();
            gameController.ClearSendedBeamsList();
            Matrix = TowersMatrix.Instance.Matrix; //����� ��������� ������, ������ ��� ��� ���������!

            var highlightedSpheres = gameController.SpheresHighlighted.ToList();
            var sphere = highlightedSpheres.LastOrDefault();
            if (sphere != null)
            {
                sphere.StopAllCoroutines();
                sphere.CreateArrowsFromEditor();
            }

            var startsphere = highlightedSpheres.Where(s => s.IsStart).FirstOrDefault();
            highlightedSpheres.Remove(startsphere);

            foreach (var s in highlightedSpheres)
                s.TurnOnOffLight(false);

            TowersMatrix.Instance.Teleports.ToList().ForEach(t => t.ActivateWaveFromTeleport(false));
            gameController.waitingForPortalChoose = false;

            levelObjectsPlace.SetActive(true);
            returnButton.gameObject.SetActive(false);
            gameController.SetDirectionalLightIntensity(1);
        }

        

        /// <summary>
        /// ������� �� ������ ��������� � ������ �������� ������.
        /// </summary>
        public void SaveLevelButtonInOptionsMenuClick()
        {
            gameController.IsEditingLevel = false;
            if (LevelName != string.Empty)
            {
                var towersMatrix = TowersMatrix.Instance;
                towersMatrix.GetMatrixFromScreen();
                string fileName = $"u_{LevelName}";

                SaveLoadUtil.SaveLevelIntoFile(towersMatrix.Matrix, LevelType.PathOfLight, 0, fileName, inDarkneesToggle.isOn ? 1 : 0, GameController.Instance.Settings.sphereLightIntensity, GameController.Instance.Settings.sphereLightRange);
                print($"Successfully saved into: {fileName}");
                optionsLabel.gameObject.SetActive(false);
                UnityTools.ExecuteWithDelay(delegate { gameController.IsEditingLevel = true; }, 1);
            }
            else
            {
                ShowTextInputDialog(true, string.Empty, SaveNewLevel);
                optionsLabel.SetActive(false);
            }

        }

        /// <summary>
        /// ������� �� ������ OK � ������� ����� ������ ��� ���������� ������ ������.
        /// </summary>
        private void SaveNewLevel()
        {
            gameController.IsEditingLevel = false;

            if (textInputField.text.Length < 3)
            {
                UIController.Instance.ShowInformationLabel(gameController.languageController.minimum3Characters, 3, true);
                return;
            }

            if (SaveLoadUtil.GetAllUserLevelsNames().Contains(textInputField.text))
            {
                UIController.Instance.ShowInformationLabel(gameController.languageController.thisNameAlreadyExists, 2, true);
                return;
            }
            
            LevelName = textInputField.text;
            UIController.Instance.SetLevelNameToLabel(textInputField.text);
            SaveLevelButtonInOptionsMenuClick();

            ShowTextInputDialog(false);
            UnityTools.ExecuteWithDelay(delegate { gameController.IsEditingLevel = true; }, 1);
        }


        public void CancelButtonWhenRenamingLevelClick()
        {
            gameController.IsEditingLevel = false;
            ShowTextInputDialog(false);
            UnityTools.ExecuteWithDelay(delegate { gameController.IsEditingLevel = true; }, 1);
        }


        public void RenameLevel(string levelName)
        {
            if (textInputField.text.Length < 3)
            {
                UIController.Instance.ShowInformationLabel(gameController.languageController.minimum3Characters, 3, true);
                return;
            }

            if (SaveLoadUtil.GetAllUserLevelsNames().Contains(textInputField.text))
            {
                UIController.Instance.ShowInformationLabel(gameController.languageController.thisNameAlreadyExists, 2, true);
                return;
            }

            SaveLoadUtil.RenameUserLevelFile("u_" + levelName, "u_" + textInputField.text);
            ShowTextInputDialog(false);
        }



        public void AnalyzeLevelButtonClick()
        {
            UnityTools.ExecuteWithDelay(() => { optionsLabel.SetActive(false); }, 1);

            if (TowersMatrix.Instance.Spheres.Where(s => s.IsStart).Count() == 0)
            {
                UIController.Instance.ShowInformationLabel(gameController.languageController.thereIsNoStartingSphere, 2, true);
                return;
            }

            analyzer.Analyze(inDarkneesToggle.isOn);
            analyzingCoroutine = StartCoroutine(Analyzing());
        }

        private IEnumerator Analyzing()
        {
            gameController.IsEditingLevel = false;
            winWaysCountLabel.SetActive(true);
            darkCover.SetActive(true);

            while(analyzer.IsAnalyzing)
            {
                winWaysCountTMP.text = analyzer.WinWayCount.ToString();
                yield return null;
            }

            gameController.IsEditingLevel = true;
            winWaysCountLabel.SetActive(false);
            darkCover.SetActive(false);

            TowersMatrix towersMatrix = TowersMatrix.Instance;
            var winSpheres = analyzer.WinWaySpheres;
            for (int i = 0; i < winSpheres.Count; i++)
            {
                if(winSpheres[i] != null)
                {
                    for (int j = 0; j < winSpheres[i].Length; j++)
                    {
                        if (towersMatrix.Matrix[winSpheres[i][j].X, winSpheres[i][j].Y] is TowerInfo tower)
                        {
                            tower.winningWayIndex = j;
                            towersMatrix.Spheres.SingleOrDefault(s => s.transform.position.x == tower.X && s.transform.position.y == tower.Y).winningWayIndex = j;
                            //Debug.Log($"tower: {tower}");
                        }

                        if (Matrix[winSpheres[i][j].X, winSpheres[i][j].Y] is TowerInfo _tower)
                        {
                            _tower.winningWayIndex = j;
                        }
                    }
                    
                }
            }

            

            UIController.Instance.ShowInformationLabel(gameController.languageController.winningWaysNumber + $" {analyzer.WinWayCount}. {gameController.languageController.reward}: {analyzer.StarsReward}" , 5, true);
            analyzingCoroutine = null;
        }

        /// <summary>
        /// ������� �� ������ "������������" � ����. ������� ���������, ��� ������������ ����� ����� �����������.
        /// </summary>
        public void PublishLevelInMenuButtonClick()
        {
            UIController.Instance.ShowInformationLabel(gameController.languageController.youCanPublishALevelAfterCompletingIt, 5, true);
        }

        /// <summary>
        /// ������� �� ������ "������������" ����� �����������.
        /// </summary>
        public void PublishLevelButtonClick()
        {
            ShowTextInputDialog(true, LevelName, PublishLevel);
        }




        public void BackButtonOnWinLabelClick()
        {
            UIController.Instance.SetActiveForWinLabelGameObject(false);

            EnterEditLevelMode();
            SetTextToEditLevelButton();

            if (hittenFinishTower != null)
            {
                int X = (int)hittenFinishTower.transform.position.x;
                int Y = (int)hittenFinishTower.transform.position.y;

                Destroy(hittenFinishTower.gameObject);
                hittenFinishTower = TowersMatrix.Instance.CreatePlayObject<Tower>(Matrix[X, Y]);
                hittenFinishTower.GetComponentInChildren<ParticleSystem>().Play();
            }
        }

        /// <summary>
        /// ������� �� ������ "��� �������".
        /// </summary>
        public void HowToCreateButtonClick()
        {
            howToCreateLabel.SetActive(true);
            optionsLabel.SetActive(false);
            gameController.IsEditingLevel = false;
        }

        /// <summary>
        /// ������� �� ������ ����� �� ��������� "��� �������".
        /// </summary>
        public void HowToCreateBackButtonClick()
        {
            howToCreateLabel.SetActive(false);
            gameController.IsEditingLevel = true;
        }


        /// <summary>
        /// ��� ������� ����� ������ ���� ������.
        /// </summary>
        public void OnLevelMenuButtonClick()
        {
            IsLevelMenuButtonClick = true;
            UnityTools.ExecuteWithDelay(() => { IsLevelMenuButtonClick = false; }, 1);
        }


        #endregion


        


        /// <summary>
        /// ��������/������ ������ ����� ������.
        /// </summary>
        /// <param name="show">��������/������</param>
        /// <param name="text">�����, ������������ � InputField.</param>
        /// <param name="action">���������� �������, ����������� ��� ������� ������ �� ����� ����� ������.</param>
        public void ShowTextInputDialog(bool show, string text = null, UnityAction action = null)
        {
            darkCover.SetActive(show);
            textInputLabel.gameObject.SetActive(show);
            if (text != null)
                textInputField.text = text;

            if(action != null)
                SetActionToOKButtonInNamingDialog(action);
        }

        /// <summary>
        /// ������������� �������, ����������� ��� ������� ������ �� � ������� ������������ ������. ��������� �������� �������.
        /// </summary>
        /// <param name="action"></param>
        private void SetActionToOKButtonInNamingDialog(UnityAction action)
        {
            okButtonOnTextInputDialog.onClick.RemoveAllListeners();
            okButtonOnTextInputDialog.onClick.AddListener(action);
        }



        /// <summary>
        /// ������������� ������� ������ "�������������" � ����������� �� ��������� ������.
        /// </summary>
        private void SetTextToEditLevelButton()
        {
            var buttonTMP = playLevelButton.GetComponentInChildren<TextMeshProUGUI>();
            if (gameController.IsEditingLevel)
            {
                buttonTMP.text = gameController.languageController.playLevel;
            }
            else
            {
                buttonTMP.text = gameController.languageController.editLevel;
            }
        }


        /// <summary>
        /// ��� ����� � ��������� ����� ��� �������� ������ �� �����������.
        /// </summary>
        /// <param name="tower"></param>
        public async void BeamHitFinishSphere(Tower tower)
        {
            UIController.Instance.SetActiveForWinLabelGameObject(true);
            StartCoroutine(UIController.Instance.WinLevelCoverAppearing());
            await System.Threading.Tasks.Task.Delay((int)(secondsAfterFinish * 1000));
            UIController.Instance.SetActiveForPublishButtonsOnWinLabel(true);
            hittenFinishTower = tower;
        }


        private void OnQuitLevel()
        {
            gameController.IsEditingLevel = false;
            gameController.IsCreateLevelMode = false;
            gameController.IsBeamLaunchAllowed = true;
            optionsLabel.SetActive(false);
            levelObjectsPlace.SetActive(false);
            playLevelButton.gameObject.SetActive(false);
            UIController.Instance.SetActiveForSkipLevelButtonGameObject(true);
        }


        public void LevelSaved()
        {
            createdLevelsLabel.SetActive(false);
        }


        private void PublishLevel()
        {
            if (textInputField.text.Length < 3)
            {
                UIController.Instance.ShowInformationLabel(gameController.languageController.minimum3Characters, 3, true);
                return;
            }

            LevelName = textInputField.text;
            Debug.Log($"LevelName: {LevelName}");
            StartCoroutine(PublishingLevel());
        }

        private IEnumerator PublishingLevel()
        {
            analyzer.Analyze(inDarkneesToggle.isOn);
            yield return analyzingCoroutine = StartCoroutine(Analyzing());

            var userLevel = new UserLevelInfo()
            {
                rating = 0,
                launched = 0,
                passed = 0,
                rated = 0,
                levelNumber = 0,

                reward = analyzer.StarsReward,
                levelName = LevelName,
                userName = gameController.PlayerName,
                userID = gameController.StateData.UserId,
                level = SaveLoadUtil.SaveLevelIntoString(Matrix, inDarkneesToggle.isOn ? 0 : 1, gameController.Settings.sphereLightIntensity, gameController.Settings.sphereLightRange)
            };


            Debug.Log($"Publishing level");
            UIController.Instance.ShowInformationInMenu(gameController.languageController.waiting, secondsTimeoutWhenPublishing);

            var clientComponent = ClientPlayerIO.Instance;
            if (clientComponent.Client == null)
            {
                clientComponent.GetClient();

                float timePassed = 0;
                while (clientComponent.Client == null && clientComponent.Error == null)
                {
                    yield return null;
                    timePassed += Time.deltaTime;
                    Debug.Log($"timePassed: {timePassed}");
                    if (timePassed > secondsTimeoutWhenPublishing)
                    {
                        Debug.LogWarning($"Time is out: {timePassed} seconds passed.");
                        UIController.Instance.ShowInformationInMenu(gameController.languageController.failedToPublishTheConnectionTimeHasExpired);
                        yield break;
                    }
                }
            }


            if (clientComponent.Client != null)
            {
                //��������� �� ���������� �� ���� ����� ������� � ����� �� ���������.
                clientComponent.Client.BigDB.LoadRange("UsersLevelsInfo", "userID", null, null, null, /*��� ���� ���-��, ����������� PIO*/1000, usersLevels =>
                {
                    for (int i = 0; i < usersLevels.Length; i++)
                    {
                        Debug.Log(usersLevels[i]);

                        string levelName = usersLevels[i].GetString("LevelName");
                        if (levelName == userLevel.levelName)
                        {
                            UIController.Instance.ShowInformationInMenu(gameController.languageController.youHaveAlreadyPublishedALevelWithThatName);
                            return;
                        }
                    }

                    Publicate();
                },
                er =>
                {
                    Debug.Log($"can't load table. er: {er}");
                    Publicate();
                });
            }

            //��� ������� ����������.
            void Publicate()
            {
                Debug.Log($"LoadSingle!");
                successfulAddedInFirstTableOnPIO = false;
                successfulAddedInSecondTableOnPIO = false;

                clientComponent.Client.BigDB.LoadSingle("UsersLevelsInfo", "levelNumber", null, successObjects =>
                {
                    //����� ������, ��� ������� ����������� ������ ����� ������ �� ������� PIO. � ���� �� ����� � ��������� � ��������, ����� ��������� 
                    //����� ������� ������, ����� ������.
                    uint levelNumber = successObjects.GetUInt("levelNumber");
                    //uint levelNumber = 0;

                    Debug.Log($"successObjects: {successObjects}, " +
                        $"levelNumber: {levelNumber}");

                    levelNumber++;
                    string levelKey = levelNumber.ToString();
                    Debug.Log($"levelKey for new level: {levelKey}");


                    //��������� � ������ ������� ��� ������ �� ������, ����� ������ ������.

                    PlayerIOClient.DatabaseObject dbo = new PlayerIOClient.DatabaseObject();
                    dbo.Set("rating", userLevel.rating);
                    dbo.Set("launched", userLevel.launched);
                    dbo.Set("passed", userLevel.passed);
                    dbo.Set("rated", userLevel.rated);
                    dbo.Set("levelNumber", levelNumber);
                    dbo.Set("reward", userLevel.reward);


                    dbo.Set("LevelName", userLevel.levelName);
                    dbo.Set("userName", userLevel.userName);
                    dbo.Set("userID", userLevel.userID);
                    //dbo.Set("level", userLevel.level);

                    clientComponent.Client.BigDB.CreateObject("UsersLevelsInfo", levelKey, dbo, dbo =>
                    {
                        dbo.Save();
                        successfulAddedInFirstTableOnPIO = true;
                        SuccessfulPublication();
                    }, error =>
                    {
                        Debug.Log($"Error when publishing: {error}");
                        UIController.Instance.ShowInformationInMenu(gameController.languageController.errorWhenPublishing + $" {error}");
                    });


                    //��������� �� ������ ������� ����� ������ � ��� �������.

                    PlayerIOClient.DatabaseObject dbo2 = new PlayerIOClient.DatabaseObject();
                    dbo2.Set("levelNumber", levelNumber);
                    dbo2.Set("level", userLevel.level);

                    clientComponent.Client.BigDB.CreateObject("UsersLevels", levelKey, dbo2, dbo2 =>
                    {
                        dbo2.Save();
                        successfulAddedInSecondTableOnPIO = true;
                        SuccessfulPublication();
                    }, error =>
                    {
                        Debug.Log($"Error when publishing: {error}");
                        UIController.Instance.ShowInformationInMenu(gameController.languageController.errorWhenPublishing + $" {error}");
                    });
                },
                errorObject =>
                {
                    Debug.Log($"Error when publishing: {errorObject}");
                    UIController.Instance.ShowInformationInMenu(gameController.languageController.errorWhenPublishing + $" {errorObject}");
                });
            }
        }


        private void SuccessfulPublication()
        {
            if (successfulAddedInFirstTableOnPIO && successfulAddedInSecondTableOnPIO)
            {
                Debug.Log($"Successful publication!");
                UIController.Instance.ShowInformationInMenu(gameController.languageController.successfulPublication);

                ShowTextInputDialog(false);
                BackButtonOnWinLabelClick();
            }
        }


        private char ValidateLevelName(string input, int charIndex, char addedChar)
        {
            if (char.IsLetterOrDigit(addedChar) || addedChar == ' ')
                return addedChar;
            else return '\0';
        }
    }
}