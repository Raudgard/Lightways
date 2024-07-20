using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;


namespace Ads
{
    public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
    {
        [SerializeField] string androidGameID = "5284177";
        [SerializeField] string iOSGameID = "5284176";
        [SerializeField] bool testMode;
        private string gameID;

        //public bool initialized = false;

        private void Awake()
        {
            gameID = (Application.platform == RuntimePlatform.IPhonePlayer) ? iOSGameID : androidGameID;
        }

        private void Start()
        {
            StartCoroutine(StartInitialize());
        }


        private IEnumerator StartInitialize()
        {
            while (!Advertisement.isInitialized)
            {
                //Debug.Log("Try to Initialize.");
                try
                {
                    Advertisement.Initialize(gameID, testMode, this);
                }
                catch { }
                yield return new WaitForSeconds(1);
            }

        }

        public void OnInitializationComplete()
        {
            //Debug.Log("Инициализация рекламы прошла успешно.");
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Ошибка инициализации: {error.ToString()} - {message}");
        }

    }
}
