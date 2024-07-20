using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Fields;
using UnityEditor;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;


namespace Saving
{
    /// <summary>
    /// Класс для сохранения и загрузки файлов уровней.
    /// </summary>
    public static class SaveLoadUtil
    {
        /// <summary>
        /// Сохраняет матрицу в файл через JSON.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        public static void SaveMatrixIntoFile(Matrix matrix, string fileName)
        {
            string path = string.Empty;
#if UNITY_EDITOR_WIN
            path = Path.Combine(Application.dataPath, "Resources", fileName + "_lvl.json");
#elif PLATFORM_ANDROID || UNITY_ANDROID
        path = Path.Combine(Application.persistentDataPath, fileName + "_lvl.json");
#endif

            string fileToSave = JsonConvert.SerializeObject(matrix);

            try
            {
                File.WriteAllText(path, fileToSave);
            }
            catch (Exception e)
            {
                Debug.LogError("Error when saving!");
                Debug.LogError($"Error message: {e.Message}");
                Debug.LogError($"Error stackTrace: {e.StackTrace}");
            }
        }


        public static Matrix LoadMatrixFromFile(string fileName)
        {
            string loadedText;
            string path = Path.Combine(Application.persistentDataPath, fileName + "_lvl.json");
            if (File.Exists(path))
            {
                loadedText = File.ReadAllText(path);
            }
            else
            {
                var loadedFile = Resources.Load(fileName + "_lvl") as TextAsset;
                loadedText = loadedFile.text;
            }

            Matrix matrix = JsonConvert.DeserializeObject<Matrix>(loadedText);
            return matrix;
        }


