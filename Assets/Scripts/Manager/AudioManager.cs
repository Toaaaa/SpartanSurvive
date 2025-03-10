using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Sound Clip")]
    public List<AudioClip> audioList;// 0:힐, 1:장비 상호작용, 2:문.

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void PlaySFX(int index)
    {
        audioSource.PlayOneShot(audioList[index]);
    }
}
