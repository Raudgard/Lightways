using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using Fields;
using UnityEngine.UI;
using TMPro;

namespace Modes
{
    /// <summary>
    /// Черная дыра для перемещения на поле для Black Holes Riddles.
    /// </summary>
    public class BlackHoleDrag : MonoBehaviour
    {
        [Tooltip("Image этой черной дыры.")]
        public Image BHImage;

        [Tooltip("Image кольца вокруг черной дыры во время перетаскивания.")]
        public Image circleImage;

        [Tooltip("Image этой ЧД.")]
        public TextMeshProUGUI letterOnCenter;

        public BlackHoleInfo.BlackHoleSize size;
        public Sprite[] BHSprites;

        //[Tooltip("Минимальный квадрат дистанции до ровной координаты, чтобы ЧД \"примагнитилась\" к ней.")]
        //public float distanceSqrToCoordinateTreshold;
        //[HideInInspector]
        [Tooltip("Изначальное место внизу экрана для этого изображения черной дыры.")]
        public Vector3 originPosition;

        /// <summary>
        /// Координаты разрешенной точки.
        /// </summary>
        public Vector2 AllowedPoint { get; set; }

        public bool IsInstalled { get; set; } = false;

        private Camera cameraMain;
        private Camera cameraUI;
        private BlackHolesRiddlesController controller;
        //private RectTransform rectTransform;
        private RectTransform blackHolesPlaceRectTransform;

        private Vector2 checkingPoint = new Vector2(-1, -1);
        private bool isPositionAllowed;

        private void Start()
        {
            blackHolesPlaceRectTransform = gameObject.transform.parent.GetComponent<RectTransform>();
            GetComponent<UITools.DragAndDropObject>().cameraUI = UIController.Instance.cameraUI;
            cameraMain = Camera.main;
            cameraUI = UIController.Instance.cameraUI;
        }

        public void Initialize(BlackHolesRiddlesController controller) => this.controller = controller;

        public void OnDrag(PointerEventData eventData, float verticalRatio)
        {
            //Debug.Log($"OnDrag. eventData.position: {eventData.position}, eventData.delta: {eventData.delta}, eventData.scrollDelta: {eventData.scrollDelta}");
            GameController.Instance.IsBeamLaunchAllowed = false;
            var transform = gameObject.transform;

            if (IsBlackHoleIntoBlackHolesPlace(out _))
            {
                //circleImage.color = Color.white;
                circleImage.enabled = false;
                transform.localScale = Vector3.one;
                return;
            }
            
            var point = NearestWholeCoordinate(verticalRatio, out Vector3 additiveVector);
            var pos = cameraMain.WorldToScreenPoint(point) * verticalRatio - additiveVector;

            //Debug.Log($"NearestWholeCoordinate: {point}, WorldToScreenPoint: {pos}");
            transform.position = new Vector3(pos.x, pos.y, -500);
            transform.localScale = controller.ScaleForBlackHoleOnField;

            if (point == checkingPoint)
                return;
            else
                checkingPoint = point;


            int _pointX = (int)point.x;
            int _pointY = (int)point.y;

            isPositionAllowed = IsPositionAllowed(_pointX, _pointY);

            circleImage.enabled = true;
            if (isPositionAllowed)
            {
                circleImage.color = Color.green;
                AllowedPoint = new Vector2(_pointX, _pointY);
            }
            else
            {
                circleImage.color = Color.red;
            }

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //Debug.Log($"OnEndDrag. eventData.position: {eventData.position}, eventData.delta: {eventData.delta}, eventData.scrollDelta: {eventData.scrollDelta}");
            StartCoroutine(SetAllowToBeamSendWithOneFrameDelay());

            if (IsBlackHoleIntoBlackHolesPlace(out _))
            {
                //возвращает на свое место.
                ReturnToOriginPosition();
                return;
            }


            if(isPositionAllowed)
            {
                //circleImage.color = Color.white;
                circleImage.enabled = false;
                controller.BlackHoleDragInstalled(this);
                IsInstalled = true;
            }
            else
            {
                ReturnToOriginPosition();
            }

        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //Debug.Log($"OnPointerDown. eventData.position: {eventData.position}, eventData.delta: {eventData.delta}, eventData.scrollDelta: {eventData.scrollDelta}");
        }



        private Vector2 NearestWholeCoordinate(float verticalRatio, out Vector3 additiveVector)
        {
            //verticalRatio = cameraUI.orthographicSize * 2 / Screen.height;
            float X = Screen.width / 2 * verticalRatio - cameraUI.transform.position.x;
            float Y = Screen.height / 2 * verticalRatio - cameraUI.transform.position.y;

            additiveVector = new Vector3(X, Y, -1000) ;
            var relativePosition = (gameObject.transform.position + additiveVector)/verticalRatio;
            var worldPointPosition = (Vector2)cameraMain.ScreenToWorldPoint(relativePosition) ;

            //Debug.Log($"relativePosition: {relativePosition}, verticalRatio: {verticalRatio}, worldPointPosition: {worldPointPosition}, additiveVector: {additiveVector}");


            int Xbottom = (int)worldPointPosition.x;
            int Xtop = Mathf.CeilToInt(worldPointPosition.x);
            if (Xbottom == Xtop) Xtop += 1;
            int Ybottom = (int)worldPointPosition.y;
            int Ytop = Mathf.CeilToInt(worldPointPosition.y);
            if (Ybottom == Ytop) Ytop += 1;

            //Debug.Log($"Xbottom: {Xbottom}, Xtop: {Xtop}, Ybottom: {Ybottom}, Ytop: {Ytop}");

            Vector2[] points = new Vector2[4]
            {
                new Vector2(Xbottom, Ybottom),
                new Vector2(Xbottom, Ytop),
                new Vector2(Xtop, Ybottom),
                new Vector2(Xtop, Ytop),
            };

            Dictionary<Vector2, float> pointsAndDistances = new Dictionary<Vector2, float>(4)
            {
                {points[0], (points[0] - worldPointPosition).sqrMagnitude},
                {points[1], (points[1] - worldPointPosition).sqrMagnitude},
                {points[2], (points[2] - worldPointPosition).sqrMagnitude},
                {points[3], (points[3] - worldPointPosition).sqrMagnitude}
            };

            var minDistance = pointsAndDistances.Values.Min();
            return pointsAndDistances.First(p => p.Value == minDistance).Key;
        }


