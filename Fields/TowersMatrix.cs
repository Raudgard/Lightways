using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System;
using Saving;

namespace Fields
{
    public class TowersMatrix : MonoBehaviour
    {
        #region Singleton
        private static TowersMatrix instance;

        public static TowersMatrix Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        [SerializeField] private Matrix matrix;

        public Matrix Matrix { get { return matrix; } set { matrix = value; } }
        public AutomaticLevelCreator automaticLevelCreator;
        public FieldBackground fieldBackground;
        public int attemptsToCreateMatrix;

        [Space]
        [Header("Переменные, необходимые для сохранения файла из инспектора.")]

        public LevelType levelType;
        public int timeForFTLLevel;

        [Space]

        [Tooltip("Размеры матрицы по горизонтали.")]
        public int sizeX;
        [Tooltip("Размеры матрицы по вертикали. Предпочтительно (2 * sizeX).")]
        public int sizeY;

        [Space]

        //[SerializeField] private ClickController clickController;
        [SerializeField] private InputHandlers inputHandlers;
        [SerializeField] private GameController gameController;
        [SerializeField] private Prefabs prefabs;

        [Space]

        [SerializeField] private GameObject TOWERS_HUB;
        [SerializeField] private GameObject TELEPORTS_HUB;
        [SerializeField] private GameObject BLACKHOLES_HUB;




        /// <summary>
        /// Список объектов в активной матрице. Равен списку объектов, фактически присутствующих на поле.
        /// </summary>
        public PlayObject[] ObjectsInMatrix
        {
            get
            {
                var spheres = Spheres as PlayObject[];
                var teleports = Teleports as PlayObject[];
                var blackHoles = BlackHoles as PlayObject[];
                return spheres.Union(teleports).Union(blackHoles).ToArray();
            }

        }


        [Tooltip("В процессе создания матрицы.")]
        public bool isCreating = false;

        [HideInInspector]
        public bool vizualizeCoordinates = false;

        /// <summary>
        /// Сферы, практически присутствующие в сцене.
        /// </summary>
        public Tower[] Spheres => TOWERS_HUB.GetComponentsInChildren<Tower>().ToArray();

