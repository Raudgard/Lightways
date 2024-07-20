using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fields;
using TMPro;
using System.Linq;

namespace Modes
{
    public class BlackHolesRiddlesController : MonoBehaviour
    {
        [SerializeField]
        private GameObject blackHolesPlace;

        [SerializeField]
        private Button goButton;
        

        [SerializeField]
        private GameObject loseLabel;

        [SerializeField]
        private TextMeshProUGUI loseTextTMP;

        [SerializeField]
        private Button tryAgainButton;


        /// <summary>
        /// Список изображений всех черных дыр.
        /// </summary>
        private List<BlackHoleDrag> blackHolesDrag = new List<BlackHoleDrag>();

        //[Tooltip("Список изображений черных дыр, установленных на место в уровне.")]
        /// <summary>
        /// Список изображений черных дыр, установленных на место в уровне.
        /// </summary>
        private List<BlackHoleDrag> installedBlackHoles = new List<BlackHoleDrag>();

        [SerializeField]
        [Tooltip("Ортографический размер камеры, при котором масштаб изображения ЧД на поле равен 1.")]
        private float normalOrthographicSize;

        /// <summary>
        /// Масштаб изображения ЧД, когда она находится над полем.
        /// </summary>
        public Vector3 ScaleForBlackHoleOnField
        {
            get
            {
                var scale = normalOrthographicSize / Camera.main.orthographicSize;
                return new Vector3(scale, scale, 1);
            }
        }

        public bool InteractibleForRestartButton => GameController.Instance.currentLevelStateData.starsMaxOnThisSession > 1;

        /// <summary>
        /// Список ЧД по задумке уровня.
        /// </summary>
        public List<BlackHoleInfo> correctBlackHoles = new List<BlackHoleInfo>();

        /// <summary>
        /// Установленные на поле изображеия черных дыр и точки вокруг них, в которые нельзя ставить другую ЧД.
        /// </summary>
        public Dictionary<BlackHoleDrag, Vector2[]> lockedSells = new Dictionary<BlackHoleDrag, Vector2[]>();

        /// <summary>
        /// Все объекты MarkCircle на уровне.
        /// </summary>
        private MarkCircleByClick[] marksCircleAll;

        /// <summary>
        /// Сейчас будет демонстрация правильного расположения черных дыр?
        /// </summary>
        private bool isShowingCorrectPlacing = false;
        
        /// <summary>
        /// Сейчас будет демонстрация правильного расположения черных дыр?
        /// </summary>
        public bool IsShowingCorrrectPlacing => isShowingCorrectPlacing;

        private void Start()
        {
            UIController.Instance.onQuitLevel += delegate
            {
                isShowingCorrectPlacing = false;
                SetBlackHolesPlaceInactive();
                installedBlackHoles.Clear();
                correctBlackHoles.Clear();
                blackHolesDrag.Clear();
                lockedSells.Clear();
                ShowButtons(false);
                DeleteAllBlackHolesFromMatrix();
                StartCoroutine(TowersMatrix.Instance.ClearHUBs());
                DeleteAllBlackHolesToDrag();
                //Debug.Log($"BlackHolesRiddlesController: quit level. TowersMatrix.Instance.BlackHoles.Length: {TowersMatrix.Instance.BlackHoles.Length}");
            };

        }

        /// <summary>
        /// Передача контроля скрипту при запуске уровня.
        /// </summary>
        public void Initialize()
        {
            //GameController.Instance.isBeamLaunchAllowed = false;
            //StartCoroutine(WaitingForLevelToBeCreated());

            
            blackHolesDrag.Clear();

            if(!isShowingCorrectPlacing) 
            {
                blackHolesPlace.SetActive(true);

                correctBlackHoles = new List<BlackHoleInfo>(TowersMatrix.Instance.Matrix.BlackHoles);
                CreateBlackHolesToDrag(TowersMatrix.Instance.BlackHoles);
                DeleteAllBlackHolesFromMatrix();
                StartCoroutine(DeleteAllBlackHolesFromLevel());
                AddMarkCircleObjectToAllSpheres();
            }
        }

