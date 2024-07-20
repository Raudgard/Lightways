using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Settings", order = 2)]
public class Settings : ScriptableObject
{
    //public float speedOfLightBeam;

    [Tooltip("Количество частиц \"пыли\" в одной единице расстояния.")]
    public int countParticlesOfWhiteDustByOneUnitOfDistance;

    public float sphereLightIntensity;
    public float sphereLightRange;

    public float particlesSpeedOnFinish;
    public int maxParticlesOnFinishExplosion;
    public float lightIntensityForFinishTower;

    [Tooltip("Максимальное количество уровней в каждой сложности.")]
    public int levelsCountInEachDifficulty;

    [Tooltip("Максимальное количество уровней, которые можно пропустить.")]
    public int maxSkippedLevels;

    [Tooltip("Время, в течение которого нельзя пройти уровень на большее количество звезд. Если он не был пройден на максимум.")]
    public float penaltyTimeInMinutes;

    [Tooltip("Минимальный процент созданного количества объектов в матрице от их общего количества, ниже которого матрица забраковывается. От 0% до 100%.")]
    public float minPercentageOfMatrixReadiness;

    [Tooltip("Квадратный коэффициент от квадрата дальности свечения сферы для сравнения с квадратом расстояния до другой сферы, чтобы определить считается она освещенной этой башней или нет.")]
    public float coeffSqrForLightRangeSqr;

    [Tooltip("Минимальное время до начала появления новой сферы.")]
    public float minTimeBeforeNextTowerAppearance;

    [Tooltip("Максимальное время до начала появления новой сферы.")]
    public float maxTimeBeforeNextTowerAppearance;

    [Tooltip("Количество сфер, при которых время до появления следующей сферы устанавливается в указанных выше границах. Иначе умножется на коэффициент (кол-во сфер в матрице / normalCountSphereInMatrixForTimeBeforeNextTowerAppearance)")]
    public float normalCountSphereInMatrixForTimeBeforeNextTowerAppearance;

    [Tooltip("Минимальное время до начала появления следующей стрелки.")]
    public float minTimeBeforeNextArrowAppearance;

    [Tooltip("Максимальное время до начала появления следующей стрелки.")]
    public float maxTimeBeforeNextArrowAppearance;

    [Tooltip("Скорость увеличения сферы при появлении.")]
    public float sphereAppereanceIncreaseSpeed;

    [Tooltip("Скорость уменьшения сферы при появлении.")]
    public float sphereAppereanceDecreaseSpeed;

    [Tooltip("Максимальный размер стрелки при ее появлении (от ее нормального состояния).")]
    public float sphereAppearanceMaxSize;

    [Tooltip("Величина уменьшения скорости появления стрелки.")]
    public float arrowAppearanceSpeedDecreaser;

    [Tooltip("Скорость появления backgroundSmoke.")]
    public float backgroundSmokeAppearanceSpeed;

    [Tooltip("Скорость зажигания света в стартовой башне.")]
    public float lightKindlingSpeed;

    [Tooltip("Скорость затухания глобального света в темных уровнях.")]
    public float directionalLightFadingSpeed;

    [Tooltip("Значение интенсивности глобального света в темных уровнях, при котором начинает зажигаться стартовая сфера.")]
    public float directionalLightIntensityValueWhenStartSphereMustKindling;

    [Tooltip("Максимальный размер сферы (от ее нормального состояния) при попадании в нее лучом.")]
    public float sphereOnHitMaxSize;
    
    [Tooltip("Скорость увеличения сферы при попадании в нее лучом.")]
    public float sphereOnHitIncreaseSpeed;

    [Tooltip("Скорость уменьшения сферы при попадании в нее лучом.")]
    public float sphereOnHitDecreaseSpeed;

    [Tooltip("Количество дополнительных вылетающих искр из финальной сферы при попадании в нее лучом.")]
    public float sparksCountOnHitFinishSphere;

    [Tooltip("Скорость уменьшения победной звезды при полете к очкам.")]
    public float starWinDecreaseSpeed;

    [Tooltip("Максимальный размер стрелки при ее увеличении при подсказке.")]
    public float maxSizeOnHintSizeFlashing;

    [Tooltip("Скорость изменения размера стрелки при подсказке.")]
    public float speedChangingSizeOnHintSizeFlashing;
}
