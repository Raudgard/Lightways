using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Fields;
using UnityEngine.Rendering.Universal;
using Tools;


/// <summary>
/// Направления стрелок.
/// </summary>
public enum Direction
{
    Up = 0, 
    UpLeft = 45,
    Left = 90,
    DownLeft = 135,
    Down = 180,
    DownRight = 225,
    Right = 270,
    UpRight = 315,
}

public class Tower : PlayObject
{
    [SerializeField] private LineRenderer lightBeamRenderer;
    [SerializeField] private Light2D lightOfTower;
    //[SerializeField] private Light2D overlightInCenter;
    [SerializeField] private ParticleSystem lightness;
    [SerializeField] private ParticleSystem whiteDustOverSphere;
    public ParticleSystem finishSphereParticleSystem;

    [SerializeField] private SpriteRenderer sphereRenderer;
    [SerializeField] private SpriteRenderer backgroundSmoke;
    //[SerializeField] private SphereAppearanceAnimation sphereAppearanceAnimation;

    [SerializeField] private Dictionary<Direction, ParticleSystem> whiteDusts = new Dictionary<Direction, ParticleSystem>();
    private Vector3 originalSpriteScale;

    public float LightIntensity
    { 
        get { return lightOfTower.intensity; }
        set { lightOfTower.intensity = value; } 
    }
    public float LightRange
    {
        //get { return lightOfTower.range; }
        //set { lightOfTower.range = value; } 
        get { return lightOfTower.pointLightOuterRadius; }
        set { lightOfTower.pointLightOuterRadius = value; }
    }


    [Tooltip("Кол-во стрелок и их направления")]
    public Direction[] directions;
    [Tooltip("Расчетные входящие направления света, заполняемые при создании уровня.")]
    public Direction[] inputDirections;

    [SerializeField] private Arrow[] arrows;

    [Tooltip("меньше -10000 => башня из ложно-победного пути, от -10000 до 0 => башня из ложного пути, 0 => стартовая башня, больше 0 => число по очереди в выигрышном пути.")]
    public int winningWayIndex = -1;        
    public bool isFinish = false;
    public bool IsStart => winningWayIndex == 0;

    [SerializeField] private bool isActive = false;


    /// <summary>
    /// Сферы с активными стрелками.
    /// </summary>
    public bool IsActive => isActive;
    //[SerializeField]
    private bool isIlluminatedAtLeastOnce = false;
    /// <summary>
    /// Была ли хоть раз освещена сфера (светом другой сферы или своим). Если хоть раз приняла значение true, не может более принять значение false.
    /// </summary>
    public bool IsIlluminatedAtLeastOnce
    {
        get { return isIlluminatedAtLeastOnce; }
        set
        {
            isIlluminatedAtLeastOnce = isIlluminatedAtLeastOnce ? true : value;
        }
    }

    /// <summary>
    /// Зажжена ли сфера.
    /// </summary>
    [HideInInspector]
    public bool IsIlluminating => lightOfTower.enabled;


    [Tooltip("Скорость стрелок при движении \"вперед\".")]
    [SerializeField] private float speedArrowsForwardMoving;
    [Tooltip("Скорость стрелок при движении \"назад\".")]
    [SerializeField] private float speedArrowsReverceMoving;
    private Coroutine movingArrowsCoroutine = null;

    /// <summary>
    /// Сфера заблокирована. Светится красным, стрелки не двигаются. В случае попадания лучом во входящее направление.
    /// </summary>
    private bool isBlockedOnBeamHitInputDirection = false;

    /// <summary>
    /// Сфера заблокирована. Светится красным, стрелки не двигаются. В случае попадания в уже светящуюся сферу.
    /// </summary>
    private bool isBlockedWhenDoubleActivation = false;


