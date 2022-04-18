using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string TestLevel;
    public AudioClip buttonClick;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        StartCoroutine(delayStartGame());                
    }

    public void OpenMainOptions()
    { 
    
    }

    public void CloseMainOptions()
    { 
    
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    IEnumerator delayStartGame()
    {
        GameObject.Find("StartButton").GetComponent<AudioSource>().PlayOneShot(buttonClick);
        LeanTween.scaleX(gameObject, 1.2f, 0.1f).setLoopOnce();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(TestLevel);
    }
}
