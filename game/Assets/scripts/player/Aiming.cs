using UnityEngine;

public class Aiming : MonoBehaviour
{
    private GameManager gameManager;
    private UnitManager unitManager;
    private Vector3 spread;

    private float baseSpread = 0.5f;
    private float spreadMult = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        unitManager = GetComponentInParent<UnitManager>();
        transform.LookAt(unitManager.target.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.inAction) 
        {
            spread = new Vector3(
                Random.Range((-unitManager.moveSpeed * spreadMult) - baseSpread, (unitManager.moveSpeed * spreadMult) + baseSpread),
                Random.Range((-unitManager.moveSpeed * spreadMult) - baseSpread / 2, (unitManager.moveSpeed * spreadMult) + baseSpread / 2),
                Random.Range((-unitManager.moveSpeed * spreadMult) - baseSpread, (unitManager.moveSpeed * spreadMult) + baseSpread));
            transform.LookAt(unitManager.target.transform.position + spread);
        }        
    }
}
