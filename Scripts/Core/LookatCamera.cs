using UnityEngine;

public class LookatCamera : MonoBehaviour
{
    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
    }
}