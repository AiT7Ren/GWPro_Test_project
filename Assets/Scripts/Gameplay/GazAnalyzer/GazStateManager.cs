using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
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
    private CancellationTokenSource _currentCts;
    private bool _isProcessing = false;
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
        
    }
    private async Task DelayPowerChangeAndResetAsync(GazAnalyzerState state)
    {
        _isActive = !_isActive;
        _currentCts?.Cancel();
        _currentCts = new CancellationTokenSource();
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(_duration),_currentCts.Token);
            _isProcessing = false;
            TryChangeState(state);
        }
        catch (TaskCanceledException)
        {
            _isProcessing = false;
        }
    }
    
    public void TryChangeState(GazAnalyzerState state)
    {
        if(_isProcessing)return;
        if (!state.IsPowerOn&&_isActive)
        {
            ChangeStateAndDelay(_onOffState, GazStateName.OFF_STATE,state);
            return;
        }
        if (state.IsPowerOn&&!_isActive)
        {
            ChangeStateAndDelay(_onOnState, GazStateName.ON_STATE,state);
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

    private void ChangeStateAndDelay(StringUnityEvent onOffState,string stateName,GazAnalyzerState state)
    {
        _isProcessing=true;
        InvokeState(onOffState, stateName);
        _ = DelayPowerChangeAndResetAsync(state);
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
        _currentCts?.Cancel();
        _currentCts?.Dispose();
        _currentCts = null;
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