using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Saving;

public class KeysController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI keysLabelTMPro;
    [SerializeField] private Button randomPhotonsGoButton;
    [SerializeField] private Button fasterThanLightGoButton;
    [SerializeField] private Button blackHolesRiddlesGoButton;

    [SerializeField] private GameObject keysLabel;
    [SerializeField] private GameObject minusOnekeysGOinRandomPhotonsMenu;
    [SerializeField] private GameObject minusOnekeysGOinFasterThanLightMenu;
    [SerializeField] private GameObject minusOnekeysGOinBlackHolesRiddlesMenu;

    [Space]
    [Header("Adding keys once a day")]

    [SerializeField] private int keysCountToAddingOnceADay;
    [Tooltip("Час, в который добавляются ключи.")]
    [SerializeField] private int hourToAdding;
    [Tooltip("Минута, в которую добавляются ключи.")]
    [SerializeField] private int minuteToAdding;
    
    




    private IEnumerator Start()
    {
        var gameController = GameController.Instance;
        gameController.onPurchaseGame += OnGamePurchase;

        while (!gameController.isStateLoaded)
            yield return null;

        if(gameController.IsGamePurchased)
        {
            OnGamePurchase();
        }
        else
        {
            StartCoroutine(AddingKeysOnсeADay());
        }

        #region для тестирования
        //gameController.StateData.timeNextAddingKeys = DateTime.Parse($"{hourToAdding}:{minuteToAdding}:00");
        //if (gameController.StateData.timeNextAddingKeys <= DateTime.Now)
        //{
        //    Debug.Log($"прибавляем сутки ко времени след добавления ключей.");
        //    gameController.StateData.timeNextAddingKeys += TimeSpan.FromHours(24);
        //}
        //GameController.Instance.SaveState();
        #endregion


        //StartCoroutine(AddingKeysOnсeADay());

    }


    public void ChangeKeysCountText() => keysLabelTMPro.text = GameController.Instance.StateData.KeysForModesCount.ToString();


    /// <summary>
    /// Проверяет, хватает ли ключей для интерактивности кнопок Go в режимах. Если ключей 0, то кнопки становятся неактивными.
    /// </summary>
    public void CheckInteractivityForModesGoButtons()
    {
        randomPhotonsGoButton.interactable =
            fasterThanLightGoButton.interactable =
            blackHolesRiddlesGoButton.interactable = GameController.Instance.StateData.KeysForModesCount > 0;
    }

    private void OnGamePurchase()
    {
        keysLabel.gameObject.SetActive(false);
        minusOnekeysGOinRandomPhotonsMenu.gameObject.SetActive(false);
        minusOnekeysGOinFasterThanLightMenu.gameObject.SetActive(false);
        minusOnekeysGOinBlackHolesRiddlesMenu.gameObject.SetActive(false);
    }


    /// <summary>
    /// Проверка на добавление ключей раз в сутки.
    /// </summary>
    private IEnumerator AddingKeysOnсeADay()
    {
        var stateData = GameController.Instance.StateData;
        //Debug.Log($"timeLastAddingKeys: {stateData.timeLastAddingKeys}, timeNextAddingKeys: {stateData.timeNextAddingKeys}");
        //Debug.Log($"DateTime.Now: {DateTime.Now}, DateTime.Now - timeLastAddingKeys: {DateTime.Now - stateData.timeLastAddingKeys}");

        //если прошло более 24 часов со времени последнего добавления ключей. А также при первом входе в игру.
        if (DateTime.Now - stateData.timeLastAddingKeys > TimeSpan.FromHours(24))
        {
            //Debug.Log($"Со времени последнего добавления ключей прошло более суток.");

            stateData.timeNextAddingKeys = DateTime.Parse($"{hourToAdding}:{minuteToAdding}:00");
            if (stateData.timeNextAddingKeys < DateTime.Now)
            {
                //Debug.Log($"прибавляем сутки ко времени след добавления ключей.");
                stateData.timeNextAddingKeys += TimeSpan.FromHours(24);
            }
            //Debug.Log($"timeLastAddingKeys: {stateData.timeLastAddingKeys}, timeNextAddingKeys: {stateData.timeNextAddingKeys}");
            GameController.Instance.SaveState();

            //добавляем ключи
            AddKeys(keysCountToAddingOnceADay);
        }

        //проверка ежесекундная, не перевалило ли настоящее время за время прибавления ключей.
        while(true)
        {
            if(DateTime.Now > stateData.timeNextAddingKeys)
            {
                //Debug.Log($"DateTime.Now: {DateTime.Now} late than stateData.timeNextAddingKeys: {stateData.timeNextAddingKeys}. Time to add keys.");
                
                //добавляем ключи
                AddKeys(keysCountToAddingOnceADay);
            }

            yield return new WaitForSeconds(1);
        }
    }


    private void AddKeys(int keysCount)
    {
        var stateData = GameController.Instance.StateData;
        if (stateData.KeysForModesCount >= keysCountToAddingOnceADay)
        {
            //Debug.Log($"Ключей более либо равно {keysCountToAddingOnceADay}, ничего не добавляем.");
            return;
        }

        Debug.Log($"Ключей менее {keysCountToAddingOnceADay}, добавляем до {keysCountToAddingOnceADay}.");
        stateData.KeysForModesCount = keysCount;

        stateData.timeLastAddingKeys = DateTime.Now;
        stateData.timeNextAddingKeys = DateTime.Parse($"{hourToAdding}:{minuteToAdding}:00");
        if (stateData.timeNextAddingKeys <= DateTime.Now)
        {
            Debug.Log($"прибавляем сутки ко времени след добавления ключей.");
            stateData.timeNextAddingKeys += TimeSpan.FromHours(24);
        }
        Debug.Log($"timeLastAddingKeys: {stateData.timeLastAddingKeys}, timeNextAddingKeys: {stateData.timeNextAddingKeys}");
        GameController.Instance.SaveState();
        ChangeKeysCountText();
    }

}
