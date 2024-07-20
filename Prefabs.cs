using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Modes;
using Fields;

public class Prefabs : MonoBehaviour
{
    private static Prefabs instance;
    public static Prefabs Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else Destroy(this);
    }


    public Tower towerPrefab;
    public Arrow arrowPrefab;
    public Teleport teleportPrefab;
    public ParticleSystem pariclesForFinishTower;
    public ParticleSystem whiteDust;
    public ParticleSystemForceField forceFieldForFinish;
    public Beam beamPrefab;
    public TrailRenderer beamInMainMenuPrefab;
    public BlackHole blackHoleSmallPrefab;
    public BlackHole blackHoleMediumPrefab;
    public BlackHole blackHoleLargePrefab;
    public BlackHole blackHoleSupermassivePrefab;

    public RectTransform starEmptyForLevelButton;
    public RectTransform starFilledForLevelButton;
    public RectTransform starEmptyForUI;
    public RectTransform starFilledForUI;
    public ParticleSystem starEmptyForWinLabel;
    public ParticleSystem starFilledForWinLabel;
    public Image timeBarForLevelButton;

    [Tooltip("Изображение черной дыры, которое передвигается на поле в Black Holes Riddles.")]
    public BlackHoleDrag blackHoleToDrag;


    [Tooltip("Координаты объекта зеленым зветом. Для отладки.")]
    public TextMeshPro playObjectCoordinates;

    [Tooltip("Кольцо вокруг сферы, появляющееся при нажатии. Для Black Holes Riddles.")]
    public MarkCircleByClick markCircleByClick;

    [Tooltip("Префаб объекта изображения свайпа в первом уровне.")]
    public SwipeImageInFirstLevel swipeImageInFirstLevel;

    [Tooltip("Префаб строки в доске лидеров.")]
    public RectTransform leaderboardLine;

    [Tooltip("Префаб буквы в центре сферы для режима создания уровней.")]
    public TextMeshPro letterInSphere;

    [Tooltip("Префаб строки с кнопкой загрузки уровня, созданного пользователем устройства.")]
    public GameObject currentUserLevelButton;

    [Tooltip("Префаб строки пользовательского уровня, загруженного с сервера.")]
    public UserLevels.UserLevelLabel userLevelLabel;

    [Tooltip("Префаб вылетающих из финишной сферы частиц при победе в уровне.")]
    public ParticleSystem particlesOnFinishWhenWin;

    [Tooltip("Префаб фейерверка при победе в уровне.")]
    public ParticleSystem winLabelFirework;

    [Tooltip("Хаотично разлетающиеся частички.")]
    public ParticleSystem embersChaos;

    [Tooltip("Хаотично разлетающиеся и затем возвращающиеся частички.")]
    public ParticleSystem embersChaosReturn;

}
