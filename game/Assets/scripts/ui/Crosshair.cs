using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.reset();
        LeanTween.scale(gameObject, Vector3.one * 0.13f, 1f).setEaseInOutCubic().setLoopPingPong();           
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
