using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CloudServices.LeaderBoardsFromPlayerIO;
using UnityEngine.UI;
using PlayerIOClient;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using TMPro;


public class LeaderboardController : MonoBehaviour
{
    [SerializeField] private GameObject leaderboardLabel;
    [SerializeField] private RectTransform leaderboardContentRT;
    [SerializeField] private RectTransform currentPlayerLeaderboardLine;
    [SerializeField] private TextMeshProUGUI announcementLabel;
    [SerializeField] private GameObject rankCurrentPlayer;
    [SerializeField] private GameObject nameCurrentPlayer;
    [SerializeField] private GameObject starsNumberCurrentPlayer;





    [Space]

    [Tooltip("ћаксимальное врем€ ожидани€ от сервера PlayerIO в секундах")]
    [SerializeField] private float maxTimeForWaiting;
    
    [Tooltip(" оличество отображаемых строк в доске лидеров")]
    [SerializeField] private int leaderboardLinesNumber;

    [Tooltip("»нтервальна€ позици€ между строками")]
    [SerializeField] private float intervalPositionBetweenLines;





    
    private Client client = null;
    private DatabaseObject[] database = null;
    private DatabaseObject currentPlayerDatabaseObject = null;
    /// <summary>
    /// ¬рем€, ушедшее на все запросы.
    /// </summary>
    private float estimatedTime = 0;
    /// <summary>
    /// Ѕлагополучное сохранение в базе.
    /// </summary>
    private bool isSuccessfulSave = false;
    private bool isErrorOccured = false;

    private bool IsFirstTime
    {
        get => PlayerPrefs.GetInt(Saving.SCFPP.Miscellaneous.isFirstTimeToGetDB, 1) > 0;
        set => PlayerPrefs.SetInt(Saving.SCFPP.Miscellaneous.isFirstTimeToGetDB, value ? 1 : 0);
    }

    private IEnumerator Start()
    {
        //IsFirstTime = true;
        Debug.Log($"IsFirstTime: {IsFirstTime}");

        while (!GameController.Instance.isStateLoaded)
        {
            yield return null;
        }

        GameController.Instance.colorTheme.GetActiveColors(out _, out _, out _, out Sprite backgroundSprite);
        leaderboardLabel.GetComponent<Image>().sprite = backgroundSprite;
       
        if (client == null)
        {
            Debug.Log($"client = null. Try to auth");
            AuthenticationPlayerIO.AuthenticationUser(GameController.Instance.StateData.UserId, OnSuccessAuth, OnError);
        }

        float _estTime = 0;
        while(client == null && _estTime < maxTimeForWaiting)
        {
            yield return null;
            _estTime += Time.deltaTime;
        }

        if (client != null && IsFirstTime)
        {
            StartCoroutine(FirstTimeEnterGame());
        }


        LeaderboardButtonClick();

        //StartCoroutine(AddRandomEntrys(200));
    }



    private IEnumerator FirstTimeEnterGame()
    {
        yield return StartCoroutine(CheckForSameUserId(false));
        //yield return null;
        GetCurrentPlayerDatabaseObject();
    }




    public void LeaderboardButtonClick()
    {
        announcementLabel.gameObject.SetActive(true);
        Debug.Log("LeaderboardButtonClick");
        isErrorOccured = false;

        var userId = GameController.Instance.StateData.UserId;
        Debug.Log($"UserId: {userId}");

        if (client == null)
        {
            Debug.Log($"client = null. Try to auth");
            AuthenticationPlayerIO.AuthenticationUser(userId, OnSuccessAuth, OnError);
        }

        StartCoroutine(ShowLeaderboard());
    }

    private void OnSuccessAuth(Client client)
    {
        Debug.Log($"Success Authentication! client.ConnectUserId: {client.ConnectUserId}");
        this.client = client;
    }

    private void OnError(PlayerIOError playerIOError)
    {
        Debug.LogWarning($"OnError. playerIOError: {playerIOError}");
        if (playerIOError != null)
        {
            Debug.LogWarning($"Error! " +
            $"ErrorCode: {playerIOError.ErrorCode}," +
            $"HelpLink: {playerIOError.HelpLink}" +
            $"HResult: {playerIOError.HResult}" +
            $"InnerException: {playerIOError.InnerException}" +
            $"Message: {playerIOError.Message}" +
            $"Source: {playerIOError.Source}" +
            $"StackTrace: {playerIOError.StackTrace}");
        }

        client = null;
        database = null;
        currentPlayerDatabaseObject = null;
        isSuccessfulSave = false;
        isErrorOccured = true;
        ShowErrorOrTimeExpiresLabel();
    }

