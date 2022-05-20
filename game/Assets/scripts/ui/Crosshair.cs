using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localScale = Vector3.one * 0.2f;
        LeanTween.reset();
        LeanTween.scale(gameObject, new Vector3(0.22f,0.22f,0.22f), 1f).setEaseInOutCubic().setLoopPingPong();           
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