        /// <summary>
        /// Телепорты, практически присутствующие в сцене.
        /// </summary>
        public Teleport[] Teleports => TELEPORTS_HUB.GetComponentsInChildren<Teleport>().ToArray();
        /// <summary>
        /// Черные дыры, практически присутствующие в сцене.
        /// </summary>
        public BlackHole[] BlackHoles => BLACKHOLES_HUB.GetComponentsInChildren<BlackHole>(true).ToArray();








        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                //DontDestroyOnLoad(gameObject);
            }
            else Destroy(this);
        }

        private void Start()
        {
#if UNITY_EDITOR
            if (matrix == null)
                GetMatrixFromScreen();
#endif
        }


        /// <summary>
        /// Создает матрицу в соответствии с размерами sizeX и sizeY и заполняет ее объектами с экрана.
        /// </summary>
        public void GetMatrixFromScreen()
        {
            matrix = new Matrix(sizeX, sizeY);
            //Debug.Log("GetMatrixFromScreen 1");
            //objectsInMatrix = new Tower[0];
            //objectsInMatrix = new List<GameObject>();


            //ObjectsInMatrix = new List<PlayObject>();


            Tower[] towers = TOWERS_HUB.GetComponentsInChildren<Tower>();
            //Debug.Log($"GetMatrixFromScreen 2. towers.Length: {towers.Length}");

            for (int i = 0; i < towers.Length; i++)
            {
                TowerInfo towerInfo = matrix.AddTower((int)towers[i].transform.localPosition.x, (int)towers[i].transform.localPosition.y, towers[i].winningWayIndex, towers[i].isFinish);
                towerInfo.directions = towers[i].directions.ToList();
                towerInfo.inputDirections = towers[i].inputDirections.ToList();
                towerInfo.isFinish = towers[i].isFinish;
                towerInfo.winningWayIndex = towers[i].winningWayIndex;
                //ObjectsInMatrix.Add(towers[i].playObject);
            }
            //Debug.Log($"GetMatrixFromScreen 3");

            Teleport[] teleports = TELEPORTS_HUB.GetComponentsInChildren<Teleport>();
            //Debug.Log($"GetMatrixFromScreen 4. teleports.Length: {teleports.Length}");

            for (int i = 0; i < teleports.Length; i++)
            {
                matrix.AddTeleport((int)teleports[i].transform.localPosition.x, (int)teleports[i].transform.localPosition.y);
                //ObjectsInMatrix.Add(teleports[i].playObject);
            }
            //Debug.Log($"GetMatrixFromScreen 5");

            BlackHole[] blackHoles = BLACKHOLES_HUB.GetComponentsInChildren<BlackHole>();
            //Debug.Log($"GetMatrixFromScreen 6");

            for (int i = 0; i < blackHoles.Length; i++)
            {
                matrix.AddBlackHole((int)blackHoles[i].transform.localPosition.x, (int)blackHoles[i].transform.localPosition.y, blackHoles[i].size);
                //ObjectsInMatrix.Add(blackHoles[i].playObject);
            }
            //Debug.Log($"GetMatrixFromScreen 7");

            //ShowMatrixInLog();
        }


        /// <summary>
        /// Вывести все данные о текущей матрице в лог.
        /// </summary>
        private void ShowMatrixInLog()
        {
            for (int i = 0; i < matrix.SizeX; i++)
            {
                for (int k = 0; k < matrix.SizeY; k++)
                {
                    string mes = $"({i}, {k}): ";
                    if (matrix[i, k] != null)
                    {
                        if(matrix[i, k] is TowerInfo)
                        {
                            mes += "Sphere => ";
                            foreach (var dir in (TowerInfo)matrix[i, k])
                            {
                                mes += dir + ", ";
                            }
                        }
                        else if(matrix[i, k] is TeleportInfo)
                        {
                            mes += "Teleport ";
                        }
                        else if(matrix[i, k] is BlackHoleInfo)
                        {
                            mes += $"Black Hole {((BlackHoleInfo)matrix[i,k]).size}";
                        }
                        
                    }
                    else mes += "null";
                    print(mes);

                }
            }
        }

        /// <summary>
        /// Возвращает ближайший объект, который находится на пути по указанному направлению от этой башни.
        /// </summary>
        /// <param name="positionFrom">Позиция башни, откуда исходит поиск.</param>
        /// <param name="direction">Направление поиска.</param>
        /// <returns>Ближайший объект по заданному направлению из заданной точки. Null - если объекта на пути нет.</returns>
        public PlayObject GetNextObject(Vector2 positionFrom, Direction direction, out List<BlackHoleInfluenceData> blackHolesData)
        {
            MatrixUnit unitOnTheWay = matrix.BeamCast((int)positionFrom.x, (int)positionFrom.y, direction, out var beam);
            blackHolesData = beam.influencesBH;
            if (unitOnTheWay == null) return null;

            PlayObject PO = ObjectsInMatrix.Where(obj => obj.transform.position.x == unitOnTheWay.X && obj.transform.position.y == unitOnTheWay.Y).FirstOrDefault();

            if (PO == null)
            {
                Debug.LogError("Объект не найден!");
            }
            return PO;
        }

        public void SaveLevelIntoFile(string fileName, LevelType levelType, int timeForFTLlevel)
        {
            //SaveLoadUtil.SaveMatrixIntoFile(matrix, fileName);
            SaveLoadUtil.SaveLevelIntoFile(matrix, levelType, timeForFTLlevel, fileName, gameController.DirectionalLightIntensity, gameController.Settings.sphereLightIntensity, gameController.Settings.sphereLightRange);
        }

        public IEnumerator LoadLevelFromFile(string fileName)
        {
            //Matrix matrix = SaveLoadUtil.LoadMatrixFromFile(fileName);

            Level level = SaveLoadUtil.LoadLevelFromFile(fileName);
            Debug.Log($"level type: {level.levelType}, time for FTL: {level.timeForFTLlevel}");


            if (level.matrix == null) 
                yield break;

            gameController.currentLevel = level;
            matrix = level.matrix;
            gameController.SetDirectionalLightIntensity(level.directionalLightIntensity);
            gameController.Settings.sphereLightIntensity = level.sphereLightIntensity;

            //gameController.Settings.sphereLightRange = level.sphereLightRange;
            yield return StartCoroutine(CreatePlayObjectsFromMatrix());

        }

        /// <summary>
        /// Создает матрицу с помощью AutomaticLevelCreator
        /// </summary>
        public void CreateMatrix()
        {
            StartCoroutine(CreatingMatrix());
        }


        private IEnumerator CreatingMatrix()
        {
            isCreating = true;

            if (GameController.Instance != null)
                GameController.Instance.ClearHighlightedTowers();

            //ObjectsInMatrix = new List<PlayObject>();
            UIController.Instance.ActivateCreatingLevelBar(true);
            automaticLevelCreator.matrix = null;
            int attempts = attemptsToCreateMatrix;

            while (attempts > 0 && automaticLevelCreator.matrix == null)
            {
                yield return StartCoroutine(automaticLevelCreator.CreateMatrix(this));
                attempts--;
            }
            //Matrix matrix = automaticLevelCreator.CreateMatrix(this);
            if (automaticLevelCreator.matrix == null)
            {
                print("Матрица в automaticLevelCreator равна нулю.");
                gameController.AddKeys(1);
                UIController.Instance.OnCannotCreateLevel();
                yield break;
            }
            matrix = automaticLevelCreator.matrix;
            UIController.Instance.ActivateCreatingLevelBar(false);

            print("Метод создания матрицы прошел успешно до конца.");

            yield return StartCoroutine(CreatePlayObjectsFromMatrix());
            VizualizeCoordinatesForAllPlayObjects(vizualizeCoordinates);
            isCreating = false;
            inputHandlers.OnLevelLoad();

            //fieldBackground.CreateLines();
        }


        /// <summary>
        /// Создает и обновляет игровые объекты на поле в соответствии с матрицей (загруженной или созданной).
        /// </summary>
        public IEnumerator CreatePlayObjectsFromMatrix()
        {
            if(GameController.Instance != null)
                GameController.Instance.ClearHighlightedTowers();

            sizeX = matrix.SizeX;
            sizeY = matrix.SizeY;

            gameController.finishTowers.Clear();

            //ObjectsInMatrix = new List<PlayObject>();

            yield return StartCoroutine(ClearHUBs());

            fieldBackground.CreateBackgroundAndLines(sizeX, sizeY);
            //List<PlayObject> POs = new List<PlayObject>();

            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    if (matrix[i, j] is TowerInfo)
                    {
                        Tower tower = CreateTower(i, j, (TowerInfo)matrix[i, j]);
                        //POs.Add(tower.playObject);
                        tower.transform.SetParent(TOWERS_HUB.transform);
                    }
                    else if (matrix[i, j] is TeleportInfo)
                    {
                        Teleport teleport = CreateTeleport(i, j, (TeleportInfo)matrix[i, j]);
                        //POs.Add(teleport.playObject);
                        teleport.transform.SetParent(TELEPORTS_HUB.transform);
                    }
                    else if (matrix[i, j] is BlackHoleInfo)
                    {
                        BlackHole blackHole = CreateBlackHole(i, j, (BlackHoleInfo)matrix[i, j]);
                        //POs.Add(blackHole.playObject);
                        blackHole.transform.SetParent(BLACKHOLES_HUB.transform);
                    }
                }
            }

            //POs.Sort((x, y) => x.name.CompareTo(y.name));
            //ObjectsInMatrix = POs;
        }


        /// <summary>
        /// Создает отдельный игровой объект.
        /// </summary>
        public T CreatePlayObject<T>(MatrixUnit unit) where T: PlayObject
        {
            if(unit == null)
            {
                //Debug.LogError("Попытка создать пустой объект!");
                throw new Exception("Попытка создать пустой объект!");
                //return default;
            }

            if ((unit is TowerInfo && typeof(T) != typeof(Tower)) ||
                (unit is TeleportInfo && typeof(T) != typeof(Teleport)) ||
                (unit is BlackHoleInfo && typeof(T) != typeof(BlackHole)))
            {
                throw new Exception("Matrix unit не соответствует создаваемому PlayObject.");
            }



            if (unit is TowerInfo towerInfo)
            {
                Tower tower = CreateTower(unit.X, unit.Y, towerInfo);
                //ObjectsInMatrix.Add(tower.playObject);
                tower.transform.SetParent(TOWERS_HUB.transform);
                return tower as T;
            }
            else if (unit is TeleportInfo teleportInfo)
            {
                Teleport teleport = CreateTeleport(unit.X, unit.Y, teleportInfo);
                //ObjectsInMatrix.Add(teleport.playObject);
                teleport.transform.SetParent(TELEPORTS_HUB.transform);
                return teleport as T;
            }
            else if (unit is BlackHoleInfo blackHoleInfo)
            {
                BlackHole blackHole = CreateBlackHole(unit.X, unit.Y, blackHoleInfo);
                //ObjectsInMatrix.Add(blackHole.playObject);
                blackHole.transform.SetParent(BLACKHOLES_HUB.transform);
                return blackHole as T;
            }

            throw new NotImplementedException();
        }



        private Tower CreateTower(int Xcoordinate, int Ycoordinate, TowerInfo towerInfo)
        {
            Tower tower = Instantiate(prefabs.towerPrefab);
            tower.directions = towerInfo.directions.ToArray();
            tower.inputDirections = towerInfo.inputDirections.ToArray();
            tower.winningWayIndex = towerInfo.winningWayIndex;
            tower.isFinish = towerInfo.isFinish;

            tower.LightIntensity = gameController.Settings.sphereLightIntensity;
            tower.LightRange = gameController.Settings.sphereLightRange;

            tower.transform.localPosition = new Vector3(Xcoordinate, Ycoordinate, tower.transform.localPosition.z);
            tower.CreateArrows();
            string coordinates = $"({towerInfo.X}, {towerInfo.Y})";

            
            if (tower.winningWayIndex > -1)
            {
                int winWayIndexForName = tower.winningWayIndex + 1;
                string number;
                if (winWayIndexForName < 10) number = "000" + winWayIndexForName;
                else if (winWayIndexForName < 100) number = "00" + winWayIndexForName;
                else if (winWayIndexForName < 1000) number = "0" + winWayIndexForName;
                else number = tower.winningWayIndex.ToString();
                tower.name = $"Tower WinningWay #{number} {coordinates}";

                if (tower.winningWayIndex == 0)
                {
                    tower.name += "  Start";
                }
            }
            else if (tower.winningWayIndex < -9999)
            {
                tower.name = $"Tower FalseWinWay #{tower.winningWayIndex} {coordinates}";
            }
            else
            {
                tower.name = $"Tower False {coordinates} ({tower.winningWayIndex})";
            }

            if (tower.isFinish)
            {
                SetTowerFinish(tower);
            }
            return tower;
        }

        /// <summary>
        /// Делает ее больше размером, добавляет частицы и т.д.
        /// </summary>
        /// <param name="tower"></param>
        public void SetTowerFinish(Tower tower)
        {
            ParticleSystem particles = Instantiate(prefabs.pariclesForFinishTower);
            particles.transform.SetParent(tower.transform);
            particles.transform.localPosition = new Vector3(0, 0, -1);
            particles.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            tower.finishSphereParticleSystem = particles;

            tower.transform.localScale = new Vector3(0.2f, 0.2f, 1);

            var forceFieldForFinish = Instantiate(prefabs.forceFieldForFinish);
            forceFieldForFinish.transform.SetParent(tower.transform);
            forceFieldForFinish.transform.localPosition = new Vector3(0, 0, -1);
            var particleForcesModule = particles.externalForces;
            particleForcesModule.AddInfluence(forceFieldForFinish);
            if (!gameController.finishTowers.Contains(tower))
                gameController.finishTowers.Add(tower);
            else
                Debug.Log($"Непредвиденная ошибка! Добавлена такая же финишная сфера в список финишных сфер!");
            //tower.GetComponentInChildren<Light>().intensity = gameController.lightIntensityForFinishTower;
            tower.GetComponentInChildren<Light2D>().intensity = gameController.Settings.lightIntensityForFinishTower;

            tower.SetActiveForBackgroundSmoke(false);

            tower.isFinish = true;
            tower.name += "  Finish";
        }


        private Teleport CreateTeleport(int Xcoordinate, int Ycoordinate, TeleportInfo teleportInfo)
        {
            Teleport teleport = Instantiate(prefabs.teleportPrefab);

            teleport.transform.localPosition = new Vector3(Xcoordinate, Ycoordinate, teleport.transform.localPosition.z);
            string coordinates = $"({teleportInfo.X}, {teleportInfo.Y})";
            teleport.name = $"Teleport {coordinates}";

            return teleport;
        }

        private BlackHole CreateBlackHole(int Xcoordinate, int Ycoordinate, BlackHoleInfo blackHoleInfo)
        {
            BlackHole blackHole = blackHoleInfo.size switch
            {
                BlackHoleInfo.BlackHoleSize.Small => Instantiate(prefabs.blackHoleSmallPrefab),
                BlackHoleInfo.BlackHoleSize.Medium => Instantiate(prefabs.blackHoleMediumPrefab),
                BlackHoleInfo.BlackHoleSize.Large => Instantiate(prefabs.blackHoleLargePrefab),
                BlackHoleInfo.BlackHoleSize.Supermassive => Instantiate(prefabs.blackHoleSupermassivePrefab),
                _ => throw new Exception()
            };

            blackHole.transform.localPosition = new Vector3(Xcoordinate, Ycoordinate, blackHole.transform.localPosition.z);
            string coordinates = $"({blackHoleInfo.X}, {blackHoleInfo.Y})";
            blackHole.name = $"Black hole {coordinates}";
            blackHole.size = blackHoleInfo.size;

            return blackHole;
        }



        /// <summary>
        /// Удаляет все игровые объекты из TOWERS_HUB, TELEPORTS_HUB, BLACKHOLES_HUB.
        /// </summary>
        public IEnumerator ClearHUBs()
        {
            var towers = TOWERS_HUB.GetComponentsInChildren<Tower>();
            var teleports = TELEPORTS_HUB.GetComponentsInChildren<Teleport>();
            var blackholes = BLACKHOLES_HUB.GetComponentsInChildren<BlackHole>(true);

            var GOs = new List<GameObject>();
            GOs.AddRange(towers.Select(go => go.gameObject));
            GOs.AddRange(teleports.Select(go => go.gameObject));
            GOs.AddRange(blackholes.Select(go => go.gameObject));

            //var GOs = ObjectsInMatrix;

            foreach (var GO in GOs)
            {

                Destroy(GO);

//#if UNITY_EDITOR
//                DestroyImmediate(GO);
//#else
//                Destroy(GO);
//#endif
            }
            //Debug.Log("ClearHUBs()");
            yield return null;
            //ObjectsInMatrix.Clear();
        }

        /// <summary>
        /// Удаляет из списка и уничтожает игровой объект.
        /// </summary>
        /// <param name="playObject"></param>
        public void DeletePlayObject(PlayObject playObject)
        {
            if (playObject != null)
            {
                //if (ObjectsInMatrix.Contains(playObject))
                //    ObjectsInMatrix.Remove(playObject);

                Destroy(playObject.gameObject);
            }
            else
            {
                Debug.LogWarning("Deleting playObject = null!");
            }
        }

        /// <summary>
        /// Удаляет из списка и уничтожает игровой объект.
        /// </summary>
        public void DeletePlayObject(int X, int Y)
        {
            var playObjects = ObjectsInMatrix.Where(o => o.transform.position.x == X && o.transform.position.y == Y);
            var playObject = playObjects.SingleOrDefault();

            if (playObject != null)
            {
                //if (ObjectsInMatrix.Contains(playObject))
                //    ObjectsInMatrix.Remove(playObject);

                Destroy(playObject.gameObject);
            }
            else
            {
                Debug.LogWarning("Deleting playObject = null!");
            }
        }


        /// <summary>
        /// Подсветить победные башни. Или убрать подстветку.
        /// </summary>
        /// <param name="highlightOn"></param>
        public void HighlightWinningTowers(bool highlightOn)
        {
            Tower[] towers = TOWERS_HUB.GetComponentsInChildren<Tower>().Where(tower => tower.winningWayIndex > -1).ToArray();

            foreach (Tower tower in towers)
            {
                var lights = tower.GetComponentsInChildren<Light2D>();
                foreach(var light in lights)
                {
                    light.enabled = highlightOn;
                }
            }
        }


       

        /// <summary>
        /// Проверяет содержится ли координата в матрице.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public bool IsValidCoordinates((int X, int Y) coordinates) => coordinates.X > -1 && coordinates.X <= sizeX && coordinates.Y > -1 && coordinates.Y <= sizeY;



        /// <summary>
        /// Показывает координаты у каждого игрового объекта на поле.
        /// </summary>
        public void VizualizeCoordinatesForAllPlayObjects(bool vizualOn)
        {
            //var units = FindObjectsOfType<PlayObject>();
            var units = ObjectsInMatrix;

            for (int i = 0; i < units.Length; i++)
            {
                units[i].VisualizeCoordinates(vizualOn);
            }
        }




        private void OnDrawGizmos()
        {
            if (matrix == null || matrix.occupiedSellsWinWay == null)
            {
                return;
            }

            

            Gizmos.color = Color.red;
            matrix.occupiedSellsWinWay.ForEach(o => Gizmos.DrawSphere(new Vector3(o.X, o.Y, 0), 0.2f));


            var occupiedSellsWithoutWinWay = matrix.occupiedSellsWinWay.Except(matrix.occupiedSells).ToList();
            Gizmos.color = Color.yellow;
            occupiedSellsWithoutWinWay.ForEach(o => Gizmos.DrawSphere(new Vector3(o.X, o.Y, 0), 0.2f));



            Gizmos.color = Color.red;
            matrix.occupiedSells.ForEach(o => Gizmos.DrawSphere(new Vector3(o.X, o.Y, 0), 0.2f));


            Gizmos.color = Color.green;
            matrix.GetAllWinTowers(true).ForEach(t => Gizmos.DrawWireSphere(new Vector3(t.X, t.Y, 0), 0.3f));




            //Debug.Log($"occup sells count: {matrix.occupiedSells.Count}");



        }



    }




    

    
}
