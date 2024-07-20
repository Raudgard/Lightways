using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Fields
{
    public static class BHPoints
    {
        private enum InputDirectionType
        {
            /// <summary>
            /// Луч сначала проходит НАД черной дырой или СПРАВА от черной дыры (в направлениях Up и Down).
            /// </summary>
            Main,

            /// <summary>
            /// Луч проходит сначала ПОД черной дырой или СЛЕВА от черной дыры (в направлениях Up и Down).
            /// </summary>
            Mirror
        }


        /// <summary>
        /// Получить точки искривления луча под воздействием черной дыры для LineRenderer этого луча.
        /// </summary>
        /// <param name="blackHole"></param>
        /// <param name="direction"></param>
        /// <param name="inputPoint"></param>
        /// <param name="zPosition"></param>
        /// <returns></returns>
        public static Vector3[] GetPoints(BlackHoleInfo blackHole, Direction direction, (int X, int Y) inputPoint, float zPosition)
        {
            var inputType = DefineInputDirectionType(blackHole, direction, inputPoint);
            Vector2[] points;
            Vector3[] res;

            points = blackHole.size switch
            {
                BlackHoleInfo.BlackHoleSize.Small => Small.GetPoints(direction, inputType),
                BlackHoleInfo.BlackHoleSize.Medium => Medium.GetPoints(direction, inputType),
                BlackHoleInfo.BlackHoleSize.Large => Large.GetPoints(direction, inputType),
                BlackHoleInfo.BlackHoleSize.Supermassive => Supermassive.GetPoints(direction, inputType),
                _ => throw new System.Exception()
            };



            res = new Vector3[points.Length];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = new Vector3(points[i].x, points[i].y, zPosition);
            }
            return res;
        }


        /// <summary>
        /// Определяет тип луча относительно его прохождения мимо черной дыры (т.к. относительно прохождения лучи парны).
        /// Если луч сначала проходит НАД черной дырой то main, или СПРАВА от черной дыры (в направлениях Up и Down).
        /// Если проходит сначала ПОД черной дырой то mirror, или СЛЕВА от черной дыры (в направлениях Up и Down).
        /// </summary>
        /// <param name="blackHole">Черная дыра.</param>
        /// <param name="direction">Направление луча.</param>
        /// <param name="inputPoint">Точка начала искривления луча под воздействием черной дыры.</param>
        /// <returns></returns>
        private static InputDirectionType DefineInputDirectionType(BlackHoleInfo blackHole, Direction direction, (int X, int Y) inputPoint)
        {
            var BHposX = blackHole.X;
            var BHposY = blackHole.Y;

            if (direction == Direction.Right)
            {
                if (inputPoint.Y > BHposY)
                {
                    //Debug.Log($"1 case. Direction.Right Main");
                    return InputDirectionType.Main;
                }
                else
                {
                    //Debug.Log($"2 case. Direction.Right Mirror");
                    return InputDirectionType.Mirror;
                }
            }

            if (direction == Direction.Down)
            {
                if (inputPoint.X > BHposX)
                {
                    //Debug.Log($"3 case. Direction.Down Main");
                    return InputDirectionType.Main;
                }
                else
                {
                    //Debug.Log($"4 case. Direction.Down Mirror");
                    return InputDirectionType.Mirror;
                }
            }

            if (direction == Direction.Left)
            {
                if (inputPoint.Y > BHposY)
                {
                    //Debug.Log($"5 case. Direction.Left Main");
                    return InputDirectionType.Main;
                }
                else
                {
                    //Debug.Log($"6 case. Direction.Left Mirror");
                    return InputDirectionType.Mirror;
                }
            }

            if (direction == Direction.Up)
            {
                if (inputPoint.X > BHposX)
                {
                    //Debug.Log($"7 case. Direction.Up Main");
                    return InputDirectionType.Main;
                }
                else
                {
                    //Debug.Log($"8 case. Direction.Up Mirror");
                    return InputDirectionType.Mirror;
                }
            }

            if (direction == Direction.UpLeft)
            {
                //if (BHposY - inputPoint.Y == 1)
                if (BHposY == inputPoint.Y)
                {
                    //Debug.Log($"9 case. Direction.UpLeft Main");
                    return InputDirectionType.Main;
                }
                else
                {
                    //Debug.Log($"10 case. Direction.UpLeft Mirror");
                    return InputDirectionType.Mirror;
                }
            }

            if (direction == Direction.UpRight)
            {
                //if (BHposY - inputPoint.Y == 1)
                if (BHposY == inputPoint.Y) 
                {
                    //Debug.Log($"11 case. Direction.UpRight Main");
                    return InputDirectionType.Main;
                }
                else
                {
                    //Debug.Log($"12 case. Direction.UpRight Mirror");
                    return InputDirectionType.Mirror;
                }
            }

            if (direction == Direction.DownRight)
            {
                //if (BHposX - inputPoint.X == 1)
                if (BHposX == inputPoint.X)
                {
                    //Debug.Log($"13 case. Direction.DownRight Main");
                    return InputDirectionType.Main;
                }
                else
                {
                    //Debug.Log($"14 case. Direction.DownRight Mirror");
                    return InputDirectionType.Mirror;
                }
            }

            if (direction == Direction.DownLeft)
            {
                //if (BHposX - inputPoint.X == -1)
                if (BHposX == inputPoint.X)
                {
                    //Debug.Log($"15 case. Direction.DownLeft Main");
                    return InputDirectionType.Main;
                }
                else
                {
                    //Debug.Log($"16 case. Direction.DownLeft Mirror");
                    return InputDirectionType.Mirror;
                }
            }




            //if ((direction == Direction.Right && inputPoint.Y > BHposY) ||
            //   (direction == Direction.UpLeft && BHposY - inputPoint.Y == 1))
            //{
            //    Debug.Log($"1 case. InputDirectionType.Main");
            //    return InputDirectionType.Main;
            //}
            //if ((direction == Direction.Right && inputPoint.Y < BHposY) ||
            //   (direction == Direction.DownLeft /*&& blackHole.transform.position.y - inputPoint.Y == -1*/)) //DownLeft is always mirror
            //{
            //    Debug.Log($"2 case. InputDirectionType.Mirror");
            //    return InputDirectionType.Mirror;
            //}


            //if ((direction == Direction.Down && inputPoint.X > BHposX) ||
            //   (direction == Direction.UpRight /*&& inputPoint.Y > blackHole.transform.position.y*/)) //UpRight is always main
            //{
            //    Debug.Log($"3 case. InputDirectionType.Main");
            //    return InputDirectionType.Main;
            //}
            //if ((direction == Direction.Down && inputPoint.X < BHposX) ||
            //   (direction == Direction.UpLeft && BHposX - inputPoint.X == -1))
            //{
            //    Debug.Log($"4 case. InputDirectionType.Mirror");
            //    return InputDirectionType.Mirror;
            //}


            //if (direction == Direction.Left && inputPoint.Y > BHposY) /*||*/
            ////   (direction == Direction.UpRight && blackHole.transform.position.y - inputPoint.Y == 1)) //UpRight is always main
            //{
            //    Debug.Log($"5 case. InputDirectionType.Main");
            //    return InputDirectionType.Main;
            //}
            //if ((direction == Direction.Left && inputPoint.Y < BHposY) ||
            //   (direction == Direction.DownRight && BHposY - inputPoint.Y == -1))
            //{
            //    Debug.Log($"6 case. InputDirectionType.Mirror");
            //    return InputDirectionType.Mirror;
            //}


            //if ((direction == Direction.Up && inputPoint.X > BHposX) ||
            //   (direction == Direction.DownRight && BHposX - inputPoint.X == 1)) 
            //{
            //    Debug.Log($"7 case. InputDirectionType.Main");
            //    return InputDirectionType.Main;
            //}
            //if (direction == Direction.Up && inputPoint.X < BHposX) /*||*/
            ////(direction == Direction.DownLeft && BHposX - inputPoint.Y == -1)) //DownLeft is always mirror
            //{
            //    Debug.Log($"8 case. InputDirectionType.Mirror");
            //    return InputDirectionType.Mirror;
            //}

            //Debug.Log($"{blackHole.transform.position}, direction: {direction}, inputPoint: {inputPoint}");
            throw new System.Exception($"Ошибка в методе определения InputDirectionType. {blackHole}, direction: {direction}, inputPoint: {inputPoint}.");
        }


        private static Vector2[] ReversePoints(Vector2[] points)
        {
            return points.Reverse().ToArray();
        }


        /// <summary>
        /// 45 degrees
        /// </summary>
        private static class Small
        {
            private static Vector2[] pointsDirectionRight_Main =
            {
               new Vector2(-0.951f, 1f),
                new Vector2(-0.94f, 0.9992f),
                new Vector2(-0.92f, 0.9976f),
                new Vector2(-0.90f, 0.996f),
                new Vector2(-0.85f, 0.9919f),
                new Vector2(-0.80f, 0.9875f),
                new Vector2(-0.70f, 0.978f),
                new Vector2(-0.60f, 0.967f),
                new Vector2(-0.50f, 0.954f),
                new Vector2(-0.40f, 0.9381f),
                new Vector2(-0.30f, 0.9181f),
                new Vector2(-0.20f, 0.8922f),
                new Vector2(-0.15f, 0.8764f),
                new Vector2(-0.10f, 0.8582f),
                new Vector2(-0.05f, 0.8375f),

                new Vector2(0.0f, 0.814f),
                new Vector2(0.05f, 0.7877f),
                new Vector2(0.1f, 0.7586f),
                new Vector2(0.2f, 0.6937f),
                new Vector2(0.3f, 0.6194f),
                new Vector2(0.5f, 0.456f),
                new Vector2(0.8f, 0.1908f),

            };
            private static Vector2[] pointsDirectionRight_Mirror =
            {
                new Vector2(-0.951f, -1f),
                new Vector2(-0.94f, -0.9992f),
                new Vector2(-0.92f, -0.9976f),
                new Vector2(-0.9f, -0.996f),
                new Vector2(-0.85f, -0.9919f),
                new Vector2(-0.8f, -0.9875f),
                new Vector2(-0.7f, -0.978f),
                new Vector2(-0.6f, -0.967f),
                new Vector2(-0.5f, -0.954f),
                new Vector2(-0.4f, -0.9381f),
                new Vector2(-0.3f, -0.9181f),
                new Vector2(-0.2f, -0.8922f),
                new Vector2(-0.15f, -0.8764f),
                new Vector2(-0.1f, -0.8582f),
                new Vector2(-0.05f, -0.8375f),

                new Vector2(0.0f, -0.814f),
                new Vector2(0.05f, -0.7877f),
                new Vector2(0.1f, -0.7586f),
                new Vector2(0.2f, -0.6937f),
                new Vector2(0.3f, -0.6194f),
                new Vector2(0.5f, -0.456f),
                new Vector2(0.8f, -0.1908f),
            };
            private static Vector2[] pointsDirectionLeft_Main =
            {
                new Vector2(0.951f, 1f),
                new Vector2(0.94f, 0.9992f),
                new Vector2(0.92f, 0.9976f),
                new Vector2(0.90f, 0.996f),
                new Vector2(0.85f, 0.9919f),
                new Vector2(0.80f, 0.9875f),
                new Vector2(0.70f, 0.978f),
                new Vector2(0.60f, 0.967f),
                new Vector2(0.50f, 0.954f),
                new Vector2(0.40f, 0.9381f),
                new Vector2(0.30f, 0.9181f),
                new Vector2(0.20f, 0.8922f),
                new Vector2(0.15f, 0.8764f),
                new Vector2(0.10f, 0.8582f),
                new Vector2(0.05f, 0.8375f),

                new Vector2(0.0f, 0.814f),
                new Vector2(-0.05f, 0.7877f),
                new Vector2(-0.1f, 0.7586f),
                new Vector2(-0.2f, 0.6937f),
                new Vector2(-0.3f, 0.6194f),
                new Vector2(-0.5f, 0.456f),
                new Vector2(-0.8f, 0.1908f),
            };
            private static Vector2[] pointsDirectionLeft_Mirror =
            {
                new Vector2(0.951f, -1f),
                new Vector2(0.94f, -0.9992f),
                new Vector2(0.92f, -0.9976f),
                new Vector2(0.9f, -0.996f),
                new Vector2(0.85f, -0.9919f),
                new Vector2(0.8f, -0.9875f),
                new Vector2(0.7f, -0.978f),
                new Vector2(0.6f, -0.967f),
                new Vector2(0.5f, -0.954f),
                new Vector2(0.4f, -0.9381f),
                new Vector2(0.3f, -0.9181f),
                new Vector2(0.2f, -0.8922f),
                new Vector2(0.15f, -0.8764f),
                new Vector2(0.1f, -0.8582f),
                new Vector2(0.05f, -0.8375f),

                new Vector2(0.0f, -0.814f),
                new Vector2(-0.05f, -0.7877f),
                new Vector2(-0.1f, -0.7586f),
                new Vector2(-0.2f, -0.6937f),
                new Vector2(-0.3f, -0.6194f),
                new Vector2(-0.5f, -0.456f),
                new Vector2(-0.8f, -0.1908f),
                //new Vector2(f, 0.f),
            };
            private static Vector2[] pointsDirectionDown_Main =
            {
                new Vector2(1f, 0.951f),
                new Vector2(0.9992f, 0.94f),
                new Vector2(0.9976f, 0.92f),
                new Vector2(0.996f, 0.9f),
                new Vector2(0.9919f, 0.85f),
                new Vector2(0.9875f, 0.8f),
                new Vector2(0.978f, 0.7f),
                new Vector2(0.967f, 0.6f),
                new Vector2(0.954f, 0.5f),
                new Vector2(0.9381f, 0.4f),
                new Vector2(0.9181f, 0.3f),
                new Vector2(0.8922f, 0.2f),
                new Vector2(0.8764f, 0.15f),
                new Vector2(0.8582f, 0.1f),
                new Vector2(0.8375f, 0.05f),

                new Vector2(0.814f, 0.0f),
                new Vector2(0.7877f, -0.05f),
                new Vector2(0.7586f, -0.1f),
                new Vector2(0.6937f, -0.2f),
                new Vector2(0.6194f, -0.3f),
                new Vector2(0.456f, -0.5f),
                new Vector2(0.1908f, -0.8f),
                //new Vector2(0.436f, -0.4f), // перекресток, если брать отсюда координаты для 90 градусного отклонения.
            };
            private static Vector2[] pointsDirectionDown_Mirror =
            {
                new Vector2(-1f, 0.951f),
                new Vector2(-0.9992f, 0.94f),
                new Vector2(-0.9976f, 0.92f),
                new Vector2(-0.996f, 0.9f),
                new Vector2(-0.9919f, 0.85f),
                new Vector2(-0.9875f, 0.8f),
                new Vector2(-0.978f, 0.7f),
                new Vector2(-0.967f, 0.6f),
                new Vector2(-0.954f, 0.5f),
                new Vector2(-0.9381f, 0.4f),
                new Vector2(-0.9181f, 0.3f),
                new Vector2(-0.8922f, 0.2f),
                new Vector2(-0.8764f, 0.15f),
                new Vector2(-0.8582f, 0.1f),
                new Vector2(-0.8375f, 0.05f),

                new Vector2(-0.814f, 0.0f),
                new Vector2(-0.7877f, -0.05f),
                new Vector2(-0.7586f, -0.1f),
                new Vector2(-0.6937f, -0.2f),
                new Vector2(-0.6194f, -0.3f),
                new Vector2(-0.456f, -0.5f),
                new Vector2(-0.1908f, -0.8f),
            };
            private static Vector2[] pointsDirectionUp_Main =
            {
                new Vector2(1f, -0.951f),
                new Vector2(0.9992f, -0.94f),
                new Vector2(0.9976f, -0.92f),
                new Vector2(0.996f, -0.9f),
                new Vector2(0.9919f, -0.85f),
                new Vector2(0.9875f, -0.8f),
                new Vector2(0.978f, -0.7f),
                new Vector2(0.967f, -0.6f),
                new Vector2(0.954f, -0.5f),
                new Vector2(0.9381f, -0.4f),
                new Vector2(0.9181f, -0.3f),
                new Vector2(0.8922f, -0.2f),
                new Vector2(0.8764f, -0.15f),
                new Vector2(0.8582f, -0.1f),
                new Vector2(0.8375f, -0.05f),

                new Vector2(0.814f, 0.0f),
                new Vector2(0.7877f, 0.05f),
                new Vector2(0.7586f, 0.1f),
                new Vector2(0.6937f, 0.2f),
                new Vector2(0.6194f, 0.3f),
                new Vector2(0.456f, 0.5f),
                new Vector2(0.1908f, 0.8f),
            };
            private static Vector2[] pointsDirectionUp_Mirror =
            {
                new Vector2(-1f, -0.951f),
                new Vector2(-0.9992f, -0.94f),
                new Vector2(-0.9976f, -0.92f),
                new Vector2(-0.996f, -0.9f),
                new Vector2(-0.9919f, -0.85f),
                new Vector2(-0.9875f, -0.8f),
                new Vector2(-0.978f, -0.7f),
                new Vector2(-0.967f, -0.6f),
                new Vector2(-0.954f, -0.5f),
                new Vector2(-0.9381f, -0.4f),
                new Vector2(-0.9181f, -0.3f),
                new Vector2(-0.8922f, -0.2f),
                new Vector2(-0.8764f, -0.15f),
                new Vector2(-0.8582f, -0.1f),
                new Vector2(-0.8375f, -0.05f),

                new Vector2(-0.814f, 0.0f),
                new Vector2(-0.7877f, 0.05f),
                new Vector2(-0.7586f, 0.1f),
                new Vector2(-0.6937f, 0.2f),
                new Vector2(-0.6194f, 0.3f),
                new Vector2(-0.456f, 0.5f),
                new Vector2(-0.1908f, 0.8f),
            };


            public static Vector2[] GetPoints(Direction direction, InputDirectionType inputDirectionType)
            {
                return direction switch
                {
                    Direction.Right => inputDirectionType == InputDirectionType.Main ? pointsDirectionRight_Main : pointsDirectionRight_Mirror,
                    Direction.Left => inputDirectionType == InputDirectionType.Main ? pointsDirectionLeft_Main : pointsDirectionLeft_Mirror,
                    Direction.Down => inputDirectionType == InputDirectionType.Main ? pointsDirectionDown_Main : pointsDirectionDown_Mirror,
                    Direction.Up => inputDirectionType == InputDirectionType.Main ? pointsDirectionUp_Main : pointsDirectionUp_Mirror,

                    Direction.UpLeft => inputDirectionType == InputDirectionType.Main ? ReversePoints(pointsDirectionRight_Main) : ReversePoints(pointsDirectionDown_Mirror),
                    Direction.DownLeft => inputDirectionType == InputDirectionType.Main ? ReversePoints(pointsDirectionUp_Mirror) : ReversePoints(pointsDirectionRight_Mirror),
                    Direction.UpRight => inputDirectionType == InputDirectionType.Main ? ReversePoints(pointsDirectionLeft_Main) : ReversePoints(pointsDirectionDown_Main),
                    Direction.DownRight => inputDirectionType == InputDirectionType.Main ? ReversePoints(pointsDirectionUp_Main) : ReversePoints(pointsDirectionLeft_Mirror),

                    _ => throw new System.NotImplementedException()
                };
                
            }

        }

        /// <summary>
        /// 90 degrees
        /// </summary>
        private static class Medium
        {
            #region Hyperbola points (more cornering)
            //private static Vector2[] pointsDirectionRight_Main =
            //{
            //    new Vector2(-0.9f, 0.9967f),
            //    new Vector2(-0.8f, 0.9931f),
            //    new Vector2(-0.7f, 0.9891f),
            //    new Vector2(-0.6f, 0.9847f),
            //    new Vector2(-0.5f, 0.9796f),
            //    new Vector2(-0.4f, 0.9739f),
            //    new Vector2(-0.3f, 0.9674f),
            //    new Vector2(-0.2f, 0.9598f),
            //    new Vector2(-0.1f, 0.951f),
            //    new Vector2(0f, 0.9405f),
            //    new Vector2(0.1f, 0.9278f),
            //    new Vector2(0.2f, 0.9122f),
            //    new Vector2(0.3f, 0.8925f),
            //    new Vector2(0.4f, 0.8669f),
            //    new Vector2(0.45f, 0.8509f),
            //    new Vector2(0.5f, 0.8321f),
            //    new Vector2(0.55f, 0.8097f),
            //    new Vector2(0.6f, 0.7824f),
            //    new Vector2(0.65f, 0.7485f),
            //    new Vector2(0.7026f, 0.7026f), //точка симметрии

            //    new Vector2(0.7485f, 0.65f),
            //    new Vector2(0.7824f, 0.6f),
            //    new Vector2(0.8097f, 0.55f),
            //    new Vector2(0.8321f, 0.5f),
            //    new Vector2(0.8509f, 0.45f),
            //    new Vector2(0.8669f, 0.4f),
            //    new Vector2(0.8925f, 0.3f),
            //    new Vector2(0.9122f, 0.2f),
            //    new Vector2(0.9278f, 0.1f),
            //    new Vector2(0.9405f, 0f),
            //    new Vector2(0.951f, -0.1f),
            //    new Vector2(0.9598f, -0.2f),
            //    new Vector2(0.9674f, -0.3f),
            //    new Vector2(0.9739f, -0.4f),
            //    new Vector2(0.9796f, -0.5f),
            //    new Vector2(0.9847f, -0.6f),
            //    new Vector2(0.9891f, -0.7f),
            //    new Vector2(0.9931f, -0.8f),
            //    new Vector2(0.9967f, -0.9f),

            //};
            //private static Vector2[] pointsDirectionRight_Mirror =
            //{
            //    new Vector2(-0.9f, -0.9967f),
            //    new Vector2(-0.8f, -0.9931f),
            //    new Vector2(-0.7f, -0.9891f),
            //    new Vector2(-0.6f, -0.9847f),
            //    new Vector2(-0.5f, -0.9796f),
            //    new Vector2(-0.4f, -0.9739f),
            //    new Vector2(-0.3f, -0.9674f),
            //    new Vector2(-0.2f, -0.9598f),
            //    new Vector2(-0.1f, -0.951f),
            //    new Vector2(0f, -0.9405f),
            //    new Vector2(0.1f, -0.9278f),
            //    new Vector2(0.2f, -0.9122f),
            //    new Vector2(0.3f, -0.8925f),
            //    new Vector2(0.4f, -0.8669f),
            //    new Vector2(0.45f, -0.8509f),
            //    new Vector2(0.5f, -0.8321f),
            //    new Vector2(0.55f, -0.8097f),
            //    new Vector2(0.6f, -0.7824f),
            //    new Vector2(0.65f, -0.7485f),
            //    new Vector2(0.7026f, -0.7026f), //точка симметрии

            //    new Vector2(0.7485f, -0.65f),
            //    new Vector2(0.7824f, -0.6f),
            //    new Vector2(0.8097f, -0.55f),
            //    new Vector2(0.8321f, -0.5f),
            //    new Vector2(0.8509f, -0.45f),
            //    new Vector2(0.8669f, -0.4f),
            //    new Vector2(0.8925f, -0.3f),
            //    new Vector2(0.9122f, -0.2f),
            //    new Vector2(0.9278f, -0.1f),
            //    new Vector2(0.9405f, -0f),
            //    new Vector2(0.951f, 0.1f),
            //    new Vector2(0.9598f, 0.2f),
            //    new Vector2(0.9674f, 0.3f),
            //    new Vector2(0.9739f, 0.4f),
            //    new Vector2(0.9796f, 0.5f),
            //    new Vector2(0.9847f, 0.6f),
            //    new Vector2(0.9891f, 0.7f),
            //    new Vector2(0.9931f, 0.8f),
            //    new Vector2(0.9967f, 0.9f),
            //};
            //private static Vector2[] pointsDirectionLeft_Main =
            //{
            //    new Vector2(0.9f, 0.9967f),
            //    new Vector2(0.8f, 0.9931f),
            //    new Vector2(0.7f, 0.9891f),
            //    new Vector2(0.6f, 0.9847f),
            //    new Vector2(0.5f, 0.9796f),
            //    new Vector2(0.4f, 0.9739f),
            //    new Vector2(0.3f, 0.9674f),
            //    new Vector2(0.2f, 0.9598f),
            //    new Vector2(0.1f, 0.951f),
            //    new Vector2(0f, 0.9405f),
            //    new Vector2(-0.1f, 0.9278f),
            //    new Vector2(-0.2f, 0.9122f),
            //    new Vector2(-0.3f, 0.8925f),
            //    new Vector2(-0.4f, 0.8669f),
            //    new Vector2(-0.45f, 0.8509f),
            //    new Vector2(-0.5f, 0.8321f),
            //    new Vector2(-0.55f, 0.8097f),
            //    new Vector2(-0.6f, 0.7824f),
            //    new Vector2(-0.65f, 0.7485f),
            //    new Vector2(-0.7026f, 0.7026f), //точка симметрии

            //    new Vector2(-0.7485f, 0.65f),
            //    new Vector2(-0.7824f, 0.6f),
            //    new Vector2(-0.8097f, 0.55f),
            //    new Vector2(-0.8321f, 0.5f),
            //    new Vector2(-0.8509f, 0.45f),
            //    new Vector2(-0.8669f, 0.4f),
            //    new Vector2(-0.8925f, 0.3f),
            //    new Vector2(-0.9122f, 0.2f),
            //    new Vector2(-0.9278f, 0.1f),
            //    new Vector2(-0.9405f, 0f),
            //    new Vector2(-0.951f, -0.1f),
            //    new Vector2(-0.9598f, -0.2f),
            //    new Vector2(-0.9674f, -0.3f),
            //    new Vector2(-0.9739f, -0.4f),
            //    new Vector2(-0.9796f, -0.5f),
            //    new Vector2(-0.9847f, -0.6f),
            //    new Vector2(-0.9891f, -0.7f),
            //    new Vector2(-0.9931f, -0.8f),
            //    new Vector2(-0.9967f, -0.9f),
            //};
            //private static Vector2[] pointsDirectionLeft_Mirror =
            //{
            //    new Vector2(0.9f, -0.9967f),
            //    new Vector2(0.8f, -0.9931f),
            //    new Vector2(0.7f, -0.9891f),
            //    new Vector2(0.6f, -0.9847f),
            //    new Vector2(0.5f, -0.9796f),
            //    new Vector2(0.4f, -0.9739f),
            //    new Vector2(0.3f, -0.9674f),
            //    new Vector2(0.2f, -0.9598f),
            //    new Vector2(0.1f, -0.951f),
            //    new Vector2(0f, -0.9405f),
            //    new Vector2(-0.1f, -0.9278f),
            //    new Vector2(-0.2f, -0.9122f),
            //    new Vector2(-0.3f, -0.8925f),
            //    new Vector2(-0.4f, -0.8669f),
            //    new Vector2(-0.45f, -0.8509f),
            //    new Vector2(-0.5f, -0.8321f),
            //    new Vector2(-0.55f, -0.8097f),
            //    new Vector2(-0.6f, -0.7824f),
            //    new Vector2(-0.65f, -0.7485f),
            //    new Vector2(-0.7026f, -0.7026f), //точка симметрии

            //    new Vector2(-0.7485f, -0.65f),
            //    new Vector2(-0.7824f, -0.6f),
            //    new Vector2(-0.8097f, -0.55f),
            //    new Vector2(-0.8321f, -0.5f),
            //    new Vector2(-0.8509f, -0.45f),
            //    new Vector2(-0.8669f, -0.4f),
            //    new Vector2(-0.8925f, -0.3f),
            //    new Vector2(-0.9122f, -0.2f),
            //    new Vector2(-0.9278f, -0.1f),
            //    new Vector2(-0.9405f, 0f),
            //    new Vector2(-0.951f, 0.1f),
            //    new Vector2(-0.9598f, 0.2f),
            //    new Vector2(-0.9674f, 0.3f),
            //    new Vector2(-0.9739f, 0.4f),
            //    new Vector2(-0.9796f, 0.5f),
            //    new Vector2(-0.9847f, 0.6f),
            //    new Vector2(-0.9891f, 0.7f),
            //    new Vector2(-0.9931f, 0.8f),
            //    new Vector2(-0.9967f, 0.9f),
            //};
            //private static Vector2[] pointsDirectionDownRight_Main =
            //{
            //    new Vector2(0.0499f, 0.95f),
            //    new Vector2(0.1481f, 0.85f),
            //    new Vector2(0.2439f, 0.75f),
            //    new Vector2(0.3364f, 0.65f),
            //    new Vector2(0.4244f, 0.55f),
            //    new Vector2(0.5062f, 0.45f),
            //    new Vector2(0.544f, 0.4f),
            //    new Vector2(0.5793f, 0.35f),
            //    new Vector2(0.6116f, 0.3f),
            //    new Vector2(0.6404f, 0.25f),
            //    new Vector2(0.6651f, 0.2f),
            //    new Vector2(0.6852f, 0.15f),
            //    new Vector2(0.7f, 0.1f),
            //    new Vector2(0.7091f, 0.05f),
            //    new Vector2(0.7121f, 0f), //точка симметрии

            //    new Vector2(0.7091f, -0.05f),
            //    new Vector2(0.7f, -0.1f),
            //    new Vector2(0.6852f, -0.15f),
            //    new Vector2(0.6651f, -0.2f),
            //    new Vector2(0.6404f, -0.25f),
            //    new Vector2(0.6116f, -0.3f),
            //    new Vector2(0.5793f, -0.35f),
            //    new Vector2(0.544f, -0.4f),
            //    new Vector2(0.5062f, -0.45f),
            //    new Vector2(0.4244f, -0.55f),
            //    new Vector2(0.3364f, -0.65f),
            //    new Vector2(0.2439f, -0.75f),
            //    new Vector2(0.1481f, -0.85f),
            //    new Vector2(0.0499f, -0.95f),
            //};
            //private static Vector2[] pointsDirectionDownLeft_Main =
            //{
            //    new Vector2(-0.0499f, 0.95f),
            //    new Vector2(-0.1481f, 0.85f),
            //    new Vector2(-0.2439f, 0.75f),
            //    new Vector2(-0.3364f, 0.65f),
            //    new Vector2(-0.4244f, 0.55f),
            //    new Vector2(-0.5062f, 0.45f),
            //    new Vector2(-0.544f, 0.4f),
            //    new Vector2(-0.5793f, 0.35f),
            //    new Vector2(-0.6116f, 0.3f),
            //    new Vector2(-0.6404f, 0.25f),
            //    new Vector2(-0.6651f, 0.2f),
            //    new Vector2(-0.6852f, 0.15f),
            //    new Vector2(-0.7f, 0.1f),
            //    new Vector2(-0.7091f, 0.05f),
            //    new Vector2(-0.7121f, 0f), //точка симметрии

            //    new Vector2(-0.7091f, -0.05f),
            //    new Vector2(-0.7f, -0.1f),
            //    new Vector2(-0.6852f, -0.15f),
            //    new Vector2(-0.6651f, -0.2f),
            //    new Vector2(-0.6404f, -0.25f),
            //    new Vector2(-0.6116f, -0.3f),
            //    new Vector2(-0.5793f, -0.35f),
            //    new Vector2(-0.544f, -0.4f),
            //    new Vector2(-0.5062f, -0.45f),
            //    new Vector2(-0.4244f, -0.55f),
            //    new Vector2(-0.3364f, -0.65f),
            //    new Vector2(-0.2439f, -0.75f),
            //    new Vector2(-0.1481f, -0.85f),
            //    new Vector2(-0.0499f, -0.95f),
            //};
            //private static Vector2[] pointsDirectionUpLeft_Main =
            //{
            //    new Vector2(0.95f, 0.0499f),
            //    new Vector2(0.85f, 0.1481f),
            //    new Vector2(0.75f, 0.2439f),
            //    new Vector2(0.65f, 0.3364f),
            //    new Vector2(0.55f, 0.4244f),
            //    new Vector2(0.45f, 0.5062f),
            //    new Vector2(0.4f, 0.544f),
            //    new Vector2(0.35f, 0.5793f),
            //    new Vector2(0.3f, 0.6116f),
            //    new Vector2(0.25f, 0.6404f),
            //    new Vector2(0.2f, 0.6651f),
            //    new Vector2(0.15f, 0.6852f),
            //    new Vector2(0.1f, 0.7f),
            //    new Vector2(0.05f, 0.7091f),
            //    new Vector2(0f, 0.7121f), //точка симметрии

            //    new Vector2(-0.05f, 0.7091f),
            //    new Vector2(-0.1f, 0.7f),
            //    new Vector2(-0.15f, 0.6852f),
            //    new Vector2(-0.2f, 0.6651f),
            //    new Vector2(-0.25f, 0.6404f),
            //    new Vector2(-0.3f, 0.6116f),
            //    new Vector2(-0.35f, 0.5793f),
            //    new Vector2(-0.4f, 0.544f),
            //    new Vector2(-0.45f, 0.5062f),
            //    new Vector2(-0.55f, 0.4244f),
            //    new Vector2(-0.65f, 0.3364f),
            //    new Vector2(-0.75f, 0.2439f),
            //    new Vector2(-0.85f, 0.1481f),
            //    new Vector2(-0.95f, 0.0499f),
            //};
            //private static Vector2[] pointsDirectionDownLeft_Mirror =
            //{
            //    new Vector2(0.95f, -0.0499f),
            //    new Vector2(0.85f, -0.1481f),
            //    new Vector2(0.75f, -0.2439f),
            //    new Vector2(0.65f, -0.3364f),
            //    new Vector2(0.55f, -0.4244f),
            //    new Vector2(0.45f, -0.5062f),
            //    new Vector2(0.4f, -0.544f),
            //    new Vector2(0.35f, -0.5793f),
            //    new Vector2(0.3f, -0.6116f),
            //    new Vector2(0.25f, -0.6404f),
            //    new Vector2(0.2f, -0.6651f),
            //    new Vector2(0.15f, -0.6852f),
            //    new Vector2(0.1f, -0.7f),
            //    new Vector2(0.05f, -0.7091f),
            //    new Vector2(0f, -0.7121f), //точка симметрии

            //    new Vector2(-0.05f, -0.7091f),
            //    new Vector2(-0.1f, -0.7f),
            //    new Vector2(-0.15f, -0.6852f),
            //    new Vector2(-0.2f, -0.6651f),
            //    new Vector2(-0.25f, -0.6404f),
            //    new Vector2(-0.3f, -0.6116f),
            //    new Vector2(-0.35f, -0.5793f),
            //    new Vector2(-0.4f, -0.544f),
            //    new Vector2(-0.45f, -0.5062f),
            //    new Vector2(-0.55f, -0.4244f),
            //    new Vector2(-0.65f, -0.3364f),
            //    new Vector2(-0.75f, -0.2439f),
            //    new Vector2(-0.85f, -0.1481f),
            //    new Vector2(-0.95f, -0.0499f),
            //};
            #endregion

            #region Ellipse points (more rounded)
            private static Vector2[] pointsDirectionRight_Main =
            {
                new Vector2(-0.95f, 0.9995f),
                new Vector2(-0.90f, 0.9981f),
                new Vector2(-0.85f, 0.9957f),
                new Vector2(-0.80f, 0.9923f),
                new Vector2(-0.75f, 0.9879f),
                new Vector2(-0.70f, 0.9824f),
                new Vector2(-0.65f, 0.9759f),
                new Vector2(-0.60f, 0.9682f),
                new Vector2(-0.55f, 0.9594f),
                new Vector2(-0.50f, 0.9495f),
                new Vector2(-0.45f, 0.9383f),
                new Vector2(-0.40f, 0.9259f),
                new Vector2(-0.35f, 0.9122f),
                new Vector2(-0.30f, 0.8971f),
                new Vector2(-0.25f, 0.8807f),
                new Vector2(-0.20f, 0.8627f),
                new Vector2(-0.15f, 0.8433f),
                new Vector2(-0.10f, 0.8222f),
                new Vector2(-0.05f, 0.7994f),
                new Vector2(0.00f, 0.7749f),
                new Vector2(0.05f, 0.7484f),
                new Vector2(0.10f, 0.7199f),
                new Vector2(0.15f, 0.6893f),
                new Vector2(0.20f, 0.6563f),
                new Vector2(0.25f, 0.6208f),
                new Vector2(0.30f, 0.5826f),
                new Vector2(0.35f, 0.5415f),
                new Vector2(0.40f, 0.4971f),
                new Vector2(0.4495f, 0.4495f),//точка симметрии

                new Vector2(0.4971f, 0.40f),
                new Vector2(0.5415f, 0.35f),
                new Vector2(0.5826f, 0.30f),
                new Vector2(0.6208f, 0.25f),
                new Vector2(0.6563f, 0.20f),
                new Vector2(0.6893f, 0.15f),
                new Vector2(0.7199f, 0.10f),
                new Vector2(0.7484f, 0.05f),
                new Vector2(0.7749f, 0.00f),
                new Vector2(0.7994f, -0.05f),
                new Vector2(0.8222f, -0.10f),
                new Vector2(0.8433f, -0.15f),
                new Vector2(0.8627f, -0.20f),
                new Vector2(0.8807f, -0.25f),
                new Vector2(0.8971f, -0.30f),
                new Vector2(0.9122f, -0.35f),
                new Vector2(0.9259f, -0.40f),
                new Vector2(0.9383f, -0.45f),
                new Vector2(0.9495f, -0.50f),
                new Vector2(0.9594f, -0.55f),
                new Vector2(0.9682f, -0.60f),
                new Vector2(0.9759f, -0.65f),
                new Vector2(0.9824f, -0.70f),
                new Vector2(0.9879f, -0.75f),
                new Vector2(0.9923f, -0.80f),
                new Vector2(0.9957f, -0.85f),
                new Vector2(0.9981f, -0.90f),
                new Vector2(0.9995f, -0.95f),
            };
            private static Vector2[] pointsDirectionRight_Mirror =
            {
                new Vector2(-0.95f, -0.9995f),
                new Vector2(-0.90f, -0.9981f),
                new Vector2(-0.85f, -0.9957f),
                new Vector2(-0.80f, -0.9923f),
                new Vector2(-0.75f, -0.9879f),
                new Vector2(-0.70f, -0.9824f),
                new Vector2(-0.65f, -0.9759f),
                new Vector2(-0.60f, -0.9682f),
                new Vector2(-0.55f, -0.9594f),
                new Vector2(-0.50f, -0.9495f),
                new Vector2(-0.45f, -0.9383f),
                new Vector2(-0.40f, -0.9259f),
                new Vector2(-0.35f, -0.9122f),
                new Vector2(-0.30f, -0.8971f),
                new Vector2(-0.25f, -0.8807f),
                new Vector2(-0.20f, -0.8627f),
                new Vector2(-0.15f, -0.8433f),
                new Vector2(-0.10f, -0.8222f),
                new Vector2(-0.05f, -0.7994f),
                new Vector2(0.00f, -0.7749f),
                new Vector2(0.05f, -0.7484f),
                new Vector2(0.10f, -0.7199f),
                new Vector2(0.15f, -0.6893f),
                new Vector2(0.20f, -0.6563f),
                new Vector2(0.25f, -0.6208f),
                new Vector2(0.30f, -0.5826f),
                new Vector2(0.35f, -0.5415f),
                new Vector2(0.40f, -0.4971f),
                new Vector2(0.4495f, -0.4495f),//точка симметрии

                new Vector2(0.4971f, -0.40f),
                new Vector2(0.5415f, -0.35f),
                new Vector2(0.5826f, -0.30f),
                new Vector2(0.6208f, -0.25f),
                new Vector2(0.6563f, -0.20f),
                new Vector2(0.6893f, -0.15f),
                new Vector2(0.7199f, -0.10f),
                new Vector2(0.7484f, -0.05f),
                new Vector2(0.7749f, 0.00f),
                new Vector2(0.7994f, 0.05f),
                new Vector2(0.8222f, 0.10f),
                new Vector2(0.8433f, 0.15f),
                new Vector2(0.8627f, 0.20f),
                new Vector2(0.8807f, 0.25f),
                new Vector2(0.8971f, 0.30f),
                new Vector2(0.9122f, 0.35f),
                new Vector2(0.9259f, 0.40f),
                new Vector2(0.9383f, 0.45f),
                new Vector2(0.9495f, 0.50f),
                new Vector2(0.9594f, 0.55f),
                new Vector2(0.9682f, 0.60f),
                new Vector2(0.9759f, 0.65f),
                new Vector2(0.9824f, 0.70f),
                new Vector2(0.9879f, 0.75f),
                new Vector2(0.9923f, 0.80f),
                new Vector2(0.9957f, 0.85f),
                new Vector2(0.9981f, 0.90f),
                new Vector2(0.9995f, 0.95f),




            };
            private static Vector2[] pointsDirectionLeft_Main =
            {
                new Vector2(0.95f, 0.9995f),
                new Vector2(0.90f, 0.9981f),
                new Vector2(0.85f, 0.9957f),
                new Vector2(0.80f, 0.9923f),
                new Vector2(0.75f, 0.9879f),
                new Vector2(0.70f, 0.9824f),
                new Vector2(0.65f, 0.9759f),
                new Vector2(0.60f, 0.9682f),
                new Vector2(0.55f, 0.9594f),
                new Vector2(0.50f, 0.9495f),
                new Vector2(0.45f, 0.9383f),
                new Vector2(0.40f, 0.9259f),
                new Vector2(0.35f, 0.9122f),
                new Vector2(0.30f, 0.8971f),
                new Vector2(0.25f, 0.8807f),
                new Vector2(0.20f, 0.8627f),
                new Vector2(0.15f, 0.8433f),
                new Vector2(0.10f, 0.8222f),
                new Vector2(0.05f, 0.7994f),
                new Vector2(0.00f, 0.7749f),
                new Vector2(-0.05f, 0.7484f),
                new Vector2(-0.10f, 0.7199f),
                new Vector2(-0.15f, 0.6893f),
                new Vector2(-0.20f, 0.6563f),
                new Vector2(-0.25f, 0.6208f),
                new Vector2(-0.30f, 0.5826f),
                new Vector2(-0.35f, 0.5415f),
                new Vector2(-0.40f, 0.4971f),
                new Vector2(-0.4495f, 0.4495f),//точка симметрии

                new Vector2(-0.4971f, 0.40f),
                new Vector2(-0.5415f, 0.35f),
                new Vector2(-0.5826f, 0.30f),
                new Vector2(-0.6208f, 0.25f),
                new Vector2(-0.6563f, 0.20f),
                new Vector2(-0.6893f, 0.15f),
                new Vector2(-0.7199f, 0.10f),
                new Vector2(-0.7484f, 0.05f),
                new Vector2(-0.7749f, 0.00f),
                new Vector2(-0.7994f, -0.05f),
                new Vector2(-0.8222f, -0.10f),
                new Vector2(-0.8433f, -0.15f),
                new Vector2(-0.8627f, -0.20f),
                new Vector2(-0.8807f, -0.25f),
                new Vector2(-0.8971f, -0.30f),
                new Vector2(-0.9122f, -0.35f),
                new Vector2(-0.9259f, -0.40f),
                new Vector2(-0.9383f, -0.45f),
                new Vector2(-0.9495f, -0.50f),
                new Vector2(-0.9594f, -0.55f),
                new Vector2(-0.9682f, -0.60f),
                new Vector2(-0.9759f, -0.65f),
                new Vector2(-0.9824f, -0.70f),
                new Vector2(-0.9879f, -0.75f),
                new Vector2(-0.9923f, -0.80f),
                new Vector2(-0.9957f, -0.85f),
                new Vector2(-0.9981f, -0.90f),
                new Vector2(-0.9995f, -0.95f),
            };
            private static Vector2[] pointsDirectionLeft_Mirror =
            {
                new Vector2(0.95f, -0.9995f),
                new Vector2(0.90f, -0.9981f),
                new Vector2(0.85f, -0.9957f),
                new Vector2(0.80f, -0.9923f),
                new Vector2(0.75f, -0.9879f),
                new Vector2(0.70f, -0.9824f),
                new Vector2(0.65f, -0.9759f),
                new Vector2(0.60f, -0.9682f),
                new Vector2(0.55f, -0.9594f),
                new Vector2(0.50f, -0.9495f),
                new Vector2(0.45f, -0.9383f),
                new Vector2(0.40f, -0.9259f),
                new Vector2(0.35f, -0.9122f),
                new Vector2(0.30f, -0.8971f),
                new Vector2(0.25f, -0.8807f),
                new Vector2(0.20f, -0.8627f),
                new Vector2(0.15f, -0.8433f),
                new Vector2(0.10f, -0.8222f),
                new Vector2(0.05f, -0.7994f),
                new Vector2(0.00f, -0.7749f),
                new Vector2(-0.05f, -0.7484f),
                new Vector2(-0.10f, -0.7199f),
                new Vector2(-0.15f, -0.6893f),
                new Vector2(-0.20f, -0.6563f),
                new Vector2(-0.25f, -0.6208f),
                new Vector2(-0.30f, -0.5826f),
                new Vector2(-0.35f, -0.5415f),
                new Vector2(-0.40f, -0.4971f),
                new Vector2(-0.4495f, -0.4495f),//точка симметрии

                new Vector2(-0.4971f, -0.40f),
                new Vector2(-0.5415f, -0.35f),
                new Vector2(-0.5826f, -0.30f),
                new Vector2(-0.6208f, -0.25f),
                new Vector2(-0.6563f, -0.20f),
                new Vector2(-0.6893f, -0.15f),
                new Vector2(-0.7199f, -0.10f),
                new Vector2(-0.7484f, -0.05f),
                new Vector2(-0.7749f, 0.00f),
                new Vector2(-0.7994f, 0.05f),
                new Vector2(-0.8222f, 0.10f),
                new Vector2(-0.8433f, 0.15f),
                new Vector2(-0.8627f, 0.20f),
                new Vector2(-0.8807f, 0.25f),
                new Vector2(-0.8971f, 0.30f),
                new Vector2(-0.9122f, 0.35f),
                new Vector2(-0.9259f, 0.40f),
                new Vector2(-0.9383f, 0.45f),
                new Vector2(-0.9495f, 0.50f),
                new Vector2(-0.9594f, 0.55f),
                new Vector2(-0.9682f, 0.60f),
                new Vector2(-0.9759f, 0.65f),
                new Vector2(-0.9824f, 0.70f),
                new Vector2(-0.9879f, 0.75f),
                new Vector2(-0.9923f, 0.80f),
                new Vector2(-0.9957f, 0.85f),
                new Vector2(-0.9981f, 0.90f),
                new Vector2(-0.9995f, 0.95f),
            };
            private static Vector2[] pointsDirectionDownRight_Main =
            {
                new Vector2(0.04817f, 0.95f),
                new Vector2(0.09284f, 0.90f),
                new Vector2(0.13424f, 0.85f),
                new Vector2(0.17256f, 0.80f),
                new Vector2(0.20794f, 0.75f),
                new Vector2(0.24054f, 0.70f),
                new Vector2(0.27046f, 0.65f),
                new Vector2(0.29783f, 0.60f),
                new Vector2(0.32270f, 0.55f),
                new Vector2(0.34520f, 0.50f),
                new Vector2(0.36540f, 0.45f),
                new Vector2(0.38330f, 0.40f),
                new Vector2(0.39900f, 0.35f),
                new Vector2(0.41250f, 0.30f),
                new Vector2(0.42380f, 0.25f),
                new Vector2(0.43310f, 0.20f),
                new Vector2(0.44030f, 0.15f),
                new Vector2(0.44540f, 0.10f),
                new Vector2(0.44850f, 0.05f),
                new Vector2(0.44950f, 0.00f), //точка симметрии

                new Vector2(0.44850f, -0.05f),
                new Vector2(0.44540f, -0.10f),
                new Vector2(0.44030f, -0.15f),
                new Vector2(0.43310f, -0.20f),
                new Vector2(0.42380f, -0.25f),
                new Vector2(0.41250f, -0.30f),
                new Vector2(0.39900f, -0.35f),
                new Vector2(0.38330f, -0.40f),
                new Vector2(0.36540f, -0.45f),
                new Vector2(0.34520f, -0.50f),
                new Vector2(0.32270f, -0.55f),
                new Vector2(0.29783f, -0.60f),
                new Vector2(0.27046f, -0.65f),
                new Vector2(0.24054f, -0.70f),
                new Vector2(0.20794f, -0.75f),
                new Vector2(0.17256f, -0.80f),
                new Vector2(0.13424f, -0.85f),
                new Vector2(0.09284f, -0.90f),
                new Vector2(0.04817f, -0.95f),
            };
            private static Vector2[] pointsDirectionDownLeft_Main =
            {
                new Vector2(-0.04817f, 0.95f),
                new Vector2(-0.09284f, 0.90f),
                new Vector2(-0.13424f, 0.85f),
                new Vector2(-0.17256f, 0.80f),
                new Vector2(-0.20794f, 0.75f),
                new Vector2(-0.24054f, 0.70f),
                new Vector2(-0.27046f, 0.65f),
                new Vector2(-0.29783f, 0.60f),
                new Vector2(-0.32270f, 0.55f),
                new Vector2(-0.34520f, 0.50f),
                new Vector2(-0.36540f, 0.45f),
                new Vector2(-0.38330f, 0.40f),
                new Vector2(-0.39900f, 0.35f),
                new Vector2(-0.41250f, 0.30f),
                new Vector2(-0.42380f, 0.25f),
                new Vector2(-0.43310f, 0.20f),
                new Vector2(-0.44030f, 0.15f),
                new Vector2(-0.44540f, 0.10f),
                new Vector2(-0.44850f, 0.05f),
                new Vector2(-0.44950f, 0.00f), //точка симметрии

                new Vector2(-0.44850f, -0.05f),
                new Vector2(-0.44540f, -0.10f),
                new Vector2(-0.44030f, -0.15f),
                new Vector2(-0.43310f, -0.20f),
                new Vector2(-0.42380f, -0.25f),
                new Vector2(-0.41250f, -0.30f),
                new Vector2(-0.39900f, -0.35f),
                new Vector2(-0.38330f, -0.40f),
                new Vector2(-0.36540f, -0.45f),
                new Vector2(-0.34520f, -0.50f),
                new Vector2(-0.32270f, -0.55f),
                new Vector2(-0.29783f, -0.60f),
                new Vector2(-0.27046f, -0.65f),
                new Vector2(-0.24054f, -0.70f),
                new Vector2(-0.20794f, -0.75f),
                new Vector2(-0.17256f, -0.80f),
                new Vector2(-0.13424f, -0.85f),
                new Vector2(-0.09284f, -0.90f),
                new Vector2(-0.04817f, -0.95f),
            };
            private static Vector2[] pointsDirectionUpLeft_Main =
            {
                new Vector2(0.95f, 0.04817f),
                new Vector2(0.90f, 0.09284f),
                new Vector2(0.85f, 0.13424f),
                new Vector2(0.80f, 0.17256f),
                new Vector2(0.75f, 0.20794f),
                new Vector2(0.70f, 0.24054f),
                new Vector2(0.65f, 0.27046f),
                new Vector2(0.60f, 0.29783f),
                new Vector2(0.55f, 0.32270f),
                new Vector2(0.50f, 0.34520f),
                new Vector2(0.45f, 0.36540f),
                new Vector2(0.40f, 0.38330f),
                new Vector2(0.35f, 0.39900f),
                new Vector2(0.30f, 0.41250f),
                new Vector2(0.25f, 0.42380f),
                new Vector2(0.20f, 0.43310f),
                new Vector2(0.15f, 0.44030f),
                new Vector2(0.10f, 0.44540f),
                new Vector2(0.05f, 0.44850f),
                new Vector2(0.00f, 0.44950f), //точка симметрии

                new Vector2(-0.05f, 0.44850f),
                new Vector2(-0.10f, 0.44540f),
                new Vector2(-0.15f, 0.44030f),
                new Vector2(-0.20f, 0.43310f),
                new Vector2(-0.25f, 0.42380f),
                new Vector2(-0.30f, 0.41250f),
                new Vector2(-0.35f, 0.39900f),
                new Vector2(-0.40f, 0.38330f),
                new Vector2(-0.45f, 0.36540f),
                new Vector2(-0.50f, 0.34520f),
                new Vector2(-0.55f, 0.32270f),
                new Vector2(-0.60f, 0.29783f),
                new Vector2(-0.65f, 0.27046f),
                new Vector2(-0.70f, 0.24054f),
                new Vector2(-0.75f, 0.20794f),
                new Vector2(-0.80f, 0.17256f),
                new Vector2(-0.85f, 0.13424f),
                new Vector2(-0.90f, 0.09284f),
                new Vector2(-0.95f, 0.04817f),
            };
            private static Vector2[] pointsDirectionDownLeft_Mirror =
            {
                new Vector2(0.95f, -0.04817f),
                new Vector2(0.90f, -0.09284f),
                new Vector2(0.85f, -0.13424f),
                new Vector2(0.80f, -0.17256f),
                new Vector2(0.75f, -0.20794f),
                new Vector2(0.70f, -0.24054f),
                new Vector2(0.65f, -0.27046f),
                new Vector2(0.60f, -0.29783f),
                new Vector2(0.55f, -0.32270f),
                new Vector2(0.50f, -0.34520f),
                new Vector2(0.45f, -0.36540f),
                new Vector2(0.40f, -0.38330f),
                new Vector2(0.35f, -0.39900f),
                new Vector2(0.30f, -0.41250f),
                new Vector2(0.25f, -0.42380f),
                new Vector2(0.20f, -0.43310f),
                new Vector2(0.15f, -0.44030f),
                new Vector2(0.10f, -0.44540f),
                new Vector2(0.05f, -0.44850f),
                new Vector2(0.00f, -0.44950f), //точка симметрии

                new Vector2(-0.05f, -0.44850f),
                new Vector2(-0.10f, -0.44540f),
                new Vector2(-0.15f, -0.44030f),
                new Vector2(-0.20f, -0.43310f),
                new Vector2(-0.25f, -0.42380f),
                new Vector2(-0.30f, -0.41250f),
                new Vector2(-0.35f, -0.39900f),
                new Vector2(-0.40f, -0.38330f),
                new Vector2(-0.45f, -0.36540f),
                new Vector2(-0.50f, -0.34520f),
                new Vector2(-0.55f, -0.32270f),
                new Vector2(-0.60f, -0.29783f),
                new Vector2(-0.65f, -0.27046f),
                new Vector2(-0.70f, -0.24054f),
                new Vector2(-0.75f, -0.20794f),
                new Vector2(-0.80f, -0.17256f),
                new Vector2(-0.85f, -0.13424f),
                new Vector2(-0.90f, -0.09284f),
                new Vector2(-0.95f, -0.04817f),
            };
            #endregion

            public static Vector2[] GetPoints(Direction direction, InputDirectionType inputDirectionType)
            {
                return direction switch
                {
                    Direction.Right => inputDirectionType == InputDirectionType.Main ? pointsDirectionRight_Main : pointsDirectionRight_Mirror,
                    Direction.Left => inputDirectionType == InputDirectionType.Main ? pointsDirectionLeft_Main : pointsDirectionLeft_Mirror,
                    Direction.Down => inputDirectionType == InputDirectionType.Main ? ReversePoints(pointsDirectionRight_Mirror) : ReversePoints(pointsDirectionLeft_Mirror),
                    Direction.Up => inputDirectionType == InputDirectionType.Main ? ReversePoints(pointsDirectionRight_Main) : ReversePoints(pointsDirectionLeft_Main),

                    Direction.UpLeft => inputDirectionType == InputDirectionType.Main ? pointsDirectionUpLeft_Main : ReversePoints(pointsDirectionDownLeft_Main),
                    Direction.DownLeft => inputDirectionType == InputDirectionType.Main ? pointsDirectionDownLeft_Main : pointsDirectionDownLeft_Mirror,
                    Direction.UpRight => inputDirectionType == InputDirectionType.Main ? ReversePoints(pointsDirectionUpLeft_Main) : ReversePoints(pointsDirectionDownRight_Main),
                    Direction.DownRight => inputDirectionType == InputDirectionType.Main ? pointsDirectionDownRight_Main : ReversePoints(pointsDirectionDownLeft_Mirror),

                    _ => throw new System.NotImplementedException()
                };

            }
        }

        /// <summary>
        /// 135 degrees
        /// </summary>
        private static class Large
        {
            private static Vector2[] pointsDirectionRight_Main =
            {
                new Vector2(-0.8284f, 1.00f),
                new Vector2(-0.7272f, 0.997f),
                new Vector2(-0.6253f, 0.9924f),
                new Vector2(-0.5226f, 0.9859f),
                new Vector2(-0.4191f, 0.9774f),
                new Vector2(-0.3145f, 0.9664f),
                new Vector2(-0.2088f, 0.9525f),
                new Vector2(-0.1016f, 0.9351f),
                new Vector2(0.0075f, 0.9133f),
                new Vector2(0.1189f, 0.8857f),
                new Vector2(0.2336f, 0.8502f),
                new Vector2(0.3533f, 0.8027f),
                new Vector2(0.4818f, 0.7338f),
                new Vector2(0.5529f, 0.6829f),
                new Vector2(0.6003f, 0.6408f),
                new Vector2(0.6363f, 0.6023f),
                new Vector2(0.6824f, 0.5392f),

                new Vector2(0.7184f, 0.4692f), //точка симметрии

                new Vector2(0.7425f, 0.3942f),
                new Vector2(0.7545f, 0.3169f),
                new Vector2(0.7563f, 0.2643f),
                new Vector2(0.7526f, 0.2009f),
                new Vector2(0.7383f, 0.1147f),
                new Vector2(0.6961f, -0.0249f),
                new Vector2(0.6451f, -0.1431f),
                new Vector2(0.5891f, -0.2493f),
                new Vector2(0.5298f, -0.3476f),
                new Vector2(0.4681f, -0.4401f),
                new Vector2(0.4046f, -0.5283f),
                new Vector2(0.3396f, -0.6129f),
                new Vector2(0.2735f, -0.6946f),
                new Vector2(0.2063f, -0.7738f),
                new Vector2(0.1383f, -0.8510f),
                new Vector2(0.0695f, -0.9263f),
            };
            private static Vector2[] pointsDirectionRight_Mirror =
            {
                new Vector2(-0.8284f, -1.00f),
                new Vector2(-0.7272f, -0.997f),
                new Vector2(-0.6253f, -0.9924f),
                new Vector2(-0.5226f, -0.9859f),
                new Vector2(-0.4191f, -0.9774f),
                new Vector2(-0.3145f, -0.9664f),
                new Vector2(-0.2088f, -0.9525f),
                new Vector2(-0.1016f, -0.9351f),
                new Vector2(0.0075f, -0.9133f),
                new Vector2(0.1189f, -0.8857f),
                new Vector2(0.2336f, -0.8502f),
                new Vector2(0.3533f, -0.8027f),
                new Vector2(0.4818f, -0.7338f),
                new Vector2(0.5529f, -0.6829f),
                new Vector2(0.6003f, -0.6408f),
                new Vector2(0.6363f, -0.6023f),
                new Vector2(0.6824f, -0.5392f),

                new Vector2(0.7184f, -0.4692f), //точка симметрии

                new Vector2(0.7425f, -0.3942f),
                new Vector2(0.7545f, -0.3169f),
                new Vector2(0.7563f, -0.2643f),
                new Vector2(0.7526f, -0.2009f),
                new Vector2(0.7383f, -0.1147f),
                new Vector2(0.6961f, 0.0249f),
                new Vector2(0.6451f, 0.1431f),
                new Vector2(0.5891f, 0.2493f),
                new Vector2(0.5298f, 0.3476f),
                new Vector2(0.4681f, 0.4401f),
                new Vector2(0.4046f, 0.5283f),
                new Vector2(0.3396f, 0.6129f),
                new Vector2(0.2735f, 0.6946f),
                new Vector2(0.2063f, 0.7738f),
                new Vector2(0.1383f, 0.8510f),
                new Vector2(0.0695f, 0.9263f),
            };
            private static Vector2[] pointsDirectionLeft_Main =
            {
                new Vector2(0.8284f, 1.00f),
                new Vector2(0.7272f, 0.997f),
                new Vector2(0.6253f, 0.9924f),
                new Vector2(0.5226f, 0.9859f),
                new Vector2(0.4191f, 0.9774f),
                new Vector2(0.3145f, 0.9664f),
                new Vector2(0.2088f, 0.9525f),
                new Vector2(0.1016f, 0.9351f),
                new Vector2(-0.0075f, 0.9133f),
                new Vector2(-0.1189f, 0.8857f),
                new Vector2(-0.2336f, 0.8502f),
                new Vector2(-0.3533f, 0.8027f),
                new Vector2(-0.4818f, 0.7338f),
                new Vector2(-0.5529f, 0.6829f),
                new Vector2(-0.6003f, 0.6408f),
                new Vector2(-0.6363f, 0.6023f),
                new Vector2(-0.6824f, 0.5392f),

                new Vector2(-0.7184f, 0.4692f), //точка симметрии

                new Vector2(-0.7425f, 0.3942f),
                new Vector2(-0.7545f, 0.3169f),
                new Vector2(-0.7563f, 0.2643f),
                new Vector2(-0.7526f, 0.2009f),
                new Vector2(-0.7383f, 0.1147f),
                new Vector2(-0.6961f, -0.0249f),
                new Vector2(-0.6451f, -0.1431f),
                new Vector2(-0.5891f, -0.2493f),
                new Vector2(-0.5298f, -0.3476f),
                new Vector2(-0.4681f, -0.4401f),
                new Vector2(-0.4046f, -0.5283f),
                new Vector2(-0.3396f, -0.6129f),
                new Vector2(-0.2735f, -0.6946f),
                new Vector2(-0.2063f, -0.7738f),
                new Vector2(-0.1383f, -0.8510f),
                new Vector2(-0.0695f, -0.9263f),
            };
            private static Vector2[] pointsDirectionLeft_Mirror =
            {
               new Vector2(0.8284f, -1.00f),
                new Vector2(0.7272f, -0.997f),
                new Vector2(0.6253f, -0.9924f),
                new Vector2(0.5226f, -0.9859f),
                new Vector2(0.4191f, -0.9774f),
                new Vector2(0.3145f, -0.9664f),
                new Vector2(0.2088f, -0.9525f),
                new Vector2(0.1016f, -0.9351f),
                new Vector2(-0.0075f, -0.9133f),
                new Vector2(-0.1189f, -0.8857f),
                new Vector2(-0.2336f, -0.8502f),
                new Vector2(-0.3533f, -0.8027f),
                new Vector2(-0.4818f, -0.7338f),
                new Vector2(-0.5529f, -0.6829f),
                new Vector2(-0.6003f, -0.6408f),
                new Vector2(-0.6363f, -0.6023f),
                new Vector2(-0.6824f, -0.5392f),

                new Vector2(-0.7184f, -0.4692f), //точка симметрии

                new Vector2(-0.7425f, -0.3942f),
                new Vector2(-0.7545f, -0.3169f),
                new Vector2(-0.7563f, -0.2643f),
                new Vector2(-0.7526f, -0.2009f),
                new Vector2(-0.7383f, -0.1147f),
                new Vector2(-0.6961f, 0.0249f),
                new Vector2(-0.6451f, 0.1431f),
                new Vector2(-0.5891f, 0.2493f),
                new Vector2(-0.5298f, 0.3476f),
                new Vector2(-0.4681f, 0.4401f),
                new Vector2(-0.4046f, 0.5283f),
                new Vector2(-0.3396f, 0.6129f),
                new Vector2(-0.2735f, 0.6946f),
                new Vector2(-0.2063f, 0.7738f),
                new Vector2(-0.1383f, 0.8510f),
                new Vector2(-0.0695f, 0.9263f),
                //new Vector2(f, 0.f),
            };
            private static Vector2[] pointsDirectionDown_Main =
            {
                new Vector2(1.00f, 0.8284f),
                new Vector2(0.997f, 0.7272f),
                new Vector2(0.9924f, 0.6253f),
                new Vector2(0.9859f, 0.5226f),
                new Vector2(0.9774f, 0.4191f),
                new Vector2(0.9664f, 0.3145f),
                new Vector2(0.9525f, 0.2088f),
                new Vector2(0.9351f, 0.1016f),
                new Vector2(0.9133f, -0.0075f),
                new Vector2(0.8857f, -0.1189f),
                new Vector2(0.8502f, -0.2336f),
                new Vector2(0.8027f, -0.3533f),
                new Vector2(0.7338f, -0.4818f),
                new Vector2(0.6829f, -0.5529f),
                new Vector2(0.6408f, -0.6003f),
                new Vector2(0.6023f, -0.6363f),
                new Vector2(0.5392f, -0.6824f),

                new Vector2(0.4692f, -0.7184f), //точка симметрии

                new Vector2(0.3942f, -0.7425f),
                new Vector2(0.3169f, -0.7545f),
                new Vector2(0.2643f, -0.7563f),
                new Vector2(0.2009f, -0.7526f),
                new Vector2(0.1147f, -0.7383f),
                new Vector2(-0.0249f, -0.6961f),
                new Vector2(-0.1431f, -0.6451f),
                new Vector2(-0.2493f, -0.5891f),
                new Vector2(-0.3476f, -0.5298f),
                new Vector2(-0.4401f, -0.4681f),
                new Vector2(-0.5283f, -0.4046f),
                new Vector2(-0.6129f, -0.3396f),
                new Vector2(-0.6946f, -0.2735f),
                new Vector2(-0.7738f, -0.2063f),
                new Vector2(-0.851f, -0.1383f),
                new Vector2(-0.9263f, -0.0695f),

            };
            private static Vector2[] pointsDirectionDown_Mirror =
            {
                new Vector2(-1.00f, 0.8284f),
                new Vector2(-0.997f, 0.7272f),
                new Vector2(-0.9924f, 0.6253f),
                new Vector2(-0.9859f, 0.5226f),
                new Vector2(-0.9774f, 0.4191f),
                new Vector2(-0.9664f, 0.3145f),
                new Vector2(-0.9525f, 0.2088f),
                new Vector2(-0.9351f, 0.1016f),
                new Vector2(-0.9133f, -0.0075f),
                new Vector2(-0.8857f, -0.1189f),
                new Vector2(-0.8502f, -0.2336f),
                new Vector2(-0.8027f, -0.3533f),
                new Vector2(-0.7338f, -0.4818f),
                new Vector2(-0.6829f, -0.5529f),
                new Vector2(-0.6408f, -0.6003f),
                new Vector2(-0.6023f, -0.6363f),
                new Vector2(-0.5392f, -0.6824f),

                new Vector2(-0.4692f, -0.7184f), //точка симметрии

                new Vector2(-0.3942f, -0.7425f),
                new Vector2(-0.3169f, -0.7545f),
                new Vector2(-0.2643f, -0.7563f),
                new Vector2(-0.2009f, -0.7526f),
                new Vector2(-0.1147f, -0.7383f),
                new Vector2(0.0249f, -0.6961f),
                new Vector2(0.1431f, -0.6451f),
                new Vector2(0.2493f, -0.5891f),
                new Vector2(0.3476f, -0.5298f),
                new Vector2(0.4401f, -0.4681f),
                new Vector2(0.5283f, -0.4046f),
                new Vector2(0.6129f, -0.3396f),
                new Vector2(0.6946f, -0.2735f),
                new Vector2(0.7738f, -0.2063f),
                new Vector2(0.851f, -0.1383f),
                new Vector2(0.9263f, -0.0695f),
            };
            private static Vector2[] pointsDirectionUp_Main =
            {
                new Vector2(1.00f, -0.8284f),
                new Vector2(0.997f, -0.7272f),
                new Vector2(0.9924f, -0.6253f),
                new Vector2(0.9859f, -0.5226f),
                new Vector2(0.9774f, -0.4191f),
                new Vector2(0.9664f, -0.3145f),
                new Vector2(0.9525f, -0.2088f),
                new Vector2(0.9351f, -0.1016f),
                new Vector2(0.9133f, 0.0075f),
                new Vector2(0.8857f, 0.1189f),
                new Vector2(0.8502f, 0.2336f),
                new Vector2(0.8027f, 0.3533f),
                new Vector2(0.7338f, 0.4818f),
                new Vector2(0.6829f, 0.5529f),
                new Vector2(0.6408f, 0.6003f),
                new Vector2(0.6023f, 0.6363f),
                new Vector2(0.5392f, 0.6824f),

                new Vector2(0.4692f, 0.7184f), //точка симметрии

                new Vector2(0.3942f, 0.7425f),
                new Vector2(0.3169f, 0.7545f),
                new Vector2(0.2643f, 0.7563f),
                new Vector2(0.2009f, 0.7526f),
                new Vector2(0.1147f, 0.7383f),
                new Vector2(-0.0249f, 0.6961f),
                new Vector2(-0.1431f, 0.6451f),
                new Vector2(-0.2493f, 0.5891f),
                new Vector2(-0.3476f, 0.5298f),
                new Vector2(-0.4401f, 0.4681f),
                new Vector2(-0.5283f, 0.4046f),
                new Vector2(-0.6129f, 0.3396f),
                new Vector2(-0.6946f, 0.2735f),
                new Vector2(-0.7738f, 0.2063f),
                new Vector2(-0.851f, 0.1383f),
                new Vector2(-0.9263f, 0.0695f),
            };
            private static Vector2[] pointsDirectionUp_Mirror =
            {
                new Vector2(-1.00f, -0.8284f),
                new Vector2(-0.997f, -0.7272f),
                new Vector2(-0.9924f, -0.6253f),
                new Vector2(-0.9859f, -0.5226f),
                new Vector2(-0.9774f, -0.4191f),
                new Vector2(-0.9664f, -0.3145f),
                new Vector2(-0.9525f, -0.2088f),
                new Vector2(-0.9351f, -0.1016f),
                new Vector2(-0.9133f, 0.0075f),
                new Vector2(-0.8857f, 0.1189f),
                new Vector2(-0.8502f, 0.2336f),
                new Vector2(-0.8027f, 0.3533f),
                new Vector2(-0.7338f, 0.4818f),
                new Vector2(-0.6829f, 0.5529f),
                new Vector2(-0.6408f, 0.6003f),
                new Vector2(-0.6023f, 0.6363f),
                new Vector2(-0.5392f, 0.6824f),

                new Vector2(-0.4692f, 0.7184f), //точка симметрии

                new Vector2(-0.3942f, 0.7425f),
                new Vector2(-0.3169f, 0.7545f),
                new Vector2(-0.2643f, 0.7563f),
                new Vector2(-0.2009f, 0.7526f),
                new Vector2(-0.1147f, 0.7383f),
                new Vector2(0.0249f, 0.6961f),
                new Vector2(0.1431f, 0.6451f),
                new Vector2(0.2493f, 0.5891f),
                new Vector2(0.3476f, 0.5298f),
                new Vector2(0.4401f, 0.4681f),
                new Vector2(0.5283f, 0.4046f),
                new Vector2(0.6129f, 0.3396f),
                new Vector2(0.6946f, 0.2735f),
                new Vector2(0.7738f, 0.2063f),
                new Vector2(0.851f, 0.1383f),
                new Vector2(0.9263f, 0.0695f),
            };


            public static Vector2[] GetPoints(Direction direction, InputDirectionType inputDirectionType)
            {
                return direction switch
                {
                    Direction.Right => inputDirectionType == InputDirectionType.Main ? pointsDirectionRight_Main : pointsDirectionRight_Mirror,
                    Direction.Left => inputDirectionType == InputDirectionType.Main ? pointsDirectionLeft_Main : pointsDirectionLeft_Mirror,
                    Direction.Down => inputDirectionType == InputDirectionType.Main ? pointsDirectionDown_Main : pointsDirectionDown_Mirror,
                    Direction.Up => inputDirectionType == InputDirectionType.Main ? pointsDirectionUp_Main : pointsDirectionUp_Mirror,

                    Direction.UpLeft => inputDirectionType == InputDirectionType.Main ? ReversePoints(pointsDirectionUp_Mirror) : ReversePoints(pointsDirectionLeft_Main),
                    Direction.DownLeft => inputDirectionType == InputDirectionType.Main ? ReversePoints(pointsDirectionLeft_Mirror) : ReversePoints(pointsDirectionDown_Mirror),
                    Direction.UpRight => inputDirectionType == InputDirectionType.Main ? ReversePoints(pointsDirectionUp_Main) : ReversePoints(pointsDirectionRight_Main),
                    Direction.DownRight => inputDirectionType == InputDirectionType.Main ? ReversePoints(pointsDirectionRight_Mirror) : ReversePoints(pointsDirectionDown_Main),

                    _ => throw new System.NotImplementedException()
                };

            }
        }

        /// <summary>
        /// 180 degrees
        /// </summary>
        private static class Supermassive
        {
            private static Vector2[] pointsDirectionRight_Main =
            {
                #region вытянутая парабола
                //new Vector2(-0.7303f, 0.995f),
                //new Vector2(-0.6191f, 0.99f),
                //new Vector2(-0.4627f, 0.98f),
                //new Vector2(-0.1569f, 0.95f),
                //new Vector2(0.0582f, 0.92f),
                //new Vector2(0.2311f, 0.89f),
                //new Vector2(0.3778f, 0.86f),
                //new Vector2(0.5060f, 0.83f),
                //new Vector2(0.6200f, 0.80f),
                //new Vector2(0.7227f, 0.77f),
                //new Vector2(0.8160f, 0.74f),
                //new Vector2(0.9013f, 0.71f),
                //new Vector2(0.9797f, 0.68f),
                //new Vector2(1.0518f, 0.65f),
                //new Vector2(1.1184f, 0.62f),
                //new Vector2(1.1800f, 0.59f),
                //new Vector2(1.2369f, 0.56f),
                //new Vector2(1.2896f, 0.53f),
                //new Vector2(1.3383f, 0.50f),
                //new Vector2(1.3832f, 0.47f),
                //new Vector2(1.4246f, 0.44f),
                //new Vector2(1.4626f, 0.41f),
                //new Vector2(1.4975f, 0.38f),
                //new Vector2(1.5292f, 0.35f),
                //new Vector2(1.5580f, 0.32f),
                //new Vector2(1.5840f, 0.29f),
                //new Vector2(1.6071f, 0.26f),
                //new Vector2(1.6276f, 0.23f),
                //new Vector2(1.6454f, 0.20f),
                //new Vector2(1.6607f, 0.17f),
                //new Vector2(1.6734f, 0.14f),
                //new Vector2(1.6836f, 0.11f),
                //new Vector2(1.6913f, 0.08f),
                //new Vector2(1.6966f, 0.05f),
                //new Vector2(1.6995f, 0.02f),
                //new Vector2(1.7000f, 0.00f), //точка симметрии

                //new Vector2(1.6995f, -0.02f),
                //new Vector2(1.6966f, -0.05f),
                //new Vector2(1.6913f, -0.08f),
                //new Vector2(1.6836f, -0.11f),
                //new Vector2(1.6734f, -0.14f),
                //new Vector2(1.6607f, -0.17f),
                //new Vector2(1.6454f, -0.20f),
                //new Vector2(1.6276f, -0.23f),
                //new Vector2(1.6071f, -0.26f),
                //new Vector2(1.5840f, -0.29f),
                //new Vector2(1.5580f, -0.32f),
                //new Vector2(1.5292f, -0.35f),
                //new Vector2(1.4975f, -0.38f),
                //new Vector2(1.4626f, -0.41f),
                //new Vector2(1.4246f, -0.44f),
                //new Vector2(1.3832f, -0.47f),
                //new Vector2(1.3383f, -0.50f),
                //new Vector2(1.2896f, -0.53f),
                //new Vector2(1.2369f, -0.56f),
                //new Vector2(1.1800f, -0.59f),
                //new Vector2(1.1184f, -0.62f),
                //new Vector2(1.0518f, -0.65f),
                //new Vector2(0.9797f, -0.68f),
                //new Vector2(0.9013f, -0.71f),
                //new Vector2(0.8160f, -0.74f),
                //new Vector2(0.7227f, -0.77f),
                //new Vector2(0.6200f, -0.80f),
                //new Vector2(0.5060f, -0.83f),
                //new Vector2(0.3778f, -0.86f),
                //new Vector2(0.2311f, -0.89f),
                //new Vector2(0.0582f, -0.92f),
                //new Vector2(-0.1569f, -0.95f),
                //new Vector2(-0.4627f, -0.98f),
                //new Vector2(-0.6191f, -0.99f),
                //new Vector2(-0.7303f, -0.995f),

                #endregion

#region круглая
                new Vector2(0.04471f, 0.999f),
                new Vector2(0.07740f, 0.997f),
                new Vector2(0.09987f, 0.995f),
                new Vector2(0.14107f, 0.99f),
                new Vector2(0.19900f, 0.98f),
                new Vector2(0.31225f, 0.95f),
                new Vector2(0.3919f, 0.92f),
                new Vector2(0.4560f, 0.89f),
                new Vector2(0.5103f, 0.86f),
                new Vector2(0.5578f, 0.83f),
                new Vector2(0.6000f, 0.80f),
                new Vector2(0.6380f, 0.77f),
                new Vector2(0.6726f, 0.74f),
                new Vector2(0.7042f, 0.71f),
                new Vector2(0.7332f, 0.68f),
                new Vector2(0.7599f, 0.65f),
                new Vector2(0.7846f, 0.62f),
                new Vector2(0.8074f, 0.59f),
                new Vector2(0.8285f, 0.56f),
                new Vector2(0.8480f, 0.53f),
                new Vector2(0.8660f, 0.50f),
                new Vector2(0.8827f, 0.47f),
                new Vector2(0.8980f, 0.44f),
                new Vector2(0.9121f, 0.41f),
                new Vector2(0.9250f, 0.38f),
                new Vector2(0.9367f, 0.35f),
                new Vector2(0.9474f, 0.32f),
                new Vector2(0.9570f, 0.29f),
                new Vector2(0.9656f, 0.26f),
                new Vector2(0.9732f, 0.23f),
                new Vector2(0.9798f, 0.20f),
                new Vector2(0.9854f, 0.17f),
                new Vector2(0.9902f, 0.14f),
                new Vector2(0.9939f, 0.11f),
                new Vector2(0.9968f, 0.08f),
                new Vector2(0.9987f, 0.05f),
                new Vector2(0.9998f, 0.02f),
                new Vector2(1.0000f, 0.00f), //точка симметрии

                new Vector2(0.9998f, -0.02f),
                new Vector2(0.9987f, -0.05f),
                new Vector2(0.9968f, -0.08f),
                new Vector2(0.9939f, -0.11f),
                new Vector2(0.9902f, -0.14f),
                new Vector2(0.9854f, -0.17f),
                new Vector2(0.9798f, -0.20f),
                new Vector2(0.9732f, -0.23f),
                new Vector2(0.9656f, -0.26f),
                new Vector2(0.9570f, -0.29f),
                new Vector2(0.9474f, -0.32f),
                new Vector2(0.9367f, -0.35f),
                new Vector2(0.9250f, -0.38f),
                new Vector2(0.9121f, -0.41f),
                new Vector2(0.8980f, -0.44f),
                new Vector2(0.8827f, -0.47f),
                new Vector2(0.8660f, -0.50f),
                new Vector2(0.8480f, -0.53f),
                new Vector2(0.8285f, -0.56f),
                new Vector2(0.8074f, -0.59f),
                new Vector2(0.7846f, -0.62f),
                new Vector2(0.7599f, -0.65f),
                new Vector2(0.7332f, -0.68f),
                new Vector2(0.7042f, -0.71f),
                new Vector2(0.6726f, -0.74f),
                new Vector2(0.6380f, -0.77f),
                new Vector2(0.6000f, -0.80f),
                new Vector2(0.5578f, -0.83f),
                new Vector2(0.5103f, -0.86f),
                new Vector2(0.4560f, -0.89f),
                new Vector2(0.3919f, -0.92f),
                new Vector2(0.31225f, -0.95f),
                new Vector2(0.19900f, -0.98f),
                new Vector2(0.14107f, -0.99f),
                new Vector2(0.09987f, -0.995f),
                new Vector2(0.07740f, -0.997f),
                new Vector2(0.04471f, -0.999f),
#endregion
            };
            private static Vector2[] pointsDirectionLeft_Main =
            {
                #region вытянутая парабола
                //new Vector2(0.7303f, 0.995f),
                //new Vector2(0.6191f, 0.99f),
                //new Vector2(0.4627f, 0.98f),
                //new Vector2(0.1569f, 0.95f),
                //new Vector2(-0.0582f, 0.92f),
                //new Vector2(-0.2311f, 0.89f),
                //new Vector2(-0.3778f, 0.86f),
                //new Vector2(-0.5060f, 0.83f),
                //new Vector2(-0.6200f, 0.80f),
                //new Vector2(-0.7227f, 0.77f),
                //new Vector2(-0.8160f, 0.74f),
                //new Vector2(-0.9013f, 0.71f),
                //new Vector2(-0.9797f, 0.68f),
                //new Vector2(-1.0518f, 0.65f),
                //new Vector2(-1.1184f, 0.62f),
                //new Vector2(-1.1800f, 0.59f),
                //new Vector2(-1.2369f, 0.56f),
                //new Vector2(-1.2896f, 0.53f),
                //new Vector2(-1.3383f, 0.50f),
                //new Vector2(-1.3832f, 0.47f),
                //new Vector2(-1.4246f, 0.44f),
                //new Vector2(-1.4626f, 0.41f),
                //new Vector2(-1.4975f, 0.38f),
                //new Vector2(-1.5292f, 0.35f),
                //new Vector2(-1.5580f, 0.32f),
                //new Vector2(-1.5840f, 0.29f),
                //new Vector2(-1.6071f, 0.26f),
                //new Vector2(-1.6276f, 0.23f),
                //new Vector2(-1.6454f, 0.20f),
                //new Vector2(-1.6607f, 0.17f),
                //new Vector2(-1.6734f, 0.14f),
                //new Vector2(-1.6836f, 0.11f),
                //new Vector2(-1.6913f, 0.08f),
                //new Vector2(-1.6966f, 0.05f),
                //new Vector2(-1.6995f, 0.02f),
                //new Vector2(-1.7000f, 0.00f), //точка симметрии

                //new Vector2(-1.6995f, -0.02f),
                //new Vector2(-1.6966f, -0.05f),
                //new Vector2(-1.6913f, -0.08f),
                //new Vector2(-1.6836f, -0.11f),
                //new Vector2(-1.6734f, -0.14f),
                //new Vector2(-1.6607f, -0.17f),
                //new Vector2(-1.6454f, -0.20f),
                //new Vector2(-1.6276f, -0.23f),
                //new Vector2(-1.6071f, -0.26f),
                //new Vector2(-1.5840f, -0.29f),
                //new Vector2(-1.5580f, -0.32f),
                //new Vector2(-1.5292f, -0.35f),
                //new Vector2(-1.4975f, -0.38f),
                //new Vector2(-1.4626f, -0.41f),
                //new Vector2(-1.4246f, -0.44f),
                //new Vector2(-1.3832f, -0.47f),
                //new Vector2(-1.3383f, -0.50f),
                //new Vector2(-1.2896f, -0.53f),
                //new Vector2(-1.2369f, -0.56f),
                //new Vector2(-1.1800f, -0.59f),
                //new Vector2(-1.1184f, -0.62f),
                //new Vector2(-1.0518f, -0.65f),
                //new Vector2(-0.9797f, -0.68f),
                //new Vector2(-0.9013f, -0.71f),
                //new Vector2(-0.8160f, -0.74f),
                //new Vector2(-0.7227f, -0.77f),
                //new Vector2(-0.6200f, -0.80f),
                //new Vector2(-0.5060f, -0.83f),
                //new Vector2(-0.3778f, -0.86f),
                //new Vector2(-0.2311f, -0.89f),
                //new Vector2(-0.0582f, -0.92f),
                //new Vector2(0.1569f, -0.95f),
                //new Vector2(0.4627f, -0.98f),
                //new Vector2(0.6191f, -0.99f),
                //new Vector2(0.7303f, -0.995f),
                #endregion

                #region круглая
                new Vector2(-0.04471f, 0.999f),
                new Vector2(-0.07740f, 0.997f),
                new Vector2(-0.09987f, 0.995f),
                new Vector2(-0.14107f, 0.99f),
                new Vector2(-0.19900f, 0.98f),
                new Vector2(-0.31225f, 0.95f),
                new Vector2(-0.3919f, 0.92f),
                new Vector2(-0.4560f, 0.89f),
                new Vector2(-0.5103f, 0.86f),
                new Vector2(-0.5578f, 0.83f),
                new Vector2(-0.6000f, 0.80f),
                new Vector2(-0.6380f, 0.77f),
                new Vector2(-0.6726f, 0.74f),
                new Vector2(-0.7042f, 0.71f),
                new Vector2(-0.7332f, 0.68f),
                new Vector2(-0.7599f, 0.65f),
                new Vector2(-0.7846f, 0.62f),
                new Vector2(-0.8074f, 0.59f),
                new Vector2(-0.8285f, 0.56f),
                new Vector2(-0.8480f, 0.53f),
                new Vector2(-0.8660f, 0.50f),
                new Vector2(-0.8827f, 0.47f),
                new Vector2(-0.8980f, 0.44f),
                new Vector2(-0.9121f, 0.41f),
                new Vector2(-0.9250f, 0.38f),
                new Vector2(-0.9367f, 0.35f),
                new Vector2(-0.9474f, 0.32f),
                new Vector2(-0.9570f, 0.29f),
                new Vector2(-0.9656f, 0.26f),
                new Vector2(-0.9732f, 0.23f),
                new Vector2(-0.9798f, 0.20f),
                new Vector2(-0.9854f, 0.17f),
                new Vector2(-0.9902f, 0.14f),
                new Vector2(-0.9939f, 0.11f),
                new Vector2(-0.9968f, 0.08f),
                new Vector2(-0.9987f, 0.05f),
                new Vector2(-0.9998f, 0.02f),
                new Vector2(-1.0000f, 0.00f), //точка симметрии

                new Vector2(-0.9998f, -0.02f),
                new Vector2(-0.9987f, -0.05f),
                new Vector2(-0.9968f, -0.08f),
                new Vector2(-0.9939f, -0.11f),
                new Vector2(-0.9902f, -0.14f),
                new Vector2(-0.9854f, -0.17f),
                new Vector2(-0.9798f, -0.20f),
                new Vector2(-0.9732f, -0.23f),
                new Vector2(-0.9656f, -0.26f),
                new Vector2(-0.9570f, -0.29f),
                new Vector2(-0.9474f, -0.32f),
                new Vector2(-0.9367f, -0.35f),
                new Vector2(-0.9250f, -0.38f),
                new Vector2(-0.9121f, -0.41f),
                new Vector2(-0.8980f, -0.44f),
                new Vector2(-0.8827f, -0.47f),
                new Vector2(-0.8660f, -0.50f),
                new Vector2(-0.8480f, -0.53f),
                new Vector2(-0.8285f, -0.56f),
                new Vector2(-0.8074f, -0.59f),
                new Vector2(-0.7846f, -0.62f),
                new Vector2(-0.7599f, -0.65f),
                new Vector2(-0.7332f, -0.68f),
                new Vector2(-0.7042f, -0.71f),
                new Vector2(-0.6726f, -0.74f),
                new Vector2(-0.6380f, -0.77f),
                new Vector2(-0.6000f, -0.80f),
                new Vector2(-0.5578f, -0.83f),
                new Vector2(-0.5103f, -0.86f),
                new Vector2(-0.4560f, -0.89f),
                new Vector2(-0.3919f, -0.92f),
                new Vector2(-0.31225f, -0.95f),
                new Vector2(-0.19900f, -0.98f),
                new Vector2(-0.14107f, -0.99f),
                new Vector2(-0.09987f, -0.995f),
                new Vector2(-0.07740f, -0.997f),
                new Vector2(-0.04471f, -0.999f),
#endregion


            };
            private static Vector2[] pointsDirectionDown_Main =
            {
  #region вытянутая парабола
                
                //new Vector2(0.995f, 0.7303f),
                //new Vector2(0.99f, 0.6191f),
                //new Vector2(0.98f, 0.4627f),
                //new Vector2(0.95f, 0.1569f),
                //new Vector2(0.92f, -0.0582f),
                //new Vector2(0.89f, -0.2311f),
                //new Vector2(0.86f, -0.3778f),
                //new Vector2(0.83f, -0.5060f),
                //new Vector2(0.80f, -0.6200f),
                //new Vector2(0.77f, -0.7227f),
                //new Vector2(0.74f, -0.8160f),
                //new Vector2(0.71f, -0.9013f),
                //new Vector2(0.68f, -0.9797f),
                //new Vector2(0.65f, -1.0518f),
                //new Vector2(0.62f, -1.1184f),
                //new Vector2(0.59f, -1.1800f),
                //new Vector2(0.56f, -1.2369f),
                //new Vector2(0.53f, -1.2896f),
                //new Vector2(0.50f, -1.3383f),
                //new Vector2(0.47f, -1.3832f),
                //new Vector2(0.44f, -1.4246f),
                //new Vector2(0.41f, -1.4626f),
                //new Vector2(0.38f, -1.4975f),
                //new Vector2(0.35f, -1.5292f),
                //new Vector2(0.32f, -1.5580f),
                //new Vector2(0.29f, -1.5840f),
                //new Vector2(0.26f, -1.6071f),
                //new Vector2(0.23f, -1.6276f),
                //new Vector2(0.20f, -1.6454f),
                //new Vector2(0.17f, -1.6607f),
                //new Vector2(0.14f, -1.6734f),
                //new Vector2(0.11f, -1.6836f),
                //new Vector2(0.08f, -1.6913f),
                //new Vector2(0.05f, -1.6966f),
                //new Vector2(0.02f, -1.6995f),
                //new Vector2(0.00f, -1.7000f), //точка симметрии

                //new Vector2(-0.02f, -1.6995f),
                //new Vector2(-0.05f, -1.6966f),
                //new Vector2(-0.08f, -1.6913f),
                //new Vector2(-0.11f, -1.6836f),
                //new Vector2(-0.14f, -1.6734f),
                //new Vector2(-0.17f, -1.6607f),
                //new Vector2(-0.20f, -1.6454f),
                //new Vector2(-0.23f, -1.6276f),
                //new Vector2(-0.26f, -1.6071f),
                //new Vector2(-0.29f, -1.5840f),
                //new Vector2(-0.32f, -1.5580f),
                //new Vector2(-0.35f, -1.5292f),
                //new Vector2(-0.38f, -1.4975f),
                //new Vector2(-0.41f, -1.4626f),
                //new Vector2(-0.44f, -1.4246f),
                //new Vector2(-0.47f, -1.3832f),
                //new Vector2(-0.50f, -1.3383f),
                //new Vector2(-0.53f, -1.2896f),
                //new Vector2(-0.56f, -1.2369f),
                //new Vector2(-0.59f, -1.1800f),
                //new Vector2(-0.62f, -1.1184f),
                //new Vector2(-0.65f, -1.0518f),
                //new Vector2(-0.68f, -0.9797f),
                //new Vector2(-0.71f, -0.9013f),
                //new Vector2(-0.74f, -0.8160f),
                //new Vector2(-0.77f, -0.7227f),
                //new Vector2(-0.80f, -0.6200f),
                //new Vector2(-0.83f, -0.5060f),
                //new Vector2(-0.86f, -0.3778f),
                //new Vector2(-0.89f, -0.2311f),
                //new Vector2(-0.92f, -0.0582f),
                //new Vector2(-0.95f, 0.1569f),
                //new Vector2(-0.98f, 0.4627f),
                //new Vector2(-0.99f, 0.6191f),
                //new Vector2(-0.995f, 0.7303f),
#endregion


                #region hyperbola
                //new Vector2(0.99f, 0.7323f),
                //new Vector2(0.98f, 0.5077f),
                //new Vector2(0.95f, 0.00895f),
                //new Vector2(0.92f, -0.3268f),
                //new Vector2(0.89f, -0.5680f),
                //new Vector2(0.86f, -0.7493f),
                //new Vector2(0.83f, -0.8904f),
                //new Vector2(0.80f, -1.0031f),
                //new Vector2(0.77f, -1.0951f),
                //new Vector2(0.74f, -1.1714f),
                //new Vector2(0.71f, -1.2355f),
                //new Vector2(0.68f, -1.2901f),
                //new Vector2(0.65f, -1.3370f),
                //new Vector2(0.62f, -1.3776f),
                //new Vector2(0.59f, -1.4130f),
                //new Vector2(0.56f, -1.4440f),
                //new Vector2(0.53f, -1.4712f),
                //new Vector2(0.50f, -1.4953f),
                //new Vector2(0.47f, -1.5166f),
                //new Vector2(0.44f, -1.5355f),
                //new Vector2(0.41f, -1.5523f),
                //new Vector2(0.38f, -1.5672f),
                //new Vector2(0.35f, -1.5803f),
                //new Vector2(0.32f, -1.5920f),
                //new Vector2(0.29f, -1.6022f),
                //new Vector2(0.26f, -1.6112f),
                //new Vector2(0.23f, -1.6189f),
                //new Vector2(0.20f, -1.6256f),
                //new Vector2(0.17f, -1.6312f),
                //new Vector2(0.14f, -1.6358f),
                //new Vector2(0.11f, -1.6395f),
                //new Vector2(0.08f, -1.6422f),
                //new Vector2(0.05f, -1.6441f),
                //new Vector2(0.02f, -1.6451f),
                //new Vector2(0.00f, -1.6453f), //точка симметрии

                //new Vector2(-0.02f, -1.6451f),
                //new Vector2(-0.05f, -1.6441f),
                //new Vector2(-0.08f, -1.6422f),
                //new Vector2(-0.11f, -1.6395f),
                //new Vector2(-0.14f, -1.6358f),
                //new Vector2(-0.17f, -1.6312f),
                //new Vector2(-0.20f, -1.6256f),
                //new Vector2(-0.23f, -1.6189f),
                //new Vector2(-0.26f, -1.6112f),
                //new Vector2(-0.29f, -1.6022f),
                //new Vector2(-0.32f, -1.5920f),
                //new Vector2(-0.35f, -1.5803f),
                //new Vector2(-0.38f, -1.5672f),
                //new Vector2(-0.41f, -1.5523f),
                //new Vector2(-0.44f, -1.5355f),
                //new Vector2(-0.47f, -1.5166f),
                //new Vector2(-0.50f, -1.4953f),
                //new Vector2(-0.53f, -1.4712f),
                //new Vector2(-0.56f, -1.4440f),
                //new Vector2(-0.59f, -1.4130f),
                //new Vector2(-0.62f, -1.3776f),
                //new Vector2(-0.65f, -1.3370f),
                //new Vector2(-0.68f, -1.2901f),
                //new Vector2(-0.71f, -1.2355f),
                //new Vector2(-0.74f, -1.1714f),
                //new Vector2(-0.77f, -1.0951f),
                //new Vector2(-0.80f, -1.0031f),
                //new Vector2(-0.83f, -0.8904f),
                //new Vector2(-0.86f, -0.7493f),
                //new Vector2(-0.89f, -0.5680f),
                //new Vector2(-0.92f, -0.3268f),
                //new Vector2(-0.95f, 0.00895f),
                //new Vector2(-0.98f, 0.5077f),
                //new Vector2(-0.99f, 0.7323f),
                #endregion
                #region  half of ellipse points old
                //new Vector2(0.99f, 0.6846f),
                //new Vector2(0.98f, 0.5550f),
                //new Vector2(0.95f, 0.3018f),
                //new Vector2(0.92f, 0.1236f),
                //new Vector2(0.89f, -0.0196f),
                //new Vector2(0.86f, -0.1411f),
                //new Vector2(0.83f, -0.2472f),
                //new Vector2(0.80f, -0.3416f),
                //new Vector2(0.77f, -0.4267f),
                //new Vector2(0.74f, -0.5040f),
                //new Vector2(0.71f, -0.5746f),
                //new Vector2(0.68f, -0.6395f),
                //new Vector2(0.65f, -0.6993f),
                //new Vector2(0.62f, -0.7544f),
                //new Vector2(0.59f, -0.8054f),
                //new Vector2(0.56f, -0.8526f),
                //new Vector2(0.53f, -0.8962f),
                //new Vector2(0.50f, -0.9365f),
                //new Vector2(0.47f, -0.9737f),
                //new Vector2(0.44f, -1.0080f),
                //new Vector2(0.41f, -1.0395f),
                //new Vector2(0.38f, -1.0683f),
                //new Vector2(0.35f, -1.0946f),
                //new Vector2(0.32f, -1.1185f),
                //new Vector2(0.29f, -1.1400f),
                //new Vector2(0.26f, -1.1592f),
                //new Vector2(0.23f, -1.1761f),
                //new Vector2(0.20f, -1.1909f),
                //new Vector2(0.17f, -1.2035f),
                //new Vector2(0.14f, -1.2140f),
                //new Vector2(0.11f, -1.2225f),
                //new Vector2(0.08f, -1.2289f),
                //new Vector2(0.05f, -1.2333f),
                //new Vector2(0.02f, -1.2356f),
                //new Vector2(0.00f, -1.2361f), //точка симметрии

                //new Vector2(-0.02f, -1.2356f),
                //new Vector2(-0.05f, -1.2333f),
                //new Vector2(-0.08f, -1.2289f),
                //new Vector2(-0.11f, -1.2225f),
                //new Vector2(-0.14f, -1.2140f),
                //new Vector2(-0.17f, -1.2035f),
                //new Vector2(-0.20f, -1.1909f),
                //new Vector2(-0.23f, -1.1761f),
                //new Vector2(-0.26f, -1.1592f),
                //new Vector2(-0.29f, -1.1400f),
                //new Vector2(-0.32f, -1.1185f),
                //new Vector2(-0.35f, -1.0946f),
                //new Vector2(-0.38f, -1.0683f),
                //new Vector2(-0.41f, -1.0395f),
                //new Vector2(-0.44f, -1.0080f),
                //new Vector2(-0.47f, -0.9737f),
                //new Vector2(-0.50f, -0.9365f),
                //new Vector2(-0.53f, -0.8962f),
                //new Vector2(-0.56f, -0.8526f),
                //new Vector2(-0.59f, -0.8054f),
                //new Vector2(-0.62f, -0.7544f),
                //new Vector2(-0.65f, -0.6993f),
                //new Vector2(-0.68f, -0.6395f),
                //new Vector2(-0.71f, -0.5746f),
                //new Vector2(-0.74f, -0.5040f),
                //new Vector2(-0.77f, -0.4267f),
                //new Vector2(-0.80f, -0.3416f),
                //new Vector2(-0.83f, -0.2472f),
                //new Vector2(-0.86f, -0.1411f),
                //new Vector2(-0.89f, -0.0196f),
                //new Vector2(-0.92f, 0.1236f),
                //new Vector2(-0.95f, 0.3018f),
                //new Vector2(-0.98f, 0.5550f),
                //new Vector2(-0.99f, 0.6846f),
                #endregion

                #region круглая
                new Vector2(0.999f, -0.04471f),
                new Vector2(0.997f, -0.07740f),
                new Vector2(0.995f, -0.09987f),
                new Vector2(0.99f, -0.14107f),
                new Vector2(0.98f, -0.19900f),
                new Vector2(0.95f, -0.31225f),
                new Vector2(0.92f, -0.3919f),
                new Vector2(0.89f, -0.4560f),
                new Vector2(0.86f, -0.5103f),
                new Vector2(0.83f, -0.5578f),
                new Vector2(0.80f, -0.6000f),
                new Vector2(0.77f, -0.6380f),
                new Vector2(0.74f, -0.6726f),
                new Vector2(0.71f, -0.7042f),
                new Vector2(0.68f, -0.7332f),
                new Vector2(0.65f, -0.7599f),
                new Vector2(0.62f, -0.7846f),
                new Vector2(0.59f, -0.8074f),
                new Vector2(0.56f, -0.8285f),
                new Vector2(0.53f, -0.8480f),
                new Vector2(0.50f, -0.8660f),
                new Vector2(0.47f, -0.8827f),
                new Vector2(0.44f, -0.8980f),
                new Vector2(0.41f, -0.9121f),
                new Vector2(0.38f, -0.9250f),
                new Vector2(0.35f, -0.9367f),
                new Vector2(0.32f, -0.9474f),
                new Vector2(0.29f, -0.9570f),
                new Vector2(0.26f, -0.9656f),
                new Vector2(0.23f, -0.9732f),
                new Vector2(0.20f, -0.9798f),
                new Vector2(0.17f, -0.9854f),
                new Vector2(0.14f, -0.9902f),
                new Vector2(0.11f, -0.9939f),
                new Vector2(0.08f, -0.9968f),
                new Vector2(0.05f, -0.9987f),
                new Vector2(0.02f, -0.9998f),
                new Vector2(0.00f, -1.0000f), //точка симметрии

                new Vector2(-0.02f, -0.9998f),
                new Vector2(-0.05f, -0.9987f),
                new Vector2(-0.08f, -0.9968f),
                new Vector2(-0.11f, -0.9939f),
                new Vector2(-0.14f, -0.9902f),
                new Vector2(-0.17f, -0.9854f),
                new Vector2(-0.20f, -0.9798f),
                new Vector2(-0.23f, -0.9732f),
                new Vector2(-0.26f, -0.9656f),
                new Vector2(-0.29f, -0.9570f),
                new Vector2(-0.32f, -0.9474f),
                new Vector2(-0.35f, -0.9367f),
                new Vector2(-0.38f, -0.9250f),
                new Vector2(-0.41f, -0.9121f),
                new Vector2(-0.44f, -0.8980f),
                new Vector2(-0.47f, -0.8827f),
                new Vector2(-0.50f, -0.8660f),
                new Vector2(-0.53f, -0.8480f),
                new Vector2(-0.56f, -0.8285f),
                new Vector2(-0.59f, -0.8074f),
                new Vector2(-0.62f, -0.7846f),
                new Vector2(-0.65f, -0.7599f),
                new Vector2(-0.68f, -0.7332f),
                new Vector2(-0.71f, -0.7042f),
                new Vector2(-0.74f, -0.6726f),
                new Vector2(-0.77f, -0.6380f),
                new Vector2(-0.80f, -0.6000f),
                new Vector2(-0.83f, -0.5578f),
                new Vector2(-0.86f, -0.5103f),
                new Vector2(-0.89f, -0.4560f),
                new Vector2(-0.92f, -0.3919f),
                new Vector2(-0.95f, -0.31225f),
                new Vector2(-0.98f, -0.19900f),
                new Vector2(-0.99f, -0.14107f),
                new Vector2(-0.995f, -0.09987f),
                new Vector2(-0.997f, -0.07740f),
                new Vector2(-0.999f, -0.04471f),
#endregion
            };
            private static Vector2[] pointsDirectionUp_Main =
            {
  #region вытянутая парабола
                
                //new Vector2(0.995f, -0.7303f),
                //new Vector2(0.99f, -0.6191f),
                //new Vector2(0.98f, -0.4627f),
                //new Vector2(0.95f, -0.1569f),
                //new Vector2(0.92f, 0.0582f),
                //new Vector2(0.89f, 0.2311f),
                //new Vector2(0.86f, 0.3778f),
                //new Vector2(0.83f, 0.5060f),
                //new Vector2(0.80f, 0.6200f),
                //new Vector2(0.77f, 0.7227f),
                //new Vector2(0.74f, 0.8160f),
                //new Vector2(0.71f, 0.9013f),
                //new Vector2(0.68f, 0.9797f),
                //new Vector2(0.65f, 1.0518f),
                //new Vector2(0.62f, 1.1184f),
                //new Vector2(0.59f, 1.1800f),
                //new Vector2(0.56f, 1.2369f),
                //new Vector2(0.53f, 1.2896f),
                //new Vector2(0.50f, 1.3383f),
                //new Vector2(0.47f, 1.3832f),
                //new Vector2(0.44f, 1.4246f),
                //new Vector2(0.41f, 1.4626f),
                //new Vector2(0.38f, 1.4975f),
                //new Vector2(0.35f, 1.5292f),
                //new Vector2(0.32f, 1.5580f),
                //new Vector2(0.29f, 1.5840f),
                //new Vector2(0.26f, 1.6071f),
                //new Vector2(0.23f, 1.6276f),
                //new Vector2(0.20f, 1.6454f),
                //new Vector2(0.17f, 1.6607f),
                //new Vector2(0.14f, 1.6734f),
                //new Vector2(0.11f, 1.6836f),
                //new Vector2(0.08f, 1.6913f),
                //new Vector2(0.05f, 1.6966f),
                //new Vector2(0.02f, 1.6995f),
                //new Vector2(0.00f, 1.7000f), //точка симметрии

                //new Vector2(-0.02f, 1.6995f),
                //new Vector2(-0.05f, 1.6966f),
                //new Vector2(-0.08f, 1.6913f),
                //new Vector2(-0.11f, 1.6836f),
                //new Vector2(-0.14f, 1.6734f),
                //new Vector2(-0.17f, 1.6607f),
                //new Vector2(-0.20f, 1.6454f),
                //new Vector2(-0.23f, 1.6276f),
                //new Vector2(-0.26f, 1.6071f),
                //new Vector2(-0.29f, 1.5840f),
                //new Vector2(-0.32f, 1.5580f),
                //new Vector2(-0.35f, 1.5292f),
                //new Vector2(-0.38f, 1.4975f),
                //new Vector2(-0.41f, 1.4626f),
                //new Vector2(-0.44f, 1.4246f),
                //new Vector2(-0.47f, 1.3832f),
                //new Vector2(-0.50f, 1.3383f),
                //new Vector2(-0.53f, 1.2896f),
                //new Vector2(-0.56f, 1.2369f),
                //new Vector2(-0.59f, 1.1800f),
                //new Vector2(-0.62f, 1.1184f),
                //new Vector2(-0.65f, 1.0518f),
                //new Vector2(-0.68f, 0.9797f),
                //new Vector2(-0.71f, 0.9013f),
                //new Vector2(-0.74f, 0.8160f),
                //new Vector2(-0.77f, 0.7227f),
                //new Vector2(-0.80f, 0.6200f),
                //new Vector2(-0.83f, 0.5060f),
                //new Vector2(-0.86f, 0.3778f),
                //new Vector2(-0.89f, 0.2311f),
                //new Vector2(-0.92f, 0.0582f),
                //new Vector2(-0.95f, -0.1569f),
                //new Vector2(-0.98f, -0.4627f),
                //new Vector2(-0.99f, -0.6191f),
                //new Vector2(-0.995f, -0.7303f),
#endregion

                #region круглая
                new Vector2(0.999f, 0.04471f),
                new Vector2(0.997f, 0.07740f),
                new Vector2(0.995f, 0.09987f),
                new Vector2(0.99f, 0.14107f),
                new Vector2(0.98f, 0.19900f),
                new Vector2(0.95f, 0.31225f),
                new Vector2(0.92f, 0.3919f),
                new Vector2(0.89f, 0.4560f),
                new Vector2(0.86f, 0.5103f),
                new Vector2(0.83f, 0.5578f),
                new Vector2(0.80f, 0.6000f),
                new Vector2(0.77f, 0.6380f),
                new Vector2(0.74f, 0.6726f),
                new Vector2(0.71f, 0.7042f),
                new Vector2(0.68f, 0.7332f),
                new Vector2(0.65f, 0.7599f),
                new Vector2(0.62f, 0.7846f),
                new Vector2(0.59f, 0.8074f),
                new Vector2(0.56f, 0.8285f),
                new Vector2(0.53f, 0.8480f),
                new Vector2(0.50f, 0.8660f),
                new Vector2(0.47f, 0.8827f),
                new Vector2(0.44f, 0.8980f),
                new Vector2(0.41f, 0.9121f),
                new Vector2(0.38f, 0.9250f),
                new Vector2(0.35f, 0.9367f),
                new Vector2(0.32f, 0.9474f),
                new Vector2(0.29f, 0.9570f),
                new Vector2(0.26f, 0.9656f),
                new Vector2(0.23f, 0.9732f),
                new Vector2(0.20f, 0.9798f),
                new Vector2(0.17f, 0.9854f),
                new Vector2(0.14f, 0.9902f),
                new Vector2(0.11f, 0.9939f),
                new Vector2(0.08f, 0.9968f),
                new Vector2(0.05f, 0.9987f),
                new Vector2(0.02f, 0.9998f),
                new Vector2(0.00f, 1.0000f), //точка симметрии

                new Vector2(-0.02f, 0.9998f),
                new Vector2(-0.05f, 0.9987f),
                new Vector2(-0.08f, 0.9968f),
                new Vector2(-0.11f, 0.9939f),
                new Vector2(-0.14f, 0.9902f),
                new Vector2(-0.17f, 0.9854f),
                new Vector2(-0.20f, 0.9798f),
                new Vector2(-0.23f, 0.9732f),
                new Vector2(-0.26f, 0.9656f),
                new Vector2(-0.29f, 0.9570f),
                new Vector2(-0.32f, 0.9474f),
                new Vector2(-0.35f, 0.9367f),
                new Vector2(-0.38f, 0.9250f),
                new Vector2(-0.41f, 0.9121f),
                new Vector2(-0.44f, 0.8980f),
                new Vector2(-0.47f, 0.8827f),
                new Vector2(-0.50f, 0.8660f),
                new Vector2(-0.53f, 0.8480f),
                new Vector2(-0.56f, 0.8285f),
                new Vector2(-0.59f, 0.8074f),
                new Vector2(-0.62f, 0.7846f),
                new Vector2(-0.65f, 0.7599f),
                new Vector2(-0.68f, 0.7332f),
                new Vector2(-0.71f, 0.7042f),
                new Vector2(-0.74f, 0.6726f),
                new Vector2(-0.77f, 0.6380f),
                new Vector2(-0.80f, 0.6000f),
                new Vector2(-0.83f, 0.5578f),
                new Vector2(-0.86f, 0.5103f),
                new Vector2(-0.89f, 0.4560f),
                new Vector2(-0.92f, 0.3919f),
                new Vector2(-0.95f, 0.31225f),
                new Vector2(-0.98f, 0.19900f),
                new Vector2(-0.99f, 0.14107f),
                new Vector2(-0.995f, 0.09987f),
                new Vector2(-0.997f, 0.07740f),
                new Vector2(-0.999f, 0.04471f),

#endregion

            };

            private static Vector2[] pointsDirectionUpLeft_Mirror =
            {

#region вытянутая парабола
                //new Vector2(-0.1f, -0.8990f),
                //new Vector2(-0.3f, -0.6907f),
                //new Vector2(-0.5f, -0.4733f),
                //new Vector2(-0.7f, -0.2456f),
                //new Vector2(-0.9f, -0.00588f),
                //new Vector2(-1.1f, 0.24906f),
                //new Vector2(-1.3f, 0.5247f),
                //new Vector2(-1.4f, 0.6736f),
                //new Vector2(-1.5f, 0.8333f),
                //new Vector2(-1.6f, 1.0096f),
                //new Vector2(-1.7f, 1.2178f),
                //new Vector2(-1.73f, 1.2934f),
                //new Vector2(-1.76f, 1.3840f),
                //new Vector2(-1.77f, 1.4211f),
                //new Vector2(-1.78f, 1.4661f),
                //new Vector2(-1.79f, 1.5392f),
                //new Vector2(-1.791f, 1.5573f),
                //new Vector2(-1.7913f, 1.5731f), //крайняя точка по оси 

                //new Vector2(-1.791f, 1.5883f),
                //new Vector2(-1.79f, 1.6046f),
                //new Vector2(-1.78f, 1.6596f),
                //new Vector2(-1.77f, 1.6865f),
                //new Vector2(-1.76f, 1.7055f),
                //new Vector2(-1.75f, 1.7201f),
                //new Vector2(-1.7361f, 1.7361f), //точка симметрии. Но не по оси X или Y, а вдоль прямой "y = -x".

                //new Vector2(-1.73f, 1.7418f),
                //new Vector2(-1.72f, 1.7501f),
                //new Vector2(-1.71f, 1.7572f),
                //new Vector2(-1.70f, 1.7632f),
                //new Vector2(-1.69f, 1.7684f),
                //new Vector2(-1.68f, 1.7728f),
                //new Vector2(-1.67f, 1.7766f),
                //new Vector2(-1.66f, 1.7799f),
                //new Vector2(-1.65f, 1.7826f),
                //new Vector2(-1.64f, 1.7849f),
                //new Vector2(-1.63f, 1.7868f),
                //new Vector2(-1.62f, 1.7883f),
                //new Vector2(-1.6f, 1.7895f),
                //new Vector2(-1.59f, 1.7909f),
                //new Vector2(-1.58f, 1.7912f),

                //new Vector2(-1.5722f, 1.7913f), //крайняя точка по оси 
                //new Vector2(-1.56f, 1.7911f),
                //new Vector2(-1.55f, 1.7907f),
                //new Vector2(-1.54f, 1.7901f),
                //new Vector2(-1.53f, 1.7892f),
                //new Vector2(-1.52f, 1.7882f),
                //new Vector2(-1.5f, 1.7857f),
                //new Vector2(-1.45f, 1.7767f),
                //new Vector2(-1.4f, 1.7645f),
                //new Vector2(-1.3f, 1.7324f),
                //new Vector2(-1.2f, 1.6924f),
                //new Vector2(-1.1f, 1.6462f),
                //new Vector2(-1.0f, 1.5949f),
                //new Vector2(-0.9f, 1.5392f),
                //new Vector2(-0.8f, 1.4798f),
                //new Vector2(-0.7f, 1.4171f),
                //new Vector2(-0.6f, 1.3513f),
                //new Vector2(-0.5f, 1.2828f),
                //new Vector2(-0.4f, 1.2118f),
                //new Vector2(-0.3f, 1.1383f),
                //new Vector2(-0.1f, 0.9847f),
                //new Vector2(0.1f, 0.8228f),
                //new Vector2(0.3f, 0.6531f),
                //new Vector2(0.5f, 0.4759f),
                //new Vector2(0.7f, 0.29122f),
                //new Vector2(0.9f, 0.09901f),

                #endregion

                #region круглая
                new Vector2(-0.5098f, -0.49f),
                new Vector2(-0.5192f, -0.48f),
                new Vector2(-0.5283f, -0.47f),
                new Vector2(-0.5454f, -0.45f),
                new Vector2(-0.5613f, -0.43f),
                new Vector2(-0.5831f, -0.40f),
                new Vector2(-0.6026f, -0.37f),
                new Vector2(-0.6200f, -0.34f),
                new Vector2(-0.6355f, -0.31f),
                new Vector2(-0.6493f, -0.28f),
                new Vector2(-0.6614f, -0.25f),
                new Vector2(-0.6720f, -0.22f),
                new Vector2(-0.6811f, -0.19f),
                new Vector2(-0.6888f, -0.16f),
                new Vector2(-0.6951f, -0.13f),
                new Vector2(-0.7000f, -0.10f),
                new Vector2(-0.7026f, -0.08f),
                new Vector2(-0.7046f, -0.06f),
                new Vector2(-0.7060f, -0.04f),
                new Vector2(-0.7068f, -0.02f),
                new Vector2(-0.7071f, -0.00f), //точка симметрии первой четверти

                new Vector2(-0.7068f, 0.02f),
                new Vector2(-0.7060f, 0.04f),
                new Vector2(-0.7046f, 0.06f),
                new Vector2(-0.7026f, 0.08f),
                new Vector2(-0.7000f, 0.10f),
                new Vector2(-0.6951f, 0.13f),
                new Vector2(-0.6888f, 0.16f),
                new Vector2(-0.6811f, 0.19f),
                new Vector2(-0.6720f, 0.22f),
                new Vector2(-0.6614f, 0.25f),
                new Vector2(-0.6493f, 0.28f),
                new Vector2(-0.6355f, 0.31f),
                new Vector2(-0.6200f, 0.34f),
                new Vector2(-0.6026f, 0.37f),
                new Vector2(-0.5831f, 0.40f),
                new Vector2(-0.5613f, 0.43f),
                new Vector2(-0.5454f, 0.45f),
                new Vector2(-0.5283f, 0.47f),
                new Vector2(-0.5192f, 0.48f),
                new Vector2(-0.5098f, 0.49f),
                new Vector2(-0.5000f, 0.50f), //точка симметрии середины (второй четверти)

                new Vector2(-0.4681f, 0.53f),
                new Vector2(-0.4317f, 0.56f),
                new Vector2(-0.3897f, 0.59f),
                new Vector2(-0.3400f, 0.62f),
                new Vector2(-0.27839f, 0.65f),
                new Vector2(-0.19391f, 0.68f),
                new Vector2(-0.1000f, 0.70f),
                new Vector2(-0.0000f, 0.7071f), //точка симметрии третьей четверти

                new Vector2(0.1000f, 0.70f),
                new Vector2(0.19391f, 0.68f),
                new Vector2(0.27839f, 0.65f),
                new Vector2(0.3400f, 0.62f),
                new Vector2(0.3897f, 0.59f),
                new Vector2(0.4317f, 0.56f),
                new Vector2(0.4681f, 0.53f),
                new Vector2(0.4792f, 0.52f),
                new Vector2(0.4898f, 0.51f),
                new Vector2(0.4949f, 0.505f),
#endregion
            };
            private static Vector2[] pointsDirectionDownLeft_Main =
            {
#region вытянутая парабола
                //new Vector2(-0.1f, 0.8990f),
                //new Vector2(-0.3f, 0.6907f),
                //new Vector2(-0.5f, 0.4733f),
                //new Vector2(-0.7f, 0.2456f),
                //new Vector2(-0.9f, 0.00588f),
                //new Vector2(-1.1f, -0.24906f),
                //new Vector2(-1.3f, -0.5247f),
                //new Vector2(-1.4f, -0.6736f),
                //new Vector2(-1.5f, -0.8333f),
                //new Vector2(-1.6f, -1.0096f),
                //new Vector2(-1.7f, -1.2178f),
                //new Vector2(-1.73f, -1.2934f),
                //new Vector2(-1.76f, -1.3840f),
                //new Vector2(-1.77f, -1.4211f),
                //new Vector2(-1.78f, -1.4661f),
                //new Vector2(-1.79f, -1.5392f),
                //new Vector2(-1.791f, -1.5573f),
                //new Vector2(-1.7913f, -1.5731f), //крайняя точка по оси 

                //new Vector2(-1.791f, -1.5883f),
                //new Vector2(-1.79f, -1.6046f),
                //new Vector2(-1.78f, -1.6596f),
                //new Vector2(-1.77f, -1.6865f),
                //new Vector2(-1.76f, -1.7055f),
                //new Vector2(-1.75f, -1.7201f),
                //new Vector2(-1.7361f, -1.7361f), //точка симметрии. Но не по оси X или Y, а вдоль прямой "y = -x".

                //new Vector2(-1.73f, -1.7418f),
                //new Vector2(-1.72f, -1.7501f),
                //new Vector2(-1.71f, -1.7572f),
                //new Vector2(-1.70f, -1.7632f),
                //new Vector2(-1.69f, -1.7684f),
                //new Vector2(-1.68f, -1.7728f),
                //new Vector2(-1.67f, -1.7766f),
                //new Vector2(-1.66f, -1.7799f),
                //new Vector2(-1.65f, -1.7826f),
                //new Vector2(-1.64f, -1.7849f),
                //new Vector2(-1.63f, -1.7868f),
                //new Vector2(-1.62f, -1.7883f),
                //new Vector2(-1.6f, -1.7895f),
                //new Vector2(-1.59f, -1.7909f),
                //new Vector2(-1.58f, -1.7912f),

                //new Vector2(-1.5722f, -1.7913f), //крайняя точка по оси 
                //new Vector2(-1.56f, -1.7911f),
                //new Vector2(-1.55f, -1.7907f),
                //new Vector2(-1.54f, -1.7901f),
                //new Vector2(-1.53f, -1.7892f),
                //new Vector2(-1.52f, -1.7882f),
                //new Vector2(-1.5f, -1.7857f),
                //new Vector2(-1.45f, -1.7767f),
                //new Vector2(-1.4f, -1.7645f),
                //new Vector2(-1.3f, -1.7324f),
                //new Vector2(-1.2f, -1.6924f),
                //new Vector2(-1.1f, -1.6462f),
                //new Vector2(-1.0f, -1.5949f),
                //new Vector2(-0.9f, -1.5392f),
                //new Vector2(-0.8f, -1.4798f),
                //new Vector2(-0.7f, -1.4171f),
                //new Vector2(-0.6f, -1.3513f),
                //new Vector2(-0.5f, -1.2828f),
                //new Vector2(-0.4f, -1.2118f),
                //new Vector2(-0.3f, -1.1383f),
                //new Vector2(-0.1f, -0.9847f),
                //new Vector2(0.1f, -0.8228f),
                //new Vector2(0.3f, -0.6531f),
                //new Vector2(0.5f, -0.4759f),
                //new Vector2(0.7f, -0.29122f),
                //new Vector2(0.9f, -0.09901f),
#endregion

                #region круглая
                new Vector2(-0.5098f, 0.49f),
                new Vector2(-0.5192f, 0.48f),
                new Vector2(-0.5283f, 0.47f),
                new Vector2(-0.5454f, 0.45f),
                new Vector2(-0.5613f, 0.43f),
                new Vector2(-0.5831f, 0.40f),
                new Vector2(-0.6026f, 0.37f),
                new Vector2(-0.6200f, 0.34f),
                new Vector2(-0.6355f, 0.31f),
                new Vector2(-0.6493f, 0.28f),
                new Vector2(-0.6614f, 0.25f),
                new Vector2(-0.6720f, 0.22f),
                new Vector2(-0.6811f, 0.19f),
                new Vector2(-0.6888f, 0.16f),
                new Vector2(-0.6951f, 0.13f),
                new Vector2(-0.7000f, 0.10f),
                new Vector2(-0.7026f, 0.08f),
                new Vector2(-0.7046f, 0.06f),
                new Vector2(-0.7060f, 0.04f),
                new Vector2(-0.7068f, 0.02f),
                new Vector2(-0.7071f, -0.00f), //точка симметрии первой четверти

                new Vector2(-0.7068f, -0.02f),
                new Vector2(-0.7060f, -0.04f),
                new Vector2(-0.7046f, -0.06f),
                new Vector2(-0.7026f, -0.08f),
                new Vector2(-0.7000f, -0.10f),
                new Vector2(-0.6951f, -0.13f),
                new Vector2(-0.6888f, -0.16f),
                new Vector2(-0.6811f, -0.19f),
                new Vector2(-0.6720f, -0.22f),
                new Vector2(-0.6614f, -0.25f),
                new Vector2(-0.6493f, -0.28f),
                new Vector2(-0.6355f, -0.31f),
                new Vector2(-0.6200f, -0.34f),
                new Vector2(-0.6026f, -0.37f),
                new Vector2(-0.5831f, -0.40f),
                new Vector2(-0.5613f, -0.43f),
                new Vector2(-0.5454f, -0.45f),
                new Vector2(-0.5283f, -0.47f),
                new Vector2(-0.5192f, -0.48f),
                new Vector2(-0.5098f, -0.49f),
                new Vector2(-0.5000f, -0.50f), //точка симметрии середины (второй четверти)

                new Vector2(-0.4949f, -0.505f),
                new Vector2(-0.4898f, -0.51f),
                new Vector2(-0.4792f, -0.52f),
                new Vector2(-0.4681f, -0.53f),
                new Vector2(-0.4317f, -0.56f),
                new Vector2(-0.3897f, -0.59f),
                new Vector2(-0.3400f, -0.62f),
                new Vector2(-0.27839f, -0.65f),
                new Vector2(-0.19391f, -0.68f),
                new Vector2(-0.1000f, -0.70f),
                new Vector2(0.0000f, -0.7071f), //точка симметрии третьей четверти

                new Vector2(0.1000f, -0.70f),
                new Vector2(0.19391f, -0.68f),
                new Vector2(0.27839f, -0.65f),
                new Vector2(0.3400f, -0.62f),
                new Vector2(0.3897f, -0.59f),
                new Vector2(0.4317f, -0.56f),
                new Vector2(0.4681f, -0.53f),

#endregion
            };
            private static Vector2[] pointsDirectionUpRight_Mirror =
            {
#region вытянутая парабола
                //new Vector2(0.1f, -0.8990f),
                //new Vector2(0.3f, -0.6907f),
                //new Vector2(0.5f, -0.4733f),
                //new Vector2(0.7f, -0.2456f),
                //new Vector2(0.9f, -0.00588f),
                //new Vector2(1.1f, 0.24906f),
                //new Vector2(1.3f, 0.5247f),
                //new Vector2(1.4f, 0.6736f),
                //new Vector2(1.5f, 0.8333f),
                //new Vector2(1.6f, 1.0096f),
                //new Vector2(1.7f, 1.2178f),
                //new Vector2(1.73f, 1.2934f),
                //new Vector2(1.76f, 1.3840f),
                //new Vector2(1.77f, 1.4211f),
                //new Vector2(1.78f, 1.4661f),
                //new Vector2(1.79f, 1.5392f),
                //new Vector2(1.791f, 1.5573f),
                //new Vector2(1.7913f, 1.5731f), //крайняя точка по оси 

                //new Vector2(1.791f, 1.5883f),
                //new Vector2(1.79f, 1.6046f),
                //new Vector2(1.78f, 1.6596f),
                //new Vector2(1.77f, 1.6865f),
                //new Vector2(1.76f, 1.7055f),
                //new Vector2(1.75f, 1.7201f),
                //new Vector2(1.7361f, 1.7361f), //точка симметрии. Но не по оси X или Y, а вдоль прямой "y = -x".

                //new Vector2(1.73f, 1.7418f),
                //new Vector2(1.72f, 1.7501f),
                //new Vector2(1.71f, 1.7572f),
                //new Vector2(1.70f, 1.7632f),
                //new Vector2(1.69f, 1.7684f),
                //new Vector2(1.68f, 1.7728f),
                //new Vector2(1.67f, 1.7766f),
                //new Vector2(1.66f, 1.7799f),
                //new Vector2(1.65f, 1.7826f),
                //new Vector2(1.64f, 1.7849f),
                //new Vector2(1.63f, 1.7868f),
                //new Vector2(1.62f, 1.7883f),
                //new Vector2(1.6f, 1.7895f),
                //new Vector2(1.59f, 1.7909f),
                //new Vector2(1.58f, 1.7912f),

                //new Vector2(1.5722f, 1.7913f), //крайняя точка по оси 
                //new Vector2(1.56f, 1.7911f),
                //new Vector2(1.55f, 1.7907f),
                //new Vector2(1.54f, 1.7901f),
                //new Vector2(1.53f, 1.7892f),
                //new Vector2(1.52f, 1.7882f),
                //new Vector2(1.5f, 1.7857f),
                //new Vector2(1.45f, 1.7767f),
                //new Vector2(1.4f, 1.7645f),
                //new Vector2(1.3f, 1.7324f),
                //new Vector2(1.2f, 1.6924f),
                //new Vector2(1.1f, 1.6462f),
                //new Vector2(1.0f, 1.5949f),
                //new Vector2(0.9f, 1.5392f),
                //new Vector2(0.8f, 1.4798f),
                //new Vector2(0.7f, 1.4171f),
                //new Vector2(0.6f, 1.3513f),
                //new Vector2(0.5f, 1.2828f),
                //new Vector2(0.4f, 1.2118f),
                //new Vector2(0.3f, 1.1383f),
                //new Vector2(0.1f, 0.9847f),
                //new Vector2(-0.1f, 0.8228f),
                //new Vector2(-0.3f, 0.6531f),
                //new Vector2(-0.5f, 0.4759f),
                //new Vector2(-0.7f, 0.29122f),
                //new Vector2(-0.9f, 0.09901f),
#endregion

                #region круглая
                new Vector2(0.5098f, -0.49f),
                new Vector2(0.5192f, -0.48f),
                new Vector2(0.5283f, -0.47f),
                new Vector2(0.5454f, -0.45f),
                new Vector2(0.5613f, -0.43f),
                new Vector2(0.5831f, -0.40f),
                new Vector2(0.6026f, -0.37f),
                new Vector2(0.6200f, -0.34f),
                new Vector2(0.6355f, -0.31f),
                new Vector2(0.6493f, -0.28f),
                new Vector2(0.6614f, -0.25f),
                new Vector2(0.6720f, -0.22f),
                new Vector2(0.6811f, -0.19f),
                new Vector2(0.6888f, -0.16f),
                new Vector2(0.6951f, -0.13f),
                new Vector2(0.7000f, -0.10f),
                new Vector2(0.7026f, -0.08f),
                new Vector2(0.7046f, -0.06f),
                new Vector2(0.7060f, -0.04f),
                new Vector2(0.7068f, -0.02f),
                new Vector2(0.7071f, 0.00f), //точка симметрии первой четверти

                new Vector2(0.7068f, 0.02f),
                new Vector2(0.7060f, 0.04f),
                new Vector2(0.7046f, 0.06f),
                new Vector2(0.7026f, 0.08f),
                new Vector2(0.7000f, 0.10f),
                new Vector2(0.6951f, 0.13f),
                new Vector2(0.6888f, 0.16f),
                new Vector2(0.6811f, 0.19f),
                new Vector2(0.6720f, 0.22f),
                new Vector2(0.6614f, 0.25f),
                new Vector2(0.6493f, 0.28f),
                new Vector2(0.6355f, 0.31f),
                new Vector2(0.6200f, 0.34f),
                new Vector2(0.6026f, 0.37f),
                new Vector2(0.5831f, 0.40f),
                new Vector2(0.5613f, 0.43f),
                new Vector2(0.5454f, 0.45f),
                new Vector2(0.5283f, 0.47f),
                new Vector2(0.5192f, 0.48f),
                new Vector2(0.5098f, 0.49f),
                new Vector2(0.5000f, 0.50f), //точка симметрии середины (второй четверти)

                new Vector2(0.4949f, 0.505f),
                new Vector2(0.4898f, 0.51f),
                new Vector2(0.4792f, 0.52f),
                new Vector2(0.4681f, 0.53f),
                new Vector2(0.4317f, 0.56f),
                new Vector2(0.3897f, 0.59f),
                new Vector2(0.3400f, 0.62f),
                new Vector2(0.27839f, 0.65f),
                new Vector2(0.19391f, 0.68f),
                new Vector2(0.1000f, 0.70f),
                new Vector2(0.0000f, 0.7071f), //точка симметрии третьей четверти

                new Vector2(-0.1000f, 0.70f),
                new Vector2(-0.19391f, 0.68f),
                new Vector2(-0.27839f, 0.65f),
                new Vector2(-0.3400f, 0.62f),
                new Vector2(-0.3897f, 0.59f),
                new Vector2(-0.4317f, 0.56f),
                new Vector2(-0.4681f, 0.53f),


#endregion
            };
            private static Vector2[] pointsDirectionDownRight_Main =
            {
                #region вытянутая парабола
                //new Vector2(0.1f, 0.8990f),
                //new Vector2(0.3f, 0.6907f),
                //new Vector2(0.5f, 0.4733f),
                //new Vector2(0.7f, 0.2456f),
                //new Vector2(0.9f, 0.00588f),
                //new Vector2(1.1f, -0.24906f),
                //new Vector2(1.3f, -0.5247f),
                //new Vector2(1.4f, -0.6736f),
                //new Vector2(1.5f, -0.8333f),
                //new Vector2(1.6f, -1.0096f),
                //new Vector2(1.7f, -1.2178f),
                //new Vector2(1.73f, -1.2934f),
                //new Vector2(1.76f, -1.3840f),
                //new Vector2(1.77f, -1.4211f),
                //new Vector2(1.78f, -1.4661f),
                //new Vector2(1.79f, -1.5392f),
                //new Vector2(1.791f, -1.5573f),
                //new Vector2(1.7913f, -1.5731f), //крайняя точка по оси 

                //new Vector2(1.791f, -1.5883f),
                //new Vector2(1.79f, -1.6046f),
                //new Vector2(1.78f, -1.6596f),
                //new Vector2(1.77f, -1.6865f),
                //new Vector2(1.76f, -1.7055f),
                //new Vector2(1.75f, -1.7201f),
                //new Vector2(1.7361f, -1.7361f), //точка симметрии. Но не по оси X или Y, а вдоль прямой "y = -x".

                //new Vector2(1.73f, -1.7418f),
                //new Vector2(1.72f, -1.7501f),
                //new Vector2(1.71f, -1.7572f),
                //new Vector2(1.70f, -1.7632f),
                //new Vector2(1.69f, -1.7684f),
                //new Vector2(1.68f, -1.7728f),
                //new Vector2(1.67f, -1.7766f),
                //new Vector2(1.66f, -1.7799f),
                //new Vector2(1.65f, -1.7826f),
                //new Vector2(1.64f, -1.7849f),
                //new Vector2(1.63f, -1.7868f),
                //new Vector2(1.62f, -1.7883f),
                //new Vector2(1.6f, -1.7895f),
                //new Vector2(1.59f, -1.7909f),
                //new Vector2(1.58f, -1.7912f),

                //new Vector2(1.5722f, -1.7913f), //крайняя точка по оси 
                //new Vector2(1.56f, -1.7911f),
                //new Vector2(1.55f, -1.7907f),
                //new Vector2(1.54f, -1.7901f),
                //new Vector2(1.53f, -1.7892f),
                //new Vector2(1.52f, -1.7882f),
                //new Vector2(1.5f, -1.7857f),
                //new Vector2(1.45f, -1.7767f),
                //new Vector2(1.4f, -1.7645f),
                //new Vector2(1.3f, -1.7324f),
                //new Vector2(1.2f, -1.6924f),
                //new Vector2(1.1f, -1.6462f),
                //new Vector2(1.0f, -1.5949f),
                //new Vector2(0.9f, -1.5392f),
                //new Vector2(0.8f, -1.4798f),
                //new Vector2(0.7f, -1.4171f),
                //new Vector2(0.6f, -1.3513f),
                //new Vector2(0.5f, -1.2828f),
                //new Vector2(0.4f, -1.2118f),
                //new Vector2(0.3f, -1.1383f),
                //new Vector2(0.1f, -0.9847f),
                //new Vector2(-0.1f, -0.8228f),
                //new Vector2(-0.3f, -0.6531f),
                //new Vector2(-0.5f, -0.4759f),
                //new Vector2(-0.7f, -0.29122f),
                //new Vector2(-0.9f, -0.09901f),
#endregion

                #region круглая
                new Vector2(0.5098f, 0.49f),
                new Vector2(0.5192f, 0.48f),
                new Vector2(0.5283f, 0.47f),
                new Vector2(0.5454f, 0.45f),
                new Vector2(0.5613f, 0.43f),
                new Vector2(0.5831f, 0.40f),
                new Vector2(0.6026f, 0.37f),
                new Vector2(0.6200f, 0.34f),
                new Vector2(0.6355f, 0.31f),
                new Vector2(0.6493f, 0.28f),
                new Vector2(0.6614f, 0.25f),
                new Vector2(0.6720f, 0.22f),
                new Vector2(0.6811f, 0.19f),
                new Vector2(0.6888f, 0.16f),
                new Vector2(0.6951f, 0.13f),
                new Vector2(0.7000f, 0.10f),
                new Vector2(0.7026f, 0.08f),
                new Vector2(0.7046f, 0.06f),
                new Vector2(0.7060f, 0.04f),
                new Vector2(0.7068f, 0.02f),
                new Vector2(0.7071f, 0.00f), //точка симметрии первой четверти

                new Vector2(0.7068f, -0.02f),
                new Vector2(0.7060f, -0.04f),
                new Vector2(0.7046f, -0.06f),
                new Vector2(0.7026f, -0.08f),
                new Vector2(0.7000f, -0.10f),
                new Vector2(0.6951f, -0.13f),
                new Vector2(0.6888f, -0.16f),
                new Vector2(0.6811f, -0.19f),
                new Vector2(0.6720f, -0.22f),
                new Vector2(0.6614f, -0.25f),
                new Vector2(0.6493f, -0.28f),
                new Vector2(0.6355f, -0.31f),
                new Vector2(0.6200f, -0.34f),
                new Vector2(0.6026f, -0.37f),
                new Vector2(0.5831f, -0.40f),
                new Vector2(0.5613f, -0.43f),
                new Vector2(0.5454f, -0.45f),
                new Vector2(0.5283f, -0.47f),
                new Vector2(0.5192f, -0.48f),
                new Vector2(0.5098f, -0.49f),
                new Vector2(0.5000f, -0.50f), //точка симметрии середины (второй четверти)

                new Vector2(0.4681f, -0.53f),
                new Vector2(0.4317f, -0.56f),
                new Vector2(0.3897f, -0.59f),
                new Vector2(0.3400f, -0.62f),
                new Vector2(0.27839f, -0.65f),
                new Vector2(0.19391f, -0.68f),
                new Vector2(0.1000f, -0.70f),
                new Vector2(0.0000f, -0.7071f), //точка симметрии третьей четверти

                new Vector2(-0.1000f, -0.70f),
                new Vector2(-0.19391f, -0.68f),
                new Vector2(-0.27839f, -0.65f),
                new Vector2(-0.3400f, -0.62f),
                new Vector2(-0.3897f, -0.59f),
                new Vector2(-0.4317f, -0.56f),
                new Vector2(-0.4681f, -0.53f),
                new Vector2(-0.4792f, -0.52f),
                new Vector2(-0.4898f, -0.51f),
                new Vector2(-0.4949f, -0.505f),

#endregion
            };
            
            
            public static Vector2[] GetPoints(Direction direction, InputDirectionType inputDirectionType)
            {
                return direction switch
                {
                    Direction.Right => inputDirectionType == InputDirectionType.Main ? pointsDirectionRight_Main : ReversePoints(pointsDirectionRight_Main),
                    Direction.Left => inputDirectionType == InputDirectionType.Main ? pointsDirectionLeft_Main : ReversePoints(pointsDirectionLeft_Main),
                    Direction.Down => inputDirectionType == InputDirectionType.Main ? pointsDirectionDown_Main : ReversePoints(pointsDirectionDown_Main),
                    Direction.Up => inputDirectionType == InputDirectionType.Main ? pointsDirectionUp_Main : ReversePoints(pointsDirectionUp_Main),

                    Direction.UpLeft => inputDirectionType == InputDirectionType.Main ? ReversePoints(pointsDirectionUpLeft_Mirror) : pointsDirectionUpLeft_Mirror,
                    Direction.DownLeft => inputDirectionType == InputDirectionType.Main ? pointsDirectionDownLeft_Main : ReversePoints(pointsDirectionDownLeft_Main),
                    Direction.UpRight => inputDirectionType == InputDirectionType.Main ? ReversePoints(pointsDirectionUpRight_Mirror) : pointsDirectionUpRight_Mirror,
                    Direction.DownRight => inputDirectionType == InputDirectionType.Main ? pointsDirectionDownRight_Main : ReversePoints(pointsDirectionDownRight_Main),

                    _ => throw new System.NotImplementedException()
                };

            }
        }


    }
}
