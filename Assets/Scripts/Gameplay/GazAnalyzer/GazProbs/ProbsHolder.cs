using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProbsHolder : MonoBehaviour
{
    //тут связь специально сделал с помощью UnityEvent
    public UnityEvent<bool> OnProbsOnActive;
    [SerializeField] private Transform[] _localTransform;
    [SerializeField] private HoldIndicator _powerImage; 
    private bool _isActive;
    [field:SerializeField,Range(1f,360f)] float _rotateSpeed;
    public void TryPower(float amount)
    {
        if(amount>=1f&&_powerImage.gameObject.activeSelf)SetActiveRotate();
        if(amount!=0&&amount<1f)_powerImage.gameObject.SetActive(true);
        else
        {
            _powerImage.gameObject.SetActive(false);
            return;
        }
        _powerImage.SetAmount(amount);
    }
    private void SetActiveRotate()
    {
        _isActive = !_isActive;
        OnProbsOnActive?.Invoke(_isActive);
        if (!_isActive) Deactivate();
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

    private void Deactivate()
    {
        foreach (var local in _localTransform)
        {
            local.transform.localRotation = Quaternion.identity;
        }
    }
}
