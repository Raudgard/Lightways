using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fields
{
    /// <summary>
    /// ����� ��� ���������� ��������� ����������� ������ � ������ ������.
    /// </summary>
    public class SwipeImageInFirstLevel : MonoBehaviour
    {
        [SerializeField] private Vector3 startPosition;
        [SerializeField] private Vector3 endPosition;

        [Tooltip("��������� �����, �� ������� ������������ �� ���������� ��������� � ���������.")]
        [SerializeField] private float smoothTime;
        [Tooltip("������� ���������� �������. ���� �� �������� ������� �������� ������, �� ���������� ������ �� ��������� �������.")]
        [SerializeField] private float tresholdSqr;
        [Tooltip("����� � �������� �� ������ ��������.")]
        [SerializeField] private float pauseTimeOnStartMovement;

        //���������� ��� ���������� ��������� �������� �������� � ������ ������ �������. ���������� ��� ������ Vector3.SmoothDamp(). 
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