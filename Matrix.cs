using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Fields;
using System.Linq;
using System;
using UnityEngine;

[Serializable]
public class Matrix
{
    #region Singletone

    private static Matrix instance;
    public static Matrix Instance => instance;

    #endregion

    [JsonRequired]
    private MatrixUnit[,] matrix;

    public MatrixUnit this[int i, int j]
    {
        get { return matrix[i, j]; }
        private set { matrix[i, j] = value; }
    }

    private List<TeleportInfo> teleports = new List<TeleportInfo>();
    private List<BlackHoleInfo> blackHoles = new List<BlackHoleInfo>();

    

    /// <summary>
    /// ������ ����������.
    /// </summary>
    public List<TeleportInfo> Teleports => GetMatrixUnits<TeleportInfo>();

    /// <summary>
    /// ������ ���������� ��� ������� �������. ��������� ����� �� ��� � � �������� Teleports, ������ ������ ������ ������������ ������.
    /// </summary>
    public List<TeleportInfo> TeleportsForAnalyzing
    {
        get
        {
            List<TeleportInfo> teleports = new List<TeleportInfo>();

            foreach (MatrixUnit unit in matrix)
            {
                if (unit is TeleportInfo teleport)
                {
                    teleports.Add(teleport);
                }
            }
            return teleports;
        }
    }


    /// <summary>
    /// ������ ������ ���.
    /// </summary>
    public List<BlackHoleInfo> BlackHoles => GetMatrixUnits<BlackHoleInfo>();

    /// <summary>
    /// ������� ����� (�� 4 ��. (������, �����, ����� � ������ �� 1 ������)) ����� � ������ �����, ��� ����������� ������� �� ��� ����� ����� ����� ������� ������ ����, � ���� ��.
    /// </summary>
    private Dictionary<(int X, int Y), BlackHoleInfo> pointsOfBlackHoleInfluence = new Dictionary<(int X, int Y), BlackHoleInfo>();

    [JsonIgnore]
    public Dictionary<(int X, int Y), BlackHoleInfo> PointsOfBlackHoleInfluence => pointsOfBlackHoleInfluence;

    private int objectsCount = 0;

    /// <summary>
    /// ���������� �������� � �������.
    /// </summary>
    public int ObjectsCount
    {
        get
        {
            if (objectsCount != 0)
                return objectsCount;
            else
            {
                int res = 0;
                for (int i = 0; i < SizeX; i++)
                {
                    for (int j = 0; j < SizeY; j++)
                    {
                        if (matrix[i, j] != null) res++;
                    }
                }
                return res;
            }
        }
    }

    private int towersCount = 0;

    /// <summary>
    /// ���������� ����� � �������.
    /// </summary>
    public int TowersCount
    {
        get
        {
            if(towersCount != 0)
                return towersCount;
            else
            {
                return GetMatrixUnits<TowerInfo>(false).Count;
            }
        }
    }

    /// <summary>
    /// ����������, ������� ������-���� ����������������, � ������� ������ ������� ������� ��������� ���� (������ �����), �� ��� ����� � ��� ��������. ��������, ������ ��� ����� �����, ������� ������ ������ ����.
    /// ��� ������� ������ ���������, � ������� �� occupiedSellsWinWay. ���� �� ������ ���������� ������ ��� �����, �������� �� ���������� ����������, ����� �� ������ 2-�. ���� ����� ������� ������� ������� ����.
    /// </summary>
    [JsonIgnore]
    public List<(int X, int Y)> occupiedSells = new List<(int X, int Y)>();

    /// <summary>
    /// ����������, ������� ������-���� ����������������, � ������� ������ ������� ������� ������� (�� �� ������� ����, �� �� ���������), �.�. ��� ������ ������������������ ����������� ��������� ����, �� ��� ����� � ��� ��������.
    /// ��������, ������ ��� ����� �����, ������� ������ ������ ����.
    /// ���������. ���� ������������ ��� ����������, �� ������� ����� ����� ������� ���. � ��� ����� � ��, �� ������� ������ ��� �� ������ �������� (���� �� ������ 2-�).
    /// ��� ����������� ������ ���������, � ������� �� occupiedSells.
    /// </summary>
    [JsonIgnore]
    public List<(int X, int Y)> occupiedSellsWinWay = new List<(int X, int Y)>();

