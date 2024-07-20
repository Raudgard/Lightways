using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fields;
using System.Linq;
using UnityToolKit;
using System;
using System.Collections.Concurrent;

public class Beam : MonoBehaviour
{
    public PlayObject emitter;
    public PlayObject reciever;
    [Tooltip("Направление луча при его отправке.")]
    [SerializeField] private Direction originDirection;
    [Tooltip("Направление луча с точки зрения объекта, в который он попадает (не всегда обратно OriginDirection из-за возможных влияний черных дыр).")]
    [SerializeField] private Direction inputDirection;
    

    public Direction OriginDirection
    {
        get { return originDirection; }
    }

    public Direction InputDirection
    {
        get { return inputDirection; }
    }

    public Beam pairedBeam;

    /*[SerializeField] private*/public LineRenderer lightBeamRenderer;

    [SerializeField] private ParticleSystem whiteDust;

    private LineMeshGenerator line;
    [SerializeField] private float leftWidthOfWhiteDust = 0.1f;
    [SerializeField] private float rightWidthOfWhiteDust = 0.1f;
    [SerializeField] private float thickOfWhiteDust = 10f;

    private float length = 0;
    public float Length => length;

    /// <summary>
    /// Уже в процессе отмены хода.
    /// </summary>
    private bool isCansellation = false;


    
    public static Beam CreateBeam(PlayObject emitter, Direction direction)
    {
        Beam beam = Instantiate(Prefabs.Instance.beamPrefab);
        beam.emitter = emitter;
        beam.originDirection = direction;
        //beam.inputDirection = beam.InputDirection; //for correct displaying in Inspector
        beam.transform.parent = emitter.transform;
        beam.transform.localPosition = Vector3.zero;
        beam.transform.localScale = Vector3.one;
        emitter.outputBeams.Add(beam);

        GameController.Instance.BeamSended(beam);
        beam.SendBeam(emitter, direction);
        return beam;
    }


    private void SendBeam(PlayObject emitter, Direction direction)
    {
        PlayObject hittenObject = TowersMatrix.Instance.GetNextObject(transform.position, direction, out var blackHolesData);

        if(blackHolesData.Count > 0)
        {
            inputDirection = blackHolesData.LastOrDefault().directionAfter.Opposite();
        }
        else
        {
            inputDirection = direction.Opposite();
        }


        this.reciever = hittenObject;
        string nameDestination = hittenObject == null ? "infinity" : $"({hittenObject.transform.position.x}, {hittenObject.transform.position.y})";
        name = $"Light Beam from ({transform.position.x}, {transform.position.y}) to {nameDestination}";
        
        GameController.Instance.musicController.BeamSendSoundPlay();
        StartCoroutine(SendingLightBeam(emitter, hittenObject, blackHolesData));

    }



    /// <summary>
    /// Посылает луч света от этой башни к назначению. Если в месте назначения есть башня, то активирует ее, и деактивирует эту.
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="reciever"></param>
    /// <returns></returns>
    private IEnumerator SendingLightBeam(PlayObject emitter, PlayObject reciever, List<BlackHoleInfluenceData> blackHolesData)
    {
        SetPointsToRenderer(reciever, blackHolesData);
        GameController.Instance.achievementsController.OnBeamSended(blackHolesData.Count());


        if (reciever != null)
        {
            if (emitter.TryGetComponent(out Tower tower))
            {
                //print("tower deactivated");
                tower.DeactivateArrows();
            }

            reciever.OnLightBeamHit(this);
        }


        yield return StartCoroutine(LengthCalc());

        List<BlackHole> blackHoles = new List<BlackHole>();
        //Debug.Log($"blackHolesData.Count: {blackHolesData.Count}");

        if (blackHolesData.Count > 0)
        {
            var BHinfos = blackHolesData.Select(bh => bh.blackHole);
            //Debug.Log($"BHinfos: {BHinfos.ToList().ToStringEverythingInARow()}");
            var BHinMatrix = TowersMatrix.Instance.BlackHoles;
            //Debug.Log($"BHinMatrix: {BHinMatrix.ToList().ToStringEverythingInARow()}");

            blackHoles = TowersMatrix.Instance.BlackHoles.Where(bh => BHinfos.Any(bhinfo => bhinfo.X == bh.transform.position.x && bhinfo.Y == bh.transform.position.y)).ToList();
        }

        if (reciever is BlackHole bh)
            blackHoles.Add(bh);

        ActivateWhiteDust(blackHoles);

        //yield return null;
    }


