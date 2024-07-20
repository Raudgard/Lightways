using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Saving;
using Fields;
using System;
using UnityEngine.Events;

namespace UserLevels
{
    /// <summary>
    /// Управляет строкой с пользовательским уровнем, загруженным c сервера.
    /// </summary>
    public class UserLevelLabel : MonoBehaviour
    {
        [SerializeField] private Button mainButton;
        [SerializeField] private TextMeshProUGUI levelNameTMPro;
        [SerializeField] private TextMeshProUGUI authorNameTMPro;
        [SerializeField] private Image fullStarsImage;

        [SerializeField] private TextMeshProUGUI ratingTMPro;
        [SerializeField] private TextMeshProUGUI launchedTMpro;
        [SerializeField] private TextMeshProUGUI passedTMpro;
        [SerializeField] private TextMeshProUGUI ratedTMpro;
        [SerializeField] private TextMeshProUGUI rewardTMpro;
        [SerializeField] private Image levelPassedCheckmark;

        private uint levelNumber;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userLevelInfo">Инфо об уровне БЕЗ самого уровня. Поле level пусто (не используется).</param>
        /// <param name="action">Метод, исполняемый при нажатии на кнопку (загружающий и запускающий уровень).</param>
        public void Initialize(UserLevelInfo userLevelInfo, UnityAction<uint, string, uint> action)
        {
            GameController gameController = GameController.Instance;
            /*levelName = */levelNameTMPro.text = userLevelInfo.levelName;
            authorNameTMPro.text = userLevelInfo.userName;
            fullStarsImage.fillAmount = userLevelInfo.rating / 5;
            ratingTMPro.text = Math.Round(userLevelInfo.rating, 2).ToString();
            launchedTMpro.text = gameController.languageController.launched + ":\n" + userLevelInfo.launched;
            passedTMpro.text = gameController.languageController.passed + ":\n" + userLevelInfo.passed;
            ratedTMpro.text = gameController.languageController.rated + ":\n" + userLevelInfo.rated;
            rewardTMpro.text = gameController.languageController.reward + ":\n" + userLevelInfo.reward;
            levelNumber = userLevelInfo.levelNumber;

            if(gameController.StateData.UserId == userLevelInfo.userID || gameController.StateData.HasThisLevelAlreadyBeenPassed(levelNumber))
            {
                levelPassedCheckmark.gameObject.SetActive(true);
            }


            gameController.colorTheme.GetActiveColors(out _, out Color linesColor, out _, out _);
            mainButton.GetComponent<Image>().color = linesColor;

            mainButton.onClick.AddListener(() => { action?.Invoke(levelNumber, userLevelInfo.levelName, userLevelInfo.reward); });
        }

    }
}