    /// <summary>
    /// ���������� ������������� ���������� � �������� ���� � ���� �������. ��������������� ��� ��������.
    /// </summary>
    [JsonIgnore]
    public int TeleportsUsedCount { get; set; }

    /// <summary>
    /// ���������� ������������� ������ ��� � �������� ���� � ���� �������. ��������������� ��� ��������.
    /// </summary>
    [JsonIgnore]
    public int BlackHolesUsedCount { get; set; }

    [JsonConstructor]
    public Matrix(int Xsize, int Ysize)
    {
        matrix = new MatrixUnit[Xsize, Ysize];
        instance = this;
    }


    public void SetSizeOfMatrix(int Xsize, int Ysize)
    {
        matrix = new MatrixUnit[Xsize, Ysize];
    }

    public int SizeX => matrix.GetLength(0);
    public int SizeY => matrix.GetLength(1);


    private List<T> GetMatrixUnits<T>(bool sorted = true) where T : MatrixUnit
    {
        var type = typeof(T);

        if (type == typeof(TowerInfo))
        {
            List<TowerInfo> towers = GetUnits<TowerInfo>();
            if (sorted)
            {
                towers.Sort((a, b) => a.winningWayIndex.CompareTo(b.winningWayIndex));
            }
            return towers as List<T>;
        }

        else if (type == typeof(TeleportInfo))
        {
            if (teleports != null)
            {
                return teleports as List<T>;
            }
            else
            {
                List<TeleportInfo> teleports = GetUnits<TeleportInfo>();
                if (sorted)
                {
                    teleports.Sort((a, b) => a.ToString().CompareTo(b.ToString()));
                }
                this.teleports = teleports;
                return teleports as List<T>;
            }
        }

        else if (type == typeof(BlackHoleInfo))
        {
            if (blackHoles != null)
                return blackHoles as List<T>;
            else
            {
                List<BlackHoleInfo> blackHoles = GetUnits<BlackHoleInfo>();
                if (sorted)
                {
                    blackHoles.Sort((a, b) => a.ToString().CompareTo(b.ToString()));
                }
                this.blackHoles = blackHoles;
                return blackHoles as List<T>;
            }
        }

        else
        {
            throw new NotImplementedException();
        }

        List<B> GetUnits<B>() where B : MatrixUnit
        {
            List<B> units = new List<B>();

            foreach (MatrixUnit unit in matrix)
            {
                if (unit is B)
                {
                    units.Add((B)unit);
                }
            }
            return units;
        }
    }

    /// <summary>
    /// ���������� ������������� �� ��������� ������� ������ ���� �����.
    /// </summary>
    /// <param name="withFinishTowers">������� �������� �����.</param>
    /// <returns></returns>
    public List<TowerInfo> GetAllTowers(bool withFinishTowers = true)
    {
        var towers = GetMatrixUnits<TowerInfo>();
        //if (!withFinishTower)
        //    towers.RemoveAt(towers.Count - 1);
        if (!withFinishTowers)
            towers = towers.Except(towers.Where(t => t.isFinish)).ToList();

        return towers;
    }


    /// <summary>
    /// ���������� ������������� �� ��������� ������� ������ ���� ����� ��������� ����.
    /// </summary>
    /// <param name="withFinishTower">������� �������� �����.</param>
    /// <returns></returns>
    public List<TowerInfo> GetAllWinTowers(bool withFinishTower = true)
    {
        List<TowerInfo> winTowers = GetAllTowers(withFinishTower).Where(tower => tower.winningWayIndex > -1).ToList();
                
        //winTowers.Sort((a, b) => a.winningWayIndex.CompareTo(b.winningWayIndex));
        return winTowers;
    }

    /// <summary>
    /// ���������� ������ ���� ����� � ������ �����.
    /// </summary>
    /// <returns></returns>
    public List<TowerInfo> GetAllFalseTower()
    {
        return GetAllTowers().Where(tower => tower.winningWayIndex < 0).ToList();
    }