    private void SetPointsToRenderer(PlayObject reciever, List<BlackHoleInfluenceData> blackHolesData)
    {
        lightBeamRenderer.positionCount = 1;
        lightBeamRenderer.SetPosition(0, new Vector3(transform.position.x, transform.position.y, 0));
        int positionCounter = 0;
        var direction = originDirection; //направление луча при его вхождении в зону действия черной дыры.
        (int X, int Y) frontDirectionPoint = ((int)transform.position.x, (int)transform.position.y);//точка, с которой начинается прямое движение луча. Т.е. с начала испускания, и после окончания воздействия каждой черной дыры.

        for (int i = 0; i < blackHolesData.Count; i++)
        {
            var blackHoleD = blackHolesData[i];

            if (blackHoleD.inputPoint != frontDirectionPoint)
            {
                //Debug.Log("1. точка испускания луча НЕ совпадает с точкой начала воздействия черной дыры");
                lightBeamRenderer.positionCount++;
                lightBeamRenderer.SetPosition(++positionCounter, new Vector3(blackHoleD.inputPoint.X, blackHoleD.inputPoint.Y, 0));
                //positionCounter++;
            }

            var points = BHPoints.GetPoints(blackHoleD.blackHole, blackHoleD.directionBefore, blackHoleD.inputPoint, 0);
            lightBeamRenderer.positionCount += points.Length;
            //int endIndex = positionCounter + points.Length;

            for (int k = 0; k < points.Length; k++)
            {
                lightBeamRenderer.SetPosition(++positionCounter, points[k] + new Vector3(blackHoleD.blackHole.X, blackHoleD.blackHole.Y, 0));
            }

            lightBeamRenderer.positionCount++;
            lightBeamRenderer.SetPosition(++positionCounter, new Vector3(blackHoleD.outputPoint.X, blackHoleD.outputPoint.Y, 0));
            frontDirectionPoint = blackHoleD.outputPoint;
            direction = blackHoleD.directionAfter;
        }

        Vector3 destination;
        if (reciever == null)
        {
            //print("Нет объекта на пути");
            Vector3 turnedDestination = new Vector3(0, (TowersMatrix.Instance.sizeX + TowersMatrix.Instance.sizeY) * 1.5f, 0).TurnOnDirection(direction);
            //destination = turnedDestination + transform.position;
            destination = turnedDestination + new Vector3(frontDirectionPoint.X, frontDirectionPoint.Y, transform.position.z);
        }
        else
        {
            //print($"Попали в игровой объект {playObj.name}.");
            destination = new Vector3(reciever.transform.position.x, reciever.transform.position.y, 0);
        }



        if ((destination.x, destination.y) != frontDirectionPoint)
        {
            //Debug.Log("2. точка назначения луча НЕ совпадает с точкой окончания воздействия черной дыры");
            lightBeamRenderer.positionCount++;
            lightBeamRenderer.SetPosition(++positionCounter, destination);
        }


        


        //Debug.Log($"blackHolesData.Count: {blackHolesData.Count}");
        //Debug.Log($"lightBeamRenderer: {positions.ToList().ToStringEverythingInARow()}");

        
    }


    private IEnumerator LengthCalc()
    {
        Vector3[] positions = new Vector3[lightBeamRenderer.positionCount];
        lightBeamRenderer.GetPositions(positions);
        int m = positions.Length - 1;
        ConcurrentDictionary<Vector2, Vector2> probe = new ConcurrentDictionary<Vector2, Vector2>(); 
        
        for (int i = 0; i < m; i++)
        {
            //length += Vector2.Distance(positions[i], positions[i + 1]);
            probe.TryAdd(positions[i], positions[i + 1]);
        }

        probe.AsParallel().ForAll(p => length += Vector2.Distance(p.Key, p.Value));

        //даю 3 кадра на обработку параллельного запроса, потому что никак невозможно узнать, когда он заканчивает вычисления
        yield return null;
        yield return null;
        yield return null;
    }



    /// <summary>
    /// Добавляет/удаляет эффект пыли к лучу света.
    /// </summary>
    /// <param name="active">Добавить / удалить.</param>
    /// <param name="direction">Направление луча.</param>
    /// <param name="beamDestination">Точка окончания луча.</param>
    private void ActivateWhiteDust(Direction direction, Vector2 beamDestination)
    {
        var whiteDust = Instantiate(Prefabs.Instance.whiteDust);
        Vector3 coordinates = Calculations.GetShapeForWhiteDust(direction, transform.position, beamDestination, out float rotation, out float scaleX);

        var emission = whiteDust.emission;
        //var burst = emission.GetBurst(0);
        int particlesCount = (int)(scaleX * GameController.Instance.Settings.countParticlesOfWhiteDustByOneUnitOfDistance);
        
        var burst = new ParticleSystem.Burst(0, particlesCount);
        emission.SetBurst(0, burst);
        
        emission.rateOverTime = particlesCount;

        whiteDust.transform.parent = transform;
        whiteDust.transform.localPosition = Vector3.zero;
        whiteDust.transform.localScale = Vector3.one;
        var shapeModule = whiteDust.shape;

        shapeModule.position = coordinates;
        shapeModule.rotation = new Vector3(0, 0, rotation);
        shapeModule.scale = new Vector3(scaleX, shapeModule.scale.y, shapeModule.scale.z);



        //if (this.whiteDust.ContainsKey(direction))
        //{
        //    Destroy(this.whiteDust[direction].gameObject);
        //    this.whiteDust.Remove(direction);
        //}

        this.whiteDust = whiteDust;
    }



