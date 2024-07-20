using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����, ���������� �� ������. ������������� �� ����������� singletone ������� ��� ������������ ������������.
/// </summary>
public class MusicSource : MonoBehaviour
{
    #region Singleton
    private static MusicSource instance;
    public static MusicSource Instance
    {
        get { return instance; }
    }
    #endregion

    public AudioSource musicAudioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }


    void Start()
    {
        musicAudioSource = GetComponent<AudioSource>();
    }

}
