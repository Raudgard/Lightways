using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Saving;


public class ColorTheme : MonoBehaviour
{
    public enum Color_Theme
    {
        Purple = 0,
        Green = 1,
        Turquoise = 2,
        Margenta = 3,
        Red = 4,
        Grey = 5,
        Orange = 6,

    }

    [Header("Game objects for color set")]
    [Space]

    [SerializeField] private SpriteRenderer background;
    [SerializeField] private LineRenderer lines;
    [SerializeField] private Image[] backgroundImages;
    [SerializeField] private GameObject spherePrefab;
    [SerializeField] private Image[] menuButtonsImages;
    [SerializeField] private Image[] selectedBorderImages;



    [Space]
    [Header("Purple theme colors")]
    [Space]

    public Color backgroundColorPurple;
    public Color linesAndBackgroundSmokeColorPurple;
    public Color menuButtonsColorPurple;
    public Sprite backgroundForImagePurple;


    [Header("Green theme colors")]
    [Space]

    public Color backgroundColorGreen;
    public Color linesAndBackgroundSmokeColorGreen;
    public Color menuButtonsColorGreen;
    public Sprite backgroundForImageGreen;


    [Header("Turquoise theme colors")]
    [Space]

    public Color backgroundColorTurquoise;
    public Color linesAndBackgroundSmokeColorTurquoise;
    public Color menuButtonsColorTurquoise;
    public Sprite backgroundForImageTurquoise;


    [Header("Margenta theme colors")]
    [Space]

    public Color backgroundColorMargenta;
    public Color linesAndBackgroundSmokeColorMargenta;
    public Color menuButtonsColorMargenta;
    public Sprite backgroundForImageMargenta;


    [Header("Red theme colors")]
    [Space]

    public Color backgroundColorRed;
    public Color linesAndBackgroundSmokeColorRed;
    public Color menuButtonsColorRed;
    public Sprite backgroundForImageRed;


    [Header("Grey theme colors")]
    [Space]

    public Color backgroundColorGrey;
    public Color linesAndBackgroundSmokeColorGrey;
    public Color menuButtonsColorGrey;
    public Sprite backgroundForImageGrey;


    [Header("Orange theme colors")]
    [Space]

    public Color backgroundColorOrange;
    public Color linesAndBackgroundSmokeColorOrange;
    public Color menuButtonsColorOrange;
    public Sprite backgroundForImageOrange;




    private Color backgroundColorCurrent;
    private Color linesAndBackgroundSmokeColorCurrent;
    private Color menuButtonsColorCurrent;
    private Sprite backgroundForImageCurrent;



    private void Start()
    {
        Color_Theme color_Theme = (Color_Theme)PlayerPrefs.GetInt(SCFPP.Options.colorTheme, 0);

        switch (color_Theme)
        {
            case Color_Theme.Purple: SetPurpleColorButtonClick(); break;
            case Color_Theme.Green: SetGreenColorButtonClick(); break;
            case Color_Theme.Turquoise: SetTurquoiseColorButtonClick(); break;
            case Color_Theme.Margenta: SetMargentaColorButtonClick(); break;
            case Color_Theme.Red: SetRedColorButtonClick(); break;
            case Color_Theme.Grey: SetGreyColorButtonClick(); break;
            case Color_Theme.Orange: SetOrangeColorButtonClick(); break;

            default: SetPurpleColorButtonClick(); break;
        }
    }



    private void SetColor(Color_Theme color_Theme, Color backgroundColor, Color linesAndBackgroundSmokeColor, Color menuButtonColor, Sprite backgroundForImages)
    {
        background.color = backgroundColor;
        lines.startColor = lines.endColor = linesAndBackgroundSmokeColor;
        spherePrefab.GetComponentsInChildren<SpriteRenderer>()[1].color = linesAndBackgroundSmokeColor;

        for (int i = 0; i < backgroundImages.Length; i++)
        {
            backgroundImages[i].sprite = backgroundForImages;
        }

        for (int i = 0; i < menuButtonsImages.Length; i++)
        {
            menuButtonsImages[i].color = menuButtonColor;
        }

        for(int i = 0; i < selectedBorderImages.Length; i++)
        {
            selectedBorderImages[i].gameObject.SetActive(i == (int)color_Theme);
        }

        PlayerPrefs.SetInt(SCFPP.Options.colorTheme, (int)color_Theme);


        backgroundColorCurrent = backgroundColor;
        linesAndBackgroundSmokeColorCurrent = linesAndBackgroundSmokeColor;
        menuButtonsColorCurrent = menuButtonColor;
        backgroundForImageCurrent = backgroundForImages;
    }


    public void SetPurpleColorButtonClick() => SetColor(Color_Theme.Purple, backgroundColorPurple, linesAndBackgroundSmokeColorPurple, menuButtonsColorPurple, backgroundForImagePurple);


    public void SetGreenColorButtonClick() => SetColor(Color_Theme.Green, backgroundColorGreen, linesAndBackgroundSmokeColorGreen, menuButtonsColorGreen, backgroundForImageGreen);

    public void SetTurquoiseColorButtonClick() => SetColor(Color_Theme.Turquoise, backgroundColorTurquoise, linesAndBackgroundSmokeColorTurquoise, menuButtonsColorTurquoise, backgroundForImageTurquoise);

    public void SetMargentaColorButtonClick() => SetColor(Color_Theme.Margenta, backgroundColorMargenta, linesAndBackgroundSmokeColorMargenta, menuButtonsColorMargenta, backgroundForImageMargenta);

    public void SetRedColorButtonClick() => SetColor(Color_Theme.Red, backgroundColorRed, linesAndBackgroundSmokeColorRed, menuButtonsColorRed, backgroundForImageRed);

    public void SetGreyColorButtonClick() => SetColor(Color_Theme.Grey, backgroundColorGrey, linesAndBackgroundSmokeColorGrey, menuButtonsColorGrey, backgroundForImageGrey);

    public void SetOrangeColorButtonClick() => SetColor(Color_Theme.Orange, backgroundColorOrange, linesAndBackgroundSmokeColorOrange, menuButtonsColorOrange, backgroundForImageOrange);


    /// <summary>
    /// Получить цвета и спрайт активные на данный момент.
    /// </summary>
    /// <param name="backgroundColor"></param>
    /// <param name="linesAndBackgroundSmokeColor"></param>
    /// <param name="menuButtonsColor"></param>
    /// <param name="backgroundForImage"></param>
    public void GetActiveColors(out Color backgroundColor, out Color linesAndBackgroundSmokeColor, out Color menuButtonsColor, out Sprite backgroundForImage)
    {
        backgroundColor = backgroundColorCurrent;
        linesAndBackgroundSmokeColor = linesAndBackgroundSmokeColorCurrent;
        menuButtonsColor = menuButtonsColorCurrent;
        backgroundForImage = backgroundForImageCurrent;
    }
}