    public IEnumerator GetEnumerator()
    {
        return matrix.GetEnumerator();
    }

    /// <summary>
    /// ���������� ������ ��������� ���� ����� � �������.
    /// </summary>
    /// <param name="withoutMatrixUnits">��� �����, ��� ������� �������� ���������.</param>
    /// <param name="includeOccupiedCells">������� ������, ������� ���������������� (��� ������������ ������ ������ ����).</param>
    /// <returns></returns>
    public List<(int X, int Y)> GetCoordinatesOfAllCells(bool withoutMatrixUnits, bool includeOccupiedCells)
    {
        List<(int X, int Y)> res = new List<(int X, int Y)>();
        int length0 = matrix.GetLength(0);
        int length1 = matrix.GetLength(1);

        for (int i = 0; i < length0; i++)
        {
            for (int j = 0; j < length1; j++)
            {
                if (withoutMatrixUnits && matrix[i,j] != null)
                    continue;



                if ((!includeOccupiedCells) && occupiedSellsWinWay.Contains((i, j)))
                {
                    //Debug.Log($"occupied sell include: ({i}, {j})");
                    continue;
                }

                res.Add((i, j));
                //Debug.Log($"added: ({i}, {j}");
            }
        }

        return res;
    }

    /// <summary>
    /// ���������� ������ ���� �������� � �������.
    /// </summary>
    /// <returns></returns>
    public List<MatrixUnit> GetAllUnits()
    {
        List<MatrixUnit> res = new List<MatrixUnit>();
        int length0 = matrix.GetLength(0);
        int length1 = matrix.GetLength(1);

        for (int i = 0; i < length0; i++)
        {
            for (int j = 0; j < length1; j++)
            {
                if (matrix[i, j] != null)
                    res.Add(matrix[i, j]);
            }
        }
        return res;
    }

    /// <summary>
    /// ��������� � ������ ������� ��������������� ����� � ������� ����� ������.
    /// </summary>
    /// <param name="matrix">�������, � ������� ����� �������� ����� ������.</param>
    /// <param name="occupiedCells">������ ����� ������� �����.</param>
    /// <param name="onlyToOccupiedCellsWinWay">true - �������� ������ � ����������� ������ ��������� ����, false - � � �����������, � � ����������� ������.</param>
    public void AddOccupiedCells(List<(int X, int Y)> occupiedCells, bool onlyToOccupiedCellsWinWay)
    {
        if (onlyToOccupiedCellsWinWay)
        {
            var adding = occupiedCells.Except(occupiedSellsWinWay).ToList();
            //Debug.Log($"adding OccupiedCellsWinWay count: {adding.Count()}: {adding.ToStringEverythingInARow()}");
            occupiedSellsWinWay.AddRange(adding);
        }
        else
        {
            var adding = occupiedCells.Except(occupiedSells).ToList();
            //Debug.Log($"adding to OccupiedCellsWinWay and to OccupiedCells count: {adding.Count()}: {adding.ToStringEverythingInARow()}");
            occupiedSells.AddRange(adding);
            occupiedSellsWinWay.AddRange(adding);
        }
    }




    public TowerInfo AddTower(int X, int Y, int winningWayIndex, bool isFinish = false)
    {
        towersCount++;
        objectsCount++;
        var towerInfo = new TowerInfo()
        {
            X = X,
            Y = Y,
            winningWayIndex = winningWayIndex,
            isFinish = isFinish
        };

        //Debug.Log($"AddTower on: ({X}, {Y}), winwayIndex: {winningWayIndex}");

        matrix[X, Y] = towerInfo;
        return towerInfo;
    }

    public TowerInfo AddTower(TowerInfo towerInfo)
    {
        towersCount++;
        objectsCount++;
        matrix[towerInfo.X, towerInfo.Y] = towerInfo;
        return towerInfo;
    }


    public TeleportInfo AddTeleport(int X, int Y)
    {
        objectsCount++;
        if (teleports == null)
            teleports = new List<TeleportInfo>();

        var teleportInfo = new TeleportInfo()
        {
            X = X,
            Y = Y,
        };

        matrix[X, Y] = teleportInfo;
        teleports.Add(teleportInfo);
        return teleportInfo;
    }

