using UnityEngine;
using UnityEngine.UI;

public class HoldIndicator : MonoBehaviour
{
    [Range(0, 1)] public float HoldPower;
    [SerializeField]private Image _holdIndicator;
    [SerializeField]private Transform _objectToHold;
    [SerializeField] private float _startAngle = 0f;
    [SerializeField] private float _endAngle = 360f;
    private float _currentAngle;
    [SerializeField] private bool _colorChange;
    [SerializeField] private Color _startColor;
    [SerializeField] private Color _endColor;
    [SerializeField] private Image[] _imageToColorCange;

    private void Start()
    {
        if (_imageToColorCange != null) return;
        var image = GetComponentsInChildren<Image>();
        _imageToColorCange = new Image[image.Length];
        _imageToColorCange = image;
        foreach (var colorIm in _imageToColorCange)
        {
            colorIm.color = _startColor;
        }
    }

    public void SetAmount(float amount)
    {
        _holdIndicator.fillAmount = amount;
        _currentAngle = Mathf.Lerp(_startAngle, _endAngle, amount);
        _objectToHold.localRotation = Quaternion.Euler(0f,0f, _currentAngle);
        if(!_colorChange)return;
        foreach (var colorIm in _imageToColorCange)
        {
            colorIm.color = Color.Lerp(_startColor, _endColor, amount);
        }
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
       SetAmount(HoldPower);
    }
#endif
}
