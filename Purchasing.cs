using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;
using TMPro;


public class Purchasing : MonoBehaviour
{
    UnityEngine.Purchasing.Price Price = new Price();
    public Product appPurchase;
    [SerializeField] private Button buyTheGameButton;

    private void OnEnable()
    {
        //Debug.Log("Purchasing OnEnable()");
        SetPurchasingButton(GameController.Instance.IsGamePurchased);

    }


    public void OnPurchaseComplete(Product appPurchase)
    {
        GameController.Instance.PurchaseGame();
        SetPurchasingButton(true);
        Debug.Log("OnPurchaseComplete!!!");

    }

    public void OnPurchaseFailed(Product appPurchase, PurchaseFailureDescription purchaseFailureDescription)
    {
        Debug.Log($"OnPurchaseFailed. message: {purchaseFailureDescription.message}, productId: {purchaseFailureDescription.productId}, reason: {purchaseFailureDescription.reason}");

    }

    public void OnProductFetched(Product appPurchase)
    {
        //Debug.Log($"OnProductFetched. availableToPurchase: {appPurchase.availableToPurchase}, hasReceipt: {appPurchase.hasReceipt}, metadata: {appPurchase.metadata}, receipt: {appPurchase.receipt}, transactionID: {appPurchase.transactionID}");
        //Debug.Log($"OnProductFetched. definition.enabled: {appPurchase.definition.enabled}, definition.id: {appPurchase.definition.id}, definition.payout: {appPurchase.definition.payout}, definition.storeSpecificId: {appPurchase.definition.storeSpecificId}, definition.type: {appPurchase.definition.type}");

    }

    /// <summary>
    /// Устанавливает текст и интерактивность кнопки "Купить игру!" в зависимости от того, куплена уже игра или нет.
    /// </summary>
    /// <param name="isGamePurchased"></param>
    private void SetPurchasingButton(bool isGamePurchased)
    {
        var tmpro = buyTheGameButton.GetComponentInChildren<TextMeshProUGUI>();
        if (isGamePurchased)
        {
            tmpro.text = GameController.Instance.languageController.gameIsPurchasedThanks;
            buyTheGameButton.interactable = false;
        }
        else
        {
            tmpro.text = GameController.Instance.languageController.buyTheGameButtonText;
            buyTheGameButton.interactable = true;
        }
    }
}
