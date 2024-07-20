using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using Fields;
using System.Linq;
using System;

public static class Calculations
{
    /// <summary>
    /// Все восемь направлений в списке.
    /// </summary>
    public static List<Direction> AllDirectionsList
    {
        get
        {
            List<Direction> directions = new List<Direction>();
            directions.Add(Direction.Up);
            directions.Add(Direction.UpLeft);
            directions.Add(Direction.Left);
            directions.Add(Direction.DownLeft);
            directions.Add(Direction.Down);
            directions.Add(Direction.DownRight);
            directions.Add(Direction.Right);
            directions.Add(Direction.UpRight);
            return directions;
        }
    }
    /// <summary>
    /// Все восемь направлений в массиве.
    /// </summary>
    public static Direction[] AllDirectionsArray
    {
        get
        {
            Direction[] directions = new Direction[8];
            directions[0] = Direction.Up;
            directions[1] = Direction.UpLeft;
            directions[2] = Direction.Left;
            directions[3] = Direction.DownLeft;
            directions[4] = Direction.Down;
            directions[5] = Direction.DownRight;
            directions[6] = Direction.Right;
            directions[7] = Direction.UpRight;
            return directions;
        }
    }

    /// <summary>
    /// 4 направления, которые при продлении образуют все 8 линий.
    /// </summary>
    public static List<Direction> HalfDirectionsList
    {
        get
        {
            List<Direction> directions = new List<Direction>();
            directions.Add(Direction.Up);
            directions.Add(Direction.UpLeft);
            directions.Add(Direction.Left);
            directions.Add(Direction.DownLeft);
            return directions;
        }
    }


    /// <summary>
    /// Возвращает координаты следующей ячейки по направлению от заданной.
    /// </summary>
    /// <param name="from">Координаты ячейки, от которой нужно смотреть.</param>
    /// <param name="direction">Направление, по которому нужно смотреть.</param>
    /// <returns></returns>
    public static (int X, int Y) NextCellOnDirection((int X, int Y) from, Direction direction)
    {
        int halfPosition = (int)direction - 180;

        int Xstep = MathF.Sign(halfPosition % 180);
        int Ystep = MathF.Sign((MathF.Abs(halfPosition) - 90) % 180);

        return (from.X + Xstep, from.Y + Ystep);
    }


    /// <summary>
    /// Вычисляет параметы для Shape модуля белой пыли вокруг луча света.
    /// </summary>
    /// <param name="direction">Направление луча.</param>
    /// <param name="sphereCoordinate">Координаты сферы, испускающей луч.</param>
    /// <param name="beamDestination">Координаты окончания луча.</param>
    /// <param name="rotation">Угол поворота для Shape модуля.</param>
    /// <param name="scaleX">Длина Shape модуля.</param>
    /// <returns>Координаты самого Shape модуля.</returns>
    public static Vector3 GetShapeForWhiteDust(Direction direction, Vector3 sphereCoordinate, Vector3 beamDestination, out float rotation, out float scaleX)
    {
        rotation = direction switch
        {
            Direction.Up => -90,
            Direction.UpLeft => -45,
            Direction.Left => 0,
            Direction.DownLeft => 45,
            Direction.Down => 90,
            Direction.DownRight => 135,
            Direction.Right => 180,
            Direction.UpRight => -135,
            _ => throw new System.NotImplementedException()
        };


        Vector2 beamVector = beamDestination - sphereCoordinate;
        scaleX = beamVector.magnitude;
        Vector2 halfBeamVector = beamVector / 2;

        //Vector3 coordinates = new Vector3((beamDestination.x - sphereCoordinate.x) / 2, (beamDestination.y - sphereCoordinate.y) / 2, 0);
        Vector3 coordinates = new Vector3(halfBeamVector.x, halfBeamVector.y, 0);

        return coordinates;
    }


    /// <summary>
    /// Рассчитывает направления лучей, исходящих из двух заданных точек, которые не пересекаются. Каждой точке присваиваются 2 либо 3 таких направления.
    /// </summary>
    /// <param name="firstPoint">Координаты первой точки.</param>
    /// <param name="secondPoint">Координаты второй точки.</param>
    /// <returns>Две заданные точки и по 2 либо 3 направления для каждой из них.</returns>
    public static void GetDisjointDirections((int X, int Y) firstPoint, (int X, int Y) secondPoint, out Direction[] directionsForFirstPoint, out Direction[] directionsForSecondPoint)
    {
        if (firstPoint.Y > secondPoint.Y && firstPoint.X > secondPoint.X)
        {
            if (firstPoint.Y - secondPoint.Y > firstPoint.X - secondPoint.X)
            {
                directionsForFirstPoint = new Direction[2]
                {
                    Direction.Up,
                    Direction.UpRight
                };
                directionsForSecondPoint = new Direction[2]
                {
                    Direction.Down,
                    Direction.DownLeft
                };
                return;
            }

            if (firstPoint.Y - secondPoint.Y < firstPoint.X - secondPoint.X)
            {

                directionsForFirstPoint = new Direction[2]
                {
                    Direction.Right,
                    Direction.UpRight
                };
                directionsForSecondPoint = new Direction[2]
                {
                    Direction.Left,
                    Direction.DownLeft
                };
                return;
            }

            if (firstPoint.Y - secondPoint.Y == firstPoint.X - secondPoint.X)
            {
                directionsForFirstPoint = new Direction[3]
                {
                    Direction.Right,
                    Direction.UpRight,
                    Direction.Up
                };
                directionsForSecondPoint = new Direction[3]
                {
                    Direction.Left,
                    Direction.DownLeft,
                    Direction.Down
                };
                return;
            }
        }

        else if (firstPoint.Y == secondPoint.Y && firstPoint.X > secondPoint.X)
        {
            directionsForFirstPoint = new Direction[3]
                {
                    Direction.Right,
                    Direction.UpRight,
                    Direction.DownRight
                };
            directionsForSecondPoint = new Direction[3]
            {
                    Direction.Left,
                    Direction.DownLeft,
                    Direction.UpLeft
            };
            return;
        }

        else if (firstPoint.Y < secondPoint.Y && firstPoint.X > secondPoint.X)
        {
            if (firstPoint.X - secondPoint.X > secondPoint.Y - firstPoint.Y)
            {
                directionsForFirstPoint = new Direction[2]
                {
                    Direction.Right,
                    Direction.DownRight
                };
                directionsForSecondPoint = new Direction[2]
                {
                    Direction.Left,
                    Direction.UpLeft
                };
                return;
            }

            if (firstPoint.X - secondPoint.X < secondPoint.Y - firstPoint.Y)
            {
                directionsForFirstPoint = new Direction[2]
                {
                    Direction.Down,
                    Direction.DownRight
                };
                directionsForSecondPoint = new Direction[2]
                {
                    Direction.Up,
                    Direction.UpLeft
                };
                return;
            }

            if (firstPoint.X - secondPoint.X == secondPoint.Y - firstPoint.Y)
            {
                directionsForFirstPoint = new Direction[3]
                {
                    Direction.Right,
                    Direction.DownRight,
                    Direction.Down
                };
                directionsForSecondPoint = new Direction[3]
                {
                    Direction.Left,
                    Direction.UpLeft,
                    Direction.Up
                };
                return;
            }
        }

        else if (firstPoint.Y < secondPoint.Y && firstPoint.X == secondPoint.X)
        {
            directionsForFirstPoint = new Direction[3]
                {
                    Direction.DownLeft,
                    Direction.DownRight,
                    Direction.Down
                };
            directionsForSecondPoint = new Direction[3]
            {
                    Direction.UpRight,
                    Direction.UpLeft,
                    Direction.Up
            };
            return;
        }

        else if (firstPoint.Y < secondPoint.Y && firstPoint.X < secondPoint.X)
        {
            if (secondPoint.Y - firstPoint.Y > secondPoint.X - firstPoint.X)
            {
                directionsForFirstPoint = new Direction[2]
                {
                    Direction.Down,
                    Direction.DownLeft
                };
                directionsForSecondPoint = new Direction[2]
                {
                    Direction.Up,
                    Direction.UpRight
                };
                return;
            }

            if (secondPoint.Y - firstPoint.Y < secondPoint.X - firstPoint.X)
            {
                directionsForFirstPoint = new Direction[2]
                {
                    Direction.Left,
                    Direction.DownLeft
                };
                directionsForSecondPoint = new Direction[2]
                {
                    Direction.Right,
                    Direction.UpRight
                };
                return;
            }

            if (secondPoint.Y - firstPoint.Y == secondPoint.X - firstPoint.X)
            {
                directionsForFirstPoint = new Direction[3]
                {
                    Direction.Left,
                    Direction.DownLeft,
                    Direction.Down
                };
                directionsForSecondPoint = new Direction[3]
                {
                    Direction.Right,
                    Direction.UpRight,
                    Direction.Up
                };
                return;
            }
        }

        else if (firstPoint.Y == secondPoint.Y && firstPoint.X < secondPoint.X)
        {
            directionsForFirstPoint = new Direction[3]
            {
                    Direction.Left,
                    Direction.DownLeft,
                    Direction.UpLeft
            };
            directionsForSecondPoint = new Direction[3]
            {
                    Direction.Right,
                    Direction.UpRight,
                    Direction.DownRight
            };
            return;
        }

        else if (firstPoint.Y > secondPoint.Y && firstPoint.X < secondPoint.X)
        {
            if (firstPoint.Y - secondPoint.Y < secondPoint.X - firstPoint.X)
            {
                directionsForFirstPoint = new Direction[2]
                {
                    Direction.Left,
                    Direction.UpLeft
                };
                directionsForSecondPoint = new Direction[2]
                {
                    Direction.Right,
                    Direction.DownRight
                };
                return;
            }

            if (firstPoint.Y - secondPoint.Y > secondPoint.X - firstPoint.X)
            {
                directionsForFirstPoint = new Direction[2]
                {
                    Direction.Up,
                    Direction.UpLeft
                };
                directionsForSecondPoint = new Direction[2]
                {
                    Direction.Down,
                    Direction.DownRight
                };
                return;
            }

            if (firstPoint.Y - secondPoint.Y == secondPoint.X - firstPoint.X)
            {
                directionsForFirstPoint = new Direction[3]
                {
                    Direction.Left,
                    Direction.UpLeft,
                    Direction.Up
                };
                directionsForSecondPoint = new Direction[3]
                {
                    Direction.Right,
                    Direction.DownRight,
                    Direction.Down
                };
                return;
            }
        }

        else if (firstPoint.Y > secondPoint.Y && firstPoint.X == secondPoint.X)
        {
            directionsForFirstPoint = new Direction[3]
                {
                    Direction.UpRight,
                    Direction.UpLeft,
                    Direction.Up
                };
            directionsForSecondPoint = new Direction[3]
            {
                    Direction.DownLeft,
                    Direction.DownRight,
                    Direction.Down
            };
            return;
        }

        Debug.Log($"GetDisjointDirections: firstPoint - {firstPoint}, secondPoint - {secondPoint}");
        throw new System.Exception("Неучтенное расположение объектов. Ошибка в методе.");
    }



