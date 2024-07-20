using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;
using Fields;
using System.Linq;
using Tools;


public class Teleport : PlayObject, IClickableObject
{
    public List<TeleportPair> teleportPairs = new List<TeleportPair>();
    [SerializeField] private LineRenderer lightBeamRenderer;
    [SerializeField] private Light2D lightMain;
    [SerializeField] private GameObject waveFromTeleport;

    private bool hintSizeFlashing = false;
    /// <summary>
    /// Необходима подсказка этим телепортом?
    /// </summary>
    public bool isNeedToHint { get; set; } = false;

    //private Dictionary<Direction, ParticleSystem> whiteDusts = new Dictionary<Direction, ParticleSystem>();

    /// <summary>
    /// True - у входящего в данный момент телепорта. В данный момент ожидает пары.
    /// </summary>
    public bool waitingForPair = false;

    /// <summary>
    /// Может быть исходящим в данный момент. (Не все могут быть исходящими для некоторых направлений во избежание закольцовывания.)
    /// </summary>
    public bool canBeOutput = false;

    private void Start()
    {
        lightBeamRenderer.SetPosition(0, new Vector3(transform.position.x, transform.position.y, 0));
        lightBeamRenderer.SetPosition(1, new Vector3(transform.position.x, transform.position.y, 0));
    }

    public void OnClick()
    {
        //print($"Teleport {transform.position.x}, {transform.position.y} clicked");
        if (!GameController.Instance.waitingForPortalChoose)
            return;

        //Если этот телепорт ожидает пары, то, соответственно, сам с собой пару создать не может.
        if (waitingForPair)
            return;

        //Если false, значит он не должен выпускать этот луч света во избежание закольцовывания.
        if (!canBeOutput)
            return;

        Teleport waitingTeleport = TowersMatrix.Instance.Teleports.Where(teleport => teleport.waitingForPair == true).FirstOrDefault();
        waitingTeleport.waitingForPair = false;
        GameController.Instance.waitingForPortalChoose = false;
        LightOffTeleportsWithoutPair();
        TowersMatrix.Instance.Teleports.ToList().ForEach(t => { t.canBeOutput = false; t.ActivateWaveFromTeleport(false); });
        //GameController.Instance.SetActiveForSwipeableArea(true);
        waitingTeleport.PairTeleports(this, waitingTeleport.inputBeams.Last());
    }

    public override void OnLightBeamHit(Beam beam)
    {
        base.OnLightBeamHit(beam);
        MusicController.Instance.TeleportInPlay();

        var embers = Instantiate(Prefabs.Instance.embersChaosReturn);
        embers.transform.SetParent(transform);
        embers.transform.localPosition = Vector3.zero;
        embers.transform.localScale = Vector3.one;

        if (TryToFindTeleportPair(out Teleport teleport))
        {
            PairTeleports(teleport, beam);
        }
        else
        {
            //Здесь будет ожидание выбора игрока.
            GameController.Instance.waitingForPortalChoose = true;
            waitingForPair = true;
            TurnOnOffLight(true);
            HighlightOutputTeleports(beam);
            //GameController.Instance.SetActiveForSwipeableArea(false);
        }

    }

    private bool TryToFindTeleportPair(out Teleport teleport)
    {
        var teleports = TowersMatrix.Instance.Teleports;
        if(teleports.Length == 2)
        {
            teleport = teleports.Where(tp => tp != this).FirstOrDefault();
            return true;
        }
        teleport = null;
        return false;
    }

