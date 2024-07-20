using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UITools
{
    /// <summary>
    /// Перед отрисовкой каждого кадра отслеживает, виден ли хотя бы один угол из перечисленных RectTransforms. Если объект полностью не виден отключает его. Включает, если виден хотя бы 1 угол.
    /// </summary>
    public class DisableIfInvisible : MonoBehaviour
    {
        [Tooltip("Список объектов, видимость которых отслеживается.")]
        public RectTransform[] rectTransforms;
        
        //[Tooltip("Камера, на видимость с которой отслеживаются объекты.")]
        //public Camera _camera;

        private Rect screenBounds; // Screen space bounds (assumes camera renders across the entire screen)

        private GameObject[] gameObjects;

        private void OnEnable()
        {
            gameObjects = rectTransforms.Select(rt => rt.gameObject).ToArray();
            screenBounds = new Rect(0f, 0f, Screen.width, Screen.height); // Screen space bounds (assumes camera renders across the entire screen)
        }

        void Update()
        {
            for (int i = 0; i < rectTransforms.Length; i++)
            {
                //bool isVisible = rectTransforms[i].IsVisibleFrom(_camera);
                bool isVisible = IsVisible(rectTransforms[i]);
                //Debug.Log($"name: {gameObjects[i].name}, isVisible: {isVisible}");
                
                if (gameObjects[i].activeSelf != isVisible)
                {
                    //Debug.Log($"change state: gameObject.activeSelf: {gameObjects[i].activeSelf}");
                    gameObjects[i].SetActive(isVisible);
                }
            }
        }



        private bool IsVisible(RectTransform rectTransform)
        {
            Vector3[] objectCorners = new Vector3[4];
            rectTransform.GetWorldCorners(objectCorners);
            var _visibleCorners = objectCorners.Where(oc => screenBounds.Contains(oc)).Count();
            //Debug.Log($"_visibleCorners: {_visibleCorners}");

            return _visibleCorners > 0;
        }





    }






    //public static class RendererExtensions
    //{
    //    /// <summary>
    //    /// Counts the bounding box corners of the given RectTransform that are visible from the given Camera in screen space.
    //    /// </summary>
    //    /// <returns>The amount of bounding box corners that are visible from the Camera.</returns>
    //    /// <param name="rectTransform">Rect transform.</param>
    //    /// <param name="camera">Camera.</param>
    //    private static int CountCornersVisibleFrom(this RectTransform rectTransform, Camera camera)
    //    {
    //        Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height); // Screen space bounds (assumes camera renders across the entire screen)
    //        Debug.Log($"screenBounds: {screenBounds}");


    //        Vector3[] objectCorners = new Vector3[4];
    //        rectTransform.GetWorldCorners(objectCorners);
    //        Debug.Log($"objectCorners[0]: {objectCorners[0]}, objectCorners[1]: {objectCorners[1]}, objectCorners[2]: {objectCorners[2]}, objectCorners[3]: {objectCorners[3]}");

    //        var _visibleCorners = objectCorners.Where(oc => screenBounds.Contains(camera.WorldToScreenPoint(oc))).Count();

    //        int visibleCorners = 0;
    //        Vector3 tempScreenSpaceCorner; // Cached
    //        for (var i = 0; i < objectCorners.Length; i++) // For each corner in rectTransform
    //        {
    //            tempScreenSpaceCorner = camera.WorldToScreenPoint(objectCorners[i]); // Transform world space position of corner to screen space
    //            Debug.Log($"tempScreenSpaceCorner: {tempScreenSpaceCorner}");
    //            Debug.Log($"Camera contains probe. {objectCorners[i]} - {screenBounds.Contains(objectCorners[i])}");


    //            if (screenBounds.Contains(tempScreenSpaceCorner)) // If the corner is inside the screen
    //            {
    //                visibleCorners++;
    //            }
    //        }

    //        Debug.Log($"_visibleCorners: {_visibleCorners}, visibleCorners: {visibleCorners}");
    //        return visibleCorners;
    //    }

    //    /// <summary>
    //    /// Determines if this RectTransform is fully visible from the specified camera.
    //    /// Works by checking if each bounding box corner of this RectTransform is inside the cameras screen space view frustrum.
    //    /// </summary>
    //    /// <returns><c>true</c> if is fully visible from the specified camera; otherwise, <c>false</c>.</returns>
    //    /// <param name="rectTransform">Rect transform.</param>
    //    /// <param name="camera">Camera.</param>
    //    public static bool IsFullyVisibleFrom(this RectTransform rectTransform, Camera camera)
    //    {
    //        return CountCornersVisibleFrom(rectTransform, camera) == 4; // True if all 4 corners are visible
    //    }

    //    /// <summary>
    //    /// Determines if this RectTransform is at least partially visible from the specified camera.
    //    /// Works by checking if any bounding box corner of this RectTransform is inside the cameras screen space view frustrum.
    //    /// </summary>
    //    /// <returns><c>true</c> if is at least partially visible from the specified camera; otherwise, <c>false</c>.</returns>
    //    /// <param name="rectTransform">Rect transform.</param>
    //    /// <param name="camera">Camera.</param>
    //    public static bool IsVisibleFrom(this RectTransform rectTransform, Camera camera)
    //    {
    //        return CountCornersVisibleFrom(rectTransform, camera) > 0; // True if any corners are visible
    //    }
    //}


}