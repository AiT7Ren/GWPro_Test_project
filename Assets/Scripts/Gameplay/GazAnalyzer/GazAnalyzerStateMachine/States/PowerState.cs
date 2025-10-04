using System.Collections;
using UnityEngine;
using Helpers;

public class PowerState : IGazState
{
    private readonly Material _material;
    private readonly Color _color;
    private Color _originalColor;
    private readonly float _duration;
    private readonly bool _isActive;
    private readonly float _emissionIntensity;
    private float _elapsedTime;
    public PowerState(Material material, Color color, float duration, bool on,float emissionIntensity=0f)
    {
        _material = material;
        _color = color;
        _duration = duration;
        _isActive = on;
        _emissionIntensity = emissionIntensity;
    }

    public void Enter()
    {
        _elapsedTime = 0f;
        _originalColor=_material.color;
    }

    public void Exit()
    {
        _material.color = _color;
        BackLight(_isActive);
    }
    public void Update(float deltaTime, Transform transform = null)
    {
        _elapsedTime += deltaTime;
        if (_elapsedTime >= _duration)
        {
            Exit();
            return;
        }
        _material.color = Color.Lerp(_originalColor, _color, _elapsedTime / _duration);
    }
    private void BackLight(bool active)
    {
        if (active)
        {
            _material.EnableKeyword("_EMISSION");
            _material.SetColor("_EmissionColor", _color*_emissionIntensity);
        }
        else
        {
            _material.DisableKeyword("_EMISSION");
            _material.SetColor("_EmissionColor", Color.black);
        }
    
    }
}
