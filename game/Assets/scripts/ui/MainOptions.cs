using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class MainOptions : MonoBehaviour
{
    private Toggle _fullscreenTog, _vsyncTog;
    private GameObject _closeOptions, _applyButton, _closeBorder, _applyBorder;

    private AudioClip _buttonClick;
    private AudioSource _audioUI;

    [SerializeField] private List<ResItem> _resolutions = new List<ResItem>();
    private int _selectedRes;
    private TextMeshProUGUI _resLabel;

    [SerializeField] private AudioMixer _mainMixer;
    private Slider _masterVol, _musicVol, _effectsVol;

    // Start is called before the first frame update
    void Start()
    {
        _fullscreenTog = GameObject.Find("FullScreenToggle").GetComponent<Toggle>();
        _vsyncTog = GameObject.Find("VsyncToggle").GetComponent<Toggle>();

        _closeOptions = GameObject.Find("CloseOptions");
        _applyButton = GameObject.Find("ApplyButton");
        _buttonClick = GameObject.Find("AudioManager").GetComponent<AudioSourcesUI>().ClickButton;
        _closeBorder = _closeOptions.transform.Find("ButtonBorder").gameObject;
        _applyBorder = _applyButton.transform.Find("ButtonBorder").gameObject;        

        _resLabel = GameObject.Find("ResolutionText").GetComponent<TextMeshProUGUI>();
        _masterVol = GameObject.Find("MasterSlider").GetComponent<Slider>();
        _musicVol = GameObject.Find("MusicSlider").GetComponent<Slider>();
        _effectsVol = GameObject.Find("SFXSlider").GetComponent<Slider>();

        _audioUI = GameObject.Find("MainUI").GetComponent<AudioSource>();

        //Set graphical options
        _fullscreenTog.isOn = Screen.fullScreen;

        if (QualitySettings.vSyncCount == 0)
        {
            _vsyncTog.isOn = false;
        }
        else
        {
            _vsyncTog.isOn = true;
        }

        //Get current resolution and update options label
        bool foundRes = false;
        for (int i = 0; i < _resolutions.Count; i++)
        {
            if (Screen.width == _resolutions[i].horizontal && Screen.height == _resolutions[i].vertical)
            {
                foundRes = true;
                _selectedRes = i;
                UpdateResLabel();
            }
        }

        //Add new user resolution
        if (!foundRes)
        {
            ResItem newRes = new ResItem();
            newRes.horizontal = Screen.width;
            newRes.vertical = Screen.height;
            _resolutions.Add(newRes);
            _selectedRes = _resolutions.Count - 1;
            UpdateResLabel();
        }

        //Set volume sliders
        _mainMixer.GetFloat("MasterVolume", out float vol);
        _masterVol.value = vol;

        _mainMixer.GetFloat("MusicVolume", out vol);
        _masterVol.value = vol;

        _mainMixer.GetFloat("EffectsVolume", out vol);
        _masterVol.value = vol;
    }

    //Resolution buttons
    public void ResLeft()
    {
        _selectedRes--;
        if (_selectedRes < 0)
        {
            _selectedRes = 0;
        }
        UpdateResLabel();
    }

    public void ResRight()
    {
        _selectedRes++;
        if (_selectedRes > _resolutions.Count - 1)
        {
            _selectedRes = _resolutions.Count - 1;
        }
        UpdateResLabel();
    }

    //Update label resolution in options
    public void UpdateResLabel()
    {
        _resLabel.text = _resolutions[_selectedRes].horizontal.ToString() + " x " + _resolutions[_selectedRes].vertical.ToString();
    } 

    //Close options button
    public void CloseMainOptions()
    {
        _audioUI.PlayOneShot(_buttonClick);
        MainMenu.BorderAnim(_closeBorder, 1.2f, 3);

        //Delay for animations and sounds
        this.Wait(MainMenu.ButtonDelay, () => {
            MainMenu.BorderAnim(_closeBorder, 1f, 1);
            MainMenu.BorderAnim(_applyBorder, 1f, 1);
            MainMenu.ScaleDown(gameObject);
        });
    }

    //Apply options button
    public void ApplyOptions()
    {
        _audioUI.PlayOneShot(_buttonClick);
        MainMenu.BorderAnim(_applyBorder, 1.2f, 3);

        //Resoluton
        Screen.SetResolution(_resolutions[_selectedRes].horizontal, _resolutions[_selectedRes].vertical, _fullscreenTog.isOn);

        //VSYNC
        if (_vsyncTog.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }       

        //Delay for animations and sounds
        this.Wait(MainMenu.ButtonDelay, () => {
            MainMenu.BorderAnim(_closeBorder, 1f, 1);
            MainMenu.BorderAnim(_applyBorder, 1f, 1);
            MainMenu.ScaleDown(gameObject);
        });        
    }

    //Audio settings
    public void SetMasterVolume()
    {
        _mainMixer.SetFloat("MasterVolume", _masterVol.value);
        PlayerPrefs.SetFloat("MasterVolume", _masterVol.value);
    }

    public void SetMusicVolume()
    {
        _mainMixer.SetFloat("MusicVolume", _musicVol.value);
        PlayerPrefs.SetFloat("MusicVolume", _musicVol.value);
    }

    public void SetSFXVolume()
    {
        _mainMixer.SetFloat("EffectsVolume", _effectsVol.value);
        PlayerPrefs.SetFloat("EffectsVolume", _effectsVol.value);
    }
}

//Resolutions list
[System.Serializable]
public class ResItem 
{
    public int horizontal, vertical;   
}
