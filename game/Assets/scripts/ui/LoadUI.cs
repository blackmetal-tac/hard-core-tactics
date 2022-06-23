using UnityEngine;
using UnityEngine.UI;

public class LoadUI : MonoBehaviour
{
    private Image loadingBar;
    private CanvasGroup canvasGroup;
    public float alpha = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        loadingBar = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (loadingBar.fillAmount < 1)
        {
            loadingBar.fillAmount += Time.deltaTime * 0.6f;            
        }
        if (canvasGroup.alpha > alpha)
        {
            canvasGroup.alpha -= Time.deltaTime * 0.5f;
        }
    }
}
