using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string TestLevel;
    private AudioSource audioUI;
    public AudioClip buttonClick;

    public static GameObject optionsScreen;
    private GameObject startButton, openOptions, exitButton, startBorder, optionsBorder, exitBorder;

    public static float buttonDelay = 0.4f;

    // Start is called before the first frame update
    void Start()
    {
        optionsScreen = GameObject.Find("MainOptions");
        startButton = GameObject.Find("StartButton");
        openOptions = GameObject.Find("OptionsButton");        
        exitButton = GameObject.Find("CloseButton");

        startBorder = startButton.transform.GetChild(1).gameObject;
        optionsBorder = openOptions.transform.GetChild(1).gameObject;
        exitBorder = exitButton.transform.GetChild(1).gameObject;

        audioUI = startButton.GetComponentInParent<AudioSource>();      

        //Set options screen to 0 (invisible)
        optionsScreen.transform.localScale = Vector3.zero;

        LeanTween.reset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Buttons
    public void StartGame()
    {
        audioUI.PlayOneShot(buttonClick);
        LeanTween.scaleX(startBorder, 1.2f, 0.1f).setRepeat(3);

        //Delay for animation
        this.Wait(buttonDelay, ()=> {
            startBorder.transform.localScale = Vector3.one;
            SceneManager.LoadScene(TestLevel);
        });        
    }

    public void OpenMainOptions()
    {
        audioUI.PlayOneShot(buttonClick);
        LeanTween.scaleX(optionsBorder, 1.2f, 0.1f).setRepeat(3);

        //Delay for animation
        this.Wait(buttonDelay, () => {
            optionsBorder.transform.localScale = Vector3.one;
        });
        LeanTween.scale(optionsScreen, Vector3.one, 0.2f);
    }

    public void CloseGame()
    {
        audioUI.PlayOneShot(buttonClick);
        LeanTween.scaleX(exitBorder, 1.2f, 0.1f).setRepeat(3);

        //Delay for animation
        this.Wait(buttonDelay, () => {
            exitBorder.transform.localScale = Vector3.one;
            Application.Quit();            
        });        
    }
}
