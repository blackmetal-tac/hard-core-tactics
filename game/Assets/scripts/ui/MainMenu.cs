using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    public string Level;
    private AudioSource audioUI;
    private AudioClip buttonClick;

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

        startBorder = startButton.transform.Find("ButtonBorder").gameObject;
        optionsBorder = openOptions.transform.Find("ButtonBorder").gameObject;
        exitBorder = exitButton.transform.Find("ButtonBorder").gameObject;

        audioUI = GameObject.Find("MainUI").GetComponent<AudioSource>();
        buttonClick = GameObject.Find("AudioManager").GetComponent<AudioSourcesUI>().clickButton;
    }

    //Buttons
    public void StartGame()
    {
        audioUI.PlayOneShot(buttonClick);
        BorderAnim(startBorder, 1.2f, 3);

        //Delay for animation
        this.Wait(buttonDelay, ()=> {
            BorderAnim(startBorder, 1f, 1);
            SceneManager.LoadScene(Level);
        });        
    }

    public void OpenMainOptions()
    {
        audioUI.PlayOneShot(buttonClick);
        BorderAnim(optionsBorder, 1.2f, 3);

        //Delay for animation
        this.Wait(buttonDelay, () => {
            BorderAnim(optionsBorder, 1f, 1);
        });
        optionsScreen.transform.DOScale(Vector3.one, buttonDelay).SetEase(Ease.OutBack);
    }

    public void CloseGame()
    {
        audioUI.PlayOneShot(buttonClick);
        BorderAnim(exitBorder, 1.2f, 3);

        //Delay for animation
        this.Wait(buttonDelay, () => {
            BorderAnim(exitBorder, 1f, 1);
            Application.Quit();            
        });        
    }

    public static void BorderAnim(GameObject gameObject, float scaleX, int loops)
    {
        gameObject.transform.DOScaleX(scaleX, 0.1f).SetLoops(loops);
    }

    public static void ScaleDown(GameObject gameObject) 
    {
        gameObject.transform.DOScale(Vector3.zero, 0.2f);
    }
}
