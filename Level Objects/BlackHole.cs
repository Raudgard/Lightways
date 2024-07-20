using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using Tools;

public class BlackHole : PlayObject, IClickableObject
{
    private enum ClickState
    {
        NoShow = 0,
        LetterOnlyShow = 1,
        LetterAndAreaShow = 2,
        AreaOnlyShow = 3,
    }


    public BlackHoleInfo.BlackHoleSize size;
    public ParticleSystemForceField forceField;
    [SerializeField] private GameObject letterGameObject;
    [SerializeField] private GameObject circleOfAttractionArea;

    public bool IsBHAppearCompletely => bhRendererAppear && attractionAreaAppear && lettersAppear;

    private bool bhRendererAppear = false;
    private bool attractionAreaAppear = false;
    private bool lettersAppear = false;

    private ClickState currentClickState;
    private int clickCounter = 0;
    private float origAlphaForCircleOfAttractionArea;


    private void Start()
    {
        SetStateFromOptions();
    }

    private void SetStateFromOptions()
    {
        int x = GameController.Instance.options.showBHSizeToggle.isOn ? 1 : 0;
        int y = GameController.Instance.options.showBHAreaToggle.isOn ? 1 : 0;
        int z = y > x ? 2 : 0;
        clickCounter = x + y + z;
        SetClickState(clickCounter);
    }

    public void OnClick()
    {
        SetClickState(++clickCounter);
    }


    
    private void SetClickState(int clickCount)
    {
        int remainder = clickCount % 4;
        switch (remainder)
        {
            case 0:
            default:
                letterGameObject.SetActive(false);
                circleOfAttractionArea.SetActive(false);
                break;
            case 1:
                letterGameObject.SetActive(true);
                circleOfAttractionArea.SetActive(false);
                break;

            case 2:
                letterGameObject.SetActive(true);
                circleOfAttractionArea.SetActive(true);
                break;

            case 3:
                letterGameObject.SetActive(false);
                circleOfAttractionArea.SetActive(true);
                break;
        }

        currentClickState = (ClickState)remainder;
        //Debug.Log($"clickCount: {clickCount}, remainder: {remainder}, currentClickState: {currentClickState}");

    }


    public void SetActiveForLetter(bool active) => letterGameObject.SetActive(active);



    public void SetSizeAreaAndLetterTransparent(bool transparent)
    {
        var spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        if (transparent)
        {
            origAlphaForCircleOfAttractionArea = spriteRenderers[1].color.a;
            spriteRenderers[0].color = new Color(spriteRenderers[0].color.r, spriteRenderers[0].color.g, spriteRenderers[0].color.b, 0);
            spriteRenderers[1].color = new Color(spriteRenderers[1].color.r, spriteRenderers[1].color.g, spriteRenderers[1].color.b, 0);
            GetComponentInChildren<TextMeshPro>(true).alpha = 0;
        }
        else
        {
            spriteRenderers[0].color = new Color(spriteRenderers[0].color.r, spriteRenderers[0].color.g, spriteRenderers[0].color.b, 1);
            spriteRenderers[1].color = new Color(spriteRenderers[1].color.r, spriteRenderers[1].color.g, spriteRenderers[1].color.b, origAlphaForCircleOfAttractionArea);
            GetComponentInChildren<TextMeshPro>(true).alpha = 1;
        }

    }


    /// <summary>
    /// Плавное появление черной дыры.
    /// </summary>
    /// <returns></returns>
    public void Appearance()
    {
        bhRendererAppear = false;
        var spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
        SetSizeAreaAndLetterTransparent(false);
        var spriteRendererTransform = spriteRenderer.transform;
        var originalScale = spriteRendererTransform.localScale;
        spriteRendererTransform.localScale = Vector3.zero;
        var settings = GameController.Instance.Settings;

        Vector3 maxScale = originalScale * settings.sphereAppearanceMaxSize;
        UnityTools.ChangeScaleLinearly(spriteRendererTransform, Vector3.zero, maxScale, settings.sphereAppereanceIncreaseSpeed, delegate
        {
            UnityTools.ChangeScaleLinearly(spriteRendererTransform, maxScale, originalScale, settings.sphereAppereanceIncreaseSpeed, delegate
            {
                bhRendererAppear = true;
            });
        });
    }


    public void CircleOfAttractionAreaAppearance()
    {
        attractionAreaAppear = false;
        var spriteRenderer = GetComponentsInChildren<SpriteRenderer>(true)[1];
        var spriteRendererTransform = spriteRenderer.transform;
        var originalScale = spriteRendererTransform.localScale;
        spriteRendererTransform.localScale = Vector3.zero;
        var settings = GameController.Instance.Settings;

        Vector3 maxScale = originalScale * settings.sphereAppearanceMaxSize;
        UnityTools.ChangeScaleLinearly(spriteRendererTransform, Vector3.zero, maxScale, settings.sphereAppereanceIncreaseSpeed, delegate
        {
            UnityTools.ChangeScaleLinearly(spriteRendererTransform, maxScale, originalScale, settings.sphereAppereanceIncreaseSpeed, delegate
            {
                attractionAreaAppear = true;
            });
        });
        
    }

    public void LettersAppearance()
    {
        lettersAppear = false;
        var letters = GetComponentInChildren<TextMeshPro>(true);
        var lettersTransform = letters.transform;
        var originalScale = lettersTransform.localScale;
        lettersTransform.localScale = Vector3.zero;
        var settings = GameController.Instance.Settings;

        Vector3 maxScale = originalScale * settings.sphereAppearanceMaxSize;
        UnityTools.ChangeScaleLinearly(lettersTransform, Vector3.zero, maxScale, settings.sphereAppereanceIncreaseSpeed, delegate
        {
            UnityTools.ChangeScaleLinearly(lettersTransform, maxScale, originalScale, settings.sphereAppereanceIncreaseSpeed, delegate
            {
                lettersAppear = true;
            });
        });

        
    }



}
