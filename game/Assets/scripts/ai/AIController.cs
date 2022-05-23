using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public static float HP;

    // Start is called before the first frame update
    void Start()
    {
        HP = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {
            this.transform.localScale = Vector3.zero;
        }
    }
}