        /// <summary>
        /// Появление Черных дыр.
        /// </summary>
        /// <returns></returns>
        private IEnumerator BlackHolesAppearance()
        {
            UIController.Instance.SetActiveForScreenCover(true);
            var gameController = GameController.Instance;
            float minTime = gameController.Settings.minTimeBeforeNextTowerAppearance;
            float maxTime = gameController.Settings.maxTimeBeforeNextTowerAppearance;
            int countSpheres = TowersMatrix.Instance.Matrix.TowersCount;
            float normalSphereCount = gameController.Settings.normalCountSphereInMatrixForTimeBeforeNextTowerAppearance;

            var blackHoles = new List<BlackHole>(TowersMatrix.Instance.BlackHoles);

            while (blackHoles.Count > 0)
            {
                var bh = blackHoles.GetRandomNext();
                if (bh == null)
                {
                    Debug.Log($"bh == NULL!");
                    continue;
                }
                else
                {
                    Debug.Log($"bh.size: {bh.size}, position: {bh.transform.position}");
                }

                bh.Appearance();
                bh.CircleOfAttractionAreaAppearance();
                bh.LettersAppearance();
                yield return new WaitForSeconds(Random.Range(minTime, maxTime) * (normalSphereCount / countSpheres) * 4);
            }

            UIController.Instance.SetActiveForScreenCover(false);
        }



        /// <summary>
        /// Создает объекты BlackHoleDrag на полосе внизу экрана из списка Черных Дыр. 
        /// Принимает аргументом BlackHole[] или List<BlackHole> или BlackHoleInfo[] или List<BlackHoleInfo>.
        /// </summary>
        /// <typeparam name="T">BlackHole[] or List<BlackHole> or BlackHoleInfo[] or List<BlackHoleInfo></typeparam>
        /// <param name="blackHoles">BlackHole[] or List<BlackHole> or BlackHoleInfo[] or List<BlackHoleInfo></param>
        private void CreateBlackHolesToDrag<T>(T blackHoles)
        {
            BlackHoleInfo.BlackHoleSize[] sizes;

            if (blackHoles is BlackHole[] bhs) { sizes = bhs.Select(bh => bh.size).ToArray(); }
            else if (blackHoles is List<BlackHole> bhsList) { sizes = bhsList.Select(bh => bh.size).ToArray(); }
            else if (blackHoles is BlackHoleInfo[] bhis) { sizes = bhis.Select(bh => bh.size).ToArray(); }
            else if (blackHoles is List<BlackHoleInfo> bhisList) { sizes = bhisList.Select(bh => bh.size).ToArray(); }
            else throw new System.NotImplementedException();

            for (int i = 0; i < sizes.Length; i++)
            {
                var BHtoDrag = Instantiate(Prefabs.Instance.blackHoleToDrag);
                BHtoDrag.size = sizes[i];
                BHtoDrag.transform.SetParent(blackHolesPlace.transform);
                BHtoDrag.transform.localScale = Vector3.one;
                BHtoDrag.originPosition =
                BHtoDrag.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(250 * (i - 1), 0, 0);
                BHtoDrag.BHImage.sprite = BHtoDrag.BHSprites[(int)BHtoDrag.size / 45 - 1];


                var rectT = BHtoDrag.GetComponent<RectTransform>();
                //Debug.Log($"anchoredPosition: {rectT.anchoredPosition}, anchoredPosition3D: {rectT.anchoredPosition3D}, localPosition: {rectT.localPosition}, position: {rectT.position}");

                BHtoDrag.letterOnCenter.text = BHtoDrag.size switch
                {
                    BlackHoleInfo.BlackHoleSize.Small => "S",
                    BlackHoleInfo.BlackHoleSize.Medium => "M",
                    BlackHoleInfo.BlackHoleSize.Large => "L",
                    BlackHoleInfo.BlackHoleSize.Supermassive => "SM",
                    _ => throw new System.NotImplementedException()
                };

                blackHolesDrag.Add(BHtoDrag); 
                BHtoDrag.Initialize(this);
            }
        }



        /// <summary>
        /// Удаляет все черные дыры из матрицы.
        /// </summary>
        private void DeleteAllBlackHolesFromMatrix() => TowersMatrix.Instance.Matrix.DeleteAllBlackHoles();

        /// <summary>
        /// Удаляет все черные дыры из уровня.
        /// </summary>
        private IEnumerator DeleteAllBlackHolesFromLevel()
        {
            var _blackHoles = TowersMatrix.Instance.BlackHoles;
            for (int i = 0; i < _blackHoles.Length; i++)
            {
                //TowersMatrix.Instance.ObjectsInMatrix.Remove(_blackHoles[i]);
                while(!_blackHoles[i].IsBHAppearCompletely)
                { 
                    yield return null; 
                }
                //Destroy(_blackHoles[i].gameObject);
                TowersMatrix.Instance.DeletePlayObject(_blackHoles[i]);
            }
        }