    private void OnError(string errorMessage)
    {
        Debug.LogWarning($"OnError. errorMessage: {errorMessage}");
        client = null;
        database = null;
        currentPlayerDatabaseObject = null;
        isSuccessfulSave = false;
        isErrorOccured = true;
        ShowErrorOrTimeExpiresLabel();
    }



    private IEnumerator ShowLeaderboard()
    {
        estimatedTime = 0;

        while (client == null && estimatedTime < maxTimeForWaiting)
        {
            yield return null;
            estimatedTime += Time.deltaTime;
        }

        if (client == null)
        {
            OnError("Error! Cannot download client!");
            yield break;
        }

        //Debug.Log($"1. timeEstimated: {estimatedTime}");


        if (client != null)
        {
            GetCurrentPlayerDatabaseObject();
        }
        else
        {
            OnError("Error!  client == null!");
            yield break;
        }

        while (!isSuccessfulSave && estimatedTime < maxTimeForWaiting)
        {
            yield return null;
            estimatedTime += Time.deltaTime;
        }

        if (!isSuccessfulSave)
        {
            OnError("Error! Cannot save file in DB!");
            yield break;
        }

        //Debug.Log($"2. timeEstimated: {estimatedTime}");

        yield return StartCoroutine(GetDatabase());
        yield return StartCoroutine(FormCurrentPlayerLeaderboardLine());


        if (database != null)
        {
            FormattingLeaderboardTable();
            if (!isErrorOccured)
            {
                //OpenLeaderboardTable();
                rankCurrentPlayer.SetActive(true);
                nameCurrentPlayer.SetActive(true);
                starsNumberCurrentPlayer.SetActive(true);
                announcementLabel.gameObject.SetActive(false);
            }
            else
            {
                ShowErrorOrTimeExpiresLabel();
            }
        }
    }


    


    private IEnumerator GetDatabase()
    {
        client.BigDB.LoadRange("Stars", "Stars", null, null, null, 500, db =>
        {
            database = db;
        },
        OnError);

        while (database == null && estimatedTime < maxTimeForWaiting)
        {
            yield return null;
            estimatedTime += Time.deltaTime;
        }

        if (database == null)
        {
            OnError("Error! Cannot download database!");
            yield break;
        }

        //Debug.Log($"3. timeEstimated: {estimatedTime}");
    }

    private IEnumerator FormCurrentPlayerLeaderboardLine()
    {
        while (currentPlayerDatabaseObject == null && estimatedTime < maxTimeForWaiting)
        {
            yield return null;
            estimatedTime += Time.deltaTime;
        }

        if (currentPlayerDatabaseObject == null)
        {
            OnError("Error! Cannot download currentPlayerDatabaseObject!");
            yield break;
        }

        //Debug.Log($"4. timeEstimated: {estimatedTime}");

        var currentPlayerValues = currentPlayerLeaderboardLine.GetComponentsInChildren<Text>(true);
        currentPlayerValues[0].text = GetCurrentPlayerRank().ToString();
        currentPlayerValues[1].text = GameController.Instance.PlayerName;
        currentPlayerValues[2].text = GameController.Instance.StarsCount.ToString();
    }



    private void FormattingLeaderboardTable()
    {
        var childCount = leaderboardContentRT.childCount;
        for (int i = 0; i < childCount; i++)
        {
            var line = leaderboardContentRT.GetChild(i);
            if (line != null)
            {
                Destroy(line.gameObject);
            }
        }


        var showingLeaders = database.Take(leaderboardLinesNumber).ToArray();

        for (int i = 0; i < showingLeaders.Length; i++)
        {
            //Debug.Log($"database[i]: {showingLeaders[i]}");

            var line = Instantiate(Prefabs.Instance.leaderboardLine, leaderboardContentRT);
            var values = line.GetComponentsInChildren<Text>();
            values[0].text = (i + 1).ToString();

            try
            {
                values[1].text = showingLeaders[i].GetString("Name", "New player");
                values[2].text = showingLeaders[i].GetInt("StarsNumber", 0).ToString();
            }
            catch (Exception e)
            {
                OnError($"Error when getting properties: {e}");
                values[1].text = "ERROR!";
                values[2].text = (-1).ToString();
            }

            line.anchoredPosition = new Vector2(line.anchoredPosition.x, -intervalPositionBetweenLines * i);

            if (i % 2 == 1)
            {
                var backG = line.GetComponentInChildren<Image>();
                Destroy(backG.gameObject);
            }

            //Debug.Log($"showingLeaders[i].Key: {showingLeaders[i].Key} " +
            //    $"client.ConnectUserId: {client.ConnectUserId}");

            //≈сли текущий игрок попадает в отображаемый список, то строка выдел€етс€.
            if (showingLeaders[i].Key == client.ConnectUserId)
            {
                values[0].color = Color.yellow;
                values[1].color = Color.yellow;
                values[2].color = Color.yellow;
            }
        }

    }


    