    /// <summary>
    /// Соединяет два телепорта. Вызывается у ВХОДЯЩЕГО телепорта.
    /// </summary>
    /// <param name="outputTeleport">ВЫХОДЯЩИЙ телепорт.</param>
    /// <param name="originalBeam">Луч, входящий в телепорт.</param>
    public void PairTeleports(Teleport outputTeleport, Beam originalBeam)
    {
        Beam beam = this.inputBeams.Last();
        TeleportPair teleportPair = new TeleportPair()
        {
            emitter = beam.emitter,
            inputTeleport = this,
            inputDirection = beam.InputDirection,
            outputTeleport = outputTeleport,
            outputDirection = beam.InputDirection.Opposite(),
            reciever = TowersMatrix.Instance.GetNextObject(outputTeleport.transform.position, beam.OriginDirection, out _)
        };

        teleportPairs.Add(teleportPair);
        TurnOnOffLight(true);
        outputTeleport.TurnOnOffLight(true);
        outputTeleport.teleportPairs.Add(teleportPair);
        outputTeleport.SendLightBeam(teleportPair.outputDirection, originalBeam);
    }

    /// <summary>
    /// Посылает луч света в соответствии с заданным направлением.
    /// </summary>
    /// <param name="direction"></param>
    public void SendLightBeam(Direction direction, Beam originalBeam)
    {
        Beam beam = Beam.CreateBeam(this, direction);
        beam.pairedBeam = originalBeam;
        originalBeam.pairedBeam = beam;

        var embers = Instantiate(Prefabs.Instance.embersChaos);
        embers.transform.SetParent(transform);
        embers.transform.localPosition = Vector3.zero;
        embers.transform.localScale = Vector3.one;

        isNeedToHint = false;

        MusicController.Instance.TeleportOutPlay();
    }


    public void UnpairLastTeleportsPair()
    {
        Teleport pairedTeleport = teleportPairs.Last().inputTeleport;
        pairedTeleport.teleportPairs.RemoveAt(pairedTeleport.teleportPairs.Count - 1);
        teleportPairs.RemoveAt(teleportPairs.Count - 1);

        LightOffTeleportsWithoutPair();
        //if(teleportPairs.Count == 0)
        //    TurnOnOffLight(false);
        //if(pairedTeleport.teleportPairs.Count == 0)
        //    pairedTeleport.TurnOnOffLight(false);
    }

    public override void OnLightBeamLeft(Beam beam)
    {
        base.OnLightBeamLeft(beam);
        waitingForPair = false;
        LightOffTeleportsWithoutPair();
    }

    public void TurnOnOffLight(bool on)
    {
        lightMain.gameObject.SetActive(on);
    }

    /// <summary>
    /// Включает подсветку, переменную и визуальную волну у телепортов, которые могут исходящими для луча. Исключаются телепорты, луч из которых сразу попадает в другой телепорт
    /// либо в сферу испустившую данный луч света, во избежание закольцовывание луча до бесконечности.
    /// </summary>
    public void HighlightOutputTeleports(Beam beam)
    {
        var teleports = TowersMatrix.Instance.Teleports.ToList();
        teleports.Remove(this);

        foreach (var t in teleports)
        {
            var oppositeInputDirection = beam.InputDirection.Opposite();
            PlayObject hittenObject = TowersMatrix.Instance.GetNextObject(t.transform.position, oppositeInputDirection, out var BHData);

            if ((!(hittenObject is Teleport) || (BHData.Count > 0 && BHData.LastOrDefault().directionAfter != oppositeInputDirection)) && (t != beam.emitter))
            {
                bool accepted = true;
                var directionChecked = BHData.Count > 0 ? BHData.LastOrDefault().directionAfter.Opposite() : beam.InputDirection;
                if (hittenObject is Tower tower && (tower.IsIlluminating || tower.directions.Contains(directionChecked)))
                {
                    accepted = false;
                }

                //Debug.Log($"teleport: {t}, directionChecked: {directionChecked}, oppositeInputDirection: {oppositeInputDirection}, accepted: {accepted}");

                if (accepted)
                {
                    t.TurnOnOffLight(true);
                    t.canBeOutput = true;
                    t.ActivateWaveFromTeleport(true);
                }
            }
        }
    }

