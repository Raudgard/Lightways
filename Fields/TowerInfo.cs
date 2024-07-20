using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;


namespace Fields
{
    /// <summary>
    /// ���������� � ����� ��� ������������.
    /// </summary>
    public class TowerInfo : MatrixUnit
    {
        /// <summary>
        /// ������ -10000 => ����� �� �����-��������� ����, �� -10000 �� 0 => ����� �� ������� ����, 0 => ��������� �����, ������ 0 => ����� �� ������� � ���������� ����.
        /// </summary>
        public int winningWayIndex = -1;
        public bool isFinish = false;
        public bool IsStart => winningWayIndex == 0;

        public List<Direction> directions = new List<Direction>();
        public List<Direction> inputDirections = new List<Direction>();


        public IEnumerator GetEnumerator()
        {
            return directions.GetEnumerator();
        }

        public override string ToString()
        {
            string res = $"{{Sphere on ({X}, {Y}), winWayIndex: {winningWayIndex}. Directions: ";
            foreach (var dir in directions)
                res += dir + ", ";

            res.Remove(res.Length - 2);

            res += ". InputDirections: ";
            foreach (var dir in inputDirections)
                res += dir + ", ";

            res.Remove(res.Length - 2);
            res += "}";
            return res;
        }


        public static TowerInfo Copy(TowerInfo towerInfo)
        {
            return new TowerInfo()
            {
                directions = new List<Direction>(towerInfo.directions),
                inputDirections = new List<Direction>(towerInfo.inputDirections),
                isFinish = towerInfo.isFinish,
                winningWayIndex = towerInfo.winningWayIndex,
                X = towerInfo.X,
                Y = towerInfo.Y
            };
        }
    }
}

