using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public AudioSource audioSource;
    public float MusicVolume;
    public Slider volumeSlider;
    public bool NoPlayerPrefs;

    // Start is called before the first frame update

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        if (NoPlayerPrefs)
        {
            PlayerPrefs.SetFloat("volume", 0.5f);
        }

        MusicVolume = PlayerPrefs.GetFloat("volume");
        audioSource.volume = MusicVolume;
        volumeSlider.value = MusicVolume;
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.volume = MusicVolume;
        PlayerPrefs.SetFloat("volume", MusicVolume);
    }

    public void VolumeUpdater(float volume)
    {
        MusicVolume = volume;
    }
}
