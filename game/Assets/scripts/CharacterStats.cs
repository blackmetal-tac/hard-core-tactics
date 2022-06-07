using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public float HP {get; set;}

    public float shrinkTimer {get; set;}

    // Start is called before the first frame update
    void Start()
    {
        HP = 1f;       
    }

    // Update is called once per frame
    void Update()
    {
        //Death
        if (HP <= 0)
        {
            //this.transform.localScale = Vector3.zero;
        }
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;
    }
}
