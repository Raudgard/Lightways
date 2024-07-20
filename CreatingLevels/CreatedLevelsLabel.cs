using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fields;
using Saving;
using TMPro;
using System.Linq;

namespace UserLevels
{
    /// <summary>
    /// Управляет загрузкой и отображением уровней, созданных пользователем устройства.
    /// </summary>
    public class CreatedLevelsLabel : MonoBehaviour
    {
        [SerializeField] private UserLevelCreate userLevelCreateComponent;
        [SerializeField] private RectTransform contentRectT;
        [SerializeField] private AreYouSureLabel areYouSureLabel;


        void OnEnable()
        {
            CreateButtonsLines();
        }


        /// <summary>
        /// Создаем строки с кнопками загрузки сохраненных игроком уровней.
        /// </summary>
        private void CreateButtonsLines()
        {
            //Debug.Log($"CreateButtonsLines!");

            //удаляем старые кнопки
            var buttonsLines = contentRectT.GetComponentsInChildren<RectTransform>().ToList();
            if(buttonsLines.Contains(contentRectT)) buttonsLines.Remove(contentRectT);
            buttonsLines.ForEach(b => Destroy(b.gameObject));

            //получаем названия уровней.
            var levelNames = SaveLoadUtil.GetAllUserLevelsNames();
            //levelNames.ForEach(l => Debug.Log(l));

            //устанавливаем размер области для отображения кнопок.
            Vector2 sizeOfButton = Prefabs.Instance.currentUserLevelButton.GetComponent<RectTransform>().sizeDelta;
            float verticalSize = sizeOfButton.y + 20;
            float gorizontalSize = sizeOfButton.x;

            contentRectT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, levelNames.Count * verticalSize);

            //создаем кнопки
            for (int i = 0; i < levelNames.Count; i++)
            {
                var line = Instantiate(Prefabs.Instance.currentUserLevelButton);

                var rectT = line.GetComponent<RectTransform>();
                rectT.SetParent(contentRectT);
                rectT.localScale = Vector3.one;
                rectT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, verticalSize * i, verticalSize);
                rectT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, gorizontalSize);
                rectT.transform.position = new Vector3(rectT.transform.position.x, rectT.transform.position.y, -500);

                //rectT.anchoredPosition = new Vector2(rectT.anchoredPosition.x, -(rectT.sizeDelta.y + 10) * i);
                //rectT.localPosition = new Vector3(0, -(rectT.sizeDelta.y + 10) * i, 0);
                string levelName = line.GetComponentInChildren<TextMeshProUGUI>().text = levelNames[i];
                line.name = $"User level button  {levelName}";

                var buttons = line.GetComponentsInChildren<Button>();
                buttons[0].onClick.AddListener(() => { MainButtonClickHandler(levelName); });
                buttons[1].onClick.AddListener(() => { RenameLevelButtonClick(levelName); });
                buttons[2].onClick.AddListener(() => { DeleteButtonClickHandler(levelName); });


            }
        }

        private void MainButtonClickHandler(string levelName)
        {
            string fileName = $"u_{levelName}";
            var level = SaveLoadUtil.LoadUserLevelFromFile(fileName);
            if (level == null)
                return;

            userLevelCreateComponent.LoadLevel(level, levelName);
        }

       

        /// <summary>
        /// Нажатие на кнопку переименования уровня.
        /// </summary>
        private void RenameLevelButtonClick(string levelName)
        {
            userLevelCreateComponent.ShowTextInputDialog(true, levelName, () =>
            {
                userLevelCreateComponent.RenameLevel(levelName);
                CreateButtonsLines();
            });
        }

        private void DeleteButtonClickHandler(string levelName)
        {
            areYouSureLabel.Initialize(() =>
            {
                string fileName = $"u_{levelName}";
                SaveLoadUtil.DeleteUserLevelFile(fileName);
                CreateButtonsLines();
            });
        }

    }
}