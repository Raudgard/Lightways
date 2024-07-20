using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Fields
{
    /// <summary>
    /// ���������� � �������������� ���� ��� ��� ������ �� ����� �������� �������.
    /// </summary>
    public struct BeamSearchData
    {
        /// <summary>
        /// ����� ������ ����.
        /// </summary>
        public (int X, int Y) originPoint;

        /// <summary>
        /// ������, ����������� ���.
        /// </summary>
        public MatrixUnit emitter;

        /// <summary>
        /// ����� ��������� ����.
        /// </summary>
        public (int X, int Y) endPoint;

        /// <summary>
        /// ������, ����������� ���.
        /// </summary>
        public MatrixUnit reciever;

        /// <summary>
        /// ����������� ���� ��� ��� ������.
        /// </summary>
        public Direction originDirection;

        /// <summary>
        /// ����������� ���� ��� ��� ��������� (������� � ��������) � ����� ������ ���� (� �� �������). �� ������ ��������� � originDirection ��-�� ���������� ������� ������ ���.
        /// </summary>
        public Direction endDirection;

        /// <summary>
        /// ����������, ������� �������� ��� (��� ��������� �����). �������� ����� ���������� ����, ���� � ��� ��� �����.
        /// </summary>
        public List<(int X, int Y)> cellsUnderBeam;

        /// <summary>
        /// ������ ��������� ������� ������ ���.
        /// </summary>
        public List<BlackHoleInfluenceData> influencesBH;

        /// <summary>
        /// ��� ���������� �������������� ���? ��������� �������������� ��� influencesBH.Count > 32.
        /// </summary>
        public bool IsLoopingBeam => influencesBH.Count > 32;


        /// <summary>
        /// ����������� ���� � ������������ ����� (� ������ ��������� ������ ���).
        /// </summary>
        /// <param name="point">�����, � ������� ����� ������� �����������.</param>
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
        /// ���������� ������ ��������� ������ (��� � ������ occupiedSellsWinway) ����� ������� ��������� ������ ����. ���� ����� ���, �� ����� �������������, � �.�. ���� ������� ���, ���� ��������� ����� ����� ���, �� ���������� ������ ������.
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
        /// ���� �� ����� ����� ����������� � ����� ���� � �������� �����.
        /// </summary>
        /// <param name="beamSearchData">���, �� ����������� � ������� ����� ���������.</param>
        /// <param name="intersectsPoints">��������� ����� �����������.</param>
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

