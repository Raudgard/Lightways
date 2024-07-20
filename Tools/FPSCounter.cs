using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Tools
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] bool showFPS;
        [SerializeField] private TextMeshProUGUI FPS_TPMtext;
        [SerializeField] private float FPS_ShowDelay;

        void Start()
        {
            if (showFPS)
            {
                FPS_TPMtext.gameObject.SetActive(true);
                StartCoroutine(ShowFPS());
            }
            else
            {
                FPS_TPMtext.gameObject.SetActive(false);
            }
        }

        private IEnumerator ShowFPS()
        {
            while (true)
            {
                FPS_TPMtext.text = "" + (int)(1 / Time.deltaTime);
                yield return new WaitForSeconds(FPS_ShowDelay);
            }
        }
    }
}