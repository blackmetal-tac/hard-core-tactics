using UnityEngine;
using DG.Tweening;

public class Crosshair : MonoBehaviour
{
    public float crosshairSize = 0.2f;
    public float animOffset = 0.02f;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.one * crosshairSize;
        transform.DOScale(Vector3.one * (crosshairSize + animOffset), 1f).SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
