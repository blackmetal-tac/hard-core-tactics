using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string TestLevel;
    public AudioClip buttonClick;

    private GameObject optionsScreen;
    private GameObject closeOptions;

    // Start is called before the first frame update
    void Start()
    {
        optionsScreen = GameObject.Find("MainOptions");
        closeOptions = GameObject.Find("CloseOptions").transform.GetChild(1).gameObject;
        optionsScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        GameObject.Find("StartButton").GetComponent<AudioSource>().PlayOneShot(buttonClick);
        LeanTween.scaleX(GameObject.Find("StartButton").transform.GetChild(1).gameObject, 1.2f, 0.1f).setRepeat(5);

        this.Wait(0.6f, ()=> { 
            SceneManager.LoadScene(TestLevel);
            LeanTween.scaleX(GameObject.Find("StartButton").transform.GetChild(1).gameObject, 1f, 0.1f);
        });        
    }

    public void OpenMainOptions()
    {
        GameObject.Find("OptionsButton").GetComponent<AudioSource>().PlayOneShot(buttonClick);
        LeanTween.scaleX(GameObject.Find("OptionsButton").transform.GetChild(1).gameObject, 1.2f, 0.1f).setRepeat(5);        

        this.Wait(0.6f, () => {
            optionsScreen.SetActive(true);
            LeanTween.scaleX(GameObject.Find("OptionsButton").transform.GetChild(1).gameObject, 1f, 0.1f);            
        });        
    }

    public void CloseMainOptions()
    {
        closeOptions.GetComponent<AudioSource>().volume = 0.05f;
        closeOptions.GetComponent<AudioSource>().PlayOneShot(buttonClick);
        LeanTween.scaleX(closeOptions, 1.2f, 0.1f).setRepeat(5);

        this.Wait(0.6f, () => {
            optionsScreen.SetActive(false);
            LeanTween.scaleX(closeOptions, 1f, 0.1f);
        });
    }

    public void CloseGame()
    {
        GameObject.Find("CloseButton").GetComponent<AudioSource>().PlayOneShot(buttonClick);
        LeanTween.scaleX(GameObject.Find("CloseButton").transform.GetChild(1).gameObject, 1.2f, 0.1f).setRepeat(5);

        this.Wait(0.6f, () => { 
            Application.Quit();
            LeanTween.scaleX(GameObject.Find("CloseButton").transform.GetChild(1).gameObject, 1f, 0.1f);
        });        
    }
}
