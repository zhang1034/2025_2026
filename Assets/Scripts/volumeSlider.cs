using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider slider;

    void Start()
    {
        slider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", value);
    }
}
