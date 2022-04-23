using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string TestLevel;
    public AudioClip buttonClick;

    public static GameObject optionsScreen;
    private GameObject startButton;
    private GameObject openOptions;    
    private GameObject exitButton;
    private GameObject applyButton;

    public static float buttonDelay = 0.4f;

    // Start is called before the first frame update
    void Start()
    {
        optionsScreen = GameObject.Find("MainOptions");
        startButton = GameObject.Find("StartButton");
        openOptions = GameObject.Find("OptionsButton");        
        exitButton = GameObject.Find("CloseButton");
        applyButton = GameObject.Find("ApplyButton");

        //Set options screen to 0 (invisible)
        optionsScreen.transform.localScale = new Vector3(0, 0, 0);
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
            startButton.transform.GetChild(1).gameObject.transform.localScale = new Vector3(1, 1, 1);
            SceneManager.LoadScene(TestLevel);
        });        
    }

    public void OpenMainOptions()
    {
        openOptions.GetComponent<AudioSource>().PlayOneShot(buttonClick);
        LeanTween.scaleX(openOptions.transform.GetChild(1).gameObject, 1.2f, 0.1f).setRepeat(3);

        //Delay for animation
        this.Wait(buttonDelay, () => {
            openOptions.transform.GetChild(1).gameObject.transform.localScale = new Vector3(1, 1, 1);
        });
        LeanTween.scale(optionsScreen, new Vector3(1, 1, 1), 0.2f);
    }

    public void CloseGame()
    {
        exitButton.GetComponent<AudioSource>().PlayOneShot(buttonClick);
        LeanTween.scaleX(exitButton.transform.GetChild(1).gameObject, 1.2f, 0.1f).setRepeat(3);

        //Delay for animation
        this.Wait(buttonDelay, () => {
            exitButton.transform.GetChild(1).gameObject.transform.localScale = new Vector3(1, 1, 1);
            Application.Quit();            
        });        
    }
}
