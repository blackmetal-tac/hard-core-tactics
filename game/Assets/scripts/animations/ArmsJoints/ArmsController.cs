using UnityEngine;

public class ArmsController : MonoBehaviour
{
    // Joints    
    private Transform _shoulder, _elbow, _wrist;

    // Start is called before the first frame update
    void Start()
    {
        _shoulder = GetComponentInChildren<Shoulder>().transform;
        _elbow = GetComponentInChildren<Elbow>().transform;
        _wrist = GetComponentInChildren<Wrist>().transform;
    }

    
    public void Reset(float speed)
    {     
        RotateJoint(_shoulder, Quaternion.Euler(0, 5, 0), speed); 
        RotateJoint(_elbow, Quaternion.Euler(-80, -5, 0), speed); 
        RotateJoint(_wrist, Quaternion.Euler(50, 0, 0), speed);  
    }

    private void RotateJoint(Transform joint, Quaternion rotation, float speed)
    {
        joint.localRotation = Quaternion.Slerp(joint.localRotation, rotation, speed);
    }
}
