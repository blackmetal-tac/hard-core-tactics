using UnityEngine;

public class Shield : MonoBehaviour
{
    public float HP;
    private float shrinkTimer;
    private Collider shieldCollider;
    [HideInInspector] public float scale;
    [HideInInspector] public Material material;

    void Start()
    {
        shieldCollider = GetComponent<Collider>();
        material = GetComponentInChildren<MeshRenderer>().sharedMaterial;
        shrinkTimer = transform.parent.GetComponentInParent<UnitManager>().shrinkTimer;
    }

    void FixedUpdate()
    {        
        transform.localScale = (0.9f + scale) * Vector3.one;
        material.SetFloat("_Alpha", 0.3f * HP);

        if (HP > 0)
        {
            shieldCollider.enabled = true;
        }
    }

    public void TakeDamage(float damage)
    {
        // Reset HP bar damage animation
        shrinkTimer = 0.5f;
        if (HP > 0)
        {
            HP -= damage;
        }

        // Shield down
        if (HP <= 0)
        {
            shieldCollider.enabled = false;
        }
    }
}
