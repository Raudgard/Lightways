using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

using System.Linq;
using UnityEngine.UI;
using TMPro;


public class LogotypeController : MonoBehaviour
{
    [SerializeField] private float speedForStartBeam;
    [SerializeField] private float speedForLettersBeams;
    [SerializeField] private float maxIntensityForMainLightInSpere;
    [SerializeField] private float maxIntensityForCenterLightInSpere;

    [SerializeField] private float intensityUpSpeed;
    [SerializeField] private int rateOverTimeForEmission;
    [Range(0, 1)]
    [SerializeField] private float minBloomIntensity;
    [Range(0, 1)]
    [SerializeField] private float maxBloomIntensity;
    [Range(0, 1)]
    [SerializeField] private float normalBloomIntensity;
    [SerializeField] private float speedOfIncreasingBloomIntensity;
    [SerializeField] private float speedOfDecreasingBloomIntensity;

    //[SerializeField] private PostProcessVolume postProcessVolume;
    [SerializeField] private Volume postProcessVolume;




    [SerializeField] private Vector3 finishCoordinateForStartBeam;
    [SerializeField] private Transform sphereTransform;
    //[SerializeField] private Light ligthInSphere;
    [SerializeField] private Light2D mainLigthInSphere;
    [SerializeField] private Light2D centerLigthInSphere;


    [SerializeField] private ParticleSystem particlesInSpere;

    [SerializeField] private LineRenderer startBeam;
    [SerializeField] private LineRenderer beamToSphere;
    [SerializeField] private LineRenderer beamToCenterFromLeft;
    [SerializeField] private LineRenderer beamToCenterFromRight;
    [SerializeField] private Vector3 finishCoordinateForCenterBeams;


    [SerializeField] private LineRenderer[] lettersBeams;

    [SerializeField] private Button[] buttonsOfMainMenu;
    [SerializeField] private TextMeshProUGUI[] textOfMainMenuButtons;
    [SerializeField] private Image[] bordersOfMainMenuButtons;


    [SerializeField] private Color disabledColor;
    [HideInInspector] public bool isLogoLoaded = false;


