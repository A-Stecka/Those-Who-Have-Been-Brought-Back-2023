using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundEffectsController : MonoBehaviour
{
    public Slider slider;
    private AudioSource[] _audioSources;

    void Awake()
    {
        _audioSources = GetComponentsInChildren<AudioSource>();
        if (GlobalController.Instance != null)
        {
            for (int i = 0; i < _audioSources.Length; i++)
            {
                _audioSources[i].volume = GlobalController.Instance.soundEffectsVolume;
            }
            slider.value = GlobalController.Instance.soundEffectsVolume;
        }
    }

    private void OnDestroy()
    {
        SaveVolume();
    }

    private void SaveVolume()
    {
        if (_audioSources.Length != 0)
        {
            GlobalController.Instance.soundEffectsVolume = _audioSources[0].volume;
        }
    }

    public void OnSliderValueChanged()
    {
        for (int i = 0; i < _audioSources.Length; i++)
        {
            _audioSources[i].volume = slider.value;
        }
        SaveVolume();
    }
}
