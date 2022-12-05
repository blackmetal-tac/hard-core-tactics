using UnityEngine;

public class Shield : MonoBehaviour
{
    [HideInInspector] public float HP;
	[HideInInspector] public float Scale;
    [HideInInspector] public Material Material;
    [HideInInspector] public string ShieldID;
    private float _shrinkTimer;
    private Collider _shieldCollider;
	
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
        _shrinkTimer = 0.5f;
        if (HP > 0)
        {
            HP -= damage;
            Debug.Log("Took damage");
        }

        // Shield down
        if (HP <= 0)
        {
            _shieldCollider.enabled = false;
        }
    }
}