    public static Dictionary<(int X, int Y), (BeamSearchData beamFromFirstPoint, BeamSearchData beamFromSecondPoint)> GetIntersectionsCoordinates(Matrix matrix, (int X, int Y) firstPoint, Direction directionFromFirstPoint, (int X, int Y) secondPoint)
    {
        //var res = new Dictionary<(int X, int Y), (List<BlackHoleInfluenceData>, List<BlackHoleInfluenceData>)>();
        //var res = new Dictionary<(int X, int Y), (BeamSearchData beamFromFirstPoint, BeamSearchData beamFromSecondPoint)>();
        var res = new ConcurrentDictionary<(int X, int Y), (BeamSearchData beamFromFirstPoint, BeamSearchData beamFromSecondPoint)>();

        //Если точки одинаковые, то возвращаем пустой список. НО! Нужно разобраться, почему иногда проверяет одинаковые точки.
        //Это происходит в случаях, если луч выходит из телепорта и после этого нужно создать сферу, луч из которой опять идет в телепорт. Проверяются ВСЕ телепорты на пересечения, и этот телепорт тоже. Уберу эту возможность и больше не должно срабатывать.
        if (firstPoint == secondPoint)
        {
            Debug.LogWarning($"GetIntersectionsCoordinates: firstPoint == secondPoint == {firstPoint}!!! directionFromFirstPoint: {directionFromFirstPoint}");
            return res.ToDictionary(a=>a.Key, a=>a.Value);
        }

        AllDirectionsArray.AsParallel().ForAll((direction) =>
        {
            if (matrix.GetIntersectionOfDirections(firstPoint, directionFromFirstPoint, secondPoint, direction, out var beamFromFirstPoint, out var beamFromSecondPoint))
            {
                if (!res.ContainsKey(beamFromFirstPoint.endPoint))
                    res.TryAdd(beamFromFirstPoint.endPoint, (beamFromFirstPoint, beamFromSecondPoint));
                //else
                //{
                //    Debug.Log($"This intersection point can reach more than one directions \n Direction ONE:  firstPoint: {firstPoint}, directionFromFirstPoint: {directionFromFirstPoint}, secondPoint: {secondPoint}, dirFromSecondPoint: {direction}, intersection point: {beamFromFirstPoint.endPoint}," +
                //    $" \n  Direction TWO: firstPoint: {res[beamFromFirstPoint.endPoint].beamFromFirstPoint.originPoint}, directionFromFirstPoint: {res[beamFromFirstPoint.endPoint].beamFromFirstPoint.originDirection}, secondPoint: {res[beamFromFirstPoint.endPoint].beamFromSecondPoint.originPoint}, dirFromSecondPoint: {res[beamFromFirstPoint.endPoint].beamFromSecondPoint.originDirection}, intersection point: {beamFromFirstPoint.endPoint},");
                //}
                //Debug.Log($"Coordinates added: {coordinates}, firstPoint: {firstPoint}, secondPoint: {secondPoint}, directionFromFirstPoint: {directionFromFirstPoint}, направление от третьей точки ко второй: {directionsFromSecondPoint[i]} ");
            }
        });

        return res.ToDictionary(a => a.Key, a => a.Value);
    }


    public static Dictionary<(int X, int Y), (BeamSearchData beamFromFirstPoint, BeamSearchData beamFromSecondPoint)> GetIntersectionsCoordinates(List<BeamSearchData> firstBeams, List<BeamSearchData> secondBeams)
    {
        var res = new ConcurrentDictionary<(int X, int Y), (BeamSearchData beamFromFirstPoint, BeamSearchData beamFromSecondPoint)>();


        firstBeams.AsParallel().ForAll((beam) =>
        {
            for (int i = 0; i < secondBeams.Count; i++)
            {
                if (beam.IntersectsWith(secondBeams[i], out var points))
                {
                    var point = points.GetRandomNext();
                    beam.endPoint = point;
                    res.TryAdd(point, (beam, secondBeams[i]));
                }
            }

        });

        return res.ToDictionary(a => a.Key, a => a.Value);
    }









