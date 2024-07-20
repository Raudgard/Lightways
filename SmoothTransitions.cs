using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SmoothTransitions
{
    public static IEnumerator SmoothColor(TextMeshProUGUI textUGUI, Color beginColor, Color targetColor, float speed)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * speed;
            textUGUI.color = Color.Lerp(beginColor, targetColor, t);
            yield return null;
        }
    }


    

}