    private void ActivateWhiteDust(List<BlackHole> blackHoles)
    {
        //Debug.Log($"black holes: {blackHoles.ToStringEverythingInARow()}");
        line = new LineMeshGenerator(leftWidthOfWhiteDust, rightWidthOfWhiteDust, thickOfWhiteDust);
        //if(!TryGetComponent(typeof(MeshFilter), out var filter))
        //var filter = gameObject.AddComponent<MeshFilter>();
        //filter.mesh = line.mesh;
        Vector3[] positions = new Vector3[lightBeamRenderer.positionCount];
        lightBeamRenderer.GetPositions(positions);
        for (int i = 0; i < positions.Length; i++)
        {
            line.Add(positions[i]);
        }
        

        //Debug.Log($"filter mesh: {filter.mesh.vertices.ToList().ToStringEverythingInARow()}");
        //Debug.Log($"line mesh: {line.mesh.vertices.ToList().ToStringEverythingInARow()}");

        var whiteDust = Instantiate(Prefabs.Instance.whiteDust);
        var shapeModule = whiteDust.shape;

        shapeModule.position = new Vector3(0 - transform.position.x, 0 - transform.position.y, transform.position.z);
        //shapeModule.rotation = new Vector3(0, 0, rotation);
        shapeModule.scale = Vector3.one;
        shapeModule.shapeType = ParticleSystemShapeType.Mesh;
        shapeModule.meshShapeType = ParticleSystemMeshShapeType.Triangle;
        shapeModule.mesh = line.mesh;

        //Debug.Log($"triangles: {shapeModule.mesh.triangles.Length}, vertices: {shapeModule.mesh.vertices.Length}, uv: {shapeModule.mesh.uv.Length}");
        //Debug.Log($"shapeModule.mesh.vertices.Length: {shapeModule.mesh.vertices.Length}");


        var emission = whiteDust.emission;
        //var burst = emission.GetBurst(0);
        //int particlesCount = (int)(scaleX * GameController.Instance.Settings.countParticlesOfWhiteDustByOneUnitOfDistance);
        int particlesCount = (int)(GameController.Instance.Settings.countParticlesOfWhiteDustByOneUnitOfDistance * Length);

        var burst = new ParticleSystem.Burst(0, particlesCount);
        emission.SetBurst(0, burst);

        emission.rateOverTime = particlesCount;

        whiteDust.transform.parent = transform;
        whiteDust.transform.localPosition = Prefabs.Instance.whiteDust.transform.localPosition;
        whiteDust.transform.localScale = Vector3.one;

        var forces = whiteDust.externalForces;
        forces.enabled = true;
        forces.influenceFilter = ParticleSystemGameObjectFilter.List;
        foreach(var bh in blackHoles)
        {
            forces.AddInfluence(bh.forceField);
        }
        


        //if (this.whiteDust.ContainsKey(direction))
        //{
        //    Destroy(this.whiteDust[direction].gameObject);
        //    this.whiteDust.Remove(direction);
        //}

        this.whiteDust = whiteDust;


    }



    public void ReturnMove()
    {
        if (isCansellation)
            return;

        isCansellation = true;
        GameController.Instance.musicController.BeamReturnSoundPlay();

        if (reciever != null)
        {
            reciever.OnLightBeamLeft(this);
        }

        if (emitter.outputBeams.Contains(this))
            emitter.outputBeams.Remove(this);

        if(emitter.TryGetComponent(out Tower tower))
        {
            tower.ActivateTower();
        }

        if (pairedBeam != null)
        {
            if(emitter.TryGetComponent(out Teleport teleport))
            {
                //print($"emitter: {emitter}");
                teleport.UnpairLastTeleportsPair();
            }
            


            pairedBeam.ReturnMove();
        }

        GameController.Instance.BeamWasCanselled(this);
        Destroy(gameObject);
    }


//#if UNITY_EDITOR
//    private void OnDrawGizmos()
//    {
//        if (line != null)
//            line.DrawGizmos(this.transform);
//    }

//#endif











}
