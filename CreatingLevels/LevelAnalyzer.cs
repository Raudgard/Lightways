using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using Fields;
using UnityEngine.Rendering.Universal;

namespace UserLevels
{
    /// <summary>
    /// Анализирует текущую матрицу. Возвращает количество победных путей.
    /// </summary>
    public class LevelAnalyzer : MonoBehaviour
    {
        [SerializeField] private CreatingLevelBar creatingLevelBar;
        [SerializeField] private TextMeshProUGUI analyzingText;
        
        [Space]

        [Tooltip("Интенсивность служебного света.")]
        [SerializeField] float serviceLightIntensity;
        [Tooltip("Радиус служебного света.")]
        [SerializeField] float serviceLightRadius;

        [Space]

        [Tooltip("Процент содержания объектов в матрице от общего числа клеток для добавления одной звезды в награду")]
        [Range(0, 100)]
        [SerializeField] int percentMatrixObjectsForAddingOneStar;

        [Tooltip("Процент содержания объектов в матрице от общего числа клеток для добавления еще одной звезды в награду")]
        [Range(0, 100)]
        [SerializeField] int percentMatrixObjectsForAddingOneMoreStar;

        [Tooltip("Минимальный размер уровня по Х для добавления звезды в награду (в сочетании с процентом содержания объектов)")]
        [Range(5, 12)]
        [SerializeField] int minSizeOfLevelForStar;

        [Tooltip("Процент минимального количества сфер в наименьшем по длине победном пути по отношению к короткой стороне матрицы для добавления звезды в награду.")]
        [Range(0, 200)]
        [SerializeField] int percentOfWinWaySpheres;

        [Tooltip("Коэффициент, на который умножается количество сфер для получения минимально необходимого количества стрелок.")]
        [SerializeField] float arrowsNeededCoef;

        [Tooltip("Процент содержания Телепортов и искривлений от Черных дыр в матрице от общего числа клеток для добавления одной звезды в награду.")]
        [SerializeField] float teleportsAndBHInfCoef;


        private Matrix matrix;
        /// <summary>
        /// Количество победных путей в текущей матрице.
        /// </summary>
        public int WinWayCount { get; private set; } = 0;

        public List<TowerInfo[]> WinWaySpheres { get; private set; } = new List<TowerInfo[]>();

        public uint StarsReward { get; private set; } = 1;


        private bool isAnalyzing = false;
        /// <summary>
        /// Анализатор в процессе анализа?
        /// </summary>
        public bool IsAnalyzing => isAnalyzing;

        private int unitsAnalizedCount { get; set; } = 0;
        private int directionsAnalizedCount { get; set; } = 0;

        /// <summary>
        /// Текущая анализируемая последовательность объектов в матрице. Используется во время анализа.
        /// </summary>
        private Stack<ObjectInfo> unitsInfo;
        //private List<MatrixUnit> winWayObjects;
        private List<ObjectInfo[]> winWayObjects;

        //Словарь объектов в матрице и их "служебного света", зажигаемого при проведении анализа.
        private Dictionary<PlayObject, Light2D> objLightPairs = new Dictionary<PlayObject, Light2D>();


        private Coroutine analyzingCoroutine = null;


        public void Analyze(bool inDarkness)
        {
            Debug.Log("Analyze");
            isAnalyzing = true;
            matrix = Matrix.Instance;

            WinWayCount = 0;
            unitsAnalizedCount = 0;
            directionsAnalizedCount = 0;
            unitsInfo = new Stack<ObjectInfo>();
            //winWayObjects = new List<MatrixUnit>();
            winWayObjects = new List<ObjectInfo[]>();


            analyzingCoroutine = StartCoroutine(Analyzing(inDarkness));
        }


