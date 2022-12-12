using UnityEngine;
using System.Collections.Generic;

public class Shield : MonoBehaviour
{
    [HideInInspector] public float HP;
	[HideInInspector] public float Scale;
    [HideInInspector] public Material Material;
    [HideInInspector] public string ShieldID;
    [HideInInspector] public float ShrinkTimer;
    private Collider _shieldCollider;

    [System.Serializable]
    public class ShieldModes
    {
        public string ModeName;
        public int Regen;
    }
    public List<ShieldModes> weaponModes;
	
    void Start()
    {
        _shieldCollider = GetComponent<Collider>();
        Material = GetComponentInChildren<MeshRenderer>().sharedMaterial;
    }

    void FixedUpdate()
    {        
        transform.localScale = (0.9f + Scale) * Vector3.one;
        Material.SetFloat("_Alpha", 0.3f * HP);

        if (HP > 0)
        {
            _shieldCollider.enabled = true;
        }
    }

    public void TakeDamage(float damage)
    {
        // Reset HP bar damage animation
        ShrinkTimer = 0.5f;
        if (HP > 0)
        {
            HP -= damage;            
        }

        // Shield down
        if (HP <= 0)
        {
            _shieldCollider.enabled = false;
        }
    }
}
