using UnityEngine;

public class Aiming : MonoBehaviour
{
    private GameObject target;
    private Vector3 spread;
    public static float spreadPower = 1f;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        spread = new Vector3(Random.Range(-spreadPower, spreadPower),
            Random.Range(-spreadPower/2, spreadPower/2),
            Random.Range(-spreadPower, spreadPower));
        this.transform.LookAt(target.transform.position + spread);
    }
}
