using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainOptions : MonoBehaviour
{
    private Toggle fullscreenTog, vsyncTog;
    private GameObject closeOptions;
    private GameObject applyButton;

    private AudioClip buttonClick;

    public List<ResItem> resolutions = new List<ResItem>();
    private int selectedRes;
    private TextMeshProUGUI resLabel;

    // Start is called before the first frame update
    void Start()
    {
        fullscreenTog = GameObject.Find("FullScreenToggle").GetComponent<Toggle>();
        vsyncTog = GameObject.Find("VsyncToggle").GetComponent<Toggle>();
        closeOptions = GameObject.Find("CloseOptions");
        applyButton = GameObject.Find("ApplyButton");
        buttonClick = GameObject.Find("MainMenuCanvas").GetComponent<MainMenu>().buttonClick;
        resLabel = GameObject.Find("ResolutionText").GetComponent<TextMeshProUGUI>(); 

        fullscreenTog.isOn = Screen.fullScreen;

        if (QualitySettings.vSyncCount == 0)
        {
            vsyncTog.isOn = false;
        }
        else 
        {
            vsyncTog.isOn = true;
        }

        LeanTween.reset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    public void UpdateResLabel()
    {
        resLabel.text = resolutions[selectedRes].horizontal.ToString() + " x " + resolutions[selectedRes].vertical.ToString();
    }

    public void CloseMainOptions()
    {
        closeOptions.GetComponent<AudioSource>().PlayOneShot(buttonClick);
        LeanTween.scaleX(closeOptions.transform.GetChild(1).gameObject, 1.2f, 0.1f).setRepeat(3);

        this.Wait(MainMenu.buttonDelay, () => {
            closeOptions.transform.GetChild(1).gameObject.transform.localScale = new Vector3(1, 1, 1);
            applyButton.transform.GetChild(1).gameObject.transform.localScale = new Vector3(1, 1, 1);

            LeanTween.scale(MainMenu.optionsScreen, new Vector3(0, 0, 0), 0.2f);
        });
    }

    public void ApplyOptions()
    {
        applyButton.GetComponent<AudioSource>().PlayOneShot(buttonClick);
        LeanTween.scaleX(applyButton.transform.GetChild(1).gameObject, 1.2f, 0.1f).setRepeat(3);

        Screen.SetResolution(resolutions[selectedRes].horizontal, resolutions[selectedRes].vertical, fullscreenTog.isOn);

        if (vsyncTog.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }

        this.Wait(MainMenu.buttonDelay, () => {
            closeOptions.transform.GetChild(1).gameObject.transform.localScale = new Vector3(1, 1, 1);
            applyButton.transform.GetChild(1).gameObject.transform.localScale = new Vector3(1, 1, 1);

            LeanTween.scale(MainMenu.optionsScreen, new Vector3(0, 0, 0), 0.2f);
        });

        SongName.UpdateStartPos();
    }
}

[System.Serializable]
public class ResItem 
{
    public int horizontal, vertical;   
}
