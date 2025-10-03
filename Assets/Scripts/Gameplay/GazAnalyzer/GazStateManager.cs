using System;
using System.Collections;
using Helpers;
using UnityEngine;

public interface IGazStateManager
{
    void TryChangeState(GazAnalyzerState state);
}

public class GazStateManager : IGazStateManager,IDisposable
{
    private readonly IGazAnalyzStateMachine _stateMachine;
    private readonly StringUnityEvent _onOffState;
    private readonly StringUnityEvent _onOnState;
    private readonly StringUnityEvent _onDangerState;
    private readonly StringUnityEvent _onScaneState;
    private readonly StringUnityEvent _onLeakFindState;
    private readonly CoroutineHolder _coroutineHolder;
    private bool _isActive=false;
    private float _duration;
    private Coroutine _delayCoroutine;
    public GazStateManager( IGazAnalyzStateMachine stateMachine,
        StringUnityEvent onOffState, StringUnityEvent onOnState,
        StringUnityEvent onDangerState, StringUnityEvent onScaneState,
        StringUnityEvent onLeakFindState,float duration )
    {
        _stateMachine = stateMachine;
        _onOffState = onOffState;
        _onOnState = onOnState;
        _onDangerState = onDangerState;
        _onScaneState = onScaneState;
        _onLeakFindState = onLeakFindState;
        _duration=duration;
        _coroutineHolder=CoroutineHolder.Instance;
    }
    private IEnumerator DelayPowerChange(GazAnalyzerState gazState,bool isActive)
    {
        yield return new WaitForSeconds(_duration);
        _isActive=isActive;
        _delayCoroutine = null;
        TryChangeState(gazState);

    }
    public void TryChangeState(GazAnalyzerState state)
    {
        
        if (!state.IsPowerOn)
        {
            InvokeState(_onOffState, GazStateName.OFF_STATE);
            if(_delayCoroutine==null)_delayCoroutine = _coroutineHolder.StartCoroutine(DelayPowerChange(state,false));
            return;
        }
        if (state.IsPowerOn&&!_isActive)
        {
            InvokeState(_onOnState, GazStateName.ON_STATE);
            if(_delayCoroutine==null)_delayCoroutine = _coroutineHolder.StartCoroutine(DelayPowerChange(state,true));
            return;
        }
        if(!_isActive)return;
        if (state.IsInLeakDetection)
        {
            InvokeState(_onLeakFindState, GazStateName.LEAKDET_STATE);
            return;
        }
        if (state.IsInDanger)
        {
            InvokeState(_onDangerState, GazStateName.DANGER_STATE);
            return;
        }
        InvokeState(_onScaneState, GazStateName.SCANE_STATE);
    }

    private void InvokeState(StringUnityEvent eventMethod, string stateName)
    {
        if (eventMethod?.GetPersistentEventCount() > 0)
        {
            eventMethod.Invoke(stateName);
        }
        else
        {
            Debug.Log($"{stateName} used from code");
            _stateMachine.ChangeState(stateName);
        }
    }
    public void Dispose()
    {
        if(_delayCoroutine!=null)_coroutineHolder.StopCoroutine(_delayCoroutine);
        _delayCoroutine=null;
    }
}

public readonly struct GazAnalyzerState
{
    public bool IsPowerOn { get; }
    public bool IsInDanger { get; }
    public bool IsInLeakDetection { get; }

    public GazAnalyzerState(bool isPowerOn, bool isInDanger,
        bool isInLeakDetection)
    {
        IsPowerOn = isPowerOn;
        IsInDanger = isInDanger;
        IsInLeakDetection = isInLeakDetection;
    }
}