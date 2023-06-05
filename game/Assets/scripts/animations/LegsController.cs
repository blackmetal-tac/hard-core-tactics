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
        
        // Set standing position
        SetRotation(LeftHip, -105);
        SetRotation(RightHip, -105);
        SetRotation(LeftKnee, 90);
        SetRotation(RightKnee, 90);
        SetRotation(LeftKnee2, -105);
        SetRotation(RightKnee2, -105);
        SetRotation(LeftFoot, 30);
        SetRotation(RightFoot, 30);
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    private void SetRotation(Transform joint, int rotation)
    {
        joint.localRotation = Quaternion.Euler(rotation, 0, 0);
    }
}
