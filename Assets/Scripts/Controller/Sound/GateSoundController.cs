using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateSoundController : MonoBehaviour
{
    public AudioSource audioSource;

    public void PlaySoundEffect()
    {
        audioSource.Play();
    }

}
