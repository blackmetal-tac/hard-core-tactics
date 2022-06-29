using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class MainOptions : MonoBehaviour
{
    private Toggle fullscreenTog, vsyncTog;
    private GameObject closeOptions, applyButton, closeBorder, applyBorder;

    private AudioClip buttonClick;
    private AudioSource audioUI;

    public List<ResItem> resolutions = new List<ResItem>();
    private int selectedRes;
    private TextMeshProUGUI resLabel;

    public AudioMixer mainMixer;
    private Slider masterVol, musicVol, effectsVol;

    // Start is called before the first frame update
    void Start()
    {
        fullscreenTog = GameObject.Find("FullScreenToggle").GetComponent<Toggle>();
        vsyncTog = GameObject.Find("VsyncToggle").GetComponent<Toggle>();

        closeOptions = GameObject.Find("CloseOptions");
        applyButton = GameObject.Find("ApplyButton");
        buttonClick = GameObject.Find("AudioManager").GetComponent<AudioSourcesUI>().clickButton;
        closeBorder = closeOptions.transform.GetChild(1).gameObject;
        applyBorder = applyButton.transform.GetChild(1).gameObject;        

        resLabel = GameObject.Find("ResolutionText").GetComponent<TextMeshProUGUI>();
        masterVol = GameObject.Find("MasterSlider").GetComponent<Slider>();
        musicVol = GameObject.Find("MusicSlider").GetComponent<Slider>();
        effectsVol = GameObject.Find("SFXSlider").GetComponent<Slider>();

        audioUI = GameObject.Find("MainUI").GetComponent<AudioSource>();

        //Set graphical options
        fullscreenTog.isOn = Screen.fullScreen;

        if (QualitySettings.vSyncCount == 0)
        {
            vsyncTog.isOn = false;
        }
        else
        {
            vsyncTog.isOn = true;
        }

        //Get current resolution and update options label
        bool foundRes = false;
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (Screen.width == resolutions[i].horizontal && Screen.height == resolutions[i].vertical)
            {
                foundRes = true;
                selectedRes = i;
                UpdateResLabel();
            }
        }

        //Add new user resolution
        if (!foundRes)
        {
            ResItem newRes = new ResItem();
            newRes.horizontal = Screen.width;
            newRes.vertical = Screen.height;
            resolutions.Add(newRes);
            selectedRes = resolutions.Count - 1;
            UpdateResLabel();
        }

        //Set volume sliders
        mainMixer.GetFloat("MasterVolume", out float vol);
        masterVol.value = vol;

        mainMixer.GetFloat("MusicVolume", out vol);
        masterVol.value = vol;

        mainMixer.GetFloat("EffectsVolume", out vol);
        masterVol.value = vol;
    }

    //Resolution buttons
    public void ResLeft()
    {
        selectedRes--;
        if (selectedRes < 0)
        {
            selectedRes = 0;
        }
        UpdateResLabel();
    }

    public void ResRight()
    {
        selectedRes++;
        if (selectedRes > resolutions.Count - 1)
        {
            selectedRes = resolutions.Count - 1;
        }
        UpdateResLabel();
    }

    //Update label resolution in options
    public void UpdateResLabel()
    {
        resLabel.text = resolutions[selectedRes].horizontal.ToString() + " x " + resolutions[selectedRes].vertical.ToString();
    } 

    //Close options button
    public void CloseMainOptions()
    {
        audioUI.PlayOneShot(buttonClick);
        MainMenu.BorderAnim(closeBorder, 1.2f, 3);

        //Delay for animations and sounds
        this.Wait(MainMenu.buttonDelay, () => {
            MainMenu.BorderAnim(closeBorder, 1f, 1);
            MainMenu.BorderAnim(applyBorder, 1f, 1);
            MainMenu.ScaleDown(gameObject);
        });
    }

    //Apply options button
    public void ApplyOptions()
    {
        audioUI.PlayOneShot(buttonClick);
        MainMenu.BorderAnim(applyBorder, 1.2f, 3);

        //Resoluton
        Screen.SetResolution(resolutions[selectedRes].horizontal, resolutions[selectedRes].vertical, fullscreenTog.isOn);

        //VSYNC
        if (vsyncTog.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }       

        //Delay for animations and sounds
        this.Wait(MainMenu.buttonDelay, () => {
            MainMenu.BorderAnim(closeBorder, 1f, 1);
            MainMenu.BorderAnim(applyBorder, 1f, 1);
            MainMenu.ScaleDown(gameObject);
        });        
    }

    //Audio settings
    public void SetMasterVolume()
    {
        mainMixer.SetFloat("MasterVolume", masterVol.value);
        PlayerPrefs.SetFloat("MasterVolume", masterVol.value);
    }

    public void SetMusicVolume()
    {
        mainMixer.SetFloat("MusicVolume", musicVol.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVol.value);
    }

    public void SetSFXVolume()
    {
        mainMixer.SetFloat("EffectsVolume", effectsVol.value);
        PlayerPrefs.SetFloat("EffectsVolume", effectsVol.value);
    }
}

//Resolutions list
[System.Serializable]
public class ResItem 
{
    public int horizontal, vertical;   
}