    void Start()
    {
        //CreateArrows();
        lightBeamRenderer.SetPosition(0, new Vector3(transform.position.x, transform.position.y, 0));
        lightBeamRenderer.SetPosition(1, new Vector3(transform.position.x, transform.position.y, 0));
        lightness.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    /// <summary>
    /// Активирует башню, если она стартовая.
    /// </summary>
    public void ActivateStartTower()
    {
        if (winningWayIndex == 0 && !GameController.Instance.IsEditingLevel)
        {
            ActivateTower();
        }
    }

    /// <summary>
    /// Возникает при попадании луча.
    /// </summary>
    /// <param name="beam"></param>
    public override void OnLightBeamHit(Beam beam)
    {
        base.OnLightBeamHit(beam);

        if (IsIlluminating)
        {
            BlockTowerOnDoubleActivation(true);
            return;
        }

        if (directions.Contains(beam.InputDirection))
        {
            BlockTowerOnBeamHitInputDirection(true);
            return;
        }


        var embers = Instantiate(Prefabs.Instance.embersChaos);
        embers.transform.SetParent(transform);
        embers.transform.localPosition = Vector3.zero;
        embers.transform.localScale = Vector3.one;
        SphereIncreaseAndDecrease();

        if (winningWayIndex < 0)
            GameController.Instance.FalseTowerActivated(this);
        //нужно, чтобы было после FalseTowerActivated, чтобы неверная башня, подсвечивающая последнюю неподсвеченную победную башню, не засчитывалась в неверно зажженные.
        ActivateTower();
        GameController.Instance.musicController.SphereIlluminatedSoundPlay();

        if (isFinish)
        {
            finishSphereParticleSystem.Stop();
            var emission = embers.emission;
            emission.SetBurst(emission.burstCount++, new ParticleSystem.Burst(0, GameController.Instance.Settings.sparksCountOnHitFinishSphere));
        }
    }


    public override void OnLightBeamLeft(Beam beam)
    {
        base.OnLightBeamLeft(beam);


        if (isBlockedWhenDoubleActivation)
        {
            BlockTowerOnDoubleActivation(false);
            return;
        }

        if (isBlockedOnBeamHitInputDirection)
        {
            BlockTowerOnBeamHitInputDirection(false);
            return;
        }

        //print($"input beams count: {inputBeams.Count}");

        if (inputBeams.Count == 0)
        {
            DeactivateArrows();
            TurnOnOffLight(false);
        }
    }



    private void SphereIncreaseAndDecrease()
    {
        var spriteTransform = sphereRenderer.GetComponent<Transform>();
        var settings = GameController.Instance.Settings;
        Vector3 maxSize = originalSpriteScale * settings.sphereOnHitMaxSize;

        UnityTools.ChangeScaleLinearly(spriteTransform, originalSpriteScale, maxSize, settings.sphereOnHitIncreaseSpeed, delegate
        {
            UnityTools.ChangeScaleLinearly(spriteTransform, maxSize, originalSpriteScale, settings.sphereOnHitDecreaseSpeed, null);
        }); ;

    }


    /// <summary>
    /// Активирует Башню.
    /// </summary>
    public void ActivateTower()
    {
        GameController.Instance.TowerActivated(this);
        TurnOnOffLight(true);
        isActive = true;
        movingArrowsCoroutine = StartCoroutine(MoveArrows());
    }

    private IEnumerator MoveArrows()
    {
        float t = 1;
        while (true)
        {
            while (t > 0)
            {
                t -= Time.deltaTime * speedArrowsReverceMoving;
                foreach (var arrow in arrows)
                {
                    arrow.SetArrowPosition(t);
                }
                //Debug.Log($"ratio: {ratio}");
                yield return null;
            }

            while (t < 1)
            {
                t += Time.deltaTime * speedArrowsForwardMoving;
                foreach (var arrow in arrows)
                {
                    arrow.SetArrowPosition(t);
                }
                //Debug.Log($"ratio: {ratio}");
                yield return null;
            }
        }
    }



    

    /// <summary>
    /// Деактивирует башню.
    /// </summary>
    public void DeactivateArrows()
    {
        isActive = false;
        StopCoroutine(movingArrowsCoroutine);
        foreach (var arrow in arrows)
        {
            arrow.SetArrowPosition(1);
            arrow.IsNeedToHint = false;
        }
    }


    /// <summary>
    /// Создает стрелки в соответствии с количеством и направлением, указанными в directions.
    /// </summary>
    public void CreateArrows()
    {
#if UNITY_EDITOR
        bool hasError = CheckForErrorsWhenCreatingArrows();
        //if (hasError) return;
#endif

        arrows = new Arrow[directions.Length];

        for (int i = 0; i < directions.Length; i++)
        {
            var arrow = CreateArrow();

            arrow.gameObject.name = "Arrow " + directions[i];
            arrow.direction = directions[i];
            arrow.towerOwner = this;
            //print($"arrow local position: {arrow.transform.localPosition}");
            arrow.transform.SetParent(transform);
            arrow.TakeStartingPositionsAndRotation();
            arrows[i] = arrow;
            //print($"arrow local position: {arrow.transform.localPosition}");
        }
    }


    /// <summary>
    /// Создание стрелок такое же, как и в обычном методе, но сначала идет выявление и удаление старых стрелок.
    /// </summary>
    public void CreateArrowsFromEditor()
    {
        var existingArrows = GetComponentsInChildren<Arrow>();

        foreach (var arrow in existingArrows)
        {
#if UNITY_EDITOR
            DestroyImmediate(arrow.gameObject);
#else
            Destroy(arrow);
#endif
        }

        bool hasError = CheckForErrorsWhenCreatingArrows();
        //if (hasError) return;

        arrows = new Arrow[directions.Length];

        for (int i = 0; i < directions.Length; i++)
        {
            var arrow = CreateArrow();

            arrow.gameObject.name = "Arrow " + directions[i];
            arrow.direction = directions[i];
            arrow.towerOwner = this;
            //print($"arrow local position: {arrow.transform.localPosition}");
            arrow.transform.SetParent(transform);
            arrow.TakeStartingPositionsAndRotation();
            arrows[i] = arrow;
            //print($"arrow local position: {arrow.transform.localPosition}");
        }
    }

    /// <summary>
    /// Добавляет направление в массив направлений и создает стрелку в соответствии.
    /// </summary>
    /// <param name="direction"></param>
    public void CreateDirectionAndArrow(Direction direction)
    {
        if (directions.Contains(direction))
            return;

        var directionsList = directions.ToList();
        directionsList.Add(direction);
        directions = directionsList.ToArray();

        var arrow = CreateArrow();
        arrow.gameObject.name = "Arrow " + direction;
        arrow.direction = direction;
        arrow.towerOwner = this;
        arrow.transform.SetParent(transform);
        arrow.TakeStartingPositionsAndRotation();
    }

    /// <summary>
    /// Удаляет направление в массиве направлений и соответствующую стрелку.
    /// </summary>
    /// <param name="direction"></param>
    public void DeleteDirectionAndArrow(Direction direction)
    {
        if (!directions.Contains(direction))
            return;

        var directionsList = directions.ToList();
        directionsList.Remove(direction);
        directions = directionsList.ToArray();

        var arrow = GetComponentsInChildren<Arrow>().Single(a => a.direction == direction);
        Destroy(arrow.gameObject);
    }



    /// <summary>
    /// Создает стрелку.
    /// </summary>
    /// <returns></returns>
    private Arrow CreateArrow()
    {
        //prefabs = FindObjectOfType<Prefabs>();          //необходимо делать здесь, чтобы была возможность использовать не в режиме проигрывания.
        //Arrow arrow = Instantiate(prefabs.arrowPrefab);

        Arrow arrow = Instantiate(Prefabs.Instance.arrowPrefab);
        return arrow;
    }


    private bool CheckForErrorsWhenCreatingArrows()
    {
        bool hasError = false;
        for (int i = 0; i < directions.Length; i++)
        {
            for (int k = 0; k < i; k++)
            {
                if (directions[k] == directions[i])
                {
                    Debug.LogWarning($"У башни {name} одинаковые стрелки  \"{directions[i]}\"  № {k} и № {i}!", this);
                    hasError = true;
                }
            }

            for(int k = 0; k < inputDirections.Length; k++)
            {
                if (directions[i] == inputDirections[k])
                {
                    Debug.LogWarning($"Стрелка \"{directions[i]}\" совпадает со входящим направлением!", this);
                    hasError = true;
                }
            }
            
        }
        return hasError;
    }

    /// <summary>
    /// Посылает луч света в соответствии с заданным направлением.
    /// </summary>
    /// <param name="direction"></param>
    public void SendLightBeam(Direction direction)
    {
        Beam.CreateBeam(this, direction);
        DeactivateArrows();
    }


    /// <summary>
    /// Включает либо выключает свет в башне.
    /// </summary>
    /// <param name="lightOn"></param>
    public void TurnOnOffLight(bool lightOn)
    {
        if (!lightOn)
        {
            GameController.Instance.TowerOff(this);
            var main = lightness.main;
            main.simulationSpeed = 2;
            lightness.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            whiteDustOverSphere.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        else
        {
            var main = lightness.main;
            main.simulationSpeed = 1;
            lightness.Play();
            whiteDustOverSphere.Play();
        }

        lightOfTower.enabled = lightOn;
        //overlightInCenter.enabled = lightOn;
        SetActiveForBackgroundSmoke(!lightOn);
    }

    /// <summary>
    /// Включает/выключает цветовую подсветку сферы.
    /// </summary>
    /// <param name="active"></param>
    public void SetActiveForBackgroundSmoke(bool active)
    {
        backgroundSmoke.gameObject.SetActive(active);
    }

    
    /// <summary>
    /// Блокирует/разблокирует сферу при вторичном попадании луча.
    /// </summary>
    /// <param name="block"></param>
    private void BlockTowerOnDoubleActivation(bool block)
    {
        lightOfTower.color = block ? Color.red : Color.white;
        isBlockedWhenDoubleActivation = block;
        GameController.Instance.IsBeamLaunchAllowed = !block;
    }


    /// <summary>
    /// Блокирует/разблокирует сферу при попадании луча во входящее направление.
    /// </summary>
    /// <param name="block"></param>
    private void BlockTowerOnBeamHitInputDirection(bool block)
    {
        TurnOnOffLight(block);

        if(block) 
            GameController.Instance.musicController.WrongSphereIlluminatedSoundPlay();

        lightOfTower.color = block ? Color.red : Color.white;
        isBlockedOnBeamHitInputDirection = block;
        GameController.Instance.IsBeamLaunchAllowed = !block;
    }


    /// <summary>
    /// Выставляет активность для видимых проявлений сферы: рендереры самой сферы, фона, света стартовой сферы и стрелок.
    /// </summary>
    /// <param name="active"></param>
    public void SetActiveForAllVisibleThings(bool active)
    {
        sphereRenderer.enabled = active;
        var originalColor = backgroundSmoke.color;
        backgroundSmoke.color = new Color(originalColor.r, originalColor.g, originalColor.b, active ? 1 : 0);


        if (winningWayIndex == 0)
        {
            //TurnOnOffLight(false);
            SetActiveForBackgroundSmoke(false);
        }
    }

    public void SetActiveForArrows(bool active)
    {
        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i].gameObject.SetActive(active);
        }
    }