        private bool IsPositionAllowed(int X, int Y)
        {
            var matrix = TowersMatrix.Instance.Matrix;
            if (X < 0 || X >= matrix.SizeX || Y < 0 || Y >= matrix.SizeY)
                return false;
            
            List<(int X, int Y)> pointsToCheck = new List<(int X, int Y)>();
            pointsToCheck.Add((X, Y));
            pointsToCheck.Add((X, Y + 1));
            pointsToCheck.Add((X - 1, Y + 1));
            pointsToCheck.Add((X - 1, Y));
            pointsToCheck.Add((X - 1, Y - 1));
            pointsToCheck.Add((X, Y - 1));
            pointsToCheck.Add((X + 1, Y - 1));
            pointsToCheck.Add((X + 1, Y));
            pointsToCheck.Add((X + 1, Y + 1));

            if(X == 0)
            {
                pointsToCheck.Remove((X - 1, Y));
                pointsToCheck.Remove((X - 1, Y - 1));
                pointsToCheck.Remove((X - 1, Y + 1));
            }

            if (X == matrix.SizeX - 1)
            {
                pointsToCheck.Remove((X + 1, Y));
                pointsToCheck.Remove((X + 1, Y - 1));
                pointsToCheck.Remove((X + 1, Y + 1));
            }

            if (Y == 0)
            {
                pointsToCheck.Remove((X, Y - 1));
                pointsToCheck.Remove((X - 1, Y - 1));
                pointsToCheck.Remove((X + 1, Y - 1));
            }

            if (Y == matrix.SizeY - 1)
            {
                pointsToCheck.Remove((X, Y + 1));
                pointsToCheck.Remove((X - 1, Y + 1));
                pointsToCheck.Remove((X + 1, Y + 1));
            }

            IEnumerable<Vector2> cells = new List<Vector2>();
            foreach (var kvp in controller.lockedSells)
            {
                if(kvp.Key != this)
                    cells = cells.Concat(kvp.Value);
            }
            //Debug.Log($"cells.Count: {cells.Count()}");

            return pointsToCheck.All(p => matrix[p.X, p.Y] == null) && !cells.Contains(new Vector2(X, Y));
        }


        /// <summary>
        /// Проверяет, находится ли изображение черной дыры над местом для черных дыр (внизу экрана).
        /// </summary>
        /// <returns></returns>
        private bool IsBlackHoleIntoBlackHolesPlace(out Vector3 relativePos)
        {
            Vector3 worldToScreenPoint = cameraUI.WorldToScreenPoint(gameObject.transform.position);
            //var worldToViewportPoint = cameraUI.WorldToViewportPoint(gameObject.transform.position);
            relativePos = new Vector3(worldToScreenPoint.x, worldToScreenPoint.y * 1080 / Screen.width, worldToScreenPoint.z);
            //Debug.Log($"worldToScreenPoint: {worldToScreenPoint},  relativePos: {relativePos}");

            return relativePos.y < blackHolesPlaceRectTransform.rect.height;

        }


        public void OnScalingOrScrolling()
        {

        }


        /// <summary>
        /// Создает черную дыру в уровне на месте, где находится изображение этой ЧД. И уничтожает этот объект.
        /// </summary>
        public IEnumerator CreateBlackHole()
        {
            (int X, int Y) coordinates = ((int)AllowedPoint.x, (int)AllowedPoint.y);

            BlackHoleInfo blackHoleInfo = TowersMatrix.Instance.Matrix.AddBlackHole(coordinates.X, coordinates.Y, size);
            var blackHole = TowersMatrix.Instance.CreatePlayObject<BlackHole>(blackHoleInfo);
            
            var gameController = GameController.Instance;

            gameController.IsBeamLaunchAllowed = false;
            UIController.Instance.SetActiveForScreenCover(true);

            blackHole.SetSizeAreaAndLetterTransparent(true);

            blackHole.Appearance();
            blackHole.CircleOfAttractionAreaAppearance();
            blackHole.LettersAppearance();

            Destroy(gameObject);

            while (!blackHole.IsBHAppearCompletely)
            {
                yield return null;
            }

            gameController.IsBeamLaunchAllowed = true;
            UIController.Instance.SetActiveForScreenCover(false);
        }

        public void ReturnToOriginPosition()
        {
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition3D = originPosition;
            rectTransform.localScale = Vector3.one;
            controller.BlackHoleDragUninstalled(this);
            //circleImage.color = Color.white;
            circleImage.enabled = false;
            isPositionAllowed = false;
            IsInstalled = false;
        }


        private IEnumerator SetAllowToBeamSendWithOneFrameDelay()
        {
            yield return null;
            GameController.Instance.IsBeamLaunchAllowed = true;
        }
    }
}