        private IEnumerator Analyzing(bool inDarkness)
        {
            creatingLevelBar.gameObject.SetActive(true);
            analyzingText.gameObject.SetActive(true);

            var spheresFromMatrix = new List<TowerInfo>(matrix.GetAllTowers());
            int unitsCount = spheresFromMatrix.Count + matrix.TeleportsForAnalyzing.Count;
            var startSphere = spheresFromMatrix.Single(t => t.winningWayIndex == 0);
            var copyOfStartSphere = TowerInfo.Copy(startSphere);
            ObjectInfo startSphereState = new ObjectInfo(null, copyOfStartSphere, new BeamSearchData() { influencesBH = new List<BlackHoleInfluenceData>() });
            unitsInfo.Push(startSphereState);


            //
            var towerMatrix = TowersMatrix.Instance;
            var objectsInMatrix = towerMatrix.ObjectsInMatrix;
            objLightPairs.Clear();
            objLightPairs = new Dictionary<PlayObject, Light2D>();
            foreach (var o in objectsInMatrix)
            {
                if (o != null)
                {
                    var lightGO = o.GetComponentsInChildren<Light2D>(true).Select(l => l.gameObject).SingleOrDefault(g => g.CompareTag("develop light"));
                    if (lightGO == null)
                    {
                        var go = new GameObject("develop light");
                        go.transform.SetParent(o.transform);
                        go.transform.localPosition = Vector3.zero;
                        go.tag = "develop light";

                        var light = go.AddComponent<Light2D>();
                        light.lightType = Light2D.LightType.Point;
                        light.intensity = serviceLightIntensity;
                        light.pointLightInnerRadius = 0;
                        light.pointLightOuterRadius = serviceLightRadius;
                        light.pointLightInnerAngle = light.pointLightOuterAngle = 360;
                        light.blendStyleIndex = 1;
                        light.enabled = false;

                        objLightPairs.Add(o, light);
                    }
                    else
                    {
                        objLightPairs.Add(o, lightGO.GetComponent<Light2D>());
                    }
                }
            }



            while (unitsInfo.Count > 0)
            {
                creatingLevelBar.SetValues(unitsAnalizedCount, unitsCount);
                var currentUnit = unitsInfo.Peek();

                //Debug.Log($"currentUnit = unitsInfo.Peek(): {currentUnit}");

                if (currentUnit.DirectionTeleports.outputTeleports.Count > 0)
                {
                    var newCurrentUnit = new ObjectInfo(currentUnit, currentUnit.DirectionTeleports.outputTeleports[0], currentUnit.DirectionTeleports.OutputDirection, currentUnit.DirectionTeleports.BeamSearchData);
                    unitsInfo.Push(newCurrentUnit);
                    currentUnit = newCurrentUnit;
                    //Debug.Log($"nitsInfo.Push(newCurrentUnit): {newCurrentUnit}");
                }

                if (currentUnit.outputDirections.Count == 0)
                {
                    //Debug.Log($"currentUnit: {currentUnit}, currentUnit directions count = 0");
                    var unitPop = unitsInfo.Pop();
                    //Debug.Log($"unitsInfo.Pop(): {unitPop}");

                    unitsAnalizedCount++;
                    HighlightAnalyzingUnits();
                    yield return null;
                    continue;
                }

                //Debug.Log($"currentUnit: {currentUnit}, direction: {currentUnit.outputDirections[0]}");

                //
                HighlightAnalyzingUnits();

                CheckDirection(currentUnit, currentUnit.outputDirections[0]);
                yield return null;
            }

            GetWinWaySpheres();
            AnalyzeForReward(inDarkness);
            creatingLevelBar.SetValues(unitsCount, unitsCount);
            yield return null;

            Debug.Log($"Analyzing end!");
            Debug.Log($"WinWayCount: {WinWayCount}, unitsAnalizedCount: {unitsAnalizedCount}, directionsAnalizedCount: {directionsAnalizedCount}");

            SwitchOffAllServiceLight();

            for (int i = 0; i < winWayObjects.Count; i++)
            {
                string winWayCoordinates = "winWayCoordinates: ";
                for (int j = 0; j < winWayObjects[i].Length; j++)
                {
                    winWayCoordinates += $"({winWayObjects[i][j].Unit.X}, {winWayObjects[i][j].Unit.Y}), ";
                }
                //winWayCoordinates += $"({winWayObjects[i].X}, {winWayObjects[i].Y}), ";
                winWayCoordinates = winWayCoordinates.Remove(winWayCoordinates.Length - 1);
                Debug.Log(winWayCoordinates);
            }
            analyzingText.gameObject.SetActive(false);
            creatingLevelBar.gameObject.SetActive(false);

            isAnalyzing = false;

            //подсвечиваем анализируемую в данный момент цепочку объектов unitsInfo
            void HighlightAnalyzingUnits()
            {
                var coordinates = unitsInfo.Select(u => (u.Unit.X, u.Unit.Y));
                foreach (var o in objectsInMatrix)
                {
                    var c = ((int)o.transform.position.x, (int)o.transform.position.y);
                    if (coordinates.AsParallel().Contains(c))
                    {
                        objLightPairs[o].enabled = true;
                    }
                    else
                    {
                        objLightPairs[o].enabled = false;
                    }
                }
            }
        }


