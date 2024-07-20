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

public class ClientPlayerIO : MonoBehaviour
{
    #region Singleton
    private static ClientPlayerIO instance;
    public static ClientPlayerIO Instance
    {
        get { return instance; }
    }
    #endregion


    [SerializeField] private GameObject messageLabel;
    [SerializeField] private TextMeshProUGUI messageTMP;

    [Space]

    [Tooltip("Максимальное время ожидания от сервера PlayerIO в секундах")]
    [SerializeField] private float maxTimeForWaiting;

    


    public Client Client => client;

    public ErrorConnectToPIO Error { get; private set; } = null;


    private Client client = null;
    private float estimatedTime = 0;
    GameController gameController = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this);
        }

    }

    private IEnumerator Start()
    {
        //Debug.Log($"client = null. Try to auth");
        gameController = GameController.Instance;
        yield return new WaitForSeconds(1.5f);
        
        while (!gameController.isStateLoaded)
            yield return null;

        if (gameController.ClientPlayerIO != null)
            client = gameController.ClientPlayerIO;

        //Не нужен нам на старте клиент, потому что если у пользователя нет интернета, то прям на старте будет вылезать табличка об отсутствии связи.

        //else
        //{
        //    GetClient();
        //}
    }

    /// <summary>
    /// Пытается получить клиент от удаленного сервера. В результате либо Client, либо Error не будут равны null.
    /// </summary>
    public void GetClient()
    {
        StartCoroutine(GettingClient());
    }

    private IEnumerator GettingClient()
    {
        messageLabel.SetActive(true);
        client = null;
        Error = null; 
        
        while (!gameController.isStateLoaded)
            yield return null;

        AuthenticationPlayerIO.AuthenticationUser(GameController.Instance.StateData.UserId, OnSuccessAuth, OnError);

        estimatedTime = 0;
        while (client == null && estimatedTime < maxTimeForWaiting)
        {
            yield return null;
            estimatedTime += Time.deltaTime;
        }

        if(client == null)
        {
            Error = new ErrorConnectToPIO(true, null);
            messageLabel.SetActive(false);
        }
    }


    private void OnSuccessAuth(Client client)
    {
        Debug.Log($"Success Authentication! client.ConnectUserId: {client.ConnectUserId}");
        this.client = client;
        Error = null;
        messageLabel.SetActive(false);
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
        //database = null;
        Error = new ErrorConnectToPIO(false, playerIOError);
        messageLabel.SetActive(false);

        //currentPlayerDatabaseObject = null;
        //isSuccessfulSave = false;
        //isErrorOccured = true;
        StartCoroutine(ShowErrorOrTimeExpiresLabel());
    }



    private IEnumerator ShowErrorOrTimeExpiresLabel()
    {
        messageLabel.SetActive(true);

        if (estimatedTime >= maxTimeForWaiting)
        {
            //UIController.Instance.ShowInformationInMenu(GameController.Instance.languageController.theResponseWaitingTimeHasBeenExceeded);
            messageTMP.text = GameController.Instance.languageController.theResponseWaitingTimeHasBeenExceeded;
        }
        else
        {
            //UIController.Instance.ShowInformationInMenu(GameController.Instance.languageController.Thereisnoconnectiontotheremoteserver);
            messageTMP.text = GameController.Instance.languageController.Thereisnoconnectiontotheremoteserver;
        }

        yield return new WaitForSeconds(3);
        messageLabel.SetActive(false);

    }

}


public class ErrorConnectToPIO
{
    public bool IsConnectionTimeExpired { get; }
    public PlayerIOError Error { get; } = null;

    public ErrorConnectToPIO(bool isConnectionTimeExpired, PlayerIOError error)
    {
        IsConnectionTimeExpired = isConnectionTimeExpired;
        Error = error;
    }

}