    public void SphereAppearance()
    {
        if (isFinish)
        {
            finishSphereParticleSystem.Play();
        }

        sphereRenderer.enabled = true;
        var rendererTransform = sphereRenderer.GetComponent<Transform>();
        var originalScale = rendererTransform.localScale;
        rendererTransform.localScale = Vector3.zero;
        var settings = GameController.Instance.Settings;
        Vector3 maxScale = originalScale * settings.sphereAppearanceMaxSize;
        UnityTools.ChangeScaleLinearly(rendererTransform, Vector3.zero, maxScale, settings.sphereAppereanceIncreaseSpeed, delegate
        {
            UnityTools.ChangeScaleLinearly(rendererTransform, maxScale, originalScale, settings.sphereAppereanceIncreaseSpeed, delegate
            {
                originalSpriteScale = rendererTransform.localScale = originalScale;
                SetActiveForAllVisibleThings(true);
                GameController.Instance.appearedSpheres++;
            });
        });
    }




    /// <summary>
    /// Корутина, запускающая последовательно корутины появления стрелки.
    /// </summary>
    /// <returns></returns>
    public IEnumerator ArrowsAppearance()
    {
        foreach (var a in arrows)
        {
            //StartCoroutine(ArrowAppearance(a));
            ArrowAppearance(a);

            var time = Random.Range(GameController.Instance.Settings.minTimeBeforeNextArrowAppearance, GameController.Instance.Settings.maxTimeBeforeNextArrowAppearance);
            yield return new WaitForSeconds(time);
        }
    }

