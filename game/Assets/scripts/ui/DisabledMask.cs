using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisabledMask : MonoBehaviour
{
    private bool init = false;

    // Start is called before the first frame update
    void Start()
    {
        this.Wait(2, () => {
            init = true;
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.inAction || !init)
        {
            transform.localScale = Vector3.one;
        }
        else
        {
            transform.localScale = Vector3.zero;
        }
    }
}