    private void GetCurrentPlayerDatabaseObject()
    {
        isSuccessfulSave = false;
        client.BigDB.LoadOrCreate("Stars", client.ConnectUserId, dbo => 
        {
            dbo.Set("Name", GameController.Instance.PlayerName);
            dbo.Set("StarsNumber", GameController.Instance.StarsCount);
            dbo.Save(true, false, () => 
            {
                isSuccessfulSave = true;
            }, OnError);
            currentPlayerDatabaseObject = dbo; 
        }, OnError);
    }

    /// <summary>
    /// ѕолучает текущее место игрока из базы. 
    /// </summary>
    /// <returns>“екущее место игрока. -1 - если игрок в базе не найден.</returns>
    private int GetCurrentPlayerRank()
    {
        for (int i = 0; i < database.Length; i++)
        {
            if (database[i].Key == currentPlayerDatabaseObject.Key)
                return i + 1;
        }

        //нет в базе
        return -1;
    }



    //private void OpenLeaderboardTable() => leaderboardLabel.SetActive(true);

    //private void CloseLeaderboardTable() => leaderboardLabel.SetActive(false);





    /// <summary>
    /// ¬ случае первого входа в базу данных, смотрим есть ли там такой же userId (ну вдруг совпало), и если есть
    /// присваиваем новый id пока он не станет уникальным.
    /// </summary>
    private IEnumerator CheckForSameUserId(bool showError)
    {
        if (showError)
        {
            client.BigDB.LoadRange("Stars", "Stars", null, null, null, 500, db =>
            {
                database = db;
            });
        }
        else
        {
            client.BigDB.LoadRange("Stars", "Stars", null, null, null, 500, db =>
            {
                database = db;
            },
            OnError);
        }

        float _estimatedTime = estimatedTime;
        
        while(database == null && _estimatedTime < maxTimeForWaiting)
        {
            yield return null;
        }

        if (database == null)
        {
            if (showError)
            { OnError("Error! Database == null!"); }
            yield break;
        }
        
        var keys = database.Select(dbo => dbo.Key);
        while (keys.Contains(GameController.Instance.StateData.UserId))
        {
            Debug.Log($"DB contain same key: {GameController.Instance.StateData.UserId}");
            GameController.Instance.StateData.SetRandomUserId();
            Debug.Log($"new key: {GameController.Instance.StateData.UserId}");
        }

        GameController.Instance.SaveState();
        IsFirstTime = false;
    }


    //public void LeaderboardBackButtonClick() => CloseLeaderboardTable();





    private void ShowErrorOrTimeExpiresLabel()
    {
        //loadingCover.SetActive(false);

        if (estimatedTime >= maxTimeForWaiting)
        {
            //UIController.Instance.ShowInformationInMenu(GameController.Instance.languageController.theResponseWaitingTimeHasBeenExceeded);
            announcementLabel.text = GameController.Instance.languageController.theResponseWaitingTimeHasBeenExceeded;
        }
        else
        {
            //UIController.Instance.ShowInformationInMenu(GameController.Instance.languageController.Thereisnoconnectiontotheremoteserver);
            announcementLabel.text = GameController.Instance.languageController.Thereisnoconnectiontotheremoteserver;
        }
    }

    



    public void LeaderboardButtonClickNew()
    {
        SceneManager.LoadScene("LeaderboardScene", LoadSceneMode.Single);
    }

    public void LeaderboardBackButtonClickNew()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }





    private IEnumerator AddRandomEntrys(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int unixTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(new System.Random().Next(0, 1000000).ToString())).ComputeHash(System.Text.Encoding.UTF8.GetBytes(unixTime + "" + unixTime));
            string _userId = unixTime + "" + BitConverter.ToString(hmac).Replace("-", "").ToLowerInvariant(); 
            
            string UserId = _userId.Substring(0, 50);
            string playerName = GetRandomName();
            int starsCount = new System.Random().Next(0, 11000);

            AuthenticationPlayerIO.AuthenticationUser(UserId, (cl) =>
            {
                client.BigDB.LoadOrCreate("Stars", UserId, dbo =>
                {
                    dbo.Set("Name", playerName);
                    dbo.Set("StarsNumber", starsCount);
                    dbo.Save();
                });
            }, a => { });

            Debug.Log($"Add entry. playerName: {playerName}, starsCount: {starsCount}, id: {UserId}");

            yield return new WaitForSeconds(0.5f);
        }


        string GetRandomName()
        {
            int length = new System.Random().Next(4, 10);
            string name = "";
            for (int i = 0; i < length; i++)
            {
                name += 'A' + new System.Random().Next(0, 26);
            }
            return name;
        }


        Debug.Log("End of adding random entrys!!");
    }



}