        /// <summary>
        /// Сохраняет уровень в текст JSON.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="directionalLightIntensity"></param>
        /// <param name="sphereLightIntensity"></param>
        /// <param name="sphereLightRange"></param>
        /// <returns></returns>
        public static string SaveLevelIntoString(Matrix matrix, float directionalLightIntensity, float sphereLightIntensity, float sphereLightRange)
        {
            Level level = new Level();
            level.matrix = matrix;
            level.directionalLightIntensity = directionalLightIntensity;
            level.sphereLightIntensity = sphereLightIntensity;
            level.sphereLightRange = sphereLightRange;
            return JsonConvert.SerializeObject(level, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
        }


        public static Level LoadLevelFromString(string level)
        {
            return JsonConvert.DeserializeObject<Level>(level, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
        }


        public static void SaveLevelIntoFile(Matrix matrix, LevelType levelType, int timeForFTLlevel, string fileName, float directionalLightIntensity, float sphereLightIntensity, float sphereLightRange)
        {
            Level level = new Level();
            level.matrix = matrix;
            level.levelType = levelType;
            level.timeForFTLlevel = timeForFTLlevel;
            level.directionalLightIntensity = directionalLightIntensity;
            level.sphereLightIntensity = sphereLightIntensity;
            level.sphereLightRange = sphereLightRange;
            SaveLevelIntoFile(level, fileName);
        }



        public static void SaveLevelIntoFile(Level level, string fileName)
        {
            string path = string.Empty;
#if UNITY_EDITOR_WIN
            //path = Path.Combine(Application.dataPath, "Resources", fileName + "_lvl.dat");
            path = Path.Combine(Application.dataPath, "StreamingAssets", fileName + "_lvl.dat");
#elif PLATFORM_ANDROID || UNITY_ANDROID
        path = Path.Combine(Application.persistentDataPath, fileName + "_lvl.dat");
#endif
            string dataToSave = JsonConvert.SerializeObject(level, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
            Debug.Log($"fileName: {fileName}, in matrix {level.matrix.ObjectsCount} objects.");

            //print("File:");
            //print(fileToSave);
            //Debug.Log($"Application.dataPath: {Application.dataPath}");
            //Debug.Log($"Application.persistentDataPath: {Application.persistentDataPath}");
            //Debug.Log($"Application.streamingAssetsPath {Application.streamingAssetsPath}");

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Create(path);
                SaveData saveData = new SaveData();
                saveData.data = dataToSave;
                saveData.directionalLightIntensity = level.directionalLightIntensity;
                saveData.sphereLightIntensity = level.sphereLightIntensity;
                saveData.sphereLightRange = level.sphereLightRange;
                bf.Serialize(file, saveData);
                file.Dispose();
                file.Close();
                Debug.Log("Game data saved!");
            }
            catch (Exception e)
            {
                Debug.LogError("Error when saving!");
                Debug.LogError($"Error message: {e.Message}");
                Debug.LogError($"Error stackTrace: {e.StackTrace}");
            }
        }


        public static List<string> GetAllUserLevelsNames()
        {
            string path = string.Empty;
#if UNITY_EDITOR_WIN
            path = Path.Combine(Application.dataPath, "Resources");
#elif PLATFORM_ANDROID || UNITY_ANDROID
        path = Path.Combine(Application.persistentDataPath);
#endif

            DirectoryInfo d = new DirectoryInfo(path);
            FileInfo[] files = d.GetFiles("*_lvl.dat", SearchOption.TopDirectoryOnly);
            var fileNamesTemp = files.Select(f => f.Name);
            var fileNames = fileNamesTemp.Except(fileNamesTemp.Where(f => !f.StartsWith("u_"))).ToList();
            for (int i = 0; i < fileNames.Count; i++)
            {
                fileNames[i] = fileNames[i].Remove(fileNames[i].Length - 8, 8).Remove(0, 2);
            }
            return fileNames;
        }


        public static Level LoadUserLevelFromFile(string fileName)
        {
            string path = string.Empty;
#if UNITY_EDITOR_WIN
            path = Path.Combine(Application.dataPath, "Resources", fileName + "_lvl.dat");
#elif PLATFORM_ANDROID || UNITY_ANDROID
        path = Path.Combine(Application.persistentDataPath, fileName + "_lvl.dat");
#endif
            //Debug.Log($"path: {path}");
            Level level = null;
            if (File.Exists(path))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(path, FileMode.Open);
                SaveData saveData = (SaveData)bf.Deserialize(file);
                
                //Debug.Log($"saveData.mat: {saveData.data}, saveData.dir: {saveData.directionalLightIntensity}, saveData.int: {saveData.sphereLightIntensity}, saveData.range: {saveData.sphereLightRange}");

                level = JsonConvert.DeserializeObject<Level>(saveData.data, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
                level.directionalLightIntensity = saveData.directionalLightIntensity;
                level.sphereLightIntensity = saveData.sphereLightIntensity;
                level.sphereLightRange = saveData.sphereLightRange;
                level.matrix.AddPointsOfBlackHolesInfluence();
                file.Dispose();
                file.Close();

                Debug.Log($"Game data loaded! path: {path}");
            }
            else
            {
                Debug.LogWarning("There is no save data!");
            }

            return level;
        }


        public static bool DeleteUserLevelFile(string fileName)
        {
            string path = string.Empty;
#if UNITY_EDITOR_WIN
            path = Path.Combine(Application.dataPath, "Resources", fileName + "_lvl.dat");
#elif PLATFORM_ANDROID || UNITY_ANDROID
        path = Path.Combine(Application.persistentDataPath, fileName + "_lvl.dat");
#endif
            if (File.Exists(path))
            {
                try
                { 
                    File.Delete(path);
                    Debug.Log($"File successfully deleted: {path}");
                    return true;
                }
                catch(Exception e)
                {
                    Debug.LogWarning($"Cannot delete file: {path}. Exception: {e}");
                    UIController.Instance.ShowInformationInMenu($"Cannot delete file: {path}. Exception: {e}");
                    return false;
                }
            }
            else
            {
                Debug.LogWarning("There is no save data!");
                return false;
            }
        }


        public static bool RenameUserLevelFile(string oldName, string newName)
        {
            string oldPath = string.Empty;
            string newPath = string.Empty;

#if UNITY_EDITOR_WIN
            oldPath = Path.Combine(Application.dataPath, "Resources", oldName + "_lvl.dat");
            newPath = Path.Combine(Application.dataPath, "Resources", newName + "_lvl.dat");
#elif PLATFORM_ANDROID || UNITY_ANDROID
            oldPath = Path.Combine(Application.persistentDataPath, oldName + "_lvl.dat");
            newPath = Path.Combine(Application.persistentDataPath, newName + "_lvl.dat");
#endif
            if (File.Exists(oldPath))
            {
                try
                {
                    File.Move(oldPath, newPath);
                    Debug.Log($"File successfully moved from: {oldPath}, to {newPath}");
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Cannot rename file: {oldPath}. Exception: {e}");
                    UIController.Instance.ShowInformationInMenu($"Cannot rename file: {oldPath}. Exception: {e}");
                    return false;
                }
            }
            else
            {
                Debug.LogWarning("There is no save data!");
                return false;
            }
        }





        public static Level LoadLevelFromFile(string fileName)
        {
            Level level = null;
            using UnityWebRequest webRequest = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, fileName + "_lvl.dat"));
            webRequest.SendWebRequest();
            var timeStart = Time.realtimeSinceStartup;
            float timePassed = 0;

            while (!webRequest.isDone)
            {
                //await Task.Delay(1);
                timePassed = Time.realtimeSinceStartup - timeStart;
                //Debug.Log($"timePassed: {timePassed}, Time.realtimeSinceStartup: {Time.realtimeSinceStartup}, timeStart: {timeStart}");
                if (timePassed > 10)
                {
                    Debug.LogError("Прошло более 10 секунд. Уровень не может быть загружен!");
                    break;
                }
            }

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("webRequestError: " + webRequest.error);
            }
            else
            {
                //Debug.Log("Received webRequest.downloadHandler.data.Length: " + webRequest.downloadHandler.data.Length);
                MemoryStream memoryStream = new MemoryStream(webRequest.downloadHandler.data);
                BinaryFormatter bf = new BinaryFormatter();
                SaveData saveData = (SaveData)bf.Deserialize(memoryStream);

                //Debug.Log($"saveData.mat: {saveData.data}, saveData.dir: {saveData.directionalLightIntensity}, saveData.int: {saveData.sphereLightIntensity}, saveData.range: {saveData.sphereLightRange}");
                level = JsonConvert.DeserializeObject<Level>(saveData.data, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
                level.directionalLightIntensity = saveData.directionalLightIntensity;
                level.sphereLightIntensity = saveData.sphereLightIntensity;
                level.sphereLightRange = saveData.sphereLightRange;
                level.matrix.AddPointsOfBlackHolesInfluence();
                memoryStream.Dispose();
                memoryStream.Close();
            }

            return level;
        }


        






/// <summary>
/// Сохраняет состояние прохождения игрока в файл.
/// </summary>
/// <param name="stateData"></param>
public static void SaveState(StateData stateData)
        {
            string path = string.Empty;
#if UNITY_EDITOR_WIN
            path = Path.Combine(Application.dataPath, "Resources", "State.dat");
#elif PLATFORM_ANDROID || UNITY_ANDROID
            path = Path.Combine(Application.persistentDataPath, "State.dat");
#endif

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Create(path);
                bf.Serialize(file, stateData);
                file.Dispose();
                file.Close();
                Debug.Log("State data saved!");
            }
            catch (Exception e)
            {
                Debug.LogError("Error when saving state data!");
                Debug.LogError($"Error message: {e.Message}");
                Debug.LogError($"Error stackTrace: {e.StackTrace}");
            }
        }


