using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string TestLevel;

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
        SceneManager.LoadScene(TestLevel);
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
}