        private void CheckDirection(ObjectInfo objectInfo, Direction direction)
        {
            var matrixObject = matrix.BeamCast(objectInfo.Unit.X, objectInfo.Unit.Y, direction, out BeamSearchData beamSearchData);
            //Debug.Log($"hittent matrixObject: {matrixObject}");
            //Debug.Log($"Remove current direction: {direction}");
            objectInfo.outputDirections.Remove(direction);
            directionsAnalizedCount++;

            if (objectInfo.Unit is TeleportInfo outputTeleport)
            {

                if (!objectInfo.PreviousObjectInfo.DirectionTeleports.outputTeleports.Remove(outputTeleport))
                    Debug.LogError($"CANNOT Remove teleport: {outputTeleport}");
                //else Debug.Log($"Remove teleport: {outputTeleport}");
            }

            switch (matrixObject)
            {
                case null:
                case BlackHoleInfo:

                    break;

                case TowerInfo:
                    var hittenSphere = (TowerInfo)matrixObject;

                    var units = unitsInfo.Select(u => u.Unit);
                    if (Calculations.ContainsSameCoordinates(units, matrixObject))
                    {
                        //Debug.LogWarning($"Sphere already in List!");

                        //string _units = "units: ";
                        //foreach (var u in units)
                        //    _units += $"({u.X}, {u.Y})";

                        //Debug.Log($"_units: {_units}, matrixObject: {matrixObject}");
                        return;
                    }

                    if (hittenSphere.directions.Contains(beamSearchData.endDirection.Opposite()))
                    {
                        //Debug.Log($"Луч попадает во входящее направление сферы {hittenSphere}");
                    }
                    else
                    {
                        if (hittenSphere.isFinish)
                        {
                            //Debug.Log($"Reach final Sphere!");
                            WinWayCount++;
                            //var _winWayObjects = unitsInfo.Select(s => s.Unit).ToList();
                            var _winWayObjects = new List<ObjectInfo>(unitsInfo);
                            _winWayObjects.Reverse();
                            ObjectInfo finishSphereObject = new ObjectInfo(objectInfo, TowerInfo.Copy(hittenSphere), beamSearchData);
                            _winWayObjects.Add(finishSphereObject);
                            var winway = _winWayObjects.ToArray();
                            winWayObjects.Add(winway);
                            //winWayObjects.Reverse();
                            //winWayObjects.Add(TowerInfo.Copy(hittenSphere));
                            unitsAnalizedCount = unitsInfo.Count;
                        }
                        else
                        {
                            var copySphere = TowerInfo.Copy(hittenSphere);
                            ObjectInfo nextSphereState = new ObjectInfo(objectInfo, copySphere, beamSearchData);
                            unitsInfo.Push(nextSphereState);

                        }
                    }
                    break;

                case TeleportInfo:
                    var inputTeleport = (TeleportInfo)matrixObject;
                    var outputTeleports = GetOutputTeleports(inputTeleport, beamSearchData.endDirection.Opposite());

                    //foreach (var t in outputTeleports)
                    //    Debug.Log($"output teleport: {t}");


                    objectInfo.DirectionTeleports = new DirectionTeleports(objectInfo, beamSearchData.endDirection, outputTeleports, beamSearchData);
                    break;

                default: Debug.LogError("Error when analysing! unaccounted object!");
                    break;


            }

        }


