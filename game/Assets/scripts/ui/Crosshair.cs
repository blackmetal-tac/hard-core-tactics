using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public float crosshairSize = 0.2f;
    public float animOffset = 0.02f;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.one * crosshairSize;
        LeanTween.reset();

        var leanSeq = LeanTween.sequence();

        leanSeq.insert( LeanTween.scale(gameObject, new Vector3(crosshairSize + animOffset, 
            crosshairSize + animOffset, crosshairSize + animOffset), 1f)
            .setEaseInOutCubic().setLoopPingPong());          
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
