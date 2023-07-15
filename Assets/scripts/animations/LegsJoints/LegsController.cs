using UnityEngine;

public class LegsController : MonoBehaviour
{
    // Joints    
    private Transform _hips, _leftHip, _rightHip, _leftKnee, _rightKnee, _leftKnee2, _rightKnee2, _leftFoot, _rightFoot;

    [SerializeField] private float legsHeight;

    // Start is called before the first frame update
    void Start()
    {       
        _hips = GetComponentInChildren<Hips>().transform;
        _leftHip = GetComponentInChildren<LeftHip>().transform;
        _rightHip = GetComponentInChildren<RightHip>().transform;
        _leftKnee = GetComponentInChildren<LeftKnee>().transform;
        _rightKnee = GetComponentInChildren<RightKnee>().transform;
        _leftKnee2 = GetComponentInChildren<LeftKnee2>().transform;
        _rightKnee2 = GetComponentInChildren<RightKnee2>().transform;
        _leftFoot = GetComponentInChildren<LeftFoot>().transform;
        _rightFoot = GetComponentInChildren<RightFoot>().transform;
    }

    public void Glide(bool left, float speed)
    {
        _hips.transform.localPosition = Vector3.Slerp(_hips.transform.localPosition, 
            new Vector3(_hips.transform.localPosition.x, _hips.transform.localPosition.y, legsHeight - 0.04f), speed); // z - 0.04

        if (!left)
        {
            RotateJoint(_hips.transform, Quaternion.Euler(0, 5, 0), speed);  // y + 5 = right turn
            RotateJoint(_leftHip, Quaternion.Euler(-15, 5, 0), speed); // y + 5
            RotateJoint(_rightHip, Quaternion.Euler(-15, -5, 0), speed); // y - 5
            RotateJoint(_leftKnee, Quaternion.Euler(100, 0, 0), speed); // x - 5
            RotateJoint(_rightKnee, Quaternion.Euler(125, 0, 0), speed); // x + 20
            RotateJoint(_leftKnee2, Quaternion.Euler(-115, 0, 0), speed); // x + 5
            RotateJoint(_rightKnee2, Quaternion.Euler(-140, 0, 0), speed); // x - 20
            RotateJoint(_leftFoot, Quaternion.Euler(30, -10, -10), speed); // y - 10 z - 10
            RotateJoint(_rightFoot, Quaternion.Euler(30, 0, 0), speed); // ---
        }
        else
        {
            RotateJoint(_hips.transform, Quaternion.Euler(0, -5, 0), speed);  
            RotateJoint(_leftHip, Quaternion.Euler(-15, 5, 0), speed); 
            RotateJoint(_rightHip, Quaternion.Euler(-15, -5, 0), speed); 
            RotateJoint(_leftKnee, Quaternion.Euler(125, 0, 0), speed); 
            RotateJoint(_rightKnee, Quaternion.Euler(100, 0, 0), speed); 
            RotateJoint(_leftKnee2, Quaternion.Euler(-140, 0, 0), speed); 
            RotateJoint(_rightKnee2, Quaternion.Euler(-115, 0, 0), speed);
            RotateJoint(_leftFoot, Quaternion.Euler(30, 0, 0), speed); 
            RotateJoint(_rightFoot, Quaternion.Euler(30, 10, 10), speed);       
        }

    }

    public void Reset(float speed)
    {
        _hips.transform.localPosition = Vector3.Slerp(_hips.transform.localPosition, 
            new Vector3(_hips.transform.localPosition.x, _hips.transform.localPosition.y, legsHeight), speed); 
        RotateJoint(_hips.transform, Quaternion.Euler(0, 0, 0), speed);  
        RotateJoint(_leftHip, Quaternion.Euler(-15, 0, 0), speed); 
        RotateJoint(_rightHip, Quaternion.Euler(-15, 0, 0), speed); 
        RotateJoint(_leftKnee, Quaternion.Euler(105, 0, 0), speed); 
        RotateJoint(_rightKnee, Quaternion.Euler(105, 0, 0), speed);
        RotateJoint(_leftKnee2, Quaternion.Euler(-120, 0, 0), speed); 
        RotateJoint(_rightKnee2, Quaternion.Euler(-120, 0, 0), speed); 
        RotateJoint(_leftFoot, Quaternion.Euler(30, 0, 0), speed); 
        RotateJoint(_rightFoot, Quaternion.Euler(30, 0, 0), speed); 
    }

    private void RotateJoint(Transform joint, Quaternion rotation, float speed)
    {
        joint.localRotation = Quaternion.Slerp(joint.localRotation, rotation, speed);
    }
}
