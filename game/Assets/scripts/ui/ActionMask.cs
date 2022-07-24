using UnityEngine;

public class ActionMask : MonoBehaviour
{
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        this.Wait(gameManager.loadTime, () =>
        {
            transform.localScale = Vector3.zero;            
        });
    }
}
