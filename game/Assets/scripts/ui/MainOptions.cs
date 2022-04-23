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

        LeanTween.reset();
    }

    // Update is called once per frame
    void Update()
    {

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
        closeOptions.GetComponent<AudioSource>().PlayOneShot(buttonClick);
        LeanTween.scaleX(closeOptions.transform.GetChild(1).gameObject, 1.2f, 0.1f).setRepeat(3);

        //Delay for animations and sounds
        this.Wait(MainMenu.buttonDelay, () => {
            closeOptions.transform.GetChild(1).gameObject.transform.localScale = new Vector3(1, 1, 1);
            applyButton.transform.GetChild(1).gameObject.transform.localScale = new Vector3(1, 1, 1);

            LeanTween.scale(MainMenu.optionsScreen, new Vector3(0, 0, 0), 0.2f);
        });
    }

    //Apply options button
    public void ApplyOptions()
    {
        applyButton.GetComponent<AudioSource>().PlayOneShot(buttonClick);
        LeanTween.scaleX(applyButton.transform.GetChild(1).gameObject, 1.2f, 0.1f).setRepeat(3);

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
            closeOptions.transform.GetChild(1).gameObject.transform.localScale = new Vector3(1, 1, 1);
            applyButton.transform.GetChild(1).gameObject.transform.localScale = new Vector3(1, 1, 1);

            LeanTween.scale(MainMenu.optionsScreen, new Vector3(0, 0, 0), 0.2f);
        });

        //Update running text position
        SongName.UpdateStartPos();
    }
}

//Resolutions list
[System.Serializable]
public class ResItem 
{
    public int horizontal, vertical;   
}
