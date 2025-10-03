using System.Collections;
using UnityEngine;
using Helpers;

public class PowerState : IGazState
{
    CoroutineHolder _coroutineHolder;
    private Material _material;
    private Color _color;
    private float _duration;
    private bool _isActive;
    private float _emissionIntensity;
    private Coroutine _displayChange;
    public PowerState(Material material, Color color, float duration, bool on,float emissionIntensity=0f)
    {
        _material = material;
        _color = color;
        _duration = duration;
        _isActive = on;
        _emissionIntensity = emissionIntensity;
        _coroutineHolder=CoroutineHolder.Instance;
    }

    public void Enter()
    {
        _displayChange =_coroutineHolder.StartCoroutine(LerpColorOverTime(_material.color, _color, _duration));
    }

    public void Exit()
    {
        _coroutineHolder.StopCoroutine(_displayChange);
        _material.color = _color;
        BackLight(_isActive);
    }

    public void Update(float deltaTime, Transform transform = null)
    {

    }

    private IEnumerator LerpColorOverTime(Color from, Color to, float duration)
    {
        var elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            var t = elapsedTime / duration;
            t = Mathf.SmoothStep(0f, 1f, t);
            var currentColor = Color.Lerp(from, to, t);
            _material.color = currentColor;
            yield return null;
        }
        Exit();
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
