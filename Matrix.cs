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
    /// Список телепортов.
    /// </summary>
    public List<TeleportInfo> Teleports => GetMatrixUnits<TeleportInfo>();

    /// <summary>
    /// Список телепортов для анализа уровней. Результат такой же как и у свойства Teleports, только список всегда составляется заново.
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
    /// Список черных дыр.
    /// </summary>
    public List<BlackHoleInfo> BlackHoles => GetMatrixUnits<BlackHoleInfo>();

    /// <summary>
    /// Словарь точек (по 4 шт. (сверху, снизу, слева и справа на 1 клетку)) рядом с черной дырой, при прохождении которых на луч света будет иметь влияние черная дыра, и сами ЧД.
    /// </summary>
    private Dictionary<(int X, int Y), BlackHoleInfo> pointsOfBlackHoleInfluence = new Dictionary<(int X, int Y), BlackHoleInfo>();

    [JsonIgnore]
    public Dictionary<(int X, int Y), BlackHoleInfo> PointsOfBlackHoleInfluence => pointsOfBlackHoleInfluence;

    private int objectsCount = 0;

    /// <summary>
    /// Количество объектов в матрице.
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
    /// Количество башен в матрице.
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
    /// Координаты, занятые какими-либо взаимодействиями, в которые нельзя ставить объекты ПОБЕДНОГО ПУТИ (другие можно), но луч света в них проходит. Например, клетки под лучом света, область вокруг черной дыры.
    /// Это МЕНЬШИЙ список координат, в отличии от occupiedSellsWinWay. Сюда не входят координаты клеток под лучом, вышедшим из неучтенных телепортов, когда их больше 2-х. Сюда МОЖНО ставить объекты ложного пути.
    /// </summary>
    [JsonIgnore]
    public List<(int X, int Y)> occupiedSells = new List<(int X, int Y)>();

    /// <summary>
    /// Координаты, занятые какими-либо взаимодействиями, в которые нельзя ставить никакие объекты (ни из ложного пути, ни из победного), т.к. это собьет последовательность прохождения победного пути, но луч света в них проходит.
    /// Например, клетки под лучом света, область вокруг черной дыры.
    /// Пояснение. Сюда записываются все координаты, по которым игрок может послать луч. В том числе и те, по которым пойдет луч из других порталов (если их больше 2-х).
    /// Это РАСШИРЕННЫЙ список координат, в отличии от occupiedSells.
    /// </summary>
    [JsonIgnore]
    public List<(int X, int Y)> occupiedSellsWinWay = new List<(int X, int Y)>();

    /// <summary>
    /// Количество использований телепортов в победном пути в этой матрице. Устанавливается при создании.
    /// </summary>
    [JsonIgnore]
    public int TeleportsUsedCount { get; set; }

    /// <summary>
    /// Количество использований черных дыр в победном пути в этой матрице. Устанавливается при создании.
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
    /// Возвращает упорядоченный по победному индексу список всех башен.
    /// </summary>
    /// <param name="withFinishTowers">Включая финишную башню.</param>
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
    /// Возвращает упорядоченный по победному индексу список всех башен победного пути.
    /// </summary>
    /// <param name="withFinishTower">Включая финишную башню.</param>
    /// <returns></returns>
    public List<TowerInfo> GetAllWinTowers(bool withFinishTower = true)
    {
        List<TowerInfo> winTowers = GetAllTowers(withFinishTower).Where(tower => tower.winningWayIndex > -1).ToList();
                
        //winTowers.Sort((a, b) => a.winningWayIndex.CompareTo(b.winningWayIndex));
        return winTowers;
    }

    /// <summary>
    /// Возвращает список всех башен с ложных путей.
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
    /// Возвращает список координат всех ячеек в матрице.
    /// </summary>
    /// <param name="withoutMatrixUnits">Без ячеек, уже занятых игровыми объектами.</param>
    /// <param name="includeOccupiedCells">Включая ячейки, занятые взаимодействиями (как пространство вокруг черной дыры).</param>
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
    /// Возвращает список всех объектов в матрице.
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
    /// Добавляет в список занятых взаимодействием ячеек у матрицы новые ячейки.
    /// </summary>
    /// <param name="matrix">Матрица, в которую нужно добавить новые ячейки.</param>
    /// <param name="occupiedCells">Список новых занятых ячеек.</param>
    /// <param name="onlyToOccupiedCellsWinWay">true - добавить только в расширенный список победного пути, false - и в расширенный, и в уменьшенный списки.</param>
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
    /// Добавляет в список точек влияний черных дыр, имеющихся в матрице. Необходимо запускать этот метод ПОСЛЕ загрузки матрицы из файла.
    /// </summary>
    /// <param name="blackHoleInfo">Информация о конкретной черной дыре, 4 точки влияния которой нужно добавить. Если null, то добавляет эти точки по ВСЕМ черным дырам в матрице.</param>
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
    /// Посылает гипотетический луч в матрице от начальной координаты в указанном направлении.
    /// </summary>
    /// <param name="X">Координата Х начальной точки.</param>
    /// <param name="Y">Координата Y начальной точки.</param>
    /// <param name="direction">В каком направлении проверять.</param>
    /// <returns>Первый объект на пути. Если объекта нет, то null.</returns>
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
        var lastFreeCells = new List<(int X, int Y)>();    //ссылка на список свободных клеток у последней черной дыры.

        //Сохраняем предыдущую точку прохождения луча. Она нужна для определения точки начала искривления луча под воздействием черной дыры.
        (int X, int Y) previousPoint = (X, Y);      //предыдущая точка.
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

            //в случае, если след. точка это черная дыра. Т.е. луч упирается в нее.
            if (intoBlackHole)
            {
                //Debug.Log($"луч упирается в черную дыру: {outputPoint}");
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
    /// Рассчитывает точку пересечения гипотетических лучей, учитывая объекты, находящиеся в матрице (то есть берет свободные точки только ДО них), но не учитывает точки, занятые взаимодействиями.
    /// </summary>
    /// <param name="firstPoint">Начало первого луча.</param>
    /// <param name="firstDirection">Направление первого луча.</param>
    /// <param name="secondPoint">Начало второго луча</param>
    /// <param name="secondDirection">Направление второго луча.</param>
    /// <param name="intersectionPoint">Искомая точка пересечения.</param>
    /// <returns>true - если точка пересечения найдена (у обоих лучей она будет endPoint), false - если не найдена (тогда endPoint у лучей будут либо объекты, в которые он упирается, либо конец матрицы).</returns>
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
    /// Удаляет все черные дыры из матрицы.
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
