using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fields;
using System.Linq;
using UnityEngine.UI;

public class Hint : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI minusKeysCountText;
    [SerializeField] private GameObject minusKeys;


    public int hintCostForPathOfLight;
    public int hintCostForRandomPhotons;
    public int hintCostForFasterThanLight;
    public int hintCostForBlackHolesRiddles;

    private int currentHintCost;
    private GameController gameController;

    private IEnumerator Start()
    {
        while (GameController.Instance == null || !GameController.Instance.isStateLoaded)
        {
            yield return null;
        }
        gameController = GameController.Instance;

        if (gameController.IsGamePurchased)
        {
            minusKeys.SetActive(false);
        }
        else
        {
            gameController.onLevelLoaded += OnLevelLoad;
        }
    }


    public void OnLevelLoad()
    {
        //Debug.Log($"loaded level type: {gameController.currentLevelStateData.levelType}");

        switch (GameController.Instance.currentLevelStateData.levelType)
        {
            case Saving.LevelType.PathOfLight:
                minusKeysCountText.text = "-" + hintCostForPathOfLight;
                currentHintCost = hintCostForPathOfLight;
                    break;
            case Saving.LevelType.RandomPhotons:
                minusKeysCountText.text = "-" + hintCostForRandomPhotons;
                currentHintCost = hintCostForRandomPhotons;
                break;
            case Saving.LevelType.FasterThanLight:
                minusKeysCountText.text = "-" + hintCostForFasterThanLight;
                currentHintCost = hintCostForFasterThanLight;
                break;
            case Saving.LevelType.BlackHolesRiddles:
                minusKeysCountText.text = "-" + hintCostForBlackHolesRiddles;
                currentHintCost = hintCostForBlackHolesRiddles;
                break;
            default: throw new System.NotImplementedException();
        }
    }



    public void HintButtonClick()
    {
        //Debug.Log($"HintButtonClick");
        var gameController = GameController.Instance;

        if (!gameController.IsGamePurchased && gameController.StateData.KeysForModesCount < currentHintCost)
        {
            UIController.Instance.ShowInformationLabel(gameController.languageController.NotEnoughKeys, 2);
            return;
        }

        var levelType = gameController.currentLevelStateData.levelType;
        if (levelType == Saving.LevelType.PathOfLight || levelType == Saving.LevelType.RandomPhotons || levelType == Saving.LevelType.FasterThanLight)
        {
            var highlightedSpheres = gameController.SpheresHighlighted;
            var winSpheres = TowersMatrix.Instance.Spheres.Where(t => t.winningWayIndex >= 0).OrderBy(ws => ws.winningWayIndex). /*Select(t => t.gameObject).*/ToArray();

            Tower lastRightSphere = winSpheres[0];
            Tower nextRightSphere = winSpheres[0];

            for (int i = 0; i < highlightedSpheres.Count; i++)
            {
                if (highlightedSpheres[i] != winSpheres[i])
                {
                    lastRightSphere = winSpheres[i - 1];
                    nextRightSphere = winSpheres[i];
                    break;
                }
                else
                {
                    lastRightSphere = winSpheres[i];
                    nextRightSphere = winSpheres[i + 1];
                }
            }

            //Debug.Log($"last right highlighted sphere: {lastRightSphere}, nextRightSphere: {nextRightSphere}");

            Direction[] allDirections = lastRightSphere.directions;
            Direction rightDirection = Direction.Up;
            Teleport rightTeleport = null;

            foreach (var dir in allDirections)
            {
                var nextObject = TowersMatrix.Instance.GetNextObject(lastRightSphere.transform.position, dir, out var BHdata);
                if (nextObject == nextRightSphere)
                {
                    rightDirection = dir;
                    //Debug.Log($"rightDirection: {rightDirection}");
                    break;
                }
                else if (nextObject is Teleport teleport)
                {
                    var allExitTeleports = TowersMatrix.Instance.Teleports.ToList();
                    allExitTeleports.Remove(teleport);
                    Direction directionOut = (BHdata != null && BHdata.Count > 0) ? BHdata.LastOrDefault().directionAfter : dir;

                    foreach (var t in allExitTeleports)
                    {
                        nextObject = TowersMatrix.Instance.GetNextObject(t.transform.position, directionOut, out _);
                        if (nextObject == nextRightSphere)
                        {
                            rightDirection = dir;
                            rightTeleport = t;
                            //Debug.Log($"rightDirection: {rightDirection}, rightTeleport: {t}");
                            break;
                        }
                    }
                }
            }

            
            if (lastRightSphere.HintActivate(rightDirection))
            {
                gameController.KeySpent(currentHintCost);
            }

            if (rightTeleport != null)
            {
                rightTeleport.HintActivate(lastRightSphere);
            }

        }
        else if (levelType == Saving.LevelType.BlackHolesRiddles)
        {
            if (gameController.GetComponent<Modes.BlackHolesRiddlesController>().HintActivate())
            {
                gameController.KeySpent(currentHintCost);
            }
        }
        else
        {
            throw new System.NotImplementedException();
        }

    }

}