    public BlackHoleInfo AddBlackHole(int X, int Y, BlackHoleInfo.BlackHoleSize size)
    {
        objectsCount++;
        var blackHoleInfo = new BlackHoleInfo(X, Y, size);

        List<(int X, int Y)> occupSells = new List<(int X, int Y)>
        {
            (X, Y + 1),
            (X - 1, Y + 1),
            (X - 1, Y),
            (X - 1, Y - 1),
            (X, Y - 1),
            (X + 1, Y - 1),
            (X + 1, Y),
            (X + 1, Y + 1),
        };

        AddOccupiedCells(occupSells, false);
        AddPointsOfBlackHolesInfluence(blackHoleInfo);

        matrix[X, Y] = blackHoleInfo;
        blackHoles.Add(blackHoleInfo);
        return blackHoleInfo;
    }

    /// <summary>
    /// ��������� � ������ ����� ������� ������ ���, ��������� � �������. ���������� ��������� ���� ����� ����� �������� ������� �� �����.
    /// </summary>
    /// <param name="blackHoleInfo">���������� � ���������� ������ ����, 4 ����� ������� ������� ����� ��������. ���� null, �� ��������� ��� ����� �� ���� ������ ����� � �������.</param>
    public void AddPointsOfBlackHolesInfluence(BlackHoleInfo blackHoleInfo = null)
    {
        if (blackHoleInfo != null)
        {
            int X = blackHoleInfo.X;
            int Y = blackHoleInfo.Y;
            pointsOfBlackHoleInfluence.Add((X, Y + 1), blackHoleInfo);
            pointsOfBlackHoleInfluence.Add((X - 1, Y), blackHoleInfo);
            pointsOfBlackHoleInfluence.Add((X, Y - 1), blackHoleInfo);
            pointsOfBlackHoleInfluence.Add((X + 1, Y), blackHoleInfo);
            //Debug.Log("pointsOfBlackHoleInfluence.Add");
            return;
        }

        foreach(var BH in BlackHoles)
        {
            AddPointsOfBlackHolesInfluence(BH);
        }
    }


    /// <summary>
    /// �������� �������������� ��� � ������� �� ��������� ���������� � ��������� �����������.
    /// </summary>
    /// <param name="X">���������� � ��������� �����.</param>
    /// <param name="Y">���������� Y ��������� �����.</param>
    /// <param name="direction">� ����� ����������� ���������.</param>
    /// <returns>������ ������ �� ����. ���� ������� ���, �� null.</returns>
    public MatrixUnit BeamCast(int X, int Y, Direction direction, out BeamSearchData beam)
    {
        beam = new BeamSearchData() 
        {
            originPoint = (X, Y),
            originDirection = direction,
            emitter = matrix[X, Y],
        };

        //blackHolesData = new List<BlackHoleInfluenceData>();
        beam.influencesBH = new List<BlackHoleInfluenceData>();
        int Xmax = matrix.GetLength(0) - 1;
        int Ymax = matrix.GetLength(1) - 1;
        //Debug.Log($"Xmax: {Xmax}, Ymax: {Ymax}");
        //freeCells = new List<(int X, int Y)>();
        beam.cellsUnderBeam = new List<(int X, int Y)>();

        var steps = Calculations.NextCellOnDirection((0, 0), direction);
        int Xstep = steps.X;
        int Ystep = steps.Y;
        var lastFreeCells = new List<(int X, int Y)>();    //������ �� ������ ��������� ������ � ��������� ������ ����.

        //��������� ���������� ����� ����������� ����. ��� ����� ��� ����������� ����� ������ ����������� ���� ��� ������������ ������ ����.
        (int X, int Y) previousPoint = (X, Y);      //���������� �����.
        X += Xstep;
        Y += Ystep;

        while (X > -1 && X <= Xmax && Y > -1 && Y <= Ymax)
        {
            //Debug.Log($"(X, Y): {(X, Y)}");
            //Debug.Log($"pointsOfBlackHoleInfluence: {pointsOfBlackHoleInfluence.Keys.ToList().ToStringEverythingInARow()}");
            if (beam.IsLoopingBeam)
                break;


            if (IsPointAffectedByBlackHole((X, Y), previousPoint, direction, out var blackHoleInfluenceData))
            {
                blackHoleInfluenceData.freeCellsAfterBH = new List<(int X, int Y)>();
                lastFreeCells = blackHoleInfluenceData.freeCellsAfterBH;
                //blackHolesData.Add(blackHoleInfluenceData);
                beam.influencesBH.Add(blackHoleInfluenceData);

                direction = blackHoleInfluenceData.directionAfter;
                steps = Calculations.NextCellOnDirection((0, 0), direction);
                Xstep = steps.X;
                Ystep = steps.Y;

                X = blackHoleInfluenceData.outputPoint.X;
                Y = blackHoleInfluenceData.outputPoint.Y;
                previousPoint = (X, Y);
                X += Xstep;
                Y += Ystep;
                continue;
            }

            var unit = matrix[X, Y];
            if (unit != null)
            {
                beam.endDirection = direction;
                beam.reciever = unit;
                beam.endPoint = (X, Y);
                return unit;
            }

            //freeCells.Add((X, Y));
            beam.cellsUnderBeam.Add((X, Y));
            lastFreeCells.Add((X, Y));
            previousPoint = (X, Y);
            X += Xstep;
            Y += Ystep;
        }

        //Debug.Log($"freeCells: {freeCells.Count}, blackHolesData.Count: {blackHolesData.Count}");
        beam.endDirection = direction;
        beam.reciever = null;
        beam.endPoint = (X -= Xstep, Y -= Ystep);
        return null;
    }


