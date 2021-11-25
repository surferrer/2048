using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SetPanel : MonoBehaviour,View
{
    public GameObject gameObject;

    public Slider slider_sound;
    public Slider slider_music;


    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        slider_sound.value = PlayerPrefs.GetFloat(ConstValue.sound,0);
        slider_music.value = PlayerPrefs.GetFloat(ConstValue.music, 0);
        gameObject.SetActive(true);
    }

    public void SetSound()
    {
        PlayerPrefs.SetFloat(ConstValue.sound, slider_sound.value);
    }
    public void SetMusic()
    {
        PlayerPrefs.SetFloat(ConstValue.music, slider_music.value);
    }


}
