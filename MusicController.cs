using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saving;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class MusicController : MonoBehaviour
{
    #region Singleton
    private static MusicController instance;
    public static MusicController Instance
    {
        get { return instance; }
    }
    #endregion

    [SerializeField] private AudioSource audioSourceForSounds;
    [SerializeField] private AudioSource audioSourceForMusic;
    [SerializeField] private AudioSource audioSourceForTimer;


    [SerializeField] private Slider musicVolumeInOptionsSlider;
    [SerializeField] private Slider soundsVolumeInOptionsSlider;
    [SerializeField] private Slider musicVolumeInLevelMenuSlider;
    [SerializeField] private Slider soundsVolumeInLevelMenuSlider;
    [SerializeField] private TextMeshProUGUI musicVolumeInOptionsText;
    [SerializeField] private TextMeshProUGUI soundsVolumeInOptionsText;
    [SerializeField] private TextMeshProUGUI musicVolumeInLevelMenuText;
    [SerializeField] private TextMeshProUGUI soundsVolumeInLevelMenuText;





    [Space]

    [SerializeField] private AudioClip beamReturn;
    [SerializeField] private AudioClip beamSend;
    [SerializeField] private AudioClip loseLevel;
    [SerializeField] private AudioClip menuButton;
    [SerializeField] private AudioClip sliderInModesValueChange;
    [SerializeField] private AudioClip sphereIlluminated;
    [SerializeField] private AudioClip winLevel;
    [SerializeField] private AudioClip wrongSphereIlluminated;

    [SerializeField] private AudioClip starFullHitOnWinLevel;
    [SerializeField] private AudioClip starEmptyAppearOnWinLevel;
    [SerializeField] private AudioClip textAppearingOnWinLevel;
    [SerializeField] private AudioClip buttonAppearingOnWinLevel;

    [SerializeField] private AudioClip fireworkStart;
    [SerializeField] private AudioClip fireworkShortShot;

    [SerializeField] private AudioClip levelStarting;

    [SerializeField] private AudioClip starUprising;
    [SerializeField] private AudioClip starAdding;

    [SerializeField] private AudioClip teleportIn;
    [SerializeField] private AudioClip teleportOut;









    [Space]


    [Range(0, 1)]
    [SerializeField] private float menuButtonVolume;

    [Range(0, 1)]
    [SerializeField] private float sliderInModesValueChangeVolume;

    [Range(0, 1)]
    [SerializeField] private float beamSendVolume;

    [Range(0, 1)]
    [SerializeField] private float sphereIlluminatedVolume;

    [Range(0, 1)]
    [SerializeField] private float wrongSphereIlluminatedVolume;

    [Range(0, 1)]
    [SerializeField] private float winLevelVolume;

    [Range(0, 1)]
    [SerializeField] private float loseLevelVolume;

    [Range(0, 1)]
    [SerializeField] private float starFullHitOnWinLevelVolume;

    [Range(0, 1)]
    [SerializeField] private float starEmptyAppearOnWinLevelVolume;

    [Range(0, 1)]
    [SerializeField] private float textAppearingOnWinLevelVolume;

    [Range(0, 1)]
    [SerializeField] private float buttonAppearingOnWinLevelVolume;

    [Range(0, 1)]
    [SerializeField] private float fireworkStartVolume;

    [Range(0, 1)]
    [SerializeField] private float fireworkShortShotVolume;

    [Range(0, 1)]
    [SerializeField] private float levelStartingVolume;

    [Range(0, 1)]
    [SerializeField] private float starUprisingVolume;

    [Range(0, 1)]
    [SerializeField] private float starAddingVolume;

    [Range(0, 1)]
    [SerializeField] private float teleportInVolume;

    [Range(0, 1)]
    [SerializeField] private float teleportOutVolume;

    

    [Space]


    private float musicVolume;
    private float soundsVolume;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    private void Start()
    {
        audioSourceForMusic = MusicSource.Instance.musicAudioSource;
        LoadPlayerPrefs();
    }


    /// <summary>
    /// Загружает сохраненные настройки игрока по поводу включенных музыки и звуков.
    /// </summary>
    private void LoadPlayerPrefs()
    {
        musicVolume = PlayerPrefs.GetFloat(SCFPP.Options.musicVolume, 1);
        soundsVolume = PlayerPrefs.GetFloat(SCFPP.Options.soundsVolume, 1);

        MusicVolumeSliderValueChanged(musicVolume);
        SoundsVolumeSliderValueChanged(soundsVolume);
    }


    public void MusicVolumeSliderValueChanged(float value)
    {
        musicVolume = musicVolumeInOptionsSlider.value = musicVolumeInLevelMenuSlider.value = value;
        musicVolumeInOptionsText.text = musicVolumeInLevelMenuText.text = $"{(int)(value * 100)}";
        audioSourceForMusic.volume = musicVolume;
    }

    public void SoundsVolumeSliderValueChanged(float value)
    {
        soundsVolume = soundsVolumeInOptionsSlider.value = soundsVolumeInLevelMenuSlider.value = value;
        soundsVolumeInOptionsText.text = soundsVolumeInLevelMenuText.text = $"{(int)(value * 100)}";
        audioSourceForSounds.volume = soundsVolume;
    }

    public void MusicVolumeSliderPointerUp(BaseEventData baseEventData)
    {
        //Debug.Log($"MusicVolumeSliderPointerUp, musicVolume: {musicVolume}");
        PlayerPrefs.SetFloat(SCFPP.Options.musicVolume, musicVolume);
        audioSourceForMusic.volume = musicVolume;
    }

    public void SoundsVolumeSliderPointerUp(BaseEventData baseEventData)
    {
        //Debug.Log($"SoundsVolumeSliderPointerUp, soundsVolume: {soundsVolume}");
        PlayerPrefs.SetFloat(SCFPP.Options.soundsVolume, soundsVolume);
        audioSourceForSounds.volume = soundsVolume;
        MenuButtonClickSoundPlay();
    }








    public void MenuButtonClickSoundPlay() => audioSourceForSounds.PlayOneShot(menuButton, menuButtonVolume);
  
    public void SliderInModesSoundPlay() => audioSourceForSounds.PlayOneShot(sliderInModesValueChange, sliderInModesValueChangeVolume);

    public void BeamSendSoundPlay() => audioSourceForSounds.PlayOneShot(beamSend, beamSendVolume);

    public void BeamReturnSoundPlay() => audioSourceForSounds.PlayOneShot(beamReturn, beamSendVolume);

    public void SphereIlluminatedSoundPlay() => audioSourceForSounds.PlayOneShot(sphereIlluminated, sphereIlluminatedVolume);

    public void WrongSphereIlluminatedSoundPlay() => audioSourceForSounds.PlayOneShot(wrongSphereIlluminated, wrongSphereIlluminatedVolume);
    
    public void WinLevelSoundPlay() => audioSourceForSounds.PlayOneShot(winLevel, winLevelVolume);

    public void LoseLevelSoundPlay() => audioSourceForSounds.PlayOneShot(loseLevel, loseLevelVolume);

    public void StarFullOnWinLevelPlay() => audioSourceForSounds.PlayOneShot(starFullHitOnWinLevel, starFullHitOnWinLevelVolume);

    public void StarEmptyOnWinLevelPlay() => audioSourceForSounds.PlayOneShot(starEmptyAppearOnWinLevel, starEmptyAppearOnWinLevelVolume);

    public void TextAppearingOnWinLevelPlay() => audioSourceForSounds.PlayOneShot(textAppearingOnWinLevel, textAppearingOnWinLevelVolume);

    public void ButtonAppearingOnWinLevelPlay() => audioSourceForSounds.PlayOneShot(buttonAppearingOnWinLevel, buttonAppearingOnWinLevelVolume);

    public void LevelStartingPlay() => audioSourceForSounds.PlayOneShot(levelStarting, levelStartingVolume);

    public void TimerTickingPlay() => audioSourceForTimer.Play();
    public void TimerTickingStop() => audioSourceForTimer.Stop();

    public void StarUprisingPlay() => audioSourceForSounds.PlayOneShot(starUprising, starUprisingVolume);

    public void StarAddingPlay() => audioSourceForSounds.PlayOneShot(starAdding, starAddingVolume);

    public void TeleportInPlay() => audioSourceForSounds.PlayOneShot(teleportIn, teleportInVolume);

    public void TeleportOutPlay() => audioSourceForSounds.PlayOneShot(teleportOut, teleportOutVolume);





    private void FireworkStartPlay() => audioSourceForSounds.PlayOneShot(fireworkStart, fireworkStartVolume);

    private void FireworkShortShotPlay() => audioSourceForSounds.PlayOneShot(fireworkShortShot, fireworkShortShotVolume);


    public void FireworkPlay()
    {
        FireworkStartPlay();
        Tools.UnityTools.ExecuteWithDelay(() =>
        {
            FireworkShortShotPlay();
            Tools.UnityTools.ExecuteWithDelay(() =>
            {
                FireworkShortShotPlay();
            }, 0.2f);

        }, 1.5f);
    }



}