    /// <summary>
    /// Корутина, ответственная за появление каждой конкретной стрелки.
    /// </summary>
    /// <returns></returns>
    private void ArrowAppearance(Arrow arrow)
    {
        arrow.gameObject.SetActive(true);
        var _transform = arrow.GetComponent<Transform>();
        var originalScale = _transform.localScale;
        _transform.localScale = Vector3.zero;
        var settings = GameController.Instance.Settings;

        Vector3 maxScale = originalScale * settings.sphereAppearanceMaxSize;
        UnityTools.ChangeScaleLinearly(_transform, Vector3.zero, maxScale, settings.sphereAppereanceIncreaseSpeed, delegate
        {
            UnityTools.ChangeScaleLinearly(_transform, maxScale, originalScale, settings.sphereAppereanceIncreaseSpeed, null);
        });
    }


    public IEnumerator BackgroundSmokeAppearance()
    {
        var speed = GameController.Instance.Settings.backgroundSmokeAppearanceSpeed;
        float t = 0;
        var originalColor = backgroundSmoke.color;
        float a = 0;

        while (backgroundSmoke.color.a < 1)
        {
            backgroundSmoke.color = new Color(originalColor.r, originalColor.g, originalColor.b, a);
            t += Time.fixedDeltaTime;
            a += t * speed;
            yield return null;
        }
    }