        /// <summary>
        /// Удаляет все объекты черных дыр из места внизу экрана, которые предназначены для перетаскивания на поле.
        /// </summary>
        private void DeleteAllBlackHolesToDrag()
        {
            Debug.Log("DeleteAllBlackHolesToDrag");

            var BHs = blackHolesPlace.GetComponentsInChildren<BlackHoleDrag>(true);
            for (int i = 0; i < BHs.Length; i++)
            {
                Debug.Log("DeleteAllBlackHolesToDrag   Destroy BlackHoleDrag");
                Destroy(BHs[i].gameObject);
            }
        }

        private void SetBlackHolesPlaceInactive() => blackHolesPlace.SetActive(false);


        public void BlackHoleDragInstalled(BlackHoleDrag blackHoleDrag)
        {
            //lockedSells.AddRange(GetLockedSells(blackHoleDrag.AllowedPoint));
            if (!installedBlackHoles.Contains(blackHoleDrag))
            {
                installedBlackHoles.Add(blackHoleDrag);
                lockedSells.Add(blackHoleDrag, GetLockedSells(blackHoleDrag.AllowedPoint));
                Debug.Log($"installedBlackHoles.Count: {installedBlackHoles.Count}, blackHolesDrag.Count: {blackHolesDrag.Count}");
                if(installedBlackHoles.Count == blackHolesDrag.Count)
                {
                    ShowButtons(true);
                }
            }
            else
            {
                lockedSells[blackHoleDrag] = GetLockedSells(blackHoleDrag.AllowedPoint);
            }
        }

        public void BlackHoleDragUninstalled(BlackHoleDrag blackHoleDrag)
        {
            if(lockedSells.ContainsKey(blackHoleDrag))
                lockedSells.Remove(blackHoleDrag);
            
            if (installedBlackHoles.Contains(blackHoleDrag))
            {
                installedBlackHoles.Remove(blackHoleDrag);
                if(goButton.gameObject.activeSelf)
                {
                    ShowButtons(false);
                }
            }
        }

        private Vector2[] GetLockedSells(Vector2 blackHolePosition)
        {
            return new Vector2[]
                {
                    new Vector2(blackHolePosition.x, blackHolePosition.y),

                    new Vector2(blackHolePosition.x, blackHolePosition.y + 1),
                    new Vector2(blackHolePosition.x - 1, blackHolePosition.y + 1),
                    new Vector2(blackHolePosition.x - 1, blackHolePosition.y),
                    new Vector2(blackHolePosition.x - 1, blackHolePosition.y - 1),
                    new Vector2(blackHolePosition.x, blackHolePosition.y - 1),
                    new Vector2(blackHolePosition.x + 1, blackHolePosition.y - 1),
                    new Vector2(blackHolePosition.x + 1, blackHolePosition.y),
                    new Vector2(blackHolePosition.x + 1, blackHolePosition.y + 1),

                    new Vector2(blackHolePosition.x - 2, blackHolePosition.y),
                    new Vector2(blackHolePosition.x - 2, blackHolePosition.y + 1),
                    new Vector2(blackHolePosition.x - 2, blackHolePosition.y - 1),
                    new Vector2(blackHolePosition.x + 2, blackHolePosition.y),
                    new Vector2(blackHolePosition.x + 2, blackHolePosition.y + 1),
                    new Vector2(blackHolePosition.x + 2, blackHolePosition.y - 1),
                    new Vector2(blackHolePosition.x, blackHolePosition.y - 2),
                    new Vector2(blackHolePosition.x + 1, blackHolePosition.y - 2),
                    new Vector2(blackHolePosition.x - 1, blackHolePosition.y - 2),
                    new Vector2(blackHolePosition.x, blackHolePosition.y + 2),
                    new Vector2(blackHolePosition.x + 1, blackHolePosition.y + 2),
                    new Vector2(blackHolePosition.x - 1, blackHolePosition.y + 2)
                };
        }


        private void ShowButtons(bool show)
        {
            goButton.gameObject.SetActive(show);
            //cancelButton.gameObject.SetActive(show);
        }


        public void OnGoButtonClick()
        {
            foreach(var bhd in installedBlackHoles)
            {
                blackHolesDrag.Remove(bhd);
                StartCoroutine(bhd.CreateBlackHole());
            }

            ShowButtons(false);
            blackHolesPlace.SetActive(false);
            installedBlackHoles.Clear();
            GameController.Instance.IsBeamLaunchAllowed = true;
            DestroyAllMarksCircle();
        }


