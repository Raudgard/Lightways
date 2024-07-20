using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Settings", order = 2)]
public class Settings : ScriptableObject
{
    //public float speedOfLightBeam;

    [Tooltip("���������� ������ \"����\" � ����� ������� ����������.")]
    public int countParticlesOfWhiteDustByOneUnitOfDistance;

    public float sphereLightIntensity;
    public float sphereLightRange;

    public float particlesSpeedOnFinish;
    public int maxParticlesOnFinishExplosion;
    public float lightIntensityForFinishTower;

    [Tooltip("������������ ���������� ������� � ������ ���������.")]
    public int levelsCountInEachDifficulty;

    [Tooltip("������������ ���������� �������, ������� ����� ����������.")]
    public int maxSkippedLevels;

    [Tooltip("�����, � ������� �������� ������ ������ ������� �� ������� ���������� �����. ���� �� �� ��� ������� �� ��������.")]
    public float penaltyTimeInMinutes;

    [Tooltip("����������� ������� ���������� ���������� �������� � ������� �� �� ������ ����������, ���� �������� ������� ���������������. �� 0% �� 100%.")]
    public float minPercentageOfMatrixReadiness;

    [Tooltip("���������� ����������� �� �������� ��������� �������� ����� ��� ��������� � ��������� ���������� �� ������ �����, ����� ���������� ��������� ��� ���������� ���� ������ ��� ���.")]
    public float coeffSqrForLightRangeSqr;

    [Tooltip("����������� ����� �� ������ ��������� ����� �����.")]
    public float minTimeBeforeNextTowerAppearance;

    [Tooltip("������������ ����� �� ������ ��������� ����� �����.")]
    public float maxTimeBeforeNextTowerAppearance;

    [Tooltip("���������� ����, ��� ������� ����� �� ��������� ��������� ����� ��������������� � ��������� ���� ��������. ����� ��������� �� ����������� (���-�� ���� � ������� / normalCountSphereInMatrixForTimeBeforeNextTowerAppearance)")]
    public float normalCountSphereInMatrixForTimeBeforeNextTowerAppearance;

    [Tooltip("����������� ����� �� ������ ��������� ��������� �������.")]
    public float minTimeBeforeNextArrowAppearance;

    [Tooltip("������������ ����� �� ������ ��������� ��������� �������.")]
    public float maxTimeBeforeNextArrowAppearance;

    [Tooltip("�������� ���������� ����� ��� ���������.")]
    public float sphereAppereanceIncreaseSpeed;

    [Tooltip("�������� ���������� ����� ��� ���������.")]
    public float sphereAppereanceDecreaseSpeed;

    [Tooltip("������������ ������ ������� ��� �� ��������� (�� �� ����������� ���������).")]
    public float sphereAppearanceMaxSize;

    [Tooltip("�������� ���������� �������� ��������� �������.")]
    public float arrowAppearanceSpeedDecreaser;

    [Tooltip("�������� ��������� backgroundSmoke.")]
    public float backgroundSmokeAppearanceSpeed;

    [Tooltip("�������� ��������� ����� � ��������� �����.")]
    public float lightKindlingSpeed;

    [Tooltip("�������� ��������� ����������� ����� � ������ �������.")]
    public float directionalLightFadingSpeed;

    [Tooltip("�������� ������������� ����������� ����� � ������ �������, ��� ������� �������� ���������� ��������� �����.")]
    public float directionalLightIntensityValueWhenStartSphereMustKindling;

    [Tooltip("������������ ������ ����� (�� �� ����������� ���������) ��� ��������� � ��� �����.")]
    public float sphereOnHitMaxSize;
    
    [Tooltip("�������� ���������� ����� ��� ��������� � ��� �����.")]
    public float sphereOnHitIncreaseSpeed;

    [Tooltip("�������� ���������� ����� ��� ��������� � ��� �����.")]
    public float sphereOnHitDecreaseSpeed;

    [Tooltip("���������� �������������� ���������� ���� �� ��������� ����� ��� ��������� � ��� �����.")]
    public float sparksCountOnHitFinishSphere;

    [Tooltip("�������� ���������� �������� ������ ��� ������ � �����.")]
    public float starWinDecreaseSpeed;

    [Tooltip("������������ ������ ������� ��� �� ���������� ��� ���������.")]
    public float maxSizeOnHintSizeFlashing;

    [Tooltip("�������� ��������� ������� ������� ��� ���������.")]
    public float speedChangingSizeOnHintSizeFlashing;
}