    private bool IsPointAffectedByBlackHole((int X, int Y) point, (int X, int Y) previousPoint, Direction direction, out BlackHoleInfluenceData blackHoleInfluenceData)
    {
        blackHoleInfluenceData = new BlackHoleInfluenceData();
        if (pointsOfBlackHoleInfluence.ContainsKey(point))
        {
            //Debug.Log($"Input point of Black Hole influence: ({X}, {Y})");
            var blackHole = pointsOfBlackHoleInfluence[point];
            var outputPoint = Calculations.GetOutputPointAndDirectionAfterBlackHoleInfluence(point, direction, blackHole, out var directionAfter, out bool intoBlackHole);
            //Debug.Log($"outputPoint: {outputPoint}");

            //� ������, ���� ����. ����� ��� ������ ����. �.�. ��� ��������� � ���.
            if (intoBlackHole)
            {
                //Debug.Log($"��� ��������� � ������ ����: {outputPoint}");
                return false;
            }

            blackHoleInfluenceData.blackHole = blackHole;
            blackHoleInfluenceData.directionBefore = direction;
            blackHoleInfluenceData.directionAfter = directionAfter;
            blackHoleInfluenceData.inputPoint = direction.IsDiagonal() ? point : previousPoint;
            blackHoleInfluenceData.outputPoint = outputPoint;
            blackHoleInfluenceData.isBeamGoingIntoBlackHole = intoBlackHole;
            return true;
        }

        return false;
    }