        public void BeamHitWinSphere()
        {
            var gameController = GetComponent<GameController>();
            
            if (gameController.SpheresHighlighted.Count == TowersMatrix.Instance.Spheres.Length)
            {
                //Debug.Log($"Все сферы были зажжены! Победа!");
                gameController.onWinLevel.Invoke();
            }
            else
            {
                LoseLevel();
            }
        }

        private void LoseLevel()
        {
            var gameController = GameController.Instance;
            Debug.Log($"Было зажжено {gameController.SpheresHighlighted.Count} сфер из {TowersMatrix.Instance.Spheres.Length}. Поражение.");
            loseLabel.SetActive(true);
            UIController.Instance.SetActiveForMinusOneStarGOOnLoseLabel(!gameController.IsStoryLevel);
            gameController.musicController.LoseLevelSoundPlay();

            UIController.Instance.currentScreen = ActiveScreen.LoseLabel;
            UIController.Instance.SetInteractibleForShowCorrectPlacementButton();
            loseTextTMP.text = gameController.languageController.NotAllSpheresWereIlluminatedString;

            if (gameController.IsStoryLevel)
            {
                tryAgainButton.gameObject.SetActive(true);
            }
            else
            {
                if (gameController.currentLevelStateData.starsMaxOnThisSession > 1)
                {
                    tryAgainButton.gameObject.SetActive(true);
                }
                else
                {
                    tryAgainButton.gameObject.SetActive(false);
                }
            }
        }


        public void RestartLevelButtonClick() => RestartLevel(!GameController.Instance.IsStoryLevel);
        public void TryAgainButtonClick() => RestartLevel(!GameController.Instance.IsStoryLevel);



        private void RestartLevel(bool minus1star)
        {
            var levelStateData = GameController.Instance.currentLevelStateData;
            if (minus1star && levelStateData.starsMaxOnThisSession > 0)
                levelStateData.starsMaxOnThisSession--;

            DeleteAllBlackHolesFromMatrix();
            correctBlackHoles.ForEach(cbh => TowersMatrix.Instance.Matrix.AddBlackHole(cbh.X, cbh.Y, cbh.size));

            Debug.Log($"RestartLevel. BH count: {TowersMatrix.Instance.BlackHoles.Length}");

            loseLabel.gameObject.SetActive(false);
            DeleteAllBlackHolesToDrag();
            //CreateBlackHolesToDrag(correctBlackHoles);


            StartCoroutine(GameController.Instance.LoadCustomLevel(levelStateData, true, true));
            //StartCoroutine(DeleteAllBlackHolesFromLevel());

            blackHolesPlace.SetActive(false);
            goButton.gameObject.SetActive(false);
            installedBlackHoles.Clear();
            lockedSells.Clear();

            //var blackHolesDrag = blackHolesPlace.GetComponentsInChildren<BlackHoleDrag>();
            //foreach (var bhd in blackHolesDrag)
            //{
            //    bhd.ReturnToOriginPosition();
            //}

            //AddMarkCircleObjectToAllSpheres();
            UIController.Instance.currentScreen = ActiveScreen.Game;
        }


        public void OnShowCorrectPlacementButtonClick()
        {
            var levelStateData = GameController.Instance.currentLevelStateData;
            levelStateData.starsMaxOnThisSession = 0;

            foreach (var bh in TowersMatrix.Instance.BlackHoles)
            {
                Destroy(bh.gameObject);
            }

            TowersMatrix.Instance.Matrix.DeleteAllBlackHoles();
            TowersMatrix.Instance.Matrix.PointsOfBlackHoleInfluence.Clear();
            //Debug.Log($"OnShowCorrectPlacementButtonClick. BH count in matrix: {TowersMatrix.Instance.Matrix.BlackHoles.Count()}");

            foreach (var bh in correctBlackHoles)
            {
                TowersMatrix.Instance.Matrix.AddBlackHole(bh.X, bh.Y, bh.size);
            }
            
            loseLabel.gameObject.SetActive(false);
            isShowingCorrectPlacing = true;
            StartCoroutine(GameController.Instance.LoadCustomLevel(levelStateData, true, true));
            blackHolesPlace.SetActive(false);
            goButton.gameObject.SetActive(false);

            if(UIController.Instance.currentScreen == ActiveScreen.IntoLevelMenu)
            {
                UIController.Instance.BackButtonClick();
            }
            else
            {
                UIController.Instance.currentScreen = ActiveScreen.Game;
            }

            //Debug.Log($"OnShowCorrectPlacementButtonClick. BH count in HUB: {TowersMatrix.Instance.BlackHoles.Length}");

        }


