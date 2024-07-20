using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fields;

/// <summary>
/// Класс, отвечающий за появление обучающих текстов в уровнях.
/// </summary>
public class Tutorial : MonoBehaviour
{
    private SwipeImageInFirstLevel swipeImageInFirstLevel;

    public void StoryLevelLoading(Difficulty difficulty, int level)
    {
        switch (difficulty)
        {
            case Difficulty.Very_light:
                switch (level)
                {
                    case 1:
                        UIController.Instance.ShowInfoOnBlack(GameController.Instance.languageController.Tutorial_beginning);

                        if (swipeImageInFirstLevel == null)
                        {
                            swipeImageInFirstLevel = Instantiate(Prefabs.Instance.swipeImageInFirstLevel);
                        }
                        else
                        {
                            swipeImageInFirstLevel.gameObject.SetActive(true);
                        }
                        break;

                    case 21:
                        UIController.Instance.ShowInfoOnBlack(GameController.Instance.languageController.Tutorial_teleports);
                        break;

                    case 31:
                        UIController.Instance.ShowInfoOnBlack(GameController.Instance.languageController.Tutorial_BH_small);
                        break;

                    case 35:
                        UIController.Instance.ShowInfoOnBlack(GameController.Instance.languageController.Tutorial_BH_medium);
                        break;

                    case 39:
                        UIController.Instance.ShowInfoOnBlack(GameController.Instance.languageController.Tutorial_BH_large);
                        break;

                    case 45:
                        UIController.Instance.ShowInfoOnBlack(GameController.Instance.languageController.Tutorial_BH_supermassive);
                        break;

                    case 51:
                        UIController.Instance.ShowInfoOnBlack(GameController.Instance.languageController.Tutorial_mode_FTL);
                        break;

                    case 56:
                        UIController.Instance.ShowInfoOnBlack(GameController.Instance.languageController.Tutorial_mode_BHR);
                        break;
                }
                break;

            case Difficulty.Light:
                if (level == 1)
                {
                    UIController.Instance.ShowInfoOnBlack(GameController.Instance.languageController.Tutorial_minusOneStar);
                }
                break;

            case Difficulty.Darkness:
                if(level == 1)
                {
                    UIController.Instance.ShowInfoOnBlack(GameController.Instance.languageController.Tutorial_darkness);
                }
                break;

            case Difficulty.Light_shadow:
            case Difficulty.Dark:
                break;
            default: throw new System.NotImplementedException();
        }
    }
}
