using UnityEngine;

public class ActionMask : MonoBehaviour
{
    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        this.Wait(_gameManager.LoadTime, () =>
        {
            transform.localScale = Vector3.zero;         
        });
    }
}