        /// <summary>
        /// При масштабировании и скроллинге поля игроком.
        /// </summary>
        public void OnTwoFingersSwiping(float deltaPositionBetweenFingers, Vector2 middlePointDeltaPosition)
        {
            ReactionOnScrollingAndScaling();
        }

        public void OnDoubleClick(Vector2 position)
        {
            ReactionOnScrollingAndScaling();
        }



        private void ReactionOnScrollingAndScaling()
        {
            var cameraUI = UIController.Instance.cameraUI;

            foreach (var bh in installedBlackHoles)
            {
                bh.OnDrag(null, cameraUI.orthographicSize * 2 / Screen.height);
                var transform = bh.transform;
                //transform.position = camera.WorldToScreenPoint(bh.AllowedPoint);
                transform.localScale = ScaleForBlackHoleOnField;
                //Debug.Log($"bh.name: {bh.name}, bh.transform.position: {transform.position}, bh.transform.localScale: {transform.localScale}");
            }
        }


        private void AddMarkCircleObjectToAllSpheres()
        {
            var spheres = TowersMatrix.Instance.Spheres;
            marksCircleAll = new MarkCircleByClick[spheres.Length];
            var prefab = Prefabs.Instance.markCircleByClick;

            for (int i = 0; i < spheres.Length; i++)
            {
                marksCircleAll[i] = Instantiate(prefab);
                marksCircleAll[i].transform.SetParent(spheres[i].transform);
                marksCircleAll[i].transform.localPosition = Vector3.zero;
                marksCircleAll[i].transform.localScale = prefab.transform.localScale;

            }
        }


        private void DestroyAllMarksCircle()
        {
            for (int i = 0; i < marksCircleAll.Length; i++)
            {
                Destroy(marksCircleAll[i].gameObject);
            }
        }


        public bool HintActivate()
        {
            //if (blackHolesDrag.Count == 0)
            //{
            //    return false;
            //}

            BlackHoleDrag[] uninstalledBHD = blackHolesDrag.Where(bhd => !bhd.IsInstalled).ToArray();
            if (uninstalledBHD.Length == 0)
            {
                return false;
            }

            BlackHoleDrag blackHoleDrag = uninstalledBHD[0];
            //BlackHoleInfo correctBH = correctBlackHoles.Where(cbh => cbh.size == blackHoleDrag.size).FirstOrDefault();
            BlackHoleInfo correctBH = null;


            for (int i = 0; i < correctBlackHoles.Count; i++)
            {
                if(correctBlackHoles[i].size == blackHoleDrag.size &&
                    !TowersMatrix.Instance.Matrix.BlackHoles.Any(bh => bh.X == correctBlackHoles[i].X && bh.Y == correctBlackHoles[i].Y) &&
                    !installedBlackHoles.Any(bh => bh.AllowedPoint.x == correctBlackHoles[i].X && bh.AllowedPoint.y == correctBlackHoles[i].Y))
                {
                    correctBH = correctBlackHoles[i];
                    break;
                }
            }

            if (correctBH == null)
            {
                Debug.LogError("Не могу найти соответствующую correctBH");
                return false;
            }

            Debug.Log($"blackHoleDrag.size: {blackHoleDrag.size}, correctBH.size: {correctBH.size}");
            blackHoleDrag.AllowedPoint = new Vector2(correctBH.X, correctBH.Y);
            BlackHoleDragInstalled(blackHoleDrag);
            if (installedBlackHoles.Contains(blackHoleDrag))
            {
                installedBlackHoles.Remove(blackHoleDrag);
            }
            
            blackHolesDrag.Remove(blackHoleDrag);
            Destroy(blackHoleDrag.gameObject);

            TowersMatrix.Instance.Matrix.AddBlackHole(correctBH.X, correctBH.Y, correctBH.size);
            BlackHole blackHole = TowersMatrix.Instance.CreatePlayObject<BlackHole>(correctBH);
            blackHole.Appearance();

            if (blackHolesDrag.Count == 0)
            {
                OnGoButtonClick();
            }

            return true;
        }

    }
}