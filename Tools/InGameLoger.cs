using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Tools
{
    public class InGameLoger : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        public float timeOfLifeText = 60.0f;
        private Canvas canvas;
        private RectTransform rectTransform;
        private static InGameLoger Instance;

        private Image label;
        [Range(0.0f, 1.0f)]
        public float labelWidthRelativeScreen = 0.9f;
        [Range(0.0f, 1.0f)]
        public float labelHeightRelativeScreen = 0.1f;

        public float edgeField = 5;
        //[Range(0.0f, 1.0f)]
        //public float labelHeightRelativeScreen = 0.15f;
        public Color labelColor = new Color(0, 0, 0, 0.95f);

        private TextMeshProUGUI text;
        public int fontSize = 18;

        private Coroutine timerCoroutine = null;
        private float textLifeTime;

        private Vector2 shift = Vector2.zero;
        private bool dragging = false;


        /// <summary>
        /// Необходимо в случае отключенного домена (в Project Settings -> Enter Play Mode Options) при загрузке режима Play. Не работает, потому что нужен статический метод.
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        static void RunOnStart()
        {
            //Debug.Log("Unregistering logMessageReceived function");
            Application.logMessageReceived -= ShowLogMessageForTime;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<InGameLoger>();
            }
        }



        void Start()
        {
            //gameObject.AddComponent<EventTrigger>();


            FindOrCreateCanvas();
            SetSizeOfGameObject();
            CreateLabel();
            CreateCloseButton();
            //CreateTextSizeInputField();
            CreateText();
            gameObject.SetActive(false);

            Application.logMessageReceived += ShowLogMessageForTime;

            //print("name: " + gameObject.name);

            //print("Screen.width: " + Screen.width + 
            //    "  Screen.height: " + Screen.height + 
            //    " canvas.pixelRect.width: " + canvas.pixelRect.width + 
            //    " canvas.pixelRect.height:" + canvas.pixelRect.height + 
            //    " label.rectTransform.sizeDelta: " + label.rectTransform.sizeDelta +
            //    "  label.rectTransform.rect.width: " + label.rectTransform.rect.width +
            //    " label.rectTransform.rect.height: " + label.rectTransform.rect.height);
        }

        ///// <summary>
        ///// Необходимо в случае отключенного домена (в Project Settings -> Enter Play Mode Options) при загрузке режима Play.
        ///// </summary>
        //private void OnApplicationQuit()
        //{
        //    Debug.Log("on app quit  Unregistering logMessageReceived function");
        //    Application.logMessageReceived -= ShowLogMessageForTime;
        //}


        private static void ShowLogMessageForTime(string condition, string stackTrace, LogType logType)
        {
            string textToShow = string.Empty;
            if (logType == LogType.Exception)
            {
                textToShow += "EXEPTION! ";
            }
            else if (logType == LogType.Error)
            {
                textToShow += "ERROR! ";
            }
            else if (logType == LogType.Warning)
            {
                textToShow += "WARNING! ";
            }

            textToShow += "[" + DateTime.Now.ToLongTimeString() + "] " + condition;
            Instance.text.fontSize = Instance.fontSize;

            Instance.text.color = logType switch
            {
                LogType.Log => Color.white,
                LogType.Warning => Color.yellow,
                LogType.Error => Color.red,
                _ => Color.white
            };

            if (Instance.timerCoroutine == null)
            {
                Instance.gameObject.SetActive(true);
                Instance.textLifeTime = Instance.timeOfLifeText;
                Instance.text.text = textToShow;
                Instance.timerCoroutine = Instance.StartCoroutine(Instance.Timer());
            }
            else
            {
                Instance.text.text += Environment.NewLine + textToShow;
                Instance.textLifeTime = Instance.timeOfLifeText;
            }

            Instance.label.rectTransform.sizeDelta = new Vector2(Instance.label.rectTransform.sizeDelta.x, Instance.text.preferredHeight + Instance.edgeField);
            Instance.text.rectTransform.sizeDelta = new Vector2(Instance.text.rectTransform.sizeDelta.x, Instance.text.preferredHeight + Instance.edgeField);

        }

        private void ShowLogMessageWithButton(string condition, string stackTrace, LogType logType)
        {
            string textToShow = "[" + DateTime.Now.ToLongTimeString() + "] " + condition;
            textToShow += Environment.NewLine + Environment.NewLine;
            gameObject.SetActive(true);
            text.text = textToShow;

            var closeButtonGO = new GameObject();
            var button = closeButtonGO.AddComponent<Button>();
            var buttonRectTransform = closeButtonGO.AddComponent<RectTransform>();
            closeButtonGO.transform.parent = label.transform;
            buttonRectTransform.localScale = new Vector3(1, 1, 1);
            buttonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 50);
            buttonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 20);

            buttonRectTransform.anchorMin = new Vector2(1, 0);
            buttonRectTransform.anchorMax = new Vector2(1, 0);

            button.onClick.AddListener(delegate
            {
                gameObject.SetActive(false);
            });


            label.rectTransform.sizeDelta = new Vector2(label.rectTransform.sizeDelta.x, text.preferredHeight + edgeField);
            text.rectTransform.sizeDelta = new Vector2(text.rectTransform.sizeDelta.x, text.preferredHeight + edgeField);

        }


        private IEnumerator Timer()
        {
            while (textLifeTime > 0)
            {
                yield return null;
                textLifeTime -= Time.deltaTime;
            }

            gameObject.SetActive(false);
            timerCoroutine = null;
        }

        private void FindOrCreateCanvas()
        {
            canvas = FindObjectOfType<Canvas>();

            if (canvas == null)
            {
                canvas = this.gameObject.AddComponent<Canvas>();
            }
            else
            {
                gameObject.transform.parent = canvas.transform;
            }
        }

        private void SetSizeOfGameObject()
        {
            rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.localScale = new Vector3(1, 1, 1);
            rectTransform.anchorMin = new Vector2(0.5f, 0f); //привязка к середине низа
            rectTransform.anchorMax = new Vector2(0.5f, 0f); //привязка к середине низа
                                                             //rectTransform.anchoredPosition = new Vector2(0, Screen.height / 5);
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0, 100);

        }


        private void CreateLabel()
        {
            GameObject go = new GameObject("Label");
            go.transform.parent = this.transform;

            label = go.AddComponent<Image>();
            //print("label = " + label);

            RectTransform _transform = label.GetComponent<RectTransform>();
            if (_transform == null)
            {
                _transform = label.gameObject.AddComponent<RectTransform>();
            }


            _transform.localScale = new Vector3(1, 1, 1);
            _transform.anchorMin = new Vector2(0.5f, 0f); //привязка к середине низа
            _transform.anchorMax = new Vector2(0.5f, 0f); //привязка к середине низа
            _transform.pivot = new Vector2(0.5f, 0f);
            _transform.anchoredPosition = new Vector2(0, 100);
            _transform.sizeDelta = new Vector2(canvas.pixelRect.width * labelWidthRelativeScreen, canvas.pixelRect.height * labelHeightRelativeScreen);

            label.color = labelColor;


        }


        private void CreateCloseButton()
        {
            GameObject go = new GameObject("Button");
            var _transform = go.AddComponent<RectTransform>();
            var image = go.AddComponent<Image>();
            var button = go.AddComponent<Button>();
            go.transform.SetParent(label.transform);
            _transform.anchorMin = new Vector2(1, 0);
            _transform.anchorMax = new Vector2(1, 0);
            _transform.pivot = new Vector2(1, 1);
            _transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
            _transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100);
            _transform.anchoredPosition = new Vector2(0, 0);

            GameObject tmp = new GameObject("Text");
            var textComponent = tmp.AddComponent<TextMeshProUGUI>();
            tmp.transform.SetParent(go.transform);
            tmp.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            textComponent.enableAutoSizing = true;
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.text = "Close";
            textComponent.color = Color.white;

            image.color = Color.black;

            button.onClick.AddListener(() =>
            {
                Instance.timerCoroutine = null;
                gameObject.SetActive(false);
            });

        }

        /// <summary>
        /// Создает InputField для ввода размера текста во время тестирования.
        /// </summary>
        private void CreateTextSizeInputField()
        {
            GameObject go = new GameObject("TextSizeInputField");
            var _transform = go.AddComponent<RectTransform>();
            //var image = go.AddComponent<Image>();
            var inputField = go.AddComponent<TMP_InputField>();
            go.transform.SetParent(label.transform);
            _transform.anchorMin = new Vector2(1, 0);
            _transform.anchorMax = new Vector2(1, 0);
            _transform.pivot = new Vector2(1, 1);
            _transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
            _transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100);
            _transform.anchoredPosition = new Vector2(-200, 0);


            inputField.onEndEdit.AddListener((value) =>
            {
                if (int.TryParse(value, out fontSize)) { }

            });
        }



        /// <summary>
        /// 
        /// </summary>
        private void CreateText()
        {
            GameObject go = new GameObject("Text");
            go.transform.parent = label.transform;

            text = go.AddComponent<TextMeshProUGUI>();
            //print("text = " + text);
            var _transform = text.GetComponent<RectTransform>() != null ? text.GetComponent<RectTransform>() : text.gameObject.AddComponent<RectTransform>();

            _transform.localScale = new Vector3(1, 1, 1);
            _transform.pivot = new Vector2(0.5f, 1f); //точка, относительно которой происходят все действия с координатами, расположена теперь не в центре, а середине верхней стороны
            _transform.anchoredPosition = new Vector2(0, -edgeField / 2);
            _transform.anchorMin = new Vector2(0.5f, 1); //привязка к середине верха
            _transform.anchorMax = new Vector2(0.5f, 1); //привязка к середине верха
            _transform.sizeDelta = label.rectTransform.sizeDelta * 0.98f;

            //text.enableAutoSizing = true;
            text.fontSize = fontSize;

        }

        /// <summary>
        /// Унаследнованный метод из интерфейса IDragHandler, который определяет действия при захвате указателем и перемещении объекта GUI
        /// </summary>
        /// <param name="eventData"></param>   
        public void OnDrag(PointerEventData eventData)
        {
            if (!dragging)
                shift = (Vector2)label.rectTransform.position - eventData.position;

            dragging = true;
            label.rectTransform.position = eventData.position + shift;
            //textLifeTime = 1000;
        }

        /// <summary>
        /// Унаследнованный метод из интерфейса IEndDragHandler, который определяет действия при окончании перемещения указателем объекта GUI
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            dragging = false;
            //textLifeTime = 3f;
        }
    }

}