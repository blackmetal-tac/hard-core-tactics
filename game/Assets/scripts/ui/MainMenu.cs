using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string TestLevel;
    public AudioClip buttonClick;

    public static GameObject optionsScreen;
    private GameObject startButton, openOptions, exitButton;

    public static float buttonDelay = 0.4f;

    // Start is called before the first frame update
    void Start()
    {
        optionsScreen = GameObject.Find("MainOptions");
        startButton = GameObject.Find("StartButton");
        openOptions = GameObject.Find("OptionsButton");        
        exitButton = GameObject.Find("CloseButton");

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
        startButton.GetComponent<AudioSource>().PlayOneShot(buttonClick);
        LeanTween.scaleX(startButton.transform.GetChild(1).gameObject, 1.2f, 0.1f).setRepeat(3);

        //Delay for animation
        this.Wait(buttonDelay, ()=> {
            startButton.transform.GetChild(1).gameObject.transform.localScale = Vector3.one;
            SceneManager.LoadScene(TestLevel);
        });        
    }

    public void OpenMainOptions()
    {
        openOptions.GetComponent<AudioSource>().PlayOneShot(buttonClick);
        LeanTween.scaleX(openOptions.transform.GetChild(1).gameObject, 1.2f, 0.1f).setRepeat(3);

        //Delay for animation
        this.Wait(buttonDelay, () => {
            openOptions.transform.GetChild(1).gameObject.transform.localScale = Vector3.one;
        });
        LeanTween.scale(optionsScreen, Vector3.one, 0.2f);
    }

    public void CloseGame()
    {
        exitButton.GetComponent<AudioSource>().PlayOneShot(buttonClick);
        LeanTween.scaleX(exitButton.transform.GetChild(1).gameObject, 1.2f, 0.1f).setRepeat(3);

        //Delay for animation
        this.Wait(buttonDelay, () => {
            exitButton.transform.GetChild(1).gameObject.transform.localScale = Vector3.one;
            Application.Quit();            
        });        
    }
}
