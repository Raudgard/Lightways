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
    [Tooltip("���, � ������� ����������� �����.")]
    [SerializeField] private int hourToAdding;
    [Tooltip("������, � ������� ����������� �����.")]
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
            StartCoroutine(AddingKeysOn�eADay());
        }

        #region ��� ������������
        //gameController.StateData.timeNextAddingKeys = DateTime.Parse($"{hourToAdding}:{minuteToAdding}:00");
        //if (gameController.StateData.timeNextAddingKeys <= DateTime.Now)
        //{
        //    Debug.Log($"���������� ����� �� ������� ���� ���������� ������.");
        //    gameController.StateData.timeNextAddingKeys += TimeSpan.FromHours(24);
        //}
        //GameController.Instance.SaveState();
        #endregion


        //StartCoroutine(AddingKeysOn�eADay());

    }


    public void ChangeKeysCountText() => keysLabelTMPro.text = GameController.Instance.StateData.KeysForModesCount.ToString();


    /// <summary>
    /// ���������, ������� �� ������ ��� ��������������� ������ Go � �������. ���� ������ 0, �� ������ ���������� �����������.
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
    /// �������� �� ���������� ������ ��� � �����.
    /// </summary>
    private IEnumerator AddingKeysOn�eADay()
    {
        var stateData = GameController.Instance.StateData;
        //Debug.Log($"timeLastAddingKeys: {stateData.timeLastAddingKeys}, timeNextAddingKeys: {stateData.timeNextAddingKeys}");
        //Debug.Log($"DateTime.Now: {DateTime.Now}, DateTime.Now - timeLastAddingKeys: {DateTime.Now - stateData.timeLastAddingKeys}");

        //���� ������ ����� 24 ����� �� ������� ���������� ���������� ������. � ����� ��� ������ ����� � ����.
        if (DateTime.Now - stateData.timeLastAddingKeys > TimeSpan.FromHours(24))
        {
            //Debug.Log($"�� ������� ���������� ���������� ������ ������ ����� �����.");

            stateData.timeNextAddingKeys = DateTime.Parse($"{hourToAdding}:{minuteToAdding}:00");
            if (stateData.timeNextAddingKeys < DateTime.Now)
            {
                //Debug.Log($"���������� ����� �� ������� ���� ���������� ������.");
                stateData.timeNextAddingKeys += TimeSpan.FromHours(24);
            }
            //Debug.Log($"timeLastAddingKeys: {stateData.timeLastAddingKeys}, timeNextAddingKeys: {stateData.timeNextAddingKeys}");
            GameController.Instance.SaveState();

            //��������� �����
            AddKeys(keysCountToAddingOnceADay);
        }

        //�������� ������������, �� ���������� �� ��������� ����� �� ����� ����������� ������.
        while(true)
        {
            if(DateTime.Now > stateData.timeNextAddingKeys)
            {
                //Debug.Log($"DateTime.Now: {DateTime.Now} late than stateData.timeNextAddingKeys: {stateData.timeNextAddingKeys}. Time to add keys.");
                
                //��������� �����
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
            //Debug.Log($"������ ����� ���� ����� {keysCountToAddingOnceADay}, ������ �� ���������.");
            return;
        }

        Debug.Log($"������ ����� {keysCountToAddingOnceADay}, ��������� �� {keysCountToAddingOnceADay}.");
        stateData.KeysForModesCount = keysCount;

        stateData.timeLastAddingKeys = DateTime.Now;
        stateData.timeNextAddingKeys = DateTime.Parse($"{hourToAdding}:{minuteToAdding}:00");
        if (stateData.timeNextAddingKeys <= DateTime.Now)
        {
            Debug.Log($"���������� ����� �� ������� ���� ���������� ������.");
            stateData.timeNextAddingKeys += TimeSpan.FromHours(24);
        }
        Debug.Log($"timeLastAddingKeys: {stateData.timeLastAddingKeys}, timeNextAddingKeys: {stateData.timeNextAddingKeys}");
        GameController.Instance.SaveState();
        ChangeKeysCountText();
    }

}