    /// <summary>
    /// Было ли в этой игровой сессии уже загружено лого.
    /// </summary>
    private static bool IsLogoHasBeenLoaded1Time;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        IsLogoHasBeenLoaded1Time = false;
    }

    private void Awake()
    {
        //SetActiveAllLetters(false);
        FadeMainMenuButtons();
    }

    private void Start()
    {
        //Debug.Log($"IsLogoHasBeenLoaded1Time: {IsLogoHasBeenLoaded1Time}");
        if (!IsLogoHasBeenLoaded1Time)
        { 
            OnGameStart();
            IsLogoHasBeenLoaded1Time = true;
        }
        else
        {
            SetActiveAllLetters(true);
            //EnableMainMenuButtons(true);
            StartCoroutine(LightUpSphereAndButtons());
            UIController.Instance.FastLoadLogotypeCoverSetActive(false);

        }
    }

    private void SetActiveAllLetters(bool active)
    {
        foreach (var renderer in lettersBeams)
        {
            renderer.gameObject.SetActive(active);
        }
    }




    public void OnGameStart()
    {
        EnableMainMenuButtons(false);
        StartCoroutine(StartingBeams());
    }

    private IEnumerator StartingBeams()
    {
        yield return StartCoroutine(StartingBeamGo());
        StartCoroutine(StartingBeamGoEnd());
        yield return StartCoroutine(LetterStart(lettersBeams));
        yield return StartCoroutine(OriginsBeamsToCenter());
        StartCoroutine(EndOfBeamsToCenter());
        yield return StartCoroutine(OriginBeamToSphere());
        StartCoroutine(LightUpSphereAndButtons());
        StartCoroutine(EndBeamToSphere());
        UIController.Instance.FastLoadLogotypeCoverSetActive(false);
    }

    /// <summary>
    /// Запускает начало стартого луча.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartingBeamGo()
    {
        float t =  0;
        Vector3 startPosition = startBeam.GetPosition(0);

        var particlesTransform = startBeam.GetComponentInChildren<ParticleSystem>().transform;
        particlesTransform.position = startPosition;

        while (t < 1)
        {
            t += Time.deltaTime * speedForStartBeam;
            startBeam.SetPosition(0, Vector3.Lerp(startPosition, finishCoordinateForStartBeam, t));
            particlesTransform.position = Vector3.Lerp(startPosition, finishCoordinateForStartBeam, t);
            yield return null;
        }
    }

    /// <summary>
    /// Запускает конец стартого луча.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartingBeamGoEnd()
    {
        float t = 0;
        Vector3 startPosition = startBeam.GetPosition(1);
        while (t < 1)
        {
            t += Time.deltaTime * speedForStartBeam;
            startBeam.SetPosition(1, Vector3.Lerp(startPosition, finishCoordinateForStartBeam, t));
            yield return null;
        }

        yield return new WaitForSeconds(10);
        startBeam.gameObject.SetActive(false);
    }

    /// <summary>
    /// Зажигает поочередно все буквы.
    /// </summary>
    /// <param name="renderers"></param>
    /// <returns></returns>
    private IEnumerator LetterStart(params LineRenderer[] renderers)
    {
        for(int i = 0; i < renderers.Length; i++)
        {
            Vector3 finishPosition = renderers[i].GetPosition(1);
            Vector3 startPosition = renderers[i].GetPosition(0);
            renderers[i].SetPosition(1, startPosition);
            renderers[i].gameObject.SetActive(true);
            float t = 0;

            var particlesTransform = renderers[i].GetComponentInChildren<ParticleSystem>().transform;
            particlesTransform.position = startPosition;

            while (t < 1)
            {
                t += Time.deltaTime * speedForLettersBeams;
                renderers[i].SetPosition(1, Vector3.Lerp(startPosition, finishPosition, t));
                particlesTransform.position = Vector3.Lerp(startPosition, finishPosition, t);
                yield return null;
            }
        }
    }


    /// <summary>
    /// Отправляет начало лучей справа и слева к букве t.
    /// </summary>
    /// <returns></returns>
    private IEnumerator OriginsBeamsToCenter()
    {
        float t = 0;
        Vector3 startPositionLeft = beamToCenterFromLeft.GetPosition(1);
        Vector3 startPositionRight = beamToCenterFromRight.GetPosition(1);
        beamToCenterFromLeft.gameObject.SetActive(true);
        beamToCenterFromRight.gameObject.SetActive(true);

        var particlesTransformLeft = beamToCenterFromLeft.GetComponentInChildren<ParticleSystem>(true).transform;
        particlesTransformLeft.position = startPositionLeft;

        var particlesTransformRight = beamToCenterFromRight.GetComponentInChildren<ParticleSystem>(true).transform;
        particlesTransformRight.position = startPositionRight;
        yield return null;

        while (t < 1)
        {
            t += Time.deltaTime * speedForStartBeam;
            beamToCenterFromLeft.SetPosition(1, Vector3.Lerp(startPositionLeft, finishCoordinateForCenterBeams, t));
            beamToCenterFromRight.SetPosition(1, Vector3.Lerp(startPositionRight, finishCoordinateForCenterBeams, t));

            particlesTransformLeft.position = Vector3.Lerp(startPositionLeft, finishCoordinateForCenterBeams, t);
            particlesTransformRight.position = Vector3.Lerp(startPositionRight, finishCoordinateForCenterBeams, t);

            yield return null;
        }
    }

    /// <summary>
    /// Отправляет конец лучей справа и слева к букве t.
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndOfBeamsToCenter()
    {
        float t = 0;
        Vector3 startPositionLeft = beamToCenterFromLeft.GetPosition(0);
        Vector3 startPositionRight = beamToCenterFromRight.GetPosition(0);

        while (t < 1)
        {
            t += Time.deltaTime * speedForStartBeam;
            beamToCenterFromLeft.SetPosition(0, Vector3.Lerp(startPositionLeft, finishCoordinateForCenterBeams, t));
            beamToCenterFromRight.SetPosition(0, Vector3.Lerp(startPositionRight, finishCoordinateForCenterBeams, t));
            yield return null;
        }

        yield return new WaitForSeconds(10);

        beamToCenterFromLeft.gameObject.SetActive(false);
        beamToCenterFromRight.gameObject.SetActive(false);
    }

    /// <summary>
    /// Отправляет начало луча от буквы t к сфере.
    /// </summary>
    /// <returns></returns>
    private IEnumerator OriginBeamToSphere()
    {
        float t = 0;
        Vector3 startPosition = beamToSphere.GetPosition(0);
        beamToSphere.gameObject.SetActive(true);

        var particlesTransform = beamToSphere.GetComponentInChildren<ParticleSystem>().transform;
        particlesTransform.position = startPosition;
        yield return null;

        while (t < 1)
        {
            t += Time.deltaTime * speedForStartBeam;
            beamToSphere.SetPosition(0, Vector3.Lerp(startPosition, sphereTransform.position, t));
            particlesTransform.position = Vector3.Lerp(startPosition, sphereTransform.position, t);
            
            yield return null;
        }
    }

    /// <summary>
    /// Отправляет конец луча от буквы t к сфере.
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndBeamToSphere()
    {
        float t = 0;
        Vector3 startPosition = beamToSphere.GetPosition(1);
        while (t < 1)
        {
            t += Time.deltaTime * speedForStartBeam;
            beamToSphere.SetPosition(1, Vector3.Lerp(startPosition, sphereTransform.position, t));
            yield return null;
        }

        yield return new WaitForSeconds(10);
        beamToSphere.gameObject.SetActive(false);
    }


    private IEnumerator LightUpSphereAndButtons()
    {
        var emission = particlesInSpere.emission;
        emission.rateOverTime = rateOverTimeForEmission;

        float t = 0;
        float startIntensity = 0;
        while (t < 1)
        {
            t += Time.deltaTime * intensityUpSpeed;
            mainLigthInSphere.intensity = Mathf.Lerp(startIntensity, maxIntensityForMainLightInSpere, t);
            centerLigthInSphere.intensity = Mathf.Lerp(startIntensity, maxIntensityForCenterLightInSpere, t);


            ColorBlock buttonColors = buttonsOfMainMenu[0].colors;
            foreach (var button in buttonsOfMainMenu)
            {
                buttonColors.normalColor = Color.Lerp(new Color(1, 1, 1, 0), Color.white, t);
                buttonColors.disabledColor = Color.Lerp(new Color(1, 1, 1, 0), disabledColor, t);
                button.colors = buttonColors;
            }

            foreach(var tmps in textOfMainMenuButtons)
            {
                tmps.color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, t);
            }

            foreach (var border in bordersOfMainMenuButtons)
            {
                border.color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, t);
            }

            yield return null;
        }

        EnableMainMenuButtons(true);
        StartBloomTransition();

        isLogoLoaded = true;
    }


    private void EnableMainMenuButtons(bool enable)
    {
        foreach (Button button in buttonsOfMainMenu)
            button.enabled = enable;
    }


    private void FadeMainMenuButtons()
    {
        ColorBlock buttonColors = buttonsOfMainMenu[0].colors;
        foreach (var button in buttonsOfMainMenu)
        {
            buttonColors.normalColor = new Color(1, 1, 1, 0);
            buttonColors.disabledColor = new Color(1, 1, 1, 0);
            button.colors = buttonColors;
        }

        foreach (var tmps in textOfMainMenuButtons)
        {
            tmps.color = new Color(1, 1, 1, 0);
        }

        foreach (var border in bordersOfMainMenuButtons)
        {
            border.color = new Color(1, 1, 1, 0);
        }
    }


    /// <summary>
    /// Начинает медленное изменение эффекта свечения с нарастанием и ослабеванием.
    /// </summary>
    public void StartBloomTransition()
    {
        StartCoroutine(BloomTransition());
    }


    private IEnumerator BloomTransition()
    {
        //Получаем текущий Lerp.
        float t = (postProcessVolume.weight - minBloomIntensity) / (maxBloomIntensity - minBloomIntensity);

        while (UIController.Instance.currentScreen != ActiveScreen.Game && UIController.Instance.currentScreen != ActiveScreen.IntoLevelMenu)
        {
            while (t < 1 && UIController.Instance.currentScreen != ActiveScreen.Game && UIController.Instance.currentScreen != ActiveScreen.IntoLevelMenu)
            {
                t += Time.deltaTime * speedOfIncreasingBloomIntensity;
                postProcessVolume.weight = Mathf.Lerp(minBloomIntensity, maxBloomIntensity, t);
                yield return null;
            }
            t = 0;

            while (t < 1 && UIController.Instance.currentScreen != ActiveScreen.Game && UIController.Instance.currentScreen != ActiveScreen.IntoLevelMenu)
            {
                t += Time.deltaTime * speedOfDecreasingBloomIntensity;
                postProcessVolume.weight = Mathf.Lerp(maxBloomIntensity, minBloomIntensity, t);
                yield return null;
            }
            t = 0;
        }
        postProcessVolume.weight = normalBloomIntensity;


        //yield return null;
    }

    ///Показывает либо скрывает надпись названия игры и сферу.
    public void SetActiveLogotypeObjects(bool active)
    {
        SetActiveAllLetters(active);
        sphereTransform.gameObject.SetActive(active);
        postProcessVolume.gameObject.SetActive(active);
    }

    /// <summary>
    /// Очень быстро зажигает все буквы, сферу и показывает меню.
    /// </summary>
    public void LoadLogoImmediately()
    {
        //StopCoroutine(loadLogoCoroutine);
        //startBeam.gameObject.SetActive(false);
        speedForStartBeam = speedForLettersBeams = 1000;
    }

}
