using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScaleUI : MonoBehaviour
{
    public float scale = 1f;
    public float time = 1f;
    public float delay = 0f;

    // Start is called before the first frame update
    void Start()
    {
        this.Wait(delay * 0.2f, () => {
            transform.DOScale(Vector3.one * scale, time).SetEase(Ease.OutBack);
        });        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
