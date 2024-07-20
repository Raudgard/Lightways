using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fields
{
    /// <summary>
    ///  ласс дл€ управлени€ движением изображени€ свайпа в первом уровне.
    /// </summary>
    public class SwipeImageInFirstLevel : MonoBehaviour
    {
        [SerializeField] private Vector3 startPosition;
        [SerializeField] private Vector3 endPosition;

        [Tooltip("ѕримерное врем€, за которое переместитс€ от начального положени€ к конечному.")]
        [SerializeField] private float smoothTime;
        [Tooltip(" вадрат рассто€ни€ предела. ≈сли до конечной позиции осталось меньше, то возвращает объект на начальную позицию.")]
        [SerializeField] private float tresholdSqr;
        [Tooltip("ѕауза в секундах до начала движени€.")]
        [SerializeField] private float pauseTimeOnStartMovement;

        //переменна€ дл€ сохранени€ состо€ни€ скорости движени€ в каждый момент времени. Ќеобходима дл€ работы Vector3.SmoothDamp(). 
        private Vector3 velocity;



        private void OnEnable()
        {
            UIController.Instance.onQuitLevel += delegate { if (gameObject != null) gameObject.SetActive(false); };
            GameController.Instance.onWinLevel += delegate { if (gameObject != null) gameObject.SetActive(false); };
            StartCoroutine(Movement());
        }


        private IEnumerator Movement()
        {
            while (true)
            {
                transform.position = Vector3.SmoothDamp(transform.position, endPosition, ref velocity, smoothTime);

                if ((transform.position - endPosition).sqrMagnitude < tresholdSqr)
                {
                    transform.position = startPosition;
                    yield return new WaitForSeconds(pauseTimeOnStartMovement);
                }

                yield return null;
            }
        }


    }
}