    /// <summary>
    /// ������������ ����� ����������� �������������� �����, �������� �������, ����������� � ������� (�� ���� ����� ��������� ����� ������ �� ���), �� �� ��������� �����, ������� ����������������.
    /// </summary>
    /// <param name="firstPoint">������ ������� ����.</param>
    /// <param name="firstDirection">����������� ������� ����.</param>
    /// <param name="secondPoint">������ ������� ����</param>
    /// <param name="secondDirection">����������� ������� ����.</param>
    /// <param name="intersectionPoint">������� ����� �����������.</param>
    /// <returns>true - ���� ����� ����������� ������� (� ����� ����� ��� ����� endPoint), false - ���� �� ������� (����� endPoint � ����� ����� ���� �������, � ������� �� ���������, ���� ����� �������).</returns>
    //public bool GetIntersectionOfDirections((int X, int Y) firstPoint, Direction firstDirection, (int X, int Y) secondPoint, Direction secondDirection, out (int X, int Y) intersectionPoint, out List<BlackHoleInfluenceData> BHInfluenceForFirstPoint, out List<BlackHoleInfluenceData> BHInfluenceForSecondPoint)
    public bool GetIntersectionOfDirections((int X, int Y) firstPoint, Direction firstDirection, (int X, int Y) secondPoint, Direction secondDirection, out BeamSearchData beamFromFirstPoint, out BeamSearchData beamFromSecondPoint)
    {
        //CheckDirectionFrom(firstPoint.X, firstPoint.Y, firstDirection, out var firstDirectionFreeCells, out BHInfluenceForFirstPoint);
        //CheckDirectionFrom(secondPoint.X, secondPoint.Y, secondDirection, out var secondDirectionFreeCells, out BHInfluenceForSecondPoint);
        BeamCast(firstPoint.X, firstPoint.Y, firstDirection, out beamFromFirstPoint);
        BeamCast(secondPoint.X, secondPoint.Y, secondDirection, out beamFromSecondPoint);
        //BHInfluenceForFirstPoint = beamFromFirstPoint.influencesBH;
        //BHInfluenceForSecondPoint = beamFromSecondPoint.influencesBH;

        //var intersect = firstDirectionFreeCells.Intersect(secondDirectionFreeCells).ToArray();
        var intersect = beamFromFirstPoint.cellsUnderBeam.Intersect(beamFromSecondPoint.cellsUnderBeam).ToArray();
        
        if (intersect.Length > 0)
        {
            //intersectionPoint = intersect[0];
            beamFromFirstPoint.endPoint = beamFromSecondPoint.endPoint = intersect[0];
            //Debug.Log($"GetIntersectionOfDirections: firstPoint: {firstPoint}, firstDirection: {firstDirection}, secondPoint: {secondPoint}, secondDirection: {secondDirection}, intersect.length: {intersect.Length}, intersectionPoint: {intersectionPoint} ");
            return true;
        }

        //Debug.Log($"GetIntersectionOfDirections: firstPoint: {firstPoint}, firstDirection: {firstDirection}, secondPoint: {secondPoint}, secondDirection: {secondDirection}, intersect.length: {intersect.Length}, intersectionPoint: NONE ");

        //intersectionPoint = (-1000, -1000);
        return false;

    }


    public void DeleteMatrixUnit(int X, int Y)
    {
        if (matrix[X, Y] == null)
        {
            return;
        }

        else if (matrix[X, Y] is TowerInfo || matrix[X, Y] is TeleportInfo)
        {
            matrix[X, Y] = null;
        }

        else if(matrix[X, Y] is BlackHoleInfo)
        {
            matrix[X, Y] = null;
            pointsOfBlackHoleInfluence.Remove((X, Y + 1));
            pointsOfBlackHoleInfluence.Remove((X - 1, Y));
            pointsOfBlackHoleInfluence.Remove((X, Y - 1));
            pointsOfBlackHoleInfluence.Remove((X + 1, Y));
        }

        objectsCount--;
    }



    /// <summary>
    /// ������� ��� ������ ���� �� �������.
    /// </summary>
    public void DeleteAllBlackHoles()
    {
        //Debug.Log($"Delete blackHoles: {BlackHoles.ToStringEverythingInARow()}");
        BlackHoles.ForEach(bh => { matrix[bh.X, bh.Y] = null; objectsCount--; });
        //blackHoles = null;
        blackHoles.Clear();
        pointsOfBlackHoleInfluence.Clear();
    }


    //public Matrix (Matrix matrix)
    //{
    //    int length0 = matrix.matrix.GetLength(0);
    //    int length1 = matrix.matrix.GetLength(1);

    //    this.matrix = new MatrixUnit[length0, length1];
    //    for (int i = 0; i < length0; i++)
    //    {
    //        for (int j = 0; j < length1; j++)
    //        {
    //            this.matrix[i, j] = matrix.matrix[i, j];
    //        }
    //    }

    //    teleports = new List<TeleportInfo>();
    //    blackHoles = new List<BlackHoleInfo>();
    //    pointsOfBlackHoleInfluence = new Dictionary<(int X, int Y), BlackHoleInfo>(pointsOfBlackHoleInfluence);
    //    instance = null;
    //}


}
