using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Fields
{
    /// <summary>
    /// Информация о гипотетическом луче при его поиске во время создания матрицы.
    /// </summary>
    public struct BeamSearchData
    {
        /// <summary>
        /// Точка начала луча.
        /// </summary>
        public (int X, int Y) originPoint;

        /// <summary>
        /// Объект, испускающий луч.
        /// </summary>
        public MatrixUnit emitter;

        /// <summary>
        /// Точка окончания луча.
        /// </summary>
        public (int X, int Y) endPoint;

        /// <summary>
        /// Объект, принимающий луч.
        /// </summary>
        public MatrixUnit reciever;

        /// <summary>
        /// Направление луча при его старте.
        /// </summary>
        public Direction originDirection;

        /// <summary>
        /// Направление луча при его окончании (встрече с объектом) с точки зрения луча (а не объекта). Не всегда совпадает с originDirection из-за возможного влияния черных дыр.
        /// </summary>
        public Direction endDirection;

        /// <summary>
        /// Координаты, которые занимает луч (без начальной точки). Конечная точка включается сюда, если в ней нет юнита.
        /// </summary>
        public List<(int X, int Y)> cellsUnderBeam;

        /// <summary>
        /// Список возможных влияний черных дыр.
        /// </summary>
        public List<BlackHoleInfluenceData> influencesBH;

        /// <summary>
        /// Это бесконечно закольцованный луч? Считается закольцованным при influencesBH.Count > 32.
        /// </summary>
        public bool IsLoopingBeam => influencesBH.Count > 32;


        /// <summary>
        /// Направление луча в определенной точке (с учетом возможных черных дыр).
        /// </summary>
        /// <param name="point">Точка, в которой нужно вяснить направление.</param>
        /// <returns></returns>
        public Direction DirectionInPoint((int X, int Y) point, out bool wasThereBlackHoleInfluence)
        {
            for (int i = 0; i < influencesBH.Count; i++)
            {
                if (influencesBH[i].freeCellsAfterBH.Contains(point))
                {
                    wasThereBlackHoleInfluence = true;
                    return influencesBH[i].directionAfter;
                }
            }

            wasThereBlackHoleInfluence = false;
            return originDirection;
        }

        /// <summary>
        /// Возвращает список свободных клеток (уже с учетом occupiedSellsWinway) после влияния последней черной дыры. Если таких нет, то после предпоследней, и т.д. Если влияний нет, либо свободных точек нигде нет, то возвращает пустой список.
        /// </summary>
        /// <returns></returns>
        public List<(int X, int Y)> FreeCellsAfterLastBlackHole
        {
            get
            {
                for (int i = influencesBH.Count - 1; i > -1; i--)
                {
                    var points = influencesBH[i].freeCellsAfterBH.Except(Matrix.Instance.occupiedSellsWinWay).ToList();
                    if (points.Count > 0)
                        return points;
                }

                return new List<(int X, int Y)>();
            }
        }


        /// <summary>
        /// Есть ли общие точки пересечения у этого луча с заданным лучом.
        /// </summary>
        /// <param name="beamSearchData">Луч, на пересечение с которым нужно проверить.</param>
        /// <param name="intersectsPoints">Возможные точки пересечения.</param>
        /// <returns></returns>
        public bool IntersectsWith(BeamSearchData beamSearchData, out List<(int X, int Y)> intersectsPoints)
        {
            intersectsPoints = cellsUnderBeam.Intersect(beamSearchData.cellsUnderBeam).ToList();
            return intersectsPoints.Count > 0;
        }








        public override string ToString()
        {
            return $"{{BeamSearchData:  originPoint: {originPoint}, " +
                $"emitter: {emitter}, " +
                $"endPoint: {endPoint}, " +
                $"reciever: {reciever}, " +
                $"originDirection: {originDirection}, " +
                $"endDirection: {endDirection}, " +
                $"cellsUnderBeam: {cellsUnderBeam?.ToStringEverythingInARow()}, " +
                $"influencesBH: {influencesBH?.ToStringEverythingInARow()}.}}";
        }
    }
}

