using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Setting_Manager : MonoBehaviour
{
    private static Setting_Manager SETTING_MANAGER;
    [SerializeField] private GameObject _audioMenu, _keyBindingMenu, _keyBindingMenuButton;
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private AudioSource _bgm;
    [SerializeField] Slider _bgmVolumeSlider;
    [SerializeField] Slider _sfxVolumeSlider;
    private bool _isInSetting;

    /* Monobehavior methods */
    private void Awake()
    {
        if (SETTING_MANAGER == null)
        {
            SETTING_MANAGER = this;
            DontDestroyOnLoad(SETTING_MANAGER);
        }
        else
        { 
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        ToggleSettingMenu(false);
        ResetBGM();
    }


    /* Controlable in UI methods */
    public void ToggleSettingMenu(bool value)
    {
        if (SETTING_MANAGER != null && SETTING_MANAGER != this) // Trick to call this method as a prefab component
        { 
            SETTING_MANAGER.ToggleSettingMenu(value);
            return;
        }

        if (SceneManager.GetActiveScene().buildIndex == 0)
            _keyBindingMenuButton.SetActive(value);
        else
            _keyBindingMenuButton.SetActive(false);

        _isInSetting = value;
        _audioMenu.SetActive(value);
        _keyBindingMenu.SetActive(false);
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
    public void ToggleKeyBindingSettingMenu(bool value)
    {
        if (SETTING_MANAGER != null && SETTING_MANAGER != this) // Trick to call this method as a prefab component
        {
            SETTING_MANAGER.ToggleKeyBindingSettingMenu(value);
            return;
        }

        _audioMenu.SetActive(!value);
        _keyBindingMenu.SetActive(value);
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
    public void ResetBGM()
    {
        _bgm.Stop();
        _bgm.Play();
    }


    /* Input handler => Input action map */
    private void OnToggleSettingMenu()
    {
        ToggleSettingMenu(!_isInSetting);
    }
}