        /// <summary>
        /// Загружает состояние прохождения из файла.
        /// </summary>
        /// <returns></returns>
        public static StateData LoadState()
        {
            string path = string.Empty;
#if UNITY_EDITOR_WIN
            path = Path.Combine(Application.dataPath, "Resources", "State.dat");
#elif PLATFORM_ANDROID || UNITY_ANDROID
            path = Path.Combine(Application.persistentDataPath, "State.dat");
#endif
            StateData stateData;
            if (File.Exists(path))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(path, FileMode.Open);
                stateData = (StateData)bf.Deserialize(file);
                if (stateData.achievementSaveDatas == null)
                    stateData.achievementSaveDatas = new List<AchievementSaveData>();

                //Debug.Log($"saveData._UserId: {stateData._UserId}");

                file.Dispose();
                file.Close();

                Debug.Log("State data loaded!");
            }
            else
            {
                Debug.LogWarning("There is no save State data! Create new StateData object.");
                stateData = new StateData();
            }

            return stateData;
        }





        public static Dictionary<string, Matrix> LoadAllMatrix()
        {
            string path = Path.Combine(Application.dataPath, "Resources");
            string[] fileNames = Directory.GetFiles(path);
            Dictionary<string, Matrix> matrices = new Dictionary<string, Matrix>();

            foreach (string fileName in fileNames)
            {
                if (fileName.EndsWith("json"))
                {
                    string loadedText = File.ReadAllText(fileName);
                    Matrix matrix = JsonConvert.DeserializeObject<Matrix>(loadedText);
                    //Debug.Log($"matrix towers count: {matrix.TowersCount}");

                    matrices.Add(fileName, matrix);
                }

                if (fileName.EndsWith("dat"))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream file = File.Open(fileName, FileMode.Open);
                    SaveData saveData = (SaveData)bf.Deserialize(file);
                    Matrix matrix = JsonConvert.DeserializeObject<Matrix>(saveData.data);

                    matrices.Add(fileName, matrix);
                    file.Dispose();
                    file.Close();
                }
            }

            return matrices;
        }


        public static Dictionary<string, Level> ConvertMatrixToLevelsAndSave(Dictionary<string, Matrix> matrices)
        {
            Dictionary<string, Level> levels = new Dictionary<string, Level>();
            Level level = new Level();

            foreach (var matrix in matrices)
            {
                level.matrix = matrix.Value;
                level.directionalLightIntensity = 1f;
                level.sphereLightIntensity = 2;
                level.sphereLightRange = 6;
                string fileName = matrix.Key.Remove(matrix.Key.Length - 9);
                var chapters = fileName.Split('/');
                chapters = chapters.Last().Split('\\');

                fileName = chapters.Last();
                Debug.Log($"fileName: {fileName}, matrix: {level.matrix.TowersCount}");
                levels.Add(fileName, level);
                SaveLevelIntoFile(level, fileName);
            }
            return levels;
        }



        /// <summary>
        /// Устанавливает параметры для всех уровней.
        /// </summary>
        public static void SetIntensityForAllLevels()
        {
            string[] levelsNames = new string[GameController.Instance.Settings.levelsCountInEachDifficulty];
            int max = GameController.Instance.Settings.levelsCountInEachDifficulty + 1;
            for (int i = 1; i < max; i++)
            {
                levelsNames[i - 1] = $"1_{i}";
                Level level = LoadLevelFromFile(levelsNames[i - 1]);
                level.directionalLightIntensity = 1f;
                level.sphereLightIntensity = 2;
                level.sphereLightRange = 6;
                SaveLevelIntoFile(level, levelsNames[i - 1]);
            }
        }




        /// <summary>
        /// Одноразовый. Переджейсонил файлы уровней, так как перешел от TowerInfo к MatrixUnit в матрице.
        /// </summary>
        //    public static void ConvertJSON()
        //    {
        //        for (int i = 1; i < 101; i++)
        //        {
        //            string levelname = $"1_{i}";

        //            string path = string.Empty;
        //#if UNITY_EDITOR_WIN
        //            path = Path.Combine(Application.dataPath, "Resources", levelname + "_lvl.dat");
        //#elif PLATFORM_ANDROID || UNITY_ANDROID
        //        path = Path.Combine(Application.persistentDataPath, fileName + "_lvl.dat");
        //#endif

        //            Level2 level = null;
        //            Level levelRight = new Level();

        //            if (File.Exists(path))
        //            {
        //                BinaryFormatter bf = new BinaryFormatter();
        //                FileStream file = File.Open(path, FileMode.Open);
        //                SaveData saveData = (SaveData)bf.Deserialize(file);
        //                Debug.Log($"saveData.mat: {saveData.data}, saveData.dir: {saveData.directionalLightIntensity}, saveData.int: {saveData.sphereLightIntensity}, saveData.range: {saveData.sphereLightRange}");

        //                level = JsonConvert.DeserializeObject<Level2>(saveData.data);
        //                level.directionalLightIntensity = saveData.directionalLightIntensity;
        //                level.sphereLightIntensity = saveData.sphereLightIntensity;
        //                level.sphereLightRange = saveData.sphereLightRange;
        //                file.Dispose();
        //                file.Close();

        //                Debug.Log($"level.matrix.matrix.Length: {level.matrix.matrix.Length}");



        //                Matrix matrix = new Matrix(level.matrix.matrix.GetLength(0), level.matrix.matrix.GetLength(1));
        //                for(int j = 0; j < matrix.GetLength(0); j++)
        //                {
        //                    for(int k = 0; k < matrix.GetLength(1); k++)
        //                    {
        //                        if(level.matrix.matrix[j, k] != null)
        //                            matrix.AddTower(level.matrix.matrix[j, k]);
        //                    }
        //                }

        //                levelRight.matrix = matrix;
        //                levelRight.directionalLightIntensity = level.directionalLightIntensity;
        //                levelRight.sphereLightIntensity = level.sphereLightIntensity;
        //                levelRight.sphereLightRange = level.sphereLightRange;



        //                Debug.Log("Game data loaded!");
        //            }
        //            else
        //            {
        //                Debug.LogError("There is no save data!");
        //            }

        //            SaveLevelIntoFile(levelRight, levelname);
        //        }
        //    }
    }

    public class Level2
    {
        public Matrix2 matrix;

        [JsonIgnore]
        public float directionalLightIntensity;
        [JsonIgnore]
        public float sphereLightIntensity;
        [JsonIgnore]
        public float sphereLightRange;
    }


    [Serializable]
    public class Matrix2
    {
        [JsonRequired]
        public TowerInfo[,] matrix;
    }



}