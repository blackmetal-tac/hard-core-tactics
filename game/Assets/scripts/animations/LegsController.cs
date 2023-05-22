using UnityEngine;

public class LegsController : MonoBehaviour
{
    // Joints    
    private Transform LeftHip, RightHip, LeftKnee, RightKnee, LeftKnee2, RightKnee2, LeftFoot, RightFoot;

    // Start is called before the first frame update
    void Start()
    {
        LeftHip = GetComponentInChildren<LeftHip>().transform;
        RightHip = GetComponentInChildren<RightHip>().transform;
        LeftKnee = GetComponentInChildren<LeftKnee>().transform;
        RightKnee = GetComponentInChildren<RightKnee>().transform;
        LeftKnee2 = GetComponentInChildren<LeftKnee2>().transform;
        RightKnee2 = GetComponentInChildren<RightKnee2>().transform;
        LeftFoot = GetComponentInChildren<LeftFoot>().transform;
        RightFoot = GetComponentInChildren<RightFoot>().transform;
        
        LeftHip.localRotation = Quaternion.Euler(165, 0, 0);
        RightHip.localRotation = Quaternion.Euler(165, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
}
