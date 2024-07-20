using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fields;
using System;
using System.Linq;
using UnityEngine.EventSystems;

public class InputHandlers : MonoBehaviour
{
    private Transform cameraTransform;
    [SerializeField] private TowersMatrix towersMatrix;
    [SerializeField] private SwipeableArea swipeableArea;


    [Tooltip("Размер запаса, за который игрок может подвинуть камеру от самых крайних башен.")]
    [Range(0.5f, 4f)]
    [SerializeField] private float reserveDistance;
    [Tooltip("Насколько дальше можно отводить камеру, чем размер матрицы по X.")]
    [SerializeField] private float additionToMaxOrthographicSize;


    public float minOrthographicSize;
    public float maxOrthographicSize;

    [Tooltip("Коэффициент для толщины линии поля при изменении ортографического размера камеры.")]
    [SerializeField] private float coeffForLinesWidth;
    [SerializeField] private LineRenderer backgroundLinesRenderer;


    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }




    /// <summary>
    /// Здесь обрабатывается реакция на нажатие пальцем или клик левой кнопкой.
    /// </summary>
    /// <param name="touchOrLeftMouseClickPosition"></param>
    public void OnClick(Vector2 touchOrLeftMouseClickPosition)
    {
        if (UIController.Instance.currentScreen != ActiveScreen.Game)
            return;
     
        var ray = Camera.main.ScreenPointToRay(touchOrLeftMouseClickPosition);
        RaycastHit2D intersect = Physics2D.GetRayIntersection(ray);

        if (intersect.transform != null && intersect.transform.TryGetComponent(out IClickableObject clickableObject))
        {
            clickableObject.OnClick();
        }
        
    }





    public void OnOneFingerSwiping(Vector2 beganTouchPosition, Vector2 deltaPosition)
    {
        if (UIController.Instance.currentScreen != ActiveScreen.Game)
            return;


        //Debug.Log($"OnOneFingerSwiping: beganTouchPosion: {beganTouchPosion}, deltaPosition: {deltaPosition}");
        
        //if (!swipeableArea.inArea)
        //{
        //    var moveCamera = deltaPosition * Camera.main.orthographicSize / 10;
        //    cameraTransform.position += (Vector3)moveCamera;
        //}

        LimitCameraPosition();
    }



    public void OnTwoFingersSwipe(float deltaPositionBetweenFingers, Vector2 middlePointDeltaPosition)
    {
        if (UIController.Instance.currentScreen != ActiveScreen.Game)
            return;
        
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize += deltaPositionBetweenFingers, minOrthographicSize, maxOrthographicSize);
        
        var moveCamera = middlePointDeltaPosition * Camera.main.orthographicSize / 10;
        cameraTransform.position += (Vector3)moveCamera;

        LimitCameraPosition();
        ChangeLinesWidth();
    }


    public void OnEndOneFingerSwiping(Vector2 beganPosition, Vector2 endPosition)
    {
        //Debug.Log($"OnEndOneFingerSwiping");

        if (UIController.Instance.currentScreen != ActiveScreen.Game)
        {
            return;
        }

        
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
        else throw new NotImplementedException();

        //Debug.Log($"OnEndOneFingerSwiping: beganPos: {beganPosition}, endPos: {endPosition}");

        if (GameController.Instance.LastBeamSended != null && GameController.Instance.LastBeamSended.InputDirection == direction)
        {
            UIController.Instance.ReturnButtonClick();
            return;
        }
        else if (GameController.Instance.waitingForPortalChoose)
        {
            Debug.Log($"waitingForPortalChoose");
            UIController.Instance.ShowInformationLabel(GameController.Instance.languageController.selectPortalForBeamExitString, 2);
        }

        if (!GameController.Instance.IsBeamLaunchAllowed)
        {
            return;
        }

        Tower tower = GameController.Instance.ActiveTowers.FirstOrDefault();
        if (tower != null && tower.directions.Length > 0 && tower.directions.Contains(direction))
        {
            tower.SendLightBeam(direction);
            //return;
        }
    }


    public void OnDoubleClick(Vector2 position)
    {
        if (UIController.Instance.currentScreen != ActiveScreen.Game)
            return;

        OnLevelLoad();
    }


    /// <summary>
    /// Ограничивает позицию камеры в зависимости от ее ортографического размера и размера созданного поля.
    /// </summary>
    private void LimitCameraPosition()
    {
        float minX = 0f - reserveDistance;
        float maxX = towersMatrix.sizeX - 1f + reserveDistance;

        float c = towersMatrix.sizeY / 2f - towersMatrix.sizeX;
        if (c < 0) c = 0;

        float halfY = (towersMatrix.sizeY - 1f) / 2;
        //float shift = Camera.main.orthographicSize / 4f + c;
        float shift = maxOrthographicSize / 4f + c + (maxOrthographicSize - Camera.main.orthographicSize) / 2f;


        float minY = halfY - shift;
        float maxY = halfY + shift;

        //print($"minX: {minX}, minY: {minY}, maxX: {maxX}, maxY: {maxY}");
        cameraTransform.position = new Vector3(Mathf.Clamp(cameraTransform.position.x, minX, maxX), Mathf.Clamp(cameraTransform.position.y, minY, maxY), cameraTransform.position.z);
    }

    public void ChangeLinesWidth()
    {
        backgroundLinesRenderer.widthMultiplier = Camera.main.orthographicSize * coeffForLinesWidth;
    }

    public void OnLevelLoad()
    {
        //Camera.main.transform.position = new Vector3((towersMatrix.sizeX - 1) / 2, (towersMatrix.sizeY - 1) / 2, -10);
        maxOrthographicSize = towersMatrix.sizeX + additionToMaxOrthographicSize;

        Camera.main.orthographicSize = maxOrthographicSize;
        cameraTransform.position = new Vector3(maxOrthographicSize / 2 - additionToMaxOrthographicSize / 2 - 0.5f, (towersMatrix.sizeY - 1f) / 2, Camera.main.transform.position.z);
        LimitCameraPosition();
        ChangeLinesWidth();
    }

}
