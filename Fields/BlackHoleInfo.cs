using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class BlackHoleInfo : MatrixUnit
{
    public enum BlackHoleSize
    {
        Small = 45,
        Medium = 90,
        Large = 135,
        Supermassive = 180
    }

    public BlackHoleSize size;

    [JsonIgnore]
    /// <summary>
    /// 4 точки влияния ЧД, и по 4 корректных направления от каждой из них, по которым можно искать лучи для воздействия этой ЧД.
    /// Например, от точки "выше" ЧД идут 4 направления: влево, вверх-влево, вверх-вправо и вправо.
    /// </summary>
    public Dictionary<(int X, int Y), Direction[]> InfluencePointsAndCorrectDirectionsFROMThey;

    public BlackHoleInfo(int X, int Y, BlackHoleSize size)
    {
        this.X = X;
        this.Y = Y;
        this.size = size;

        InfluencePointsAndCorrectDirectionsFROMThey = new Dictionary<(int X, int Y), Direction[]>()
        {
            {
                (X, Y + 1), new Direction[4]
                { Direction.Left,
                Direction.UpLeft,
                Direction.UpRight,
                Direction.Right} 
            },

            {
                (X - 1, Y), new Direction[4]
                { Direction.Down,
                Direction.DownLeft,
                Direction.UpLeft,
                Direction.Up}
            },

            {
                (X, Y - 1), new Direction[4]
                { Direction.Left,
                Direction.DownLeft,
                Direction.DownRight,
                Direction.Right}
            },

            {
                (X + 1, Y), new Direction[4]
                { Direction.Up,
                Direction.UpRight,
                Direction.DownRight,
                Direction.Down}
            },
        };


    }


}
