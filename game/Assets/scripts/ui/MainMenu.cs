using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    public string Level;
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

        audioUI = GameObject.Find("MainUI").GetComponent<AudioSource>();

        //Set options screen to 0 (invisible)
        optionsScreen.transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Buttons
    public void StartGame()
    {
        audioUI.PlayOneShot(buttonClick);
        BorderAnim(startBorder);

        //Delay for animation
        this.Wait(buttonDelay, ()=> {
            startBorder.transform.localScale = Vector3.one;
            SceneManager.LoadScene(Level);
        });        
    }

    public void OpenMainOptions()
    {
        audioUI.PlayOneShot(buttonClick);
        BorderAnim(optionsBorder);

        //Delay for animation
        this.Wait(buttonDelay, () => {
            optionsBorder.transform.localScale = Vector3.one;
        });
        optionsScreen.transform.DOScale(Vector3.one, 0.2f);
    }

    public void CloseGame()
    {
        audioUI.PlayOneShot(buttonClick);
        BorderAnim(exitBorder);

        //Delay for animation
        this.Wait(buttonDelay, () => {
            exitBorder.transform.localScale = Vector3.one;
            Application.Quit();            
        });        
    }

    public static void BorderAnim(GameObject gameObject)
    {
        gameObject.transform.DOScaleX(1.2f, 0.1f).SetLoops(3);
    }

    public static void ScaleDown(GameObject gameObject) 
    {
        gameObject.transform.DOScale(Vector3.zero, 0.2f);
    }
}
