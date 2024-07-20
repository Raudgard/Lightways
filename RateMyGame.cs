using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Play.Review;
using Saving;
using System;

public class RateMyGame : MonoBehaviour
{
    [Tooltip("Каждые сколько побед будет появляться просьба оценить игру.")]
    [SerializeField] private int winLevelsCountWhenAppears;
    [Tooltip("Дней пройдет до того, как можно будет опять вызывать In-App-Review. Из-за Qotas.")]
    [SerializeField] private int daysBeforeNextInAppReviewCall;
    [SerializeField] private GameObject rateMyGameLabel;


    private ReviewManager reviewManager;
    private PlayReviewInfo playReviewInfo;


    void Start()
    {
        //PlayerPrefs.DeleteAll();
        bool userHasAlreadyReview = PlayerPrefs.GetInt(SCFPP.Miscellaneous.userHasAlreadyReview, 0) == 1;
        //Debug.Log($"userHasAlreadyReview: {userHasAlreadyReview}");

        if (!userHasAlreadyReview)
            GameController.Instance.onWinLevel += OnWinLevel;
    }

    private void OnWinLevel()
    {
        int winCount = PlayerPrefs.GetInt(SCFPP.Miscellaneous.winLevelsCount);
        //Debug.Log($"winCount: {winCount}");
        //Debug.Log($"winCount + 1 % winLevelsCountWhenAppears =  {winCount + 1 % winLevelsCountWhenAppears}");

        if ((winCount + 1) % winLevelsCountWhenAppears == 0)
        {
            RateMyGameLabelShow();
        }
    }

    private void RateMyGameLabelShow()
    {
        rateMyGameLabel.SetActive(true);
        StartCoroutine(RequestReviewInfoObject());
    }

    private IEnumerator RequestReviewInfoObject()
    {
        reviewManager = new ReviewManager();
        var requestFlowOperation = reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        //Debug.Log($"requestFlowOperation.GetResult().Equals(null): {requestFlowOperation.GetResult().Equals(null)}");
        //Debug.Log($"requestFlowOperation.IsDone: {requestFlowOperation.IsDone}");
        //Debug.Log($"requestFlowOperation.IsSuccessful: {requestFlowOperation.IsSuccessful}");


        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.Log($"Error: {requestFlowOperation.Error}");
            OpenPlayMarketForRate();
            yield break;
        }
        playReviewInfo = requestFlowOperation.GetResult();

        //Debug.Log($"reviewManager: {reviewManager}, playReviewInfo: {playReviewInfo}");
    }




    public void RateButtonClick()
    {
        if (HasRequiredTimePassedSincePreviousCall())
        {
            StartCoroutine(OpenReview());
        }
        else
        {
            OpenPlayMarketForRate();
        }
    }

    public void LaterButtonClick()
    {
        rateMyGameLabel.SetActive(false);
    }


    private IEnumerator OpenReview()
    {
        var launchFlowOperation = reviewManager.LaunchReviewFlow(playReviewInfo);
        //Debug.Log($"1. launchFlowOperation: {launchFlowOperation}, playReviewInfo: {playReviewInfo}");

        yield return launchFlowOperation;
        playReviewInfo = null; // Reset the object
        rateMyGameLabel.SetActive(false);

        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.Log($"Error: {launchFlowOperation.Error}");
            OpenPlayMarketForRate();
            yield break;
        }
        //Debug.Log($"2. launchFlowOperation: {launchFlowOperation}, playReviewInfo: {playReviewInfo}");

        while(launchFlowOperation.keepWaiting)
        {
            yield return null;
            Debug.Log($"keepWaiting...");

        }

        if (launchFlowOperation.IsDone && launchFlowOperation.IsSuccessful)
        {
            MarkThanUserRateGame();
            MarkThanInAppReviewWasCalled();
        }
    }


    public void RateButtonInOptionsClick()
    {
        if (HasRequiredTimePassedSincePreviousCall())
        {
            StartCoroutine(RateFromOptions());
        }
        else
        {
            OpenPlayMarketForRate();
        }
    }

    private IEnumerator RateFromOptions()
    {
        yield return StartCoroutine(RequestReviewInfoObject());
        yield return StartCoroutine(OpenReview());
    }


    /// <summary>
    /// Переходит на страницу игры в Play Market, чтобы игрок мог оценить игру.
    /// </summary>
    public void OpenPlayMarketForRate()
    {
        Application.OpenURL("market://details?id=" + Application.identifier);
        MarkThanUserRateGame();
    }


    private void MarkThanUserRateGame()
    {
        PlayerPrefs.SetInt(SCFPP.Miscellaneous.userHasAlreadyReview, 1);
        //Debug.Log($"PlayerPrefs.SetInt(SCFPP.Miscellaneous.userHasAlreadyReview, 1);");

        try
        {
            GameController.Instance.onWinLevel -= OnWinLevel;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"{e.Message}");
        }
    }

    /// <summary>
    /// Отмечает, что In-App-Review вызывалось, и время вызова. Так как результат вызова невозможно узнать, то необходимо отсчитывать время до следующего возможного вызова.
    /// Если время еще не пройдет, пользователь будет перенаправляться в магазин.
    /// </summary>
    private void MarkThanInAppReviewWasCalled()
    {
        PlayerPrefs.SetString(SCFPP.Miscellaneous.inAppReviewCalledTime, DateTime.Now.ToString());
    }

    /// <summary>
    /// Прошло ли необходимое время с момента предыдущего вызова In-App-Review?
    /// </summary>
    /// <returns></returns>
    private bool HasRequiredTimePassedSincePreviousCall()
    {
        var lastTimeCalled = PlayerPrefs.GetString(SCFPP.Miscellaneous.inAppReviewCalledTime, "01.01.0001 0:00:00");

        DateTime parsedTime = DateTime.Parse(lastTimeCalled);
        Debug.Log($"parsedTime: {parsedTime}");

        var timeNow = DateTime.Now.ToString();
        Debug.Log($"time: {timeNow}");

        var timePassed = DateTime.Now - parsedTime;
        Debug.Log($"timePassed: {timePassed}");

        return timePassed > TimeSpan.FromDays(daysBeforeNextInAppReviewCall);
        //{
        //    Debug.Log($"Прошло более {daysBeforeNextInAppReviewCall} дней");
        //}
        //else
        //{
        //    Debug.Log($"Прошло менее {daysBeforeNextInAppReviewCall} дней");
        //}
    }

}