        private List<TeleportInfo> GetOutputTeleports(TeleportInfo inputTeleport, Direction inputDirection)
        {
            //Debug.Log($"inputDirection: {inputDirection}");
            //var teleports = TowersMatrix.Instance.Teleports.ToList();
            //Debug.Log($"matrix.Teleports.Count: {matrix.TeleportsForAnalyzing.Count}");

            var teleports = new List<TeleportInfo>(matrix.TeleportsForAnalyzing);
            //Debug.Log($"1. teleports.Count: {teleports.Count}");
            teleports.Remove(inputTeleport);
            //Debug.Log($"2. teleports.Count: {teleports.Count}");
            var result = new List<TeleportInfo>();

            foreach (var t in teleports)
            {
                var oppositeInputDirection = inputDirection.Opposite();
                //PlayObject hittenObject = TowersMatrix.Instance.GetNextObject(t.transform.position, oppositeInputDirection, out var BHData);
                MatrixUnit hittenObject = matrix.BeamCast(t.X, t.Y, oppositeInputDirection, out var beamData);


                if ((!(hittenObject is TeleportInfo) || (beamData.influencesBH.Count > 0 && beamData.influencesBH.LastOrDefault().directionAfter != oppositeInputDirection)) && (t != inputTeleport))
                {
                    bool accepted = true;
                    var directionChecked = beamData.influencesBH.Count > 0 ? beamData.influencesBH.LastOrDefault().directionAfter.Opposite() : inputDirection;
                    if (hittenObject is TowerInfo tower && tower.directions.Contains(directionChecked))
                    {
                        accepted = false;
                    }

                    //Debug.Log($"teleport: {t}, directionChecked: {directionChecked}, oppositeInputDirection: {oppositeInputDirection}, accepted: {accepted}");

                    if (accepted)
                    {
                        result.Add(t);
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///Выключает анализаторскую подсветку всех объектов в уровне.
        /// </summary>
        private void SwitchOffAllServiceLight()
        {
            foreach (var o in objLightPairs.Values)
            {
                o.enabled = false;
            }
        }

        public void StopAnalysingButtonClick()
        {
            if (analyzingCoroutine == null) 
                return;

            StopCoroutine(analyzingCoroutine);
            analyzingText.gameObject.SetActive(false);
            creatingLevelBar.gameObject.SetActive(false);
            SwitchOffAllServiceLight();
            analyzingCoroutine = null;
            isAnalyzing = false;
        }


        /// <summary>
        /// Получает список всех цепочек победных сфер.
        /// </summary>
        /// <returns></returns>
        private void GetWinWaySpheres()
        {
            WinWaySpheres = new List<TowerInfo[]>();

            for (int i = 0; i < winWayObjects.Count; i++)
            {
                TowerInfo[] winWay = winWayObjects[i].Where(o => o.Unit is TowerInfo).Select(o => o.Unit).Cast<TowerInfo>().ToArray();
                WinWaySpheres.Add(winWay);
            }
        }


        /// <summary>
        /// Анализирует текущую матрицу на сложность и возвращает число звезд в качестве предлагаемой награды.
        /// </summary>
        /// <param name="isDarkness">Уровень в темноте?</param>
        private void AnalyzeForReward(bool inDarkness)
        {
            StarsReward = 1;
            if(WinWaySpheres.Count() == 0)
            {
                Debug.Log($"WinWaySpheres.Count() = 0!");
                return; 
            }

            //Размер матрицы и процент содержания общего количества объектов от размера всех клеток размера.
            int cellsCount = matrix.SizeX * matrix.SizeY;
            float percentageSpheresInMatrix = (float)matrix.ObjectsCount / cellsCount * 100;
            Debug.Log($"ObjectsCount: {matrix.ObjectsCount}, percentageInMatrix: {percentageSpheresInMatrix}");
            if (percentageSpheresInMatrix > percentMatrixObjectsForAddingOneStar && matrix.SizeX >= minSizeOfLevelForStar)
                StarsReward++;


            //Количество объектов в матрице от общего количества клеток для добавления дополнительной звезды в награду.
            if (percentageSpheresInMatrix > percentMatrixObjectsForAddingOneMoreStar)
                StarsReward++;


            //Длина минимального победного пути 
            int minLengthWinWay = 0;
            minLengthWinWay = WinWaySpheres.Select(s => s.Length).Min();
            float percentageSpheresInWinway = (float)minLengthWinWay / matrix.SizeX * 100;
            if(percentageSpheresInWinway > percentOfWinWaySpheres)
                StarsReward++;
            Debug.Log($"minLengthWinWay: {minLengthWinWay}, percentageSpheresInWinway: {percentageSpheresInWinway}");


            //Количество стрелок.
            var spheres = matrix.GetAllTowers(false);
            int neededArrows = (int)(spheres.Count * arrowsNeededCoef);
            int allArrows = spheres.Select(s => s.directions.Count).Sum();
            if (allArrows < neededArrows)
                StarsReward--;
            Debug.Log($"spheres count: {spheres.Count()}, neededArrows: {neededArrows}, allArrows: {allArrows}");



            //Количество использованных телепортов и искривлений луча от черных дыр.

            //Кол-во телепортов в каждой победной линии.
            int[] teleportsInWinWays = new int[winWayObjects.Count];
            for (int i = 0; i < winWayObjects.Count; i++)
            {
                 teleportsInWinWays[i] = winWayObjects[i].Where(w => w.Unit is TeleportInfo).Count();
                 Debug.Log($"teleportsInWinWays[{i}]: {teleportsInWinWays[i]}");
            }

            //Кол-во влияний ЧД в каждой победной линии.
            int[] blackHolesInfluencesInWinWays = new int[winWayObjects.Count];

            for (int i = 0; i < winWayObjects.Count; i++)
            {
                var inf = winWayObjects[i].Select(w => w.BeamSearchData.influencesBH).ToList();
                Debug.Log($"winWayObjects[{i}].Length: {winWayObjects[i].Length}, inf.Count: {inf.Count}");
                //inf.ForEach(d => Debug.Log($"d.Count(): {d.Count()}"));

                //inf.ForEach(d => d.ForEach(bhi => { /*if (bhi.blackHole != null) { blackHolesInfluencesInWinWays[i]++; }*/ Debug.Log($"bhi: {bhi}"); }));
                inf.ForEach(d => blackHolesInfluencesInWinWays[i]+= d.Count());

                Debug.Log($"blackHolesInfluencesInWinWays[{i}]: {blackHolesInfluencesInWinWays[i]}");
            }

            //Сумма Телепортов и влияний ЧД по каждой победной линии.
            int[] sum = new int[winWayObjects.Count];
            for(int i = 0; i < sum.Length; i++)
            {
                sum[i] = teleportsInWinWays[i] + blackHolesInfluencesInWinWays[i];
            }

            //минимальная сумма ТП и ЧД среди всех линий.
            int minTeleportsAndBHInfSum = sum.Min();
            int neededTeleportsAndBHInf = (int)(cellsCount * teleportsAndBHInfCoef);
            Debug.Log($"minTeleportsAndBHInfSum: { minTeleportsAndBHInfSum}");
            if (minTeleportsAndBHInfSum > neededTeleportsAndBHInf)
                StarsReward++;


            if (inDarkness) StarsReward++;
            if (StarsReward > 5) StarsReward = 5;
            if (StarsReward < 1) StarsReward = 1;
            Debug.Log($"StarsReward: {StarsReward}");
        }


        private class ObjectInfo
        {
            public MatrixUnit Unit { get; set; }
            public List<Direction> outputDirections;
            public DirectionTeleports DirectionTeleports { get; set; }
            public ObjectInfo PreviousObjectInfo { get; }

            /// <summary>
            /// Информация о луче, попадающем В данный объект.
            /// </summary>
            public BeamSearchData BeamSearchData { get; }

            public ObjectInfo(ObjectInfo previousObjectInfo, TowerInfo sphere, BeamSearchData beamSearchData)
            {
                PreviousObjectInfo = previousObjectInfo;
                Unit = sphere;
                outputDirections = new List<Direction>(sphere.directions);
                DirectionTeleports = new DirectionTeleports(this);
                BeamSearchData = beamSearchData;
            }

            public ObjectInfo(ObjectInfo previousObjectInfo, TeleportInfo teleport, Direction outputDirection, BeamSearchData beamSearchData)
            {
                PreviousObjectInfo = previousObjectInfo;
                Unit = teleport;
                outputDirections = new List<Direction>() { outputDirection };
                DirectionTeleports = new DirectionTeleports(this);
                BeamSearchData = beamSearchData;
            }

            public override string ToString()
            {
                string directions = "Output directions: ";
                for (int i = 0; i < outputDirections.Count; i++)
                {
                    directions += outputDirections[i] + ", ";
                }
                directions.Remove(directions.Length - 1);

                return Unit.ToString() + $"{directions}.";
            }
        }

        private class DirectionTeleports
        {
            /// <summary>
            /// Направление луча, ВЫХОДЯЩЕГО из телепортов.
            /// </summary>
            public Direction OutputDirection { get; set; }
            public List<TeleportInfo> outputTeleports;
            public ObjectInfo owner;
            public BeamSearchData BeamSearchData { get; }

            public DirectionTeleports(ObjectInfo owner, Direction outputDirection, params TeleportInfo[] outputTeleports)
            {
                OutputDirection = outputDirection;
                this.outputTeleports = outputTeleports.ToList();
                this.owner = owner;
            }

            public DirectionTeleports(ObjectInfo owner, Direction outputDirection, List<TeleportInfo> outputTeleports, BeamSearchData beamSearchData)
            {
                OutputDirection = outputDirection;
                this.outputTeleports = outputTeleports;
                this.owner = owner;
                BeamSearchData = beamSearchData;
            }

            public DirectionTeleports(ObjectInfo owner)
            {
                OutputDirection = Direction.Up;
                outputTeleports = new List<TeleportInfo>();
                this.owner = owner;
            }

        }
    }



}