    /// <summary>
    /// Медленное зажигание света.
    /// </summary>
    /// <returns></returns>
    public IEnumerator LightSlowKindlingInStartTower()
    {
        var origSphereLightIntensity = lightOfTower.intensity;
        //var origOverLightIntensity = overlightInCenter.intensity;

        var sphereLightIntensity = lightOfTower.intensity = GameController.Instance.IsDarkLevel ? 0 : 1;
        //float sphereLightIntensity = 1;

        //Debug.Log($"IsDarkLevel: {GameController.Instance.IsDarkLevel}");
        //var overLightInCenterIntensity = overlightInCenter.intensity = 0;
        var speed = GameController.Instance.Settings.lightKindlingSpeed;
        
        lightness.Play();

        lightOfTower.enabled = true;
        //overlightInCenter.enabled = true;
        float t = 0;

        while (t < 1)
        {
            lightOfTower.intensity = Mathf.Lerp(sphereLightIntensity, origSphereLightIntensity, t);
            //overlightInCenter.intensity = Mathf.Lerp(overLightInCenterIntensity, origOverLightIntensity, t);
            t += Time.fixedDeltaTime * speed;
            yield return null;
        }

        lightOfTower.intensity = origSphereLightIntensity;
        //overlightInCenter.intensity = origOverLightIntensity;

        ActivateStartTower();
    }

    /// <summary>
    /// Если обозначение подсказки не активировано, активирует и возвращет true, иначе false.
    /// </summary>
    /// <param name="rightDirection"></param>
    /// <returns></returns>
    public bool HintActivate(Direction rightDirection)
    {
        if (!directions.Contains(rightDirection))
        {
            Debug.LogError($"Сфера не содержит данное исходящее направление. Сфера: {this}, направление: {rightDirection}");
            return false;
        }

        Arrow rightArrow = arrows.Where(a => a.direction == rightDirection).FirstOrDefault();
        return rightArrow.HintActivate();
    }


    public void SetNeedHintToFalse()
    {
        foreach (var arrow in arrows)
        {
            arrow.IsNeedToHint = false;
        }
    }


}