    /// <summary>
    /// Выключает подсветку у тех телепортов, которые не имеют пару и, соответственно, входящих-выходящих лучей.
    /// </summary>
    public static void LightOffTeleportsWithoutPair()
    {
        var teleports = TowersMatrix.Instance.Teleports.Where(t => t.teleportPairs.Count == 0);
        foreach (var teleport in teleports)
            teleport.TurnOnOffLight(false);
    }

    /// <summary>
    /// Активирует/деактивирует визуализированную волну от телепорта. Используется, когда телепорт можно выбрать для выходящего луча.
    /// </summary>
    /// <param name="activate"></param>
    public void ActivateWaveFromTeleport(bool activate)
    {
        waveFromTeleport.SetActive(activate);
    }


    public void SetActiveForSpriteRenderer(bool active) => GetComponentInChildren<SpriteRenderer>(true).gameObject.SetActive(active);


    /// <summary>
    /// Плавное появление телепорта.
    /// </summary>
    /// <returns></returns>
    public void Appearance()
    {
        GetComponentInChildren<ParticleSystem>().Play();

        SetActiveForSpriteRenderer(true);
        var renderer = GetComponentInChildren<SpriteRenderer>();
        var _transform = renderer.GetComponent<Transform>();
        var originalScale = _transform.localScale;
        _transform.localScale = Vector3.zero;
        var settings = GameController.Instance.Settings;

        Vector3 maxScale = originalScale * settings.sphereAppearanceMaxSize;
        UnityTools.ChangeScaleLinearly(_transform, Vector3.zero, maxScale, settings.sphereAppereanceIncreaseSpeed, delegate
        {
            UnityTools.ChangeScaleLinearly(_transform, maxScale, originalScale, settings.sphereAppereanceIncreaseSpeed, null);
        });
    }


    /// <summary>
    /// Выделяет этот телепорт, как правильный.
    /// </summary>
    public bool HintActivate(Tower lastRightTowerWhenHint)
    {
        if (!hintSizeFlashing)
        {
            isNeedToHint = true;
            //hintSizeFlashing = StartCoroutine(SizeFlashing());
            SizeFlashing(lastRightTowerWhenHint);
            return true;
        }
        return false;
    }


    private void SizeFlashing(Tower lastRightTowerWhenHint)
    {
        hintSizeFlashing = true;
        var _transform = transform;
        Vector3 originalScale = transform.localScale;
        Vector3 maxScale = transform.localScale * GameController.Instance.Settings.maxSizeOnHintSizeFlashing;
        float speed = GameController.Instance.Settings.speedChangingSizeOnHintSizeFlashing;


        UnityTools.ChangeScaleLinearly(_transform, originalScale, maxScale, speed, delegate
        {
            UnityTools.ChangeScaleLinearly(_transform, maxScale, originalScale, speed, delegate
            {
                if (isNeedToHint)
                {
                    SizeFlashing(lastRightTowerWhenHint);
                }
                else
                {
                    hintSizeFlashing = false;
                    lastRightTowerWhenHint.SetNeedHintToFalse();
                }
            });
        });
    }


}






/// <summary>
/// Класс, содержащий информацию о паре телепортов - получившем и отправившем луч, а также паре башен - испустившей и получившей луч и их направлениях.
/// </summary>
[Serializable]
public class TeleportPair
{
    public PlayObject emitter;
    [Space]
    public Teleport inputTeleport;
    public Direction inputDirection;
    [Space]
    public Teleport outputTeleport;
    public Direction outputDirection;
    [Space]
    public PlayObject reciever;

    /// <summary>
    /// Возвращает телепорт-пару.
    /// </summary>
    /// <param name="teleport">Телепорт, который ищет пару.</param>
    /// <returns></returns>
    public Teleport GetPairedTeleport(Teleport teleport)
    {
        if (inputTeleport == teleport) return outputTeleport;
        else return inputTeleport;
    }
}
