using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SensorHolder : MonoBehaviour
{
    [SerializeField] private Transform[] _localTransform;
    private bool _isActive;
    [field:SerializeField,Range(1f,360f)] float _rotateSpeed;
    public void TryPower(bool isActive)
    {
        _isActive = isActive;
    }
    private void FixedUpdate()
    {
        if (_isActive)
        {
            foreach (var local in _localTransform)
            {
                local.transform.Rotate(0f,_rotateSpeed * Time.fixedDeltaTime, 0, Space.Self);
            }
        }
    }
}
