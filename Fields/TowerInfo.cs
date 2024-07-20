using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;


namespace Fields
{
    /// <summary>
    /// Информация о башне для сериализации.
    /// </summary>
    public class TowerInfo : MatrixUnit
    {
        /// <summary>
        /// меньше -10000 => башня из ложно-победного пути, от -10000 до 0 => башня из ложного пути, 0 => стартовая башня, больше 0 => число по очереди в выигрышном пути.
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