    /// <summary>
    /// Метод нахождения "выходящей" точки и "выходящего" направления после влияния черной дыры.
    /// </summary>
    /// <param name="inputPoint">Точка, при прохождении которой на луч будет действовать ЧД. Это всегда одна из 4 ближайших к ЧД точек (2 по горизонтали, 2 по вертикали).</param>
    /// <param name="directionBefore">Направление луча до начала влияния ЧД.</param>
    /// <param name="blackHole">Информация о ЧД.</param>
    /// <param name="directionAfter">Направление луча после влияния ЧД.</param>
    /// <param name="goingIntoBlackHole">Уходит ли луч прямиком в черную дыру.</param>
    /// <returns></returns>
    public static (int X, int Y) GetOutputPointAndDirectionAfterBlackHoleInfluence_Old((int X, int Y) inputPoint, Direction directionBefore, BlackHoleInfo blackHole, out Direction directionAfter, out bool goingIntoBlackHole)
    {
        //проверка, не упирается ли луч в черную дыру.
        if (NextCellOnDirection(inputPoint, directionBefore) == (blackHole.X, blackHole.Y))
        {
            directionAfter = directionBefore;
            goingIntoBlackHole = true;
            return (blackHole.X, blackHole.Y);
        }

        goingIntoBlackHole = false;
        var multiplierX = inputPoint.X - blackHole.X;
        var multiplierY = inputPoint.Y - blackHole.Y;

        if (Math.Abs(multiplierX) > 1 || Math.Abs(multiplierY) > 1)
        {
            throw new Exception($"Ошибка в методе определения точки и направления после влияния черной дыры. Множитель больше 1. multiplierX: {multiplierX}, multiplierY: {multiplierY}");
        }

        (int X, int Y) step;
        step = NextCellOnDirection((0, 0), directionBefore);
        (int X, int Y) cornerPoint = (0,0);

        if (!directionBefore.IsDiagonal())
        {
            //предыдущая (угловая) точка при "прямых" направлениях.
            cornerPoint = (inputPoint.X - step.X, inputPoint.Y - step.Y);
            //шаг, равный направлению "наискосок". Нужно это направление, чтоб X и Y не были равны 0. ТОгда проще рассчитать "выходящую" точку.
            step = (blackHole.X - cornerPoint.X, blackHole.Y - cornerPoint.Y);
        }


        int sign;
        (int X, int Y) outputPoint;

        if (multiplierX == 0) //две точки по вертикали: выше и ниже ЧД.
        {
            if ((multiplierY < 0 && directionBefore == Direction.Down) || (multiplierY > 0 && directionBefore == Direction.Up))
                throw new Exception($"Ошибка в методе определения точки и направления после влияния черной дыры. Невозможное направление. multiplierX:{multiplierX}, multiplierY: {multiplierY}, directionBefore: {directionBefore}");

            sign = step.X;
            outputPoint = blackHole.size switch
            {
                BlackHoleInfo.BlackHoleSize.Small => (inputPoint.X + step.X, inputPoint.Y + 2 * step.Y),
                BlackHoleInfo.BlackHoleSize.Medium => (inputPoint.X, inputPoint.Y + 2 * step.Y),
                BlackHoleInfo.BlackHoleSize.Large => throw new NotImplementedException(),
                BlackHoleInfo.BlackHoleSize.Supermassive => throw new NotImplementedException(),
                _ => throw new Exception()
            };
        }
        else if (multiplierY == 0) //две точки по горизонтали: левее и правее ЧД.
        {
            if ((multiplierX < 0 && directionBefore == Direction.Left) || (multiplierX > 0 && directionBefore == Direction.Right))
            {
                throw new Exception($"Ошибка в методе определения точки и направления после влияния черной дыры. Невозможное направление. multiplierX:{multiplierX}, multiplierY: {multiplierY}, directionBefore: {directionBefore}");
            }

            sign = step.Y;
            outputPoint = blackHole.size switch
            {
                BlackHoleInfo.BlackHoleSize.Small => (inputPoint.X + 2 * step.X, inputPoint.Y + step.Y),
                BlackHoleInfo.BlackHoleSize.Medium => (inputPoint.X + 2 * step.X, inputPoint.Y),
                BlackHoleInfo.BlackHoleSize.Large => throw new NotImplementedException(),
                BlackHoleInfo.BlackHoleSize.Supermassive => throw new NotImplementedException(),
                _ => throw new Exception()
            };
        }
        else
        {
            throw new NotImplementedException("Situation that shouldn't have happened in method GetOutputPointAfterBlackHoleInfluence");
        }

        directionAfter = DirectionMinus(directionBefore, sign * (multiplierY * (int)blackHole.size - multiplierX * (int)blackHole.size));

        switch (blackHole.size)
        {
            case BlackHoleInfo.BlackHoleSize.Small:
                if (directionBefore.IsDiagonal())
                {
                    //Debug.Log($"directionBefore.isdiagonal: {directionBefore.IsDiagonal()}, inputPoint: {inputPoint}, steps.X: {steps.X}, steps.Y: {steps.Y}, directionAfter: {directionAfter}");
                    return outputPoint;
                }
                else
                {
                    //steps = NextCellOnDirection((0, 0), directionAfter);
                    //Debug.Log($"directionBefore.isdiagonal: {directionBefore.IsDiagonal()}, inputPoint: {inputPoint}, steps.X: {steps.X}, steps.Y: {steps.Y}, directionAfter: {directionAfter}");
                    return (inputPoint.X + step.X, inputPoint.Y + step.Y);
                }

            case BlackHoleInfo.BlackHoleSize.Medium:
                if (directionBefore.IsDiagonal())
                {
                    //Debug.Log($"directionBefore.isdiagonal: {directionBefore.IsDiagonal()}, inputPoint: {inputPoint}, steps.X: {steps.X}, steps.Y: {steps.Y}, directionAfter: {directionAfter}");
                    return outputPoint;
                }
                else
                {
                    outputPoint = (cornerPoint.X + 2 * step.X, cornerPoint.Y + 2 * step.Y);
                    //Debug.Log($"directionBefore.isdiagonal: {directionBefore.IsDiagonal()}, inputPoint: {inputPoint}, outputPoint:{outputPoint}, steps.X: {steps.X}, steps.Y: {steps.Y}, directionAfter: {directionAfter}");
                    return outputPoint;
                }

            default: throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Метод нахождения "выходящей" точки и "выходящего" направления после влияния черной дыры.
    /// </summary>
    /// <param name="inputPoint">Точка, при прохождении которой на луч будет действовать ЧД. Это всегда одна из 4 ближайших к ЧД точек (2 по горизонтали, 2 по вертикали).</param>
    /// <param name="directionBefore">Направление луча до начала влияния ЧД.</param>
    /// <param name="blackHole">Информация о ЧД.</param>
    /// <param name="directionAfter">Направление луча после влияния ЧД.</param>
    /// <param name="goingIntoBlackHole">Уходит ли луч прямиком в черную дыру.</param>
    /// <returns></returns>
    public static (int X, int Y) GetOutputPointAndDirectionAfterBlackHoleInfluence((int X, int Y) inputPoint, Direction directionBefore, BlackHoleInfo blackHole, out Direction directionAfter, out bool goingIntoBlackHole)
    {
        //проверка, не упирается ли луч в черную дыру.
        if (NextCellOnDirection(inputPoint, directionBefore) == (blackHole.X, blackHole.Y))
        {
            directionAfter = directionBefore;
            goingIntoBlackHole = true;
            return (blackHole.X, blackHole.Y);
        }

        goingIntoBlackHole = false;
        var multiplierX = inputPoint.X - blackHole.X;
        var multiplierY = inputPoint.Y - blackHole.Y;

        if (Math.Abs(multiplierX) > 1 || Math.Abs(multiplierY) > 1)
        {
            throw new Exception($"Ошибка в методе определения точки и направления после влияния черной дыры. Множитель больше 1. multiplierX: {multiplierX}, multiplierY: {multiplierY}");
        }

        (int X, int Y) stepDirectionBefore;
        stepDirectionBefore = NextCellOnDirection((0, 0), directionBefore);

        int sign;

        if (multiplierX == 0) //две точки по вертикали: выше и ниже ЧД.
        {
            if ((multiplierY < 0 && directionBefore == Direction.Down) || (multiplierY > 0 && directionBefore == Direction.Up))
                throw new Exception($"Ошибка в методе определения точки и направления после влияния черной дыры. Невозможное направление. multiplierX:{multiplierX}, multiplierY: {multiplierY}, directionBefore: {directionBefore}");

            sign = stepDirectionBefore.X;
        }
        else if (multiplierY == 0) //две точки по горизонтали: левее и правее ЧД.
        {
            if ((multiplierX < 0 && directionBefore == Direction.Left) || (multiplierX > 0 && directionBefore == Direction.Right))
            {
                throw new Exception($"Ошибка в методе определения точки и направления после влияния черной дыры. Невозможное направление. multiplierX:{multiplierX}, multiplierY: {multiplierY}, directionBefore: {directionBefore}");
            }

            sign = stepDirectionBefore.Y;
        }
        else
        {
            throw new NotImplementedException("Situation that shouldn't have happened in method GetOutputPointAfterBlackHoleInfluence");
        }

        directionAfter = DirectionMinus(directionBefore, sign * (multiplierY * (int)blackHole.size - multiplierX * (int)blackHole.size));
        (int X, int Y) stepDirectionAfter;
        stepDirectionAfter = NextCellOnDirection((0, 0), directionAfter);

        switch (blackHole.size)
        {
            case BlackHoleInfo.BlackHoleSize.Small:
                if (directionBefore.IsDiagonal())
                {
                    return (inputPoint.X + stepDirectionBefore.X + stepDirectionAfter.X, inputPoint.Y + stepDirectionBefore.Y + stepDirectionAfter.Y);
                }
                else
                {
                    return (inputPoint.X + stepDirectionAfter.X, inputPoint.Y + stepDirectionAfter.Y);
                }

            case BlackHoleInfo.BlackHoleSize.Medium:
                if (directionBefore.IsDiagonal())
                {
                    return (inputPoint.X + stepDirectionBefore.X + stepDirectionAfter.X, inputPoint.Y + stepDirectionBefore.Y + stepDirectionAfter.Y);
                }
                else
                {
                    return (inputPoint.X + stepDirectionBefore.X + 2 * stepDirectionAfter.X, inputPoint.Y + stepDirectionBefore.Y + 2 * stepDirectionAfter.Y);
                }

            case BlackHoleInfo.BlackHoleSize.Large:
                if (directionBefore.IsDiagonal())
                {
                    return (inputPoint.X + 2 * stepDirectionBefore.X + 3 * stepDirectionAfter.X, inputPoint.Y + 2 * stepDirectionBefore.Y + 3 * stepDirectionAfter.Y);
                }
                else
                {
                    return (inputPoint.X + 2 * stepDirectionBefore.X + 2 * stepDirectionAfter.X, inputPoint.Y + 2* stepDirectionBefore.Y + 2 * stepDirectionAfter.Y);
                }

            case BlackHoleInfo.BlackHoleSize.Supermassive:

                var directionPerpendicularToBHSide = DirectionMinus(directionBefore, sign * (multiplierY * 90 - multiplierX * 90));
                var stepDirectionPerpendicularToBHSide = NextCellOnDirection((0, 0), directionPerpendicularToBHSide);

                if (directionBefore.IsDiagonal())
                {
                    return (inputPoint.X + stepDirectionPerpendicularToBHSide.X, inputPoint.Y + stepDirectionPerpendicularToBHSide.Y);
                }
                else
                {
                    return (inputPoint.X + 2 * stepDirectionPerpendicularToBHSide.X + stepDirectionAfter.X, inputPoint.Y + 2 * stepDirectionPerpendicularToBHSide.Y + stepDirectionAfter.Y);
                }


            default: throw new NotImplementedException();
        }
    }



    


    /// <summary>
    /// Рассчитывает, лежит ли вторая точка на прямой линии с первой точкой по какому либо из 8 направлений.
    /// </summary>
    /// <param name="firstPoint">Координаты первой точки.</param>
    /// <param name="secondPoint">Координаты второй точки.</param>
    /// <param name="originDirection">Направление в начале луча ОТ ПЕРВОЙ точки ко второй, в случае положительного результата.</param>
    /// <param name="endDirection">Направление в конце луча ОТ ПЕРВОЙ точки ко второй, в случае положительного результата.
    /// Совпадает с направлением начала, т.к. этот метод используется при отсутствии в матрице черных дыр и лучи всегда идут по прямой.</param>
    /// <returns>true, если лежит, false - если нет</returns>
    public static bool DoesPointLieInDirectionWithoutBH(Matrix matrix, (int X, int Y) firstPoint, (int X, int Y) secondPoint, out Direction originDirection, out Direction endDirection)
    {
        //if same coordinates
        if (firstPoint == secondPoint)
        {
            originDirection = endDirection = Direction.Up;
            return false;
        }


        if (firstPoint.X == secondPoint.X)
        {
            if (firstPoint.Y < secondPoint.Y)
            {
                originDirection = endDirection = Direction.Up;
                return true;
            }
            else
            {
                originDirection = endDirection = Direction.Down;
                return true;
            }
        }

        if (firstPoint.Y == secondPoint.Y)
        {
            if (firstPoint.X > secondPoint.X)
            {
                originDirection = endDirection = Direction.Left;
                return true;
            }
            else
            {
                originDirection = endDirection = Direction.Right;
                return true;
            }
        }

        if (firstPoint.X > secondPoint.X)
        {
            if (firstPoint.X - secondPoint.X == secondPoint.Y - firstPoint.Y)
            {
                originDirection = endDirection = Direction.UpLeft;
                return true;
            }
            if (firstPoint.X - secondPoint.X == firstPoint.Y - secondPoint.Y)
            {
                originDirection = endDirection = Direction.DownLeft;
                return true;
            }
        }

        if (secondPoint.X - firstPoint.X == firstPoint.Y - secondPoint.Y)
        {

            originDirection = endDirection = Direction.DownRight;
            return true;
        }

        if (secondPoint.X - firstPoint.X == secondPoint.Y - firstPoint.Y)
        {
            originDirection = endDirection = Direction.UpRight;
            return true;
        }

        originDirection = endDirection = Direction.Up;
        return false;
    }

    /// <summary>
    /// Рассчитывает, лежит ли объект по какому-либо из 8 направлений с первой точкой с учетом влияний черных дыр.
    /// </summary>
    /// <param name="point">Координаты точки.</param>
    /// <param name="unitCoordinates">Координаты объекта.</param>
    /// <param name="originDirection">Направление в начале луча ОТ точки к объекту, в случае положительного результата.</param>
    /// <param name="endDirection">Направление в конце луча ОТ точки к объекту, в случае положительного результата.
    /// <returns>true, если лежит, false - если нет</returns>
    public static bool DoesUnitLieInDirectionWithBH(Matrix matrix, (int X, int Y) point, (int X, int Y) unitCoordinates, out Direction originDirection, out Direction endDirection)
    {
        bool res = false;
        Direction _originDirection = Direction.Up;
        Direction _endDirection = Direction.Up;

        AllDirectionsArray.AsParallel().ForAll(dir =>
        {
            if (res == false)
            {
                //var unit = matrix.CheckDirectionFrom(point.X, point.Y, dir, out var cells, out var influenceDatas);
                var unit = matrix.BeamCast(point.X, point.Y, dir, out var beam);
                if (unit != null && (unit.X, unit.Y) == unitCoordinates)
                {
                    _originDirection = dir;
                    _endDirection = beam.endDirection;
                    //if (beam.influencesBH.Count > 0)
                    //{
                    //    _endDirection = influenceDatas.LastOrDefault().directionAfter;
                    //}
                    //else
                    //{
                    //    _endDirection = dir;
                    //}

                    res = true;
                }
                //Debug.Log($"in ForAll: firstPoint: {firstPoint}, secondPoint: {secondPoint}, dir: {dir}, cells: {cells.ToStringEverythingInARow()}, influenceDatas dir after: {influenceDatas.LastOrDefault().directionAfter}, res: {res}");
            }
        });

        originDirection = _originDirection;
        endDirection = _endDirection;
        //Debug.Log($"DoesPointLieInDirectionWithBH: res: {res}, originDirection: {originDirection}, endDirection: {endDirection}");
        return res;
    }



    private static Direction DirectionMinus(Direction firstDirection, Direction secondDirection)
    {
        var dir = firstDirection - secondDirection;
        return OverDegreesToDirection((Direction)dir);
    }

    private static Direction DirectionMinus(Direction direction, int degrees)
    {
        if (degrees % 45 != 0)
            throw new Exception("Попытка отнять градусы, не соответствующие направлению.");

        return DirectionMinus(direction, (Direction)degrees);
    }

    private static Direction DirectionPlus(Direction firstDirection, Direction secondDirection)
    {
        var dir = (int)firstDirection + secondDirection;
        return OverDegreesToDirection(dir);
    }

    private static Direction DirectionPlus(Direction direction, int degrees)
    {
        if (degrees % 45 != 0)
            throw new Exception("Попытка прибавить градусы, не соответствующие направлению.");

        return DirectionPlus(direction, (Direction)degrees);
    }

    /// <summary>
    /// Приводит к нормальному направлению в случае, если по градусам направление ушло в минус или более 315.
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private static Direction OverDegreesToDirection(Direction direction)
    {
        var dir = (int)direction % 360;
        if (dir < 0) return (Direction)dir + 360;
        return (Direction)dir;
    }





    /// <summary>
    /// Возвращает, сработало ли событие с заданным процентом.
    /// </summary>
    /// <param name="percent"></param>
    /// <returns></returns>
    public static bool IsChanceOn(int percent) => percent > new System.Random(Environment.TickCount).Next(0, 100);



    public static Vector2 NearestWholeCoordinate(Vector2 clickPoint)
    {
        var worldPointPosition = (Vector2)Camera.main.ScreenToWorldPoint(clickPoint);

        int Xbottom = (int)worldPointPosition.x;
        int Xtop = Mathf.CeilToInt(worldPointPosition.x);
        if (Xbottom == Xtop) Xtop += 1;
        int Ybottom = (int)worldPointPosition.y;
        int Ytop = Mathf.CeilToInt(worldPointPosition.y);
        if (Ybottom == Ytop) Ytop += 1;

        Vector2[] points = new Vector2[4]
        {
                new Vector2(Xbottom, Ybottom),
                new Vector2(Xbottom, Ytop),
                new Vector2(Xtop, Ybottom),
                new Vector2(Xtop, Ytop),
        };

        Dictionary<Vector2, float> pointsAndDistances = new Dictionary<Vector2, float>(4)
            {
                {points[0], (points[0] - worldPointPosition).sqrMagnitude},
                {points[1], (points[1] - worldPointPosition).sqrMagnitude},
                {points[2], (points[2] - worldPointPosition).sqrMagnitude},
                {points[3], (points[3] - worldPointPosition).sqrMagnitude}
            };

        var minDistance = pointsAndDistances.Values.Min();
        return pointsAndDistances.First(p => p.Value == minDistance).Key;
    }

    /// <summary>
    /// Проверяет, содержится ли в коллекции юнитов матрицы юнит с такими же координатами.
    /// </summary>
    /// <param name="matrixUnits">Проверяемая коллекция.</param>
    /// <param name="matrixUnit">Проверяемый юнит.</param>
    /// <returns></returns>
    public static bool ContainsSameCoordinates(IEnumerable<MatrixUnit> matrixUnits, MatrixUnit matrixUnit)
    {
        return matrixUnits.AsParallel().Any(mu => mu.X == matrixUnit.X && mu.Y == matrixUnit.Y);
    }



    private static void EllipceProbe()
    {
        var beam = UnityEngine.Object.Instantiate(Prefabs.Instance.beamPrefab);
        int pointsCount = 180;
        beam.lightBeamRenderer.positionCount = pointsCount;
        int XpositionsCount = pointsCount / 2;
        float step = (float)2 / XpositionsCount;
        float X = 2, Y;


        for (int i = 0; i < XpositionsCount; i++)
        {
            Y = (float)Math.Sqrt((1 - X * X / 4) / 2);
            beam.lightBeamRenderer.SetPosition(i * 2, new Vector3(X, Y, -1));
            beam.lightBeamRenderer.SetPosition(i * 2 + 1, new Vector3(X, -Y, -1));
            X -= step;
        }
    }

    private static void CheckMethod_GetOutputPointAfterBlackHoleInfluence()
    {
        var blackhole = new BlackHoleInfo(5, 5, BlackHoleInfo.BlackHoleSize.Small);
        //{
        //    X = 5,
        //    Y = 5,
        //    size = BlackHoleInfo.BlackHoleSize.Small
        //};

        Direction directionAfter;
        Direction directionBefore;

        var directions = new List<Direction>(AllDirectionsArray);
        directions.Remove(Direction.Down);
        for (int i = 0; i < directions.Count; i++)
        {
            directionBefore = directions[i];
            var point = GetOutputPointAndDirectionAfterBlackHoleInfluence((5, 4), directionBefore, blackhole, out directionAfter, out var intoBlackHole);
            Debug.Log($"Check method. Before dir: {directionBefore}, after: {directionAfter}, point: {point}, intoBlackHole: {intoBlackHole}");
        }


        //directionBefore = Direction.Up;
        //GetOutputPointAfterBlackHoleInfluence((5, 6), directionBefore, blackhole, out directionAfter);
        //Debug.Log($"Check method. Before dir: {directionBefore}, after: {directionAfter}");

        //directionBefore = Direction.Right;
        //GetOutputPointAfterBlackHoleInfluence((5, 6), directionBefore, blackhole, out directionAfter);
        //Debug.Log($"Check method. Input dir: {directionBefore}, output: {directionAfter}");
    }







    public static class HyperbolaProbe
    {
        //public static void RightDirection()
        //{
        //    var beam = UnityEngine.Object.Instantiate(Prefabs.Instance.beamPrefab);
        //    var blackHole = UnityEngine.Object.Instantiate(Prefabs.Instance.blackHoleSmallPrefab);
        //    var blackHoleInfo = new BlackHoleInfo() { size = BlackHoleInfo.BlackHoleSize.Small, X = 0, Y = 0 };
        //    blackHole.size = BlackHoleInfo.BlackHoleSize.Small;
        //    blackHole.transform.position = new Vector3(0, 0, -1);
        //    (int X, int Y) inputPoint = (-2, 1);
        //    (int X, int Y) outputPoint = (2, -1);

        //    var points = BHPoints.GetPoints(blackHoleInfo, Direction.Right, inputPoint, -1);
        //    beam.lightBeamRenderer.positionCount = points.Length + 4;

        //    beam.lightBeamRenderer.SetPosition(0, new Vector3(-5, 1, -1));
        //    beam.lightBeamRenderer.SetPosition(1, new Vector3(inputPoint.X, inputPoint.Y, -1));
        //    beam.lightBeamRenderer.SetPosition(beam.lightBeamRenderer.positionCount - 2, new Vector3(outputPoint.X, outputPoint.Y, -1));
        //    beam.lightBeamRenderer.SetPosition(beam.lightBeamRenderer.positionCount - 1, new Vector3(4, -3, -1));

        //    for (int i = 2; i < points.Length + 2; i++)
        //    {
        //        //Debug.Log($"i: {i} => points[i - 2]: {points[i - 2]}");
        //        beam.lightBeamRenderer.SetPosition(i, points[i - 2]);
        //    }



        //    var beam2 = UnityEngine.Object.Instantiate(Prefabs.Instance.beamPrefab);
        //    inputPoint = (-2, -1);
        //    outputPoint = (2, 1);

        //    points = BHPoints.GetPoints(blackHoleInfo, Direction.Right, inputPoint, -1);
        //    beam2.lightBeamRenderer.positionCount = points.Length + 4;

        //    beam2.lightBeamRenderer.SetPosition(0, new Vector3(-5, -1, -1));
        //    beam2.lightBeamRenderer.SetPosition(1, new Vector3(inputPoint.X, inputPoint.Y, -1));
        //    beam2.lightBeamRenderer.SetPosition(beam2.lightBeamRenderer.positionCount - 2, new Vector3(outputPoint.X, outputPoint.Y, -1));
        //    beam2.lightBeamRenderer.SetPosition(beam2.lightBeamRenderer.positionCount - 1, new Vector3(4, 3, -1));

        //    for (int i = 2; i < points.Length + 2; i++)
        //    {
        //        //Debug.Log($"i: {i} => points[i - 2]: {points[i - 2]}");
        //        beam2.lightBeamRenderer.SetPosition(i, points[i - 2]);
        //    }
        //}
        //public static void LeftDirection()
        //{
        //    var beam = UnityEngine.Object.Instantiate(Prefabs.Instance.beamPrefab);
        //    var blackHole = UnityEngine.Object.Instantiate(Prefabs.Instance.blackHoleSmallPrefab);
        //    blackHole.size = BlackHoleInfo.BlackHoleSize.Small;
        //    blackHole.transform.position = new Vector3(0, 0, -1);
        //    var blackHoleInfo = new BlackHoleInfo() { size = BlackHoleInfo.BlackHoleSize.Small, X = 0, Y = 0 };
        //    (int X, int Y) inputPoint = (2, 1);
        //    (int X, int Y) outputPoint = (-2, -1);

        //    var points = BHPoints.GetPoints(blackHoleInfo, Direction.Left, inputPoint, -1);
        //    beam.lightBeamRenderer.positionCount = points.Length + 4;

        //    beam.lightBeamRenderer.SetPosition(0, new Vector3(5, 1, -1));
        //    beam.lightBeamRenderer.SetPosition(1, new Vector3(inputPoint.X, inputPoint.Y, -1));
        //    beam.lightBeamRenderer.SetPosition(beam.lightBeamRenderer.positionCount - 2, new Vector3(outputPoint.X, outputPoint.Y, -1));
        //    beam.lightBeamRenderer.SetPosition(beam.lightBeamRenderer.positionCount - 1, new Vector3(-4, -3, -1));

        //    for (int i = 2; i < points.Length + 2; i++)
        //    {
        //        //Debug.Log($"i: {i} => points[i - 2]: {points[i - 2]}");
        //        beam.lightBeamRenderer.SetPosition(i, points[i - 2]);
        //    }



        //    var beam2 = UnityEngine.Object.Instantiate(Prefabs.Instance.beamPrefab);
        //    inputPoint = (2, -1);
        //    outputPoint = (-2, 1);

        //    points = BHPoints.GetPoints(blackHoleInfo, Direction.Left, inputPoint, -1);
        //    beam2.lightBeamRenderer.positionCount = points.Length + 4;

        //    beam2.lightBeamRenderer.SetPosition(0, new Vector3(5, -1, -1));
        //    beam2.lightBeamRenderer.SetPosition(1, new Vector3(inputPoint.X, inputPoint.Y, -1));
        //    beam2.lightBeamRenderer.SetPosition(beam2.lightBeamRenderer.positionCount - 2, new Vector3(outputPoint.X, outputPoint.Y, -1));
        //    beam2.lightBeamRenderer.SetPosition(beam2.lightBeamRenderer.positionCount - 1, new Vector3(-4, 3, -1));

        //    for (int i = 2; i < points.Length + 2; i++)
        //    {
        //        //Debug.Log($"i: {i} => points[i - 2]: {points[i - 2]}");
        //        beam2.lightBeamRenderer.SetPosition(i, points[i - 2]);
        //    }
        //}

        //public static void UpRightDirection()
        //{
        //    var beam = UnityEngine.Object.Instantiate(Prefabs.Instance.beamPrefab);
        //    var blackHole = UnityEngine.Object.Instantiate(Prefabs.Instance.blackHoleSmallPrefab);
        //    blackHole.size = BlackHoleInfo.BlackHoleSize.Small;
        //    blackHole.transform.position = new Vector3(0, 0, -1);
        //    var blackHoleInfo = new BlackHoleInfo() { size = BlackHoleInfo.BlackHoleSize.Small, X = 0, Y = 0 };
        //    (int X, int Y) inputPoint = (-2, -1);
        //    (int X, int Y) outputPoint = (2, 1);

        //    var points = BHPoints.GetPoints(blackHoleInfo, Direction.UpRight, inputPoint, -1);
        //    beam.lightBeamRenderer.positionCount = points.Length + 4;

        //    beam.lightBeamRenderer.SetPosition(0, new Vector3(-4, -3, -1));
        //    beam.lightBeamRenderer.SetPosition(1, new Vector3(inputPoint.X, inputPoint.Y, -1));
        //    beam.lightBeamRenderer.SetPosition(beam.lightBeamRenderer.positionCount - 2, new Vector3(outputPoint.X, outputPoint.Y, -1));
        //    beam.lightBeamRenderer.SetPosition(beam.lightBeamRenderer.positionCount - 1, new Vector3(5, 1, -1));

        //    for (int i = 2; i < points.Length + 2; i++)
        //    {
        //        //Debug.Log($"i: {i} => points[i - 2]: {points[i - 2]}");
        //        beam.lightBeamRenderer.SetPosition(i, points[i - 2]);
        //    }



        //    var beam2 = UnityEngine.Object.Instantiate(Prefabs.Instance.beamPrefab);
        //    inputPoint = (-1, -2);
        //    outputPoint = (1, 2);

        //    points = BHPoints.GetPoints(blackHoleInfo, Direction.UpRight, inputPoint, -1);
        //    beam2.lightBeamRenderer.positionCount = points.Length + 4;

        //    beam2.lightBeamRenderer.SetPosition(0, new Vector3(-3, -4, -1));
        //    beam2.lightBeamRenderer.SetPosition(1, new Vector3(inputPoint.X, inputPoint.Y, -1));
        //    beam2.lightBeamRenderer.SetPosition(beam2.lightBeamRenderer.positionCount - 2, new Vector3(outputPoint.X, outputPoint.Y, -1));
        //    beam2.lightBeamRenderer.SetPosition(beam2.lightBeamRenderer.positionCount - 1, new Vector3(1, 5, -1));

        //    for (int i = 2; i < points.Length + 2; i++)
        //    {
        //        //Debug.Log($"i: {i} => points[i - 2]: {points[i - 2]}");
        //        beam2.lightBeamRenderer.SetPosition(i, points[i - 2]);
        //    }
        //}
        //public static void UpLeftDirection()
        //{
        //    var beam = UnityEngine.Object.Instantiate(Prefabs.Instance.beamPrefab);
        //    var blackHole = UnityEngine.Object.Instantiate(Prefabs.Instance.blackHoleSmallPrefab);
        //    blackHole.size = BlackHoleInfo.BlackHoleSize.Small;
        //    blackHole.transform.position = new Vector3(0, 0, -1);
        //    var blackHoleInfo = new BlackHoleInfo() { size = BlackHoleInfo.BlackHoleSize.Small, X = 0, Y = 0 };
        //    (int X, int Y) inputPoint = (2, -1);
        //    (int X, int Y) outputPoint = (-2, 1);

        //    var points = BHPoints.GetPoints(blackHoleInfo, Direction.UpLeft, inputPoint, -1);
        //    beam.lightBeamRenderer.positionCount = points.Length + 4;

        //    beam.lightBeamRenderer.SetPosition(beam.lightBeamRenderer.positionCount - 1, new Vector3(-5, 1, -1));
        //    beam.lightBeamRenderer.SetPosition(beam.lightBeamRenderer.positionCount - 2, new Vector3(outputPoint.X, outputPoint.Y, -1));
        //    beam.lightBeamRenderer.SetPosition(1, new Vector3(inputPoint.X, inputPoint.Y, -1));
        //    beam.lightBeamRenderer.SetPosition(0, new Vector3(4, -3, -1));

        //    for (int i = 2; i < points.Length + 2; i++)
        //    {
        //        //Debug.Log($"i: {i} => points[i - 2]: {points[i - 2]}");
        //        beam.lightBeamRenderer.SetPosition(i, points[i - 2]);
        //    }



        //    var beam2 = UnityEngine.Object.Instantiate(Prefabs.Instance.beamPrefab);
        //    inputPoint = (1, -2);
        //    outputPoint = (-1, 2);

        //    points = BHPoints.GetPoints(blackHoleInfo, Direction.UpLeft, inputPoint, -1);
        //    beam2.lightBeamRenderer.positionCount = points.Length + 4;

        //    beam2.lightBeamRenderer.SetPosition(beam2.lightBeamRenderer.positionCount - 1, new Vector3(-1, 5, -1));
        //    beam2.lightBeamRenderer.SetPosition(beam2.lightBeamRenderer.positionCount - 2, new Vector3(outputPoint.X, outputPoint.Y, -1));
        //    beam2.lightBeamRenderer.SetPosition(1, new Vector3(inputPoint.X, inputPoint.Y, -1));
        //    beam2.lightBeamRenderer.SetPosition(0, new Vector3(3, -4, -1));

        //    for (int i = 2; i < points.Length + 2; i++)
        //    {
        //        //Debug.Log($"i: {i} => points[i - 2]: {points[i - 2]}");
        //        beam2.lightBeamRenderer.SetPosition(i, points[i - 2]);
        //    }
        //}


        //public static void DownDirection()
        //{
        //    var beam = UnityEngine.Object.Instantiate(Prefabs.Instance.beamPrefab);
        //    var blackHole = UnityEngine.Object.Instantiate(Prefabs.Instance.blackHoleSmallPrefab);
        //    blackHole.size = BlackHoleInfo.BlackHoleSize.Small;
        //    blackHole.transform.position = new Vector3(0, 0, -1);
        //    var blackHoleInfo = new BlackHoleInfo() { size = BlackHoleInfo.BlackHoleSize.Small, X = 0, Y = 0 };
        //    (int X, int Y) inputPoint = (1, 2);
        //    (int X, int Y) outputPoint = (-1, -2);

        //    var points = BHPoints.GetPoints(blackHoleInfo, Direction.Down, inputPoint, -1);
        //    beam.lightBeamRenderer.positionCount = points.Length + 4;

        //    beam.lightBeamRenderer.SetPosition(0, new Vector3(1, 5, -1));
        //    beam.lightBeamRenderer.SetPosition(1, new Vector3(inputPoint.X, inputPoint.Y, -1));
        //    beam.lightBeamRenderer.SetPosition(beam.lightBeamRenderer.positionCount - 2, new Vector3(outputPoint.X, outputPoint.Y, -1));
        //    beam.lightBeamRenderer.SetPosition(beam.lightBeamRenderer.positionCount - 1, new Vector3(-3, -4, -1));

        //    for (int i = 2; i < points.Length + 2; i++)
        //    {
        //        //Debug.Log($"i: {i} => points[i - 2]: {points[i - 2]}");
        //        beam.lightBeamRenderer.SetPosition(i, points[i - 2]);
        //    }



        //    var beam2 = UnityEngine.Object.Instantiate(Prefabs.Instance.beamPrefab);
        //    inputPoint = (-1, 2);
        //    outputPoint = (1, -2);

        //    points = BHPoints.GetPoints(blackHoleInfo, Direction.Down, inputPoint, -1);
        //    beam2.lightBeamRenderer.positionCount = points.Length + 4;

        //    beam2.lightBeamRenderer.SetPosition(0, new Vector3(-1, 5, -1));
        //    beam2.lightBeamRenderer.SetPosition(1, new Vector3(inputPoint.X, inputPoint.Y, -1));
        //    beam2.lightBeamRenderer.SetPosition(beam2.lightBeamRenderer.positionCount - 2, new Vector3(outputPoint.X, outputPoint.Y, -1));
        //    beam2.lightBeamRenderer.SetPosition(beam2.lightBeamRenderer.positionCount - 1, new Vector3(3, -4, -1));

        //    for (int i = 2; i < points.Length + 2; i++)
        //    {
        //        //    Debug.Log($"i: {i} => points[i - 2]: {points[i - 2]}");
        //        beam2.lightBeamRenderer.SetPosition(i, points[i - 2]);
        //    }
        //}
        //public static void UpDirection()
        //{
        //    var beam = UnityEngine.Object.Instantiate(Prefabs.Instance.beamPrefab);
        //    var blackHole = UnityEngine.Object.Instantiate(Prefabs.Instance.blackHoleSmallPrefab);
        //    blackHole.size = BlackHoleInfo.BlackHoleSize.Small;
        //    blackHole.transform.position = new Vector3(0, 0, -1);
        //    var blackHoleInfo = new BlackHoleInfo() { size = BlackHoleInfo.BlackHoleSize.Small, X = 0, Y = 0 };
        //    (int X, int Y) inputPoint = (1, -2);
        //    (int X, int Y) outputPoint = (-1, 2);

        //    var points = BHPoints.GetPoints(blackHoleInfo, Direction.Up, inputPoint, -1);
        //    beam.lightBeamRenderer.positionCount = points.Length + 4;

        //    beam.lightBeamRenderer.SetPosition(0, new Vector3(1, -5, -1));
        //    beam.lightBeamRenderer.SetPosition(1, new Vector3(inputPoint.X, inputPoint.Y, -1));
        //    beam.lightBeamRenderer.SetPosition(beam.lightBeamRenderer.positionCount - 2, new Vector3(outputPoint.X, outputPoint.Y, -1));
        //    beam.lightBeamRenderer.SetPosition(beam.lightBeamRenderer.positionCount - 1, new Vector3(-3, 4, -1));

        //    for (int i = 2; i < points.Length + 2; i++)
        //    {
        //        //Debug.Log($"i: {i} => points[i - 2]: {points[i - 2]}");
        //        beam.lightBeamRenderer.SetPosition(i, points[i - 2]);
        //    }


        //    var beam2 = UnityEngine.Object.Instantiate(Prefabs.Instance.beamPrefab);
        //    inputPoint = (-1, -2);
        //    outputPoint = (1, 2);

        //    points = BHPoints.GetPoints(blackHoleInfo, Direction.Up, inputPoint, -1);
        //    beam2.lightBeamRenderer.positionCount = points.Length + 4;

        //    beam2.lightBeamRenderer.SetPosition(0, new Vector3(-1, -5, -1));
        //    beam2.lightBeamRenderer.SetPosition(1, new Vector3(inputPoint.X, inputPoint.Y, -1));
        //    beam2.lightBeamRenderer.SetPosition(beam2.lightBeamRenderer.positionCount - 2, new Vector3(outputPoint.X, outputPoint.Y, -1));
        //    beam2.lightBeamRenderer.SetPosition(beam2.lightBeamRenderer.positionCount - 1, new Vector3(3, 4, -1));

        //    for (int i = 2; i < points.Length + 2; i++)
        //    {
        //        //    Debug.Log($"i: {i} => points[i - 2]: {points[i - 2]}");
        //        beam2.lightBeamRenderer.SetPosition(i, points[i - 2]);
        //    }
        //}

        //public static void DownRightDirection()
        //{
        //    var beam = UnityEngine.Object.Instantiate(Prefabs.Instance.beamPrefab);
        //    var blackHole = UnityEngine.Object.Instantiate(Prefabs.Instance.blackHoleSmallPrefab);
        //    var blackHoleInfo = new BlackHoleInfo() { size = BlackHoleInfo.BlackHoleSize.Small, X = 0, Y = 0 };
        //    blackHole.size = BlackHoleInfo.BlackHoleSize.Small;
        //    blackHole.transform.position = new Vector3(0, 0, -1);
        //    (int X, int Y) inputPoint = (-1, 2);
        //    (int X, int Y) outputPoint = (1, -2);

        //    var points = BHPoints.GetPoints(blackHoleInfo, Direction.DownRight, inputPoint, -1);
        //    beam.lightBeamRenderer.positionCount = points.Length + 4;

        //    beam.lightBeamRenderer.SetPosition(0, new Vector3(-3, 4, -1));
        //    beam.lightBeamRenderer.SetPosition(1, new Vector3(inputPoint.X, inputPoint.Y, -1));
        //    beam.lightBeamRenderer.SetPosition(beam.lightBeamRenderer.positionCount - 2, new Vector3(outputPoint.X, outputPoint.Y, -1));
        //    beam.lightBeamRenderer.SetPosition(beam.lightBeamRenderer.positionCount - 1, new Vector3(1, -5, -1));

        //    for (int i = 2; i < points.Length + 2; i++)
        //    {
        //        //Debug.Log($"i: {i} => points[i - 2]: {points[i - 2]}");
        //        beam.lightBeamRenderer.SetPosition(i, points[i - 2]);
        //    }



        //    var beam2 = UnityEngine.Object.Instantiate(Prefabs.Instance.beamPrefab);
        //    inputPoint = (-2, 1);
        //    outputPoint = (2, -1);

        //    points = BHPoints.GetPoints(blackHoleInfo, Direction.DownRight, inputPoint, -1);
        //    beam2.lightBeamRenderer.positionCount = points.Length + 4;

        //    beam2.lightBeamRenderer.SetPosition(0, new Vector3(-4, 3, -1));
        //    beam2.lightBeamRenderer.SetPosition(1, new Vector3(inputPoint.X, inputPoint.Y, -1));
        //    beam2.lightBeamRenderer.SetPosition(beam2.lightBeamRenderer.positionCount - 2, new Vector3(outputPoint.X, outputPoint.Y, -1));
        //    beam2.lightBeamRenderer.SetPosition(beam2.lightBeamRenderer.positionCount - 1, new Vector3(5, -1, -1));

        //    for (int i = 2; i < points.Length + 2; i++)
        //    {
        //        //    Debug.Log($"i: {i} => points[i - 2]: {points[i - 2]}");
        //        beam2.lightBeamRenderer.SetPosition(i, points[i - 2]);
        //    }
        //}
        //public static void DownLeftDirection()
        //{
        //    var beam = UnityEngine.Object.Instantiate(Prefabs.Instance.beamPrefab);
        //    var blackHole = UnityEngine.Object.Instantiate(Prefabs.Instance.blackHoleSmallPrefab);
        //    blackHole.size = BlackHoleInfo.BlackHoleSize.Small;
        //    blackHole.transform.position = new Vector3(0, 0, -1);
        //    var blackHoleInfo = new BlackHoleInfo() { size = BlackHoleInfo.BlackHoleSize.Small, X = 0, Y = 0 };
        //    (int X, int Y) inputPoint = (1, 2);
        //    (int X, int Y) outputPoint = (-1, -2);

        //    var points = BHPoints.GetPoints(blackHoleInfo, Direction.DownLeft, inputPoint, -1);
        //    beam.lightBeamRenderer.positionCount = points.Length + 4;

        //    beam.lightBeamRenderer.SetPosition(0, new Vector3(3, 4, -1));
        //    beam.lightBeamRenderer.SetPosition(1, new Vector3(inputPoint.X, inputPoint.Y, -1));
        //    beam.lightBeamRenderer.SetPosition(beam.lightBeamRenderer.positionCount - 2, new Vector3(outputPoint.X, outputPoint.Y, -1));
        //    beam.lightBeamRenderer.SetPosition(beam.lightBeamRenderer.positionCount - 1, new Vector3(-1, -5, -1));

        //    for (int i = 2; i < points.Length + 2; i++)
        //    {
        //        //Debug.Log($"i: {i} => points[i - 2]: {points[i - 2]}");
        //        beam.lightBeamRenderer.SetPosition(i, points[i - 2]);
        //    }



        //    var beam2 = UnityEngine.Object.Instantiate(Prefabs.Instance.beamPrefab);
        //    inputPoint = (2, 1);
        //    outputPoint = (-2, -1);

        //    points = BHPoints.GetPoints(blackHoleInfo, Direction.DownLeft, inputPoint, -1);
        //    beam2.lightBeamRenderer.positionCount = points.Length + 4;

        //    beam2.lightBeamRenderer.SetPosition(0, new Vector3(4, 3, -1));
        //    beam2.lightBeamRenderer.SetPosition(1, new Vector3(inputPoint.X, inputPoint.Y, -1));
        //    beam2.lightBeamRenderer.SetPosition(beam2.lightBeamRenderer.positionCount - 2, new Vector3(outputPoint.X, outputPoint.Y, -1));
        //    beam2.lightBeamRenderer.SetPosition(beam2.lightBeamRenderer.positionCount - 1, new Vector3(-5, -1, -1));

        //    for (int i = 2; i < points.Length + 2; i++)
        //    {
        //        //    Debug.Log($"i: {i} => points[i - 2]: {points[i - 2]}");
        //        beam2.lightBeamRenderer.SetPosition(i, points[i - 2]);
        //    }
        //}


    }




    

















    





















    public static void Research()
    {
        for(int i = 0; i < AllDirectionsArray.Length; i++)
        {
            var direction = AllDirectionsArray[i];
            int halfPosition = (int)direction - 180;
            int Xstep = MathF.Sign(halfPosition % 180);
            int Ystep = MathF.Sign((MathF.Abs(halfPosition) - 90) % 180);

            Debug.Log($"value: {AllDirectionsArray[i]}, Xstep: {Xstep}, Ystep: {Ystep}" );
        }
    }


















    //public static void CheckMethod()
    //{
    //    (int X, int Y)[] coordinates = new (int X, int Y)[10];

    //    coordinates[0] = (10, 10);
    //    coordinates[1] = (10, 12);
    //    coordinates[2] = (8, 12);
    //    coordinates[3] = (8, 10);
    //    coordinates[4] = (8, 8);
    //    coordinates[5] = (10, 8);
    //    coordinates[6] = (12, 8);
    //    coordinates[7] = (12, 10);
    //    coordinates[8] = (12, 12);
    //    coordinates[9] = (13, 15);

    //    //var res = DoesPointLieInDirection(coordinates[0], coordinates[6], out Direction direction);
    //    //Debug.Log($"6. {res} - {direction}");

    //    for (int i = 1; i < coordinates.Length; i++)
    //    {
    //        var res = DoesPointLieInDirection(coordinates[0], coordinates[i], out Direction direction);

    //        Debug.Log($"{i}. {res} - {direction}");
    //    }

    //}



    //public static void CheckMethod()
    //{
    //    (int X, int Y)[] coordinates = new (int X, int Y)[13];

    //    coordinates[0] = (10,10);
    //    coordinates[1] = (8,2);
    //    coordinates[2] = (2,8);
    //    coordinates[3] = (2,2);
    //    coordinates[4] = (2,12);
    //    coordinates[5] = (8,20);
    //    coordinates[6] = (8,12);
    //    coordinates[7] = (12,20);
    //    coordinates[8] = (20,12);
    //    coordinates[9] = (12,12);
    //    coordinates[10] = (20,8);
    //    coordinates[11] = (12,2);
    //    coordinates[12] = (12,8);

    //    for(int i = 1; i < coordinates.Length; i++)
    //    {
    //        var res = GetDisjointDirections(coordinates[0], coordinates[i]);

    //        for (int j = 0; j < res.Length; j++)
    //        {
    //            Debug.Log($"{res[j].coordinates} - {res[j].direction}");

    //        }
    //    }

    //}


    ///// <summary>
    ///// Рассчитывает координаты пересечения лучей из заданных точек в заданных направлениях.
    ///// </summary>
    ///// <param name="firstPoint">Начало первого луча.</param>
    ///// <param name="firstDirection">Направление первого луча.</param>
    ///// <param name="secondPoint">Начало второго луча.</param>
    ///// <param name="secondDirection">Направление второго луча.</param>
    ///// <param name="coordinates">Координаты пересечения. В случае отсутствия пересечения - (-1000, -1000).</param>
    ///// <returns>true, если есть пересечение в точке с ЦЕЛЫМИ координатами, false - если нет пересечения, либо пересечение в точке с НЕЦЕЛЫМИ координатами.</returns>
    //public static bool GetIntersectionsCoordinates((int X, int Y) firstPoint, Direction firstDirection, (int X, int Y) secondPoint,  Direction secondDirection, out (int X, int Y) coordinates)
    //{
    //    var firstPointEnd = GetCoordinatesForSecondPointOnDirectionForCalculation(firstPoint, firstDirection);
    //    var secondPointEnd = GetCoordinatesForSecondPointOnDirectionForCalculation(secondPoint, secondDirection);

    //    // (x1, y1) - первая точка на первой прямой, (x2, y2) - вторая точка на первой прямой, (x3, y3) - первая точка на второй прямой, (x4, y4) - вторая точка на второй прямой.
    //    float x1 = firstPoint.X,
    //        x2 = firstPointEnd.X,
    //        x3 = secondPoint.X,
    //        x4 = secondPointEnd.X,
    //        y1 = firstPoint.Y,
    //        y2 = firstPointEnd.Y,
    //        y3 = secondPoint.Y,
    //        y4 = secondPointEnd.Y;

    //    // (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)
    //    float denominator = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
    //    //Debug.Log($"denominator: {denominator}, fD: {firstDirection}, sD: {secondDirection}");


    //    if (denominator == 0)
    //    {
    //        Debug.Log("The points lie on the same straight line or on parallel ones. There are no intersections.");
    //        coordinates = (-1000, -1000);
    //        return false;
    //    }

    //    float partOne = x1 * y2 - y1 * x2;
    //    float partTwo = x3 * y4 - y3 * x4;
    //    float numeratorX = partOne * (x3 - x4) - (x1 - x2) * partTwo;
    //    float numeratorY = partOne * (y3 - y4) - (y1 - y2) * partTwo;
    //    float resX = numeratorX / denominator;
    //    float resY = numeratorY / denominator;
    //    //Debug.Log($"intersection: ({resX}, {resY})");

    //    if (resX % 1 != 0 || resY % 1 != 0)
    //    {
    //        //Debug.Log("Пересечение в точке с координатами в виде не целых чисел.");
    //        coordinates = (-1000, -1000);
    //        return false;
    //    }

    //    coordinates.X = (int)resX;
    //    coordinates.Y = (int)resY;
    //    return true;
    //}

    /// <summary>
    /// Рассчитывает координаты всех точек пересечений луча (направления), испущенного из первой точки и всех возможных лучей, испущенных из второй.
    /// </summary>
    /// <param name="firstPoint">Координаты первой точки.</param>
    /// <param name="directionFromFirstPoint">Направления луча из первой точки.</param>
    /// <param name="secondPoint">Координаты второй точки.</param>
    /// <returns>Cписок координат пересечений.</returns>
    //public static List<(int X, int Y)> GetIntersectionsCoordinates_Old((int X, int Y) firstPoint, Direction directionFromFirstPoint, (int X, int Y) secondPoint)
    //{
    //    List<(int X, int Y)> res = new List<(int X, int Y)>();

    //    Direction[] directions = new Direction[4]
    //    {
    //        Direction.Up,
    //        Direction.UpLeft,
    //        Direction.Left,
    //        Direction.DownLeft
    //    };

    //    for (int i = 0; i < directions.Length; i++)
    //    {
    //        if (directions[i] == directionFromFirstPoint) continue;

    //        if (GetIntersectionsCoordinates(firstPoint, directionFromFirstPoint, secondPoint, directions[i], out var coordinates))
    //        {
    //            //Debug.Log($"{directions[i]}, {directions[j]}. Coordinates: - {coordinates}");
    //            if (DoesPointLieInDirection(firstPoint, coordinates, out var direction) && direction == directionFromFirstPoint)
    //            {
    //                res.Add(coordinates);
    //                Debug.Log($"Coordinates added: {coordinates}, firstPoint: {firstPoint}, directionFromFirstPoint: {directionFromFirstPoint}, направление от третьей точки ко второй: {directions[i]} ");

    //            }
    //        }
    //    }
    //    return res;
    //}





    //public static void CheckMethod_GetIntersectionsCoordinates()
    //{
    //    var firstPoint = (3, 4);
    //    var secondPoint = (7, 8);

    //    Matrix matrix = new Matrix(10, 20);
    //    var directions = AllDirectionsArray;
    //    List<(int X, int Y)> intersections = new List<(int X, int Y)>();

    //    foreach(var dir in directions)
    //    {
    //        intersections.AddRange(GetIntersectionsCoordinates(matrix, firstPoint, dir, secondPoint));
    //    }

    //    int counter = 1;
    //    foreach (var res in intersections)
    //    {
    //        Debug.Log($"{counter}. intersection on: {res}");
    //        counter++;
    //    }

    //}



    ///// <summary>
    ///// Рассчитывает координаты всех точек пересечений всех прямых имеющихся направлений (Direction), которые проходят через заданные две точки.
    ///// Например, пересечение линии направления Up-Down, проходящей через первую точку, и линии Right-Left, проходящей через вторую точку.
    ///// </summary>
    ///// <param name="firstPoint">Координаты первой точки.</param>
    ///// <param name="secondPoint">Координаты второй точки.</param>
    ///// <returns>Список координат пересечений.</returns>
    //public static List<(int X, int Y)> GetIntersectionsCoordinates((int X, int Y) firstPoint, (int X, int Y) secondPoint)
    //{
    //    List<(int X, int Y)> res = new List<(int X, int Y)>();

    //    Direction[] directions = new Direction[4]
    //    {
    //        Direction.Up,
    //        Direction.UpLeft,
    //        Direction.Left,
    //        Direction.DownLeft
    //    };

    //    for (int i = 0; i < directions.Length; i++)
    //    {
    //        for (int j = 0; j < directions.Length; j++)
    //        {
    //            if (i == j) continue;

    //            if (GetIntersectionsCoordinates(firstPoint, directions[i], secondPoint, directions[j], out var coordinates))
    //            {
    //                //Debug.Log($"{directions[i]}, {directions[j]}. Coordinates: - {coordinates}");
    //                if(!res.Contains(coordinates))
    //                    res.Add(coordinates);
    //            }
    //        }
    //    }
    //    return res;
    //}


    //public static void CheckMethod()
    //{
    //    var digits = new List<int>() { 0, 1,2,3,4,5,6, 7, 8,9};

    //    for (int i = 0; i < 110; i++)
    //    {
    //        var res = digits.GetPercentNext(i);
    //        Debug.Log($"{i} percent => res: {res}");
    //    }
    //}

    ///// <summary>
    ///// Возвращает точку, лежащую по заданному направлению от заданной точки, с координатами +1 (для рассчета пересечений направлений).
    ///// </summary>
    ///// <param name="point"></param>
    ///// <param name="direction"></param>
    ///// <returns></returns>
    //private static (int X, int Y) GetCoordinatesForSecondPointOnDirectionForCalculation((int X, int Y) point, Direction direction)
    //{
    //    return direction switch
    //    {
    //        Direction.Up => (point.X, point.Y + 1),
    //        Direction.UpLeft => (point.X - 1, point.Y + 1),
    //        Direction.Left => (point.X - 1, point.Y),
    //        Direction.DownLeft => (point.X - 1, point.Y - 1),
    //        Direction.Down => (point.X, point.Y - 1),
    //        Direction.DownRight => (point.X + 1, point.Y),
    //        Direction.Right => (point.X + 1, point.Y),
    //        Direction.UpRight => (point.X + 1, point.Y + 1),
    //        _ => throw new System.NotImplementedException()
    //    };
    //}

}


