using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;

namespace Ads
{
    public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        [SerializeField] UIController uIController;
        [SerializeField] AdsInitializer adsInitializer;
        [SerializeField] GameObject AdsButton;
        [SerializeField] string androidAdID = "Rewarded_Android";
        [SerializeField] string iOSAdID = "Rewarded_iOS";
        private string adID;

        [Tooltip("Количество ключей, прибавляемых за рекламу.")]
        [Range(1, 20)]
        [SerializeField] int countKeysReward;

        private WaitForSeconds wait1Second = new WaitForSeconds(1);
        private bool isLoaded;


        private void Awake()
        {
            adID = (Application.platform == RuntimePlatform.IPhonePlayer) ? iOSAdID : androidAdID;
        }

        private IEnumerator Start()
        {
            var gameController = GameController.Instance;
            gameController.onPurchaseGame += delegate { AdsButton.SetActive(false); };

            while (!gameController.isStateLoaded)
                yield return null;

            if (gameController.IsGamePurchased)
            {
                AdsButton.SetActive(false);
            }

            StartCoroutine(AdsLoader());

        }


        private IEnumerator AdsLoader()
        {
            if (GameController.Instance.IsGamePurchased)
                yield break;

            while (true)
            {
                if (!Advertisement.isInitialized)
                {
                    //Debug.Log("!Advertisement.isInitialized");
                    yield return wait1Second;
                    continue;
                }

                if (!isLoaded)
                {
                    //Debug.Log("Advertisement.Load");
                    Advertisement.Load(adID, this);
                    yield return wait1Second;
                    continue;
                }

                yield return wait1Second;
            }
        }


        public void ShowAd()
        {
            //Debug.Log($"ShowAd. adsInitializer.initialized: {adsInitializer.initialized}" );
            if (Advertisement.isInitialized && isLoaded)
            {
                //Advertisement.Load(adID, this);
                Advertisement.Show(adID, this);
            }
            else
            {
                uIController.ShowInformationInMenu(GameController.Instance.languageController.advertisingIsNotAvailable);
            }
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            //Debug.Log("Реклама загружена: " + placementId);
            isLoaded = true;
            AdsButton.SetActive(true);
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Debug.Log($"Ошибка загрузки рекламы: {error.ToString()} - {message}");
            isLoaded = false;
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Debug.Log($"Ошибка показа рекламы: {error.ToString()} - {message}");
            isLoaded = false;
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            Debug.Log("Старт показа реклама: " + placementId);
            isLoaded = false;
            AdsButton.SetActive(false);
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            Debug.Log("клик по рекламе: " + placementId);
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            if (showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
            {
                GameController.Instance.AddKeys(countKeysReward);

                //Debug.Log("Юнити завершил показ рекламы, и добавил бонусы игроку.");
            }
        }

    }
}