using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Setting_Manager : MonoBehaviour
{
    private static GameObject SETTING_MANAGER;
    [SerializeField] private GameObject _settingMenuCanvas;
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private AudioSource _bgm;
    [SerializeField] Slider _bgmVolumeSlider;
    [SerializeField] Slider _sfxVolumeSlider;

    private void Awake()
    {
        if (SETTING_MANAGER == null)
        {
            SETTING_MANAGER = this.gameObject;
            DontDestroyOnLoad(SETTING_MANAGER);
        }
        else
        { 
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        _settingMenuCanvas.SetActive(false);
        ResetBGM();
    }

    /* Controlable in UI methods */
    public void ToggleSettingMenu(bool value)
    {
        _settingMenuCanvas.SetActive(value);
        if (value)
        {
            Time.timeScale = 0;
            _bgm.Pause();
        }
        else
        {
            Time.timeScale = 1f;
            _bgm.UnPause();
        }
    }
    public void SetBgmVolume()
    {
        /* Replace NAME with corresponding string in Audio Mixer */
        _mixer.SetFloat("NAME", _bgmVolumeSlider.value);    // Delete this comment if already replace **************
    }
    public void SetSfxVolume()
    {
        /* Replace NAME with corresponding string in Audio Mixer */
        _mixer.SetFloat("NAME", _sfxVolumeSlider.value);    // Delete this comment if already replace **************
    }

    /* Other methods */
    private void ResetBGM()
    {
        _bgm.Stop();
        _bgm.Play();
    }

    /* Input handler => Input action map*/
    private void OnToggleSettingMenu()
    {
        ToggleSettingMenu(!_settingMenuCanvas.activeInHierarchy);
    }
}
