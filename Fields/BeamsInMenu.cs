using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fields
{
    /// <summary>
    /// ќтвечает за пролетающие врем€ от времени лучи света в главном меню.
    /// </summary>
    public class BeamsInMenu : MonoBehaviour
    {
        [Tooltip("ћинимальна€ скорость движени€ луча.")]
        [SerializeField] private float minBeamSpeed;
        [Tooltip("ћаксимальна€ скорость движени€ луча.")]
        [SerializeField] private float maxBeamSpeed;
        [Tooltip("—реднее врем€ до запуска следующего луча в секундах.")]
        [SerializeField] private float averageTimeBeforeNextBeam;
        [Tooltip("–азброс времени до запуска следующего луча в секундах.")]
        [SerializeField] private float randomTimeBeforeNextBeam;
        [Tooltip("¬рем€ жизни луча в секундах.")]
        [SerializeField] private float beamLifeTime;

        [SerializeField] private float zCoordinateForPoints;

        [Tooltip("ѕр€моугольник, внутри которого лучи живы. ѕри вылете за пределы уничтожаютс€.")]
        [SerializeField] private BoxCollider2D beamsArea;

        [SerializeField] private LogotypeController logotypeController;

        private void OnEnable()
        {
            StartCoroutine(LauchBeams());
        }

        private void OnDisable()
        {
            StopAndDestroyAllBeams();
        }

        private IEnumerator LauchBeams()
        {
            while(!logotypeController.isLogoLoaded)
            {
                yield return new WaitForSeconds(0.2f);
            }

            while (true)
            {
                var startPoint = GetRandomPointOnPerimeter();
                Vector3 endPoint;
                //int counter = 0;
                do
                {
                    endPoint = GetRandomPointOnPerimeter();
                    //Debug.Log($"counter: {++counter}");
                    //Debug.Log($"startPoint: {startPoint}");
                    //Debug.Log($"endPoint: {endPoint}");
                }
                while (startPoint.x == endPoint.x || startPoint.y == endPoint.y);

                var beam = Instantiate(Prefabs.Instance.beamInMainMenuPrefab);
                beam.transform.parent = transform;
                beam.transform.position = startPoint;
                Vector3 direction = endPoint - startPoint;

                StartCoroutine(BeamMoving(beam, direction));
                yield return new WaitForSeconds(averageTimeBeforeNextBeam + Random.Range(-randomTimeBeforeNextBeam, randomTimeBeforeNextBeam));
            }
        }

        /// <summary>
        /// ¬озвращает случайную точку на периметре пр€моугольного коллайдера.
        /// </summary>
        /// <returns></returns>
        private Vector3 GetRandomPointOnPerimeter()
        {
            float x0 = beamsArea.bounds.min.x;
            float y0 = beamsArea.bounds.min.y;
            float x1 = beamsArea.bounds.max.x;
            float y1 = beamsArea.bounds.max.y;

            var list = new float[4] { x0, y0, x1, y1 };
            var randomMember = Random.Range(0, 3);
            //Debug.Log($"randomValue: {randomMember}");

            var randomValue = list[randomMember];

            if (randomMember == 0 || randomMember == 2)
            {
                //Debug.Log($"randomMember 0, 2");
                return new Vector3(randomValue, Random.Range(y0, y1), zCoordinateForPoints);
            }
            else
            {
                //Debug.Log($"randomMember 1, 3");
                return new Vector3(Random.Range(x0, x1), randomValue, zCoordinateForPoints);
            }
        }



        private IEnumerator BeamMoving(TrailRenderer beam, Vector2 direction)
        {
            var beamTransform = beam.transform;
            //Debug.Log($"start moving on: {beamTransform.position}");
            var beamSpeed = Random.Range(minBeamSpeed, maxBeamSpeed);
            float lifeTime = beamLifeTime;
            //while (beamsArea.OverlapPoint(beamTransform.position))
            
            while (lifeTime > 0)
            {
                beamTransform.Translate(direction * beamSpeed * Time.deltaTime);
                lifeTime -= Time.deltaTime;
                yield return null;
            }

            Destroy(beamTransform.gameObject);
            //beam.GetComponentInChildren<ParticleSystem>().transform.SetParent(transform);
            //Debug.Log($"end moving on: {beamTransform.position}");
        }


        private void StopAndDestroyAllBeams()
        {
            StopAllCoroutines();
            var trails = GetComponentsInChildren<TrailRenderer>();
            foreach (var t in trails)
            {
                Destroy(t.gameObject);
            }
        }
    }
}