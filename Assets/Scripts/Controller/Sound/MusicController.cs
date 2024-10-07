using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{
    public Slider slider;
    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (GlobalController.Instance != null)
        {
            _audioSource.volume = GlobalController.Instance.musicVolume;
            slider.value = GlobalController.Instance.musicVolume;
        }
    }
    private void OnDestroy()
    {
        SaveVolume();
    }

    private void SaveVolume()
    {
        GlobalController.Instance.musicVolume = _audioSource.volume;
    }

    public void OnSliderValueChanged()
    {
        _audioSource.volume = slider.value;
        SaveVolume();
    }
}
