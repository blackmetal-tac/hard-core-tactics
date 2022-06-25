using UnityEngine;

public class DisabledMask : MonoBehaviour
{
    private GameManager gameManager;
    private bool loading;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        loading = true;
        this.Wait(gameManager.loadTime, () =>
        {
            loading = false;
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.inAction || loading)
        {
            transform.localScale = Vector3.one;
        }
        else
        {
            transform.localScale = Vector3.zero;
        }
    }
}
