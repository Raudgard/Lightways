using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fields
{
    public class FieldBackground : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private TowersMatrix towersMatrix;
        [SerializeField] private Transform backgroundTransform;

        [Tooltip("Количество запасных линий \"за границей видимости\"")]
        public int reserveLines;
        public float zPosition;

        private Vector3 backgroundStartPosition;
        private Vector3 backgroundStartScale;

        private void Awake()
        {
            backgroundStartPosition = backgroundTransform.localPosition;
            backgroundStartScale = backgroundTransform.localScale;
            //Debug.Log($"backgroundStartPosition: {backgroundStartPosition}, backgroundStartScale: {backgroundStartScale}");
        }


        public void CreateBackgroundAndLines(int sizeX, int sizeY)
        {
            int verticalLinesCount = sizeX + (2 * reserveLines);
            if (verticalLinesCount % 2 != 0) verticalLinesCount++;
            int verticalLinesPointsCount = verticalLinesCount * 2;
            //print($"verticalLinesPointsCount = {verticalLinesPointsCount}");

            int horizontalLinesCount = sizeY + (2 * reserveLines);
            int horizontaLinesPointsCount = horizontalLinesCount * 2;
            //print($"horizontaLinesPointsCount = {horizontaLinesPointsCount}");

            int pointsCount = verticalLinesPointsCount + horizontaLinesPointsCount - 1;
            //print($"pointsCount: {pointsCount}, horizontalCount: {horizontalLinesCount}, vecticalCount: {verticalLinesCount}");

            Vector3[] positions = new Vector3[pointsCount];
            int maxYPosition = horizontalLinesCount - reserveLines - 1;
            int maxXPosition = verticalLinesCount - reserveLines - 1;

            for(int i = 0; i < verticalLinesPointsCount; i++)
            {
                int div = i % 4;

                switch (div)
                {
                    case 0:  positions[i] = new Vector3(-reserveLines + (i / 2), -reserveLines, zPosition) ;break;
                    case 1:  positions[i] = new Vector3(-reserveLines + ((i - 1) / 2), maxYPosition, zPosition); break;
                    case 2:  positions[i] = new Vector3(-reserveLines + (i / 2), maxYPosition, zPosition); break;
                    case 3:  positions[i] = new Vector3(-reserveLines + ((i - 1) / 2), -reserveLines, zPosition); break;
                    default: throw new System.Exception();
                }
            }


            for (int i = verticalLinesPointsCount; i < pointsCount; i++)
            {
                int div = i % 4;

                switch (div)
                {
                    case 0: positions[i] = new Vector3(-reserveLines, -reserveLines + ((i - verticalLinesPointsCount) / 2), zPosition); break;
                    case 1: positions[i] = new Vector3(-reserveLines, -reserveLines + ((i - verticalLinesPointsCount + 1) / 2), zPosition); break;
                    case 2: positions[i] = new Vector3(maxXPosition, -reserveLines + ((i - verticalLinesPointsCount) / 2), zPosition); break;
                    case 3: positions[i] = new Vector3(maxXPosition, -reserveLines + +((i - verticalLinesPointsCount + 1) / 2), zPosition); break;
                    default: throw new System.Exception();
                }
            }

            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);
            SetBackgroundSize(sizeX, sizeY);
        }


        private void SetBackgroundSize(int sizeX, int sizeY)
        {
            //Debug.Log($"backgroundStartPosition: {backgroundStartPosition}");
            backgroundTransform.localPosition = new Vector3(sizeX / 2, sizeY / 2, backgroundStartPosition.z);
            //Debug.Log($"backgroundTransform.localPosition: {backgroundTransform.localPosition}");
            backgroundTransform.localScale = new Vector3(sizeX * 3 + 1, sizeY * 3, 1);
        }


        public void SetBackgroundToStartPosition()
        {
            backgroundTransform.localPosition = backgroundStartPosition;
            backgroundTransform.localScale = backgroundStartScale;
        }

    }
}
